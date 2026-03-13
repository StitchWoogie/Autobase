using System;
using System.Drawing;

namespace GraphicModule
{
	internal static class DemandChartThemeHelper
	{
		public static int ClampTheme(int theme)
		{
			return Math.Max(0, Math.Min(2, theme));
		}

		public static Color ApplySurface(Color c, int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Blend(c, Color.FromArgb(24, 30, 38), 0.62);
			if (theme == 2) return Blend(c, Color.FromArgb(250, 250, 250), 0.78);
			return c;
		}

		public static Color ApplyCardBack(Color c, int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Blend(c, Color.FromArgb(30, 38, 49), 0.66);
			if (theme == 2) return Blend(c, Color.White, 0.84);
			return c;
		}

		public static Color ApplyCardBorder(Color c, int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Blend(c, Color.FromArgb(102, 128, 146), 0.65);
			if (theme == 2) return Blend(c, Color.FromArgb(10, 10, 10), 0.70);
			return c;
		}

		public static Color ApplyGuideLine(Color c, int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Blend(c, Color.FromArgb(142, 160, 176), 0.58);
			if (theme == 2) return Blend(c, Color.Black, 0.65);
			return c;
		}

		public static Color ApplyText(Color c, int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Blend(c, Color.FromArgb(226, 233, 242), 0.82);
			if (theme == 2) return Blend(c, Color.Black, 0.92);
			return c;
		}

		public static Color ApplyAccent(Color c, int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Blend(c, Color.FromArgb(118, 198, 255), 0.25);
			if (theme == 2) return Blend(c, Color.FromArgb(0, 0, 0), 0.12);
			return c;
		}

		public static Color ApplyDanger(Color c, int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Blend(c, Color.FromArgb(255, 120, 120), 0.24);
			if (theme == 2) return Blend(c, Color.FromArgb(20, 20, 20), 0.16);
			return c;
		}

		public static Color ApplyStatus(Color c, int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Blend(c, Color.FromArgb(60, 84, 108), 0.40);
			if (theme == 2) return Blend(c, Color.FromArgb(20, 20, 20), 0.60);
			return c;
		}

		public static Color GetFrameColor(int theme)
		{
			theme = ClampTheme(theme);
			if (theme == 1) return Color.FromArgb(120, 140, 160, 180);
			if (theme == 2) return Color.FromArgb(210, 20, 20, 20);
			return Color.FromArgb(60, 74, 98, 122);
		}

		private static Color Blend(Color from, Color to, double ratio)
		{
			if (ratio <= 0) return from;
			if (ratio >= 1) return Color.FromArgb(from.A, to.R, to.G, to.B);
			int r = (int)Math.Round(from.R + (to.R - from.R) * ratio);
			int g = (int)Math.Round(from.G + (to.G - from.G) * ratio);
			int b = (int)Math.Round(from.B + (to.B - from.B) * ratio);
			return Color.FromArgb(from.A, ClampByte(r), ClampByte(g), ClampByte(b));
		}

		private static int ClampByte(int value)
		{
			return Math.Max(0, Math.Min(255, value));
		}
	}
}
