using System;
using NetTools.OldDefine;
using System.Collections;
using System.Data.OleDb;
using System.Collections.Generic;

namespace AutoLibLocal
{
    /// <summary>
    /// Summary description for MILLI_DATA_STRUCT.
    /// </summary>
    /// 
    public class RealTimeTestStruct
    {
        public string title;

        public int nSize;               // 자료 크기
        public int nGab;                // 자료 간격

        public int nCondition;			// 저장 조건
        public string tagCheckDI;		// 태그
        public int[] tag_pos;			// DI태그의 Tag position;
        public string filename_mdb;		// 현재 저장하고 있는 파일이름 MDB
        public string filename_csv;		// 현재 저장하고 있는 파일이름 CSV

        public bool bMdbOpenError;      // MDB Header와 컬럼 만들때 오류가 나면 데이터를 저장하지 않는다.  9.4.0 지원

        public int nSizeCut;
        public int nCutMethod;

        //public sbyte bSavingFlag;		// 저장이 진행중이다.
        public bool bErrorFlag;			// 오류
        public string sErrorMsg;
        public int dwRecordCount;		// 저장된 시간	milli sec
        public int nRemainedTime;
        public DateTime tStart;
        //public SYSTEMTIME old_time = new SYSTEMTIME();		//
        //public SYSTEMTIME stStart = new SYSTEMTIME();
        //public SYSTEMTIME stRecord = new SYSTEMTIME();		// Data 저장 시 시간을 저장하기 위해 start_time에서 증가시켜나간다.
        //public sbyte old_di_curr;
        //public sbyte	bTimeSave;		    // 레코드 저장할 때 시간도 저장한다. 9.4.0 부터 제외
        public bool bDateTimeMatch;         // 9.4.0 부터 지원

        public List<RealTimeTestTag> blockTag;
        public CommonDbConnection db;	    // data base

        public bool bAutoDelete;            // 자동 삭제
        public int nDaysOfAutoDelete = 365; // 자동 삭제 기간

        public int nSaveFileType = 0;       // 9.4.0 부터 지원 0=MDB, 1 = MDB+CSV, 2 = CSV

        public bool bUseTargetFolder;
        public string sTargetFolder;

        public bool bUseCsvDateFolder;

        public bool bStartFlag; // 저장이 진행중이다.
        public sbyte bOldTagValue;
        public int nOldMilliSec;
        public int nRemainMilliSec;
        public int nBufPos;

        public int nBackupStatusGatherRemain = 0;   // 데이터를 수집한 갯수 중간 중간에 현재 상태를 저장하면 다시 0이 된다.
        public int nBackupStatusOldMinute = 0;      // 데이터를 수집한 갯수 중간 중간에 현재 상태를 저장하면 다시 0이 된다.

        public string tagRunDI;		// 태그         2020-6-29 추가.
        public int[] tag_pos_run;	// DI태그의 Tag position;  2020-6-29 추가.

        public static string ChangeToFitMdbColumnName(string source)
        {
            string target = "";

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == '.') target += "_";
                else
                {
                    target += source[i];
                }
            }

            return target;
        }
    }

    [Serializable]
    public class RealTimeTestTag
    {
        public string tag;
        public EnumTagType tag_type;
        public int[] tag_pos;
        public double[] data;
        public byte[] flags;        // 2020-6-29 추가.  Run 태그의 상태
    }
}
