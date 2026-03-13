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

namespace SilverlightGraphicModule
{
    public class ObjectPublicGroupLayer : ObjectExpand
    {
        protected List<object> objectList;
        /*
        public bool bOnStudioGroupOpen = false; // 스튜디오에서 편집 시에만 사용하는 기능
        public string sSelectedTagName;
        public EnumTagType eSelectedTagType;
        */
        
        public ObjectPublicGroupLayer(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            objectList = new List<object>();
        }

        /*
        // 이 오브젝트를 닫기 전에 해야 할 일
        public override void Close()
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                ((ObjectExpand)obj).Close();
            }
        }*/

        public void AddObject(object p)
        {
            ((ObjectType)p).parentGroupLayer = this;
            objectList.Add(p);
        }
        /*
        public void InsertObject(object p, int pos)
        {
            ((ObjectType)p).parentGroupLayer = this;
            objectList.Insert(pos, p);
        }

        public override void SetBasePoint(int x, int y)
        {
            base.SetBasePoint(x, y);

            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).SetBasePoint(x, y);
            }
        }

        public int GetObjectHap()
        {
            return objectList.Count;
        }

        public EnumObjectType GetObjectType(int pos)
        {
            return ((ObjectType)objectList[pos]).enumObjectType;
        }

        public object GetPoint(int pos)
        {
            return objectList[pos];
        }

        public override void SetScreenSize(int x, int y)
        {
            base.SetScreenSize(x, y);

            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).SetScreenSize(x, y);

            }
        }

        public bool GetZone(int pos, ref int x1, ref int y1, ref int x2, ref int y2)
        {
            if (pos >= objectList.Count)
            {
                MessageBox.Show("pos >= ObjectHap", "GetZone");
                return false;
            }

            object obj;
            obj = objectList[pos];

            ((ObjectExpand)obj).GetZone(ref x1, ref y1, ref x2, ref y2);

            return true;
        }

        public override void SetObjectOpticMethod(int method)
        {
            base.SetObjectOpticMethod(method);

            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).SetObjectOpticMethod(method);
            }
        }

        public override void SetModuleSize(int x, int y)
        {
            base.SetModuleSize(x, y);

            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).SetModuleSize(x, y);

            }
        }

        public override void SetOpticRate(int rate)
        {
            base.SetOpticRate(rate);

            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                ((ObjectExpand)obj).SetOpticRate(rate);

            }
        }*/

        public override void EventTimer(UserControl form)
        {
            int l;
            object obj;

            base.EventTimer(form);  // 그룹일때도 확장 기능을 검사한다.

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                ((ObjectExpand)obj).EventTimer(form);
            }
        }
        /*
        public override void EventTag(System.Windows.Forms.Form form, AutoLib.COMM_EVENT_STRUCT tagevent)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                ((ObjectExpand)obj).EventTag(form, tagevent);
            }
        }

*/
        public void FreeAllObjectBuf()
        {
            objectList.Clear();
        }

        public override bool WmLeftButtonDown(UserControl form, MouseEventArgs e)
        {
            if (base.WmLeftButtonDown(form, e))
                return true;

            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                if (((ObjectExpand)obj).WmLeftButtonDown(form, e))
                    return true;

            }

