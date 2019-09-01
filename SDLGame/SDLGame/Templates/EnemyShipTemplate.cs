using Artemis;
using Artemis.Attributes;
using Artemis.Interface;
using SDLGame.Component;

namespace SDLGame.Templates
{
	[ArtemisEntityTemplate(Name)]
	public class EnemyShipTemplate : IEntityTemplate
	{
		/// <summary>The name.</summary>
		public const string Name = "EnemyShipTemplate";

		public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
		{
			entity.Group = "SHIPS";

			entity.AddComponentFromPool<TransformComponent>();
			entity.AddComponent(new SpatialFormComponent("EnemyShip"));
			entity.AddComponent(new HealthComponent(10));
			entity.AddComponent(new WeaponComponent());
			entity.AddComponent(new EnemyComponent());
			entity.AddComponent(new VelocityComponent());

			return entity;
		}
	}
}