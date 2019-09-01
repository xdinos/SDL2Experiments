using System;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using SDLGame.Component;

namespace SDLGame.Systems
{
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 1)]
	public class MovementSystem : EntityComponentProcessingSystem<TransformComponent, VelocityComponent>
	{
		/// <summary>Processes the specified entity.</summary>
		/// <param name="entity">The entity.</param>
		public override void Process(Entity entity,
		                             TransformComponent transformComponent,
		                             VelocityComponent velocityComponent)
		{
			if (velocityComponent != null && transformComponent != null)
			{
				var ms = (float) TimeSpan.FromTicks(EntityWorld.Delta).TotalMilliseconds;

				transformComponent.X += (float) (System.Math.Cos(velocityComponent.AngleAsRadians) * velocityComponent.Speed * ms);
				transformComponent.Y += (float) (System.Math.Sin(velocityComponent.AngleAsRadians) * velocityComponent.Speed * ms);
			}

		}
	}
}