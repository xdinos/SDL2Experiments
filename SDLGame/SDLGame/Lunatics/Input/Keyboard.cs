using System.Collections.Generic;
using SDL2;

namespace Lunatics.Input
{
	public static class Keyboard
	{
		public static bool IsKeyDown(SDL.SDL_Keycode key)
		{
			return _keys.Contains(key);
		}

		internal static void SetKeys(List<SDL.SDL_Keycode> keys)
		{
			_keys = keys;
		}

		static List<SDL.SDL_Keycode> _keys;
	}
}