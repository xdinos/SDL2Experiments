using System.Collections.Generic;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Lunatics.Framework.Math;
using SDLGame.Component;
using SDLGame.Templates;

namespace SDLGame.Systems
{
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 1)]
	internal class CollisionSystem : EntitySystem
	{
		/// <summary>Initializes a new instance of the <see cref="CollisionSystem" /> class.</summary>
		public CollisionSystem()
			: base(Aspect.All(typeof(TransformComponent)))
		{
		}

		/// <summary>Processes the entities.</summary>
		/// <param name="entities">The entities.</param>
		protected override void ProcessEntities(IDictionary<int, Entity> entities)
		{
			var bullets = EntityWorld.GroupManager.GetEntities("BULLETS");
			var ships = EntityWorld.GroupManager.GetEntities("SHIPS");
			if (bullets != null && ships != null)
			{
				// being brutal !!!
				for (var shipIndex = 0; ships.Count > shipIndex; ++shipIndex)
				{
					var ship = ships.Get(shipIndex);
					for (var bulletIndex = 0; bullets.Count > bulletIndex; ++bulletIndex)
					{
						var bullet = bullets.Get(bulletIndex);

						if (CollisionExists(bullet, ship))
						{
							var bulletTransform = bullet.GetComponent<TransformComponent>();
							var bulletExplosion = EntityWorld.CreateEntityFromTemplate(BulletExplosionTemplate.Name);
							bulletExplosion.GetComponent<TransformComponent>().Position = bulletTransform.Position;
							bulletExplosion.Refresh();
							bullet.Delete();

							var healthComponent = ship.GetComponent<HealthComponent>();
							healthComponent.AddDamage(4);

							if (!healthComponent.IsAlive)
							{
								var shipTransform = ship.GetComponent<TransformComponent>();
								var shipExplosion = EntityWorld.CreateEntityFromTemplate(ShipExplosionTemplate.Name);
								shipExplosion.GetComponent<TransformComponent>().Position = shipTransform.Position;
								shipExplosion.Refresh();
								ship.Delete();
								break;
							}
						}
					}
				}
			}
		}

		/// <summary>The collision exists.</summary>
		/// <param name="entity1">The entity 1.</param>
		/// <param name="entity2">The entity 2.</param>
		/// <returns>The <see cref="bool" />.</returns>
		private bool CollisionExists(Entity entity1, Entity entity2)
		{
			return Vector2.Distance(entity1.GetComponent<TransformComponent>().Position, entity2.GetComponent<TransformComponent>().Position) < 20;
		}
	}
}