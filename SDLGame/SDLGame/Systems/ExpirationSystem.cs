using System;
using Artemis;
using Artemis.Attributes;
using Artemis.System;
using SDLGame.Component;

namespace SDLGame.Systems
{
	[ArtemisEntitySystem]
	public class ExpirationSystem : EntityComponentProcessingSystem<ExpiresComponent>
	{
		public override void Process(Entity entity, ExpiresComponent expiresComponent)
		{
			if (expiresComponent == null) return;

			float ms = (float)TimeSpan.FromTicks(EntityWorld.Delta).TotalMilliseconds;
			expiresComponent.ReduceLifeTime(ms);

			if (expiresComponent.IsExpired)
			{
				entity.Delete();
			}
		}
	}
}