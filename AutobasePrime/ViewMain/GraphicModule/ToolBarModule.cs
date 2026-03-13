using System;
using GraphicModule;
using System.Collections;
using AutoLib;
using System.Drawing;
using System.IO;
using NetTools;
using System.Windows.Forms;
using AutoLibLocal;

namespace GraphicModule
{
    public class TOOLBAR_MODULE_LIST
    {
        public FormGraphicChild form;
        public int position;
        public int size;
        public string filename;
    }

    /// <summary>
    /// Summary description for ToolBarModule.
    /// </summary>
    public class ToolBarModule
    {
        public static ArrayList listToolBar = new ArrayList();

        public ToolBarModule()
        {
        }


        /// <summary>
        /// 리사이즈 드래그 중 모든 툴바모듈의 그래픽 갱신 억제
        /// </summary>
        public static void SuspendAllGraphicUpdate()
        {
            for (int l = 0; l < listToolBar.Count; l++)
            {
                TOOLBAR_MODULE_LIST list = (TOOLBAR_MODULE_LIST)listToolBar[l];
                list.form?.SuspendGraphicUpdate();
            }
        }

        /// <summary>
        /// 리사이즈 드래그 종료 후 모든 툴바모듈의 그래픽 갱신 재개
        /// </summary>
        public static void ResumeAllGraphicUpdate()
        {
            for (int l = 0; l < listToolBar.Count; l++)
            {
                TOOLBAR_MODULE_LIST list = (TOOLBAR_MODULE_LIST)listToolBar[l];
                list.form?.ResumeGraphicUpdate();
            }
        }

        public static void LoadList()
        {
            listToolBar = new ArrayList();

            TextReader reader;
            string path;
            TOOLBAR_MODULE_LIST list;
            string buf = "";
            CommaBlockString comma = new CommaBlockString();

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                reader = MakeFilePath.OpenOldNew("ToolBar", "ToolBar.lst", "ToolBar.lstx");
            }
            else
            {
                path = MakeFilePath.Project("ToolBar", "ToolBar.lstx");

                if (File.Exists(path))
                {
                    reader = new StreamReader(path);
                }
                else
                {
                    path = MakeFilePath.Project("ToolBar", "ToolBar.lst");
                    if (File.Exists(path))
                    {
                        if (Tools.IsLangKorean())
                        {
                            if (MessageBox.Show("이전 버전의 툴바 윈도우 설정이 존재합니다.\n이전 버전의 데이터를 복사하겠습니까?", "이전버전 확인", MessageBoxButtons.YesNo)
                                != DialogResult.Yes) return;
                        }
                        else
                        {
                            if (MessageBox.Show("ToolBar file of old version is already exist.\nUse old version?", "Old version exist", MessageBoxButtons.YesNo)
                                != DialogResult.Yes) return;
                        }

                        reader = new StreamReader(path, System.Text.Encoding.Default);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (reader == null) return;
            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;
                comma.Set(buf);

                list = new TOOLBAR_MODULE_LIST();
                comma.GetInt(ref list.position);
                comma.GetInt(ref list.size);
                comma.GetString(ref list.filename);

                listToolBar.Add(list);
            }

            reader.Close();
        }

        public static void FrameToolBarCreate(Form mainform)
        {
            TOOLBAR_MODULE_LIST list;
            int l;

            string filename;

            for (l = 0; l < listToolBar.Count; l++)
            {
                list = (TOOLBAR_MODULE_LIST)listToolBar[l];

                filename = MakeFilePath.Graphic(list.filename);
                list.form = new FormGraphicChild(filename);

                list.form.FormBorderStyle = FormBorderStyle.None;
                list.form.TopLevel = false;

                list.form.WindowState = FormWindowState.Normal;
                list.form.Size = new Size(0, 0);
                if (list.position == 0) list.form.Dock = DockStyle.Left;
                else if (list.position == 1) list.form.Dock = DockStyle.Top;
                else if (list.position == 2) list.form.Dock = DockStyle.Right;
                else list.form.Dock = DockStyle.Bottom;

                TotalConfig.formMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																							 list.form});

                TotalConfig.formMain.Controls.SetChildIndex(list.form, 0);
            }

            MoveFrameToolBar(mainform);

            for (l = 0; l < listToolBar.Count; l++)
            {
                list = (TOOLBAR_MODULE_LIST)listToolBar[l];
                list.form.AutoScroll = false;
                list.form.Show();
            }
        }

