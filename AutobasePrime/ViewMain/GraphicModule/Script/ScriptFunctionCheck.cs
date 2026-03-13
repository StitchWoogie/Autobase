using System;
using System.Threading.Tasks;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ScriptFunctionCheck.
	/// </summary>
	public class ScriptFunctionCheck
	{
		public ScriptFunctionCheck()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static async Task<(int, object val)> Function_CheckAsync(ScriptClass scriptClass, string command, string argument, bool need_retn_value)
		{
            int retn;
            object val = 0;

            (retn, val) = await ScriptExternalRun.scriptExternal.RunMethodFromOldScript(scriptClass, command, need_retn_value, argument).ConfigureAwait(false);

            if (retn != 0) return (retn, val);

            /* ScriptExpandRun으로 모두 이동 2016-4-1
			if(String.Compare(command, 0, "SQL", 0, 3) == 0) 
			{
				return ScriptFunctionSql.Function_SQL(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-4-1
            if(String.Compare(command, 0, "Report", 0, 6) == 0) 
            {
                return ScriptFunctionReport.Function_Report(scriptClass, command, argument, out val);
            }*/

            /*
			if(String.Compare(command, 0, "Screen", 0, 6) == 0) 
			{
				return ScriptFunctionScreen.Function_Screen(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-16
			if(String.Compare(command, 0, "Alarm", 0, 5) == 0) 
			{
				return ScriptFunctionAlarm.Function_Alarm(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-20
			// 이 체크가 "Data" 함수보다 먼저 체크되어야 한다."
			if(String.Compare(command, 0, "Database", 0, 8) == 0) 
			{	
				return ScriptFunctionDatabase.Function_Database(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-20
			if(String.Compare(command, 0, "Data", 0, 4) == 0) 
			{
				return ScriptFunctionData.Function_Data(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-30
			if(String.Compare(command, 0, "PlcScan", 0, 7) == 0) 
			{
				return ScriptFunctionPlcScan.Function_PlcScan(scriptClass, command, argument, out val);
			}*/
            /* ScriptExpandRun으로 모두 이동 2016-3-19
			if(String.Compare(command, 0, "ComboBox", 0, 8) == 0) 
			{
				return ScriptFunctionControlComboBox.Function_ComboBox(scriptClass, command, argument, out val);
			}
            */
            /* ScriptExpandRun으로 모두 이동 2016-3-22
			if(String.Compare(command, 0, "DbTrend", 0, 7) == 0) 
			{
				// 이 체크가 "Db" 함수보다 먼저 체크되어야 한다."
				return ScriptFunctionDbTrend.Function_DbTrend(scriptClass, command, argument, out val);
			}*/
            /*
			if(String.Compare(command, 0, "Db", 0, 2) == 0) 
			{	
				return ScriptFunctionDb.Function_Db(scriptClass, command, argument, out val);
			}*/
            /* ScriptExpandRun으로 모두 이동 2016-3-19
			if(String.Compare(command, 0, "CheckBox", 0, 8) == 0) 
			{
				return ScriptFunctionControlCheckBox.Function_CheckBox(scriptClass, command, argument, out val);
			}*/
            /* ScriptExpandRun으로 모두 이동 2016-3-20
			if(String.Compare(command, 0, "ListBox", 0, 7) == 0) 
			{
				return ScriptFunctionControlListBox.Function_ListBox(scriptClass, command, argument, out val);
			}*/
            /* ScriptExpandRun으로 모두 이동 2016-3-13
			if(String.Compare(command, 0, "EditBox", 0, 7) == 0) 
			{
				return ScriptFunctionControlEditBox.Function_EditBox(scriptClass, command, argument, out val);
			}*/
            /* ScriptExpandRun으로 모두 이동 2016-3-203
			if(String.Compare(command, 0, "RadioButton", 0, 11) == 0) 
			{
				return ScriptFunctionControlRadioButton.Function_RadioButton(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-28
			if(String.Compare(command, 0, "Mdi", 0, 3) == 0) 
			{
				return ScriptFunctionMdi.Function_Mdi(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-30
			if(String.Compare(command, 0, "MultiTrend", 0, 10) == 0) 
			{
				return ScriptFunctionMultiTrend.Function_MultiTrend(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-29
			if(String.Compare(command, 0, "MultiGraph", 0, 10) == 0) 
			{
				return ScriptFunctionMultiGraph.Function_MultiGraph(scriptClass, command, argument, out val);
			}*/
            /* ScriptExpandRun으로 모두 이동 2016-3-22
            if (String.Compare(command, 0, "Dde", 0, 3) == 0)
            {
                return ScriptFunctionDde.Function_Dde(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-22
            if(String.Compare(command, 0, "Dialog", 0, 6) == 0) 
            {
                return ScriptFunctionDialog.Function_Dialog(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-27
			if(String.Compare(command, 0, "Key", 0, 3) == 0) 
			{
				return ScriptFunctionKey.Function_Key(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-28
			if(String.Compare(command, 0, "MilliData", 0, 9) == 0) 
			{
				return ScriptFunctionMilliData.Function_MilliData(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-28
            if (String.Compare(command, 0, "MilliTrend", 0, 10) == 0)
            {
                return ScriptFunctionMilliTrend.Function_MilliTrend(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-27
			if(String.Compare(command, 0, "Log", 0, 3) == 0) 
			{
				return ScriptFunctionLog.Function_Log(scriptClass, command, argument, out val);
			}*/

            /*
			if(String.Compare(command, 0, "TestGraph", 0, 9) == 0) 
			{
				return ScriptFunctionRealTimeTestGraph.Function_TestGraph(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-4-2
			if(String.Compare(command, 0, "XYGraph", 0, 7) == 0) 
			{
				return ScriptFunctionXYGraph.Function_XYGraph(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-24
			if(String.Compare(command, 0, "Excel", 0, 5) == 0) 
			{
				return ScriptFunctionExcel.Function_Excel(scriptClass, command, argument, out val);
			}*/

            /*
			if(String.Compare(command, 0, "Tag", 0, 3) == 0) 
			{
				return ScriptFunctionTag.Function_Tag(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-20
			if(String.Compare(command, 0, "Csv", 0, 3) == 0) 
			{
				return ScriptFunctionCsv.Function_Csv(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-4-1
			if(String.Compare(command, 0, "Sms", 0, 3) == 0) 
			{
				return ScriptFunctionSms.Function_Sms(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-28
			if(String.Compare(command, 0, "Menu", 0, 4) == 0) 
			{
				return ScriptFunctionMenu.Function_Menu(scriptClass, command, argument, out val);
			}*/

            /*
			if(String.Compare(command, 0, "Process", 0, 7) == 0) 
			{
				return ScriptFunctionProcess.Function_Process(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-13
			if(String.Compare(command, 0, "Object", 0, 6) == 0) 
			{
				return ScriptFunctionObject.Function_Object(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-19
			if(String.Compare(command, 0, "CommandLine", 0, 11) == 0) 
			{
				return ScriptFunctionCommandLine.Function_CommandLine(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-16
			if(String.Compare(command, 0, "Bitmap", 0, 6) == 0) 
			{
				return ScriptFunctionBitmap.Function_Bitmap(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-3-16
			if(String.Compare(command, 0, "Animation", 0, 9) == 0) 
			{
				return ScriptFunctionAnimation.Function_Animation(scriptClass, command, argument, out val);
			}*/

            /* ScriptExpandRun으로 모두 이동 2016-4-2
            if (String.Compare(command, 0, "Web", 0, 3) == 0)
            {
                return ScriptFunctionWeb.Function_Web(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-4-1
            if (String.Compare(command, 0, "Set", 0, 3) == 0)
            {
                return ScriptFunctionSet.Function_Set(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-27
            if (String.Compare(command, 0, "Get", 0, 3) == 0)
            {
                return ScriptFunctionGet.Function_Get(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-27
            if (String.Compare(command, 0, "Global", 0, 6) == 0)
            {
                return ScriptFunctionGlobal.Function_Global(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-16
            if (String.Compare(command, 0, "Schedule", 0, 8) == 0)
            {
                return ScriptFunctionSchedule.Function_Schedule(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-30
            if (String.Compare(command, 0, "String", 0, 6) == 0)
            {
                return ScriptFunctionString.Function_String(scriptClass, command, argument, out val);
            }*/

            /*
            if (String.Compare(command, 0, "System", 0, 6) == 0)
            {
                return ScriptFunctionSystem.Function_System(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-4-2
            if (String.Compare(command, 0, "ToolBar", 0, 7) == 0)
            {
                return ScriptFunctionToolBar.Function_ToolBar(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-30
            if (String.Compare(command, 0, "Print", 0, 5) == 0)
            {
                return ScriptFunctionPrint.Function_Print(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-19
            if (String.Compare(command, 0, "DatePicker", 0, 10) == 0)
            {
                return ScriptFunctionControlDatePicker.Function_DatePicker(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-16
            if (String.Compare(command, 0, "Circle", 0, 6) == 0)
            {
                return ScriptFunctionCircle.Function_Circle(scriptClass, command, argument, out val);
            }*/

            /* 2016-3-28 부터는 ScriptCall 함수가 필요없다.
            if (String.Compare(command, 0, "Mouse", 0, 5) == 0)
            {
                return ScriptFunctionMouse.Function_Mouse(scriptClass, command, argument, out val);
            }*/

            /* 2016-3-15 부터는 ScriptCall 함수가 필요없다.
            if (String.Compare(command, 0, "Script", 0, 6) == 0)
            {
                return ScriptFunctionScript.Function_Script(scriptClass, command, argument, out val);
            }*/

            /* ScriptExpandRun으로 모두 이동 2016-3-14
			retn = ScriptFunctionStringAnsi.Function_StringAnsi(scriptClass, command, argument, out val);
			if(retn != 0)	return retn;*/

            /* ScriptExpandRun으로 모두 이동 2016-3-25
			retn = ScriptFunctionFile.Function_File(scriptClass, command, argument, out val);
			if(retn != 0)	return retn;*/

            /* ScriptExpandRun으로 모두 이동 2016-3-15
			retn = ScriptFunctionAnalog.Function_Analog(scriptClass, command, argument, out val);
			if(retn != 0)	return retn;*/

            /* ScriptExpandRun으로 모두 이동 2016-3-31
			retn = ScriptFunctionTime.Function_Time(scriptClass, command, argument, out val);
			if(retn != 0)	return retn;*/

            // Functio_Get으로 이동되었다.
            //retn = ScriptFunctionProtect.Function_Protect(scriptClass, command, argument, out val);
            //if(retn != 0)	return retn;

            /* ScriptExpandRun으로 모두 이동 2016-3-28
			retn = ScriptFunctionMath.Function_Math(scriptClass, command, argument, out val);
			if(retn != 0)	return retn;*/

            /* ScriptExpandRun으로 모두 이동 2016-3-29
			retn = ScriptFunctionModule.Function_Module(scriptClass, command, argument, out val);
			if(retn != 0)	return retn;*/

            /* ScriptExpandRun으로 모두 이동 2016-3-23
			retn = ScriptFunctionElse.Function_Else(scriptClass, command, argument, out val);
			if(retn != 0)	return retn;*/

            return (0, val);    // 일치하는 함수가 없다.
        }
	}
}
