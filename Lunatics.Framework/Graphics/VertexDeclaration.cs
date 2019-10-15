using System;

namespace Lunatics.Framework.Graphics
{
	public struct VertexElement
	{
		#region Public Properties

		public int Offset { get; set; }

		public VertexElementFormat VertexElementFormat { get; set; }

		public VertexElementUsage VertexElementUsage { get; set; }

		public int UsageIndex { get; set; }

		#endregion

		#region Public Constructor

		public VertexElement(
			int offset,
			VertexElementFormat elementFormat,
			VertexElementUsage elementUsage,
			int usageIndex
		) : this()
		{
			Offset = offset;
			UsageIndex = usageIndex;
			VertexElementFormat = elementFormat;
			VertexElementUsage = elementUsage;
		}

		#endregion

		#region Public Static Operators and Override Methods

		public override int GetHashCode()
		{
			// TODO: Fix hashes
			return 0;
		}

		public override string ToString()
		{
			return "{{Offset:" + Offset.ToString() +
			       " Format:" + VertexElementFormat.ToString() +
			       " Usage:" + VertexElementUsage.ToString() +
			       " UsageIndex: " + UsageIndex.ToString() +
			       "}}";
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (obj.GetType() != base.GetType())
			{
				return false;
			}

			return this == (VertexElement) obj;
		}

		public static bool operator ==(VertexElement left, VertexElement right)
		{
			return left.Offset == right.Offset &&
			       left.UsageIndex == right.UsageIndex &&
			       left.VertexElementUsage == right.VertexElementUsage &&
			       left.VertexElementFormat == right.VertexElementFormat;
		}

		public static bool operator !=(VertexElement left, VertexElement right)
		{
			return !(left == right);
		}

		#endregion
	}

	public class VertexDeclaration : GraphicsResource
	{
		#region Public Properties

		public int VertexStride { get; }

		#endregion

		#region Internal Variables

		internal VertexElement[] elements;

		#endregion

		#region Public Constructors

		public VertexDeclaration(params VertexElement[] elements)
			: this(GetVertexStride(elements), elements)
		{
		}

		public VertexDeclaration(int vertexStride, params VertexElement[] elements)
		{
			if (elements == null || elements.Length == 0)
			{
				throw new ArgumentNullException(nameof(elements), "Elements cannot be empty");
			}

			this.elements = (VertexElement[]) elements.Clone();
			VertexStride = vertexStride;
		}

		#endregion

		#region Public Methods

		public VertexElement[] GetVertexElements()
		{
			return (VertexElement[]) elements.Clone();
		}

		#endregion

		#region Internal Static Methods

		/// <summary>
		/// Returns the VertexDeclaration for Type.
		/// </summary>
		/// <param name="vertexType">A value type which implements the IVertexType interface.</param>
		/// <returns>The VertexDeclaration.</returns>
		/// <remarks>
		/// Prefer to use VertexDeclarationCache when the declaration lookup
		/// can be performed with a templated type.
		/// </remarks>
		internal static VertexDeclaration FromType(Type vertexType)
		{
			if (vertexType == null)
			{
				throw new ArgumentNullException(nameof(vertexType), "Cannot be null");
			}

			if (!vertexType.IsValueType)
			{
				throw new ArgumentException(nameof(vertexType), "Must be value type");
			}

			if (!(Activator.CreateInstance(vertexType) is IVertexType type))
			{
				throw new ArgumentException("vertexData does not inherit IVertexType");
			}

			VertexDeclaration vertexDeclaration = type.VertexDeclaration;
			if (vertexDeclaration == null)
			{
				throw new ArgumentException("vertexType's VertexDeclaration cannot be null");
			}

			return vertexDeclaration;
		}

		#endregion

		#region Private Static VertexElement Methods

		private static int GetVertexStride(VertexElement[] elements)
		{
			var max = 0;

			for (var i = 0; i < elements.Length; i += 1)
			{
				var start = elements[i].Offset + GetTypeSize(elements[i].VertexElementFormat);
				if (max < start)
					max = start;
			}

			return max;
		}

		private static int GetTypeSize(VertexElementFormat elementFormat)
		{
			switch (elementFormat)
			{
				case VertexElementFormat.Single:
					return 4;
				case VertexElementFormat.Vector2:
					return 8;
				case VertexElementFormat.Vector3:
					return 12;
				case VertexElementFormat.Vector4:
					return 16;
				case VertexElementFormat.Color:
					return 4;
				case VertexElementFormat.Byte4:
					return 4;
				case VertexElementFormat.Short2:
					return 4;
				case VertexElementFormat.Short4:
					return 8;
				case VertexElementFormat.NormalizedShort2:
					return 4;
				case VertexElementFormat.NormalizedShort4:
					return 8;
				case VertexElementFormat.HalfVector2:
					return 4;
				case VertexElementFormat.HalfVector4:
					return 8;
			}

			return 0;
		}

		#endregion
	}
}