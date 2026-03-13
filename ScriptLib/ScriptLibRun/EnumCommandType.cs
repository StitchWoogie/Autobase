using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptLibRun
{
    public enum EnumCommandType
    {
        type_none = 0,
        type_for = 1,
        type_while = 2,
        type_foreach = 3,
        type_if = 4,
        type_else = 5,
        type_switch = 6,
        type_method = 7,
        type_declaration = 8,
        type_block = 9,
        type_return = 10,
        type_substitution = 11,
        type_break = 12,
        type_continue = 13,
        type_goto = 14,
        type_label = 15,        // goto label 문
        type_voidvalue = 16,    // x++, ++x, x--, --y 와 같이 실행은 하지만 반환값이 필요없는 경우 RecursiveValue에 이미 구현되어 있으므로 RecursiveValue를 그래도 사용한다.
        type_lastline = 17,     // 디버그용으로 만드 커맨드이다. 함수의 맨 마지막 줄에서 Debug Break 하기위해 추가되었다.
                                // Command Block에서 제일 마지막 줄을 커맨드로 등록한다. 디버그에서 트레이스 할 때 마지막이 없으면 변수를 디버그 할 수 없다. 디버그용으로 만드 커맨드이다.
    }
}
