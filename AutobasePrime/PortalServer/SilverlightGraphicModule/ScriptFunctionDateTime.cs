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
using AutoLibLocal;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionDateTime
    {
        static int ProcDateTimeNew(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            try
            {
                val = new DateTime((int)args[0], (int)args[1], (int)args[2], (int)args[3], (int)args[4], (int)args[5]);
            }
            catch (Exception exception)
            {
                scriptClass.ErrorMessage(exception.Message);
                val = 0;
                return -1;
            }
            return 1;
        }

        static int ProcDateTimeNow(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            if (method_name == "DateTimeUtcNow")
                val = DateTime.UtcNow;
            else
                val = DateTime.Now;

            return 1;
        }

        static int ProcDateTimeYear(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            DateTime t;
            try
            {
                t = (DateTime)args[0];
            }
            catch (Exception exception)
            {
                scriptClass.ErrorMessage(exception.Message);
                val = 0;
                return -1;
            }

            if (method_name == "DateTimeYear")
                val = t.Year;
            else if (method_name == "DateTimeMonth")
                val = t.Month;
            else if (method_name == "DateTimeDay")
                val = t.Day;
            else if (method_name == "DateTimeHour")
                val = t.Hour;
            else if (method_name == "DateTimeMinute")
                val = t.Minute;
            else if (method_name == "DateTimeSecond")
                val = t.Second;
            else if (method_name == "DateTimeMillisecond")
                val = t.Millisecond;
            else if (method_name == "DateTimeDayOfWeek")
                val = (int)t.DayOfWeek;
            else if (method_name == "DateTimeDayOfYear")
                val = t.DayOfYear;
            else
                val = 0;

            return 1;
        }

        static int ProcDateTimeAddYears(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            DateTime t;
            try
            {
                t = (DateTime)args[0];
            }
            catch (Exception exception)
            {
                scriptClass.ErrorMessage(exception.Message);
                val = 0;
                return -1;
            }

            int counts = (int)args[1];

            if (method_name == "DateTimeAddYears")
                val = t.AddYears(counts);
            else if (method_name == "DateTimeAddMonths")
                val = t.AddMonths(counts);
            else if (method_name == "DateTimeAddDays")
                val = t.AddDays(counts);
            else if (method_name == "DateTimeAddHours")
                val = t.AddHours(counts);
            else if (method_name == "DateTimeAddMinutes")
                val = t.AddMinutes(counts);
            else if (method_name == "DateTimeAddSeconds")
                val = t.AddSeconds(counts);
            else if (method_name == "DateTimeAddMilliseconds")
                val = t.AddMilliseconds(counts);
            else
                val = t;

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "DateTime";

            prepare.AddMethod(prename, "DateTimeNew", "object", new ScriptExternalRun.DeleMethod(ProcDateTimeNew), "in:int:year", "in:int:month", "in:int:day", "in:int:hour", "in:int:minute", "in:int:second");

            prepare.AddMethod(prename, "DateTimeNow", "object", new ScriptExternalRun.DeleMethod(ProcDateTimeNow));
            prepare.AddMethod(prename, "DateTimeUtcNow", "object", new ScriptExternalRun.DeleMethod(ProcDateTimeNow));

            prepare.AddMethod(prename, "DateTimeYear", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");
            prepare.AddMethod(prename, "DateTimeMonth", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");
            prepare.AddMethod(prename, "DateTimeDay", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");
            prepare.AddMethod(prename, "DateTimeHour", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");
            prepare.AddMethod(prename, "DateTimeMinute", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");
            prepare.AddMethod(prename, "DateTimeSecond", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");
            prepare.AddMethod(prename, "DateTimeMillisecond", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");
            prepare.AddMethod(prename, "DateTimeDayOfWeek", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");
            prepare.AddMethod(prename, "DateTimeDayOfYear", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeYear), "in:object:datetime");

            prepare.AddMethod(prename, "DateTimeAddYears", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeAddYears), "in:object:datetime", "in:int:years");
            prepare.AddMethod(prename, "DateTimeAddMonths", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeAddYears), "in:object:datetime", "in:int:months");
            prepare.AddMethod(prename, "DateTimeAddDays", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeAddYears), "in:object:datetime", "in:int:days");
            prepare.AddMethod(prename, "DateTimeAddHours", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeAddYears), "in:object:datetime", "in:int:hours");
            prepare.AddMethod(prename, "DateTimeAddMinutes", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeAddYears), "in:object:datetime", "in:int:minutes");
            prepare.AddMethod(prename, "DateTimeAddSeconds", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeAddYears), "in:object:datetime", "in:int:seconds");
            prepare.AddMethod(prename, "DateTimeAddMilliseconds", "int", new ScriptExternalRun.DeleMethod(ProcDateTimeAddYears), "in:object:datetime", "in:int:milliseconds");
        }
    }
}

