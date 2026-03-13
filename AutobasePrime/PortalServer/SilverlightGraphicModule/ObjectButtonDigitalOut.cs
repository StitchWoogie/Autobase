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
using System.Collections.Generic;
using NetTools.OldDefine;
using AutoLibLocal;
using AutoLib;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectButtonDigitalOut : ObjectButtonPublic
    {
        List<object> blockMember = new List<object>();

        int nDoMethod;
        int nDoDelayTime;

        public int DoMethod
        {
            get
            {
                return nDoMethod;
            }
            set
            {
                nDoMethod = value;
            }
        }

        public int DoDelayTime
        {
            get
            {
                return nDoDelayTime;
            }
            set
            {
                nDoDelayTime = value;
            }
        }

        public List<object> Member
        {
            get
            {
                return blockMember;
            }
            set
            {
                blockMember = value;
            }
        }

        public ObjectButtonDigitalOut(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general, ObjectArgsButtonPublic args, int do_method, List<object> block, int do_delaytime)
            : base(ocp, parent_canvas, rect, eid, lf, general, args)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ButtonDigitalOut;
            blockMember = block;
            nDoMethod = do_method;
            nDoDelayTime = do_delaytime;
        }

        public static void ControlDoGroup(List<object> block, int flag, int do_delay_time)
        {
            BUTTON_DOUT_STRUCT member;
            int[] pos = new int[1];

            if (block != null)
            {
                for (int l = 0; l < block.Count; l++)
                {
                    member = (BUTTON_DOUT_STRUCT)block[l];

                    if (!SharedData.userInfo.IsHaveTagRight(member.tag))
                    {
                        string msg;
                        if (NetTools.Tools.IsLangKorean())
                        {
                            msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자 이름으로\nLOGIN 하시기 바랍니다.\n\n태그={0}", member.tag);
                            MessageBox.Show(msg, "작동 권한 없음", MessageBoxButton.OK);
                        }
                        else if (NetTools.Tools.IsLangChinese())
                        {
                            msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}", member.tag);
                            MessageBox.Show(msg, "没有启动权限", MessageBoxButton.OK);
                        }
                        else
                        {
                            msg = String.Format("Access to the TAG is denied.\nLOGIN another username to control the TAG\n\nTAG name={0}", member.tag);
                            MessageBox.Show(msg, "Access denied", MessageBoxButton.OK);
                        }

                        return;
                    }
                }

                for (int l = 0; l < block.Count; l++)
                {
                    member = (BUTTON_DOUT_STRUCT)block[l];

                    TagPublicClass tp = TagLib.GetStructPublic(member.tag, ref pos);

                    string msg;
                    if (Tools.IsLangKorean())
                    {
                        if (flag == 1) msg = String.Format("수동작동 ON");
                        else msg = String.Format("수동작동 OFF");
                    }
                    else if (Tools.IsLangJapanese())
                    {
                        if (flag == 1) msg = String.Format("手動出力 ON");
                        else msg = String.Format("手動出力 OFF");
                    }
                    else if (Tools.IsLangChinese())
                    {
                        if (flag == 1) msg = String.Format("手动操作 ON");
                        else msg = String.Format("手动操作 OFF");
                    }
                    else
                    {
                        if (flag == 1) msg = String.Format("Manual Operation ON");
                        else msg = String.Format("Manual Operation OFF");
                    }

                    // AlarmUtil.AlarmDataSave(tp, msg, EnumAlarmType.HAND_OPERATION);

                    TagWrite.WriteCurr(tp, (sbyte)flag, true, do_delay_time);
                }
            }
        }

        protected override void OnClicked()
        {
            int flag = 0;

            if (nDoMethod == 1)
            {
                flag = 1;
            }
            else if (nDoMethod == 2)
            {
                flag = 0;
            }
            else
            {
                DialogControl.PageControlDigitalOut page = new SilverlightGraphicModule.DialogControl.PageControlDigitalOut(blockMember);

                SilverlightDialogControl.MyDialogCommon dialog = new SilverlightDialogControl.MyDialogCommon(page);

                page.SetParent(dialog);

                dialog.ShowDialog("DO Output");

                return;
            }

            ControlDoGroup(blockMember, flag, this.nDoDelayTime);
        }

        /*
        public override bool WmLeftButtonUp(UserControl form, System.Windows.Forms.MouseEventArgs e)
        {
            bool retn = base.WmLeftButtonUp(form, e);

            int flag;

            if (retn == true)
            {
                if (nDoMethod == 1)
                {
                    flag = 1;
                }
                else if (nDoMethod == 2)
                {
                    flag = 0;
                }
                else
                {
                    ControlBoxButtonDigitalOut dialog = new ControlBoxButtonDigitalOut(blockMember);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        flag = dialog.bOffOn;
                    }
                    else
                    {
                        return true;
                    }
                }

                BUTTON_DOUT_STRUCT member;
                int[] pos = new int[1];

                if (blockMember != null)
                {
                    for (int l = 0; l < blockMember.Count; l++)
                    {
                        member = (BUTTON_DOUT_STRUCT)blockMember[l];

                        if (!SharedData.userInfo.IsHaveTagRight(member.tag))
                        {
                            string msg;
                            if (NetTools.Tools.IsLangKorean())
                            {
                                msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자 이름으로\nLOGIN 하시기 바랍니다.\n\n태그={0}", member.tag);
                                MessageBox.Show(msg, "작동 권한 없음");
                            }
                            else if (NetTools.Tools.IsLangChinese())
                            {
                                msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}", member.tag);
                                MessageBox.Show(msg, "没有启动权限");
                            }
                            else
                            {
                                msg = String.Format("Access to the TAG is denied.\nLOGIN another username to control the TAG\n\nTAG name={0}", member.tag);
                                MessageBox.Show(msg, "Access denied");
                            }

                            return true;
                        }
                    }

                    for (int l = 0; l < blockMember.Count; l++)
                    {
                        member = (BUTTON_DOUT_STRUCT)blockMember[l];

                        TagPublicClass tp = TagLib.GetStructPublic(member.tag, ref pos);

                        string msg;
                        if (Tools.IsLangKorean())
                        {
                            if (flag == 1) msg = String.Format("수동작동 ON");
                            else msg = String.Format("수동작동 OFF");
                        }
                        else if (Tools.IsLangJapanese())
                        {
                            if (flag == 1) msg = String.Format("手動出力 ON");
                            else msg = String.Format("手動出力 OFF");
                        }
                        else if (Tools.IsLangChinese())
                        {
                            if (flag == 1) msg = String.Format("手动操作 ON");
                            else msg = String.Format("手动操作 OFF");
                        }
                        else
                        {
                            if (flag == 1) msg = String.Format("Manual Operation ON");
                            else msg = String.Format("Manual Operation OFF");
                        }
                        AlarmUtil.AlarmDataSave(tp, msg, EnumAlarmType.HAND_OPERATION);

                        TagWrite.WriteCurr(tp, (sbyte)flag, true, this.nDoDelayTime);
                    }
                }
            }

            return retn;
        }
        

        public override void ObjectSave(CommaTextWriter writer)
        {
            BUTTON_DOUT_STRUCT member;

            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            SaveObjectItem.String(writer, this.Text);
            writer.WriteLine("\tTagDigitalOutputMethod,{0},", nDoMethod);
            writer.WriteLine("\tTagDigitalOutputDelayTime,{0},", nDoDelayTime);	// ON후OFF시 지연 시간.
            if (blockMember != null)
            {
                for (int l = 0; l < blockMember.Count; l++)
                {
                    member = (BUTTON_DOUT_STRUCT)blockMember[l];
                    writer.WriteLine("\tButtonDoutMember,{0},", member.tag);
                }
            }
            ObjectSaveFont(writer);
        }

        public override void GetMultiSelectTagList(ArrayList block)
        {
            base.GetMultiSelectTagList(block);

            BUTTON_DOUT_STRUCT member;
            EnumTagType tag_type = 0;

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (BUTTON_DOUT_STRUCT)blockMember[i];

                TagLib.GetTagTypeAndPos(member.tag, ref tag_type, ref member.tag_pos);

                ObjectGroup.AddTagList(block, member.tag, tag_type);
            }
        }

        public override void SetMultiSelectTagList(ArrayList block)
        {
            base.SetMultiSelectTagList(block);

            BUTTON_DOUT_STRUCT member;

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (BUTTON_DOUT_STRUCT)blockMember[i];

                string tag = member.tag;
                if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                    member.tag = tag;
            }
        }*/
    }
}
