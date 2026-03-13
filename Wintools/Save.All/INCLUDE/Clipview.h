#if	!defined (__CLIPVIEW_H)
#define	__CLIPVIEW_H

typedef struct {
	WORD	struct_size;
	WORD	version;
	char  keyword[80];
	DWORD image_size;
} INFO_FILE_HEADER;

extern char *szClassNameClipViewWorkDir;
extern char *szClassNameClipViewAlbum;

void ClipViewRegisterClassAlbum(HINSTANCE hinstance);
void ClipViewRegisterClassWorkDir(HINSTANCE hinstance);

#endif

