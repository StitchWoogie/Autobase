using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public abstract class ScriptLibMemberPublic
    {
        //public string sName;  // 모든 멤버들은 공통적으로 이름을 가진다.
                                // class, enum, method, class variable, delegate, struct, interface 등이 이 클래스를 상속 받는다.

        public EnumScriptLibMember eMember;

        public abstract string GetName();

        // static, const, enum 같은 값은 stack에 변수가 있는것이 아니고 전역에 있다
        public virtual bool GetStaticValue(ScriptRunConfiguration src, out object retnvalue)
        {
            retnvalue = 0;
            return true;
        }

        public virtual async Task<(bool success, object retnvalue)> GetStaticValueAsync(ScriptRunConfiguration src)
        {
            await Task.CompletedTask; // 경고해결용

            object retnvalue;
            bool success = GetStaticValue(src, out retnvalue);
            return (success, retnvalue);
        }

        // static, const, enum 같은 값은 stack에 변수가 있는것이 아니고 전역에 있다
        public virtual bool SetStaticValue(ScriptRunConfiguration src, object value)
        {
            return true;
        }

        public virtual void SetBreakPoints(string filename, List<int> array)
        {
            /*
            for (int i = 0; i < arrayMember.Count; i++)
            {
                arrayMember[i].SetBreakPoints(filename, array);
            }*/
        }

        public virtual string MakeDecompiledFile(int depth)
        {
            return "";
        }
    }

    public enum EnumScriptLibMember {
        Variable,   // Class에 들어있는 변수
        Method,     // Class에 들어있는 메소드
        Class,      // Class
        Enum,       // Enum
        EnumItem,   // Enum에 들어 있는 아이템
    }
}
