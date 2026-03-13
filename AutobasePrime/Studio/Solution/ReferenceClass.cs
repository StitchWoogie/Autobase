using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Studio.Solution
{
    public enum EnumReferenceType
    {
        Project,
        Assembly,
    }

    public class ReferenceClass
    {
        public EnumReferenceType type;
        public string sReferenceName;
    }
}
