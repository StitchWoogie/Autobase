using System;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;
using NetTools;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ScriptFunctionComboBox.
	/// </summary>
	public class ScriptFunctionDatabase
	{
        /*
		public ScriptFunctionDatabase()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static int Function_DatabaseSetFilter(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;;
			string class_name;
			string string_where;
			string string_orderby;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out class_name))		return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out string_where))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out string_orderby))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				val = scriptClass.ExecuteClassName(ObjectDatabase.arrayClassList, class_name, command, string_where, string_orderby);
			}

			return 1;
		}

		static int Function_DatabaseReLoad(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;;
			string class_name;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out class_name))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				val = scriptClass.ExecuteClassName(ObjectDatabase.arrayClassList, class_name, command);
			}

			return 1;
		}

		static int Function_DatabaseGetCurSel(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string class_name;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out class_name))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				val = scriptClass.ExecuteClassName(ObjectDatabase.arrayClassList, class_name, command);
			}

			return 1;
		}

		static int Function_DatabaseSetCurSel(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string class_name;
			int pos;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out class_name))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out pos))				return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				val = scriptClass.ExecuteClassName(ObjectDatabase.arrayClassList, class_name, command, (int)pos);
			}

			return 1;
		}

		static int Function_DatabaseGetValue(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string class_name;
			string field_name;
			string var;
			int pos;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out class_name))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out pos))				return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out field_name))	return -1;
			arg.GetArgument(out var);
			if(!scriptClass.IsStringVar(var, "DatabaseGetValue() arg4"))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				string result;
				result = (string)scriptClass.ExecuteClassNameStringReturn(ObjectDatabase.arrayClassList, class_name, command, (int)pos, field_name);
				if(result != null) 
					if(!scriptClass.ChangeStringVar(var, result))		return -1;
			}

			return 1;
		}

		static int Function_DatabaseSetConnection(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string class_name;
			string connection;
			string table;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out class_name))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out connection))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out table))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				val = scriptClass.ExecuteClassName(ObjectDatabase.arrayClassList, class_name, command, connection, table);
			}

			return 1;
		}

        static int Function_DatabaseSetSelect(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string class_name;
            string select;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out class_name)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out select)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = scriptClass.ExecuteClassName(ObjectDatabase.arrayClassList, class_name, command, select);
            }

            return 1;
        }

		static int Function_DatabaseSetTable(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string class_name;
			string table;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out class_name))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out table))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				val = scriptClass.ExecuteClassName(ObjectDatabase.arrayClassList, class_name, command, table);
			}

			return 1;
		}

		public static int Function_Database(ScriptClass scriptClass, string command, string argument, out object val)
		{
			if(command == "DatabaseSetFilter") 
			{
				return Function_DatabaseSetFilter(scriptClass, command, argument, out val);
			}
			else if(command == "DatabaseReLoad") 
			{
				return Function_DatabaseReLoad(scriptClass, command, argument, out val);
			}
			else if(command == "DatabaseGetCurSel") 
			{
				return Function_DatabaseGetCurSel(scriptClass, command, argument, out val);
			}
			else if(command == "DatabaseSetCurSel") 
			{
				return Function_DatabaseSetCurSel(scriptClass, command, argument, out val);
			}
			else if(command == "DatabaseGetValue") 
			{
				return Function_DatabaseGetValue(scriptClass, command, argument, out val);
			}
			else if(command == "DatabaseSetConnection") 
			{
				return Function_DatabaseSetConnection(scriptClass, command, argument, out val);
			}
            else if (command == "DatabaseSetSelect")
            {
                return Function_DatabaseSetSelect(scriptClass, command, argument, out val);
            }
			else if(command == "DatabaseSetTable") 
			{
				return Function_DatabaseSetTable(scriptClass, command, argument, out val);
			}
			else 
			{
				if(Tools.IsLangKorean()) 
				{
					scriptClass.ErrorMessage(String.Format("지원되지 않는 Database 함수입니다.\n({0})", command));
				}
				else if(Tools.IsLangChinese()) 
				{
					scriptClass.ErrorMessage(String.Format("不支持的 Database 函数。({0})", command));
				}
				else 
				{
					scriptClass.ErrorMessage(String.Format("Undefined Database function.\n({0})", command));
				}
				val = 0;
				return -1;
			}
		}*/

        static async Task<(int, object val)> Run_ObjectPublic(ScriptClass scriptClass, string method_name,  object[] args)
        {
            object val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = await scriptClass.ExecuteClassName(ObjectDatabase.arrayClassList, (string)args[0], method_name, args);
            }

            return (1, val);
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            if (!TotalConfig.GetOemTypeRight(EnumOemTypeRight.Database)) return;  //20250113 PSU
            string prename = "Database";

            prepare.AddMethod(prename, "DatabaseGetCurSel", "int", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname");
            prepare.AddMethod(prename, "DatabaseGetValue", "int", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:int:row_pos", "in:string:column", "out:string:data");
            prepare.AddMethod(prename, "DatabaseReLoad", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname");
            prepare.AddMethod(prename, "DatabaseSetConnection", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:string:connection", "in:string:table");
            prepare.AddMethod(prename, "DatabaseSetCurSel", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:int:index");
            prepare.AddMethod(prename, "DatabaseSetFilter", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:string:where", "in:string:orderby");
            prepare.AddMethod(prename, "DatabaseSetSelect", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:string:select");
            prepare.AddMethod(prename, "DatabaseSetTable", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:string:table");
			prepare.AddMethod(prename, "DatabaseSetRecordLimit", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:int:limit");
        }
	}
}

