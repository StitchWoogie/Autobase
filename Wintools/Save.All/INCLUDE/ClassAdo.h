#if	!defined (__ClassAdo_H)
#define	__ClassAdo_H

//#import "c:\Program Files\Common Files\system\ado\msadox.dll" no_namespace
//#import "c:\Program Files\Common Files\system\ado\msado15.dll" rename("EOF","EndOfFile")

#include <msadox.tlh>
#include <msado15.tlh>

#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

class ClassAdoConnection {
		char bOpenFlag;
		bool bShowError;
		
	public:
		CString sErrMsg;
		bool bErrorFlag;

		ClassAdoConnection(bool flag_error_show = true);
		~ClassAdoConnection();
		int IsOpen() { return bOpenFlag; }
		void Close();
		
		// provider 가 NULL이면 *.MDB를 사용한다.
		void Open(char bMdbOrString, const char *filename);
		//void Create(char *filename);
		void Create(const char *filename);

		ADODB::_ConnectionPtr pConnection;
		_CatalogPtr pCatalog;

		void ComErrorDisplay(_com_error &e, char *title, ...);
		void SetErrorShow(bool flag) { bShowError = flag; }

		void Execute(const char *command);
};

/*
class ClassAdoCatalog {
	public:
		ClassAdoCatalog();
		~ClassAdoCatalog();
		void Create(char *filename);

		_CatalogPtr pCatalog;
};
*/

class ClassAdoRecordset {
		ClassAdoConnection *dbAdo;
		//char bAddNewFlag;
		char bOpenFlag;

	public:
		ClassAdoRecordset();
		~ClassAdoRecordset();
		
		void Open(ClassAdoConnection *db, const char *table_name, enum ADODB::CursorTypeEnum cursor_type, LONG options);
		void Open(ClassAdoConnection *db, const char *table_name, enum ADODB::CursorTypeEnum cursor_type, enum ADODB::LockTypeEnum lock_type, LONG options);

		void AddNew();
		void SetFieldValue(long pos, COleVariant &var);
		void SetFieldValue(const char *field_name, COleVariant &var);
		void SetFieldValue(const char *field_name, COleDateTime &var);
		//COleVariant &GetFieldValue(const char *field_name);
		int  GetFieldValue(const char *field_name, COleVariant &var);
		int  GetFieldValue(const char *field_name, COleDateTime &var);
		enum DataTypeEnum GetFieldType(const char *field_name);
		enum DataTypeEnum GetFieldType(int pos);
		void Update();
		int  Delete();
		void DeleteAll();
		void Close();
		int  GetRecordCount();
		void MoveFirst();
		void MoveNext();
		void Move(int pos);
		int  GetFieldCount();
		int  GetFieldName(char *field, int pos);
		int  IsOpen() { return bOpenFlag; }
		int  IsFieldExist(const char *field);
		//int  CreateField(const char *field_name, enum ADODB::DataTypeEnum type, int size);
		int  EndOfFile();
		
		ADODB::_RecordsetPtr pRstThis;
		
};

class ClassAdoTableDef {
		ClassAdoConnection *dbAdo;		
	public:
		ClassAdoTableDef(ClassAdoConnection *db);
		~ClassAdoTableDef();
		
		void Create(char *table_name);
		int Open(char *table_name);
		void CreateField(const char *field_name, enum DataTypeEnum type, int size);
		void AddField(const char *field_name, enum DataTypeEnum type, int size);
		//int  GetFieldCount();
		void Append();
		void Close();
		int  IsTableExist(const char *table);
		int  GetTableCount();
		void GetTableName(int pos, CString &string);

		int  IsFieldExist(const char *field);

		_TablePtr pTableThis;
};

#endif
