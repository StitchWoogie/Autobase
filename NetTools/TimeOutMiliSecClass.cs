using System;
using System.Threading;

namespace NetTools
{
	/// <summary>
	/// Summary description for TimeOutMiliSecClass.
	/// </summary>
	public class TimeOutMiliSecClass
	{
		public TimeOutMiliSecClass()
		{
			//
			// TODO: Add constructor logic here
			//
			Reset();
		}

		DateTime old_t;
		long dwCurrCount;

		void SetTime(int milisec)
		{
			old_t = DateTime.Now;
			dwCurrCount = milisec;
		}

		public void Reset()
		{
			SetTime(0);
		}

		public bool IsTimeOut(int time_out)
		{
			DateTime t;

			t = DateTime.Now;

			if(t.Second == old_t.Second) 
			{	// 초가 변함 없다.
				dwCurrCount += (t.Millisecond-old_t.Millisecond);
			}
			else if(t.Second > old_t.Second) 
			{	// 초는 변하고 분은 변하지 않았다.
				dwCurrCount += ((t.Second*1000L+t.Millisecond)-
					(old_t.Second*1000L+old_t.Millisecond));
			}
			else 
			{	// 분이 넘어가서 초 단위가 작아졌다.
				dwCurrCount += (((t.Second+60)*1000L+t.Millisecond)-
					(old_t.Second*1000L+old_t.Millisecond));	
			}

			old_t = t;
			if(dwCurrCount >= time_out)	return true;

			//Sleep(1);	// Sleep을 여기서 주면 안됨
			// TimeOut을 사용하는 Method에서 따로처리할 것
			return false;
		}

		public void Minus(int milisec)
		{
			if(milisec < 1)	return;
			while(dwCurrCount >= milisec)
				dwCurrCount -= milisec;
		}

        public void SleepRemain()
        {
            int count = (int)dwCurrCount;
            if (count < 1) count = 1;

            Thread.Sleep(count);
            Reset();
        }
	}
}

