// TAG size O.K

// english O.K
#include "stdafx.h"

#include <dataswap.h>
#include <ClassAdo.h>
#include <tools.h>

#define TESTHR(x) if FAILED(x) _com_issue_error(hr);

void ClassAdoConnection::ComErrorDisplay(_com_error &e, char *title, ...)
{
	bErrorFlag = ON;
	va_list ap;
	StackChar imsi(1000);

	va_start(ap, title);
	vsprintf(imsi.data, (const char*)title, ap);
	va_end(ap);

	_bstr_t bstrSource(e.Source());
	_bstr_t bstrDescription(e.Description());
	
	sErrMsg.Format("Error=%s\nSource:%s\ndescription:%s\nErrorMessage:%s",imsi.data, (LPCSTR)bstrSource,(LPCSTR)bstrDescription, e.ErrorMessage());

	if(bShowError) {
		AfxGetMainWnd()->MessageBox(sErrMsg, "Error", MB_OK);
	}
}

ClassAdoConnection::ClassAdoConnection(bool flag_error_show)
{
	HRESULT hr = S_OK;
	bOpenFlag = OFF;
	bShowError = flag_error_show;
	bErrorFlag = OFF;

	try {
		TESTHR(hr = pCatalog.CreateInstance(__uuidof (Catalog)));
		//TESTHR(hr = pConnection.CreateInstance(__uuidof (ADODB::Connection)));
	}
	catch(_com_error &e) {
		ComErrorDisplay(e, "ClassAdoConnection::ClassAdoConnection Catalog");
	}

	try {
		//TESTHR(hr = pCatalog.CreateInstance(__uuidof (Catalog)));
		TESTHR(hr = pConnection.CreateInstance(__uuidof (ADODB::Connection)));
	}
	catch(_com_error &e) {
		ComErrorDisplay(e, "ClassAdoConnection::ClassAdoConnection Connection");
	}
}

ClassAdoConnection::~ClassAdoConnection()
{
	Close();
}

/*
void ClassAdoConnection::Create(char *filename)
{
	HRESULT hr = S_OK;

	char connect_string[MAXPATH];

	sprintf(connect_string, "Provider=Microsoft.JET.OLEDB.4.0;Data source = %s;", filename);

	_bstr_t strcnn(connect_string);

	try
	{
		pCatalog->Create(strcnn);
		//pCatalog->PutActiveConnection(strcnn);

		pConnection->Open(strcnn, "", "", ADODB::adConnectUnspecified);
		//pCatalog->PutActiveConnection(pConnection);
		pCatalog->PutActiveConnection(strcnn);
		bOpenFlag = ON;
	}
	catch(_com_error &e) {
		ComErrorDisplay(e, "ClassAdoConnection::Create");
	}
}
*/

void ClassAdoConnection::Open(char bMdbOrString, const char *filename)
{
	HRESULT hr = S_OK;

	CString connect_string;

	if(bMdbOrString == 0) {
		connect_string.Format("Provider=Microsoft.JET.OLEDB.4.0;Data source=%s;", filename);
	}
	else {
		connect_string.Format("%s", filename);
	}

	_bstr_t strcnn(connect_string);

	try
	{
		//pCatalog->PutActiveConnection(strcnn);
		//pConnection->Mode = ADODB::adModeReadWrite;
		pConnection->Open(strcnn, "", "", ADODB::adConnectUnspecified);
		//pCatalog->PutActiveConnection(pConnection);
		//pCatalog->PutActiveConnection(strcnn);
		//pCatalog->ActiveConnection = pConnection;
		pCatalog->PutActiveConnection(_variant_t((IDispatch *)pConnection));
		bOpenFlag = ON;
	}
	catch(_com_error &e) {
		ComErrorDisplay(e, "ClassAdoConnection::Open");
	}
}

void ClassAdoConnection::Close()
{
	if(bOpenFlag == OFF)	return;
	bOpenFlag = OFF;
	pConnection->Close();
}

void ClassAdoConnection::Create(const char *filename)
{
	HRESULT hr = S_OK;

	char connect_string[MAXPATH];

	sprintf(connect_string, "Provider=Microsoft.JET.OLEDB.4.0;Data source = %s;", filename);

	_bstr_t strcnn(connect_string);

	try
	{
		pCatalog->Create(strcnn);
		pCatalog->PutActiveConnection(strcnn);

		//pConnection->Open(strcnn, "", "", ADODB::adConnectUnspecified);
		//bOpenFlag = ON;
	}
	catch(_com_error &e) {
		ComErrorDisplay(e, "ClassAdoConnection::Create");
	}
}

void ClassAdoConnection::Execute(const char *command)
{
	_bstr_t strSqlCommand(command);
	try {
		pConnection->Execute(strSqlCommand, NULL, ADODB::adExecuteNoRecords);
	}
	catch(_com_error &e) {
		ComErrorDisplay(e, "ClassAdoConnection::Execute");
	}
}

