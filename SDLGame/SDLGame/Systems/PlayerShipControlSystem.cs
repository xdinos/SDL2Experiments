using System;
using System.CodeDom;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Artemis.Utils;
using Lunatics.Input;
using SDL2;
using SDLGame.Component;
using SDLGame.Templates;

namespace SDLGame.Systems
{
	/// <summary>The player ship control system.</summary>
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Update)]
	public class PlayerShipControlSystem : TagSystem
	{
		private Game _game;

		/// <summary>The missile launch timer.</summary>
		private readonly Timer missileLaunchTimer;
		
		
		/// <summary>Initializes a new instance of the <see cref="PlayerShipControlSystem" /> class.</summary>
		public PlayerShipControlSystem()
			: base("PLAYER")
		{
			this.missileLaunchTimer = new Timer(new TimeSpan(0, 0, 0, 0, 150));
		}

		/// <summary>Override to implement code that gets executed when systems are initialized.</summary>
		public override void LoadContent()
		{
			_game = BlackBoard.GetEntry<Game>("Game");
		}

		/// <summary>Processes the specified entity.</summary>
		/// <param name="entity">The entity.</param>
		public override void Process(Entity entity)
		{
			TransformComponent transformComponent = entity.GetComponent<TransformComponent>();

			//KeyboardState keyboardState = Keyboard.GetState();
			var ts = TimeSpan.FromTicks(EntityWorld.Delta);
			float keyMoveSpeed = (float) (0.3f * ts.TotalMilliseconds);
			if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_a) ||
				Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_LEFT)
				/*@event.key.keysym.sym == SDL.SDL_Keycode.SDLK_a  || @event.key.keysym.sym == SDL.SDL_Keycode.SDLK_LEFT*/)
			{
				transformComponent.X -= keyMoveSpeed;
				if (transformComponent.X < 32)
				{
					transformComponent.X = 32;
				}
			}
			else if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_d) ||
			         Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_RIGHT)
				/*@event.key.keysym.sym == SDL.SDL_Keycode.SDLK_d || @event.key.keysym.sym == SDL.SDL_Keycode.SDLK_RIGHT*/)
			{
				transformComponent.X += keyMoveSpeed;
				if (transformComponent.X > _game._displayMode.w - 32)
				{
					transformComponent.X = _game._displayMode.w - 32;
				}
			}



			if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_SPACE) ||
			    Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_RETURN) ||
			    Mouse.LeftButton == Mouse.ButtonState.Pressed
				/*@event.key.keysym.sym == SDL.SDL_Keycode.SDLK_SPACE || @event.key.keysym.sym == SDL.SDL_Keycode.SDLK_RETURN
				keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Enter)*/)
			{
				if (this.missileLaunchTimer.IsReached(this.EntityWorld.Delta))
				{
					this.AddMissile(transformComponent);
					this.AddMissile(transformComponent, 89, -9);
					this.AddMissile(transformComponent, 91, +9);
				}
			}
		}

		/// <summary>Adds the missile.</summary>
		/// <param name="transformComponent">The transform component.</param>
		/// <param name="angle">The angle.</param>
		/// <param name="offsetX">The offset X.</param>
		private void AddMissile(TransformComponent transformComponent, float angle = 90.0f, float offsetX = 0.0f)
		{
			Entity missile = this.EntityWorld.CreateEntityFromTemplate(MissileTemplate.Name);

			missile.GetComponent<TransformComponent>().X = transformComponent.X + 1 + offsetX;
			missile.GetComponent<TransformComponent>().Y = transformComponent.Y - 20;
			
			missile.GetComponent<VelocityComponent>().Speed = -0.5f;
			missile.GetComponent<VelocityComponent>().Angle = angle;
		}
	}
}