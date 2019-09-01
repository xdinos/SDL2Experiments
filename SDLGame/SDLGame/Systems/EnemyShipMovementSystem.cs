using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using SDLGame.Component;

namespace SDLGame.Systems
{
	
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
	internal class EnemyShipMovementSystem : EntityComponentProcessingSystem<TransformComponent, VelocityComponent, EnemyComponent>
	{
		private Game _game;

		/// <summary>Override to implement code that gets executed when systems are initialized.</summary>
		public override void LoadContent()
		{
			_game = BlackBoard.GetEntry<Game>("Game");
		}

		/// <summary>Processes the specified entity.</summary>
		/// <param name="entity">The entity.</param>
		public override void Process(Entity entity, TransformComponent transformComponent, VelocityComponent velocityComponent, EnemyComponent enemyComponent)
		{
			if (transformComponent != null && 
			    (transformComponent.X < 0 || 
			     transformComponent.X > _game._displayMode.w))
			{
				velocityComponent.AddAngle(180);
			}
		}
	}
}