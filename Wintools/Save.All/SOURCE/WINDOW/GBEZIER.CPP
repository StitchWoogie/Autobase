#if	defined (__BORLANDC__)
#include <mem.h>
#else
#include <memory.h>
#endif

#include <tools.h>
#include <glib.h>

static double factorial(int q)
{
	int f = 1, c;
	for(c = q; c > 1; c--)
		f *= c;
	return(f);
}

double c(int n, int i)
{
	return(factorial(n) / (factorial(i) * factorial(n-i)));
}

static double blend(int i, int n, double u, double binomial)
{
	double partial;
	int j;

	partial = binomial;
	for(j = 1; j <= i; j++)
		partial = partial * u;
	for(j = 1; j <= (n-i); j++)
		partial = partial * (1.0 - u);
	return(partial);
}

void bezierFcn (double *x, double *y, double u, double coeff[], int n, int *p)
{
	int i;
	double b;

	*x = *y = 0;
	for(i = 0; i <= n; i++) {
		b = blend (i, n, u, coeff[i]);
		*x = *x + (b * p[i*2]);
		*y = *y + (b * p[i*2+1]);
	}
}

void gbezier(HDC hdc, void *p, int npts, int segments)
{
	int i;
	double u, x, y;
	double coeff[20];
	int *pp = (int*) p;

	for( i = 0; i < npts; i++)
		coeff[i] = c(npts - 1, i);
	for(i = 0; i <= segments; i++) {
		u = (double) i / segments;
		bezierFcn(&x, &y, u, coeff, npts - 1, pp);
		x+=0.5;
		y+=0.5;
		if( i == 0)
			MoveTo(hdc, (int)x, (int)y);
		else {
			//if(_WRITE_FORM == XOR_PUT) {
			//	gputpixel(_LINE_POINTX, _LINE_POINTY, ~ggetpixel(_LINE_POINTX, _LINE_POINTY));
			//}
			LineTo(hdc, (int)x, (int)y);
		}
	}
}






