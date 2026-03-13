// TAG size O.K
#if	!defined (__CATTAG9_H)
#define __CATTAG9_H

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#if	!defined (__TOOLS_H)
#include <tools.h>
#endif

#pragma pack(push, 1)

typedef struct {
	int		nTagType;
	char	tag[256];
	char	description[256];
	int		tag_pos;
} TagPublicStruct;

extern Block blockTagList9;

void TagLoad9();

#pragma pack(pop)

#endif




