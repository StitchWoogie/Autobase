using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using GraphicModule;
using NetTools;
using NetTools.OldDefine;
using Svg;
using Svg.Transforms;
using System.Collections.Generic;
using System.Threading.Tasks;

//SVG 요소 추가 20241024 PSU
namespace GraphicModule
{
    [Serializable]
    public class ObjectArgsSVG
    {
        public string sSvgFile;
        public int nOverlayMethod;  //사용안하지만 유지.
        public int nRotateFlip;
    }

    [Serializable]
    public class ObjectSVG : ObjectExpand
    {
        ObjectArgsSVG objArgs;
        int nWidth = 0;
        int nHeight = 0;

        [NonSerialized]
        SvgDocument svgDocument;


        [NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        [NonSerialized]
        System.Windows.Forms.Form formParent;

        [NonSerialized]
        private Bitmap cachedBitmap;
        [NonSerialized]
        private Size cachedSize;
        [NonSerialized]
        private int cachedRotateFlip;

        public ObjectArgsSVG ObjectArgs
        {
            set
            {
                objArgs = value;
                this.SetFileName(value.sSvgFile);
            }
            get
            {
                return objArgs;
            }
        }

        public ObjectSVG(ObjectCommonProperty ocp, System.Windows.Forms.Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, ObjectArgsSVG args)
            : base(ocp, rect, eid, null, general)
        {
            enumObjectType = EnumObjectType.SVG;
            bSupportObjectOnCE = false;
            objArgs = args;
            formParent = form;

            SetFileName(args.sSvgFile);

            if (nRight == -9999 && nBottom == -9999)
            {
                nRight = nLeft + nWidth - 1;
                nBottom = nTop + nHeight - 1;
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);
            }
        }

