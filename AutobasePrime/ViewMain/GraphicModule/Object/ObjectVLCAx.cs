using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NetTools.OldDefine;
using System.Drawing;
using NetTools;
using AutoLibLocal;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.Linq;


namespace GraphicModule
{
    [Serializable]
    public class ObjectArgsVLCAx
    {
        public string mrl;
        public string options;
        public bool autoplay;
        public bool autoloop;
        public int volume;
    }

    [Serializable]
    public class ObjectVLCAx : ObjectExpand
    {
        ObjectArgsVLCAx objArgs;

        [NonSerialized]
        private VideoView videoView;

        [NonSerialized]
        private LibVLC libVLC;

        [NonSerialized]
        private MediaPlayer mediaPlayer;

        //[NonSerialized]
        //private MediaList mediaList;

        //private MediaListPlayer _mediaListPlayer; //현재 버전에서 제거되어 따로 playlist 기능 구현

        [NonSerialized]
        private List<MediaItem> playlist = new List<MediaItem>();

        [NonSerialized]
        private int currentPlaylistIndex = -1;

        // 플레이리스트 아이템 클래스
        private class MediaItem
        {
            public string Mrl { get; set; }
            public string Options { get; set; }

            public MediaItem(string mrl, string options)
            {
                Mrl = mrl;
                Options = options ?? string.Empty;
            }
        }

        [NonSerialized]
        private Media currentMedia; // 현재 재생 중인 미디어 참조 보관

        [NonSerialized]
        private bool isPlaying = false; // 재생 상태 추적


        [NonSerialized]
        Form formParent; // 20250312 PSU

        [NonSerialized]
        private bool isInitialized = false;

        [NonSerialized]
        private bool isAutoLoopEnabled = false;

        [NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();


        public ObjectArgsVLCAx ObjectArgs
        {
            set
            {
                objArgs = value;
            }
            get
            {
                return objArgs;
            }
        }

        public ObjectVLCAx(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsVLCAx args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.VLCAx;
            bSupportObjectOnCE = false;
            objArgs = args;
            formParent = form; //20250312 PSU

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                try
                {
                    InitializeLibVLC();
                    CreateVideoView(form);
                    SetupVideoViewProperties();

                    arrayClassList.Add(this);
                    _ = base.SetToolTipOnChildWindow(videoView);
                    OnVisible(ExpandCalcVisible());

                    // Form이 표시된 후 미디어 설정
                    form.Shown += OnFormShown;
                }
                catch (Exception ex)
                {
                    CleanupResources();
                    string message = "VLC Media Player object Load Failed. " + ex.Message;
                    MessageDisplay.Show(message);
                }
            }
        }


        private void InitializeLibVLC()
        {
            // LibVLC 초기화 (한 번만 수행)
            try
            {
                LibVLCSharp.Shared.Core.Initialize();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("LibVLC initializing Failed: " + ex.Message);
            }

            // LibVLC 인스턴스 생성
            var options = new List<string>();
            if (!string.IsNullOrEmpty(objArgs.options))
            {
                options.AddRange(objArgs.options.Split(' '));
            }

            libVLC = new LibVLC(options.ToArray());
            mediaPlayer = new MediaPlayer(libVLC);

            //mediaPlayer.Volume = objArgs.volume;

            // 이벤트 등록
            mediaPlayer.EndReached += OnMediaEndReached;
            mediaPlayer.Playing += OnMediaPlaying;
            mediaPlayer.Stopped += OnMediaStopped;
            mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
        }

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            HandlePlaylistNext();
        }

        private void OnMediaPlaying(object sender, EventArgs e)
        {
            isPlaying = true;

            Debug.WriteLine($"재생 시작됨 - 인덱스: {currentPlaylistIndex}");
        }

        private void OnMediaStopped(object sender, EventArgs e)
        {
            isPlaying = false;
            Debug.WriteLine("재생 정지됨");
        }



        private void OnMediaEndReached(object sender, EventArgs e)
        {
            try
            {
                // UI 스레드에서 실행
                if (formParent?.InvokeRequired == true)
                {
                    formParent.BeginInvoke(new Action(() => HandlePlaylistNext()));
                }
                else
                {
                    HandlePlaylistNext();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"플레이리스트 처리 중 오류: {ex.Message}");
            }
        }

