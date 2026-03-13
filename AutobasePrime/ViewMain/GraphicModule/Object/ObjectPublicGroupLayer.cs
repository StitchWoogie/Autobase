using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NetTools.OldDefine;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using AutoLibLocal;
using NetTools;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GraphicModule
{
    [Serializable]
    public class ObjectPublicGroupLayer : ObjectExpand
    {
        protected ArrayList objectList;

        public bool bOnStudioGroupOpen = false; // 스튜디오에서 편집 시에만 사용하는 기능
        public string sSelectedTagName;
        public EnumTagType eSelectedTagType;

        public ObjectPublicGroupLayer(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general)
            : base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
            objectList = new ArrayList();
		}

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
        }

        public override void Dispose()
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                ((ObjectExpand)obj).Dispose();
            }
        }

        public void AddObject(object p)
        {
            ((ObjectType)p).parentGroupLayer = this;
            objectList.Add(p);
        }

        public void InsertObject(object p, int pos)
        {
            ((ObjectType)p).parentGroupLayer = this;

            if (pos < 0 || pos >= objectList.Count)    // pos 가 Insert위치를 넘어가는 경우가 있어서 처리했다. 2010-9-2
                objectList.Add(p);
            else
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

        public int GetPosition(object obj)
        {
            for (int i = 0; i < objectList.Count; i++)            
            {
                if (obj == objectList[i]) return i;
            }

            return -1;
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
        }

        private int currentIndex = 0;

        public override async Task EventTimerAsync(System.Windows.Forms.Form form)
        {
            try
            {
                await base.EventTimerAsync(form);  // 그룹일때도 확장 기능을 검사한다.

                if (ConfigViewMain.bUseEventTimerBatching)
                {
                    // 배칭 모드: 설정된 배치 크기만큼만 처리
                    int processed = 0;
                    int batchSize = ConfigViewMain.nEventTimerBatchSize > 0 ? ConfigViewMain.nEventTimerBatchSize : 1; // 0 이하 방지

                    // 인덱스 유효성 검사 (objectList가 변경되었을 수 있음)
                    if (currentIndex >= objectList.Count)
                        currentIndex = 0;

                    while (processed < batchSize && currentIndex < objectList.Count)
                    {
                        var obj = objectList[currentIndex];
                        await ((ObjectExpand)obj).EventTimerAsync(form);

                        currentIndex++;
                        processed++;
                    }

                    // 모든 객체 처리 완료 시 처음부터 다시
                    if (currentIndex >= objectList.Count)
                        currentIndex = 0;
                }
                else
                {
                    // 기본 모드: 모든 객체를 한 번에 처리
                    // 배칭 모드에서 기본 모드로 전환 시 인덱스 초기화
                    currentIndex = 0;

                    for (int l = 0; l < objectList.Count; l++)
                    {
                        var obj = objectList[l];
                        await ((ObjectExpand)obj).EventTimerAsync(form);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex, $"GroupEventTimer_Index_{currentIndex}");
            }
        }

        //public override async Task EventTimerAsync(System.Windows.Forms.Form form)
        //{
        //   // int l;
        //    //object obj = null;

        //    try
        //    {
        //        await base.EventTimerAsync(form);  // 그룹일때도 확장 기능을 검사한다.

        //        //for (l = 0; l < objectList.Count; l++)
        //        //{
        //        //    obj = objectList[l];
        //        //    await ((ObjectExpand)obj).EventTimerAsync(form);
        //        //}

        //        int processed = 0;
        //        const int batchSize = 20;

        //        while (processed < batchSize && currentIndex < objectList.Count)
        //        {
        //            var obj = objectList[currentIndex];
        //            await ((ObjectExpand)obj).EventTimerAsync(form);

        //            currentIndex++;
        //            processed++;
        //        }

        //        // 모든 객체 처리 완료 시 처음부터 다시
        //        if (currentIndex >= objectList.Count)
        //            currentIndex = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        //LogException(ex, $"GroupEventTimer_Object_{obj?.GetType().Name}");
        //        LogException(ex, $"GroupEventTimer_Index_{currentIndex}");
        //    }
        //}

        //public override async Task EventTimerAsync(System.Windows.Forms.Form form)
        //{
        //    int l;
        //    object obj = null;

        //    try
        //    {
        //        await base.EventTimerAsync(form);  // 그룹일때도 확장 기능을 검사한다.

        //        for (l = 0; l < objectList.Count; l++)
        //        {
        //            obj = objectList[l];
        //            await ((ObjectExpand)obj).EventTimerAsync(form);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException(ex, $"GroupEventTimer_Object_{obj?.GetType().Name}");           
        //    }
        //}

        private void LogException(Exception ex, string context)
        {
            System.Diagnostics.Debug.WriteLine($"[{context}] Exception: {ex.Message}");
        }

        public override void EventTag(System.Windows.Forms.Form form, TagPublicClass tagevent)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                ((ObjectExpand)obj).EventTag(form, tagevent);
            }
        }


        public void FreeAllObjectBuf()
        {
            objectList.Clear();
        }

        public override async Task<bool> WmLeftButtonDown(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            if (!CheckResponseOnVisible()) return false;

            if (await base.WmLeftButtonDown(form, e))
                return true;

            int i;
            object obj;

            // 그룹에서 회전이 적용되었을 때 마우스 위치가 잘잡히지 않는다. 아래 엔진을 적용해도 잘되지 않는다. MBSENGSCADA GS에서는 그룹의 회전을 뺐다. 2020-9-15
            //int sx, sy;
            //GetMousePositionByRotate(e, out sx, out sy);
            //MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, sx, sy, e.Delta);

            ObjectExpand oe;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                oe = (ObjectExpand)obj;

                //if (oe.enumObjectType == EnumObjectType.Group)
                //{
                //    if (oe.WmLeftButtonDown(form, e2))
                //        return true;
                //}
                //else
                //{
                    if (await oe.WmLeftButtonDown(form, e))
                        return true;
                //}
            }

            return false;
        }

        public override async Task<bool> WmLeftButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            if (await base.WmLeftButtonUp(form, e))
                return true; ;

            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                if (await ((ObjectExpand)obj).WmLeftButtonUp(form, e))
                    return true; ;

            }

            return false;
        }

        public override async Task<bool> WmRightButtonDown(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            if (!CheckResponseOnVisible()) return false;

            if (await base.WmRightButtonDown(form, e))
            {
                sSelectedTagName = "";  // 오른쪽 버튼을 눌렀을 때 태그의 컨텍스트 메뉴를 보여주지 않는다.
                return true;
            }

            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                    case EnumObjectType.Layer:
                        if (await ((ObjectPublicGroupLayer)obj).WmRightButtonDown(form, e))
                        {
                            sSelectedTagName = ((ObjectPublicGroupLayer)obj).sSelectedTagName;
                            eSelectedTagType = ((ObjectPublicGroupLayer)obj).eSelectedTagType;

                            return true;
                        }
                        break;
                    default:
                        if (await ((ObjectExpand)obj).WmRightButtonDown(form, e))
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

        public override async Task< bool> WmRightButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            if (await base.WmRightButtonUp(form, e))
                return true; ;

            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                if (await ((ObjectExpand)obj).WmRightButtonUp(form, e))
                    return true; ;

            }

            return false;
        }

        public override async Task<bool> WmMouseMove(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            if (await base.WmMouseMove(form, e))
                return true;


            int i;
            object obj;

            for (i = objectList.Count - 1; i >= 0; i--)
            {
                obj = objectList[i];

                if (await((ObjectExpand)obj).WmMouseMove(form, e))
                    return true;
            }

            return false;
        }

        public void DeleteOneObject(int pos)
        {
            objectList.RemoveAt(pos);
        }

        public void Remove(object obj)
        {
            objectList.Remove(obj);
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

            /*
            for (int i = 0; i < objectList.Count; i++)
            {
                if (pos[depth] == i)
                {
                    ObjectType type = (ObjectType)objectList[i];

                    if (pos.Length == depth + 1)
                    { // 마지막
                        object temp = obj;
                        obj = objectList[i];
                        objectList[i] = temp;
                    }
                    else
                    {
                        if (type.enumObjectType == EnumObjectType.Layer || type.enumObjectType == EnumObjectType.Group)
                        {
                            ((ObjectPublicGroupLayer)type).RecurseTempObject(ref obj, pos, depth+1);
                        }
                    }

                    return;
                }
            }*/
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

            //if (i >= objectList.Count)
            //    return false; // 배열 오버

            if (pos.Length == depth + 1)    // 마지막까지 들어왔다.
            {
                InsertObject(obj, i);
                return true;
                //return objectList[i];
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

            //if (i >= objectList.Count)
            //    return false; // 배열 오버

            if (pos_fr.Length == depth + 1)    // 마지막까지 들어왔다.
            {
                ChangePos(pos_fr[depth], pos_to[depth]);
                //InsertObject(obj, i);
                return true;
                //return objectList[i];
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
        }

        public override void GetMultiSelectTagList(ArrayList block)
        {
            int l;
            object obj;

            base.GetMultiSelectTagList(block);

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).GetMultiSelectTagList(block);

            }
        }

        public override void SetMultiSelectTagList(ArrayList block)
        {
            int l;
            object obj;

            base.SetMultiSelectTagList(block);

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).SetMultiSelectTagList(block);
            }
        }

        public override async Task EventTimerOnPreview(System.Windows.Forms.Form form)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                await ((ObjectExpand)obj).EventTimerOnPreview(form);

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
                    case EnumObjectType.Layer:

                        string title = ((ObjectPublicGroupLayer)obj).objGeneral.sOnStudioTitle;

                        if (title.Length == 0)
                        {
                            title = ((ObjectType)obj).enumObjectType.ToString();
                        }

                        TreeNode node = new TreeNode(title);
                        //parent.Nodes.Add(node);
                        parent.Nodes.Insert(0, node);

                        ((ObjectPublicGroupLayer)obj).AddObjectInfo(node);
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
                    case EnumObjectType.Layer:
                        ((ObjectPublicGroupLayer)obj).CallBackObject(type, call);
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
            base.SetCommonProperty(cp);  // Group인 경우 자신도 해야한다.

            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                ((ObjectExpand)obj).SetCommonProperty(cp);

            }
        }

        public bool ExecuteClassNameOnlyObject(Form form, string classname, string command, out object retn_value, params object[] args)
        {
            int l;
            object obj;
            bool retn = false;
            ObjectExpand expand;
            bool this_flag = false;
            retn_value = 0;

            string classname_except_this;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                        // 2023-8-10 추가 Gruop도 ObjectSetRect를 사용할 수 있도록 변경함. 
                        expand = (ObjectExpand)obj;
                       

                        // 문자열을 비교할 때 this.EditBox1, Edit 클래스를 비교할 때 StringNCompare를 사용하면 안된다. classname_except_this에 미리 문자열을 만들어서 비교해야 한다. 2022-8-3 추가
                        if (String.Compare(classname, 0, "this.", 0, 5) == 0)
                        {
                            this_flag = true;
                            classname_except_this = classname.Substring(5); // this. 제거된 클래스명
                        }
                        else
                        {
                            classname_except_this = classname;  // this. 제거된 클래스명
                        }
                        // ScriptFunctionObject.formParentScriptAction = 스크립트를 실행한 오브젝트의 Form this. 을 이용하여 ObjectSetText 같은 함수를 사용할 때 쓴다. 2022-8-3 추가.
                        if (this_flag && ScriptFunctionObject.formParentScriptAction == objCommonProperty.form &&
                            String.Compare(classname_except_this, expand.objGeneral.sClassName) == 0)//String.Compare(classname, 5, expand.objGeneral.sClassName, 0, expand.objGeneral.sClassName.Length) == 0)
                        {
                            retn = expand.ExecuteClassNameOnlyObject(command, out retn_value, args);
                            if (retn) return true;
                            break;
                        }

                        if (classname == expand.objGeneral.sClassName)
                        {
                            retn = expand.ExecuteClassNameOnlyObject(command, out retn_value, args);
                            if (retn) return true;
                            break;
                        }

                        retn = ((ObjectPublicGroupLayer)obj).ExecuteClassNameOnlyObject(form, classname, command, out retn_value, args);
                        if (retn) return true;
                        break;
                    case EnumObjectType.Layer:
                        retn = ((ObjectPublicGroupLayer)obj).ExecuteClassNameOnlyObject(form, classname, command, out retn_value, args);
                        if (retn) return true;
                        break;
                    case EnumObjectType.ControlTabControl:
                        retn = ((ObjectControlTabControl)obj).ExecuteClassNameOnlyObject(form, classname, command, out retn_value, args);
                        if (retn) return true;
                        // break를 하지않고 밑의 default를 실행한다. TabControl 스크립트를 실행한다.
                        goto case_default;
                    default:
                case_default:
                        expand = (ObjectExpand)obj;

                        //string classname_except_this;

                        // 문자열을 비교할 때 this.EditBox1, Edit 클래스를 비교할 때 StringNCompare를 사용하면 안된다. classname_except_this에 미리 문자열을 만들어서 비교해야 한다. 2022-8-3 추가
                        if (String.Compare(classname, 0, "this.", 0, 5) == 0)
                        {
                            this_flag = true;
                            classname_except_this = classname.Substring(5); // this. 제거된 클래스명
                        }
                        else
                        {
                            classname_except_this = classname;  // this. 제거된 클래스명
                        }
                        // ScriptFunctionObject.formParentScriptAction = 스크립트를 실행한 오브젝트의 Form this. 을 이용하여 ObjectSetText 같은 함수를 사용할 때 쓴다. 2022-8-3 추가.
                        if (this_flag && ScriptFunctionObject.formParentScriptAction == objCommonProperty.form &&
                            String.Compare(classname_except_this, expand.objGeneral.sClassName) == 0)//String.Compare(classname, 5, expand.objGeneral.sClassName, 0, expand.objGeneral.sClassName.Length) == 0)
                        {
                            retn = expand.ExecuteClassNameOnlyObject(command, out retn_value, args);
                            if (retn) return true;
                            break;
                        }

                        if (classname == expand.objGeneral.sClassName)
                        {
                            retn = expand.ExecuteClassNameOnlyObject(command, out retn_value, args);
                            if (retn) return true;
                        }

                        break;
                }
            }

            return retn;
        }

        // 현재는 스포이드에서만 사용한다.
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
                    case EnumObjectType.Layer:
                        retn = ((ObjectPublicGroupLayer)obj).GetObjectOfMouseZone(e);
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

        //--------------------------------------------------------
        // 내가 속한 그룹의 화면상의 좌표를 구한다.
        //--------------------------------------------------------

        public void SetZoneAtPercent100(SIZE sizeGroup, RECT rView)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                        {
                            ObjectGroup group = (ObjectGroup)obj;

                            RECT r = new RECT();
                            int p_sizex, p_sizey;

                            group.GetZone(ref r);

                            p_sizex = rView.right - rView.left + 1;
                            p_sizey = rView.bottom - rView.top + 1;

                            r.left = r.left * p_sizex / sizeGroup.cx + rView.left;
                            r.top = r.top * p_sizey / sizeGroup.cy + rView.top;
                            r.right = r.right * p_sizex / sizeGroup.cx + rView.left;
                            r.bottom = r.bottom * p_sizey / sizeGroup.cy + rView.top;

                            group.SetZoneAtPercent100(group.sizeGroup, r);

                            ((ObjectExpand)obj).SetZoneAtPercent100Expand(sizeGroup, rView);
                        }
                        break;
                    case EnumObjectType.Layer:
                        ((ObjectLayer)obj).SetZoneAtPercent100(sizeGroup, rView);
                        break;
                    default:
                        ((ObjectExpand)obj).SetZoneAtPercent100Expand(sizeGroup, rView);
                        break;
                }
            }
        }

        public void UpdateParentGroupLayer()
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                    case EnumObjectType.Layer:
                        {
                            ((ObjectExpand)obj).parentGroupLayer = this;
                            ((ObjectPublicGroupLayer)obj).UpdateParentGroupLayer();
                            break;
                        }
                    default:
                        ((ObjectExpand)obj).parentGroupLayer = this;
                        break;
                }
            }
        }

        public void GetMaxBorderThick(ref int thick)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                    case EnumObjectType.Layer:
                        {
                            ((ObjectPublicGroupLayer)obj).GetMaxBorderThick(ref thick);
                            break;
                        }
                    default:
                        {
                            int t = ((ObjectExpand)obj).GetBorderThick();
                            if (t > thick) thick = t;
                        }
                        break;
                }
            }
        }


        public override async Task MakeWebPublishFile(string target_dir)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                
                await ((ObjectExpand)obj).MakeWebPublishFile(target_dir);
            }
        }

        /// <summary>
        /// 그룹에 속한 요소를 다른 위치로 이동하거나 했을 때 그룹의 크기를 다시 정해야 하는 경우가 있다.
        /// </summary>
        public override void EditReCalcGroupSize()
        {
            int l;
            ObjectExpand obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = (ObjectExpand)objectList[l];

                obj.EditReCalcGroupSize();

                if (obj.enumObjectType == EnumObjectType.Group)
                {
                    ((ObjectGroup)obj).EditReCalcGroupRect(objCommonProperty.form);
                }
            }
        }

        public override void SetSameTagMouseZone(string tag, bool visible)
        {
            int l;
            ObjectExpand obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = (ObjectExpand)objectList[l];

                obj.SetSameTagMouseZone(tag, visible);
            }
        }

        public override void SaveAnimationFileToVersion3()
        {
            int i;
            ObjectExpand obj;

            for (i = 0; i < objectList.Count; i++)
            {
                obj = (ObjectExpand)objectList[i];

                obj.SaveAnimationFileToVersion3();
            }
        }
    }
}
