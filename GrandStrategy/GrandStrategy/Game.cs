using System;
using System.IO;
using Lunatics.Framework;
using Lunatics.Framework.Graphics;
using Lunatics.Framework.Input;
using Lunatics.Mathematics;

namespace GrandStrategy
{

	public class Game : Lunatics.Framework.Game
	{
		public Game(Func<Lunatics.Framework.Game, GamePlatform> platformFactory,
		            Func<GraphicsAdapter, PresentationParameters, GraphicsDevice> graphicsDeviceFactory)
			: base(platformFactory, graphicsDeviceFactory)
		{
			PreferredBackBufferWidth = 1280;
			PreferredBackBufferHeight = 720;
		}

		protected override void Initialize()
		{
			var vp = GraphicsDevice.Viewport;
			_position = new Vector2(vp.Width / 2f, vp.Height / 2f);

			base.Initialize();
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
		
		protected override void LoadContent()
		{
			base.LoadContent();

			_mapTexture = Texture2D.Load(GraphicsDevice, "assets/map.png");
			//_mapTexture = Texture2D.Load(GraphicsDevice, "assets/tex.png");
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_vertexShader = GraphicsDevice.CreateVertexShader(File.ReadAllText("assets/Shaders/VertexShader_OpenGL.txt"));
			_pixelShader = GraphicsDevice.CreatePixelShader(File.ReadAllText("assets/Shaders/PixelShader_OpenGL.txt"));

			_vertexBuffer = GraphicsDevice.CreateVertexBuffer(VertexPositionColorTexture.VertexDeclaration, 6, BufferUsage.None, false);
			_vertexBuffer.SetData(_bufferData1);

			_renderer = SharpCEGui.OpenGLRenderer.OpenGL3Renderer.Create();
			//new SharpCEGuiNLogger();
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), GameName);
			var file = Path.Combine(path, "CEGUISharp.log");
			_system = SharpCEGui.Base.System.Create(_renderer,
													null/*new PackedResourceProvider(Path.Combine(GetCurrentDirectory(),"resources.pack"))*/,
													new SharpCEGui.Base.DefaultXmlParser(),
													null,
													String.Empty,
													file);

			//var scheme = String.Empty;
			//var prefix = "00-";

			//InitializeScheme(ref scheme, ref prefix);
			//InitialiseDefaultResourceGroups();
			//InitialiseResourceGroupDirectories();

#if WINDOWS
            _system.GetClipboard().SetNativeProvider(new WindowsClipboardProvider());
#elif __MACOS__
            _system.GetClipboard().SetNativeProvider(new MacOSClipboardProvider());
#endif

			_guiContext = _system.GetDefaultGUIContext();
			_guiContext.GetCursor().SetVisible(false);
			_inputAggregator = new SharpCEGui.Base.InputAggregator(_guiContext);
			_inputAggregator.Initialise();

			// TODO: ?? Window.TextInput += (sender, args) => _inputAggregator.InjectChar(args.Character);

			// TODO: _viewManager = new ViewManager(this, scheme, prefix);
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
			var ms = Mouse.GetState();
			var ks = Keyboard.GetState();

			if (ks.IsKeyDown(Keys.A))
				_position.X += 10;
			if (ks.IsKeyDown(Keys.D))
				_position.X -= 10;
			if (ks.IsKeyDown(Keys.W))
				_position.Y += 10;
			if (ks.IsKeyDown(Keys.S))
				_position.Y -= 10;

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
			                     new Vector4(1f, 0f, 0f, 1f), 1f, 0);

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
							  _position, 
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

		private Vector2 _position;

		private Shader _pixelShader;
		private Shader _vertexShader;
		private Texture2D _mapTexture;
		private SpriteBatch _spriteBatch;
		
		private VertexBuffer _vertexBuffer;

		private SharpCEGui.Base.System _system;
		private SharpCEGui.Base.Renderer _renderer;
		private SharpCEGui.Base.GUIContext _guiContext;
		private SharpCEGui.Base.InputAggregator _inputAggregator;

		public const string GameName = "WBMX";

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