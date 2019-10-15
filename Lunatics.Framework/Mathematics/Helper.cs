using System;

namespace Lunatics.Framework.Mathematics
{
	public class Helper
	{
		public const float Pi = (float)Math.PI;

		public static float Clamp(float value, float min, float max)
		{
			value = (value > max) ? max : value;
			value = (value < min) ? min : value;
			return value;
		}

		internal static int Clamp(int value, int min, int max)
		{
			value = (value > max) ? max : value;
			value = (value < min) ? min : value;
			return value;
		}

		/// <summary>
		/// Find the current machine's Epsilon for the float data type.
		/// (That is, the largest float, e,  where e == 0.0f is true.)
		/// </summary>
		private static float GetMachineEpsilonFloat()
		{
			float machineEpsilon = 1.0f;
			float comparison;

			/* Keep halving the working value of machineEpsilon until we get a number that
			 * when added to 1.0f will still evaluate as equal to 1.0f.
			 */
			do
			{
				machineEpsilon *= 0.5f;
				comparison = 1.0f + machineEpsilon;
			}
			while (comparison > 1.0f);

			return machineEpsilon;
		}
	}
}