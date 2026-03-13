using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ScriptLibRun
{
    /// <summary>
    /// 
    /// </summary>
    public class ScriptLibLibrary
    {
        public string sNameLibrary;
        public List<ScriptLibNamespace> arrayNamespace = new List<ScriptLibNamespace>();
        public bool bInternalLibrary = false;   // 내부에 내장하고 있는 라이브러리이다.

        public virtual bool Split(ScriptLibMain main, object file, string body)
        {
            return false;
        }

        public virtual void Compile(ScriptLibMain main) { }

        public void SetBreakPoints(string filename, List<int> array)
        {
            for (int i = 0; i < arrayNamespace.Count; i++)
            {
                arrayNamespace[i].SetBreakPoints(filename, array);
            }
        }
        
        public void Load(string filename, byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.Version)
                {
                    int major, minor, build, revision;
                    block.ReadVersion(out major, out minor, out build, out revision);

                    if (major > ScriptWriter.nVersionMajor)
                    {
                        string msg = String.Format("Filename={0}\nFile Version={1}.{2}.{3}.{4}\nCurrent Version={5}.{6}.{7}.{8}", filename, 
                            major, minor, build, revision, ScriptWriter.nVersionMajor, ScriptWriter.nVersionMinor, ScriptWriter.nVersionBuild, ScriptWriter.nVersionRevision);
                        MessageBox.Show(msg, "Script file version too High.");
                        return;
                    }
                }
                else if (block.type == EnumBlockType.NamespaceBlock)
                {
                    ScriptLibNamespace sln = new ScriptLibNamespace(this);
                    sln.Load(block.block_data, ref line);
                    arrayNamespace.Add(sln);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }

        public string MakeDecompiledString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < arrayNamespace.Count; i++)
            {
                if (i > 0) sb.AppendLine(); // 두번째 부터는 한칸 띄워준다.

                sb.Append(arrayNamespace[i].MakeDecompiledFile());
            }

            return sb.ToString();
        }

        /*
        public void MakeDecompiledFile(string filename)
        {
            string target = Path.ChangeExtension(filename, ".cs");

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < arrayNamespace.Count; i++)
            {
                if (i > 0)  sb.AppendLine(); // 두번째 부터는 한칸 띄워준다.

                sb.Append(arrayNamespace[i].MakeDecompiledFile());
            }

            TextWriter writer = new StreamWriter(target);
            writer.Write(sb.ToString());
            writer.Flush();
            writer.Close();
        }*/
    }
}
