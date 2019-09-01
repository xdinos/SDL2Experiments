using Artemis.Interface;

namespace SDLGame.Component
{
	/// <summary>The health.</summary>
	public class HealthComponent : IComponent
	{
		/// <summary>Initializes a new instance of the <see cref="HealthComponent"/> class.</summary>
		public HealthComponent()
			: this(0.0f)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="HealthComponent"/> class.</summary>
		/// <param name="points">The points.</param>
		public HealthComponent(float points)
		{
			Points = MaximumHealth = points;
		}

		/// <summary>Gets or sets the health points.</summary>
		/// <value>The Points.</value>
		public float Points { get; set; }

		/// <summary>Gets the health percentage.</summary>
		/// <value>The health percentage.</value>
		public double HealthPercentage => System.Math.Round(Points / MaximumHealth * 100f);

		/// <summary>Gets a value indicating whether is alive.</summary>
		/// <value><see langword="true" /> if this instance is alive; otherwise, <see langword="false" />.</value>
		public bool IsAlive => Points > 0;

		/// <summary>Gets the maximum health.</summary>
		/// <value>The maximum health.</value>
		public float MaximumHealth { get; }

		/// <summary>The add damage.</summary>
		/// <param name="damage">The damage.</param>
		public void AddDamage(int damage)
		{
			Points -= damage;
			if (Points < 0)
			{
				Points = 0;
			}
		}
	}
}