        public static void AddOneToolBar(string file, int position, int size)
        {
            TOOLBAR_MODULE_LIST list = new TOOLBAR_MODULE_LIST();

            list.position = position;
            list.size = size;
            list.filename = file;

            string filename = MakeFilePath.Graphic(list.filename);
            list.form = new FormGraphicChild(filename);
            //
            list.form.FormBorderStyle = FormBorderStyle.None;
            list.form.TopLevel = false;

            list.form.WindowState = FormWindowState.Normal;
            list.form.Size = new Size(0, 0);

            if (list.position == 0) list.form.Dock = DockStyle.Left;
            else if (list.position == 1) list.form.Dock = DockStyle.Top;
            else if (list.position == 2) list.form.Dock = DockStyle.Right;
            else list.form.Dock = DockStyle.Bottom;

            TotalConfig.formMain.Controls.AddRange(new System.Windows.Forms.Control[] {list.form});

            // 아래의 SetChildIndex를 하지 않으면 Script에서 ToolBar Add 하면 기본 메뉴를 밀어내고 끼어드는 현상이 발생한다. 2014-10-22
            TotalConfig.formMain.Controls.SetChildIndex(list.form, 0);

            list.form.AutoScroll = false;
            list.form.Show();

            listToolBar.Add(list);

            MoveFrameToolBar(TotalConfig.formMain);
        }

        public static void DeleteOneToolBar(int index)
        {
            if (index < 0 || index >= listToolBar.Count) return;

            TOOLBAR_MODULE_LIST list;

            list = (TOOLBAR_MODULE_LIST)listToolBar[index];

            TotalConfig.formMain.Controls.Remove(list.form);

            listToolBar.RemoveAt(index);

            MoveFrameToolBar(TotalConfig.formMain);
        }

        public static void ChangeOneToolBar(int index, string file, int position, int size)
        {
            if (index < 0 || index >= listToolBar.Count) return;

            TOOLBAR_MODULE_LIST list;

            list = (TOOLBAR_MODULE_LIST)listToolBar[index];

            list.position = position;
            list.size = size;
            list.filename = file;

            string filename = MakeFilePath.Graphic(list.filename);
            list.form.Controls.Clear();

            // 이것이 없으면 스크롤이 잘 안된다. 2019-2-26
            Label label = new Label();
            label.Text = "label1";
            label.Left = -label.Width;
            list.form.Controls.Add(label);
            
            list.form.LoadFile(filename);
            //list.form = new FormGraphicChild(filename); // 다시 폼을 설정하는 것이 낫다. 기존의 child 콘트롤 들이 남아있다. 새로 만드는것은 순서가 바뀌어서 보류된것 같다.

            if (list.position == 0) list.form.Dock = DockStyle.Left;
            else if (list.position == 1) list.form.Dock = DockStyle.Top;
            else if (list.position == 2) list.form.Dock = DockStyle.Right;
            else list.form.Dock = DockStyle.Bottom;

            MoveFrameToolBar(TotalConfig.formMain);
            list.form.Invalidate();     // 같은 위치인 경우 화면이 갱신이 안되어서 추가 2008.12.23

            list.form.ScrollUpdate();   // 화면이 크거나 작아질수 있으므로 스크롤을 업데이트 한다...  2019-2-26
        }

        public static void MoveFrameToolBar(Form mainform)
        {
            Rectangle rectangle = mainform.ClientRectangle;

            int x1 = rectangle.Left;
            int y1 = rectangle.Top;
            int x2 = rectangle.Right;
            int y2 = rectangle.Bottom;

            /*
            for (int i = 0; i < mainform.Controls.Count; i++)
            {
                if (mainform.Controls[i].GetType() == typeof(MenuStrip))
                {
                    MenuStrip ms = (MenuStrip)(mainform.Controls[i]);
                    if (ms.Visible)
                    {
                        y1 += ms.Height;
                    }
                    break;
                }
            }*/

            TOOLBAR_MODULE_LIST list;
            int l;

            for (l = 0; l < listToolBar.Count; l++)
            {
                list = (TOOLBAR_MODULE_LIST)listToolBar[l];

                if (list.position == 0)
                {		// left
                    //if(x1+list.size >= x2)	list.size = 0;
                    list.form.Location = new Point(x1, y1);
                    list.form.Size = new Size(list.size, y2 - y1);
                    //list.form.SetBounds(
                    //MoveWindow(list.hwnd, x1, y1, list.size, y2-y1, TRUE);
                    x1 += list.size;
                }
                else if (list.position == 1)
                {	// top
                    //if(y1+list.size >= y2)	list.size = 0;
                    list.form.Location = new Point(x1, y1);
                    list.form.Size = new Size(x2 - x1, list.size);
                    //list.form.Left = x1;
                    //list.form.Top = y1;
                    //list.form.Width = x2-x1;
                    //list.form.Height = list.size;
                    //list.form.SetBounds(x1, y1, x2-x1, list.size);
                    //MoveWindow(list.hwnd, x1, y1, x2-x1, list.size, TRUE);
                    y1 += list.size;
                }
                else if (list.position == 2)
                {	// right
                    //if(x2-list.size <= x1)	list.size = 0;
                    list.form.Location = new Point(x2 - list.size, y1);
                    list.form.Size = new Size(list.size, y2 - y1);
                    //MoveWindow(list.hwnd, x2-list.size, y1, list.size, y2-y1, TRUE);
                    x2 -= list.size;
                }
                else
                {							// bottom
                    //if(y2-list.size <= y1)	list.size = 0;
                    list.form.Location = new Point(x1, y2 - list.size);
                    list.form.Size = new Size(x2 - x1, list.size);
                    //MoveWindow(list.hwnd, x1, y2-list.size, x2-x1, list.size, TRUE);
                    y2 -= list.size;
                }
            }
        }

