#include "stdafx.h"

#include <tools.h>
#include <math.h>

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

/*
Original Source

10 '         Sunrise-Sunset
20 GoSub 300
30 INPUT "Lat, Long (deg)";B5,L5
40 INPUT "Time zone (hrs)";H
50 L5 = L5 / 360: Z0 = H / 24
60 GoSub 1170: T = (J - 2451545) + F
70 TT = T / 36525 + 1: ' TT = centuries
80 '               from 1900.0
90 GoSub 410: T = T + Z0
100 '
110 '       Get Sun's Position
120 GoSub 910: A(1) = A5: D(1) = D5
130 T = T + 1
140 GoSub 910: A(2) = A5: D(2) = D5
150 If A(2) < A(1) Then A(2) = A(2) + P2
160 Z1 = DR * 90.833: ' Zenith dist.
170 S = Sin(B5 * DR): C = Cos(B5 * DR)
180 Z = Cos(Z1): M8 = 0: W8 = 0: Print
190 A0 = A(1): D0 = D(1)
200 DA = A(2) - A(1): DD = D(2) - D(1)
210 For C0 = 0 To 23
220 P = (C0 + 1) / 24
230 A2 = A(1) + P * DA: D2 = D(1) + P * DD
240 GoSub 490
250 A0 = A2: D0 = D2: V0 = V2
260 Next
270 GoSub 820: '  Special msg?
280 End
290 '
300 '        Constants
310 Dim A(2), D(2)
320 P1 = 3.14159265: P2 = 2 * P1
330 DR = P1 / 180: K1 = 15 * DR * 1.0027379
340 S$ = "Sunset at  "
350 R$ = "Sunrise at "
360 M1$ = "No sunrise this date"
370 M2$ = "No sunset this date"
380 M3$ = "Sun down all day"
390 M4$ = "Sun up all day"
400 Return
410 '     LST at 0h zone time
420 T0 = T / 36525
430 S = 24110.5 + 8640184.813 * T0
440 S = S + 86636.6 * Z0 + 86400 * L5
450 S = S / 86400: S = S - Int(S)
460 T0 = S * 360 * DR
470 Return
480 '
490 '  Test an hour for an event
500 L0 = T0 + C0 * K1: L2 = L0 + K1
510 H0 = L0 - A0: H2 = L2 - A2
520 H1 = (H2 + H0) / 2: '  Hour angle,
530 D1 = (D2 + D0) / 2: '  declination,
540 '                at half hour
550 If C0 > 0 Then GoTo 570
560 V0 = S * Sin(D0) + C * Cos(D0) * Cos(H0) - Z
570 V2 = S * Sin(D2) + C * Cos(D2) * Cos(H2) - Z
580 If Sgn(V0) = Sgn(V2) Then GoTo 800
590 V1 = S * Sin(D1) + C * Cos(D1) * Cos(H1) - Z
600 A = 2 * V2 - 4 * V1 + 2 * V0: B = 4 * V1 - 3 * V0 - V2
610 D = B * B - 4 * A * V0: If D < 0 Then GoTo 800
620 D = Sqr(D)
630 If V0 < 0 And V2 > 0 Then Print R$;
640 If V0 < 0 And V2 > 0 Then M8 = 1
650 If V0 > 0 And V2 < 0 Then Print S$;
660 If V0 > 0 And V2 < 0 Then W8 = 1
670 E = (-B + D) / (2 * A)
680 If E > 1 Or E < 0 Then E = (-B - D) / (2 * A)
690 T3 = C0 + E + 1 / 120: ' Round off
700 H3 = Int(T3): M3 = Int((T3 - H3) * 60)
710 Print USING; "##:##"; H3; M3;
720 H7 = H0 + E * (H2 - H0)
730 N7 = -Cos(D1) * Sin(H7)
740 D7 = C * Sin(D1) - S * Cos(D1) * Cos(H7)
750 AZ = Atn(N7 / D7) / DR
760 If D7 < 0 Then AZ = AZ + 180
770 If AZ < 0 Then AZ = AZ + 360
780 If AZ > 360 Then AZ = AZ - 360
790 Print USING; ",  azimuth ###.#"; AZ
800 Return
810 '
820 '   Special-message routine
830 If M8 = 0 And W8 = 0 Then GoTo 870
840 If M8 = 0 Then Print M1$
850 If W8 = 0 Then Print M2$
860 GoTo 890
870 If V2 < 0 Then Print M3$
880 If V2 > 0 Then Print M4$
890 Return
900 '
910 '   Fundamental arguments
920 '     (Van Flandern &
930 '     Pulkkinen, 1979)
940 L = 0.779072 + 0.00273790931 * T
950 G = 0.993126 + 0.0027377785 * T
960 L = L - Int(L): G = G - Int(G)
970 L = L * P2: G = G * P2
980 V = 0.39785 * Sin(L)
990 V = V - 0.01 * Sin(L - G)
1000 V = V + 0.00333 * Sin(L + G)
1010 V = V - 0.00021 * TT * Sin(L)
1020 U = 1 - 0.03349 * Cos(G)
1030 U = U - 0.00014 * Cos(2 * L)
1040 U = U + 0.00008 * Cos(L)
1050 W = -0.0001 - 0.04129 * Sin(2 * L)
1060 W = W + 0.03211 * Sin(G)
1070 W = W + 0.00104 * Sin(2 * L - G)
1080 W = W - 0.00035 * Sin(2 * L + G)
1090 W = W - 0.00008 * TT * Sin(G)
1100 '
1110 '    Compute Sun's RA and Dec
1120 S = W / Sqr(U - V * V)
1130 A5 = L + Atn(S / Sqr(1 - S * S))
1140 S = V / Sqr(U): D5 = Atn(S / Sqr(1 - S * S))
1150 R5 = 1.00021 * Sqr(U)
1160 Return
1165 '
1170 '     Calendar --> JD
1180 INPUT "Year, Month, Day";Y,M,D
1190 G = 1: If Y < 1583 Then G = 0
1200 D1 = Int(D): F = D - D1 - 0.5
1210 J = -Int(7 * (Int((M + 9) / 12) + Y) / 4)
1220 If G = 0 Then GoTo 1260
1230 S = Sgn(M - 9): A = Abs(M - 9)
1240 J3 = Int(Y + S * Int(A / 7))
1250 J3 = -Int((Int(J3 / 100) + 1) * 3 / 4)
1260 J = J + Int(275 * M / 9) + D1 + G * J3
1270 J = J + 1721027 + 2 * G + 367 * Y
1280 If F >= 0 Then GoTo 1300
1290 F = F + 1: J = J - 1
1300 Return
1310 '
1320 '   This program by Roger W. Sinnott calculates the times of sunrise
1330 '   and sunset on any date, accurate to the minute within several
1340 '   centuries of the present.  It correctly describes what happens in the
1350 '   arctic and antarctic regions, where the Sun may not rise or set on
1360 '   a given date.  Enter north latitudes positive, west longitudes
1370 '   negative.  For the time zone, enter the number of hours west of
1380 '   Greenwich (e.g., 5 for EST, 4 for EDT).  The calculation is
1390 '   discussed in Sky & Telescope for August 1994, page 84.
*/



