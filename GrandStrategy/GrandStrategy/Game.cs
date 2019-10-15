using System;
using Lunatics.Framework;
using Lunatics.Framework.DesktopGL.Graphics;
using Lunatics.Framework.Graphics;
using Lunatics.Mathematics;

namespace GrandStrategy
{

	public class Game : Lunatics.Framework.Game
	{
		public Game()
			: base(g => new Lunatics.Framework.DesktopGL.SdlPlatform(g),
			       (adapter, presentationParameters) => new OpenGLGraphicsDevice(adapter, presentationParameters))
		{

		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private readonly string VertexShaderCode =
			@"#version 330 core 
layout(location = 0) in vec3 vertexPosition_modelspace;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 vertexUV;
uniform mat4 MVP;
out vec4 fragmentColor;
out vec2 UV;
void main() {
	//gl_Position.xyz = vertexPosition_modelspace;
	//gl_Position.w = 1.0;
	gl_Position =  MVP * vec4(vertexPosition_modelspace, 1);
	fragmentColor = vertexColor;
	UV = vertexUV;
}";

		private readonly string PixelShaderCode =
			@"#version 330 core
uniform sampler2D myTextureSampler;
in vec4 fragmentColor;
in vec2 UV;
out vec4 color;
void main() {
	//color = vec3(0,1,0);
	//color = fragmentColor;
	color = texture( myTextureSampler, UV )*fragmentColor;
}";

		protected override void LoadContent()
		{
			base.LoadContent();

			_mapTexture = Texture2D.Load(GraphicsDevice, "assets/map.png");
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_vertexShader = GraphicsDevice.CreateVertexShader(VertexShaderCode);
			_pixelShader = GraphicsDevice.CreatePixelShader(PixelShaderCode);

			_vertexBuffer = GraphicsDevice.CreateVertexBuffer(VertexPositionColorTexture.VertexDeclaration, 6, BufferUsage.None, false);
			_vertexBuffer.SetData(_bufferData);
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();

			if (_spriteBatch != null)
			{
				_spriteBatch.Dispose();
				_spriteBatch = null;
			}
		}

		protected override void BeginRun()
		{
			base.BeginRun();
		}

		protected override void EndRun()
		{
			base.EndRun();
		}

		protected override void Update(TimeSpan elapsedGameTime)
		{
			base.Update(elapsedGameTime);
		}

		protected override bool BeginDraw()
		{
			if (!base.BeginDraw())
				return false;

			return true;
		}

		protected override void Draw(TimeSpan elapsedGameTime)
		{
			GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil,
			                     new Vector4(0f, 0f, 0f, 1f), 1f, 0);

			//_spriteBatch.Begin();
			//_spriteBatch.Draw(_mapTexture, Vector2.Zero, Color.White);
			//_spriteBatch.End();

			var projection = Matrix.PerspectiveFovRH(MathUtils.DegreesToRadians(45f), 4f / 3f, 0.1f, 100.0f);
			var view = Matrix.LookAtRH(
				new Vector3(0, 0, 3), // Camera is at (4,3,3), in World Space
				new Vector3(0, 0, 0), // and looks at the origin
				new Vector3(0, 1, 0) // Head is up (set to 0,-1,0 to look upside-down)
			);
			var model = Matrix.Identity;
			var mvp = model * view * projection;

			GraphicsDevice.SetVertexBuffer(_vertexBuffer);

			GraphicsDevice.VertexShader = _vertexShader;
			GraphicsDevice.PixelShader = _pixelShader;
			

			_vertexShader.SetMatrix4("MVP", ref mvp);
			//_pixelShader.Set

			GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

			base.Draw(elapsedGameTime);
		}

		protected override void EndDraw()
		{
			base.EndDraw();
		}


		private Shader _pixelShader;
		private Shader _vertexShader;
		private Texture2D _mapTexture;
		private SpriteBatch _spriteBatch;
		
		private VertexBuffer _vertexBuffer;
		private Matrix _globalTransformation = Matrix.Identity;

		private VertexPositionColorTexture[] _bufferData =
		{
			new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, 0.0f), new Color(1f, 0f, 0f, 1f), new Vector2(0f,1f)),
			new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f, 0.0f), new Color(0, 1f, 0f, 1f), new Vector2(0f,0f)),
			new VertexPositionColorTexture(new Vector3(1.0f, -1.0f, 0.0f), new Color(0, 0f, 1f, 1f), new Vector2(1f,1f)),

			new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f, 0.0f), new Color(0, 1f, 0f, 1f), new Vector2(0f,0f)),
			new VertexPositionColorTexture(new Vector3(1.0f, 1.0f, 0.0f), new Color(1f, 0f, 0f, 1f), new Vector2(1f,0f)),
			new VertexPositionColorTexture(new Vector3(1.0f, -1.0f, 0.0f), new Color(0, 0f, 1f, 1f), new Vector2(1f,1f)),
		};
	}
}