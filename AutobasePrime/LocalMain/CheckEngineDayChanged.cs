using System;
using NetTools;
using System.IO;
using AutoLibLocal;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading.Tasks;

namespace LocalMain
{
	/// <summary>
	/// Summary description for CheckEngineDayChanged.
	/// </summary>
	public class CheckEngineDayChanged
	{
		public CheckEngineDayChanged()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//--------------------------------------------------------------------------------------------
		//	하루가 바뀌기 바로 이전 해야할 일들을 모아 놓았다.
		//	이 함수를 실행된 다음 하루가 바뀐다.
		//--------------------------------------------------------------------------------------------

		public static async Task WorkOnBeforeChangeDay(DateTime t)
		{
            // DeleteOldDataByMonthLimit();				// 하루가 지나면 자료 저장기간이 지난 파일은 지운다.
            CheckEngineAutoDeleteThread.bGoAutoDeleteMonthLimit = true;

			CheckEngineOnOffList.DigitalOnOffListSaveOnDayChanged(t);
											// ON OFF List를 저장한다.

			CheckEngineSchedule.ScheduleChangeOnDateChanged();

            await CheckEngineMilliData.CheckAutoDelete();
		}

		

	}

	
}


