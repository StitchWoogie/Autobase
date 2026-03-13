using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NetTools.OldDefine;
using System.Collections.Generic;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    //[Serializable]
    public class ObjectArgsAnalogStatus
    {
        
    }
    //[Serializable]
    public class ANALOG_STATUS_STRUCT
    {
        public int type;
        public string filename;
    }

    /// <summary>
    /// Summary description for ObjectAnalogString.
    /// </summary>
    //[Serializable]
    public class ObjectAnalogStatus : ObjectTag
    {
        ObjectArgsAnalogStatus objArgs;

        List<object> blockStatus;
        AnimationClass[] animationList = new AnimationClass[16];
        int nCurrFrame;
        int nCurrStruct;
        //int nMaxFrame;
        //int nMaxClock;
        //int nWidth;
        //int nHeight;

        public List<object> Member
        {
            get
            {
                return blockStatus;
            }
            set
            {
                blockStatus = value;
                LoadAnimation();
                ChangeAnimationStruct();
            }
        }

        public ObjectAnalogStatus(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsAnalogStatus args, List<object> status_struct)
            : base(ocp, rect, eid, general, null, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //
        
            enumObjectType = EnumObjectType.AnalogStatus;
            objArgs = args;
            blockStatus = status_struct;

            LoadAnimation();

            nCurrFrame = 0;
            nCurrStruct = -1;
            //nMaxFrame = 0;
            //nMaxClock = 0;
            //nWidth = 50;
            //nHeight = 50;

            Image child = new Image();

            child.Stretch = Stretch.Fill;
            parent_canvas.Children.Add(child);
            SetShapeOriginal(child);
            MoveShape();

            //SetFileNameOn(args.sFileOn);    // 속에 ChangeAnimationStruct()가 포함되어 있다.
            //SetFileNameOff(args.sFileOff);  // 속에 ChangeAnimationStruct()가 포함되어 있다.

            ChangeAnimationStruct();

            /*
            // 이전버전 Left Top 좌표만 있던 시절

            if (rect.right == -9999 && rect.bottom == -9999)
            {
                UpdateZone(nLeft, nTop, base.nLeft + nWidth - 1, base.nTop + nHeight - 1);
                CreateMouseZone(eid, nLeft, nTop, nRight, nBottom, rMouse);
            }*/

        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

        

            if (nTagPos[0] == TagLib.TAG_NOT_FOUND)
            {
                DrawClass.PopBox2(g, x1, y1, x2, y2, Color.FromArgb(0xC0, 0xC0, 0xC0));

                Font font = MakeFont();
                StringFormat format = new StringFormat();

                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                if (Tools.IsLangKorean())
                    DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, "태그없음", Color.Black, Color.FromArgb(0xC0, 0xC0, 0xC0), font, format);
                else if (Tools.IsLangJapanese())
                    DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, "タグない", Color.Black, Color.FromArgb(0xC0, 0xC0, 0xC0), font, format);
                else if (Tools.IsLangChinese())
                    DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, "没有标记", Color.Black, Color.FromArgb(0xC0, 0xC0, 0xC0), font, format);
                else
                    DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, "No Tag", Color.Black, Color.FromArgb(0xC0, 0xC0, 0xC0), font, format);


                return;
            }
            else if (nCurrStruct == -1)
            {
                DrawClass.PopBox2(g, x1, y1, x2, y2, Color.FromArgb(0xC0, 0xC0, 0xC0));

                Font font = MakeFont();
                StringFormat format = new StringFormat();

                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                if (Tools.IsLangKorean())
                {
                    DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, "설정필요", Color.Black, Color.FromArgb(0xC0, 0xC0, 0xC0), font, format);
                }
                else
                {
                    DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, "Need Set", Color.Black, Color.FromArgb(0xC0, 0xC0, 0xC0), font, format);
                }

                return;
            }

            ANALOG_STATUS_STRUCT status = (ANALOG_STATUS_STRUCT)blockStatus[nCurrStruct];
            if (status.type == 0)
            {
                DrawClass.PopBox2(g, x1, y1, x2, y2, Color.FromArgb(0xC0, 0xC0, 0xC0));

                Font font = MakeFont();
                StringFormat format = new StringFormat();

                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                if (Tools.IsLangKorean())
                    DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, "설정필요", Color.Black, Color.FromArgb(0xC0, 0xC0, 0xC0), font, format);
                else
                    DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, "Need Set", Color.Black, Color.FromArgb(0xC0, 0xC0, 0xC0), font, format);


            }
            else
            {
                animation[nCurrStruct].Putimage(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, nCurrFrame);
            }
        }*/

        public override void EventTimerObject(UserControl form)
        {
            base.EventTimerObject(form);

            if (this.ChangeAnimationStruct())
            {
                InvalidateObject(form);
            }

            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            ai.bNeedDataCurr = true;

            int nMaxClock = animation.GetRPM();
            int nMaxFrame = animation.GetMaxFrame();

            int speed = nMaxClock;

            if (speed == 0) return;

            DateTime dt = DateTime.Now;

            int frame;

            if (speed < 0)
            {
                frame = (int)(Math.Abs(speed) * nMaxFrame * (dt.Second * 1000 + dt.Millisecond) / 60000);
                frame = frame % nMaxFrame;
                frame = nMaxFrame - 1 - frame;
            }
            else
            {
                frame = (int)(speed * nMaxFrame * (dt.Second * 1000 + dt.Millisecond) / 60000);
                frame = frame % nMaxFrame;
            }

            if (nCurrFrame != frame)
            {
                nCurrFrame = frame;
                InvalidateObject(form);
                Image child = (Image)GetShapeOriginal();
                child.Source = animation.GetImageSource(nCurrFrame);
            }
        }
        /*
        public override void EventTag(System.Windows.Forms.Form form, COMM_EVENT_STRUCT tagevent)
        {
            if (tagevent.tag_type != EnumTagType.AI) return;
            if (String.Compare(tagevent.tag, sTagName, true) != 0) return;

            ChangeAnimationStruct();

            InvalidateObject(form);
        }*/

        void LoadAnimation()
        {
            if (blockStatus == null) return;

            int i;
            string filename;

            ANALOG_STATUS_STRUCT status;

            for (i = 0; i < 16; i++)
            {
                status = (ANALOG_STATUS_STRUCT)blockStatus[i];
                if (status.type == 0) continue;	// 설정 안됨

                filename = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(objCommonProperty, status.filename);

                animationList[i] = new AnimationClass();
                animationList[i].LoadImage(filename, false, 0);


            }
        }

        public static ushort[] WORD_MASK = {	0x0001, 0x0002, 0x0004, 0x0008,
												0x0010, 0x0020, 0x0040, 0x0080,
												0x0100, 0x0200, 0x0400, 0x0800,
												0x1000, 0x2000, 0x4000, 0x8000 };

        bool ChangeAnimationStruct()
        {
            int i;
            TagAiClass ai = TagLib.GetStructAI(sTagName, ref nTagPos);

            if (nTagPos[0] == TagLib.TAG_NOT_FOUND) return false;	// AI 가 아닐때

            ANALOG_STATUS_STRUCT status;

            for (i = 0; i < 16; i++)
            {
                status = (ANALOG_STATUS_STRUCT)blockStatus[i];
                if (status.type == 0) continue;	// 설정 안됨

                if (status.type >= 1 && status.type <= 16)
                {
                    if ((WORD_MASK[status.type - 1] & (ushort)ai.real_curr) != 0)	// ai.curr로 하면 이전에 사용하던 롯데호텔 가스감시와 같은 부분은 동작되지 않는다. ai.curr로 사용하고 싶으면 옵션으로 추가해야 할것이다.
                    {
                        goto ok_i_seek_change;
                    }
                }
                else if (status.type >= 17 && status.type <= 32)
                {
                    if ((WORD_MASK[status.type - 17] & (ushort)ai.real_curr) == 0)
                    {
                        goto ok_i_seek_change;
                    }
                }
                else if (status.type == 33)
                {	// curr >= HIHI
                    if (ai.curr >= ai.hihi)
                        goto ok_i_seek_change;
                }
                else if (status.type == 34)
                {	// curr >= HIGH
                    if (ai.curr >= ai.high)
                        goto ok_i_seek_change;
                }
                else if (status.type == 35)
                {	// curr >= LOW
                    if (ai.curr >= ai.low)
                        goto ok_i_seek_change;
                }
                else if (status.type == 36)
                {	// curr >= LOLO
                    if (ai.curr >= ai.lolo)
                        goto ok_i_seek_change;
                }
                else if (status.type == 37)
                {	// curr <= HIHI
                    if (ai.curr <= ai.hihi)
                        goto ok_i_seek_change;
                }
                else if (status.type == 38)
                {	// curr <= HIGH
                    if (ai.curr <= ai.high)
                        goto ok_i_seek_change;
                }
                else if (status.type == 39)
                {	// curr <= LOW
                    if (ai.curr <= ai.low)
                        goto ok_i_seek_change;
                }
                else if (status.type == 40)
                {	// curr <= LOLO
                    if (ai.curr <= ai.lolo)
                        goto ok_i_seek_change;
                }
                else
                {											// 평상 시
                    goto ok_i_seek_change;
                }
            }
            return false;

        ok_i_seek_change:

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (nCurrStruct == i) return false;
            }

            nCurrStruct = i;
            animation = animationList[nCurrStruct];

            //nMaxFrame = animation[nCurrStruct].GetMaxFrame();
            //if (nCurrFrame >= nMaxFrame) nCurrFrame = 0;
            //nMaxClock = animation[nCurrStruct].GetRPM();
            //nWidth = animation[nCurrStruct].Width();
            //nHeight = animation[nCurrStruct].Height();
            nCurrStruct = i;

            Image child = (Image)GetShapeOriginal();
            child.Source = animation.GetImageSource(nCurrFrame);

            return true;
        }

        AnimationClass animation = new AnimationClass();

        /*
        public override void ObjectSave(CommaTextWriter writer)
        {
            ANALOG_STATUS_STRUCT status;

            ObjectSaveTag(writer);

            for (int i = 0; i < blockStatus.Count; i++)
            {
                status = (ANALOG_STATUS_STRUCT)blockStatus[i];
                writer.WriteLine("\tMemberAnalogStatus,{0},{1}", status.type, status.filename);
            }
        }

        public override void GetFamilyFile(ArrayList block)
        {
            if (blockStatus == null) return;
            FAMILY_FILE_STRUCT family;
            ANALOG_STATUS_STRUCT status;

            for (int i = 0; i < 16; i++)
            {
                status = (ANALOG_STATUS_STRUCT)blockStatus[i];

                if (status.type == 0) continue;
                if (status.filename.Length == 0) continue;
                family = new FAMILY_FILE_STRUCT();
                family.filename = status.filename;
                block.Add(family);
            }
        }

        public override void ChangeFamilyFile(ArrayList block)
        {
            if (blockStatus == null) return;

            FAMILY_FILE_STRUCT family;
            ANALOG_STATUS_STRUCT status;

            for (int i = 0; i < 16; i++)
            {
                status = (ANALOG_STATUS_STRUCT)blockStatus[i];

                if (status.type == 0) continue;
                if (status.filename.Length == 0) continue;

                family = ObjectGroup.GetMatchFamilyFile(status.filename, block);
                if (family != null)
                {
                    status.filename = family.change;
                }
            }
        }*/

    }
}