/*
ClassAdoCatalog::ClassAdoCatalog()
{
	HRESULT hr = S_OK;

	try {
		TESTHR(hr = pCatalog.CreateInstance(__uuidof (Catalog)));
		//TESTHR(hr = pConnection.CreateInstance(__uuidof (ADODB::Connection)));
	}
	catch(_com_error &e) {
		ComErrorDisplay(e, "ClassAdoCatalog::ClassAdoCatalog");
	}
}

ClassAdoCatalog::~ClassAdoCatalog()
{

}

void ClassAdoCatalog::Create(char *filename)
{
	HRESULT hr = S_OK;

	char connect_string[MAXPATH];

	sprintf(connect_string, "Provider=Microsoft.JET.OLEDB.4.0;Data source = %s;", filename);

	_bstr_t strcnn(connect_string);

	try
	{
		pCatalog->Create(strcnn);
		pCatalog->PutActiveConnection(strcnn);

		//pConnection->Open(strcnn, "", "", ADODB::adConnectUnspecified);
		//bOpenFlag = ON;
	}
	catch(_com_error &e) {
		ComErrorDisplay(e, "ClassAdoCatalog::Create");
	}
}
*/

ClassAdoRecordset::ClassAdoRecordset()
{
	HRESULT hr = S_OK;

	try {
		TESTHR(hr = pRstThis.CreateInstance(__uuidof(ADODB::Recordset)));
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordSet::ClassAdoRecordSet");
	}

	//dbAdo = db;
	bOpenFlag = OFF;
}

ClassAdoRecordset::~ClassAdoRecordset()
{
	Close();
}

void ClassAdoRecordset::Open(ClassAdoConnection *db, const char *table_name, enum ADODB::CursorTypeEnum cursor_type, enum ADODB::LockTypeEnum lock_type, LONG options)
{
	// You have to explicitly pass the Cursor type and LockType to the Recordset here
	HRESULT hr = S_OK;

	dbAdo = db;

	try {
		TESTHR(hr = pRstThis.CreateInstance(__uuidof(ADODB::Recordset)));
		pRstThis->CursorLocation = ADODB::adUseClient;
		pRstThis->Open(table_name,_variant_t((IDispatch *) dbAdo->pConnection, true),cursor_type,lock_type,options);

		//pRstThis->Supports(ADODB::adAddNew);
		bOpenFlag = ON;
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordSet::Open");
	}
}

void ClassAdoRecordset::Open(ClassAdoConnection *db, const char *table_name, enum ADODB::CursorTypeEnum cursor_type, LONG options)
{
	// You have to explicitly pass the Cursor type and LockType to the Recordset here
	HRESULT hr = S_OK;

	dbAdo = db;

	try {
		TESTHR(hr = pRstThis.CreateInstance(__uuidof(ADODB::Recordset)));
		pRstThis->CursorLocation = ADODB::adUseClient;
		pRstThis->Open(table_name,_variant_t((IDispatch *) dbAdo->pConnection, true),cursor_type,ADODB::adLockOptimistic,options);

		//pRstThis->Supports(ADODB::adAddNew);
		bOpenFlag = ON;
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordSet::Open");
	}
}

void ClassAdoRecordset::AddNew()
{
	if(bOpenFlag == OFF)	return;

	try {
		pRstThis->AddNew();
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordSet::AddNew");
	}
}

void ClassAdoRecordset::SetFieldValue(long field_pos, COleVariant &var)
{
	if(bOpenFlag == OFF)	return;
	
	try {
		pRstThis->Fields->GetItem(field_pos)->Value = var;	
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordSet::SetFieldValue\nFieldPos=%d", field_pos);
	}
}

void ClassAdoRecordset::SetFieldValue(const char *field_name, COleVariant &var)
{
	if(bOpenFlag == OFF)	return;
	
	try {
		pRstThis->Fields->GetItem(field_name)->Value = var;	
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordSet::SetFieldValue Field=%s", field_name);
	}
}

void ClassAdoRecordset::SetFieldValue(const char *field_name, COleDateTime &var)
{
	if(bOpenFlag == OFF)	return;

	try {
		pRstThis->Fields->GetItem(field_name)->Value = (COleVariant)var;
	}
	catch (_com_error &/*e*/) {

	}
}

int ClassAdoRecordset::GetFieldValue(const char *field_name, COleVariant &var)
{
	if(bOpenFlag == OFF)	return 0;

	try {
		CString imsi;
		imsi.Format("""%s""", field_name);
		var = pRstThis->Fields->GetItem((const char*)imsi)->Value;
	}
	catch(_com_error &/*e*/) {
		var = field_name;
		//ComErrorDisplay(e, "ClassAdoRecordset::GetFieldValue\nField=%s", field_name);
	}
	
	return 1;
}

