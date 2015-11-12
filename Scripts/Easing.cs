/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Dreambound Studios AB
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;

namespace Dreambound
{
	public static class Easing
	{
		// Adapted from source : http://www.robertpenner.com/easing/
		public static float Ease(float linearStep, float acceleration, EasingType type)
		{
			float easedStep = acceleration > 0 ? EaseIn(linearStep, type) : acceleration < 0 ? EaseOut(linearStep, type) : (float)linearStep;
			return MathHelper.Lerp(linearStep, easedStep, Math.Abs(acceleration));
		}

		public static float EaseIn(float linearStep, EasingType type)
		{
			switch (type)
			{
				case EasingType.Step:
					return linearStep < 0.5 ? 0 : 1;
				case EasingType.Linear:
					return (float)linearStep;
				case EasingType.Sine:
					return Sine.EaseIn(linearStep);
				case EasingType.Quadratic:
					return Power.EaseIn(linearStep, 2);
				case EasingType.Cubic:
					return Power.EaseIn(linearStep, 3);
				case EasingType.Quartic:
					return Power.EaseIn(linearStep, 4);
				case EasingType.Quintic:
					return Power.EaseIn(linearStep, 5);
			}

			throw new NotImplementedException();
		}

		public static float EaseOut(float linearStep, EasingType type)
		{
			switch (type)
			{
				case EasingType.Step:
					return linearStep < 0.5 ? 0 : 1;
				case EasingType.Linear:
					return (float)linearStep;
				case EasingType.Sine:
					return Sine.EaseOut(linearStep);
				case EasingType.Quadratic:
					return Power.EaseOut(linearStep, 2);
				case EasingType.Cubic:
					return Power.EaseOut(linearStep, 3);
				case EasingType.Quartic:
					return Power.EaseOut(linearStep, 4);
				case EasingType.Quintic:
					return Power.EaseOut(linearStep, 5);
			}
			throw new NotImplementedException();
		}

		public static float EaseInOut(float linearStep, EasingType easeInType, EasingType easeOutType)
		{
			return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
		}

		public static float EaseInOut(float linearStep, EasingType type)
		{
			switch (type)
			{
				case EasingType.Step:
					return linearStep < 0.5 ? 0 : 1;
				case EasingType.Linear:
					return (float)linearStep;
				case EasingType.Sine:
					return Sine.EaseInOut(linearStep);
				case EasingType.Quadratic:
					return Power.EaseInOut(linearStep, 2);
				case EasingType.Cubic:
					return Power.EaseInOut(linearStep, 3);
				case EasingType.Quartic:
					return Power.EaseInOut(linearStep, 4);
				case EasingType.Quintic:
					return Power.EaseInOut(linearStep, 5);
			}
			throw new NotImplementedException();
		}

		static class Sine
		{
			public static float EaseIn(float s)
			{
				return (float)Math.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1;
			}

			public static float EaseOut(float s)
			{
				return (float)Math.Sin(s * MathHelper.HalfPi);
			}

			public static float EaseInOut(float s)
			{
				return (float)(Math.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1) / 2;
			}
		}

		static class Power
		{
			public static float EaseIn(float s, int power)
			{
				return (float)Math.Pow(s, power);
			}

			public static float EaseOut(float s, int power)
			{
				var sign = power % 2 == 0 ? -1 : 1;
				return (float)(sign * (Math.Pow(s - 1, power) + sign));
			}

			public static float EaseInOut(float s, int power)
			{
				s *= 2;
				if (s < 1)
					return EaseIn(s, power) / 2;
				var sign = power % 2 == 0 ? -1 : 1;
				return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
			}
		}
	}

	public enum EasingType
	{
		Step,
		Linear,
		Sine,
		Quadratic,
		Cubic,
		Quartic,
		Quintic
	}

	public static class MathHelper
	{
		public const float Pi = (float)Math.PI;
		public const float HalfPi = (float)(Math.PI / 2f);

		public static float Lerp(float from, float to, float step)
		{
			return (float)((to - from) * step + from);
		}
	}
}