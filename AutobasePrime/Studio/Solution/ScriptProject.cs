using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace Studio.Solution
{
    public class ScriptProject
    {
        public string sProjectFilename;
        public string sDefaultNamespace;

        public List<string> arraySource = new List<string>();  // Source 파일 들
        public List<ReferenceClass> arrayReference = new List<ReferenceClass>();  // Reference

        public bool bCompiled = false;          // 한번 컴파일 되었으면 다시 할 필요가 없다.

        public void LoadScriptProject()
        {
            arraySource.Clear();  // 새로 고침을 대비하여 기존 리스트를 삭제하고 다시 읽는다.

            sDefaultNamespace = Path.GetFileNameWithoutExtension(sProjectFilename);

            if (!File.Exists(sProjectFilename))
            {
                return;
            }
            TextReader reader = new StreamReader(sProjectFilename);

            string one_line;

            CommaTextReader comma = new CommaTextReader();
            string buf;
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);

                buf = comma.GetString();
                if (buf == "Source")
                {
                    arraySource.Add(comma.GetString());
                }
                else if (buf == "DefaultNamespace")
                {
                    sDefaultNamespace = comma.GetString();
                }
                else if (buf == "Reference")
                {
                    ReferenceClass reference = new ReferenceClass();
                    buf = comma.GetString();

                    try
                    {
                        reference.type = (EnumReferenceType)Enum.Parse(typeof(EnumReferenceType), buf);
                    }
                    catch
                    {
                        reference.type = EnumReferenceType.Project;
                    }

                    reference.sReferenceName = comma.GetString();
                    arrayReference.Add(reference);
                }
            }

            reader.Close();
        }

        public void SaveScriptProject()
        {
            string project_dir = Path.GetDirectoryName(sProjectFilename);

            Directory.CreateDirectory(project_dir); // 초기의 LocalScript 프로젝트는 안만들어진 경우가 있다.

            CommaTextWriter writer = new CommaTextWriter(sProjectFilename);

            for (int i = 0; i < arraySource.Count; i++)
            {
                writer.WriteLine("Source,{0},", arraySource[i]);
            }

            writer.WriteLine("DefaultNamespace,{0},", sDefaultNamespace);

            for (int i = 0; i < arrayReference.Count; i++)
            {
                writer.Write("Reference,{0},", arrayReference[i].type);
                writer.Write("{0},", arrayReference[i].sReferenceName);
            }
            writer.WriteLine();

            writer.Close();
        }
    }
}
