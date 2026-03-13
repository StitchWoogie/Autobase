using System;
using System.Collections.Generic;
using NetTools;
using System.IO;

namespace AutoLibLocal
{
	public class TagListStruct 
	{
		public string tag;
		public int[] tag_pos;
		public EnumTagType type;
	}

	/// <summary>
	/// Summary description for TagLib.
	/// </summary>
	public class TagLib
	{
		public const int DEFAULT_SCANTIME = 500;
		public static bool bInit = false;

		public static int TAG_NOT_FOUND = -1;	// 태그가 없다는 표시이다. 이 ID는 -범위에서 계속 변경될 수 있다.
		// 값이 변경되는 이유는 태그가 수정되었을 때 없는 이름이 다시 존재할 수 있으므로 다시 읽을 수 있도록 한다.

		public static TagListStruct[] tagListAll;
		public static bool bChangedByLocalMain = false;	// LocalMain에서 태그속성이 바뀌었다.

		public static TagGrClass groupRoot = new TagGrClass();

		// TerminalClass에서 불리어진다.
		public static void InitByTerminalClass()
		{
			bInit = true;

            ReMakeTagList();
		}

        /// <summary>
        /// 태그편집기 등에서 태그를 추가하거나 삭제, 이름변경 등으 변화가 있을 때 TAG_NOT_FOUND를 바꾸어야 태그 없음에서 다시 태그를 찾는다.
        /// </summary>
        public static void ChangeTagNotFound()
        {
            if (TAG_NOT_FOUND <= -9999)
            {
                TAG_NOT_FOUND = -1;
            }
            else
            {
                TAG_NOT_FOUND--;
            }
        }

        /// <summary>
        /// 태그를 추가하거나 삭제하고 난후 이 부분을 불러주어야 전체 태그리스트가 업데이트 된다. 
        /// </summary>
        public static void ReMakeTagList()
        {
            tagListAll = MakeTagList();
        }

		/// <summary>
		/// 준비 되거나 준비되지 않은 배열에 Pos를 복사한다.
		/// </summary>
		/// <param name="poss"></param>
		/// <param name="depth"></param>
		/// <param name="add"></param>
		static void AddPos(ref int[] poss, int depth, int add)
		{
			if(poss == null) 
			{
				poss = new int[1];
			}

			if(depth >= poss.Length) 
			{
				int[] imsi = new int[depth+1];
				Array.Copy(poss, imsi, depth);
				imsi[depth] = add;
				poss = imsi;
			}
			else 
			{
				poss[depth] = add;
			}
		}

