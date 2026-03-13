using System;
using System.Collections.Generic;
using System.Text;

namespace AutoLibLocal
{
    [Serializable]
    public class OpcWriteItem
    {
        public string servername;
        public string groupname;
        public string itemname;
        public int pos;
        public object data;
        public int writeSource = 0;             // 0 = 감시 프로그램의 출력 데이터 OpcClient에서만 사용하는 변수
        public bool bManualOperation = false;   // 수동 출력의 여부
    }
}
