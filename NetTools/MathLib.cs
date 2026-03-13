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

		static public double MathDegreeToRadian(double degree)
		{
            degree %= 360;
			return PI/180.0 * (double)degree;
		}

		//-----------------------------------------------------------------------------
		//	라디안을 각으로 바꾼다.
		//------------------------------------------------------------------------------

		static public double MathRadianToDegree(double radian)
		{
            return (double)(radian / PI * 180.0);
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
		// 기울기를 각도로 변환한다. 인자는 컴퓨터 좌표이므로 y를 바꿔주어야 정확하게 계산된다.
		// 수학 좌표로 계산.
		//------------------------------------------------------------------------------

        static public double MathGradientToDegree(int x, int y)
		{
			y *= -1;

			if(x == 0) 
			{
				if(y > 0)	return 90;
				else			return 270;
			}
			else if(y == 0) 
			{
				if(x > 0)	return 0;
				else			return 180;
			}
			else 
			{
				double angle = MathRadianToDegree(Math.Atan((double)Math.Abs(y)/(double)Math.Abs(x)));
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

		static public void MathGetEllipsePoint(int cx, int cy, float rx, float ry, float angle, out int pointx, out int pointy)
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
				pointx = (int)(cx+rx);
				pointy = cy;
				return;
			}
			else if(angle == 90) 
			{
				pointx = cx;
				pointy = (int)(cy-ry);
				return;
			}
			else if(angle == 180) 
			{
				pointx = (int)(cx-rx);
				pointy = cy;
				return;
			}
			else if(angle == 270) 
			{
				pointx = cx;
				pointy = (int)(cy+ry);
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

			pointx = cx+(int)x;
			pointy = cy-(int)y;
		}

        static public void MathGetEllipsePoint(int cx, int cy, double rx, double ry, double angle, out double pointx, out double pointy)
        {
            double x, y;
            double tansqrt;
            double radian;
            double rxsqrt, rysqrt;

            if (rx == 0 && ry == 0)
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
            if (angle == 0)
            {
                pointx = (cx + rx);
                pointy = cy;
                return;
            }
            else if (angle == 90)
            {
                pointx = cx;
                pointy = (cy - ry);
                return;
            }
            else if (angle == 180)
            {
                pointx = (cx - rx);
                pointy = cy;
                return;
            }
            else if (angle == 270)
            {
                pointx = cx;
                pointy = (cy + ry);
                return;
            }

            radian = PI / 180.0 * (double)angle;

            tansqrt = Math.Tan(radian) * Math.Tan(radian);

            rxsqrt = (double)rx * (double)rx;
            rysqrt = (double)ry * (double)ry;

            if ((angle >= 0 && angle <= 90) || (angle > 270 && angle <= 360))
            {
                x = Math.Sqrt(1.0 / (rysqrt + rxsqrt * tansqrt)) * (double)rx * (double)ry;
            }
            else
            {
                x = -Math.Sqrt(1.0 / (rysqrt + rxsqrt * tansqrt)) * (double)rx * (double)ry;
            }

            y = Math.Tan(radian) * x;

            pointx = cx + x;
            pointy = cy - y;
        }

		//------------------------------------------------------------------------------
		//	주어진 직각 삼각형의 빗변의 크기를 구한다.
		//------------------------------------------------------------------------------

		public static double MathGetHypotenuse(int x, int y)
		{
			x = Math.Abs(x);
			y = -1*Math.Abs(y);

			double radian = MathGradientToRadian(x, y);

            double value = y / Math.Sin(radian);
			if(value < 0)	value *= -1;
			return value;
		}


	}
}