        public static void Invalidate(string filename)
        {
            TOOLBAR_MODULE_LIST list;
            int l;

            for (l = 0; l < listToolBar.Count; l++)
            {
                list = (TOOLBAR_MODULE_LIST)listToolBar[l];
                if (String.Compare(filename, list.filename, true) == 0)
                {
                    list.form.Invalidate();
                }
            }
        }

        public static void Update(string filename)
        {
            TOOLBAR_MODULE_LIST list;
            int l;

            for (l = 0; l < listToolBar.Count; l++)
            {
                list = (TOOLBAR_MODULE_LIST)listToolBar[l];
                if (String.Compare(filename, list.filename, true) == 0)
                {
                    list.form.Update();
                }
            }
        }

        static public bool ExecuteClassNameOnlyObject(string classname, string command, out object retn_value, params object[] args)
        {
            TOOLBAR_MODULE_LIST list;
            int l;
            bool retn = false;
            retn_value = 0;

            for (l = 0; l < listToolBar.Count; l++)
            {
                list = (TOOLBAR_MODULE_LIST)listToolBar[l];

                retn = list.form.objectGraphic.ExecuteClassNameOnlyObject(list.form, classname, command, out retn_value, args);

                if (retn)
                    return true;
            }

            return retn;
        }

        public static void Save()
        {
            TextWriter writer;
            string path;
            TOOLBAR_MODULE_LIST list;

            path = MakeFilePath.Project("ToolBar", "ToolBar.lstx");

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            writer = new StreamWriter(path);

            if (writer == null)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("파일을 쓸 수 없습니다", path);
                else
                    MessageBox.Show("Cannot write the file.", path);

                return;
            }

            for (int i = 0; i < listToolBar.Count; i++)
            {
                list = (TOOLBAR_MODULE_LIST)listToolBar[i];

                writer.WriteLine("{0},{1},{2},", list.position, list.size, list.filename);
            }

            writer.Close();
        }

        public static void Close()
        {
            TOOLBAR_MODULE_LIST list;
            int l;
            object retn = 0;

            for (l = 0; l < listToolBar.Count; l++)
            {
                list = (TOOLBAR_MODULE_LIST)listToolBar[l];

                list.form.Close();
            }
        }
    }
}

/*
 * 
 * void FrameToolBarCreate(HWND hwnd)
{
	TOOLBAR_MODULE_LIST list;
	DWORD l;

	for(l = 0; l < blockToolBarList.GetCount(); l++) {
		blockToolBarList.GetBlock(&list, l);
		strcpy(sNewGraphicModuleName, list.filename);
		list.hwnd = CreateWindow (szViewGraphicChildClass, "",
				  WS_CHILD | WS_HSCROLL | WS_VSCROLL | WS_VISIBLE | WS_BORDER,
				  0, 0,
				  0, 0,
				  hwnd, HMENU(l+10), hInst, NULL);
		blockToolBarList.SetBlock(&list, l);
	}
}

void MoveFrameToolBar(ref int x1, ref int y1, ref int x2, ref int y2)
{
	TOOLBAR_MODULE_LIST list;
	DWORD l;

	for(l = 0; l < blockToolBarList.GetCount(); l++) {
		blockToolBarList.GetBlock(&list, l);
		if(list.position == 0) {		// left
			if(x1+list.size >= x2)	list.size = 0;
			MoveWindow(list.hwnd, x1, y1, list.size, y2-y1, TRUE);
			x1 += list.size;
		}
		else if(list.position == 1) {	// top
			if(y1+list.size >= y2)	list.size = 0;
			MoveWindow(list.hwnd, x1, y1, x2-x1, list.size, TRUE);
			y1 += list.size;
		}
		else if(list.position == 2) {	// right
			if(x2-list.size <= x1)	list.size = 0;
			MoveWindow(list.hwnd, x2-list.size, y1, list.size, y2-y1, TRUE);
			x2 -= list.size;
		}
		else {							// bottom
			if(y2-list.size <= y1)	list.size = 0;
			MoveWindow(list.hwnd, x1, y2-list.size, x2-x1, list.size, TRUE);
			y2 -= list.size;
		}
	}
}
*/

