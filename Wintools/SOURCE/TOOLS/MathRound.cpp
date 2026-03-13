#include "stdafx.h"
#include <dos.h>

#include <tools.h>

int MathRound(double f)
{
	CString str;

	str.Format("%.0f", f);

	return atoi(str);
}





