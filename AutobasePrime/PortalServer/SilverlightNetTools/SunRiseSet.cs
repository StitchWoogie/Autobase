using System;
using NetTools.OldDefine;

namespace NetTools
{
	/// <summary>
	/// Summary description for SunRiseSet.
	/// </summary>
	public class SunRiseSet
	{
		static double P1 = 3.14159265;
		static double P2 = 2 * P1;
		static double DR = P1 / 180;
		static double K1 = 15 * DR * 1.0027379;

		static double G;
		static double T, TT;

		static double S, A5, D5, R5;

		public SunRiseSet()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static int GetTime(int org_year, int org_mon, int day, double longitude, double latitude, int gh, SYSTEMTIME tRise, SYSTEMTIME tSet)
		{
			tRise.wHour = 0;
			tRise.wMinute = 0;
			tRise.wSecond = 0;
			tSet.wHour = 0;
			tSet.wMinute = 0;
			tSet.wSecond = 0;

			double[] aA = new double[3];
			double[] aD = new double[3];
	
			//CString S$ = "Sunset at  ";
			//CString R$ = "Sunrise at ";
			//CString M1$ = "No sunrise this date";
			//CString M2$ = "No sunset this date";
			//CString M3$ = "Sun down all day";
			//CString M4$ = "Sun up all day";

			double B5 = latitude;
			double L5 = longitude;
			double H = gh;

			L5 = L5 / 360;
			double Z0 = H / 24;

			//60 GoSub 1170:
			//1170 '     Calendar --> JD
			double Y = org_year;
			double M = org_mon;
			double D = day;

			G = 1;
			if(Y < 1583) G = 0;
			double D1 = (int)D;
			double F = D - D1 - 0.5;
			double J = -(int)(7 * ((int)((M + 9) / 12) + Y) / 4);
			double A;
			double J3=0;

			if(G == 0) 
			{ //Then GoTo 1260
				
			}
			else 
			{
				S = Sgn(M - 9);
				A = Math.Abs(M - 9);
				J3 = (int)(Y + S * (int)(A / 7));
				J3 = -(int)(((int)(J3 / 100) + 1) * 3 / 4);
			}

			J = J + (int)(275 * M / 9) + D1 + G * J3;
			J = J + 1721027 + 2 * G + 367 * Y;
			if(F >= 0) 
			{// Then GoTo 1300

			}
			else 
			{
				F = F + 1;
				J = J - 1;
			}

			T = (J - 2451545) + F;
			TT = T / 36525 + 1;//: ' TT = centuries
			//80 '               from 1900.0

			//90 GoSub 410:
			//410 '     LST at 0h zone time
			double T0 = T / 36525;
			S = 24110.5 + 8640184.813 * T0;
			S = S + 86636.6 * Z0 + 86400 * L5;
			S = S / 86400;
			S = S - (int)(S);
			T0 = S * 360 * DR;
	
			T = T + Z0;
			//100 '
			//110 '       Get Sun's Position

			//120 GoSub 910:
			Function910();
			aA[1] = A5;
			aD[1] = D5;
			T = T + 1;

			//140 GoSub 910:
			Function910();
			aA[2] = A5;
			aD[2] = D5;

			if(aA[2] < aA[1]) aA[2] = aA[2] + P2;
			double Z1 = DR * 90.833;//: ' Zenith dist.
			S = Math.Sin(B5 * DR);
			double C = Math.Cos(B5 * DR);
			double Z = Math.Cos(Z1);

			double M8 = 0;
			double W8 = 0;
			//Print
			double A0 = aA[1];
			double D0 = aD[1];
			double DA = aA[2] - aA[1];
			double DD = aD[2] - aD[1];

			double V0=0;
			double V2=0;

			for(double C0 = 0; C0 <= 23; C0++) 
			{	// ?????? <= 23 or < 23
				double P = (C0 + 1) / 24;
				double A2 = aA[1] + P * DA;
				double D2 = aD[1] + P * DD;
		
				//GoSub 490
				//490 '  Test an hour for an event
				double L0 = T0 + C0 * K1;
				double L2 = L0 + K1;
				double H0 = L0 - A0;
				double H2 = L2 - A2;
				double H1 = (H2 + H0) / 2;//: '  Hour angle,
				D1 = (D2 + D0) / 2;//: '  declination,
				//540 '                at half hour
		
				if(C0 > 0) 
				{ //Then GoTo 570

				}
				else 
				{
					V0 = S * Math.Sin(D0) + C * Math.Cos(D0) * Math.Cos(H0) - Z;
				}
				V2 = S * Math.Sin(D2) + C * Math.Cos(D2) * Math.Cos(H2) - Z;

				if(Sgn(V0) != Sgn(V2)) 
				{ 
					double V1 = S * Math.Sin(D1) + C * Math.Cos(D1) * Math.Cos(H1) - Z;
					A = 2 * V2 - 4 * V1 + 2 * V0;
					double B = 4 * V1 - 3 * V0 - V2;
					D = B * B - 4 * A * V0;
					if(D >= 0) 
					{
						D = Math.Sqrt(D);
						//630 If V0 < 0 And V2 > 0 Then Print R$;
						if(V0 < 0 && V2 > 0) M8 = 1;
						//650 If V0 > 0 And V2 < 0 Then Print S$;
						if(V0 > 0 && V2 < 0) W8 = 1;
				
						double E;
						if((2*A) == 0)
							E = (-B + D);
						else
							E = (-B + D) / (2 * A);

						if(E > 1 || E < 0) 
						{
							if((2*A) == 0)
								E = (-B - D);
							else
								E = (-B - D) / (2 * A);
						}

						double T3 = C0 + E + 1 / 120;//: ' Round off
						double H3 = (int)(T3);
						double M3 = (int)((T3 - H3) * 60);
						double S3 = (int)((((T3 - H3) * 60) - M3)*60);
				
						//710 Print USING; "##:##"; H3; M3;
				
						if(V0 < 0 && V2 > 0) 
						{
							tRise.wHour = (ushort)H3;
							tRise.wMinute = (ushort)M3;
							tRise.wSecond = (ushort)S3;
						}
						if(V0 > 0 && V2 < 0) 
						{
							tSet.wHour = (ushort)H3;
							tSet.wMinute = (ushort)M3;
							tSet.wSecond = (ushort)S3;
						}

						double H7 = H0 + E * (H2 - H0);
						double N7 = -Math.Cos(D1) * Math.Sin(H7);
						double D7 = C * Math.Sin(D1) - S * Math.Cos(D1) * Math.Cos(H7);
						double AZ;
						if(D7 == 0 || DR == 0)
							AZ = Math.Atan(N7);
						else
							AZ = Math.Atan(N7 / D7) / DR;
						if(D7 < 0)		AZ = AZ + 180;
						if(AZ < 0)		AZ = AZ + 360;
						if(AZ > 360)	AZ = AZ - 360;

						//790 Print USING; ",  azimuth ###.#"; AZ
						//800 Return
					}
				}

				A0 = A2;
				D0 = D2;
				V0 = V2;
			}

			if(M8 == 0 && W8 == 0) 
			{
				if(V2 < 0)	
				{
					//Sun down all day;	
				}
				else 
				{	// V2 > 0
					//Sun up all day"
				}
			}
			else if(M8 == 0) 
			{
				//CString M1$ = "No sunrise this date";
			}
			else if(W8 == 0) 
			{
				//CString M2$ = "No sunset this date";
			}
	
			return 1;
		}

