namespace VidTools.Vis
{
	// All functions take a time value 't' between 0 and 1 and return the eased result.
	// Ease In: starts slow and accelerates
	// Ease Out: starts fast and decelerates
	// Ease InOut: starts slow, speeds up in the middle, and slows down at the end

	// Thanks to www.easings.net for most of these functions.

	public static class Ease
	{

		public enum EaseType
		{
			Linear,
			QuadIn, QuadOut, QuadInOut,
			CubicIn, CubicOut, CubicInOut,
			QuarticIn, QuarticOut, QuarticInOut,
			OvershootIn, OvershootOut, OvershootInOut,
			ElasticOut
		}


		public static float GetEasing(float t, EaseType type) => type switch
		{
			EaseType.Linear => Linear.GetValue(t),
			EaseType.QuadIn => Quadratic.In(t),
			EaseType.QuadOut => Quadratic.Out(t),
			EaseType.QuadInOut => Quadratic.InOut(t),
			EaseType.CubicIn => Cubic.In(t),
			EaseType.CubicOut => Cubic.Out(t),
			EaseType.CubicInOut => Cubic.InOut(t),
			EaseType.QuarticIn => Quartic.In(t),
			EaseType.QuarticOut => Quartic.Out(t),
			EaseType.QuarticInOut => Quartic.InOut(t),
			EaseType.OvershootIn => Overshoot.In(t),
			EaseType.OvershootOut => Overshoot.Out(t),
			EaseType.OvershootInOut => Overshoot.InOut(t),
			EaseType.ElasticOut => Elastic.Out(t),
			_ => 0
		};

		// Linear easing (i.e. no easing)
		public static class Linear
		{
			public static float GetValue(float t) => Clamp01(t);
		}

		// Quadratic easing
		public static class Quadratic
		{
			public static float In(float t) => Square(Clamp01(t));

			public static float Out(float t) => 1 - Square(1 - Clamp01(t));

			public static float InOut(float t) => 3 * Square(Clamp01(t)) - 2 * Cube(Clamp01(t));
		}

		// Cubic easing
		public static class Cubic
		{
			public static float In(float t) => Cube(Clamp01(t));

			public static float Out(float t) => 1 - Cube(1 - Clamp01(t));

			public static float InOut(float t)
			{
				t = Clamp01(t);
				int r = (int)System.Math.Round(t);
				return 4 * Cube(t) * (1 - r) + (1 - 4 * Cube(1 - t)) * r;
			}
		}

		public static class Quartic
		{
			public static float In(float t) => Quart(Clamp01(t));

			public static float Out(float t) => 1 - Quart(1 - Clamp01(t));

			public static float InOut(float t)
			{
				t = Clamp01(t);
				int r = (int)System.Math.Round(t);
				return 8 * Quart(t) * (1 - r) + (1 - 8 * Quart(1 - t)) * r;
			}
		}

		public static class Overshoot
		{
			const float c = 1.70158f;
			const float cp1 = c + 1;

			public static float In(float t)
			{
				t = Clamp01(t);
				return (c + 1) * Cube(t) - c * Square(t);
			}

			public static float Out(float t)
			{
				t = Clamp01(t);
				return 1 + (c + 1) * Cube(t - 1) + c * Square(t - 1);
			}

			public static float InOut(float t)
			{
				t = Clamp01(t);
				const float c2 = c * 1.525f;

				float a = (Square(2 * t) * ((c2 + 1) * 2 * t - c2)) / 2;
				float b = (Square(2 * t - 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
				return t < 0.5 ? a : b;
			}
		}

		public static class Asymmetric
		{
			public static float QuadInCubeOut(float t)
			{
				float blend = Cubic.InOut(t);
				return Lerp(Quadratic.In(t), Cubic.Out(t), blend);
			}

			public static float CubeInQuadOut(float t)
			{
				float blend = Cubic.InOut(t);
				return Lerp(Cubic.In(t), Quadratic.Out(t), blend);
			}
		}

		public static class Elastic
		{
			public static float Out(float t)
			{
				t = Clamp01(t);
				if (t == 0 || t == 1)
				{
					return t;
				}

				float a = System.MathF.Pow(2, -10 * t);
				float b = System.MathF.Sin((t - .075f) * (2 * System.MathF.PI) / 0.3f);
				return a * b + 1;
			}
		}

		static float Clamp01(float t) => System.Math.Clamp(t, 0, 1);
		static float Square(float x) => x * x;
		static float Cube(float x) => x * x * x;
		static float Quart(float x) => x * x * x * x;
		static float Lerp(float a, float b, float t) => a * (1 - Clamp01(t)) + b * Clamp01(t);

	}
}