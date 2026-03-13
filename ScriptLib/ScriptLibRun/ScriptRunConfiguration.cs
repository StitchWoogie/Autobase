using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptLibRun
{
    // Run시 각 스크립트마다 고유한 환경을 가진다.
    public class ScriptRunConfiguration
    {
        public ScriptLibMain slmain;
        public object externalConfig;        // 외부 함수/변수에서 사용하는 환경. Autobase에서는 ScriptClass의 포인터를 사용할 수 있다.
    }
}
