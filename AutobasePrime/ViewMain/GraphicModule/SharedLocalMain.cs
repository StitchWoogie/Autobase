using System;
using System.Collections;
using System.IO;
using NetTools;
using AutoLibLocal;
using AutoLib;
using NetTools.OldDefine;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;

//스크립트 실행 시 TestMode Check용 추가 20241010 PSU
namespace GraphicModule
{
    /// <summary>
    /// Summary description for SharedLocalMain.
    /// </summary>
    public class SharedLocalMain
    {
        public static bool bTestMode = TotalConfig.GetAutoBaseTestMode();

        public SharedLocalMain()
        {
            // Constructor logic here if needed
        }
    }
}