using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoLibLocal
{
    public class MyNumericUpDown : NumericUpDown
    {
        public MyNumericUpDown()
        {
            // SBAS는 에디터로는 편집을 할 수 없도록 만든다.
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.ReadOnly = true;
            }
            else if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
                this.ReadOnly = true;
            }
        }

        int nSampleProperty = 0;

        public int SampleProperty
        {
            get
            {
                return nSampleProperty;
            }
            set
            {
                nSampleProperty = value;
            }
        }
    }
}