        private void SetupVideoViewProperties()
        {
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            GetViewZone(ref x1, ref y1, ref x2, ref y2);
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            videoView.Left = x1;
            videoView.Top = y1;
            videoView.Width = x2 - x1;
            videoView.Height = y2 - y1;
            videoView.Show();

            isInitialized = true;
        }
        private void CreateVideoView(Form form)
        {
            videoView = new VideoView();
            videoView.Name = objGeneral.sClassName;

            form.Controls.Add(videoView);
            videoView.MediaPlayer = mediaPlayer;
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            try
            {
                if (mediaPlayer != null && isInitialized)
                {

                    if (!string.IsNullOrEmpty(objArgs.mrl))
                    {
                        // 초기 MRL을 플레이리스트에 추가
                        AddToPlaylist(objArgs.mrl, objArgs.options);
                    }

                    // 자동 재생 설정
                    if (objArgs.autoplay)
                    {
                        PlayPlaylist();
                    }

                    // 자동 반복 설정
                    if (objArgs.autoloop)
                    {
                        SetAutoLoop(true);
                    }   

                    // 볼륨 설정 audio는 프로세스 단위로 동시에 적용된다. 여러 개의 vlc를 한 form에서 사용할 때 audio를 끄고싶은 영상은 옵션에서 no audio 옵션 사용.
                    mediaPlayer.Volume = objArgs.volume;
                }
            }
            catch (Exception ex)
            {
                if(Tools.IsLangKorean())
                    MessageDisplay.Show("미디어 설정 중 오류가 발생했습니다: " + ex.Message);
                else
                    MessageDisplay.Show("An error occurred while setting up media: " + ex.Message);
            }
        }

        private void AddToPlaylist(string mrl, string options = null)
        {
            if (string.IsNullOrEmpty(mrl)) return;

            try
            {
                var mediaItem = new MediaItem(mrl, options);
                playlist.Add(mediaItem);
                Debug.WriteLine($"플레이리스트에 추가됨: {mrl} (총 {playlist.Count}개)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"플레이리스트 추가 중 오류: {ex.Message}");
                throw; // 또는 적절한 처리
            }
        }

        private void RemoveFromPlaylist(int index)
        {
            if (index >= 0 && index < playlist.Count)
            {
                playlist.RemoveAt(index);

                // 현재 재생 중인 인덱스 조정
                if (currentPlaylistIndex > index)
                {
                    currentPlaylistIndex--;
                }
                else if (currentPlaylistIndex == index)
                {
                    // 현재 재생 중인 항목이 제거된 경우
                    if (currentPlaylistIndex >= playlist.Count)
                    {
                        currentPlaylistIndex = playlist.Count - 1;
                    }
                }

                Debug.WriteLine($"플레이리스트에서 제거됨 (인덱스: {index}, 남은 항목: {playlist.Count}개)");
            }
        }



        private void PlayCurrentMedia()
        {
            if (currentPlaylistIndex < 0 || currentPlaylistIndex >= playlist.Count || mediaPlayer == null)
                return;

            try
            {
                // 기존 미디어 정리
                if (currentMedia != null)
                {
                    currentMedia.Dispose();
                    currentMedia = null;
                }


                var currentItem = playlist[currentPlaylistIndex];
                Debug.WriteLine($"재생 중: {currentItem.Mrl} (인덱스: {currentPlaylistIndex})");

                // RTSP 스트림을 위한 미디어 옵션 설정
                var mediaOptions = new List<string>();

                // 아이템별 옵션 처리
                if (!string.IsNullOrEmpty(currentItem.Options))
                {
                    var options = currentItem.Options.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var option in options)
                    {
                        var trimmedOption = option.Trim();
                        if (trimmedOption.StartsWith(":"))
                        {
                            mediaOptions.Add(trimmedOption);
                        }
                    }
                }

                // RTSP 스트림의 경우 항상 TCP 사용 및 최적화 설정
                if (currentItem.Mrl.StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase))
                {
                    // rtsp-tcp가 이미 옵션에 없으면 추가
                    bool hasTcpOption = mediaOptions.Any(opt => opt.Contains("rtsp-tcp"));
                    if (!hasTcpOption)
                    {
                        mediaOptions.Add(":rtsp-tcp");
                    }

                    // RTSP 전용 하드웨어 디코딩 비활성화 (RTSP 스트림에서만)
                    bool hasHwOption = mediaOptions.Any(opt => opt.Contains("avcodec-hw"));
                    if (!hasHwOption)
                    {
                        mediaOptions.Add(":avcodec-hw=none");
                    }
                }


                // 새 미디어 생성 및 보관
                currentMedia = new Media(libVLC, currentItem.Mrl, FromType.FromLocation);

                // 미디어 옵션 적용
                foreach (var option in mediaOptions)
                {
                    currentMedia.AddOption(option);
                }

                mediaPlayer.Play(currentMedia);

                mediaPlayer.Volume = objArgs.volume;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"미디어 재생 중 오류: {ex.Message}");
                MessageDisplay.Show($"미디어 재생 중 오류가 발생했습니다: {ex.Message}");

