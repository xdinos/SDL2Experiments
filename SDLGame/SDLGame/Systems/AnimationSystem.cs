using System;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using SDLGame.Component;

namespace SDLGame.Systems
{
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 1)]
	public class AnimationSystem : EntityComponentProcessingSystem<AnimationComponent>
	{
		public override void Process(Entity entity, AnimationComponent animationComponent)
		{
			animationComponent?.AnimatedSprite.Update((float)TimeSpan.FromTicks(EntityWorld.Delta).TotalSeconds);
		}
	}
}