            return false;
        }

        

        public override bool WmLeftButtonUp(UserControl form, MouseEventArgs e)
        {
            if (base.WmLeftButtonUp(form, e))
                return true; ;

            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                if (((ObjectExpand)obj).WmLeftButtonUp(form, e))
                    return true; ;

            }

            return false;
        }

        public override void OnTagFileReaded()
        {
            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                ((ObjectExpand)obj).OnTagFileReaded();
            }
        }

        /*
        public override bool WmRightButtonDown(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                    case EnumObjectType.Layer:
                        if (((ObjectPublicGroupLayer)obj).WmRightButtonDown(form, e))
                        {
                            sSelectedTagName = ((ObjectPublicGroupLayer)obj).sSelectedTagName;
                            eSelectedTagType = ((ObjectPublicGroupLayer)obj).eSelectedTagType;

                            return true;
                        }
                        break;
                    default:
                        if (((ObjectExpand)obj).WmRightButtonDown(form, e))
                        {
                            string obj_name = obj.ToString();
                            if (obj_name == "GraphicModule.ObjectAnalogMeter" ||
                                obj_name == "GraphicModule.ObjectAnalogRectangle" ||
                                obj_name == "GraphicModule.ObjectAnalogRotate" ||
                                obj_name == "GraphicModule.ObjectAnalogStatus" ||
                                obj_name == "GraphicModule.ObjectAnalogString" ||
                                obj_name == "GraphicModule.ObjectDigitalAnimation" ||
                                obj_name == "GraphicModule.ObjectDigitalCircle" ||
                                obj_name == "GraphicModule.ObjectDigitalRectangle" ||
                                obj_name == "GraphicModule.ObjectDigitalString" ||
                                obj_name == "GraphicModule.ObjectStringString")
                            {
                                sSelectedTagName = ((ObjectTag)obj).sTagName;
                                eSelectedTagType = ((ObjectTag)obj).nTagType;
                            }
                            else
                            {
                                sSelectedTagName = "";
                            }
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        public override bool WmRightButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                if (((ObjectExpand)obj).WmRightButtonUp(form, e))
                    return true; ;

            }

            return false;
        }*/

        public override bool WmMouseMove(UserControl form, MouseEventArgs e)
        {
            if (base.WmMouseMove(form, e))
                return true;


            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                if (((ObjectExpand)obj).WmMouseMove(form, e))
                    return true;
            }

            return false;
        }

        /*
        public void DeleteOneObject(int pos)
        {
            objectList.RemoveAt(pos);
        }

        public void RecurseTempObject(ref object obj, int[] pos, int depth)
        {
            int i = pos[depth];

            if (i >= objectList.Count)
                return; // 배열 오버

            if (pos.Length == depth + 1)
            { // 마지막까지 들어왔다.
                //bool selected = ((ObjectType)objectList[i]).bOnStudioSelected;

                object temp = obj;
                obj = objectList[i];
                objectList[i] = temp;

                //((ObjectType)objectList[i]).bOnStudioSelected = selected;   // 이전의 선택도 돌려주는 것이 좋다.
            }
            else
            {
                ObjectType type = (ObjectType)objectList[i];

                if (type.enumObjectType == EnumObjectType.Layer || type.enumObjectType == EnumObjectType.Group)
                {
                    ((ObjectPublicGroupLayer)type).RecurseTempObject(ref obj, pos, depth + 1);
                }
            }

     
        }

        public object RecurseSeekObject(int[] pos, int depth)
        {
            int i = pos[depth];

            if (i >= objectList.Count)
                return null; // 배열 오버

            if (pos.Length == depth + 1)    // 마지막까지 들어왔다.
            {
                return objectList[i];
            }
            else
            {
                ObjectType type = (ObjectType)objectList[i];

                if (type.enumObjectType == EnumObjectType.Layer || type.enumObjectType == EnumObjectType.Group)
                {
                    object obj = ((ObjectPublicGroupLayer)type).RecurseSeekObject(pos, depth + 1);
                    return obj;
                }
            }

            return null;
        }

        public bool RecurseDeleteObject(int[] pos, int depth)
        {
            int i = pos[depth];

            if (i >= objectList.Count)
                return false; // 배열 오버

            if (pos.Length == depth + 1)    // 마지막까지 들어왔다.
            {
                objectList.RemoveAt(i);
                return true;
                //return objectList[i];
            }
            else
            {
                ObjectType type = (ObjectType)objectList[i];

                if (type.enumObjectType == EnumObjectType.Layer || type.enumObjectType == EnumObjectType.Group)
                {
                    bool retn = ((ObjectPublicGroupLayer)type).RecurseDeleteObject(pos, depth + 1);
                    return retn;
                }
            }

            return false;
        }

        public bool RecurseInsertObject(object obj, int[] pos, int depth)
        {
            int i = pos[depth];

     
            if (pos.Length == depth + 1)    // 마지막까지 들어왔다.
            {
                InsertObject(obj, i);
                return true;
                 }
            else
            {
                ObjectType type = (ObjectType)objectList[i];

                if (type.enumObjectType == EnumObjectType.Layer || type.enumObjectType == EnumObjectType.Group)
                {
                    bool retn = ((ObjectPublicGroupLayer)type).RecurseInsertObject(obj, pos, depth + 1);
                    return retn;
                }
            }

            return false;
        }

        public void MovePost(int pos)
        {
            if (pos >= objectList.Count) return;

            if (pos == objectList.Count - 1) return;

            object obj = objectList[pos];
            objectList.RemoveAt(pos);
            objectList.Add(obj);
        }

        public void MovePrevPost(int pos)
        {
            if (pos >= objectList.Count) return;

            if (pos == objectList.Count - 1) return;	// 이미 위에 있다.

            object obj = objectList[pos];
            objectList.RemoveAt(pos);
            objectList.Insert(pos + 1, obj);
        }

        public void MoveBack(int pos)
        {
            if (objectList.Count <= 0) return;

            if (pos >= objectList.Count) return;

            if (pos == 0) return;

            object obj = objectList[pos];
            objectList.RemoveAt(pos);
            objectList.Insert(0, obj);
        }

        public void MoveNextBack(int pos)
        {
            if (objectList.Count <= 0) return;

            if (pos >= objectList.Count) return;

            if (pos == 0) return;

            object obj = objectList[pos];
            objectList.RemoveAt(pos);
            objectList.Insert(pos - 1, obj);
        }

        void ChangePos(int pos_fr, int pos_to)
        {
            object temp = objectList[pos_fr];
            objectList.RemoveAt(pos_fr);
            objectList.Insert(pos_to, temp);
        }

        public bool RecurseChangePos(int[] pos_fr, int[] pos_to, int depth)
        {
            int i = pos_fr[depth];

     
            if (pos_fr.Length == depth + 1)    // 마지막까지 들어왔다.
            {
                ChangePos(pos_fr[depth], pos_to[depth]);
     
                return true;
     
            }
            else
            {
                ObjectType type = (ObjectType)objectList[i];

                if (type.enumObjectType == EnumObjectType.Layer || type.enumObjectType == EnumObjectType.Group)
                {
                    bool retn = ((ObjectPublicGroupLayer)type).RecurseChangePos(pos_fr, pos_to, depth + 1);
                    return retn;
                }
            }

            return false;
        }


        public int ChangeObject(int pos, object p)
        {
            objectList[pos] = p;
            return 1;
        }

        public void TempUndo(ref ArrayList undo)
        {
            ArrayList temp;

            temp = undo;
            undo = objectList;
            objectList = temp;
        }

        /// <summary>
        /// 그림영역이 있지만 오브젝트가 실제로 그려진 위치는 더 클 수 있다.
        ///	실제 영역을 찾는다. 
        /// </summary>
        /// <returns></returns>

        public Rectangle GetObjectExistZone()
        {
            int l;

            int bx1 = 0, by1 = 0, bx2 = 0, by2 = 0;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            ObjectExpand obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = (ObjectExpand)objectList[l];

                if (obj.enumObjectType == EnumObjectType.Layer)
                {
                    Rectangle r = ((ObjectPublicGroupLayer)obj).GetObjectExistZone();

                    x1 = r.Left;
                    y1 = r.Top;
                    x2 = r.Right;
                    y2 = r.Bottom;
                }
                else
                {
                    GetZone(l, ref x1, ref y1, ref x2, ref y2);
                }

                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                if (x1 < bx1) bx1 = x1;
                if (y1 < by1) by1 = y1;
                if (x2 > bx2) bx2 = x2;
                if (y2 > by2) by2 = y2;
            }

            return new Rectangle(bx1, by1, bx2 - bx1 + 1, by2 - by1 + 1);
        }

        public override void GetFamilyFile(ArrayList block)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).GetFamilyFile(block);

            }
        }

        public override void ChangeFamilyFile(ArrayList block)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).ChangeFamilyFile(block);

            }
        }*/

        public override void GetMultiSelectTagList(List<object> block)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).GetMultiSelectTagList(block);

            }
        }

        public override void SetMultiSelectTagList(List<object> block)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).SetMultiSelectTagList(block);

            }
        }

        /*
        public override void EventTimerOnPreview(System.Windows.Forms.Form form)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).EventTimerOnPreview(form);

            }
        }

        public override void AddObjectInfo(TreeNode parent)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                        TreeNode node = new TreeNode("ObjectGroup");
                        parent.Nodes.Add(node);
                        ((ObjectGroup)obj).AddObjectInfo(node);
                        break;
                    default:
                        ((ObjectExpand)obj).AddObjectInfo(parent);

                        break;
                }
            }
        }

        public void CallBackObject(string type, ObjectExpand.DelegateCallBackObject call)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                        ((ObjectGroup)obj).CallBackObject(type, call);
                        break;
                    default:
                        if (type == obj.ToString())
                        {
                            call(obj);
                        }

                        break;
                }
            }
        }


        public override void SetCommonProperty(ObjectCommonProperty cp)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).SetCommonProperty(cp);

            }
        }*/

        public object ExecuteClassNameOnlyObject(Page form, string classname, string command, params object[] args)
        {
            int l;
            object obj;
            object retn = null;
            ObjectExpand expand;
            bool this_flag;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                    case EnumObjectType.Layer:
                        retn = ((ObjectPublicGroupLayer)obj).ExecuteClassNameOnlyObject(form, classname, command, args);
                        break;
                    default:
                        expand = (ObjectExpand)obj;

                        this_flag = String.Compare(classname, 0, "this.", 0, 5) == 0;

                        if (this_flag && form == objCommonProperty.rootPage &&
                            String.Compare(classname, 5, expand.objGeneral.sClassName, 0, expand.objGeneral.sClassName.Length) == 0)
                        {
                            retn = expand.ExecuteClassNameOnlyObject(command, args);
                            break;
                        }

                        if (classname == expand.objGeneral.sClassName)
                            retn = expand.ExecuteClassNameOnlyObject(command, args);

                        break;
                }
            }

            return retn;
        }

        /*
        public object GetObjectOfMouseZone(MouseEventArgs e)
        {
            object retn = null;
            object obj;

            for (int i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                        retn = ((ObjectGroup)obj).GetObjectOfMouseZone(e);
                        if (retn != null) return retn;
                        break;
                    default:
                        ObjectExpand expand = (ObjectExpand)obj;
                        if (expand.IsMouseInViewZone(e))
                            return expand;

                        break;
                }
            }

            return null;
        }

        */
    }
}