                // 재생 실패시 다음 곡으로 넘어가기 (무한 루프 방지를 위해 조건부)
                if (playlist.Count > 1 && currentPlaylistIndex < playlist.Count - 1)
                {
                    HandlePlaylistNext();
                }
            }
        }

        private void PlayPlaylist()
        {
            if (playlist.Count == 0)
            {
                Debug.WriteLine("플레이리스트가 비어있습니다.");
                return;
            }
            // 첫 번째 아이템부터 재생
            currentPlaylistIndex = 0;
            PlayCurrentMedia();

            Debug.WriteLine($"플레이리스트 재생 시작 (총 {playlist.Count}개, AutoLoop: {isAutoLoopEnabled})");
        }

        private void StopPlaylist()
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                currentPlaylistIndex = -1;
                Debug.WriteLine("플레이리스트 정지됨");
            }
        }

        private void ClearPlaylist()
        {
            StopPlaylist();
            playlist.Clear();
            currentPlaylistIndex = -1;
            Debug.WriteLine("플레이리스트 클리어됨");
        }



        private void PlayNextInPlaylist()
        {
            if (playlist.Count == 0) return;

            if (currentPlaylistIndex < playlist.Count - 1)
            {
                currentPlaylistIndex++;
                PlayCurrentMedia();
            }
            else if (isAutoLoopEnabled)
            {
                currentPlaylistIndex = 0;
                PlayCurrentMedia();
            }
        }

        private void PlayPreviousInPlaylist()
        {
            if (playlist.Count == 0) return;

            if (currentPlaylistIndex > 0)
            {
                currentPlaylistIndex--;
                PlayCurrentMedia();
            }
            else if (isAutoLoopEnabled)
            {
                currentPlaylistIndex = playlist.Count - 1;
                PlayCurrentMedia();
            }
        }


        private void SetAutoLoop(bool enable)
        {
            if (mediaPlayer == null) return;

            isAutoLoopEnabled = enable;
            Debug.WriteLine($"AutoLoop 설정: {(enable ? "활성화" : "비활성화")} - 플레이리스트 순환 재생");
        }


        // 플레이리스트 다음 곡 또는 순환 재생 처리
        private void HandlePlaylistNext()
        {
            if (playlist.Count == 0) return;

            // 다음 곡으로 이동
            currentPlaylistIndex++;

            // 플레이리스트 끝에 도달한 경우
            if (currentPlaylistIndex >= playlist.Count)
            {
                if (isAutoLoopEnabled)
                {
                    // autoloop 활성화시 처음부터 다시 재생
                    currentPlaylistIndex = 0;
                    Debug.WriteLine("플레이리스트 순환: 처음부터 다시 재생");
                }
                else
                {
                    // autoloop 비활성화시 재생 종료
                    currentPlaylistIndex = -1;
                    Debug.WriteLine("플레이리스트 재생 완료");
                    return;
                }
            }

            // 현재 인덱스의 미디어 재생
            PlayCurrentMedia();
        }

  
        #region Close & CleanUpResources

        public override void Close()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Remove(this);
                CleanupResources();
            }
        }

        private void CleanupResources()
        {
            try
            {
                if (formParent != null)
                {
                    formParent.Shown -= OnFormShown;
                }

                if (videoView != null)
                {
                    if (formParent?.Controls.Contains(videoView) == true)
                    {
                        formParent.Controls.Remove(videoView);
                    }
                    videoView.Dispose();
                    videoView = null;
                }

                if (mediaPlayer != null)
                {
                    // 이벤트 해제
                    mediaPlayer.EndReached -= OnMediaEndReached;
                    mediaPlayer.Playing -= OnMediaPlaying;
                    mediaPlayer.Stopped -= OnMediaStopped;

                    mediaPlayer.Stop();
                    mediaPlayer.Dispose();
                    mediaPlayer = null;
                }

                // 현재 미디어 정리
                if (currentMedia != null)
                {
                    currentMedia.Dispose();
                    currentMedia = null;
                }

                if (libVLC != null)
                {
                    libVLC.Dispose();
                    libVLC = null;
                }

                playlist.Clear();
                currentPlaylistIndex = -1;
                isAutoLoopEnabled = false;
                isInitialized = false;
                isPlaying = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"VLC CleanupResources error: {ex.Message}");
            }
        }
        #endregion

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                Font font = MakeFont();

                int cyChar = (int)font.GetHeight() + 1;
                int cxChar = (int)(font.GetHeight() / 2);

                RECT r = new RECT();

                DrawClass.PopBox2(g, x1, y1, x2, y2, Color.LightGray);

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                //format.FormatFlags |= StringFormatFlags.NoWrap;
                Brush brush = Brushes.Black;

                r.left = x1;
                r.top = y1;
                r.right = x2;
                r.bottom = y2;

                //DrawClass.DrawText(g, this.objArgs.mrl, font, brush, r, format);

                string str = objGeneral.sClassName;
                DrawClass.DrawText(g, str, font, brush, r, format);
            }

            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {

                UpdateControlState(videoView, formParent);
            }
        }


        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (videoView != null)
                {
                    Font font = MakeFont();
                    videoView.Font = font;
                    videoView.Left = x1;
                    videoView.Top = y1;
                    videoView.Width = x2 - x1;
                    videoView.Height = y2 - y1;
                }
            }
        }

        private void SafeInvoke(Action action)
        {
            if (formParent?.InvokeRequired == true)
            {
                formParent.Invoke(action); // BeginInvoke 대신 Invoke 사용 (동기화 필요)
            }
            else
            {
                action();
            }
        }

        private T SafeInvoke<T>(Func<T> func)
        {
            if (formParent?.InvokeRequired == true)
            {
                return (T)formParent.Invoke(func);
            }
            else
            {
                return func();
            }
        }

        public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            await Task.CompletedTask; // 경고 해결용

            if (mediaPlayer == null) return 0;

            try
            {
                return SafeInvoke(() =>
                {
                    switch (command)
                    {
                        case "VLCPlaylistAdd":
                            if (args.Length >= 2)
                            {
                                string mrl = args[1] as string;
                                string options = args.Length >= 3 ? args[2] as string : null;

                                if (!string.IsNullOrEmpty(mrl))
                                {
                                    AddToPlaylist(mrl, options);
                                }
                            }
                            return 1;

                        case "VLCPlaylistRemove":
                            if (args.Length >= 2 && args[1] is int removeIndex)
                            {
                                RemoveFromPlaylist(removeIndex);
                            }
                            return 1;

                        case "VLCPlaylistClear":
                            ClearPlaylist();
                            return 1;

                        case "VLCPause":
                            if (mediaPlayer.CanPause)
                            {
                                mediaPlayer.Pause();
                            }
                            return 1;

                        case "VLCResume":
                            if (mediaPlayer.State == VLCState.Paused)
                            {
                                mediaPlayer.Play();
                            }
                            return 1;

                        case "VLCPlaylistPlay":
                            PlayPlaylist();
                            return 1;

                        case "VLCPlaylistStop":
                            StopPlaylist();
                            return 1;

                        case "VLCPlaylistNext":
                            PlayNextInPlaylist();
                            return 1;

                        case "VLCPlaylistPrevious":
                            PlayPreviousInPlaylist();
                            return 1;

                        case "VLCAutoLoop":
                            if (args.Length >= 2 && args[1] is int loopValue)
                            {
                                SetAutoLoop(loopValue != 0);
                            }
                            return 1;



                        case "VLCAudioMute":
                            if (args.Length >= 2 && args[1] is int muteValue)
                            {
                                mediaPlayer.Mute = muteValue != 0;
                            }
                            return 1;

                        case "VLCAudioVolume":
                            if (args.Length >= 2 && args[1] is int volume)
                            {
                                objArgs.volume = Math.Max(0, Math.Min(100, volume));
                                mediaPlayer.Volume = Math.Max(0, Math.Min(100, volume));
                            }
                            return 1;



                        default:
                            return 0;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Vlc Script Error ({command}): {ex.Message}");
                return 0;
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            ObjectSaveFont(writer);

            writer.Write("\tStringOption,");
            writer.Write("{0},", objArgs.mrl);
            writer.Write("{0},", objArgs.options);
            writer.Write("{0},", objArgs.autoplay);
            writer.Write("{0},", objArgs.autoloop);
            writer.Write("{0},", objArgs.volume);
            writer.WriteLine();
        }

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                this.videoView.Visible = flag;
            }
        }
    }
}
