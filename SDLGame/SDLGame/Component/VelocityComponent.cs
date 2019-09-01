using Artemis.Interface;

namespace SDLGame.Component
{
	/// <summary>The velocity.</summary>
	public class VelocityComponent : IComponent
	{
		/// <summary>To radians.</summary>
		private const float ToRadians = (float)(System.Math.PI / 180.0);

		/// <summary>Initializes a new instance of the <see cref="VelocityComponent" /> class.</summary>
		public VelocityComponent()
			: this(0.0f, 0.0f)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="VelocityComponent" /> class.</summary>
		/// <param name="velocity">The velocity.</param>
		public VelocityComponent(float velocity)
			: this(velocity, 0.0f)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="VelocityComponent" /> class.</summary>
		/// <param name="velocity">The velocity.</param>
		/// <param name="angle">The angle.</param>
		public VelocityComponent(float velocity, float angle)
		{
			this.Speed = velocity;
			this.Angle = angle;
		}

		/// <summary>Gets or sets the angle.</summary>
		/// <value>The angle.</value>
		public float Angle { get; set; }

		/// <summary>Gets the angle as radians.</summary>
		/// <value>The angle as radians.</value>
		public float AngleAsRadians
		{
			get
			{
				return this.Angle * ToRadians;
			}
		}

		/// <summary>Gets or sets the speed.</summary>
		/// <value>The speed.</value>
		public float Speed { get; set; }

		/// <summary>The add angle.</summary>
		/// <param name="angle">The angle.</param>
		public void AddAngle(float angle)
		{
			this.Angle = (this.Angle + angle) % 360;
		}
	}
}