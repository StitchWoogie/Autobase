using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightGraphicModule
{
    class SystemValue
    {
        public static void Init()
        {
            SilverlightGraphicModule.ScriptFunctionSystem.procSystemValueGet = new SilverlightGraphicModule.ScriptFunctionSystem.DeleSystemValueGet(SystemValueGet);
            SilverlightGraphicModule.ScriptFunctionSystem.procSystemValueSet = new SilverlightGraphicModule.ScriptFunctionSystem.DeleSystemValueSet(SystemValueSet);
        }

        public static object SystemValueGet(EnumSystemValue item)
        {
            /*
            if (item == EnumSystemValue.LocalMainSharedDatabaseActive)
                return SharedDatabase.configWebServer.bActive;
            else if (item == EnumSystemValue.LocalMainSharedDatabaseSaveAlarmTable)
                return SharedDatabase.configWebServer.bTableAlarmFile;
            else if (item == EnumSystemValue.LocalMainSharedDatabaseSaveTagTable)
                return SharedDatabase.configWebServer.bTableTagExchange;

            else if (item == EnumSystemValue.LocalMainScheduleActive)
                return CheckEngineSchedule.bActiveSchedule;
            else if (item == EnumSystemValue.LocalMainScriptActive)
                return CheckEngineAlwaysScript.bActiveScript;*/

            if (item == EnumSystemValue.UserControlBoxDescription)
                return SilverlightDialogControl.MyDialogUserControl.sTempUserControlBoxDescription;
            else if (item == EnumSystemValue.UserControlBoxMaxValue)
                return SilverlightDialogControl.MyDialogUserControl.sTempUserControlBoxMaxValue;
            else if (item == EnumSystemValue.UserControlBoxMinValue)
                return SilverlightDialogControl.MyDialogUserControl.sTempUserControlBoxMinValue;
            else if (item == EnumSystemValue.UserControlBoxTitle)
                return SilverlightDialogControl.MyDialogUserControl.sTempUserControlBoxTitle;
            else if (item == EnumSystemValue.UserControlBoxTag)
                return SilverlightDialogControl.MyDialogUserControl.sTempUserControlBoxTag;

            else
                return 0;
        }

        public static int SystemValueSet(EnumSystemValue item, object data)
        {
            int retn = 1;

            /*
            if (item == EnumSystemValue.LocalMainSharedDatabaseActive)
                SharedDatabase.configWebServer.bActive = ConvertTool.ToBoolean(data);
            else if (item == EnumSystemValue.LocalMainSharedDatabaseSaveAlarmTable)
                SharedDatabase.configWebServer.bTableAlarmFile = ConvertTool.ToBoolean(data);
            else if (item == EnumSystemValue.LocalMainSharedDatabaseSaveTagTable)
                SharedDatabase.configWebServer.bTableTagExchange = ConvertTool.ToBoolean(data);

            else if (item == EnumSystemValue.LocalMainScheduleActive)
            {
                bool old_value = CheckEngineSchedule.bActiveSchedule;

                CheckEngineSchedule.bActiveSchedule = ConvertTool.ToBoolean(data);

                FormLocalMain.formMain.Invalidate(true);

                if (old_value == false && CheckEngineSchedule.bActiveSchedule == true)
                {
                    FormSchedule.OnScheduleStructChanged();
                }
            }
            else if (item == EnumSystemValue.LocalMainScriptActive)
            {
                CheckEngineAlwaysScript.bActiveScript = ConvertTool.ToBoolean(data);
                FormAlwaysScript.SetTitle();
            }

            else*/
                retn = 0;

            return retn;
        }
    }
}
