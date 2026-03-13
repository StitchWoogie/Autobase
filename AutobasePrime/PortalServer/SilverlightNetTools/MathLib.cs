using System;

namespace NetTools
{
	/// <summary>
	/// Summary description for Math.
	/// </summary>
	public class MathLib
	{
		public MathLib()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//-----------------------------------------------------------------------------
		//	각을 라디안으로 바꾼다.
		//------------------------------------------------------------------------------

		static public double MathDegreeToRadian(float degree)
		{
            degree %= 360;
			return PI/180.0 * (double)degree;
		}

		//-----------------------------------------------------------------------------
		//	라디안을 각으로 바꾼다.
		//------------------------------------------------------------------------------

		static public float MathRadianToDegree(double radian)
		{
			return (float) (radian/PI * 180.0);
		}

		//------------------------------------------------------------------------------
		// 기울기를 라디안으로 변환한다.
		// 수학 좌표로 계산.
		//------------------------------------------------------------------------------

		static  public double MathGradientToRadian(int x, int y)
		{
			return MathDegreeToRadian(MathGradientToDegree(x, y));
		}

		//------------------------------------------------------------------------------
		// 기울기를 각도로 변환한다.
		//	수학 좌표로 계산.
		//------------------------------------------------------------------------------

		static public float MathGradientToDegree(int x, int y)
		{
			y *= -1;

			if(x == 0) 
			{
				if(y > 0)	return (float)90;
				else			return (float)270;
			}
			else if(y == 0) 
			{
				if(x > 0)	return (float)0;
				else			return (float)180;
			}
			else 
			{
				float angle = MathRadianToDegree(Math.Atan((double)Math.Abs(y)/(double)Math.Abs(x)));
				angle %= 90.0f;
				if(x > 0 && y > 0)		return angle;
				else if(x > 0 && y < 0) return 360-angle;
				else if(x < 0 && y > 0)	return 180-angle;
				else							return angle+180;
			}
		}

		//------------------------------------------------------------------------------
		//	주어진 타원에서 각도에 존재하는 컴퓨터 좌표를 구한다.
		//------------------------------------------------------------------------------

		static double PI = 3.141592;

        static public void MathGetEllipsePoint(double cx, double cy, double rx, double ry, double angle, out double pointx, out double pointy)
		{
			double x, y;
			double tansqrt;
			double radian;
			double rxsqrt, rysqrt;

			if(rx == 0 && ry == 0) 
			{	// 반지름이 0,0 일때는 점이 항상 중앙이다. 
				pointx = cx;
				pointy = cy;
				return;
			}

			rx = Math.Abs(rx);
			ry = Math.Abs(ry);

            angle = angle % 360;
            if (angle < 0) angle += 360;

			//-----------------------------------
			// 직각일 때는 빠르게 계산될 수 있다.
			//-----------------------------------
			if(angle == 0) 
			{
				pointx = (cx+rx);
				pointy = cy;
				return;
			}
			else if(angle == 90) 
			{
				pointx = cx;
				pointy = (cy-ry);
				return;
			}
			else if(angle == 180) 
			{
				pointx = (cx-rx);
				pointy = cy;
				return;
			}
			else if(angle == 270) 
			{
				pointx = cx;
				pointy = (cy+ry);
				return;
			}

			radian = PI/180.0 * (double)angle;

			tansqrt = Math.Tan(radian)*Math.Tan(radian);

			rxsqrt = (double) rx * (double) rx;
			rysqrt = (double) ry * (double) ry;

			if((angle >= 0 && angle <= 90) || (angle > 270 && angle <= 360)) 
			{
				x =  Math.Sqrt(1.0 / (rysqrt + rxsqrt * tansqrt)) * (double)rx * (double)ry;
			}
			else 
			{
				x = -Math.Sqrt(1.0 / (rysqrt + rxsqrt * tansqrt)) * (double)rx * (double)ry;
			}

			y = Math.Tan(radian) * x;

			pointx = cx+x;
			pointy = cy-y;
		}

		/*
		//------------------------------------------------------------------------------
		//	주어진 직각 삼각형의 빗변의 크기를 구한다.
		//------------------------------------------------------------------------------

		float MathGetHypotenuse(int x, int y)
		{
			x = abs(x);
			y = -1*abs(y);
			float angle = MathGradientToDegree(x, y);

			float value = (float)(y/sin(MathDegreeToRadian(angle)));
			if(value < 0)	value *= -1;
			return value;
		}
		*/


	}
}