        public override void Close()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Remove(this);
            }
        }

        private string sErrorString;

        void SetErrorString(string err)
        {
            sErrorString = err;
        }

        private int errorState = 0; // 새로운 필드: 0은 정상, 1은 에러 상태

        private float aspectRatio = 1.0f;  // 종횡비 저장
        private float originalWidth;  // 원본 SVG 너비
        private float originalHeight; // 원본 SVG 높이

        void SetFileName(string filename)
        {
            DisposeCachedBitmap();

            string path = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(objCommonProperty, filename);

            if (!File.Exists(path))
            {
                SetErrorString(String.Format("File not found ({0})", path));
                svgDocument = null;
                errorState = 1; // 에러 상태 설정
                return;
            }

            // 파일 크기 확인
            long fileSizeInBytes = new FileInfo(path).Length;
            long fileSizeInMB = fileSizeInBytes / (1024 * 1024);

            if (fileSizeInMB >= 1)
            {
                // 경고 메시지 설정
                string warningMessage = String.Format("Warning: The SVG file size is {0}MB, which exceeds the recommended limit of 1MB.", fileSizeInMB);
                SetErrorString(warningMessage);
                svgDocument = null;
                nWidth = 500;
                nHeight = 20;
                errorState = 1; // 에러 상태 설정
                return;

            }

            try
            {
                if (objCommonProperty.bLoadByZipStream)
                {
                    using (MemoryStream stream = ObjectAnimation.RestoreFromZipStream(objCommonProperty.streamZip, filename))
                    {
                        svgDocument = SvgDocument.Open<SvgDocument>(stream);
                    }
                }
                else
                {
                    svgDocument = SvgDocument.Open(path);
                    errorState = 0;
                }

                // 원본 SVG 크기 정보 추출
                if (svgDocument.ViewBox != null && svgDocument.ViewBox.Width > 0 && svgDocument.ViewBox.Height > 0)
                {
                    originalWidth = svgDocument.ViewBox.Width;
                    originalHeight = svgDocument.ViewBox.Height;
                }
                else if (svgDocument.Width != null && svgDocument.Height != null)
                {
                    originalWidth = svgDocument.Width.Value;
                    originalHeight = svgDocument.Height.Value;
                }


                // viewBox가 없는 경우 생성
                if (svgDocument.ViewBox == null || svgDocument.ViewBox.Width == 0)
                {
                    svgDocument.ViewBox = new SvgViewBox(0, 0, originalWidth, originalHeight);
                }

                svgDocument.AspectRatio = new SvgAspectRatio
                {
                    Align = SvgPreserveAspectRatio.xMidYMid,
                    Slice = false
                };


                // 처음 로드할 때만 크기 설정
                if (nWidth == 0 || nHeight == 0)
                {
                    nWidth = (int)originalWidth;
                    nHeight = (int)originalHeight;
                }

                // 종횡비 저장
                aspectRatio = originalWidth / originalHeight;

            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("Error loading SVG: {0}", ex.Message));
                svgDocument = null;
            }

        }

        // svgDocument 객체를 사용하기 전에 필요시 다시 로드하는 방식
        private void LoadSvgDocumentIfNeeded()
        {
            if (svgDocument == null && !string.IsNullOrEmpty(objArgs.sSvgFile) && errorState == 0)
            {
                SetFileName(objArgs.sSvgFile);
            }
        }


        private void CreateOrUpdateCache(int swidth, int sheight)
        {
            const int THRESHOLD = 5;

            if (cachedBitmap != null &&
                Math.Abs(cachedSize.Width - swidth) < THRESHOLD &&
                Math.Abs(cachedSize.Height - sheight) < THRESHOLD &&
                cachedRotateFlip == objArgs.nRotateFlip)
            {
                return;
            }

            if (svgDocument == null)
            {
                DisposeCachedBitmap();
                SetErrorString("Invalid SVG document");
                return;
            }

            if (swidth <= 0 || sheight <= 0)
            {
                DisposeCachedBitmap();
                SetErrorString(string.Format("Invalid dimensions: Width={0}, Height={1}", swidth, sheight));
                return;
            }

            DisposeCachedBitmap();

            try
            {
                cachedBitmap = new Bitmap(swidth, sheight);
                using (Graphics g = Graphics.FromImage(cachedBitmap))
                {
                    // 스케일 계산
                    float scaleX = (float)swidth / svgDocument.ViewBox.Width;
                    float scaleY = (float)sheight / svgDocument.ViewBox.Height;

                    // Matrix를 사용하여 스케일링 적용
                    g.ScaleTransform(scaleX, scaleY);

                    // 원본 크기로 렌더링
                    svgDocument.Draw(g);
                }

                if (objArgs.nRotateFlip != 0)
                {
                    cachedBitmap.RotateFlip((RotateFlipType)objArgs.nRotateFlip);
                }

                cachedSize = new Size(swidth, sheight);
                cachedRotateFlip = objArgs.nRotateFlip;
            }
            catch (Exception ex)
            {
                DisposeCachedBitmap();
                svgDocument = null;
                SetErrorString(string.Format("Error creating bitmap: {0}", ex.Message));
            }
        }

        //public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        //{
        //    LoadSvgDocumentIfNeeded();

        //    if (errorState == 1 || svgDocument == null)
        //    {
        //        SafeException.SafeDrawString(g, sErrorString, new Font("Gulim", 9), Brushes.Black, x1, y1);
        //        return;
        //    }

        //    if (x1 > x2) Tools.Temp(ref x1, ref x2);
        //    if (y1 > y2) Tools.Temp(ref y1, ref y2);

        //    int swidth = x2 - x1 + 1;
        //    int sheight = y2 - y1 + 1;

        //    RotateFlipType rotateFlipType = (RotateFlipType)objArgs.nRotateFlip;

        //    if (rotateFlipType == RotateFlipType.Rotate90FlipNone ||
        //        rotateFlipType == RotateFlipType.Rotate270FlipNone ||
        //        rotateFlipType == RotateFlipType.Rotate90FlipX ||
        //        rotateFlipType == RotateFlipType.Rotate90FlipY)
        //    {
        //        int temp = swidth;
        //        swidth = sheight;
        //        sheight = temp;
        //    }

        //    CreateOrUpdateCache(swidth, sheight);

        //    if (cachedBitmap != null)
        //    {
        //        g.DrawImage(cachedBitmap, x1, y1, cachedBitmap.Width, cachedBitmap.Height);
        //    }
        //    else
        //    {
        //        SafeException.SafeDrawString(g, sErrorString, new Font("Gulim", 9), Brushes.Black, x1, y1);
        //    }
        //}


        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            LoadSvgDocumentIfNeeded();

            if (errorState == 1 || svgDocument == null)
            {
                SafeException.SafeDrawString(g, sErrorString, new Font("Gulim", 9), Brushes.Black, x1, y1);
                return;
            }

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            int targetWidth = x2 - x1 + 1;
            int targetHeight = y2 - y1 + 1;

            RotateFlipType rotateFlipType = (RotateFlipType)objArgs.nRotateFlip;

            // 회전된 상태에 따라 캐시 비트맵의 크기 결정
            int cacheWidth = targetWidth;
            int cacheHeight = targetHeight;

            if (rotateFlipType == RotateFlipType.Rotate90FlipNone ||
                rotateFlipType == RotateFlipType.Rotate270FlipNone ||
                rotateFlipType == RotateFlipType.Rotate90FlipX ||
                rotateFlipType == RotateFlipType.Rotate90FlipY)
            {
                cacheWidth = targetHeight;
                cacheHeight = targetWidth;
            }

            // 캐시 업데이트 또는 생성
            CreateOrUpdateCache(cacheWidth, cacheHeight);

            if (cachedBitmap != null)
            {
                // 타겟 크기로 스케일링하여 그리기
                Rectangle destRect = new Rectangle(x1, y1, targetWidth, targetHeight);
                Rectangle srcRect = new Rectangle(0, 0, cachedBitmap.Width, cachedBitmap.Height);


                //고품질 보간, 스케일차이가 미미해 불필요.           
                //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.DrawImage(cachedBitmap, destRect, srcRect, GraphicsUnit.Pixel);
            }
            else
            {
                SafeException.SafeDrawString(g, sErrorString, new Font("Gulim", 9), Brushes.Black, x1, y1);
            }
        }

        private void DisposeCachedBitmap()
        {
            if (cachedBitmap != null)
            {
                cachedBitmap.Dispose();
                cachedBitmap = null;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            svgDocument = null;
            DisposeCachedBitmap();
        }


        public void SetOriginalSize()
        {
            objArgs.nRotateFlip = 0;

            if (nLeft > nRight) Tools.Temp(ref nLeft, ref nRight);
            if (nTop > nBottom) Tools.Temp(ref nTop, ref nBottom);

            nRight = nLeft + nWidth - 1;
            nBottom = nTop + nHeight - 1;

            ExpandCalcObjectRect();
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.FileName(writer, objArgs.sSvgFile);
            SaveObjectItem.OverlayMethod(writer, 0, objArgs.nRotateFlip);
        }

        public override void GetFamilyFile(System.Collections.ArrayList block)
        {
            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

            family.filename = objArgs.sSvgFile;
            block.Add(family);
        }

        public override void ChangeFamilyFile(System.Collections.ArrayList block)
        {
            FAMILY_FILE_STRUCT family;
            family = ObjectGroup.GetMatchFamilyFile(objArgs.sSvgFile, block);
            if (family == null) return;
            objArgs.sSvgFile = family.change;
        }

        public override void AddObjectInfo(System.Windows.Forms.TreeNode parent)
        {
            string info = System.IO.Path.GetExtension(this.ToString()).Substring(1);
            info += String.Format(", {0}", this.objArgs.sSvgFile);

            System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode(info);
            parent.Nodes.Add(node);
        }

        public override string GetObjectMainTitle()
        {
            return objArgs.sSvgFile;
        }

        public override void EditFlipHorz()
        {
            AnimationClass.CalcFlipHorz(ref objArgs.nRotateFlip);
            //ApplyRotateFlip(); //Display 시 적용
        }

        public override void EditFlipVert()
        {
            AnimationClass.CalcFlipVert(ref objArgs.nRotateFlip);
            //ApplyRotateFlip(); //Display 시 적용
        }

        public override void EditRotateRight(int nx1, int ny1, int nx2, int ny2)
        {
            AnimationClass.CalcRotateRight(ref objArgs.nRotateFlip);
            //ApplyRotateFlip(); //Display 시 적용

            UpdateZone(objCommonProperty.form, nx1, ny1, nx2, ny2);
        }

        public override void EditRotateLeft(int nx1, int ny1, int nx2, int ny2)
        {
            AnimationClass.CalcRotateLeft(ref objArgs.nRotateFlip);
            //ApplyRotateFlip(); //Display 시 적용

            UpdateZone(objCommonProperty.form, nx1, ny1, nx2, ny2);
        }


        public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (this.formParent.InvokeRequired) // UI 스레드가 아닌 경우
            {
                return await (Task<object>)this.formParent.Invoke(new Func<Task<object>>(() =>
                    ExecuteClassName(bHandOperation, command, args) // 다시 UI 스레드에서 호출
                ));
            }
            bool retn = false;
            if (command == "SVGSetFile")
            {
                SetFileName((string)args[1]);
                InvalidateObject(formParent);
                return 1;
            }
            else if (command == "SVGElementColor")
            {
                retn = SetElementColor((string)args[0], (string)args[1], Color.FromArgb((int)args[2]), (int)args[3], (int)args[4]);

                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                //InvalidateObject(formParent);
                return 1;
            }
            else if (command == "SVGElementVisible")
            {
                retn = SetElementVisible((string)args[0], (string)args[1], (int)args[2]);
                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                //InvalidateObject(formParent);
                return 1;
            }
            else if (command == "SVGElementRotate")
            {
                retn = RotateElement((string)args[0], (string)args[1], (float)args[2]);
                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                return 1;
            }
            else if (command == "SVGElementMove")
            {
                retn = MoveElement((string)args[0], (string)args[1], (float)args[2], (float)args[3]);
                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                return 1;
            }
            else if (command == "SVGElementText")
            {
                retn = SetElementText((string)args[0], (string)args[1], (string)args[2]);
                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                return 1;
            }
            else if (command == "SVGElementTextSize")
            {
                retn = SetElementTextSize((string)args[0], (string)args[1], (float)args[2], (string)args[3]);
                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                return 1;
            }

            else if (command == "SVGElementScale")
            {
                retn = ScaleElement((string)args[0], (string)args[1], (float)args[2], (float)args[3]);
                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                return 1;
            }

            else if (command == "SVGElementTransform")
            {
                float rotate = 0f;
                float scaleX = 1f;
                float scaleY = 1f;
                float translateX = 0f;
                float translateY = 0f;
                bool useRotate = false;
                bool useScale = false;
                bool useTranslate = false;

                // args의 길이에 따라 값 설정
                if (args.Length > 2) // rotate
                {
                    rotate = (float)args[2];
                    useRotate = true;
                }
                if (args.Length > 4) // scale
                {
                    scaleX = (float)args[3];
                    scaleY = (float)args[4];
                    useScale = true;
                }
                if (args.Length > 6) // translate
                {
                    translateX = (float)args[5];
                    translateY = (float)args[6];
                    useTranslate = true;
                }

                retn = TransformElement(
                    (string)args[0],    // className
                    (string)args[1],    // elementId
                    rotate,             // rotateAngle
                    scaleX,            // scaleX
                    scaleY,            // scaleY
                    translateX,         // translateX
                    translateY,         // translateY
                    useRotate,         // use rotate
                    useScale,          // use scale
                    useTranslate       // use translate
                );

                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                return 1;
            }

            else if (command == "SVGElementReset")
            {
                retn = ResetElement((string)args[0], (string)args[1]);
                if (!retn && ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(sErrorString);
                    return 0;
                }
                return 1;
            }

            return 0;
        }
        private bool SetElementColor(string className, string elementId, Color color, int flagFill, int flagStroke)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementColor, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementColor, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false;
                }

                SvgVisualElement visualElement = element as SvgVisualElement;

                if (visualElement != null)
                {
                    if (flagFill != 0) visualElement.Fill = new SvgColourServer(color);
                    if (flagStroke != 0) visualElement.Stroke = new SvgColourServer(color);
                    cachedBitmap = null; // 캐시 무효화
                    return true;
                }
                else
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementColor, ClassName: {0}, Element with id '{1}' is not a visual element.", className, elementId));
                    return false;
                }

            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @SVGElementColor, ClassName: {0}, {1}.", className, ex.Message));
                DisposeCachedBitmap();
                svgDocument = null;
                return false;
            }
        }

        private bool SetElementVisible(string className, string elementId, int flag)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementVisible, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementVisible, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false; ;
                }

                SvgVisualElement visualElement = element as SvgVisualElement;

                if (visualElement != null)
                {
                    if (flag == 0) visualElement.Visible = false;
                    else visualElement.Visible = true;
                    cachedBitmap = null; // 캐시 무효화
                    return true;
                }
                else
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementVisible, ClassName: {0}, Element with id '{1}' is not a visual element.", className, elementId));
                    DisposeCachedBitmap();
                    svgDocument = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @SVGElementColor, ClassName: {0}, {1}.", className, ex.Message));
                return false;
            }
        }


        //중요. transform 은 위치->회전->크기 순으로 적용한다. 
        // 회전 시 x,y 축도 같이 회전하므로 위치 이동을 뒤에 하면 원치않는 방향으로 이동.
        // 회전 시, 좌표축을 설정하지 않으면 원점(0,0) 기준으로 회전한다.
        // scale을 먼저 적용하면 이후의 translate값도 영향을 받는다.

        private bool MoveElement(string className, string elementId, float dx, float dy)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementMove, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementMove, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false;
                }

                // 기존 transform들을 저장
                List<SvgTransform> existingTransforms = new List<SvgTransform>();
                List<SvgTransform> scaleRelatedTransforms = new List<SvgTransform>();

                if (element.Transforms != null)
                {
                    // Scale 관련 Transform과 rotate 구분하여 저장
                    for (int i = 0; i < element.Transforms.Count - 1; i++)
                    {
                        if (element.Transforms[i] is SvgTranslate &&
                            element.Transforms[i + 1] is SvgScale)
                        {
                            // Scale 관련 transform 쌍 저장
                            scaleRelatedTransforms.Add(element.Transforms[i]);     // translate
                            scaleRelatedTransforms.Add(element.Transforms[i + 1]); // scale
                            i++; // scale transform은 이미 처리했으므로 건너뛰기
                        }
                        else if (element.Transforms[i] is SvgRotate)
                        {
                            // rotate transform 저장
                            existingTransforms.Add(element.Transforms[i]);
                        }
                    }

                    // 마지막 transform이 rotate인 경우 처리
                    if (element.Transforms.Count > 0 &&
                        element.Transforms[element.Transforms.Count - 1] is SvgRotate)
                    {
                        existingTransforms.Add(element.Transforms[element.Transforms.Count - 1]);
                    }
                }

                // transforms 초기화
                element.Transforms = new SvgTransformCollection();

                // 1. 새로운 Translate 적용
                element.Transforms.Add(new SvgTranslate(dx, dy));

                // 2. Rotate transforms 적용
                foreach (var transform in existingTransforms)
                {
                    if (transform is SvgRotate)
                    {
                        element.Transforms.Add(transform);
                    }
                }

                // 3. Scale 관련 transforms 적용 (translate + scale 쌍 유지)
                foreach (var transform in scaleRelatedTransforms)
                {
                    element.Transforms.Add(transform);
                }

                cachedBitmap = null;
                return true;
            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @SVGElementMove, ClassName: {0}, {1}.", className, ex.Message));
                DisposeCachedBitmap();
                svgDocument = null;
                return false;
            }
        }

        private void RemoveExistingTranslation(SvgElement element)
        {
            if (element.Transforms != null)
            {
                for (int i = element.Transforms.Count - 1; i >= 0; i--)
                {
                    if (element.Transforms[i] is SvgTranslate)
                    {
                        element.Transforms.RemoveAt(i);
                    }
                }
            }
        }



        private bool RotateElement(string className, string elementId, float angle)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementRotate, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementRotate, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false;
                }

                SvgVisualElement visualElement = element as SvgVisualElement;
                if (visualElement == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementRotate, ClassName: {0}, Element is not a visual element.", className));
                    return false;
                }

                // 기존 transform들을 저장
                List<SvgTransform> existingTransforms = new List<SvgTransform>();
                List<SvgTransform> scaleRelatedTransforms = new List<SvgTransform>();

                if (element.Transforms != null)
                {
                    // Scale 관련 Transform과 일반 translate 구분하여 저장
                    for (int i = 0; i < element.Transforms.Count - 1; i++)
                    {
                        if (element.Transforms[i] is SvgTranslate &&
                            element.Transforms[i + 1] is SvgScale)
                        {
                            // Scale 관련 transform 쌍 저장
                            scaleRelatedTransforms.Add(element.Transforms[i]);     // translate
                            scaleRelatedTransforms.Add(element.Transforms[i + 1]); // scale
                            i++; // scale transform은 이미 처리했으므로 건너뛰기
                        }
                        else if (element.Transforms[i] is SvgTranslate)
                        {
                            // 일반 translate 저장
                            existingTransforms.Add(element.Transforms[i]);
                        }
                    }

                    // 마지막 transform이 translate이고 scale 쌍이 아닌 경우 처리
                    if (element.Transforms.Count > 0 &&
                        element.Transforms[element.Transforms.Count - 1] is SvgTranslate &&
                        !(element.Transforms.Count > 1 && element.Transforms[element.Transforms.Count - 2] is SvgTranslate &&
                          element.Transforms[element.Transforms.Count - 1] is SvgScale))
                    {
                        existingTransforms.Add(element.Transforms[element.Transforms.Count - 1]);
                    }
                }

                // transforms 초기화
                element.Transforms = new SvgTransformCollection();

                visualElement = element as SvgVisualElement; // transform 적용 후 다시 계산
                float centerX = visualElement.Bounds.X + visualElement.Bounds.Width / 2;
                float centerY = visualElement.Bounds.Y + visualElement.Bounds.Height / 2;

                // 1. Translate transforms 먼저 적용
                foreach (var transform in existingTransforms)
                {
                    if (transform is SvgTranslate)
                    {
                        element.Transforms.Add(transform);
                    }
                }

                // 2. 새로운 Rotate 적용 ( 중심점 기준)
                element.Transforms.Add(new SvgRotate(angle, centerX, centerY));

                // 3. Scale 관련 transforms 마지막으로 적용 (translate + scale 쌍 유지)
                foreach (var transform in scaleRelatedTransforms)
                {
                    element.Transforms.Add(transform);
                }

                cachedBitmap = null;
                return true;
            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @RotateElement, ClassName: {0}, {1}.", className, ex.Message));
                DisposeCachedBitmap();
                svgDocument = null;
                return false;
            }
        }

        private void RemoveExistingRotation(SvgElement element)
        {
            if (element.Transforms != null)
            {
                for (int i = element.Transforms.Count - 1; i >= 0; i--)
                {
                    if (element.Transforms[i] is SvgRotate)
                    {
                        element.Transforms.RemoveAt(i);
                    }
                }
            }
        }


        private bool SetElementText(string className, string elementId, string newText)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementText, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementText, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false;
                }

                SvgText textElement = element as SvgText;
                if (textElement != null)
                {
                    textElement.Text = newText;
                    cachedBitmap = null; // 캐시 무효화
                    return true;
                }
                else
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementText, ClassName: {0}, Element with id '{1}' is not a text element.", className, elementId));
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @SVGElementText, ClassName: {0}, {1}.", className, ex.Message));
                DisposeCachedBitmap();
                svgDocument = null;
                return false;
            }
        }

        private bool SetElementTextSize(string className, string elementId, float size, string unit)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementTextSize, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementTextSize, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false;
                }

                SvgText textElement = element as SvgText;
                if (textElement != null)
                {
                    // 단위에 따라 SvgUnitType 설정
                    SvgUnitType unitType;
                    switch (unit.ToLower())
                    {
                        case "vw":
                            unitType = SvgUnitType.Percentage; // vw는 percentage로 처리
                            size = size * 100 / svgDocument.ViewBox.Width; // vw를 percentage로 변환
                            break;
                        case "vh":
                            unitType = SvgUnitType.Percentage;
                            size = size * 100 / svgDocument.ViewBox.Height;
                            break;
                        case "em":
                            unitType = SvgUnitType.Em;
                            break;
                        case "%":
                            unitType = SvgUnitType.Percentage;
                            break;
                        case "px":
                        default:
                            unitType = SvgUnitType.Pixel;
                            break;
                    }

                    textElement.FontSize = new SvgUnit(unitType, size);
                    cachedBitmap = null; // 캐시 무효화
                    return true;
                }
                else
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementTextSize, ClassName: {0}, Element with id '{1}' is not a text element.", className, elementId));
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @SVGElementTextSize, ClassName: {0}, {1}.", className, ex.Message));
                DisposeCachedBitmap();
                svgDocument = null;
                return false;
            }
        }

        private bool TransformElement(string className, string elementId,
    float rotateAngle, float scaleX, float scaleY, float translateX, float translateY,
    bool useRotate, bool useScale, bool useTranslate)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementTransform, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementTransform, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false;
                }

                // transform 컬렉션 초기화 또는 생성, 좌표 계산 전에 초기화해야 함.
                if (element.Transforms == null)
                {
                    element.Transforms = new SvgTransformCollection();
                }
                else
                {
                    element.Transforms.Clear();
                }

                // 회전 중심점을 계산하려면 수동으로 설정
                float centerX = 0;
                float centerY = 0;

                // 요소가 SvgVisualElement인지 확인하고 캐스팅
                SvgVisualElement visualElement = element as SvgVisualElement;
                if (visualElement == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementRotate, ClassName: {0}, Element is not a visual element.", className));
                    return false;
                }
                // X, Y, Width, Height 속성을 이용해 중심점 계산
                centerX = visualElement.Bounds.X + visualElement.Bounds.Width / 2;
                centerY = visualElement.Bounds.Y + visualElement.Bounds.Height / 2;

                // 이동 적용
                if (useTranslate)
                {
                    element.Transforms.Add(new SvgTranslate(translateX, translateY));
                }

                // 회전 적용
                if (useRotate)
                {
                    element.Transforms.Add(new SvgRotate(rotateAngle, centerX, centerY));
                }

                if (useScale)
                {
                    // 1. 원래 위치로 되돌리기 (scale이 적용된 좌표로 이동)
                    element.Transforms.Add(new SvgTranslate(-centerX * (scaleX - 1), -centerY * (scaleY - 1)));

                    // 2. scale 적용
                    element.Transforms.Add(new SvgScale(scaleX, scaleY));
                }

                cachedBitmap = null; // 캐시 무효화
                return true;
            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @SVGElementTransform, ClassName: {0}, {1}.", className, ex.Message));
                DisposeCachedBitmap();
                svgDocument = null;
                return false;
            }
        }

        private bool ScaleElement(string className, string elementId, float scaleX, float scaleY)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementScale, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementScale, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false;
                }

                // 요소가 SvgVisualElement인지 확인하고 캐스팅
                SvgVisualElement visualElement = element as SvgVisualElement;
                if (visualElement == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementScale, ClassName: {0}, Element is not a visual element.", className));
                    return false;
                }

                // 기존 transform들을 저장
                List<SvgTransform> existingTransforms = new List<SvgTransform>();

                // transform 컬렉션이 없으면 생성
                if (element.Transforms != null)
                {
                    // 마지막 scale 쌍 제거 (있는 경우)
                    RemoveLastScale(element);

                    for (int i = 0; i < element.Transforms.Count; i++)
                    {
                        existingTransforms.Add(element.Transforms[i]);
                    }
                }

                // transforms 초기화
                element.Transforms = new SvgTransformCollection();

                visualElement = element as SvgVisualElement; // transform 적용 후 다시 계산
                float centerX = visualElement.Bounds.X + visualElement.Bounds.Width / 2;
                float centerY = visualElement.Bounds.Y + visualElement.Bounds.Height / 2;

                // 1. 기존 transforms 먼저 적용
                foreach (var transform in existingTransforms)
                {
                    element.Transforms.Add(transform);
                }

                // 새로운 scale transform 쌍 추가
                element.Transforms.Add(new SvgTranslate(-centerX * (scaleX - 1), -centerY * (scaleY - 1)));
                element.Transforms.Add(new SvgScale(scaleX, scaleY));

                cachedBitmap = null; // 캐시 무효화
                return true;
            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @SVGElementScale, ClassName: {0}, {1}.", className, ex.Message));
                DisposeCachedBitmap();
                svgDocument = null;
                return false;
            }
        }

        private void RemoveLastScale(SvgElement element)
        {
            if (element.Transforms != null && element.Transforms.Count >= 2)
            {
                // 마지막 scale transform 쌍 찾기
                for (int i = element.Transforms.Count - 1; i > 0; i--)
                {
                    if (element.Transforms[i] is SvgScale &&
                        element.Transforms[i - 1] is SvgTranslate)
                    {
                        // scale과 그 앞의 translate 제거
                        element.Transforms.RemoveAt(i);
                        element.Transforms.RemoveAt(i - 1);
                        break; // 마지막 scale 쌍만 제거
                    }
                }
            }
        }

        private void RemoveExistingScale(SvgElement element)
        {
            if (element.Transforms != null)
            {
                for (int i = element.Transforms.Count - 1; i >= 0; i--)
                {
                    if (element.Transforms[i] is SvgScale)
                    {
                        // scale transform 제거
                        element.Transforms.RemoveAt(i);

                        // scale 앞의 translate 확인 및 제거
                        if (i > 0 && element.Transforms[i - 1] is SvgTranslate)
                        {
                            element.Transforms.RemoveAt(i - 1);
                            i--; // 인덱스 조정
                        }
                    }
                }
            }
        }

        private void RemoveExistingTranslate(SvgElement element)
        {
            if (element.Transforms != null)
            {
                // 모든 기존 transform을 제거
                element.Transforms.Clear();
            }
        }

        private bool ResetElement(string className, string elementId)
        {
            try
            {
                if (svgDocument == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementReset, ClassName: {0}, SVG document is not loaded.", className));
                    return false;
                }

                SvgElement element = svgDocument.GetElementById(elementId);
                if (element == null)
                {
                    SetErrorString(String.Format("ScriptError: @SVGElementReset, ClassName: {0}, Element with id '{1}' not found.", className, elementId));
                    return false;
                }

                // transform 컬렉션 초기화
                if (element.Transforms != null)
                {
                    element.Transforms.Clear();
                }
                else
                {
                    element.Transforms = new SvgTransformCollection();
                }

                cachedBitmap = null; // 캐시 무효화
                return true;
            }
            catch (Exception ex)
            {
                SetErrorString(String.Format("ScriptError: @SVGElementReset, ClassName: {0}, {1}.", className, ex.Message));
                DisposeCachedBitmap();
                svgDocument = null;
                return false;
            }
        }


        //public override void MakeWebPublishFile(string target_dir)
        //{
        //    MakeWebPublishFile(target_dir, objArgs.sSvgFile, objArgs.nRotateFlip, (objArgs.nOverlayMethod == 0));
        //    //string targetFile = Path.Combine(target_dir, "WebPublish", Path.GetFileNameWithoutExtension(objArgs.sSvgFile) + ".svg");
        //    //Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
        //    //svgDocument.Write(targetFile);
        //}

    }
}