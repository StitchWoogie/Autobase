#if	!defined (__PlcScanSharedMemory_H)
#define __PlcScanSharedMemory_H

class PlcScanSharedMemory {
	#define MAX_BUF	10000	// 사이즈는 바꾸지 않도록 한다.

#pragma pack(1)
	typedef struct {
		short	target;
		short	current;
		unsigned char buf[MAX_BUF];	
	} RING_STRUCT;

	typedef struct {
		RING_STRUCT recv;	// client는 recv,send순이고 server는 send,recv순으로 위치를 정렬해야 한다.
		RING_STRUCT send;
	} SHARED_STRUCT;
#pragma pack()

	HANDLE hHandleFile;
	SHARED_STRUCT *sharedMemory;
public:
	PlcScanSharedMemory();
	~PlcScanSharedMemory();
	void Open(char *name);
	void Close();
	void WriteByte(unsigned char b);
	void WriteBytes(unsigned char *buf, int size);
	int ReadBytes(unsigned char *buf, int size);
	bool IsOpen() { return (sharedMemory != NULL); }
	void ClearRecvBuf();
};

#endif


