namespace Lunatics.Framework.Input
{
	/// <summary>
	/// Represents a mouse state with cursor position and button press information.
	/// </summary>
	public struct MouseState
	{
		/// <summary>
		/// Gets horizontal position of the cursor.
		/// </summary>
		public int X { get; internal set; }

		/// <summary>
		/// Gets vertical position of the cursor.
		/// </summary>
		public int Y { get; internal set; }

		/// <summary>
		/// Gets state of the left mouse button.
		/// </summary>
		public ButtonState LeftButton { get; internal set; }

		/// <summary>
		/// Gets state of the middle mouse button.
		/// </summary>
		public ButtonState MiddleButton { get; internal set; }

		/// <summary>
		/// Gets state of the right mouse button.
		/// </summary>
		public ButtonState RightButton { get; internal set; }

		public int HorizontalScrollWheelValue { get; internal set; }

		public int VerticalScrollWheelValue { get; internal set; }
		
		public MouseState(int x,
		                  int y,
		                  int scrollX,
		                  int scrollY,
		                  ButtonState leftButton,
		                  ButtonState middleButton,
		                  ButtonState rightButton)
			: this()
		{
			X = x;
			Y = y;
			HorizontalScrollWheelValue = scrollX;
			VerticalScrollWheelValue = scrollY;
			LeftButton = leftButton;
			MiddleButton = middleButton;
			RightButton = rightButton;
		}

		/// <summary>
		/// Compares whether two MouseState instances are equal.
		/// </summary>
		/// <param name="left">MouseState instance on the left of the equal sign.</param>
		/// <param name="right">MouseState instance on the right of the equal sign.</param>
		/// <returns>true if the instances are equal; false otherwise.</returns>
		public static bool operator ==(MouseState left, MouseState right)
		{
			return (left.X == right.X &&
			        left.Y == right.Y &&
			        left.LeftButton == right.LeftButton &&
			        left.MiddleButton == right.MiddleButton &&
			        left.RightButton == right.RightButton &&
			        left.VerticalScrollWheelValue == right.VerticalScrollWheelValue &&
			        left.HorizontalScrollWheelValue == right.HorizontalScrollWheelValue);
		}

		/// <summary>
		/// Compares whether two MouseState instances are not equal.
		/// </summary>
		/// <param name="left">MouseState instance on the left of the equal sign.</param>
		/// <param name="right">MouseState instance on the right of the equal sign.</param>
		/// <returns>true if the objects are not equal; false otherwise.</returns>
		public static bool operator !=(MouseState left, MouseState right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Compares whether current instance is equal to specified object.
		/// </summary>
		/// <param name="obj">The MouseState to compare.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return (obj is MouseState) && (this == (MouseState) obj);
		}

		/// <summary>
		/// Gets the hash code for MouseState instance.
		/// </summary>
		/// <returns>Hash code of the object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}