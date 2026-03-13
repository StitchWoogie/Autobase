using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;

namespace ScriptLibRun
{
    public class ScriptLibTools
    {
        /// <summary>
        /// Begin 다음에 있는 column_pos, row_pos를 읽어온다.
        /// </summary>
        /// <param name="comma"></param>
        public static void LoadColumnRowPos(CommaTextReader comma, out int col_pos, out int row_pos)
        {
            col_pos = comma.GetInt();
            row_pos = comma.GetInt();
        }

        // namespace, class, method 를 분리한다.
        public static void SplitNamespaceClassMethod(string source, out string name_namespace, out string name_class, out string name_method)
        {
            // namespace, class, method 를 분리한다.
            name_method = source;
            name_class = null;
            name_namespace = null;

            for (int i = name_method.Length - 1; i >= 0; i--)
            {
                if (name_method[i] == '.')
                {
                    name_class = name_method.Substring(0, i);
                    name_method = name_method.Substring(i + 1);
                    goto seeked_method;
                }
            }
            return;
            seeked_method:
            for (int i = name_class.Length - 1; i >= 0; i--)
            {
                if (name_class[i] == '.')
                {
                    name_namespace = name_class.Substring(0, i);
                    name_class = name_class.Substring(i + 1);
                    break;
                }
            }
        }

        // 할당한 클래스의 이름을 찾는데 사용된다.
        // namespace, class 를 분리한다.
        public static void SplitNamespaceClass(string source, out string name_namespace, out string name_class)
        {
            // namespace, class, method 를 분리한다.
            name_class = source;
            name_namespace = null;

            for (int i = name_class.Length - 1; i >= 0; i--)
            {
                if (name_class[i] == '.')
                {
                    name_namespace = name_class.Substring(0, i);
                    name_class = name_class.Substring(i + 1);
                    break;
                }
            }
        }

        /// <summary>
        /// 앞 뒤의 모든 공간을 없애주고 cursor를 이동해 준다.
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public static string TrimWithCursorPos(string buf, ref int col_pos, ref int row_pos)
        {
            for (int i = 0; i < buf.Length; i++, col_pos++)
            {
                if (buf[i] == ' ') continue;
                if (buf[i] == '\t') continue;

                break;
            }

            return buf.Trim();
        }
    }
}
