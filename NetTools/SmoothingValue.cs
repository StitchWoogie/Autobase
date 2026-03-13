using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetTools
{
    public class SmoothingValue
    {
        double[] values = null;
        int pos = 0;
        int MAX_GATHER = 10;

        public void SetMaxGather(int max)
        {
            MAX_GATHER = max;
            values = null;
        }

        public void AddValue(double value)
        {
            if (values == null)
            {
                values = new double[MAX_GATHER];
                for (int i = 0; i < MAX_GATHER; i++)
                {
                    values[i] = value;
                }
            }
            else
            {
                values[pos] = value;
                pos++;
                pos %= MAX_GATHER;
            }
        }

        public double GetAverage()
        {
            if (values == null) return 0;

            double ave = 0;
            for (int i = 0; i < MAX_GATHER; i++)
            {
                ave += values[i];
            }

            ave /= MAX_GATHER;

            return ave;
        }

    }
}
