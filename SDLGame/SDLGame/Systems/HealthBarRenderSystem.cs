using System;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using SDLGame.Component;

namespace SDLGame.Systems
{
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
	public class HealthBarRenderSystem : EntityComponentProcessingSystem<HealthComponent, TransformComponent>
	{
		private Game _game;
		private IntPtr _renderer;

		public override void LoadContent()
		{
			_game = BlackBoard.GetEntry<Game>("Game");
			_renderer = BlackBoard.GetEntry<IntPtr>("rendererPtr");
		}

		public override void Process(Entity entity, HealthComponent healthComponent, TransformComponent transformComponent)
		{
			if (healthComponent != null)
			{
				if (transformComponent != null)
				{
					string text = healthComponent.HealthPercentage + "%";
					float c = (float)(healthComponent.HealthPercentage / 100);
					var r = (byte) (255 * (1f - c));
					var g = (byte) (255 * c);


					var size = _game._font.MesureText(text);
					_game._font.DrawText(_renderer, text,
					                     transformComponent.X - (size.X * 0.5f),
					                     transformComponent.Y + 25,
					                     r, g, 0x00);

					//this.spriteBatch.DrawString(this.font, text, new Vector2(transformComponent.X, transformComponent.Y + 25), color, 0.0f, this.font.MeasureString(text) * 0.5f, 1.0f, SpriteEffects.None, 0.5f);
				}
			}
		}
	}
}