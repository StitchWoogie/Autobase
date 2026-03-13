using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;
using System.IO;
using System.Windows.Forms;

namespace ScriptLibEdit
{
    public class EditScriptLibLibrary : ScriptLibLibrary
    {
        // string sDefaultNamespace = "My";    // namespace가 선언되지 않았을 때 지정할 기본이름

        public override bool Split(ScriptLibMain main, object file, string body)
        {
            EditScriptLibFile f = (EditScriptLibFile)file;

            ScriptLibNamespace sln = AddNamespaceClass(main, f.sDefaultNamespace);
            //ScriptLibNamespace sln = AddNamespaceClass(main, sDefaultNamespace);

            return sln.Split(main, file, body, 0, 0);
        }

        // Compile전에 준비해야 하는 것들
        public void PrepareBeforeCompile()
        {
            for (int i = 0; i < arrayNamespace.Count; i++)
            {
                ((EditScriptLibNamespace)arrayNamespace[i]).PrepareBeforeCompile();
            }
        }

        public override void Compile(ScriptLibMain main)
        {
            for (int i = 0; i < arrayNamespace.Count; i++)
            {
                ((ScriptLibNamespace)arrayNamespace[i]).Compile(main);
            }
        }

        public ScriptLibNamespace AddNamespaceClass(ScriptLibMain main, string name)
        {
            for (int i = 0; i < arrayNamespace.Count; i++)
            {
                if (arrayNamespace[i].sNameNamespace == name) return arrayNamespace[i];
            }

            EditScriptLibNamespace nameclass = new EditScriptLibNamespace(main, this);
            nameclass.sNameNamespace = name;

            arrayNamespace.Add(nameclass);

            return nameclass;
        }

        public void Save(string target_dir)
        {
            BinaryWriter writer;

            if (!Directory.Exists(target_dir))
            {
                Directory.CreateDirectory(target_dir);
            }

            if (bInternalLibrary) return;    // 내장된 라이브러리이다.

            string filename = String.Format("{0}\\{1}.objx", target_dir, sNameLibrary);

            try
            {
                FileStream stream = File.Open(filename, FileMode.Create);
                writer = new BinaryWriter(stream);
            }
            catch (Exception exception)
            {
                string message = String.Format("Filename ={0}\n\nMessage = {1}", filename, exception.Message);
                MessageBox.Show(message, "File Write Open Error");
                return;
            }

            if (writer == null)
            {
                string message = String.Format("{0}\nCan't write the file.", filename);
                MessageBox.Show(message, "File Writing Error");
                return;
            }

            ScriptWriter wr = new ScriptWriter();
            SaveToWriter(wr, 0);

            byte[] buffer = wr.ToArray();

            writer.Write(buffer);
            writer.Flush();
            writer.Close();
        }

        void SaveToWriter(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            writer.WriteVersion();

            for (int i = 0; i < arrayNamespace.Count; i++)
            {
                // 기본 라이브러리는 저장할 필요가 없다.
                if (arrayNamespace[i].GetType() == typeof(EditScriptLibNamespace))
                {
                    ((EditScriptLibNamespace)arrayNamespace[i]).SaveToStream(writer, tab_depth + 1);
                }
            }

            parent_writer.WriteBlock(EnumBlockType.LibraryBlock, writer);
        }
    }
}
