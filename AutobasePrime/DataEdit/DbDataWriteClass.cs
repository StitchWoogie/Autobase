using System;
using System.Data;
using System.IO;
using AutoLibLocal;
using AutoLib;
using NetTools;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace DataEdit
{
	/// <summary>
	/// Summary description for DbDataWriteClass.
	/// </summary>
	public class DbDataWriteClass
	{
		public DbDataWriteClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static public void MakeHeaderAndField(DateTime dTime, string tag, string des, string range, string msg)
		{
			string		filename;
			
			filename = String.Format("{0}\\DATAEDIT", DataEditConfig.sDirData);

			Directory.CreateDirectory(filename);
			filename += "\\"+DataEditConfig.sSaveFilename;
			MdbLib.MdbTool.MdbCreate(filename);

			string dsn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", filename);
			CommonDbConnection db = new CommonDbConnection(EnumDbConnectionType.OleDb, dsn, false);
			db.Open();
            
			CheckTable check = new CheckTable();			
			check.AddColumn("ModifyDate", EnumDbDataType.DateTime, 0);
			check.AddColumn("Tag", EnumDbDataType.String, 0);
			check.AddColumn("Description", EnumDbDataType.String, 0);
			check.AddColumn("ModifyRange", EnumDbDataType.String, 0);
			check.AddColumn("Message", EnumDbDataType.String, 0);
			check.Check(db, EnumDbType.MDB, "ModifyHistory");
			
			CommonDbCommand command = new CommonDbCommand(db.connType);
			command.Connection = db;
			command.CommandText = String.Format("INSERT INTO ModifyHistory (ModifyDate,Tag,Description,ModifyRange,Message) VALUES ({0},'{1}','{2}','{3}','{4}')",
				DbTool.MakeDateTimeString(EnumDbType.MDB, dTime), tag, des, range, msg);
			command.ExecuteNonQuery();
		}
		
	}
}
