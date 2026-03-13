// 수학 계산
double MathDegreeToRadian(float degree);	//	각을 라디안으로 바꾼다.
float  MathRadianToDegree(double radian);	//	라디안을 각으로 바꾼다.
float  MathGradientToDegree(int x, int y);	// 기울기를 각도로 변환한다.
double MathGradientToRadian(int x, int y);	// 기울기를 각도로 변환한다.
void   MathGetEllipsePoint(int cx, int cy, float rx, float ry, float angle, int *px, int *py);	//	주어진 타원에서 각도에 존재하는 컴퓨터 좌표를 구한다.
float  MathGetHypotenuse(int x, int y);		//	주어진 직각 삼각형의 빗변의 크기를 구한다.