enum DataTypeEnum ClassAdoRecordset::GetFieldType(const char *field_name)
{
	if(bOpenFlag == OFF)	return adError;

	try {
		PropertyPtr pProperty = NULL;
		_variant_t vIndex;

		vIndex = (short)0;

		pProperty = pRstThis->Properties->GetItem(vIndex);

		return adError;

	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordset::GetFieldType\nField=%s", field_name);
	}
	
	return adError;
}

int ClassAdoRecordset::GetFieldValue(const char *field_name, COleDateTime &var)
{
	if(bOpenFlag == OFF)	return 0;
	
	var = pRstThis->Fields->GetItem(field_name)->Value;

	return 1;
}

void ClassAdoRecordset::Update()
{
	if(bOpenFlag == OFF)	return;
	
	try {
		pRstThis->Update();
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordset::Update");
	}
}

int ClassAdoRecordset::Delete()
{
	if(bOpenFlag == OFF)	return 0;
	
	try {
		pRstThis->Delete(ADODB::adAffectCurrent);
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordset::Delete");
		return 0;
	}

	return 1;
}

void ClassAdoRecordset::DeleteAll()
{
	if(bOpenFlag == OFF)	return;
	
	try {
		//pRstThis->Delete(ADODB::adAffectAll);
		pRstThis->Filter = "";
		pRstThis->Delete(ADODB::adAffectGroup);
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordset::DeleteAll");
	}
}

void ClassAdoRecordset::Close()
{
	if(bOpenFlag == OFF)	return;

	bOpenFlag = OFF;

	try {
		pRstThis->Close();
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordset::Close");
	}
}

int  ClassAdoRecordset::GetRecordCount()
{
	if(bOpenFlag == OFF)	return 0;

	int count = pRstThis->GetRecordCount();
	if(count < 0)
		count = 0;

	return count;
}

int  ClassAdoRecordset::EndOfFile()
{
	if(bOpenFlag == OFF)	return 1;

	return pRstThis->EndOfFile;
}

/*
void ClassAdoRecordset::Resync()
{
	if(bOpenFlag == OFF)	return;

	pRstThis->CursorLocation = ADODB::adUseClient;
	pRstThis->Resync(ADODB::adAffectAll, ADODB::adResyncAllValues);
}
*/

void ClassAdoRecordset::MoveFirst()
{
	if(bOpenFlag == OFF)	return;

	pRstThis->MoveFirst();
}

void ClassAdoRecordset::MoveNext()
{
	if(bOpenFlag == OFF)	return;

	try {
		pRstThis->MoveNext();
	}
	catch(_com_error &/*e*/) {
		//dbAdo->ComErrorDisplay(e, "ClassAdoRecordset::MoveNext");
	}
}

void ClassAdoRecordset::Move(int pos)
{
	if(bOpenFlag == OFF)	return;

	try {
		pRstThis->Move(pos);
		
	}
	catch(_com_error &/*e*/) {
		//dbAdo->ComErrorDisplay(e, "ClassAdoRecordset::MoveNext");
	}
}

int ClassAdoRecordset::GetFieldCount()
{
	if(bOpenFlag == OFF)	return 0;

	return pRstThis->Fields->GetCount();
}

int ClassAdoRecordset::GetFieldName(char *field, int pos)
{
	if(bOpenFlag == OFF) {
		field[0] = 0;
		return 0;
	}

	COleVariant var;
	var = pRstThis->Fields->Item[(short)pos]->GetName();

	CString str;
	str = var.bstrVal;
	strcpy(field, str);

	return 1;
}

int ClassAdoRecordset::IsFieldExist(const char *field)
{
	if(bOpenFlag == OFF) {
		return 0;
	}

	COleVariant var;
	int i;
	CString imsi;

	for(i = 0; i < pRstThis->Fields->GetCount(); i++) {
		var = pRstThis->Fields->Item[(short)i]->GetName();
		imsi = var.bstrVal;
		if(stricmp(imsi, field) == 0)	return 1;
	}

	return 0;
}

/*
int ClassAdoRecordset::CreateField(const char *field_name, enum ADODB::DataTypeEnum type, int size)
{
	if(bOpenFlag == OFF) {
		return 0;
	}

	try {
		pRstThis->Fields->Append(field_name, type, size, ADODB::adFldFixed);
		//pRstThis->Fields->Update();
		return 1;
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoRecordset::CreateField\nField=%s", field_name);
		return 0;
	}

	return 1;
}
*/

