using System;
using Lunatics.Mathematics;

namespace Lunatics.Framework.Graphics
{
	public sealed class EffectParameter
	{
		public string Name
		{
			get;
			private set;
		}

		public string Semantic
		{
			get;
			private set;
		}

		public int RowCount
		{
			get;
			private set;
		}

		public int ColumnCount
		{
			get;
			private set;
		}

		public void SetValue(Matrix value)
		{
			// FIXME: All Matrix sizes... this will get ugly. -flibit
			unsafe
			{
				var dstPtr = (float*)values;
				if (ColumnCount == 4 && RowCount == 4)
				{
					dstPtr[0] = value.M11;
					dstPtr[1] = value.M21;
					dstPtr[2] = value.M31;
					dstPtr[3] = value.M41;
					dstPtr[4] = value.M12;
					dstPtr[5] = value.M22;
					dstPtr[6] = value.M32;
					dstPtr[7] = value.M42;
					dstPtr[8] = value.M13;
					dstPtr[9] = value.M23;
					dstPtr[10] = value.M33;
					dstPtr[11] = value.M43;
					dstPtr[12] = value.M14;
					dstPtr[13] = value.M24;
					dstPtr[14] = value.M34;
					dstPtr[15] = value.M44;
				}
				else if (ColumnCount == 3 && RowCount == 3)
				{
					dstPtr[0] = value.M11;
					dstPtr[1] = value.M21;
					dstPtr[2] = value.M31;
					dstPtr[4] = value.M12;
					dstPtr[5] = value.M22;
					dstPtr[6] = value.M32;
					dstPtr[8] = value.M13;
					dstPtr[9] = value.M23;
					dstPtr[10] = value.M33;
				}
				else if (ColumnCount == 4 && RowCount == 3)
				{
					dstPtr[0] = value.M11;
					dstPtr[1] = value.M21;
					dstPtr[2] = value.M31;
					dstPtr[3] = value.M41;
					dstPtr[4] = value.M12;
					dstPtr[5] = value.M22;
					dstPtr[6] = value.M32;
					dstPtr[7] = value.M42;
					dstPtr[8] = value.M13;
					dstPtr[9] = value.M23;
					dstPtr[10] = value.M33;
					dstPtr[11] = value.M43;
				}
				else if (ColumnCount == 3 && RowCount == 4)
				{
					dstPtr[0] = value.M11;
					dstPtr[1] = value.M21;
					dstPtr[2] = value.M31;
					dstPtr[4] = value.M12;
					dstPtr[5] = value.M22;
					dstPtr[6] = value.M32;
					dstPtr[8] = value.M13;
					dstPtr[9] = value.M23;
					dstPtr[10] = value.M33;
					dstPtr[12] = value.M14;
					dstPtr[13] = value.M24;
					dstPtr[14] = value.M34;
				}
				else if (ColumnCount == 2 && RowCount == 2)
				{
					dstPtr[0] = value.M11;
					dstPtr[1] = value.M21;
					dstPtr[4] = value.M12;
					dstPtr[5] = value.M22;
				}
				else
				{
					throw new NotImplementedException($"Matrix Size: {RowCount} {ColumnCount}");
				}
			}
		}

		internal IntPtr values;
	}
}