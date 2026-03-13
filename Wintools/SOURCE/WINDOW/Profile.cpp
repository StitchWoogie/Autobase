#include <afxwin.h>
#include <stdio.h>
#include <dos.h>

#include <tools.h>
#include <glib.h>
 
void WritePrivateProfileInt(const TCHAR *section, const TCHAR *entry, int value, const TCHAR *filename)
{
	TCHAR buf[20];

	_stprintf(buf, _T("%d"), value);
	WritePrivateProfileString(section, entry, buf, filename);
}

void GetPrivateProfileTime(const TCHAR *section, TCHAR *entry, struct time *t, TCHAR *filename)
{
	TCHAR buf[40];
	int  hour, min, sec;
	TCHAR default_string[40];

	_stprintf(default_string, _T("%d:%d:%d"), t->ti_hour, t->ti_min, t->ti_sec);

	GetPrivateProfileString(section, entry, default_string, buf, GetArrLength(buf), filename);

	_stscanf(buf, _T("%d:%d:%d"), &hour, &min, &sec);

	if(hour < 0)	hour = 0;
	if(min  < 0)	min = 0;
	if(sec  < 0)	sec = 0;

	hour %= 24;
	min  %= 60;
	sec  %= 60;

	t->ti_hour = hour;
	t->ti_min = min;
	t->ti_sec = sec;
}
 
void WritePrivateProfileTime(const TCHAR *section, TCHAR *entry, struct time *t, TCHAR *filename)
{
	TCHAR buf[40];

	_stprintf(buf, _T("%02d:%02d:%02d"), t->ti_hour, t->ti_min, t->ti_sec);
	WritePrivateProfileString(section, entry, buf, filename);
}

void GetPrivateProfileDate(const TCHAR *section, TCHAR *entry, struct date *d, TCHAR *filename)
{
	TCHAR buf[40];
	int  year, mon, day;
	TCHAR default_string[40];

	_stprintf(default_string, _T("%d-%d-%d"), d->da_year, d->da_mon, d->da_day);

	GetPrivateProfileString(section, entry, default_string, buf, GetArrLength(buf), filename);

	_stscanf(buf, _T("%d-%d-%d"), &year, &mon, &day);

	d->da_year = year;
	d->da_mon  = mon;
	d->da_day  = day;
}
 
void WritePrivateProfileDate(const TCHAR *section, TCHAR *entry, struct date *d, TCHAR *filename)
{
	TCHAR buf[20];

	_stprintf(buf, _T("%d-%d-%d"), d->da_year, d->da_mon, d->da_day);
	WritePrivateProfileString(section, entry, buf, filename);
}

void WritePrivateProfileFloat(const TCHAR *section, TCHAR *entry, float value, TCHAR *filename)
{
	TCHAR buf[40];

	_stprintf(buf, _T("%f"), value);
	WritePrivateProfileString(section, entry, buf, filename);
}

float GetPrivateProfileFloat(const TCHAR *section, TCHAR *entry, float default_value, TCHAR *filename)
{
	TCHAR buf[40];
	TCHAR default_string[40];

	_stprintf(default_string, _T("%f"), default_value);

	GetPrivateProfileString(section, entry, default_string, buf, GetArrLength(buf), filename);

	return (float)_tstof(buf);
}