		static void Function910()
		{
			double L, V, U, W;
	
			//910 '   Fundamental arguments
			//920 '     (Van Flandern &
			//930 '     Pulkkinen, 1979)
			L = 0.779072 + 0.00273790931 * T;
			G = 0.993126 + 0.0027377785 * T;
			L = L - (int)L;
			G = G - (int)G;
			L = L * P2;
			G = G * P2;
			V = 0.39785 * Math.Sin(L);
			V = V - 0.01 * Math.Sin(L - G);
			V = V + 0.00333 * Math.Sin(L + G);
			V = V - 0.00021 * TT * Math.Sin(L);
			U = 1 - 0.03349 * Math.Cos(G);
			U = U - 0.00014 * Math.Cos(2 * L);
			U = U + 0.00008 * Math.Cos(L);
			W = -0.0001 - 0.04129 * Math.Sin(2 * L);
			W = W + 0.03211 * Math.Sin(G);
			W = W + 0.00104 * Math.Sin(2 * L - G);
			W = W - 0.00035 * Math.Sin(2 * L + G);
			W = W - 0.00008 * TT * Math.Sin(G);
			//1100 '
			//1110 '    Compute Sun's RA and Dec
			if(Math.Sqrt(U-V*V) == 0) 
				S = W;
			else 
				S = W / Math.Sqrt(U - V * V);

			if(Math.Sqrt(1-S*S) == 0)
				A5 = L + Math.Atan(S);
			else 
				A5 = L + Math.Atan(S / Math.Sqrt(1 - S * S));

			if(Math.Sqrt(U) == 0) 
				S = V;
			else
				S = V / Math.Sqrt(U);

			if(Math.Sqrt(1-S*S) == 0)
				D5 = Math.Atan(S);
			else
				D5 = Math.Atan(S / Math.Sqrt(1 - S * S));
			R5 = 1.00021 * Math.Sqrt(U);
		}