		static bool RecurseGetTagPosPublic(TagGrClass gr, string tag, ref int[] pos, int depth)
		{
			string group_name;
			int index = tag.IndexOf('.');
			
			if(index == -1)	
				group_name = tag;
			else
				group_name = tag.Substring(0, index);

			TagPublicClass tp;

			for(int i = 0; i < gr.arrayTag.Count; i++) 
			{
				tp = (TagPublicClass)gr.arrayTag[i];

				if(String.Compare(tp.name, group_name, StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					if(index == -1) 
					{
						AddPos(ref pos, depth, i);
						return true;
					}
					else 
					{
						if(tp.enumTagType == EnumTagType.GR) 
						{
							AddPos(ref pos, depth, i);
							return RecurseGetTagPosPublic((TagGrClass)tp, tag.Substring(index+1), ref pos, depth+1);
						}
						AddPos(ref pos, 0, TAG_NOT_FOUND);
						return false;
					}
				}
			}

			AddPos(ref pos, 0, TAG_NOT_FOUND);
			return false;	// 태그를 찾지 못함
		}

		// 위의 Method와 기능은 모두 같지만 찾았을 때 TagPublicClass의 ref를 알려준다.
		static bool RecurseGetTagPosPublic(TagGrClass gr, string tag, ref int[] pos, int depth, out TagPublicClass tp)
		{
			string group_name;
			int index = tag.IndexOf('.');
			
			if(index == -1)	
				group_name = tag;
			else
				group_name = tag.Substring(0, index);

			for(int i = 0; i < gr.arrayTag.Count; i++) 
			{
				tp = (TagPublicClass)gr.arrayTag[i];

                if (String.Compare(tp.name, group_name, StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					if(index == -1) 
					{
						AddPos(ref pos, depth, i);
						return true;
					}
					else 
					{
						if(tp.enumTagType == EnumTagType.GR) 
						{
							AddPos(ref pos, depth, i);
							return RecurseGetTagPosPublic((TagGrClass)tp, tag.Substring(index+1), ref pos, depth+1, out tp);
						}
						AddPos(ref pos, 0, TAG_NOT_FOUND);
						return false;
					}
				}
			}

			tp = null;
			AddPos(ref pos, 0, TAG_NOT_FOUND);
			return false;	// 태그를 찾지 못함
		}

		static bool GetTagPosPublic(string tag, ref int[] pos)
		{
			return RecurseGetTagPosPublic(groupRoot, tag, ref pos, 0);
		}

		

		// 위의 Method와 기능은 모두 같지만 찾았을 때 TagPublicClass의 ref를 알려준다.
		static bool RecurseGetTagTypePosMember(TagGrClass gr, string tag, out string tag_name, out EnumTagType type, ref int[] pos, out EnumTagMember member, out EnumTagMemberVar var_type, int depth, out TagPublicClass tp)
		{
			string group_name;
			int index = tag.IndexOf('.');
			
			if(index == -1)	
				group_name = tag;
			else
				group_name = tag.Substring(0, index);

			for(int i = 0; i < gr.arrayTag.Count; i++) 
			{
				tp = (TagPublicClass)gr.arrayTag[i];

                if (String.Compare(tp.name, group_name, StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					type = tp.enumTagType;
					if(index == -1) 
					{
						AddPos(ref pos, depth, i);
						member = EnumTagMember.TAG_MEMBER_curr;
						if(tp.enumTagType == EnumTagType.ST)
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						else
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						tag_name = tp.tag;
						return true;
					}
					else 
					{
						if(tp.enumTagType == EnumTagType.GR) 
						{
							AddPos(ref pos, depth, i);
							return RecurseGetTagTypePosMember((TagGrClass)tp, tag.Substring(index+1), out tag_name, out type, ref pos, out member, out var_type, depth+1, out tp);
						}

						string member_name = tag.Substring(index);

                        if (String.Compare(member_name, ".value", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_curr;
							if(tp.enumTagType == EnumTagType.ST)
								var_type = EnumTagMemberVar.MEMBER_VAR_string;
							else
								var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".tag", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_tag;
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						}
                        else if (String.Compare(member_name, ".name", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_name;
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						}
                        else if (String.Compare(member_name, ".des", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_description;
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						}
                        else if (String.Compare(member_name, ".unit", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_unit;
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						}
                        else if (String.Compare(member_name, ".desON", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_desON;
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						}
                        else if (String.Compare(member_name, ".desOFF", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_desOFF;
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						}
                        else if (String.Compare(member_name, ".port", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_port;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".station", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_station;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".address", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_address;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".extra1", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_extra1;
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						}
                        else if (String.Compare(member_name, ".extra2", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_extra2;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".hihi", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_hihi;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".high", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_high;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".low", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_low;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".lolo", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_lolo;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".active", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_act;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".full", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_full;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".base", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_base;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".plcfull", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_plc_full;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".plcbase", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_plc_base;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".viewfull", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_viewfull;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".viewbase", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_viewbase;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".assign", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_assign;
							var_type = EnumTagMemberVar.MEMBER_VAR_string;
						}
                        else if (String.Compare(member_name, ".NeedAlarmConfirm", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_NeedAlarmConfirm;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".protectscan", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_ProtectScan;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".protectcontrol", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_ProtectControl;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".ProtectAlarmEvent", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_ProtectAlarmEvent;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".ProtectAlarmData", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_ProtectAlarmData;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".SumTotal", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_fSumTotal;
							var_type = EnumTagMemberVar.MEMBER_VAR_double;
						}
                        else if (String.Compare(member_name, ".SumPart", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_fSumPart;
							var_type = EnumTagMemberVar.MEMBER_VAR_double;
						}
                        else if (String.Compare(member_name, ".AlarmLevelStatus", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_cAlarmLevelStatus;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
                        else if (String.Compare(member_name, ".Format", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_fDisplayFormat;
							var_type = EnumTagMemberVar.MEMBER_VAR_float;
						}
                        else if (String.Compare(member_name, ".alarm", StringComparison.CurrentCultureIgnoreCase) == 0) 
						{
							member = EnumTagMember.TAG_MEMBER_alarm;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
						}
						else 
						{
							tag_name = "";
							member = EnumTagMember.TAG_MEMBER_curr;
							var_type = EnumTagMemberVar.MEMBER_VAR_int;
							AddPos(ref pos, 0, TAG_NOT_FOUND);
							
							return false;
						}
                        				
						tag_name = tp.tag;
						return true;
					}
				}
			}

			tp = null;
			tag_name = "";
			member = EnumTagMember.TAG_MEMBER_curr;
			var_type = EnumTagMemberVar.MEMBER_VAR_int;
			type = EnumTagType.none;
			AddPos(ref pos, 0, TAG_NOT_FOUND);

			return false;	// 태그를 찾지 못함
		}

		public static bool GetTagTypePosMember(string tag_org, out string tag_name, out EnumTagType type, ref int[] pos, out EnumTagMember member, out EnumTagMemberVar var_type, out TagPublicClass tp)
		{
			return RecurseGetTagTypePosMember(groupRoot, tag_org, out tag_name, out type, ref pos, out member, out var_type, 0, out tp);
		}

		static TagAiClass TagNotFoundAI(string tag)
		{
			TagAiClass ai;

			ai = new TagAiClass();
			ai.tag = tag;
			ai.description = "Tag not found";
			return ai;
		}

		public static TagAiClass GetStructAI(string tag, ref int[] nTagPos)
		{
			TagPublicClass tp = GetStructPublic(tag, ref nTagPos);
			if(tp.enumTagType == EnumTagType.AI) 
			{
				return (TagAiClass)tp;
			}
			else 
			{
				return TagNotFoundAI(tag);
			}
		}

		public static TagAoClass TagNotFoundAO(string tag)
		{
			TagAoClass ao;

			ao = new TagAoClass();
			ao.tag = tag;
			ao.description = "Tag not found";
			return ao;
		}

		public static TagAoClass GetStructAO(string tag, ref int[] nTagPos)
		{
			TagPublicClass tp = GetStructPublic(tag, ref nTagPos);
			if(tp.enumTagType == EnumTagType.AO) 
			{
				return (TagAoClass)tp;
			}
			else 
			{
				return TagNotFoundAO(tag);
			}
		}
        
		public static TagDiClass TagNotFoundDI(string tag)
		{
			TagDiClass di;

			di = new TagDiClass();
			di.tag = tag;
			di.description = "Tag not found";
			return di;
		}

		public static TagDiClass GetStructDI(string tag, ref int[] nTagPos)
		{
			TagPublicClass tp = GetStructPublic(tag, ref nTagPos);
			if(tp.enumTagType == EnumTagType.DI) 
			{
				return (TagDiClass)tp;
			}
			else 
			{
				return TagNotFoundDI(tag);
			}
		}

		public static TagDoClass TagNotFoundDO(string tag)
		{
			TagDoClass dout;

			dout = new TagDoClass();
			dout.tag = tag;
			dout.description = "Tag not found";
			return dout;
		}

		public static TagDoClass GetStructDO(string tag, ref int[] nTagPos)
		{
			TagPublicClass tp = GetStructPublic(tag, ref nTagPos);
			if(tp.enumTagType == EnumTagType.DO) 
			{
				return (TagDoClass)tp;
			}
			else 
			{
				return TagNotFoundDO(tag);
			}
		}


		public static TagDoGroupClass TagNotFoundDoGroup(string tag)
		{
			TagDoGroupClass dout;

			dout = new TagDoGroupClass();
			dout.tag = tag;
			dout.description = "Tag not found";
			return dout;
		}

		public static TagDoGroupClass GetStructDoGroup(string tag, ref int[] nTagPos)
		{
			TagPublicClass tp = GetStructPublic(tag, ref nTagPos);
			if(tp.enumTagType == EnumTagType.GDO) 
			{
				return (TagDoGroupClass)tp;
			}
			else 
			{
				return TagNotFoundDoGroup(tag);
			}
		}

		public static TagStClass TagNotFoundST(string tag)
		{
			TagStClass st;

			st = new TagStClass();
			st.tag = tag;
			st.description = "Tag not found";
			return st;
		}

		public static TagStClass GetStructST(string tag, ref int[] nTagPos)
		{
			TagPublicClass tp = GetStructPublic(tag, ref nTagPos);
			if(tp.enumTagType == EnumTagType.ST) 
			{
				return (TagStClass)tp;
			}
			else 
			{
				return TagNotFoundST(tag);
			}
		}

		static TagPublicClass TagNotFoundPublic(string tag)
		{
			TagPublicClass pub;

			pub = new TagPublicClass();
			pub.tag = tag;
			pub.description = "Tag not found";
			return pub;
		}

		static TagPublicClass RecurseGetStructFromReadyPos(TagGrClass gr, string tag, ref int[] pos, int depth)
		{
			TagPublicClass tp;

			if(pos == null) 
			{
				if(!RecurseGetTagPosPublic(gr, tag, ref pos, depth, out tp))
					return TagNotFoundPublic(tag);
				return tp;
			}

			if(pos[0] == TAG_NOT_FOUND) 
			{
				return TagNotFoundPublic(tag);
			}

			if(tag == null || tag.Length == 0) 
			{
				pos[0] = TAG_NOT_FOUND;
				return TagNotFoundPublic(tag);
			}

			if(depth >= pos.Length) // 깊이보다 배열이 작다
			{
				if(!RecurseGetTagPosPublic(gr, tag, ref pos, depth, out tp))
					return TagNotFoundPublic(tag);			
		
				return tp;
			}

			int index = tag.IndexOf('.');
            
			string group_name;
			if(index == -1) 
			{
				group_name = tag;
			}
			else
				group_name = tag.Substring(0, index);

			// 태그가 삭제 되거나 다시 찾기 명령이 실행 되었다.
			if(pos[depth] < 0 || pos[depth] >= gr.arrayTag.Count)
			{
				pos[depth] = 0;
			}

            // 태그를 읽지 않은 경우이거나 그룹속의 태그가 없을 때 갯수가 없다.
            if (gr.arrayTag.Count == 0)
            {
                pos[0] = TAG_NOT_FOUND; // 9.3.4 에서 추가
                return TagNotFoundPublic(tag);
            }

			tp = (TagPublicClass)gr.arrayTag[pos[depth]];

			if(tp.name == group_name)
			{ 
				if(tp.enumTagType == EnumTagType.GR) 
				{
					return RecurseGetStructFromReadyPos((TagGrClass)tp, tag.Substring(index+1), ref pos, depth+1);
				}
				else 
				{
					return tp;
				}
			}
			else {
				// 다시 찾아준다.
				if(!RecurseGetTagPosPublic(gr, tag, ref pos, depth, out tp))
					return TagNotFoundPublic(tag);

				return tp;
			}
		}

		public static TagPublicClass GetStructPublic(string tag, ref int[] nTagPos)
		{
			return RecurseGetStructFromReadyPos(groupRoot, tag, ref nTagPos, 0);
		}

		public static TagPublicClass GetStructPublic(TagGrClass group_root, string tag, ref int[] nTagPos)
		{
			return RecurseGetStructFromReadyPos(group_root, tag, ref nTagPos, 0);
		}

        public static bool IsTagExist(string tag)
        {
            int[] pos = new int[1];

            TagPublicClass tp = GetStructPublic(tag, ref pos);

            if (pos[0] == TAG_NOT_FOUND) return false;
            return true;
        }

        public static bool GetTagTypeAndPos(string tag, ref EnumTagType type, ref int[] pos)
		{
			TagPublicClass tp = GetStructPublic(tag, ref pos);

			if(pos[0] == TAG_NOT_FOUND) 
			{
				type = tp.enumTagType;
				return false;
			}
			else 
			{
				type = tp.enumTagType;
				return true;
			}
		}

		public static bool GetTagPosAI(string tag, ref int[] pos)
		{
			EnumTagType type = 0;

			if(!GetTagTypeAndPos(tag, ref type, ref pos))	return false;
			if(type != EnumTagType.AI)	return false;

			return true;
		}

		public static bool GetTagPosAO(string tag, ref int[] pos)
		{
			EnumTagType type = 0;

			if(!GetTagTypeAndPos(tag, ref type, ref pos))	return false;
			if(type != EnumTagType.AO)	return false;

			return true;
		}

		public static bool GetTagPosDI(string tag, ref int[] pos)
		{
			EnumTagType type = 0;

			if(!GetTagTypeAndPos(tag, ref type, ref pos))	return false;
			if(type != EnumTagType.DI)	return false;

			return true;
		}

		public static bool GetTagPosDO(string tag, ref int[] pos)
		{
			EnumTagType type = 0;

			if(!GetTagTypeAndPos(tag, ref type, ref pos))	return false;
			if(type != EnumTagType.DO)	return false;

			return true;
		}

		public static bool GetTagPosST(string tag, ref int[] pos)
		{
			EnumTagType type = 0;

			if(!GetTagTypeAndPos(tag, ref type, ref pos))	return false;
			if(type != EnumTagType.ST)	return false;

			return true;
		}

		static void RecurseGetTagList(TagGrClass gr, List<object> array, EnumTagType type, int depth, ref int[] pos)
		{
			TagPublicClass tp;

			for(int i = 0; i < gr.arrayTag.Count; i++) 
			{
				tp = (TagPublicClass)gr.arrayTag[i];
				if(tp.enumTagType == EnumTagType.GR) 
				{
					AddPos(ref pos, depth, i);
					RecurseGetTagList((TagGrClass)tp, array, type, depth+1, ref pos); 
				}
				else if(tp.enumTagType == type) 
				{
					AddPos(ref pos, depth, i);

					TagListStruct list = new TagListStruct();

					list.tag = tp.tag;
					list.tag_pos = new int[depth+1];
					list.type = tp.enumTagType;
					Array.Copy(pos, list.tag_pos, depth+1);

					array.Add(list);
				}
				else {}
			}		
		}

        /*
		public static TagListStruct[] GetTagList(EnumTagType type)
		{
			ArrayList array = new ArrayList();
			int[] pos = new int[1];
			RecurseGetTagList(groupRoot, array, type, 0, ref pos);
			
			TagListStruct[] list;

			if(array.Count == 0)	return null;

			list = new TagListStruct[array.Count];

			for(int i = 0; i < array.Count; i++) 
			{
				list[i] = (TagListStruct)array[i];
			}	

			return list;
		}*/

		static void RecurseMakeTagList(TagGrClass gr, List<object> array, int depth, ref int[] pos)
		{
			TagPublicClass tp;

			for(int i = 0; i < gr.arrayTag.Count; i++) 
			{
				tp = (TagPublicClass)gr.arrayTag[i];
				if(tp.enumTagType == EnumTagType.GR) 
				{
					AddPos(ref pos, depth, i);
					RecurseMakeTagList((TagGrClass)tp, array, depth+1, ref pos);
				}
				else 
				{
					AddPos(ref pos, depth, i);

					TagListStruct list = new TagListStruct();

					list.tag = tp.tag;
					list.tag_pos = new int[depth+1];
					list.type = tp.enumTagType;
					Array.Copy(pos, list.tag_pos, depth+1);

					array.Add(list);
				}
			}		
		}

        public static TagListStruct[] MakeTagList(TagGrClass gr)
        {
            List<object> array = new List<object>();
            int[] pos = new int[1];
            RecurseMakeTagList(gr, array, 0, ref pos);

            TagListStruct[] list;

            if (array.Count == 0) return new TagListStruct[0];// return null;

            list = new TagListStruct[array.Count];

            for (int i = 0; i < array.Count; i++)
            {
                list[i] = (TagListStruct)array[i];
            }

            return list;
        }

		public static TagListStruct[] MakeTagList()
		{
            return MakeTagList(groupRoot);
		}

		static int RecurseGetTagListCount(TagGrClass gr)
		{
			TagPublicClass tp;

			int count = 0;

			for(int i = 0; i < gr.arrayTag.Count; i++) 
			{
				tp = (TagPublicClass)gr.arrayTag[i];
				if(tp.enumTagType == EnumTagType.GR) 
				{
					count += RecurseGetTagListCount((TagGrClass)tp);
				}
				else 
				{
					count ++;
				}
			}		

			return count;
		}

		/// <summary>
		/// 이 함수는 태그로딩후 그룹태그가 아닌 실제 태그수가 몇개나 존재하는지 알아보는 함수이다. 속도를 요구하는 
		/// 함수에서는 사용을 하지 않도록
		/// </summary>
		/// <returns></returns>
		public static int GetTagListCount()
		{
			return RecurseGetTagListCount(groupRoot);
		}

        /*
		public static int GetTagPosOnlyList(TagListStruct[] list, string tag)
		{
			for(int i = 0; i < list.Length; i++) 
			{
				if(String.Compare(tag, list[i].tag, true) == 0)
					return i;
			}

			return -1;
		}

		
        public static void RecurseGroupTree(TreeNode tree, List<object> array)
		{
			TagPublicClass pub;
			for(int i = 0; i < array.Count; i++) 
			{
				pub = (TagPublicClass)array[i];
				if(pub.enumTagType == EnumTagType.GR) 
				{
					TreeNode node = new TreeNode(pub.name);
                    node.ToolTipText = pub.description;
					tree.Nodes.Add(node);
					RecurseGroupTree(node, ((TagGrClass)pub).arrayTag);
				}
			}
		}

		public static void FillGroupTree(TagGrClass tag_root, TreeView tree)
		{
			TreeNode node = new TreeNode("Local");
			tree.Nodes.Add(node);
			RecurseGroupTree(node, tag_root.arrayTag);
		}

		public static void FillGroupTree(TreeView tree)
		{
			FillGroupTree(groupRoot, tree);
		}*/

		static TagGrClass RecurseGetGroupArray(TagGrClass root, string name)
		{
			int index = name.IndexOf('.');
			string group_name;

			if(index == -1) 
			{
				group_name = name;
			}
			else 
			{
				group_name = name.Substring(0, index);
			}

			TagPublicClass pub;
			for(int i = 0; i < root.arrayTag.Count; i++) 
			{
				pub = (TagPublicClass)root.arrayTag[i];
                if (String.Compare(group_name, pub.name, StringComparison.CurrentCultureIgnoreCase) == 0) 
				{
					if(pub.enumTagType == EnumTagType.GR) 
					{
						if(index == -1) 
						{
							return (TagGrClass)pub;
						}
						else 
						{
							return RecurseGetGroupArray((TagGrClass)pub, name.Substring(index+1));
						}
					}
					else 
					{
						return new TagGrClass();
					}
				}
			}
			return new TagGrClass();
		}

		public static TagGrClass GetGroupArray(TagGrClass group_root, string name)
		{
			if(name.Length == 0)	// root group을 준다.
			{
				return group_root;
			}

			return RecurseGetGroupArray(group_root, name);
		}

		public static TagGrClass GetGroupArray(string name)
		{
			return GetGroupArray(groupRoot, name);
		}

		public static void GetTagValue(string tag, out string sValue, out double fValue)
		{
            /*
            if (ConfigVarTotal.bRunByWebService)
            {
                ConfigVarTotal.GetTagValueOnWebService(tag, out sValue);
                fValue = ConvertTool.ToDouble(sValue);
                return;
            }*/

			int[] pos = new int[1];
			TagPublicClass tp = GetStructPublic(tag, ref pos);
			
			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				fValue = ai.curr;			
				sValue = ai.curr.ToString();
			}
			else if(tp.enumTagType == EnumTagType.AO) 
			{
				TagAoClass ao = (TagAoClass)tp;;
				fValue = ao.curr;			
				sValue = ao.curr.ToString();
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				TagDiClass di = (TagDiClass)tp;
				fValue = di.curr;			
				sValue = di.curr.ToString();
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
				TagDoClass dout = (TagDoClass)tp;
				fValue = dout.curr;			
				sValue = dout.curr.ToString();
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				TagStClass st = (TagStClass)tp;
				
				sValue = st.curr;
				try 
				{
					if(sValue.Length == 0)	fValue = 0;
					else					fValue = ConvertTool.ToDouble(sValue);
				}
				catch 
				{
					fValue = 0;
				}
			}
			else 
			{
				sValue = "";
				fValue = 0;
			}
		}

		public static TagGrClass GetRootGroup()
		{
			return groupRoot;
		}

		public static TagDiClass GetDirectTag(TagDiClass di)
		{
			if(di.cTagLinkType != 3)	return di;					// 간접태그가 아니면

			if(di.assign == null) return di;
			if(di.assign.pos[0] == TAG_NOT_FOUND)	return di;	// assign 되어 있지 않다.
            
			return GetStructDI(di.assign.tag, ref di.assign.pos);
		}

		public static TagDoClass GetDirectTag(TagDoClass dout)
		{
			if(dout.cTagLinkType != 3)	return dout;					// 간접태그가 아니면

			if(dout.assign == null)	return dout;
			if(dout.assign.pos[0] == TAG_NOT_FOUND)	return dout;	// assign 되어 있지 않다.
            
			return GetStructDO(dout.assign.tag, ref dout.assign.pos);
		}

		public static TagAiClass GetDirectTag(TagAiClass ai)
		{
			if(ai.cTagLinkType != 3)		return ai;					// 간접태그가 아니면

			if(ai.assign == null)	return ai;
			if(ai.assign.pos[0] == TAG_NOT_FOUND)	return ai;	// assign 되어 있지 않다.
            
			return GetStructAI(ai.assign.tag, ref ai.assign.pos);
		}

		public static TagAoClass GetDirectTag(TagAoClass ao)
		{
			if(ao.cTagLinkType != 3)	return ao;					// 간접태그가 아니면

			if(ao.assign == null)	return ao;
			if(ao.assign.pos[0] == TAG_NOT_FOUND)	return ao;	// assign 되어 있지 않다.
            
			return GetStructAO(ao.assign.tag, ref ao.assign.pos);
		}

		public static void MakeDefaultTag()
		{
			CommaTextReader comma = new CommaTextReader();
			TagAiClass ai = new TagAiClass();
			comma.Set("AI_0000,Sample Analog Input");
			TagFile.LoadToAI(ai, comma);
			TagLib.groupRoot.AddTag(ai, false);

			TagAoClass ao = new TagAoClass();
			comma.Set("AO_0000,Sample Analog Output");
			TagFile.LoadToAO(ao, comma);
            TagLib.groupRoot.AddTag(ao, false);

			TagDiClass di = new TagDiClass();
			comma.Set("DI_0000,Sample Digital Input");
			TagFile.LoadToDI(di, comma);
            TagLib.groupRoot.AddTag(di, false);

			TagDoClass dout = new TagDoClass();
			comma.Set("DO_0000,Sample Digital Output");
			TagFile.LoadToDO(dout, comma);
            TagLib.groupRoot.AddTag(dout, false);

			TagStClass st = new TagStClass();
			comma.Set("ST_0000,Sample String Tag");
			TagFile.LoadToST(st, comma);
            TagLib.groupRoot.AddTag(st, false);
		}

        /*
		static bool LocalTagLoadNewFormat()
		{
			string filename = String.Format("{0}\\TAG\\Local.tagx", TotalConfig.sDirWorkProject);

			if(!File.Exists(filename))	return false;

			TagFile tagfile = new TagFile();

			if(tagfile.LoadTag(filename, TagLib.groupRoot, System.Text.Encoding.UTF8, false) == false)	return true;	// 파일은 존재하나 읽을 수 없을 때는 구버전을 읽지 않는다.

			return true;	
		}

		/// <summary>
		/// terminal class의 tagloadall은 웹클라이언트, 로컬 겸용이고,
		/// 이 함수는 로컬의 태그만 로딩할 수 있는 함수이다.
		/// </summary>
		public static void LocalTagLoad()
		{
			if(!LocalTagLoadNewFormat()) // 파일이 존재하지 않을때만 구버전을 읽는다.
			{
				TagFileOld load = new TagFileOld();
				string path;

				path = String.Format("{0}\\TAG\\AI.TAG", TotalConfig.sDirWorkProject);
				load.TagLoadAI(path);
				path = String.Format("{0}\\TAG\\AO.TAG", TotalConfig.sDirWorkProject);
				load.TagLoadAO(path);
				path = String.Format("{0}\\TAG\\DI.TAG", TotalConfig.sDirWorkProject);
				load.TagLoadDI(path);
				path = String.Format("{0}\\TAG\\DO.TAG", TotalConfig.sDirWorkProject);
				load.TagLoadDO(path);
				path = String.Format("{0}\\TAG\\ST.TAG", TotalConfig.sDirWorkProject);
				load.TagLoadST(path);
				path = String.Format("{0}\\TAG\\do-goup.TAG", TotalConfig.sDirWorkProject);
				load.TagLoadDoGroup(path);
			}

			// 태그가 없을때는 프로그램의 원할한 구조를 위해 기본 태그를 만든다.
			if(TagLib.GetTagListCount() == 0) // TagLib.groupRoot.arrayTag.Count == 0 을 사용하면 비어있는 그룹 태그만 있을 때 프로그램이 전체적으로 다운된다.
			{
				TagLib.MakeDefaultTag();
			}

            bInit = true;   // Local에서 읽었더라도 태그를 로딩하면 플래그를 살려준다.
		}

        public static double GetTagMemberFull(string tag)
        {
            if (!TagLib.bInit)
            {
                TagLib.LocalTagLoad();
            }

            int[] pos = new int[1];
            TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);

            if (tp.enumTagType == EnumTagType.AI)
                return ((TagAiClass)tp).fFull;
            else
                return 100;
        }*/
		
	}
}
