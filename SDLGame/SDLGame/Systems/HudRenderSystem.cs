using System;
using System.Drawing;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using SDLGame.Component;

namespace SDLGame.Systems
{
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
	public class HudRenderSystem : TagSystem
	{
		private Game _game;
		private IntPtr _renderer;

		public HudRenderSystem()
			: base("PLAYER")
		{
		}

		public override void LoadContent()
		{
			_game = BlackBoard.GetEntry<Game>("Game");
			_renderer = BlackBoard.GetEntry<IntPtr>("rendererPtr");
		}

		/// <summary>Processes the specified entity.</summary>
		/// <param name="entity">The entity.</param>
		public override void Process(Entity entity)
		{
			HealthComponent healthComponent = entity.GetComponent<HealthComponent>();

			_game._font.DrawText(_renderer, "Health: " + healthComponent.HealthPercentage + "%",
			                     20,
			                     _game._displayMode.h - 100,
			                     0x00, 0x00, 0x00);
		}
	}
}