using System;
using System.IO;
using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;
using Lunatics.Input;
using Newtonsoft.Json.Linq;
using SDL2;

namespace SDLGame.AdvEn
{
	public class Engine
	{
		internal string RootPath { get;  }

		public Engine(Renderer renderer, string rootPath)
		{
			_renderer = renderer;
			RootPath = rootPath;
		}

		public void Load()
		{
			// Bridge Backstage
			// Cemetery ChucksOffice ChucksTomb CircusEntrance 
			// Bigtop BigtopFB
			// BookRoom BookStore
			// Vista MainStreet MansionEntry MansionExterior
			// MansionKitchen MansionWorkshop DeloresRoom
			// BankWF BarWF
			_activeRoom = new Room(this, "MainStreet");
		}

		public void Update(TimeSpan elapsedGameTime)
		{
			if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_LEFT))
				_cameraPos.X += 1;

			if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_RIGHT))
				_cameraPos.X -= 1;

			_activeRoom.Update(elapsedGameTime);
		}

		public void Render(Size screenSize, BMFont font)
		{
			//_renderer.SetViewPort(0, 0, 428, 240);
			
			_activeRoom.Render(_renderer, screenSize, _cameraPos, font);
		}
		
		private Vector2 _cameraPos = Vector2.Zero;

		internal SpriteSheet LoadSheet(string name)
		{
			var jSheet = JObject.Parse(File.ReadAllText(Path.Combine(RootPath, $"{name}.json")));

			if (!jSheet.ContainsKey("meta"))
				throw new InvalidOperationException("Property 'meta' not found.");

			var jMeta = jSheet["meta"] as JObject;
			if (jMeta == null)
				throw new InvalidOperationException("Property 'meta' not valid.");

			var texture = Texture2D.Load(_renderer, Path.Combine(RootPath, jMeta["image"].ToObject<string>()));
			var spriteSheet = new SpriteSheet(name, texture);
			var jFrames = (JObject)jSheet["frames"];
			foreach (var kvp in jFrames)
			{
				var item =(JObject) kvp.Value;
				var frame = (JObject) item["frame"];
				var x = frame["x"].ToObject<int>();
				var y = frame["y"].ToObject<int>();
				var w = frame["w"].ToObject<int>();
				var h = frame["h"].ToObject<int>();
				var size = new Size(item["sourceSize"]["w"].ToObject<int>(),
				                    item["sourceSize"]["h"].ToObject<int>());
				var offset = new Vector2(item["spriteSourceSize"]["x"].ToObject<int>(),
				                         item["spriteSourceSize"]["y"].ToObject<int>());
				var pivot = new Vector2(item["pivot"]["x"].ToObject<float>(),
										item["pivot"]["y"].ToObject<float>());
				spriteSheet.CreateSprite(kvp.Key,
				                         new Rectangle(x, y, w, h),
				                         size,
				                         offset, pivot);
			}

			return spriteSheet;
		}

		private Room _activeRoom;

		
		private readonly Renderer _renderer;
	}
}