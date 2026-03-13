#include "stdafx.h"
#include <tools.h>
void Temp(int& temp1, int& temp2)
{
   int temp;
   temp = temp1;
   temp1 = temp2;
   temp2 = temp;
}

void Temp(char& temp1, char& temp2)
{
   char temp;
   temp = temp1;
   temp1 = temp2;
   temp2 = temp;
}

void Temp(float& temp1, float& temp2)
{
   float temp;
   temp = temp1;
   temp1 = temp2;
   temp2 = temp;
}

void Temp(long& temp1, long& temp2)
{
   long temp;
   temp = temp1;
   temp1 = temp2;
   temp2 = temp;
}

void Temp(double& temp1, double& temp2)
{
   double temp;
   temp = temp1;
   temp1 = temp2;
   temp2 = temp;
}

void Temp(DWORD &temp1, DWORD &temp2)
{
   DWORD temp;
   temp = temp1;
   temp1 = temp2;
   temp2 = temp;
}
