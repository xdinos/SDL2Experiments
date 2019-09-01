using Artemis.Interface;

namespace SDLGame.Component
{
	public class ExpiresComponent : IComponent
	{
		/// <summary>Initializes a new instance of the <see cref="ExpiresComponent" /> class.</summary>
		public ExpiresComponent()
			: this(0.0f)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="ExpiresComponent" /> class.</summary>
		/// <param name="lifeTime">The life time.</param>
		public ExpiresComponent(float lifeTime)
		{
			LifeTime = lifeTime;
		}

		/// <summary>Gets a value indicating whether is expired.</summary>
		/// <value><see langword="true" /> if this instance is expired; otherwise, <see langword="false" />.</value>
		public bool IsExpired => LifeTime <= 0;

		/// <summary>Gets or sets the life time.</summary>
		/// <value>The life time.</value>
		public float LifeTime { get; set; }

		/// <summary>The reduce life time.</summary>
		/// <param name="lifeTimeDelta">The life time.</param>
		public void ReduceLifeTime(float lifeTimeDelta)
		{
			LifeTime -= lifeTimeDelta;
			if (LifeTime < 0)
			{
				LifeTime = 0;
			}
		}
	}
}