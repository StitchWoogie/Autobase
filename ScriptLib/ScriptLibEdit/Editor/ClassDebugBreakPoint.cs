using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptLibEdit.Editor
{
    public class ClassDebugBreakPoint
    {
        public string filename;
        public List<int> arrayPoint = new List<int>();

        public static List<ClassDebugBreakPoint> arrayBreakPoints = new List<ClassDebugBreakPoint>();

        public static void Load()
        {

        }

        public static void Save()
        {

        }
    }
}
