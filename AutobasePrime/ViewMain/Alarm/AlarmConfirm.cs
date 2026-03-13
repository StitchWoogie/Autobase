using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphicModule;
using AutoLibLocal;
using NetTools;
using NetTools.OldDefine;
using LocalMain.Alarm;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Drawing;

namespace ViewMain.Alarm
{
    public class AlarmConfirm
    {
        static AlarmConfirm()
        {
            //
            // TODO: Add constructor logic here
            //
            FormAlarmEvent.procConfirmAlarmSound = new GraphicModule.FormAlarmEvent.DelegateConfirmAlarmSound(ConfirmAlarmSound);
        }

        public static void AlarmContinueRegister(TagAiClass ai, ALARM_FILE_STRUCT alarm, bool flag)
        {
            if ((ConfigAlarm.dwAlarmFilterEvent & Tools.DWORD_MASK[alarm.alarm_type]) == 0) return;

            sbyte method = AlarmPriority.AlarmPriorityGetOptionScreen(alarm.priority);

            if (method == 0 || method == 1)
            { 	// 화면에 안 뿌리거나/한번만 뿌린다.
                return;
            }

            ai.bCurrentAlarmStatus = flag;		// 경보 상태를 등록하거나 삭제한다.

            ExecuteAlarmComfirmation(alarm, ai.tag, ai.sAlarmWaveFile, flag, method);
        }

        public static void AlarmContinueRegister(TagDiClass di, ALARM_FILE_STRUCT alarm, bool flag)
        {
            if ((ConfigAlarm.dwAlarmFilterEvent & Tools.DWORD_MASK[alarm.alarm_type]) == 0) return;

            sbyte method = AlarmPriority.AlarmPriorityGetOptionScreen(alarm.priority);

            if (method == 0 || method == 1)
            { 	// 화면에 안 뿌리거나/한번만 뿌린다.
                return;
            }

            di.bCurrentAlarmStatus = flag;	// 경보 상태를 등록하거나 삭제한다.

            ExecuteAlarmComfirmation(alarm, di.tag, di.sAlarmWaveFile, flag, method);
        }

        static void ExecuteAlarmComfirmation(ALARM_FILE_STRUCT alarm, string tag_name, string wave_file, bool flag, sbyte method)
        {
            if (flag == true)
            {
                InsertOneAlarmConfirmation(alarm, tag_name, wave_file, method);
                if (ConfigAlarm.bAlarmAutoMakeConfirmBox)
                {
                    CreatePopupAlarmConfirmation(true);
                }
            }
            else
            {
                DeleteOneAlarmConfirmation(alarm.tag, alarm.t);
            }
        }

        //------------------------------------------------------------------------------
        // 하나의 계속 경보 태그를 등록한다.
        //------------------------------------------------------------------------------

        static int InsertOneAlarmConfirmation(ALARM_FILE_STRUCT alarm, string tag_name, string wave_file, sbyte screen_method)
        {
            ALARM_CONFIRMATION_STRUCT block = new ALARM_CONFIRMATION_STRUCT();

            if (FormAlarmEvent.blockAlarmConfirmNot.Count >= 1000)
            {	// 확인만 하고 삭제를 하지 않으면 너무 많은 것이 쌓일 수 있다.
                FormAlarmEvent.blockAlarmConfirmNot.RemoveAt(0);
            }

            block.tag = alarm.tag;
            block.description = alarm.description;
            block.message = alarm.msg;

            //block.tag_type = tag_type;
            //block.tag_pos  = tag_pos;
            SYSTEMTIME.memcpy(block.t, alarm.t);
            block.sAlarmWaveFile = wave_file;
            block.screen_method = screen_method;
            block.bAlarm = 1;
            block.msg_type = alarm.alarm_type;
            block.priority = alarm.priority;
            block.port = alarm.port % 500;

            FormAlarmEvent.blockAlarmConfirmNot.Add(block);

            FormAlarmEvent.AlarmConfirmCountChanged();

            return 1;
        }

