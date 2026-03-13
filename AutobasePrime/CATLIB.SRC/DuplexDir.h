#if	!defined (__DuplexDir_H)
#define __DuplexDir_H

#pragma pack(push, 1)

#include <tools.h>

typedef struct {
	char	bUse;
	char	sPrimary[MAXPATH];
	char	sSecondary[MAXPATH];
} DUPLEX_DIR_STRUCT;

#pragma pack(pop)

#endif

