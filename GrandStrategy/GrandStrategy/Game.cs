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
	color = texture( myTextureSampler, UV );
}";

		protected override void LoadContent()
		{
			base.LoadContent();

			_mapTexture = Texture2D.Load(GraphicsDevice, "assets/map.png");
			//_mapTexture = Texture2D.Load(GraphicsDevice, "assets/tex.png");
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_vertexShader = GraphicsDevice.CreateVertexShader(VertexShaderCode);
			_pixelShader = GraphicsDevice.CreatePixelShader(PixelShaderCode);

			_vertexBuffer = GraphicsDevice.CreateVertexBuffer(VertexPositionColorTexture.VertexDeclaration, 6, BufferUsage.None, false);
			_vertexBuffer.SetData(_bufferData1);
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

			var vp = GraphicsDevice.Viewport;
			var projection = Matrix.PerspectiveFovRH(MathUtils.DegreesToRadians(45f), 
			                                         (float)vp.Width / vp.Height, 
			                                         0.1f, 
			                                         100.0f);
			var view = Matrix.LookAtRH(
				new Vector3(4, 3, 3), // Camera is at (4,3,3), in World Space
				new Vector3(0, 0, 0), // and looks at the origin
				new Vector3(0, 1, 0) // Head is up (set to 0,-1,0 to look upside-down)
			);

			var model = Matrix.Identity;
			var mvp = model * view * projection;
			var spritePrj = Matrix.OrthoOffCenterRH(0f, vp.Width, vp.Height, 0f, 0f, 1f);
			

			GraphicsDevice.VertexShader = _vertexShader;
			GraphicsDevice.PixelShader = _pixelShader;

			
			_vertexShader.SetMatrix4("MVP", ref spritePrj);
			_spriteBatch.Begin();
			_spriteBatch.Draw(_mapTexture,
							  new Vector2(400,300), 
							  null,
							  Color.White,
							  0,
							  Vector2.Zero, //new Vector2(0.5f, 0.5f),
							  Vector2.One, //new Vector2(0.0004f, 0.0005f),
							  SpriteEffects.None,
							  0);
			_spriteBatch.End();

			_vertexShader.SetMatrix4("MVP", ref mvp);
			GraphicsDevice.SetVertexBuffer(_vertexBuffer);
			GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
			//GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

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

		private VertexPositionColorTexture[] _bufferData1 =
		{
			new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, 0.0f), new Color(1f, 0f, 0f, 1f), new Vector2(0f,1f)),
			new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f, 0.0f), new Color(0, 1f, 0f, 1f), new Vector2(0f,0f)),
			new VertexPositionColorTexture(new Vector3(1.0f, -1.0f, 0.0f), new Color(0, 0f, 1f, 1f), new Vector2(1f,1f)),

			new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f, 0.0f), new Color(0, 1f, 0f, 1f), new Vector2(0f,0f)),
			new VertexPositionColorTexture(new Vector3(1.0f, 1.0f, 0.0f), new Color(1f, 0f, 0f, 1f), new Vector2(1f,0f)),
			new VertexPositionColorTexture(new Vector3(1.0f, -1.0f, 0.0f), new Color(0, 0f, 1f, 1f), new Vector2(1f,1f)),
		};

		private VertexPositionColorTexture[] _bufferData2 =
		{
			new VertexPositionColorTexture(new Vector3(-400f, 300f, 0.0f), Color.White, new Vector2(0f, 0f)),
			new VertexPositionColorTexture(new Vector3(400f, 300f, 0.0f), Color.White, new Vector2(1f, 0f)),
			new VertexPositionColorTexture(new Vector3(-400f, -300f, 0.0f), Color.White, new Vector2(0f, 1f)),

			new VertexPositionColorTexture(new Vector3(400f, -300f, 0.0f), new Color(0f, 0f, 1f, 1f), new Vector2(1f, 1f)),
			new VertexPositionColorTexture(new Vector3(-400f, -300f, 0.0f), Color.White, new Vector2(0f, 1f)),
			new VertexPositionColorTexture(new Vector3(400f, 300f, 0.0f), Color.White, new Vector2(1f, 0f)),
		};

		private VertexPositionColorTexture[] _bufferData3 =
		{
			new VertexPositionColorTexture(new Vector3(-400f, 300f, 0.0f), Color.White, new Vector2(0f, 0f)),
			new VertexPositionColorTexture(new Vector3(400f, 300f, 0.0f), Color.White, new Vector2(1f, 0f)),
			new VertexPositionColorTexture(new Vector3(-400f, -300f, 0.0f), Color.White, new Vector2(0f, 1f)),
			new VertexPositionColorTexture(new Vector3(400f, -300f, 0.0f), new Color(0f, 0f, 1f, 1f), new Vector2(1f, 1f)),
		};

		private VertexPositionColorTexture[] _bufferData4 =
		{
			new VertexPositionColorTexture(new Vector3(0f, 0f, 0.0f), Color.White, new Vector2(0f, 0f)),
			new VertexPositionColorTexture(new Vector3(256f, 0f, 0.0f), Color.White, new Vector2(1f, 0f)),
			new VertexPositionColorTexture(new Vector3(0f, 256f, 0.0f), Color.White, new Vector2(0f, 1f)),
			new VertexPositionColorTexture(new Vector3(256f, 256f, 0.0f), new Color(0f, 0f, 1f, 1f), new Vector2(1f, 1f)),
		};
	}
}