		static int Sgn(double value)
		{
			if(value > 0)	return 1;
			if(value < 0)	return -1;
			return 0;
		}
	}
}

/*
static double P1 = 3.14159265, P2 = 2 * P1;
static double DR = P1 / 180;
static double K1 = 15 * DR * 1.0027379;

static double G;
static double T, TT;

static double S, A5, D5, R5;

static void Function910()
{
	double L, V, U, W;
	
//910 '   Fundamental arguments
//920 '     (Van Flandern &
//930 '     Pulkkinen, 1979)
	L = 0.779072 + 0.00273790931 * T;
	G = 0.993126 + 0.0027377785 * T;
	L = L - (int)L;
	G = G - (int)G;
	L = L * P2;
	G = G * P2;
	V = 0.39785 * sin(L);
	V = V - 0.01 * sin(L - G);
	V = V + 0.00333 * sin(L + G);
	V = V - 0.00021 * TT * sin(L);
	U = 1 - 0.03349 * cos(G);
	U = U - 0.00014 * cos(2 * L);
	U = U + 0.00008 * cos(L);
	W = -0.0001 - 0.04129 * sin(2 * L);
	W = W + 0.03211 * sin(G);
	W = W + 0.00104 * sin(2 * L - G);
	W = W - 0.00035 * sin(2 * L + G);
	W = W - 0.00008 * TT * sin(G);
//1100 '
//1110 '    Compute Sun's RA and Dec
	if(sqrt(U-V*V) == 0) 
		S = W;
	else 
		S = W / sqrt(U - V * V);

	if(sqrt(1-S*S) == 0)
		A5 = L + atan(S);
	else 
		A5 = L + atan(S / sqrt(1 - S * S));

	if(sqrt(U) == 0) 
		S = V;
	else
		S = V / sqrt(U);

	if(sqrt(1-S*S) == 0)
		D5 = atan(S);
	else
		D5 = atan(S / sqrt(1 - S * S));
	R5 = 1.00021 * sqrt(U);
}

static int Sgn(double value)
{
	if(value > 0)	return 1;
	if(value < 0)	return -1;
	return 0;
}

int GetSunRiseSunSetTime(int org_year, int org_mon, int day, double longitude, double latitude, int gh, struct time *tRise, struct time *tSet)
{
	tRise->ti_hour = 0;
	tRise->ti_min = 0;
	tRise->ti_sec = 0;
	tSet->ti_hour = 0;
	tSet->ti_min = 0;
	tSet->ti_sec = 0;

	double aA[3], aD[3];
	
	CString S$ = "Sunset at  ";
	CString R$ = "Sunrise at ";
	CString M1$ = "No sunrise this date";
	CString M2$ = "No sunset this date";
	CString M3$ = "Sun down all day";
	CString M4$ = "Sun up all day";

	double B5 = latitude;
	double L5 = longitude;
	double H = gh;

	L5 = L5 / 360;
	double Z0 = H / 24;

	//60 GoSub 1170:
	//1170 '     Calendar --> JD
	double Y = org_year;
	double M = org_mon;
	double D = day;

	G = 1;
	if(Y < 1583) G = 0;
	double D1 = (int)D;
	double F = D - D1 - 0.5;
	double J = -(int)(7 * ((int)((M + 9) / 12) + Y) / 4);
	double A;
	double J3;

	if(G == 0) { //Then GoTo 1260
	}
	else {
		S = Sgn(M - 9);
		A = fabs(M - 9);
		J3 = (int)(Y + S * (int)(A / 7));
		J3 = -(int)(((int)(J3 / 100) + 1) * 3 / 4);
	}
	J = J + (int)(275 * M / 9) + D1 + G * J3;
	J = J + 1721027 + 2 * G + 367 * Y;
	if(F >= 0) {// Then GoTo 1300
	}
	else {
		F = F + 1;
		J = J - 1;
	}

	T = (J - 2451545) + F;
	TT = T / 36525 + 1;//: ' TT = centuries
	//80 '               from 1900.0

	//90 GoSub 410:
	//410 '     LST at 0h zone time
	double T0 = T / 36525;
	S = 24110.5 + 8640184.813 * T0;
	S = S + 86636.6 * Z0 + 86400 * L5;
	S = S / 86400;
	S = S - (int)(S);
	T0 = S * 360 * DR;
	
	T = T + Z0;
//100 '
//110 '       Get Sun's Position

	//120 GoSub 910:
	Function910();
	aA[1] = A5;
	aD[1] = D5;
	T = T + 1;

	//140 GoSub 910:
	Function910();
	aA[2] = A5;
	aD[2] = D5;

	if(aA[2] < aA[1]) aA[2] = aA[2] + P2;
	double Z1 = DR * 90.833;//: ' Zenith dist.
	S = sin(B5 * DR);
	double C = cos(B5 * DR);
	double Z = cos(Z1);

	double M8 = 0;
	double W8 = 0;
	//Print
	double A0 = aA[1];
	double D0 = aD[1];
	double DA = aA[2] - aA[1];
	double DD = aD[2] - aD[1];

	double V0;
	double V2;

	for(double C0 = 0; C0 <= 23; C0++) {	// ?????? <= 23 or < 23
		double P = (C0 + 1) / 24;
		double A2 = aA[1] + P * DA;
		double D2 = aD[1] + P * DD;
		
		//GoSub 490
		//490 '  Test an hour for an event
		double L0 = T0 + C0 * K1;
		double L2 = L0 + K1;
		double H0 = L0 - A0;
		double H2 = L2 - A2;
		double H1 = (H2 + H0) / 2;//: '  Hour angle,
		D1 = (D2 + D0) / 2;//: '  declination,
		//540 '                at half hour
		
		if(C0 > 0) { //Then GoTo 570
		}
		else {
			V0 = S * sin(D0) + C * cos(D0) * cos(H0) - Z;
		}
		V2 = S * sin(D2) + C * cos(D2) * cos(H2) - Z;

		if(Sgn(V0) != Sgn(V2)) { 
			double V1 = S * sin(D1) + C * cos(D1) * cos(H1) - Z;
			A = 2 * V2 - 4 * V1 + 2 * V0;
			double B = 4 * V1 - 3 * V0 - V2;
			D = B * B - 4 * A * V0;
			if(D >= 0) {
				D = sqrt(D);
				//630 If V0 < 0 And V2 > 0 Then Print R$;
				if(V0 < 0 && V2 > 0) M8 = 1;
				//650 If V0 > 0 And V2 < 0 Then Print S$;
				if(V0 > 0 && V2 < 0) W8 = 1;
				
				double E;
				if((2*A) == 0)
					E = (-B + D);
				else
					E = (-B + D) / (2 * A);

				if(E > 1 || E < 0) {
					if((2*A) == 0)
						E = (-B - D);
					else
						E = (-B - D) / (2 * A);
				}

				double T3 = C0 + E + 1 / 120;//: ' Round off
				double H3 = (int)(T3);
				double M3 = (int)((T3 - H3) * 60);
				double S3 = (int)((((T3 - H3) * 60) - M3)*60);
				
				//710 Print USING; "##:##"; H3; M3;
				
				if(V0 < 0 && V2 > 0) {
					tRise->ti_hour = (int)H3;
					tRise->ti_min = (int)M3;
					tRise->ti_sec = (int)S3;
				}
				if(V0 > 0 && V2 < 0) {
					tSet->ti_hour = (int)H3;
					tSet->ti_min = (int)M3;
					tSet->ti_sec = (int)S3;
				}

				double H7 = H0 + E * (H2 - H0);
				double N7 = -cos(D1) * sin(H7);
				double D7 = C * sin(D1) - S * cos(D1) * cos(H7);
				double AZ;
				if(D7 == 0 || DR == 0)
					AZ = atan(N7);
				else
					AZ = atan(N7 / D7) / DR;
				if(D7 < 0)		AZ = AZ + 180;
				if(AZ < 0)		AZ = AZ + 360;
				if(AZ > 360)	AZ = AZ - 360;

				//790 Print USING; ",  azimuth ###.#"; AZ
				//800 Return
			}
		}

		A0 = A2;
		D0 = D2;
		V0 = V2;
	}

	if(M8 == 0 && W8 == 0) {
		if(V2 < 0)	{
			//Sun down all day;	
		}
		else {	// V2 > 0
			//Sun up all day"
		}
	}
	else if(M8 == 0) {
		//CString M1$ = "No sunrise this date";
	}
	else if(W8 == 0) {
		//CString M2$ = "No sunset this date";
	}
	
	return 1;
}

*/
