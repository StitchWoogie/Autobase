using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptLibRun
{
    public enum EnumInOut
    {
        In,         
        Out,
        Ref,
        Params,
        OutParams, // 구형 스크립트에만 있는 형식 GetVarValue
    }
}
