using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoLibLocal;
using NetTools;
using System.Windows;

namespace Studio.Solution
{
    public class ScriptSolution
    {
        public static List<ScriptProject> arrayProjects = new List<ScriptProject>();

        public static ScriptProject SeekProjectByName(string name)
        {
            for (int i = 0; i < arrayProjects.Count; i++)
            {
                if (String.Compare(Path.GetFileNameWithoutExtension(arrayProjects[i].sProjectFilename), name, true) == 0)
                {
                    return arrayProjects[i];
                }
            }

            return null;
        }

        static void MakeDefaultProject()
        {
            ScriptProject project = new ScriptProject();
            project.sProjectFilename = String.Format("{0}\\Script\\LocalScript\\LocalScript.ScriptProject", TotalConfig.sDirWorkProject);
            ScriptSolution.arrayProjects.Add(project);

            project.LoadScriptProject();

            SaveScriptSolution();
        }

        public static void LoadScriptSolution()
        {
            ScriptSolution.arrayProjects.Clear();  // 새로 고침을 대비하여 기존 리스트를 삭제하고 다시 읽는다.

            string solution_file = String.Format("{0}\\Script\\Local.ScriptSolution", TotalConfig.sDirWorkProject);

            if (!File.Exists(solution_file))
            {
                MakeDefaultProject();
                return;
            }
            TextReader reader = new StreamReader(solution_file);

            string one_line;

            CommaTextReader comma = new CommaTextReader();
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);

                ScriptProject project = new ScriptProject();
                project.sProjectFilename = comma.GetString();
                ScriptSolution.arrayProjects.Add(project);
                project.LoadScriptProject();
            }

            reader.Close();

            if (arrayProjects.Count == 0)
            {
                MakeDefaultProject();
            }
        }

        public static void SaveScriptSolution()
        {
            string dir = String.Format("{0}\\Script", TotalConfig.sDirWorkProject);
            Directory.CreateDirectory(dir);
            
            string solution_file = String.Format("{0}\\Script\\Local.ScriptSolution", TotalConfig.sDirWorkProject);


            CommaTextWriter writer = new CommaTextWriter(solution_file);

            for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
            {
                writer.WriteLine("{0},", ScriptSolution.arrayProjects[i].sProjectFilename);
            }

            writer.Close();
        }
    }
}
