using Artemis;
using Artemis.Attributes;
using Artemis.Interface;
using SDLGame.Component;

namespace SDLGame.Templates
{
	[ArtemisEntityTemplate(Name)]
	public class MissileTemplate : IEntityTemplate
	{
		public const string Name = "MissileTemplate";

		public Entity BuildEntity(Entity entity, EntityWorld entityWorld, params object[] args)
		{
			entity.Group = "BULLETS";

			entity.AddComponentFromPool<TransformComponent>();
			entity.AddComponent(new SpatialFormComponent("Missile"));
			entity.AddComponent(new VelocityComponent());
			entity.AddComponent(new ExpiresComponent(2000));

			return entity;
		}
	}
}