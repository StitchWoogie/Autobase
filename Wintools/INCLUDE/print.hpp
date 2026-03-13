#if	!defined (__PRINT_HPP)
#define __PRINT_HPP

#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

#pragma pack(push, 1)

typedef struct {
	char sDevice[80];
	char sDriver[MAXPATH];
	char sPort[80];
} PRINT_DRIVER_LIST;

typedef struct {
		int	nHorzRes;		// pixel width
		int	nVertRes;		// pixel height
		int	nLogPixelSX;
		int	nLogPixelSY;
		int nPhysicalWidth;
		int	nPhysicalHeight;
		int	nPhysicalOffsetX;
		int	nPhysicalOffsetY;
} PRINT_INFO_STRUCT;

void PrintDriverFillDefault(PRINT_DRIVER_LIST *driver);
void PrintDriverFillList(Block *block);
void PrintDriverFillComboBox(HWND hwndCombo, Block *block, PRINT_DRIVER_LIST *defaultDriver);
void PrintDriverGetSelectedDriver(HWND hwndCombo, Block *block, PRINT_DRIVER_LIST *driver);

DEVMODE *CreateDevMode(char *sDevice);	// DEVMODE를 만든다.

#pragma pack(pop)

#endif 