/*
double tableMeanEclipticLonggitute[31][12] = {
	{ 339.226, 009.781, 039.351, 069.906, 099.475, 130.030, 160.585, 190.155, 220.710, 250.279, 280.834, 311.390 },
	{ 340.212, 010.767, 040.337, 070.892, 100.461, 131.016, 161.571, 191.141, 221.696, 251.265, 281.820, 312.375}, 
	{ 341.198, 011.753, 041.322, 071.877, 101.447, 132.002, 162.557, 192.126, 222.681, 252.251, 282.806, 313.361}, 
	{ 342.183, 012.738, 042.308, 072.863, 102.432, 132.987, 163.542, 193.112, 223.667, 253.236, 283.791, 314.346}, 
	{ 343.169, 013.724, 043.293, 073.849, 103.418, 133.973, 164.528, 194.098, 224.653, 254.222, 284.777, 315.332}, 
	{ 344.155, 014.710, 044.279, 074.834, 104.404, 134.959, 165.514, 195.083, 225.638, 255.208, 285.763, 316.318}, 
	{ 345.140, 015.695, 045.265, 075.820, 105.389, 135.944, 166.499, 196.069, 226.624, 256.193, 286.748, 317.303}, 
	{ 346.126, 016.681, 046.250, 076.805, 106.375, 136.930, 167.485, 197.054, 227.610, 257.179, 287.734, 318.289}, 
	{ 347.112, 017.667, 047.236, 077.791, 107.361, 137.916, 168.471, 198.040, 228.595, 258.165, 288.720, 319.275}, 
	{ 348.097, 018.652, 048.222, 078.777, 108.346, 138.901, 169.456, 199.026, 229.581, 259.150, 289.705, 320.260}, 
	{ 349.083, 019.638, 049.207, 079.762, 109.332, 139.887, 170.442, 200.011, 230.566, 260.136, 290.691, 321.246}, 
	{ 350.068, 020.624, 050.193, 080.748, 110.317, 140.873, 171.428, 200.997, 231.552, 261.122, 291.677, 322.232}, 
	{ 351.054, 021.609, 051.179, 081.734, 111.303, 141.858, 172.413, 201.983, 232.538, 262.107, 292.662, 323.217}, 
	{ 352.040, 022.595, 052.164, 082.719, 112.289, 142.844, 173.399, 202.968, 233.523, 263.093, 293.648, 324.203}, 
	{ 353.025, 023.581, 053.150, 083.705, 113.274, 143.829, 174.385, 203.954, 234.509, 264.078, 294.634, 325.189}, 
	{ 354.011, 024.566, 054.136, 084.691, 114.260, 144.815, 175.370, 204.940, 235.495, 265.064, 295.619, 326.174}, 
	{ 354.997, 025.552, 055.121, 085.676, 115.246, 145.801, 176.356, 205.925, 236.480, 266.050, 296.605, 327.160}, 
	{ 355.982, 026.537, 056.107, 086.662, 116.231, 146.786, 177.341, 206.911, 237.466, 267.035, 297.590, 328.146}, 
	{ 356.968, 027.523, 057.093, 087.648, 117.217, 147.772, 178.327, 207.897, 238.452, 268.021, 298.576, 329.131}, 
	{ 357.954, 028.509, 058.078, 088.633, 118.203, 148.758, 179.313, 208.882, 239.437, 269.007, 299.562, 330.117}, 
	{ 358.939, 029.494, 059.064, 089.619, 119.188, 149.743, 180.298, 209.868, 240.423, 269.992, 300.547, 331.102}, 
	{ 359.925, 030.480, 060.049, 090.605, 120.174, 150.729, 181.284, 210.854, 241.409, 270.978, 301.533, 332.088}, 
	{ 000.911, 031.466, 061.035, 091.590, 121.160, 151.715, 182.270, 211.839, 242.394, 271.964, 302.519, 333.074}, 
	{ 001.896, 032.451, 062.021, 092.576, 122.145, 152.700, 183.255, 212.825, 243.380, 272.949, 303.504, 334.059}, 
	{ 002.882, 033.437, 063.006, 093.561, 123.131, 153.686, 184.241, 213.810, 244.366, 273.935, 304.490, 335.045}, 
	{ 003.868, 034.423, 063.992, 094.547, 124.117, 154.672, 185.227, 214.796, 245.351, 274.921, 305.476, 336.031}, 
	{ 004.853, 035.408, 064.978, 095.533, 125.102, 155.657, 186.212, 215.782, 246.337, 275.906, 306.461, 337.016}, 
	{ 005.839, 036.394, 065.963, 096.518, 126.088, 156.643, 187.198, 216.767, 247.322, 276.892, 307.447, 338.002}, 
	{ 006.824, 037.380, 066.949, 097.504, 127.073, 157.629, 188.184, 217.753, 248.308, 277.878, 308.433, 338.988}, 
	{ 007.810, 038.365, 067.935, 098.490, 128.059, 158.614, 189.169, 218.739, 249.294, 278.863, 309.418, 0.0    },
	{ 008.796,   0.0  , 068.920,   0.0,   129.045, 159.600,   0.0,   219.724, 0.0,     279.849, 310.404, 0.0 }};   

double	tableYearHighX[4] = { +0.322, +0.107, -0.107, -0.322 };
double	tableYearHighY[10] ={ -0.577, -0.449, -0.320, -0.192, -0.064, +0.064, +0.192, +0.320, +0.449, +0.577 };

double	tableYearLowX[4] = { +0.358, +0.119, -0.119, -0.358 };
double	tableYearLowY[25] ={ -0.370, -0.339, -0.309, -0.278, -0.247, -0.216, -0.185, -0.154, -0.123, -0.093, -0.062, -0.031, 0,
                             +0.031, +0.062, +0.093, +0.123, +0.154, +0.185, +0.216, +0.247, +0.278, +0.309, +0.339, +0.370 };

double	tableYearHighX2[4] = { -2.58, -0.86, +0.86, +2.58 };
double	tableYearHighY2[10] ={ 251.97, 258.85, 265.73, 272.61, 279.49, 286.37, 293.25, 300.14, 307.02, 313.90 };

double	tableYearLowX2[4] = { -0.03, -0.01, +0.01, +0.03 };
double	tableYearLowY2[25] ={ -0.83, -0.76, -0.69, -0.62, -0.55, -0.48, -0.41, -0.34, -0.28, -0.21, -0.14, -0.07, 0,
							  +0.07, +0.14, +0.21, +0.28, +0.34, +0.41, +0.48, +0.55, +0.62, +0.69, +0.76, +0.83};

int GetSunRiseSunSet(int org_year, int org_mon, int day, double longitude, double latitude, int &rise_hour, int &rise_min, int &rise_sec, int &set_hour, int &set_min, int &set_sec)
{
	if(org_year < 1 || org_year > 3999)	return 0;	// 1년에서 3999년까지만 지원한다.
	
	int year = org_year;
	int mon = org_mon;
	int year_pos;
	int mon_pos;
	int day_pos;
	int big_year;
	int small_year;

	MinusMonth(year, mon);
	MinusMonth(year, mon);

	year_pos = year-1;
	mon_pos = mon-1;
	day_pos =day-1;

	mon_pos %= 12;
	day_pos %= 31;

	double MeanEclipticLongitudeOfTheSun = tableMeanEclipticLonggitute[day_pos][mon_pos];
	big_year = year/100;
	small_year = year%100;
	MeanEclipticLongitudeOfTheSun += tableYearHighX[big_year%4] + tableYearHighY[big_year/4];
	MeanEclipticLongitudeOfTheSun += tableYearLowX[small_year%4] + tableYearLowY[small_year/4];

	big_year = org_year/100;
	small_year = org_year%100;

	double EclipticLongitudeOfPerigeeOfTheSun = tableYearHighX2[big_year%4] + tableYearHighY2[big_year/4];
	EclipticLongitudeOfPerigeeOfTheSun += tableYearLowX2[small_year%4] + tableYearLowY2[small_year/4];

	double MeanAnomalyOfTheSun = MeanEclipticLongitudeOfTheSun-EclipticLongitudeOfPerigeeOfTheSun;

	double EclipticLongitudeOfTheSun = MeanEclipticLongitudeOfTheSun+1.915*sin(MeanAnomalyOfTheSun*PI/180)+0.020*sin(MeanAnomalyOfTheSun*2*PI/180);
	
	double DeclinationOfTheSun = asin(sin(EclipticLongitudeOfTheSun*PI/180)*0.39777)*180/PI;
	
	// 292.797297526...°-atan(sin(2×292.797297526...°)÷(23.2377+cos(2×292.797297526...°))) = 294.612846478...° 
	double RightAscensionOfTheSun;
	if((23.2377+cos(2*EclipticLongitudeOfTheSun*PI/180)) == 0)
		RightAscensionOfTheSun = sin(2*EclipticLongitudeOfTheSun*PI/180);
	else
		RightAscensionOfTheSun = sin(2*EclipticLongitudeOfTheSun*PI/180)/(23.2377+cos(2*EclipticLongitudeOfTheSun*PI/180));
	RightAscensionOfTheSun = EclipticLongitudeOfTheSun - (atan(RightAscensionOfTheSun)*180/PI);
	
	//equation of time = 294.612846478...°-292.411° = +2.201846478...°
	double EquationOfTime = RightAscensionOfTheSun-MeanEclipticLongitudeOfTheSun;

	//time of sunset = 347.000...°+asin(tan(+38.833333...°)×tan(-21.5120425524...°)+0.01454÷cos(+38.833333...°)÷cos(-21.5120425524...°))+(+2.201846478...°) = 331.91102521...° = 22h07m38s UT = 16h59m38s in Washington = 17h07m38s in the Eastern Standard Time Zone (which I don't think existed in the year 1888)
	double RedSpot;
	if(cos(latitude*PI/180) == 0 || cos(DeclinationOfTheSun*PI/180) == 0) 
		RedSpot = 0.01454;
	else
		RedSpot = 0.01454/cos(latitude*PI/180)/cos(DeclinationOfTheSun*PI/180);
	//RedSpot = 0;

	//mean time of sunset = 270°-(-77.000...°) = 347.000...°
	double MeanTimeOfSunset = 270 - longitude;

	double TimeOfSunset = tan(latitude*PI/180)*tan(DeclinationOfTheSun*PI/180)+RedSpot;
	TimeOfSunset = MeanTimeOfSunset-asin(TimeOfSunset)*180/PI+EquationOfTime;

	//mean time of sunrise = 90°-(-77.000...°) = 347.000...°
	double MeanTimeOfSunrise = 90 - longitude;

	double TimeOfSunrise = tan(latitude*PI/180)*tan(DeclinationOfTheSun*PI/180)+RedSpot;
	TimeOfSunrise = MeanTimeOfSunrise-asin(TimeOfSunrise)*180/PI+EquationOfTime;

	//TimeOfSunset = 331.91102521;
	double totaltime = (TimeOfSunset/360)*24;
	set_hour = (int)totaltime;	// 22
	totaltime = (totaltime-set_hour)*60;
	set_min = (int)totaltime;	// 7
	totaltime = (totaltime-set_min)*60;
	set_sec = (int)totaltime;	// 38

	totaltime = (TimeOfSunrise/360)*24;
	rise_hour = (int)totaltime;	// 22
	totaltime = (totaltime-rise_hour)*60;
	rise_min = (int)totaltime;	// 7
	totaltime = (totaltime-rise_min)*60;
	rise_sec = (int)totaltime;	// 38
		
	return 1;
}
*/