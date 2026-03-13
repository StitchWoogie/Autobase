// TAG size O.K
#include "cattag.h"

#pragma pack(push, 1)

void TagShareInit(HWND hwnd);
void TagShareUnInit();

TAG_AI_STRUCT *TagShareGetAI(int pos);
TAG_AO_STRUCT *TagShareGetAO(int pos);
TAG_DI_STRUCT *TagShareGetDI(int pos);
TAG_DO_STRUCT *TagShareGetDO(int pos);
TAG_ST_STRUCT *TagShareGetST(int pos);

TAG_AI_STRUCT *TagShareGetAI(const char *tag);


int TagShareGetHapAI();
int TagShareGetHapAO();
int TagShareGetHapDI();
int TagShareGetHapDO();
int TagShareGetHapST();

#pragma pack(pop)

