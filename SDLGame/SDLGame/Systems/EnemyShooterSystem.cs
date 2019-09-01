using System;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Artemis.Utils;
using SDLGame.Component;
using SDLGame.Templates;

namespace SDLGame.Systems
{
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 1)]
	public class EnemyShooterSystem : EntityComponentProcessingSystem<TransformComponent, WeaponComponent, EnemyComponent>
	{
		/// <summary>The two seconds ticks.</summary>
		private static readonly long TwoSecondsTicks = TimeSpan.FromSeconds(2).Ticks;

		/// <summary>Override to implement code that gets executed when systems are initialized.</summary>

		/// <summary>Processes the specified entity.</summary>
		/// <param name="entity">The entity.</param>
		public override void Process(Entity entity, TransformComponent transformComponent, WeaponComponent weaponComponent, EnemyComponent enemyComponent)
		{
			if (weaponComponent != null)
			{
				if ((weaponComponent.ShotAt + TwoSecondsTicks) < FastDateTime.Now.Ticks)
				{
					Entity missile = this.EntityWorld.CreateEntityFromTemplate(MissileTemplate.Name);

					missile.GetComponent<TransformComponent>().X = transformComponent.X;
					missile.GetComponent<TransformComponent>().Y = transformComponent.Y + 20;

					missile.GetComponent<VelocityComponent>().Speed = -0.5f;
					missile.GetComponent<VelocityComponent>().Angle = 270;
					weaponComponent.ShotAt = FastDateTime.Now.Ticks;
				}
			}
		}
	}
}