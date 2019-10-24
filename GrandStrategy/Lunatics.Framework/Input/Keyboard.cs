using System.Collections.Generic;

namespace Lunatics.Framework.Input
{
	/// <summary>
	/// Allows getting keystrokes from keyboard.
	/// </summary>
	public static class Keyboard
	{
		/// <summary>
		/// Returns the current keyboard state.
		/// </summary>
		/// <returns>Current keyboard state.</returns>
		public static KeyboardState GetState()
		{
			return new KeyboardState(_keys);
		}
		
		//public static Keys GetKeyFromScancodeEXT(Keys scancode)
		//{
		//	return FNAPlatform.GetKeyFromScancode(scancode);
		//}

		
		#region Internal Static Methods

		internal static void SetKeys(List<Keys> newKeys)
		{
			_keys = newKeys;
		}

		#endregion

		private static List<Keys> _keys;
	}
}