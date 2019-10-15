using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;
using Lunatics.Input;
using Newtonsoft.Json.Linq;

namespace SDLGame.AdvEn
{
	public class Polygon
	{
		public IReadOnlyList<Vector2> Points { get; }
		public Polygon(IEnumerable<Vector2> points)
		{
			Points = points.ToList();
		}
	}

	public class WalkBox
	{
		public string Name { get; }
		public Polygon Polygon { get; }

		public WalkBox(string name, IEnumerable<Vector2> points)
		{
			Name = name;
			Polygon = new Polygon(points);
		}

	}

	public class Room
	{
		public string Name { get; }
		public int Height { get; }
		public int FullScreen { get; }

		public string Sheet { get; }

		public Vector2 Size { get; }

		public Room(Engine engine, string name)
		{
			var jWimpy = JObject.Parse(File.ReadAllText(Path.Combine(engine.RootPath, $"{name}.wimpy")));

			if (jWimpy.ContainsKey("name"))
				Name = jWimpy["name"].ToString();

			if (jWimpy.ContainsKey("fullscreen"))
				FullScreen = int.Parse(jWimpy["fullscreen"].ToString());

			if (jWimpy.ContainsKey("height"))
				Height = int.Parse(jWimpy["height"].ToString());

			var posArray = jWimpy["roomsize"]
			               .ToObject<string>()
			               .Replace("{", "")
			               .Replace("}", "")
			               .Split(',');
			Size = new Vector2(int.Parse(posArray[0]), int.Parse(posArray[1]));

			if (!jWimpy.ContainsKey("sheet"))
				throw new InvalidOperationException("sheet not found.");

			Sheet = jWimpy["sheet"].ToString();
			_spriteSheet = engine.LoadSheet(Sheet);

			if (jWimpy.ContainsKey("background"))
			{
				var layer = new RoomLayer
				            {
					            ZSort = 0,
					            Parallax = Vector2.One
				            };
				var jBackground = jWimpy["background"];
				if (jBackground is JArray jArray)
				{

					foreach (JValue background in jArray)
					{
						layer.AddSprite(_spriteSheet.GetSprite(background.ToObject<string>()));
					}
				}
				else if (jBackground is JValue)
				{
					layer.AddSprite(_spriteSheet.GetSprite(jBackground.ToObject<string>()));
				}

				_layers.Add(0, new List<RoomLayer> {layer});
			}

			if (jWimpy.ContainsKey("layers"))
			{
				var jLayers = jWimpy["layers"] as JArray;
				foreach (var jLayer in jLayers)
				{
					var zsort = jLayer["zsort"].ToObject<int>();
					if (!_layers.ContainsKey(zsort))
						_layers.Add(zsort, new List<RoomLayer>());
					var items = _layers[zsort];

					var parallax = Vector2.One;
					if (jLayer["parallax"].Type == JTokenType.String)
					{
						var array = jLayer["parallax"].ToObject<string>()
						                              .Replace("{", "")
						                              .Replace("}", "")
						                              .Split(',');
						parallax = new Vector2(float.Parse(array[0]), float.Parse(array[1]));
					}
					else
					{
						parallax = new Vector2(jLayer["parallax"].ToObject<float>(), 1f);
					}

					var layer = new RoomLayer { ZSort = zsort, Parallax = parallax,};
					items.Add(layer);

					if (jLayer["name"] is JArray)
					{
						foreach (var sprite in (JArray)jLayer["name"])
						{
							layer.AddSprite(_spriteSheet.GetSprite(sprite.ToObject<string>()));
						}
					}
					else
					{
						layer.AddSprite(_spriteSheet.GetSprite(jLayer["name"].ToObject<string>()));
					}
				}
			}

			if (jWimpy.ContainsKey("objects"))
			{
				var jObjects = jWimpy["objects"] as JArray;
				foreach (JObject jObject in jObjects)
				{
					var pos = jObject["pos"]
					          .ToObject<string>()
					          .Replace("{", "")
					          .Replace("}", "")
					          .Split(',');
					var usePos = jObject["usepos"]
					          .ToObject<string>()
					          .Replace("{", "")
					          .Replace("}", "")
					          .Split(',');
					var hotspot = jObject["hotspot"]
					              .ToObject<string>()
					              .Replace("{", "")
					              .Replace("}", "")
					              .Split(',');

					var left = int.Parse(hotspot[0]);
					var top = int.Parse(hotspot[1]);
					var right = int.Parse(hotspot[2]);
					var bottom = int.Parse(hotspot[3]);

					var obj = new Object
					          {
						          Name = jObject["name"].ToObject<string>(),
						          Position = new Vector2(float.Parse(pos[0]), Size.Y- float.Parse(pos[1])),
						          UsePosition = new Vector2(float.Parse(usePos[0]), float.Parse(usePos[1])),
						          //return sf::IntRect(left, -top, right - left, -bottom + top);
						          //HotSpot = new Rectangle(int.Parse(hotspot[0]),
						          //                        -int.Parse(hotspot[1]),
						          //                        int.Parse(hotspot[2]) - int.Parse(hotspot[0]),
						          //                        -int.Parse(hotspot[3]) + int.Parse(hotspot[1])),
						          HotSpot = new Rectangle(left,-top,right- left,-bottom + top),
						          ZSort = jObject["zsort"].ToObject<int>(),
						          Prop = jObject.ContainsKey("prop") && jObject["prop"].ToObject<int>() == 1,
						          Spot = jObject.ContainsKey("spot") && jObject["spot"].ToObject<int>() == 1,
						          Trigger = jObject.ContainsKey("trigger") && jObject["trigger"].ToObject<int>() == 1,
					};

					var animations = jObject["animations"] as JArray;
					if (animations != null)
					{
						var animation = animations[0];
						var frames = animation["frames"] as JArray;
						var frame = frames.Count > 0
							            ? frames[0].ToObject<string>()
							            : string.Empty;
						if (!string.IsNullOrWhiteSpace(frame))
							obj.Sprite = _spriteSheet.GetSprite(frame);
					}
					

					_objects.Add(obj);
				}
			}

			if (jWimpy.ContainsKey("walkboxes"))
			{
				foreach (var walkBox in ((JArray)jWimpy["walkboxes"]))
				{
					var walkBoxName = walkBox.Contains("name")
						                  ? walkBox["name"].ToObject<string>()
						                  : string.Empty;
					var points = walkBox["polygon"]
					             .ToObject<string>()
					             .Split(';')
					             .Select(x =>
					                     {
						                     var coords = x.Replace("{", "")
						                                   .Replace("}", "")
						                                   .Split(',');
						                     return new Vector2(int.Parse(coords[0]), Size.Y - int.Parse(coords[1]));
					                     })
					             .ToList();
					points.Add(points[0]);
					_walkBoxes.Add(new WalkBox(walkBoxName, points));
				}
			}
		}
		