ClassAdoTableDef::ClassAdoTableDef(ClassAdoConnection *db)
{
	dbAdo = db;
	
	HRESULT hr = S_OK;

	try {
		TESTHR(hr = pTableThis.CreateInstance(__uuidof (Table)));
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoTableDef::ClassAdoTableDef");
	}
}

ClassAdoTableDef::~ClassAdoTableDef()
{

}

void ClassAdoTableDef::Create(char *table_name)
{
	HRESULT hr = S_OK;

	TESTHR(hr = pTableThis.CreateInstance(__uuidof (Table)));
	pTableThis->PutName(table_name);

	/*
	pTable->Columns->Append("EventTime",adDate,0);
	pTable->Columns->Append("EventTimeMilliSec",adSmallInt,0);
	pTable->Columns->Append("Tag",adVarWChar,20);
	pTable->Columns->Append("Description",adVarWChar,50);
	pTable->Columns->Append("Message",adVarWChar,80);
	pTable->Columns->Append("Username",adVarWChar,20);
	pTable->Columns->Append("Port",adInteger,0);
	pTable->Columns->Append("Station",adInteger,0);
	pTable->Columns->Append("Type",adInteger,0);
	pTable->Columns->Append("Address",adInteger,0);
			
	pTable->Columns->GetItem("EventTime")->Attributes = adColNullable;
	pTable->Columns->GetItem("EventTimeMilliSec")->Attributes = adColNullable;
	pTable->Columns->GetItem("Tag")->Attributes = adColNullable;
			pTable->Columns->GetItem("Description")->Attributes = adColNullable;
			pTable->Columns->GetItem("Message")->Attributes = adColNullable;
			pTable->Columns->GetItem("Username")->Attributes = adColNullable;
			pTable->Columns->GetItem("Port")->Attributes = adColNullable;
			pTable->Columns->GetItem("Station")->Attributes = adColNullable;
			pTable->Columns->GetItem("Type")->Attributes = adColNullable;
			pTable->Columns->GetItem("Address")->Attributes = adColNullable;
	*/

	//pCatalog->Tables->Append(_variant_t((IDispatch *)pTable));
}

int ClassAdoTableDef::Open(char *table_name)
{
//	HRESULT hr = S_OK;

//	TESTHR(hr = pTableThis.CreateInstance(__uuidof (Table)));
	pTableThis = dbAdo->pCatalog->Tables->GetItem(table_name);

	if(pTableThis)	return 1;
	return 0;
	//pTableThis->PutName(table_name);
	//pTableThis->ParentCatalog = dbAdo->pCatalog;
}

void ClassAdoTableDef::CreateField(const char *field_name, enum DataTypeEnum type, int size)
{
	try {
		pTableThis->Columns->Append(field_name, type, size);
		pTableThis->Columns->GetItem(field_name)->PutAttributes(adColNullable);
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoTableDef::CreateField\nField=%s", field_name);
	}
}

void ClassAdoTableDef::AddField(const char *field_name, enum DataTypeEnum type, int size)
{
	try {
		_ColumnPtr ptr = NULL;

		HRESULT hr = S_OK;
		TESTHR(hr = ptr.CreateInstance(__uuidof (Column)));

		ptr->Name = field_name;
		ptr->Type = type;
		ptr->DefinedSize = size;
		ptr->Attributes = adColNullable;

		pTableThis->Columns->Append(field_name, type, size);
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoTableDef::AddField\nField=%s", field_name);
	}
}

int  ClassAdoTableDef::IsFieldExist(const char *field)
{
	COleVariant var;
	COleVariant vars(field);
	int i;

	for(i = 0; i < pTableThis->Columns->GetCount(); i++) {
		var = pTableThis->Columns->GetItem((short)i)->GetName();

		if(vars == var)	return 1;
	}

	return 0;
}

void ClassAdoTableDef::Append()
{
	try {
		dbAdo->pCatalog->Tables->Append(_variant_t((IDispatch *)pTableThis));
	}
	catch(_com_error &e) {
		dbAdo->ComErrorDisplay(e, "ClassAdoTableDef::Append");
	}
}

void ClassAdoTableDef::Close()
{
	//dbAdo->ReOpen();
}

int ClassAdoTableDef::IsTableExist(const char *table)
{
	int count = dbAdo->pCatalog->Tables->GetCount();

	COleVariant var;
	COleVariant vars(table);

	for(int i = 0; i < count; i++) {
		var = dbAdo->pCatalog->Tables->Item[(short)i]->GetName();
		if(vars == var)	return 1;
	}
	
	return 0;
}

int ClassAdoTableDef::GetTableCount()
{
	return dbAdo->pCatalog->Tables->GetCount();
}

void ClassAdoTableDef::GetTableName(int pos, CString &string)
{
	COleVariant var = dbAdo->pCatalog->Tables->Item[(short)pos]->GetName();

	string = var.bstrVal;
}







