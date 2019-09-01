using System;
using System.Collections.Generic;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using SDLGame.Component;
using SDLGame.Templates;

namespace SDLGame.Systems
{
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 1)]
	public class EnemySpawnSystem : IntervalEntitySystem
	{
		private Random random;

		private Game _game;

		/// <summary>Initializes a new instance of the <see cref="EnemySpawnSystem" /> class.</summary>
		public EnemySpawnSystem()
			: base(new TimeSpan(0, 0, 0, 0, BlackBoard.GetEntry<int>("EnemyInterval")))
		{
		}

		/// <summary>Override to implement code that gets executed when systems are initialized.</summary>
		public override void LoadContent()
		{
			_game = BlackBoard.GetEntry<Game>("Game");
			random = new Random();
		}

		/// <summary>Processes the entities.</summary>
		/// <param name="entities">The entities.</param>
		protected override void ProcessEntities(IDictionary<int, Entity> entities)
		{
			Entity entity = EntityWorld.CreateEntityFromTemplate(EnemyShipTemplate.Name);

			entity.GetComponent<TransformComponent>().X = random.Next(_game._displayMode.w);
			entity.GetComponent<TransformComponent>().Y = random.Next(400) + 50;

			entity.GetComponent<VelocityComponent>().Speed = 0.05f;
			entity.GetComponent<VelocityComponent>().Angle = random.Next() % 2 == 0 ? 0 : 180;
		}
	}
}