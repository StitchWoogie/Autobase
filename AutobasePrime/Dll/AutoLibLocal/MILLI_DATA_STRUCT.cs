using System;
using NetTools.OldDefine;
using System.Collections;
using System.Data.OleDb;
using System.Data;
using System.Threading.Tasks;

namespace AutoLibLocal
{
    /// <summary>
    /// Summary description for MILLI_DATA_STRUCT.
    /// </summary>
    /// 
    [Serializable]
    public class MILLI_DATA_TAG
    {
        public string tag;
        public EnumTagType tag_type;
        public int[] tag_pos;
    }


    public class MILLI_DATA_STRUCT
    {
        public string title;
        public int nGab;               // 자료 간격
        public int nCondition;			// 저장 조건
        public string tagCheckDI;			// 태그
        public int[] tag_pos;			// DI태그의 Tag position;
        public string filename_mdb;		// 현재 저장하고 있는 파일이름 MDB
        public string filename_csv;		// 현재 저장하고 있는 파일이름 CSV
        public string tableName; //추가 250926 PSU

        public bool bDbOpenError;      // MDB Header와 컬럼 만들때 오류가 나면 데이터를 저장하지 않는다.  9.4.0 지원

        public int nSizeCut;
        public int nCutMethod;

        public sbyte bSavingFlag;		// 저장이 진행중이다.
        public bool bErrorFlag;			// 오류
        public string sErrorMsg;
        public int dwRecordCount;		// 저장된 시간	milli sec
        public int nRemainedTime;
        public SYSTEMTIME old_time = new SYSTEMTIME();		//
        public SYSTEMTIME stStart = new SYSTEMTIME();
        public SYSTEMTIME stRecord = new SYSTEMTIME();		// Data 저장 시 시간을 저장하기 위해 start_time에서 증가시켜나간다.
        public sbyte old_di_curr;
        //public sbyte	bTimeSave;			// 레코드 저장할 때 시간도 저장한다. 9.4.0 부터 제외
        public bool bDateTimeMatch;         // 9.4.0 부터 지원

        public ArrayList blockTag;
        public CommonDbConnection db;		// data base

        public bool bAutoDelete;            // 자동 삭제
        public int nDaysOfAutoDelete = 365; // 자동 삭제 기간

        public int nMilliDataConfigId;// (system.millidata_config 테이블 참조)

        public int nSaveFileType = 0;       // 9.4.0 부터 지원 0=MDB, 1 = MDB+CSV, 2 = CSV

        public bool bUseTargetFolder;
        public string sTargetFolder;

        public bool bUseCsvDateFolder;

        // 2023-7-27 기능추가. 경일 요청
        public bool bUseFilenameAddition;   // 날짜_시간+????.mdb 처럼 파일명에 추가로 붙여주는 기능
        public string sUseFilenameAdditionTag;   // 날짜_시간+????.mdb 처럼 파일명에 추가로 붙여주는 기능. 
        public int[] tag_posUseFilenameAddition;			

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

        
        //bool bMdbOpened = false;     // 2017-3-15 에 저장할 때만 열고 다시 닫는 옵션이 추가되어서 Db가 현재 Open상태인가를 나타내기 위해서

        public void DbOpen()
        {
            if (db.State != System.Data.ConnectionState.Open)
            {
                db.Open();
            }
        }

        public void DbClose()
        {
            if (db != null && db.State == ConnectionState.Open)
                db.Close();
        }
    }
}
