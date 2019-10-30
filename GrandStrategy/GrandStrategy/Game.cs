using System;
using System.Configuration;
using System.IO;
using Lunatics.Framework;
using Lunatics.Framework.Graphics;
using Lunatics.Framework.Input;
using Lunatics.Mathematics;
using SharpCEGui.Base;

namespace GrandStrategy
{

	public class Game : Lunatics.Framework.Game
	{
		public Game(Func<Lunatics.Framework.Game, GamePlatform> platformFactory,
		            Func<GraphicsAdapter, PresentationParameters, GraphicsDevice> graphicsDeviceFactory)
			: base(platformFactory, graphicsDeviceFactory)
		{
			PreferredBackBufferWidth = 1366;
			PreferredBackBufferHeight = 768;
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

			var scheme = String.Empty;
			var prefix = "00";

			//InitializeScheme(ref scheme, ref prefix);
			InitialiseDefaultResourceGroups();
			InitialiseResourceGroupDirectories(prefix);

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

			SchemeManager.GetSingleton().CreateFromFile("WBMLook2-00.scheme");
			_guiContext.GetCursor().SetDefaultImage("WBMLook/mouse-arrow");
			_guiContext.GetCursor().SetImage(_guiContext.GetCursor().GetDefaultImage());

			_guiContext.SetDefaultFont(FontManager.GetSingleton().Get("small-bold"));
			_guiContext.SetDefaultTooltipType(null);

			var winMgr = WindowManager.GetSingleton();
			_rootWindow = winMgr.CreateWindow("DefaultWindow", "rootWindow");

			_guiContext.SetRootWindow(_rootWindow);

			_rootWindow.AddChild(winMgr.LoadLayoutFromFile("mainMenuView2.layout"));
		}

		private Window _rootWindow;

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
			var mouseState = Mouse.GetState();
			var ks = Keyboard.GetState();

			if (ks.IsKeyDown(Keys.A))
				_position.X += 10;
			if (ks.IsKeyDown(Keys.D))
				_position.X -= 10;
			if (ks.IsKeyDown(Keys.W))
				_position.Y += 10;
			if (ks.IsKeyDown(Keys.S))
				_position.Y -= 10;

			if (_lastMouseState.X != mouseState.X ||
			    _lastMouseState.Y != mouseState.Y)
				_inputAggregator.InjectMousePosition(mouseState.X, mouseState.Y);

			if (mouseState.LeftButton == ButtonState.Pressed &&
			    _lastMouseState.LeftButton == ButtonState.Released)
			{
				_inputAggregator.InjectMouseButtonDown(MouseButton.LeftButton);
			}
			else if (mouseState.LeftButton == ButtonState.Released &&
			         _lastMouseState.LeftButton == ButtonState.Pressed)
			{
				_inputAggregator.InjectMouseButtonUp(MouseButton.LeftButton);
			}
			
			var elapsedSeconds = (float) elapsedGameTime.TotalSeconds;
			_guiContext.InjectTimePulse(elapsedSeconds);
			_system.InjectTimePulse(elapsedSeconds);

			_lastMouseState = mouseState;

			base.Update(elapsedGameTime);
		}

		private MouseState _lastMouseState;

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

			//var vp = GraphicsDevice.Viewport;
			//var projection = Matrix.PerspectiveFovRH(MathUtils.DegreesToRadians(45f), 
			//                                         (float)vp.Width / vp.Height, 
			//                                         0.1f, 
			//                                         100.0f);
			//var view = Matrix.LookAtRH(
			//	new Vector3(4, 3, 3), // Camera is at (4,3,3), in World Space
			//	new Vector3(0, 0, 0), // and looks at the origin
			//	new Vector3(0, 1, 0) // Head is up (set to 0,-1,0 to look upside-down)
			//);

			//var model = Matrix.Identity;
			//var mvp = model * view * projection;
			//var spritePrj = Matrix.OrthoOffCenterRH(0f, vp.Width, vp.Height, 0f, 0f, 1f);
			

			//GraphicsDevice.VertexShader = _vertexShader;
			//GraphicsDevice.PixelShader = _pixelShader;

			
			//_vertexShader.SetMatrix4("MVP", ref spritePrj);
			//_spriteBatch.Begin();
			//_spriteBatch.Draw(_mapTexture,
			//				  _position, 
			//				  null,
			//				  Color.White,
			//				  0,
			//				  Vector2.Zero, //new Vector2(0.5f, 0.5f),
			//				  Vector2.One, //new Vector2(0.0004f, 0.0005f),
			//				  SpriteEffects.None,
			//				  0);
			//_spriteBatch.End();

			//_vertexShader.SetMatrix4("MVP", ref mvp);
			//GraphicsDevice.SetVertexBuffer(_vertexBuffer);
			//GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
			////GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

			_renderer.BeginRendering();
			_guiContext.Draw();
			_renderer.EndRendering();
			
