using Artemis.Interface;

namespace SDLGame.Component
{
	public class WeaponComponent : IComponent
	{
		public WeaponComponent()
		{
			ShotAt = 0L;
		}

		/// <summary>Gets or sets the shot at.</summary>
		/// <value>The shot at.</value>
		public long ShotAt { get; set; }
	}
}