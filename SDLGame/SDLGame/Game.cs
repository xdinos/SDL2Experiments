using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using Artemis;
using Artemis.System;
using Lunatics.Framework.Math;
using Lunatics.Graphics;
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
		private AnimatedSprite _motwSprite;

		protected override void LoadResources()
		{
			if (Renderer == null)
				return;

			var motwTexture = Texture2D.Load(Renderer.Handle, "assets/motw.png");
			var motwAtlas = TextureAtlas.Create("assets/motw-atlas", motwTexture, 52, 72);
			var motwAnimationFactory = new SpriteSheetAnimationFactory(motwAtlas);
			motwAnimationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
			motwAnimationFactory.Add("walkSouth", new SpriteSheetAnimationData(new[] { 0, 1, 2, 1 }, isLooping: false));
			motwAnimationFactory.Add("walkWest", new SpriteSheetAnimationData(new[] { 12, 13, 14, 13 }, isLooping: false));
			motwAnimationFactory.Add("walkEast", new SpriteSheetAnimationData(new[] { 24, 25, 26, 25 }, isLooping: false));
			motwAnimationFactory.Add("walkNorth", new SpriteSheetAnimationData(new[] { 36, 37, 38, 37 }, isLooping: false));
			_motwSprite = new AnimatedSprite(motwAnimationFactory);// { Position = new Vector2(20, 20) };
			_motwSprite.Play("walkSouth").IsLooping = true;

			SDL.SDL_GetWindowDisplayMode(WindowHandle, out _displayMode);

			_font = BMFont.Load(Renderer.Handle, "assets", "roboto-regular-16pt.fnt");

			_entityWorld = new EntityWorld();

			EntitySystem.BlackBoard.SetEntry("rendererPtr", Renderer.Handle);
			EntitySystem.BlackBoard.SetEntry(nameof(Lunatics.Graphics.Renderer), Renderer);
			EntitySystem.BlackBoard.SetEntry("EnemyInterval", 500);
			EntitySystem.BlackBoard.SetEntry("Game", this);

			_entityWorld.InitializeAll(true);
			
			InitializePlayerShip(_displayMode.w, _displayMode.h);
			InitializeEnemyShips(_displayMode.w, _displayMode.h);

			var entity = _entityWorld.CreateEntity();
			//entity.Group = "SHIPS";

			entity.AddComponentFromPool<TransformComponent>();
			entity.AddComponent(new AnimationComponent(_motwSprite));
			entity.AddComponent(new SpatialFormComponent("Character") {Sprite = _motwSprite});
			
			entity.GetComponent<TransformComponent>().X = 100f;
			entity.GetComponent<TransformComponent>().Y = 100f;
			entity.Tag = "CHARACTER";
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
			entity.AddComponent(new SpatialFormComponent("PlayerShip"){ Sprite = new Sprite(Texture2D.Load(Renderer.Handle, "assets/player.png"))});
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