        public static bool IsPopupAlarmConfirmation()
        {
            return (FormPopupAlarmConfirmation.hwndPopupAlarmConfirmation != null);
        }

        //public static void CreatePopupAlarmConfirmation(bool flag)
        //{
        //    if (flag == false)
        //    {
        //        if (FormPopupAlarmConfirmation.hwndPopupAlarmConfirmation != null)
        //        {
        //            FormPopupAlarmConfirmation.hwndPopupAlarmConfirmation.Close();
        //            return;
        //        }
        //    }

        //    if (FormPopupAlarmConfirmation.hwndPopupAlarmConfirmation != null)
        //    {
        //        return;
        //    }

        //    FormPopupAlarmConfirmation form = new FormPopupAlarmConfirmation();
        //    form.Owner = TotalConfig.formMain;

        //    if (Tools.IsLangKorean())
        //    {
        //        form.Text = "경보 이벤트 창";
        //    }
        //    else if (Tools.IsLangJapanese())
        //        form.Text = "警報 イベント ウィンドウ";
        //    else if (Tools.IsLangChinese())
        //        form.Text = "警报事件窗口";
        //    else if (Tools.IsLangVietnamese())
        //        form.Text = "Sự kiện Báo động";
        //    else
        //    {
        //        form.Text = "Alarm Events Window";
        //    }


        //    int x = 0;// ConfigRunMain.rAlarmConfirmBox.left;
        //    int y = 0;// ConfigRunMain.rAlarmConfirmBox.top;
        //    int sizex = 600;// ConfigRunMain.rAlarmConfirmBox.right - ConfigRunMain.rAlarmConfirmBox.left;
        //    int sizey = 400;// ConfigRunMain.rAlarmConfirmBox.bottom - ConfigRunMain.rAlarmConfirmBox.top;
        //    int maxx, maxy;

        //    Screen screen = Screen.PrimaryScreen;

        //    maxx = screen.Bounds.Width;
        //    maxy = screen.Bounds.Height;

        //    if (x < 0) x = 0;
        //    if (y < 0) y = 0;
        //    if (x > maxx - 5) x = maxx - 100;
        //    if (y > maxy - 5) y = maxy - 100;

        //    form.StartPosition = FormStartPosition.Manual;
        //    form.Left = x;
        //    form.Top = y;
        //    form.Width = sizex;
        //    form.Height = sizey;

        //    form.Show();
        //}

        //20241010 PSU 다중모니터 사용 시 경보이벤트창 위치 수정
        public static void CreatePopupAlarmConfirmation(bool flag)
        {
            if (flag == false)
            {
                if (FormPopupAlarmConfirmation.hwndPopupAlarmConfirmation != null)
                {
                    FormPopupAlarmConfirmation.hwndPopupAlarmConfirmation.Close();
                    return;
                }
            }

            if (FormPopupAlarmConfirmation.hwndPopupAlarmConfirmation != null)
            {
                return;
            }

            FormPopupAlarmConfirmation form = new FormPopupAlarmConfirmation();
            form.Owner = TotalConfig.formMain;

            SetFormTitle(form);

            Screen targetScreen = FindAppropriateScreen(TotalConfig.formMain);
            Rectangle workingArea = targetScreen.WorkingArea;

            int width = 600;
            int height = 400;
            int x = workingArea.Left + (workingArea.Width - width) / 2;
            int y = workingArea.Top + (workingArea.Height - height) / 2;

            form.StartPosition = FormStartPosition.Manual;
            form.Location = new System.Drawing.Point(x, y);
            form.Size = new System.Drawing.Size(width, height);

            form.Show();
        }

        private static void SetFormTitle(FormPopupAlarmConfirmation form)
        {
            if (Tools.IsLangKorean())
            {
                form.Text = "경보 이벤트 창";
            }
            else if (Tools.IsLangJapanese())
                form.Text = "警報 イベント ウィンドウ";
            else if (Tools.IsLangChinese())
                form.Text = "警报事件窗口";
            else if (Tools.IsLangVietnamese())
                form.Text = "Sự kiện Báo động";
            else
            {
                form.Text = "Alarm Events Window";
            }
        }

