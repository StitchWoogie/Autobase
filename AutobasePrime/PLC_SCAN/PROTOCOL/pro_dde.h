#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

#if	!defined (__DDEML_H)
#include <ddeml.h>
#endif

typedef struct {
	char	sService[10];
	HSZ	hszService;
	Block	*blockHCONV;
} LOCAL_PROTOCOL_DDE;


