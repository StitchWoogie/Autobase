using System;

namespace OpcClient
{
	/// <summary>
	/// Summary description for TimeOutClass.
	/// </summary>
	public class TimeOutClass
	{
		int old_sec;
		int nCurrCount;

		public TimeOutClass()
		{
			//
			// TODO: Add constructor logic here
			//
			Reset();
		}

		public void SetTime(int sec)
		{
			DateTime dt = DateTime.Now;
			old_sec = dt.Second;
			nCurrCount = sec;
		}

		public void Reset()
		{
			SetTime(0);
		}

		public bool IsTimeOut(int time_out)
		{
			DateTime dt = DateTime.Now;

			if(dt.Second != old_sec) 
			{
				if(dt.Second < old_sec)
					nCurrCount += dt.Second+60-old_sec;	
				else
					nCurrCount += dt.Second-old_sec;	

				old_sec = dt.Second;
			}

			if(nCurrCount >= time_out)	return true;	// 원래 이 비교문은 if 문장속에 들어 있었는데 
			// IsTimeOut을 연속해서 여러번 부를때는 이 문장까지 들어오지 못하므로 
			// 비교문만 밖으로 나왔다.
			return false;
		}
	}
}

/*
 * 
 * #include "stdafx.h"
#include <dos.h>

#include <tools.h>

TimeOutClass :: TimeOutClass()
{
	Reset();
} 

void SetTime(int sec)
{
	//struct time t;

	gettime(&t);
	old_sec = t.ti_sec;
	nCurrCount = sec;
}

void TimeOutClass :: Reset()
{
	SetTime(0);
}

int IsTimeOut(int time_out)
{
	//struct time t;

	gettime(&t);

	if(t.ti_sec != old_sec) {
		if(t.ti_sec < old_sec)
			nCurrCount += t.ti_sec+60-old_sec;	
		else
			nCurrCount += t.ti_sec-old_sec;	

		old_sec = t.ti_sec;
//		nCurrCount++;
	}

	if(nCurrCount >= time_out)	return 1;	// 원래 이 비교문은 if 문장속에 들어 있었는데 
											// IsTimeOut을 연속해서 여러번 부를때는 이 문장까지 들어오지 못하므로 
											// 비교문만 밖으로 나왔다.
	return 0;
}
*/