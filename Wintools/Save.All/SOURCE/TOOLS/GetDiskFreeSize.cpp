#include "stdafx.h"
#include <dos.h>

#include <compiler.hpp>
#include <tools.h>

unsigned hyper GetDiskFreeSize(int disknum)
{
	unsigned hyper freesize;
   
	char   RootPathName[10];	// address of root path 
	DWORD  SectorsPerCluster;	// address of sectors per cluster 
	DWORD  BytesPerSector;	// address of bytes per sector 
	DWORD  NumberOfFreeClusters;	// address of number of free clusters  
	DWORD  TotalNumberOfClusters;	// address of total number of clusters  

	wsprintf(RootPathName, "%c:\\", disknum-1+'A');

	GetDiskFreeSpace(RootPathName, &SectorsPerCluster,
											 &BytesPerSector,
											 &NumberOfFreeClusters,
											 &TotalNumberOfClusters);
	freesize = (unsigned hyper)SectorsPerCluster*(unsigned hyper)BytesPerSector*(unsigned hyper)NumberOfFreeClusters;

	return (freesize);
}