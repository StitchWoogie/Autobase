using System; 
using System.Collections;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TagAoClass.
	/// </summary>
	/// 
	[Serializable]
	public class TagGrClass : TagPublicClass
	{
		public ArrayList arrayTag = new ArrayList();
		public TagGrClass()
		{
			//
			// TODO: Add constructor logic here
			//
			this.enumTagType = EnumTagType.GR;
		}

        public override object CopyObjectOnStudio()
		{
            TagGrClass obj = (TagGrClass)this.MemberwiseClone();
			obj.arrayTag = new ArrayList();
			for(int i = 0; i < arrayTag.Count; i++) 
			{
                obj.arrayTag.Add(((TagPublicClass)(arrayTag[i])).CopyObjectOnStudio());
			}

			return obj;
		}


        /// <summary>
        /// tag.name = tag.tag를 해주어서 태그명과 같은 이름을 name에 가지고 있어야 추가가 된다.
        /// </summary>
        /// <param name="add_tag"></param>
        /// <param name="check_exists"></param>

		public void AddTag(TagPublicClass add_tag, bool check_exists)
		{
			if(add_tag.name.Length == 0)	return;	// 태그이름이 없으면 안된다.

			int index = add_tag.name.IndexOf('.');

			if(index == -1) 
			{
                // CSV파일 같은 데서 읽어 왔을 때는 체크를 해야한다. 복사하다보면 중복되는 경우가 많다
                if (check_exists)
                {
                    TagPublicClass pub;
                    for (int i = 0; i < arrayTag.Count; i++)
                    {
                        pub = (TagPublicClass)arrayTag[i];
                        if (String.Compare(add_tag.name, pub.name, true) == 0)
                        {
                            string msg = String.Format("같은 태그명이 중복 사용 되었습니다.\n중복된 태그명은 제외됩니다.\n태그명={0}", add_tag.tag);
                            System.Windows.Forms.MessageBox.Show(msg, "태그명 중복 사용 : " + add_tag.tag);
                            EventSave.Error(msg);

                            return;
                        }
                    }   
                }

                arrayTag.Add(add_tag);
			}
			else 
			{
				string group_name = add_tag.name.Substring(0, index);
				add_tag.name = add_tag.name.Substring(index+1); 

				TagPublicClass pub;
				for(int i = 0; i < arrayTag.Count; i++) 
				{
					pub = (TagPublicClass)arrayTag[i];
					if(String.Compare(group_name, pub.name, true) == 0)
					{
						if(pub.enumTagType == EnumTagType.GR)
							((TagGrClass)pub).AddTag(add_tag, check_exists);
						else // 이런 경우는 구버전에서 앞의 태그는 AI_0000으로 사용하고 뒤의 태그는 AI_0000.KKK 식으로 사용한 경우
						{
							string msg = String.Format("앞의 태그에서 그룹명이 일반 태그명으로 사용되었기 때문에 태그로 추가할 수 없습니다.\n해당 태그는 사용할 수 없으므로 수동으로 추가해 주시기 바랍니다.\n이 메시지는 이벤트 뷰어에 저장됩니다.\n\n사용 불가 태그={0}", add_tag.tag);
							System.Windows.Forms.MessageBox.Show(msg, "태그 로딩 시 태그명 오류 : " + add_tag.tag);
							EventSave.Error(msg);
						}
						return;
					}
				}

				// 만들어진 그룹이 없을 경우
				TagGrClass gr = new TagGrClass();
				gr.name = group_name;
				if(this.tag == null || this.tag.Length == 0)
				{
					gr.tag = group_name;
				}
				else 
				{
					gr.tag = this.tag+"."+group_name;
				}

				gr.AddTag(add_tag, check_exists);
				arrayTag.Add(gr);
			}
		}
	}
}