		public void Update(TimeSpan elapsedGameTime)
		{

		}

		public void Render(Renderer renderer, Size screenSize, Vector2 cameraPos, BMFont font)
		{
			var scale = 1f * Vector2.One;

			var hss = new Size((int) (screenSize.Width / 2f),
			                   (int) (screenSize.Height / 2f));

			//var key = 0;
			foreach (var key in _layers.Keys.OrderByDescending(x=>x))
			{
				var items = _layers[key];

				foreach (var layerItem in items)
				{
					var parallax = layerItem.Parallax;
					var pos = new Vector2(
						(hss.Width - cameraPos.X) * parallax.X - hss.Width,
						(hss.Height - cameraPos.Y) * parallax.Y - hss.Height);

					var p = new Vector2(
						hss.Width + pos.X + (hss.Width - hss.Width * parallax.X),
						hss.Height + pos.Y + (hss.Height - hss.Height * parallax.Y));

					layerItem.Draw(renderer, p, scale);
				}

				//foreach (var layer in items)
				//{
				//	var parallax = layer.Parallax;
				//	var pos = new Vector2(
				//		(hss.Width - cameraPos.X) * parallax.X - hss.Width,
				//		(hss.Height - cameraPos.Y) * parallax.Y - hss.Height);

				//	var p = new Vector2(
				//		hss.Width+pos.X + (hss.Width - hss.Width * parallax.X),
				//		hss.Height+ pos.Y + (hss.Height - hss.Height * parallax.Y));


				//	layer.Sprite.Draw(renderer, p, scale);

				//	//renderer.Draw(layer.Sprite.Texture,
				//	//              layer.Sprite.Offset, 
				//	//              Vector2.Zero, 0, scale);
				//}
			}

			var textY = Size.Y* scale.Y + 40f;
			//foreach (var obj in _objects.OrderBy(x => x.ZSort))
			//foreach (var obj in _objects.Skip(0).Take(1))
			foreach (var obj in _objects.OrderByDescending(x => x.ZSort))
			//foreach (var obj in _objects.Where(x=>x.Name== "chucksTombButton"))
			{
				if (obj.Sprite != null)
				{
					obj.Sprite.Draw(renderer,
					                new Vector2(hss.Width - cameraPos.X + obj.Position.X * scale.X,
					                            hss.Height - cameraPos.Y + obj.Position.Y * scale.Y),
					                scale);
					//var pos = new Vector2(obj.Position + obj.Sprite.Offset, pos.Y);
					////var pos = new Vector2(obj.Position.X, Size.Y - obj.Position.Y);
					////var pos = obj.Position + obj.Sprite.Offset;
					//renderer.Draw(obj.Sprite.Texture,
					//			  pos,
					//			  Vector2.Zero, 0, scale);
				}
				
				var x = obj.Position.X + obj.HotSpot.X;
				var y = obj.Position.Y + obj.HotSpot.Y;

				if (!obj.Prop && !obj.Spot && !obj.Trigger)
				{
					renderer.SetDrawColor(0xff, 0xff, 0xff, 0xff);
					renderer.DrawRect(x, y,
					                  obj.HotSpot.Width,
					                  obj.HotSpot.Height,
					                  scale);
				}

				if (!obj.Prop && !obj.Spot && !obj.Trigger &&
					(Mouse.X >= x*scale.X && Mouse.X <= (x + obj.HotSpot.Width) * scale.X ||
				     Mouse.X >= (x + obj.HotSpot.Width) * scale.X &&Mouse.X <= x * scale.X) &&
				    (Mouse.Y >= y*scale.Y && Mouse.Y <= (y + obj.HotSpot.Height) * scale.Y ||
					Mouse.Y >= (y + obj.HotSpot.Height) * scale.Y) && Mouse.Y <= y * scale.Y)
				{
					renderer.SetDrawColor(0x00, 0xff, 0x00, 0xff);
					renderer.DrawRect(x, y,
					                  obj.HotSpot.Width,
					                  obj.HotSpot.Height,
					                  scale);
					renderer.SetDrawColor(0xff, 0xff, 0xff, 0xff);
					renderer.DrawRect(obj.Position.X - 1, obj.Position.Y - 1, 2, 2, scale);
					
					font.DrawText(renderer, obj.Name, 0, textY, 0xff, 0xff, 0xff);

					renderer.SetDrawColor(0x00, 0xff, 0x00, 0xff);

					var usePos = new Vector2(obj.Position.X + obj.UsePosition.X,
					                         obj.Position.Y - obj.UsePosition.Y);
					renderer.DrawRect(usePos.X - 1, usePos.Y - 1, 2, 2, scale);

					renderer.DrawLine(obj.Position, usePos, scale);

					textY += 20f;
				}
			}

			renderer.SetDrawColor(0x00, 0xff, 0x00, 0xff);
			foreach (var walkBox in _walkBoxes)
			{
				var pts = walkBox.Polygon.Points
				                 .Select(p => new Vector2(hss.Width - cameraPos.X + p.X * scale.X,
				                                          hss.Height - cameraPos.Y + p.Y * scale.Y))
				                 .ToList();
				renderer.DrawLines(pts, Vector2.One);
			}
			
			font.DrawText(renderer, $"({Mouse.X}, {Mouse.Y})", 0, Size.Y * scale.Y + 20f , 0xff, 0xff, 0xff);

			renderer.SetDrawColor(0xff, 0x00, 0x00, 0xff);
			renderer.DrawRect(Size.X/2-1, Size.Y / 2 - 1, 2, 2, scale);
		}

		private static readonly Vector2 Half = new Vector2(0.5f, 0.5f);
		private readonly SpriteSheet _spriteSheet;
		private readonly List<WalkBox> _walkBoxes = new List<WalkBox>();
		private readonly List<Object> _objects = new List<Object>();
		private readonly Dictionary<int, List<RoomLayer>> _layers = new Dictionary<int, List<RoomLayer>>();
		private readonly Dictionary<string, TextureRegion2D> _backgrounds = new Dictionary<string, TextureRegion2D>();
	}
}