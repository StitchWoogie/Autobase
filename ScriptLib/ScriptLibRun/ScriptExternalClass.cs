using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    /// <summary>
    /// 정식 스크립트가 아닌 자체 함수와 변수를 사용자가 직접 정의하여 사용한다. 
    /// Autobase에서는 ScriptExternalRun.cs 파일에서 이 클래스를 상속 받아서 이전의 @함수나 $태그 등을 연결해서 사용한다.
    /// </summary>
    public class ScriptExternalClass
    {
        string sErrorMessage;
        EnumScriptErrorType eErrorType;

        public EnumScriptErrorType ErrorType
        {
            get {
                return eErrorType;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return sErrorMessage;
            }
        }
        
        public void SetError(EnumScriptErrorType et, string msg)
        {
            eErrorType = et;
            sErrorMessage = msg;
        }

        /// <summary>
        /// 컴파일 시 외부에서 사용하는 변수가 맞는가를 체크한다.
        /// </summary>
        /// <param name="varname"></param>
        /// <returns>0 = 아니다, 1 = 맞다, 2=맞으나 오류이다. 오류메시지를 읽을 것</returns>
        public virtual int IsExistVariable(string varname)
        {
            return 0;
        }

        public virtual int IsExistMethod(string methodname, List<CommandMethodArg> args)
        {
            return 0;
        }

        // pre_pointer 를 사용하면 그냥 태그나 함수를 찾는것 보다는 10%이상의 효과가 있다.
        public virtual object RunVariable(ScriptRunConfiguration src, object pre_pointer, string varname, out object value)
        {
            value = 0;
            
            return null;
        }

        // pre_pointer 를 사용하면 그냥 태그나 함수를 찾는것 보다는 10%이상의 효과가 있다.
        public virtual async Task<(object, object retn)> RunMethodAsync(ScriptRunConfiguration src, object pre_pointer, string methodname, object[] param, List<CommandMethodArg> args)
        {
            object retn = 0;

            await Task.CompletedTask;
            return (null, retn);
        }

        // pre_pointer 를 사용하면 그냥 태그나 함수를 찾는것 보다는 10%이상의 효과가 있다.
        public virtual async Task<object> ChangeVariable(ScriptRunConfiguration src, object pre_pointer, string varname, object value)
        {
            await Task.CompletedTask;
            return null;
        }
    }

    public enum EnumScriptErrorType
    {
        Else = 0,
        TagNotFound = 1,
        VarNotFound = 2
    }
}
