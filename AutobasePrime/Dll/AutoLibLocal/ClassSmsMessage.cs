using System;
using System.Collections.Generic;
using System.Text;

namespace AutoLibLocal
{
    [Serializable]
    public class ClassSmsMessage
    {
        public string recv;
        public string send;
        public string msg;
        public int sms_type;
    }
}
