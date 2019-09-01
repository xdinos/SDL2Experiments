using System;
using Artemis;
using Artemis.System;
using Artemis.Attributes;
using Artemis.Manager;
using SDL2;
using SDLGame.Component;
using SDLGame.Spatials;

namespace SDLGame.Systems
{
	/// <summary>The render system.</summary>
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
	public class RenderSystem : EntityComponentProcessingSystem<SpatialFormComponent, TransformComponent>
	{
		private Game _game;
		private IntPtr _rendererPtr;
		

		/// <summary>Override to implement code that gets executed when systems are initialized.</summary>
		public override void LoadContent()
		{
			_game = BlackBoard.GetEntry<Game>("Game");
			_rendererPtr = BlackBoard.GetEntry<IntPtr>("rendererPtr");
		}

		public override void Process(Entity entity, SpatialFormComponent spatialFormComponent, TransformComponent transformComponent)
		{
			if (spatialFormComponent != null)
			{
				var spatialName = spatialFormComponent.SpatialFormFile;

				if (transformComponent.X >= 0 && 
				    transformComponent.Y >= 0 &&
					transformComponent.X < _game._displayMode.w && 
					transformComponent.Y < _game._displayMode.h)
				{
					// very naive render ...
					if (string.Compare("PlayerShip", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						PlayerShip.Render(_rendererPtr, transformComponent);
					}
					else if (string.Compare("Missile", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						Missile.Render(_rendererPtr, transformComponent);
					}
					else if (string.Compare("EnemyShip", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						EnemyShip.Render(_rendererPtr, transformComponent);
					}
					else if (string.Compare("BulletExplosion", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						Explosion.Render(_rendererPtr, transformComponent,10);
					}
					else if (string.Compare("ShipExplosion", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						//ShipExplosion.Render(this.spriteBatch, this.contentManager, transformComponent, Color.Yellow, 30);
					}
				}
			}
		}

	}
}