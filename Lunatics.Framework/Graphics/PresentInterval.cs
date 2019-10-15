namespace Lunatics.Framework.Graphics
{
	public enum PresentInterval
	{
		/// <summary>
		/// Equivalent to <see cref="PresentInterval.One"/>.
		/// </summary>
		Default = 0,
		/// <summary>
		/// The driver waits for the vertical retrace period, before updating window client area. Present operations are not affected more frequently than the screen refresh rate.
		/// </summary>
		One = 1,
		/// <summary>
		/// The driver waits for the vertical retrace period, before updating window client area. Present operations are not affected more frequently than every second screen refresh.
		/// </summary>
		Two = 2,
		/// <summary>
		/// The driver updates the window client area immediately. Present operations might be affected immediately. There is no limit for framerate.
		/// </summary>
		Immediate = 3,
	}
}