using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using Artemis;
using Artemis.System;
using SDL2;
using SDLGame.Component;
using SDLGame.Templates;

namespace SDLGame
{
	public class Game : Lunatics.Game
	{
		public Game() : base()
		{
			elapsedTime = TimeSpan.Zero;
		}

		public SDL.SDL_DisplayMode _displayMode;

		protected override void LoadResources()
		{
			if (Renderer == null)
				return;

			//_texturePtr = TextureManager.LoadTexture(_rendererPtr, "assets/tilemap_packed.png");

			//_player = new GameObject(_rendererPtr, _texturePtr, 408, 0, 16, 16);

			//_tileMap = TileMap.Load(_rendererPtr, "assets", "map.json");

			SDL.SDL_GetWindowDisplayMode(WindowHandle, out _displayMode);

			_font = BMFont.Load(Renderer.Handle, "assets", "roboto-regular-16pt.fnt");

			_entityWorld = new EntityWorld();

			EntitySystem.BlackBoard.SetEntry("rendererPtr", Renderer.Handle);
			EntitySystem.BlackBoard.SetEntry("EnemyInterval", 500);
			EntitySystem.BlackBoard.SetEntry("Game", this);

			_entityWorld.InitializeAll(true);
			
			InitializePlayerShip(_displayMode.w, _displayMode.h);
			InitializeEnemyShips(_displayMode.w, _displayMode.h);
		}
		
		protected override void Update(TimeSpan elapsedGameTime)
		{
			//_tileMap.Update();
			//_player.Update();

			_entityWorld.Update(elapsedGameTime.Ticks);

			++frameCounter;
			elapsedTime += elapsedGameTime;
			if (elapsedTime > OneSecond)
			{
				elapsedTime -= OneSecond;
				frameRate = frameCounter;
				frameCounter = 0;
			}
		}

		protected override void Draw()
		{
			var fps = $"fps: {frameRate}";

			_entityWorld.Draw();
			
			_font.DrawText(Renderer.Handle, fps, 0, 0, 0x00, 0x00, 0x00);
		}

		protected override void Shutdown()
		{
			base.Shutdown();
		}

		private void InitializeEnemyShips(int width, int height)
		{
			Random random = new Random();
			for (var index = 0; 2 > index; ++index)
			{
				var entity = _entityWorld.CreateEntityFromTemplate(EnemyShipTemplate.Name);
				entity.GetComponent<TransformComponent>().X = random.Next(width - 100) + 50;
				entity.GetComponent<TransformComponent>().Y = random.Next((int)((height * 0.75) + 0.5)) + 50;
				entity.GetComponent<VelocityComponent>().Speed = 0.05f;
				entity.GetComponent<VelocityComponent>().Angle = random.Next() % 2 == 0 ? 0 : 180;
			}
		}

		private void InitializePlayerShip(int width, int height)
		{
			var entity = _entityWorld.CreateEntity();
			entity.Group = "SHIPS";

			entity.AddComponentFromPool<TransformComponent>();
			entity.AddComponent(new SpatialFormComponent("PlayerShip"));
			entity.AddComponent(new HealthComponent(30));

			entity.GetComponent<TransformComponent>().X = width * 0.5f;
			entity.GetComponent<TransformComponent>().Y = height - 50;
			entity.Tag = "PLAYER";
		}

		public BMFont _font;

		private int frameRate;
		private int frameCounter;
		private TimeSpan elapsedTime;
		private EntityWorld _entityWorld;
		private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
	}
}