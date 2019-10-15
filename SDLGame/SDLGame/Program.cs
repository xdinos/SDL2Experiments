
using System;
using System.IO;
using System.Runtime.InteropServices;
using Lunatics.Framework;
using Lunatics.Framework.Graphics;
using Lunatics.Framework.Mathematics;
using Lunatics.Framework.Sdl;

namespace SDLGame
{
	public abstract class BaseGameObj
	{
		public abstract string SayHello();
	}

	public class InCodeGameObj : BaseGameObj
	{
		public override string SayHello()
		{
			return $"Hello from {nameof(InCodeGameObj)}";
		}
	}

	public class ScriptExports
	{
		public void WriteLine(string text)
		{
			System.Console.WriteLine(text);
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VertexPositionColorTexture : IVertexType
	{
		public Vector3 Position;
		public Color Color;
		public Vector2 TextureCoordinate;

		VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

		public static readonly VertexDeclaration VertexDeclaration;

		static VertexPositionColorTexture()
		{
			VertexDeclaration = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
				new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VertexPosition : IVertexType
	{
		public Vector3 Position;

		VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

		public static readonly VertexDeclaration VertexDeclaration;

		static VertexPosition()
		{
			VertexDeclaration = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));
		}
	}

	public class MyNewGame : Lunatics.Framework.Game
	{
		public MyNewGame(string title, Platform platform)
			: base(title, platform)
		{

		}

		protected override void Initialize()
		{
			base.Initialize();

			_vertexBuffer =
				GraphicsDevice.CreateVertexBuffer(VertexPosition.VertexDeclaration, 3, BufferUsage.WriteOnly);
		}

		protected override void Draw(TimeSpan elapsedGameTime)
		{
			GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil,
			                     new Vector4(0f, 0f, 0f, 1f), 0f, 0);

			var vertexData = new[]
			                 {
				                 new VertexPosition {Position = new Vector3(-1.0f, -1.0f, 0.0f)},
				                 new VertexPosition {Position = new Vector3(1.0f, -1.0f, 0.0f)},
				                 new VertexPosition {Position = new Vector3(0.0f, 1.0f, 0.0f)}
			                 };

			_vertexBuffer.SetData(vertexData);
			
			GraphicsDevice.SetVertexBuffer(_vertexBuffer);

			GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);

			//GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
			//                                  vertexData, 0, 3,
			//                                  _vertexBuffer.VertexDeclaration);

			base.Draw(elapsedGameTime);
		}

		private VertexBuffer _vertexBuffer;
	}


	static class Program
	{
		static void Main()
		{
			//var stream = File.OpenRead(@"E:\Program Files (x86)\Steam\steamapps\common\Thimbleweed Park\Resources\ThimbleweedPark.ggpack1");
			//var reader = new GGBinaryReader(new BinaryReader(stream));
			//var pack = reader.ReadPack();

			//foreach (var entry in pack.Entries)
			//{
			//	using (var ps = pack.GetEntryStream(entry.Name))
			//	{
			//		var buffer = new byte[entry.Size];
			//		ps.Read(buffer, 0, entry.Size);
			//		if (entry.Name.EndsWith(".wimpy"))
			//		{
			//			var result = reader.ReadWimpy(buffer);
			//			File.WriteAllText(Path.Combine(@"E:\Development\lunatic\_park\1", entry.Name),
			//			                  result.ToString());
			//		}
			//		else if (entry.Name.Contains("Animation"))
			//		{
			//			var result = reader.ReadWimpy(buffer);
			//			File.WriteAllText(Path.Combine(@"E:\Development\lunatic\_park\1", entry.Name),
			//			                  result.ToString());
			//		}
			//		else
			//		{
			//			File.WriteAllBytes(Path.Combine(@"E:\Development\lunatic\_park\1", entry.Name), buffer);
			//		}
			//	}
			//}

			var scriptEngine = new ScriptEngine();
			//var result = scriptEngine.EvaluateAsync<int>("2+2").GetAwaiter().GetResult();
			//var result = scriptEngine.EvaluateAsync<BaseGameObj>("new SDLGame.InCodeGameObj()").GetAwaiter().GetResult();
			var result = scriptEngine.EvaluateAsync<BaseGameObj>(
				                         File.ReadAllText("assets/TestScript.csx"),
				                         new ScriptExports())
			                         .GetAwaiter()
			                         .GetResult();
			System.Console.WriteLine(result.SayHello());

			//using (var game = new Game())
			//{
			//	game.Run();
			//}

			using (var game = new MyNewGame("MyNewGame", new SDLPlatform()))
			{
				game.Run();
			}
		}
	}

}
