using System;
using System.Collections.Generic;

namespace Lunatics.Framework.Graphics
{
	public class VertexDeclaration : GraphicsResource
	{
		public int VertexStride
		{
			get;
			private set;
		}

		/*internal*/public VertexElement[] elements;

		public VertexDeclaration(params VertexElement[] elements)
			: this(GetVertexStride(elements), elements)
		{
		}

		public VertexDeclaration(int vertexStride, params VertexElement[] elements)
		{
			if ((elements == null) || (elements.Length == 0))
			{
				throw new ArgumentNullException(nameof(elements), "Elements cannot be empty");
			}

			this.elements = (VertexElement[]) elements.Clone();
			VertexStride = vertexStride;
		}

		public VertexElement[] GetVertexElements()
		{
			return (VertexElement[])elements.Clone();
		}
		
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
				throw new ArgumentException("vertexType", "Must be value type");
			}

			IVertexType type = Activator.CreateInstance(vertexType) as IVertexType;
			if (type == null)
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

		private static int GetVertexStride(IReadOnlyList<VertexElement> elements)
		{
			var max = 0;

			for (var i = 0; i < elements.Count; i += 1)
			{
				var start = elements[i].Offset + GetTypeSize(elements[i].VertexElementFormat);
				if (max < start)
				{
					max = start;
				}
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
	}
}