using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lunatics.Graphics
{
	public abstract class Animation : IDisposable
	{
		private readonly bool _disposeOnComplete;
		private readonly Action _onCompleteAction;
		private bool _isComplete;

		protected Animation(Action onCompleteAction, bool disposeOnComplete)
		{
			_onCompleteAction = onCompleteAction;
			_disposeOnComplete = disposeOnComplete;
			IsPaused = false;
		}

		public bool IsComplete
		{
			get => _isComplete;
			protected set
			{
				if (_isComplete != value)
				{
					_isComplete = value;

					if (_isComplete)
					{
						_onCompleteAction?.Invoke();

						if (_disposeOnComplete)
							Dispose();
					}
				}
			}
		}

		public bool IsDisposed { get; private set; }
		public bool IsPlaying => !IsPaused && !IsComplete;
		public bool IsPaused { get; private set; }
		public float CurrentTime { get; protected set; }

		public virtual void Dispose()
		{
			IsDisposed = true;
		}

		//public void Update(GameTime gameTime)
		//{
		//	Update(gameTime.GetElapsedSeconds());
		//}

		public void Play()
		{
			IsPaused = false;
		}

		public void Pause()
		{
			IsPaused = true;
		}

		public void Stop()
		{
			Pause();
			Rewind();
		}

		public void Rewind()
		{
			CurrentTime = 0;
		}

		protected abstract bool OnUpdate(float deltaTime);

		public void Update(float deltaTime)
		{
			if (!IsPlaying)
				return;

			CurrentTime += deltaTime;
			IsComplete = OnUpdate(deltaTime);
		}
	}

	public class AnimatedSprite : Sprite
	{
		private readonly SpriteSheetAnimationFactory _animationFactory;
		private SpriteSheetAnimation _currentAnimation;

		public AnimatedSprite(SpriteSheetAnimationFactory animationFactory, string playAnimation = null)
			: base(animationFactory.Frames[0])
		{
			_animationFactory = animationFactory;

			if (playAnimation != null)
				Play(playAnimation);
		}

		public SpriteSheetAnimation Play(string name, Action onCompleted = null)
		{
			if (_currentAnimation == null || _currentAnimation.IsComplete || _currentAnimation.Name != name)
			{
				_currentAnimation = _animationFactory.Create(name);

				if (_currentAnimation != null)
					_currentAnimation.OnCompleted = onCompleted;
			}

			return _currentAnimation;
		}

		public void Update(float deltaTime)
		{
			if (_currentAnimation != null && !_currentAnimation.IsComplete)
			{
				_currentAnimation.Update(deltaTime);
				TextureRegion = _currentAnimation.CurrentFrame;
			}
		}
	}

	public class SpriteSheetAnimationData
	{
		public SpriteSheetAnimationData(int[] frameIndicies, float frameDuration = 0.2f, bool isLooping = true,
		                                bool isReversed = false, bool isPingPong = false)
		{
			FrameIndicies = frameIndicies;
			FrameDuration = frameDuration;
			IsLooping = isLooping;
			IsReversed = isReversed;
			IsPingPong = isPingPong;
		}

		public int[] FrameIndicies { get; }
		public float FrameDuration { get; }
		public bool IsLooping { get; }
		public bool IsReversed { get; }
		public bool IsPingPong { get; }
	}

	public class SpriteSheetAnimationFactory
	{
		private readonly Dictionary<string, SpriteSheetAnimationData> _animationDataDictionary;

		public SpriteSheetAnimationFactory(TextureAtlas textureAtlas)
			: this(textureAtlas.Regions)
		{
		}

		public SpriteSheetAnimationFactory(IEnumerable<TextureRegion2D> frames)
		{
			_animationDataDictionary = new Dictionary<string, SpriteSheetAnimationData>();
			Frames = new ReadOnlyCollection<TextureRegion2D>(frames.ToArray());
		}

		public ReadOnlyCollection<TextureRegion2D> Frames { get; }

		public void Add(string name, SpriteSheetAnimationData data)
		{
			_animationDataDictionary.Add(name, data);
		}

		public void Remove(string name)
		{
			_animationDataDictionary.Remove(name);
		}

		public SpriteSheetAnimation Create(string name)
		{
			if (_animationDataDictionary.TryGetValue(name, out var data))
			{
				var keyFrames = data.FrameIndicies
				                    .Select(i => Frames[i])
				                    .ToArray();

				return new SpriteSheetAnimation(name, keyFrames, data);
			}

			return null;
		}
	}

	public class SpriteSheetAnimation : Animation
	{
		public const float DefaultFrameDuration = 0.2f;

		public SpriteSheetAnimation(string name,
		                            TextureAtlas textureAtlas,
		                            float frameDuration = DefaultFrameDuration,
		                            bool isLooping = true,
		                            bool isReversed = false,
		                            bool isPingPong = false)
			: this(name, textureAtlas.Regions.ToArray(), frameDuration, isLooping, isReversed, isPingPong)
		{
		}

		public SpriteSheetAnimation(string name,
		                            TextureRegion2D[] keyFrames,
		                            float frameDuration = DefaultFrameDuration,
		                            bool isLooping = true,
		                            bool isReversed = false,
		                            bool isPingPong = false)
			: base(null, false)
		{
			Name = name;
			KeyFrames = keyFrames;
			FrameDuration = frameDuration;
			IsLooping = isLooping;
			IsReversed = isReversed;
			IsPingPong = isPingPong;
			CurrentFrameIndex = IsReversed ? KeyFrames.Length - 1 : 0;
		}

		public SpriteSheetAnimation(string name, TextureRegion2D[] keyFrames, SpriteSheetAnimationData data)
			: this(name, keyFrames, data.FrameDuration, data.IsLooping, data.IsReversed, data.IsPingPong)
		{
		}

		public string Name { get; }
		public TextureRegion2D[] KeyFrames { get; }
		public float FrameDuration { get; set; }
		public bool IsLooping { get; set; }
		public bool IsReversed { get; set; }
		public bool IsPingPong { get; set; }
		public new bool IsComplete => CurrentTime >= AnimationDuration;

		public float AnimationDuration => IsPingPong
			? (KeyFrames.Length * 2 - 2) * FrameDuration
			: KeyFrames.Length * FrameDuration;

		public TextureRegion2D CurrentFrame => KeyFrames[CurrentFrameIndex];
		public int CurrentFrameIndex { get; private set; }

		public float FramesPerSecond
		{
			get => 1.0f / FrameDuration;
			set => FrameDuration = value / 1.0f;
		}

		public Action OnCompleted { get; set; }

		protected override bool OnUpdate(float deltaTime)
		{
			if (IsComplete)
			{
				OnCompleted?.Invoke();

				if (IsLooping)
					CurrentTime -= AnimationDuration;
			}

			if (KeyFrames.Length == 1)
			{
				CurrentFrameIndex = 0;
				return IsComplete;
			}

			var frameIndex = (int)(CurrentTime / FrameDuration);
			var length = KeyFrames.Length;

			if (IsPingPong)
			{
				frameIndex = frameIndex % (length * 2 - 2);

				if (frameIndex >= length)
					frameIndex = length - 2 - (frameIndex - length);
			}

			if (IsLooping)
			{
				if (IsReversed)
				{
					frameIndex = frameIndex % length;
					frameIndex = length - frameIndex - 1;
				}
				else
					frameIndex = frameIndex % length;
			}
			else
				frameIndex = IsReversed ? Math.Max(length - frameIndex - 1, 0) : Math.Min(length - 1, frameIndex);

			CurrentFrameIndex = frameIndex;
			return IsComplete;
		}
	}
}