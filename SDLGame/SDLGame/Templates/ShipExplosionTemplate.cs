using Artemis;
using Artemis.Attributes;
using Artemis.Interface;
using SDLGame.Component;

namespace SDLGame.Templates
{
	[ArtemisEntityTemplate(Name)]
	public class ShipExplosionTemplate : IEntityTemplate
	{
		/// <summary>The name.</summary>
		public const string Name = "ShipExplosionTemplate";

		/// <summary>The build entity.</summary>
		/// <param name="entity">The entity.</param>
		/// <param name="entityWorld">The entity world.</param>
		/// <param name="args">The args.</param>
		/// <returns>The <see cref="Entity" />.</returns>
		public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
		{
			entity.Group = "EFFECTS";

			entity.AddComponentFromPool<TransformComponent>();
			entity.AddComponent(new SpatialFormComponent("ShipExplosion"));
			entity.AddComponent(new ExpiresComponent(1000));

			return entity;
		}
	}
}