			base.Draw(elapsedGameTime);
		}

		protected override void EndDraw()
		{
			base.EndDraw();

			WindowManager.GetSingleton().CleanDeadPool();
		}

		//void InitializeScheme(ref string scheme, ref string prefix)
		//{
		//	var schemeConfigSection = ConfigurationManager.GetSection("schemeSection") as SchemeConfigurationSection;
		//	if (schemeConfigSection != null)
		//	{
		//		var widthRatio = _graphicsDeviceManager.PreferredBackBufferWidth / (float)_minDisplayMode.Width;
		//		var heightRatio = _graphicsDeviceManager.PreferredBackBufferHeight / (float)_minDisplayMode.Height;
		//		var ratio = Math.Min(widthRatio, heightRatio);

		//		scheme = schemeConfigSection.DefaultScheme;
		//		foreach (var schemeElement in schemeConfigSection.Schemes.Cast<SchemeElement>().OrderBy(x => x.Point))
		//		{
		//			if (schemeElement.Point <= ratio)
		//			{
		//				scheme = schemeElement.Scheme;
		//				prefix = schemeElement.Prefix;
		//				UIConfiguration.Ratio = schemeElement.Point;
		//			}
		//		}
		//	}
		//	UIConfiguration.RatioPrefix = prefix.Replace("-", "");
		//}

		private static void InitialiseDefaultResourceGroups()
		{
			//// set the default resource groups to be used
			ImageManager.SetImagesetDefaultResourceGroup("imagesets");
			Font.SetDefaultResourceGroup("fonts");
			Scheme.SetDefaultResourceGroup("schemes");
			WidgetLookManager.SetDefaultResourceGroup("looknfeels");
			WindowManager.SetDefaultResourceGroup("layouts");
			//CEGUI::ScriptModule::setDefaultResourceGroup("lua_scripts");
			AnimationManager.SetDefaultResourceGroup("animations");

			//// setup default group for validation schemas
			//CEGUI::XMLParser* parser = CEGUI::System::getSingleton().getXMLParser();
			//if (parser->isPropertyPresent("SchemaDefaultResourceGroup"))
			//    parser->setProperty("SchemaDefaultResourceGroup", "schemas");
		}

		private static void InitialiseResourceGroupDirectories(string ratioPrefix)
		{
			var resourceProvider = (DefaultResourceProvider)SharpCEGui.Base.System.GetSingleton().GetResourceProvider();
			var dataPathPrefix = GetDataPathPrefix();

			// for each resource type, set a resource group directory
			var imagesetsPath = Path.Combine(dataPathPrefix, "imagesets");
			resourceProvider.SetResourceGroupDirectory("schemes", Path.Combine(dataPathPrefix, "schemes"));
			resourceProvider.SetResourceGroupDirectory("imagesets", imagesetsPath);
			resourceProvider.SetResourceGroupDirectory("fonts", Path.Combine(dataPathPrefix, ratioPrefix, "fonts"));
			resourceProvider.SetResourceGroupDirectory("layouts", Path.Combine(dataPathPrefix, ratioPrefix, "layouts"));
			resourceProvider.SetResourceGroupDirectory("looknfeels", Path.Combine(dataPathPrefix, ratioPrefix, "looknfeel"));
			resourceProvider.SetResourceGroupDirectory("lua_scripts", Path.Combine(dataPathPrefix, "lua_scripts"));
			resourceProvider.SetResourceGroupDirectory("schemas", Path.Combine(dataPathPrefix, "xml_schemas"));
			resourceProvider.SetResourceGroupDirectory("animations", Path.Combine(dataPathPrefix, "animations"));

			resourceProvider.SetResourceGroupDirectory("teams", Path.Combine(Path.Combine(dataPathPrefix, "imagesets"), "teams"));
			resourceProvider.SetResourceGroupDirectory("players", Path.Combine(Path.Combine(dataPathPrefix, "imagesets"), "players"));
			resourceProvider.SetResourceGroupDirectory("managers", Path.Combine(Path.Combine(dataPathPrefix, "imagesets"), "managers"));
			resourceProvider.SetResourceGroupDirectory("maps", Path.Combine(Path.Combine(dataPathPrefix, "imagesets"), "maps"));
			resourceProvider.SetResourceGroupDirectory("sponsors", Path.Combine(Path.Combine(dataPathPrefix, "imagesets"), "sponsors"));
		}

		private static string GetDataPathPrefix()
		{
			var path = GetResourcesDirectory();
			if (Directory.Exists(path))
				return path;

			throw new System.Exception($"'{path}' not exits.");
		}

		public static string GetResourcesDirectory()
		{
			return Path.Combine(Environment.GetEnvironmentVariable("WBMX_PATH"), "resources");
#if WINDOWS
            return Path.Combine(Environment.CurrentDirectory, "resources");
#elif __MACOS__
            return Foundation.NSBundle.MainBundle.ResourcePath;
#endif
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