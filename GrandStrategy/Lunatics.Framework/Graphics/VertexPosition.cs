using System;
using System.Runtime.InteropServices;
using Lunatics.Mathematics;

namespace Lunatics.Framework.Graphics
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VertexPosition : IVertexType
	{
		public Vector3 Position;

		public VertexPosition(Vector3 position)
		{
			Position = position;
		}

		public static readonly VertexDeclaration VertexDeclaration;

		static VertexPosition()
		{
			VertexDeclaration = new VertexDeclaration(
				new VertexElement(0,
				                  VertexElementFormat.Vector3,
				                  VertexElementUsage.Position,
				                  0));
		}

		VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
	}
}