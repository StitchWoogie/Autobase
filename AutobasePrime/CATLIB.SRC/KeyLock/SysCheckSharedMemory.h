

#pragma pack(push, 1)
	typedef struct {
		char	 done;
		char	 lock_type;
		DWORD	 serial_number;
		DWORD	 seed;
		unsigned char buf[MAX_BUF_SYS_CHECK];
	} SHARED_STRUCT_SYS_CHECK;





class SysCheckSharedMemory {
	



	HANDLE hHandleFile;
	
public:
	SHARED_STRUCT_SYS_CHECK *sharedMemory;
	SysCheckSharedMemory() 
	{
		hHandleFile = NULL;
		sharedMemory = NULL;
	}

	~SysCheckSharedMemory() 
	{
		Close();
	}

	void Open()
	{
		char *name = "_S_ys_Ch_ec_k";
		hHandleFile = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(SHARED_STRUCT_SYS_CHECK), name);

		if(hHandleFile == NULL)	return;

		bool exist_flag = false;

		if(GetLastError() == ERROR_ALREADY_EXISTS) 
			exist_flag = true;

		sharedMemory = (SHARED_STRUCT_SYS_CHECK*)MapViewOfFile(hHandleFile, FILE_MAP_WRITE, 0, 0, 0);

		if(sharedMemory != NULL) {
			ZeroMemory(sharedMemory, sizeof(SHARED_STRUCT_SYS_CHECK));
		}
	}

	void Close()
	{
		if(sharedMemory != NULL) {
			UnmapViewOfFile(sharedMemory);
			sharedMemory = NULL;
		}
		if(hHandleFile) {
			CloseHandle(hHandleFile);
			hHandleFile = NULL;
		}
	}

	bool IsOpen() { return (sharedMemory != NULL); }
};

#pragma pack(pop)