        private static Screen FindAppropriateScreen(Form mainForm)
        {
            if (mainForm != null)
            {
                return Screen.FromControl(mainForm);
            }
            return Screen.PrimaryScreen;
        }

        //------------------------------------------------------------------------------
        //	하나의 계속 경보 태그를 삭제한다.
        //------------------------------------------------------------------------------

        static void DeleteOneAlarmConfirmation(string tag, SYSTEMTIME tReturn)
        {
            ALARM_CONFIRMATION_STRUCT block;

            int l;

            for (l = 0; l < FormAlarmEvent.blockAlarmConfirmNot.Count; l++)
            {
                block = (ALARM_CONFIRMATION_STRUCT)FormAlarmEvent.blockAlarmConfirmNot[l];
                if (block.tag == tag && block.bAlarm == 1)
                {	// already alarm register
                    if (block.screen_method != 3)	// 사용자 확인 때까지 계속 남아 있음
                        FormAlarmEvent.blockAlarmConfirmNot.RemoveAt(l);
                    else
                    {
                        block.bAlarm = 0;
                        SYSTEMTIME.memcpy(block.tReturn, tReturn);
                    }

                    FormAlarmEvent.AlarmConfirmCountChanged();
                    return;
                }
            }
        }

        public static void LoadAlarmConfirmList()
        {
            string filename;
            string cfg_dir;

            cfg_dir = Application.UserAppDataPath;

            filename = String.Format("{0}\\AlarmConfirmList.lst", cfg_dir);

            if (!File.Exists(filename)) return;

            Stream s = File.OpenRead(filename);
            IFormatter format = new BinaryFormatter();

            try
            {
                FormAlarmEvent.blockAlarmConfirmNot = (ArrayList)format.Deserialize(s);
            }
            catch (Exception exception)
            {
                ErrorMsg.Show(exception, filename, "Can't load the file");
            }

            s.Close();
        }

        public static void SaveAlarmConfirmList()
        {
            string filename;
            string cfg_dir;

            cfg_dir = Application.UserAppDataPath;

            filename = String.Format("{0}\\AlarmConfirmList.lst", cfg_dir);

            Stream s = File.OpenWrite(filename);
            IFormatter format = new BinaryFormatter();

            try
            {
                format.Serialize(s, FormAlarmEvent.blockAlarmConfirmNot);
            }
            catch (Exception exception)
            {
                ErrorMsg.Show(exception, filename, "Can't save the file");
            }

            s.Close();

        }

        static int currAlarmWavePosition = 0;

        public static void AlarmConfirmationStatus()
        {
            if (FormAlarmEvent.blockAlarmConfirmNot.Count == 0) return;

            ALARM_CONFIRMATION_STRUCT block;

            for (int l = 0; l < FormAlarmEvent.blockAlarmConfirmNot.Count; l++)
            {
                currAlarmWavePosition++;
                currAlarmWavePosition %= FormAlarmEvent.blockAlarmConfirmNot.Count;

                block = (ALARM_CONFIRMATION_STRUCT)FormAlarmEvent.blockAlarmConfirmNot[currAlarmWavePosition];

                if (block.bAlarm == 1)	// 현재 알람이 진행 중일때만 경보 소리를 울린다.
                {
                    int option = AlarmPriority.AlarmPriorityGetOptionSound(block.priority);

                    if ((option == 2 || option == 3) && block.bConfirmedAlarmSound == false)
                    {
                        // AlarmSound.Play(block.sAlarmWaveFile, block.priority);
                        return;	// 소리를 울렸으면 돌아간다.
                    }
                }
            }
        }

        public static void ConfirmAlarmSound()
        {
            ALARM_CONFIRMATION_STRUCT block;

            for (int i = 0; i < FormAlarmEvent.blockAlarmConfirmNot.Count; i++)
            {
                block = (ALARM_CONFIRMATION_STRUCT)FormAlarmEvent.blockAlarmConfirmNot[i];

                block.bConfirmedAlarmSound = true;
            }
        }



    }
}

