using System;
using System.Runtime.InteropServices;
using Lunatics.Mathematics;

namespace Lunatics.Framework.Graphics
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VertexPositionColor : IVertexType
	{
		public Vector3 Position;
		public Color Color;

		public VertexPositionColor(Vector3 position, Color color)
		{
			Position = position;
			Color = color;
		}

		public static readonly VertexDeclaration VertexDeclaration;

		static VertexPositionColor()
		{
			VertexDeclaration = new VertexDeclaration(
				new VertexElement(0,
				                  VertexElementFormat.Vector3,
				                  VertexElementUsage.Position,
				                  0),
				new VertexElement(12,
				                  VertexElementFormat.Color,
				                  VertexElementUsage.Color,
				                  1));
		}

		VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
	}
}