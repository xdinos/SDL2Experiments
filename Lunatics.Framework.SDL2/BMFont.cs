using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Lunatics.Framework.Mathematics;
using Lunatics.Graphics;
using SDL2;

namespace SDLGame
{
	public class BMFont
	{
		public string Face { get; private set; }
		
		public static BMFont Load(Renderer renderer, string path, string filename)
		{
			var bmFont = new BMFont();
			var xFont = XElement.Load(Path.Combine(path, filename));

			var xInfo = xFont.Element("info");
			bmFont.Face = xInfo.Attribute("face")?.Value;

			var xCommon = xFont.Element("common");
			bmFont._ascender = XmlConvert.ToSingle(xCommon.Attribute("base").Value);
			bmFont._height = XmlConvert.ToSingle(xCommon.Attribute("lineHeight").Value);
			bmFont._descender = bmFont._ascender - bmFont._height;

			var pages = new Dictionary<string, Texture2D>();
			var xPages = xFont.Element("pages");
			foreach (var xPage in xPages.Elements("page"))
			{
				var pageId = xPage.Attribute("id").Value;
				var pageFile = xPage.Attribute("file").Value;

				pages.Add(pageId, Texture2D.Load(renderer, Path.Combine(path, pageFile)));
			}

			var xChars = xFont.Element("chars");
			foreach (var xChar in xChars.Elements("char"))
			{
				var pageId = xChar.Attribute("page").Value;
				var charId = (char)XmlConvert.ToUInt32(xChar.Attribute("id").Value);

				bmFont._glyphs[charId] = new Glyph(XmlConvert.ToSingle(xChar.Attribute("x").Value),
				                                   XmlConvert.ToSingle(xChar.Attribute("y").Value),
				                                   XmlConvert.ToSingle(xChar.Attribute("width").Value),
				                                   XmlConvert.ToSingle(xChar.Attribute("height").Value),
				                                   XmlConvert.ToSingle(xChar.Attribute("xoffset").Value),
				                                   XmlConvert.ToSingle(xChar.Attribute("yoffset").Value)/* - bmFont._ascender*/,
				                                   XmlConvert.ToSingle(xChar.Attribute("xadvance").Value),
				                                   pages[pageId]);


				//if (d_maxCodepoint < charId)
				//	d_maxCodepoint = charId;
			}

			var xKernings = xFont.Element("kernings");
			foreach (var kerning in xKernings.Elements("kerning"))
			{
				var first = (char)XmlConvert.ToInt32(kerning.Attribute("first").Value);
				var second = (char)XmlConvert.ToInt32(kerning.Attribute("second").Value);
				var amount = XmlConvert.ToSingle(kerning.Attribute("amount").Value);
				if (!bmFont._kerningPairs.ContainsKey(first))
					bmFont._kerningPairs.Add(first, new Dictionary<char, float>());
				bmFont._kerningPairs[first].Add(second, amount);
			}

			return bmFont;
		}

		public Vector2 MesureText(string text)
		{
			var lastChar = (char)0;
			var w = 0;
			foreach (var c in text)
			{
				if (_glyphs.TryGetValue(c, out var glyph))
				{

					w += (int)GetKerningAmount(lastChar, c);
					//dst.y = (int)(baseY + glyph.OffsetY)/*- (img.GetRenderedOffset().Y - img.GetRenderedOffset().Y * yScale)*/;
					
					w += (int)glyph.XAdvance;
					// apply extra spacing to space chars
					if (c == ' ')
						w += 2;
				}

				lastChar = c;
			}

			return new Vector2(w, 0);
		}

		public void DrawText(Renderer renderer, string text, float x, float y, byte r, byte g, byte b)
		{
			var baseY = y - _ascender;
			var lastChar = (char)0;
			var src = new SDL.SDL_Rect();
			var dst = new SDL.SDL_Rect { x = (int)x, y = 0 };
			
			foreach (var c in text)
			{
				if (_glyphs.TryGetValue(c, out var glyph))
				{
					src.x = (int)glyph.X;
					src.y = (int)glyph.Y;
					src.w = (int)glyph.Width;
					src.h = (int)glyph.Height;

					dst.x += (int)GetKerningAmount(lastChar, c);
					dst.y = (int)(baseY + glyph.OffsetY)/*- (img.GetRenderedOffset().Y - img.GetRenderedOffset().Y * yScale)*/;
					dst.w = (int)glyph.Width;
					dst.h = (int)glyph.Height;

					SDL.SDL_SetTextureColorMod(glyph.Texture.Handle, r, g, b);
					SDL.SDL_RenderCopy(renderer.Handle, glyph.Texture.Handle, ref src, ref dst);

					dst.x += (int)glyph.XAdvance;
					// apply extra spacing to space chars
					if (c == ' ')
						dst.x += 2;
				}

				lastChar = c;
			}
		}

		private float GetKerningAmount(char first, char second)
		{
			if (_kerningPairs.Count == 0)
				return 0f;

			if (_kerningPairs.ContainsKey(first))
			{
				var pairs = _kerningPairs[first];
				if (pairs.ContainsKey(second))
					return pairs[second];
			}

			return 0f;
		}

		private BMFont()
		{
		}

		private class Glyph
		{
			public float X { get; }
			public float Y { get; }

			public float Width { get; }
			public float Height { get; }

			public float OffsetX { get; }
			public float OffsetY { get; }
			public float XAdvance { get;  }

			public Texture2D Texture { get; }

			public Glyph(float x,
			             float y,
			             float width,
			             float height,
			             float offsetX,
			             float offsetY,
			             float xAdvance,
			             Texture2D texture)
			{
				X = x;
				Y = y;
				Width = width;
				Height = height;
				OffsetX = offsetX;
				OffsetY = offsetY;
				XAdvance = xAdvance;
				Texture = texture;
			}
		}

		private float _ascender;
		private float _height;
		private float _descender;
		
		private readonly Dictionary<char, Glyph> _glyphs = new Dictionary<char, Glyph>();
		private readonly Dictionary<char, Dictionary<char, float>> _kerningPairs = new Dictionary<char, Dictionary<char, float>>();
	}
}