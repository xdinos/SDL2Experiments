using System;
using Lunatics.Framework.Graphics;

namespace Lunatics.Framework.Input
{
	/// <summary>
	/// Allows reading position and button click information from mouse.
	/// </summary>
	public static class Mouse
	{
		public static IntPtr WindowHandle { get; set;}

		//public static bool IsRelativeMouseModeEXT
		//{
		//	get
		//	{
		//		return FNAPlatform.GetRelativeMouseMode();
		//	}
		//	set
		//	{
		//		FNAPlatform.SetRelativeMouseMode(value);
		//	}
		//}

		internal static int INTERNAL_WindowWidth = PresentationParameters.DefaultBackBufferWidth;
		internal static int INTERNAL_WindowHeight = PresentationParameters.DefaultBackBufferHeight;
		internal static int INTERNAL_BackBufferWidth = PresentationParameters.DefaultBackBufferWidth;
		internal static int INTERNAL_BackBufferHeight = PresentationParameters.DefaultBackBufferHeight;

		#region Public Events

		public static Action<int> ClickedEXT;

		#endregion

		#region Public Interface

		/// <summary>
		/// Gets mouse state information that includes position and button
		/// presses for the provided window
		/// </summary>
		/// <returns>Current state of the mouse.</returns>
		public static MouseState GetState()
		{
			return _platform.GetMouseState(WindowHandle);

			//_platform.GetMouseState(WindowHandle,
			//                        out var x,
			//                        out var y,
			//                        out var left,
			//                        out var middle,
			//                        out var right);

			//// Scale the mouse coordinates for the faux-backbuffer
			//x = (int)((double)x * INTERNAL_BackBufferWidth / INTERNAL_WindowWidth);
			//y = (int)((double)y * INTERNAL_BackBufferHeight / INTERNAL_WindowHeight);

			//return new MouseState(x,
			//                      y,
			//                      ScrollY,
			//                      left,
			//                      middle,
			//                      right);
		}

		/// <summary>
		/// Sets mouse cursor's relative position to game-window.
		/// </summary>
		/// <param name="x">Relative horizontal position of the cursor.</param>
		/// <param name="y">Relative vertical position of the cursor.</param>
		public static void SetPosition(int x, int y)
		{
			//// In relative mode, this function is meaningless
			//if (IsRelativeMouseModeEXT)
			//{
			//	return;
			//}

			//// Scale the mouse coordinates for the faux-backbuffer
			//x = (int)((double)x * INTERNAL_WindowWidth / INTERNAL_BackBufferWidth);
			//y = (int)((double)y * INTERNAL_WindowHeight / INTERNAL_BackBufferHeight);

			//FNAPlatform.SetMousePosition(WindowHandle, x, y);
		}

		#endregion

		#region Internal Methods

		internal static void INTERNAL_onClicked(int button)
		{
			if (ClickedEXT != null)
			{
				ClickedEXT(button);
			}
		}

		#endregion

		internal static void Initialize(GamePlatform platform)
		{
			if (_platform != null) 
				return;

			_platform = platform;
		}

		private static GamePlatform _platform;
	}
}