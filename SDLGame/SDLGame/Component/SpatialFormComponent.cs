﻿using Artemis.Interface;
using Lunatics.Graphics;

namespace SDLGame.Component
{
	public class AnimationComponent : IComponent
	{
		public AnimatedSprite AnimatedSprite { get; set; }

		public AnimationComponent(AnimatedSprite animatedSprite)
		{
			AnimatedSprite = animatedSprite;
		}
	}

	/// <summary>The spatial form.</summary>
	public class SpatialFormComponent : IComponent
	{
		public Sprite Sprite { get; set; }

		/// <summary>Gets or sets the spatial form file.</summary>
		/// <value>The spatial form file.</value>
		public string SpatialFormFile { get; set; }

		/// <summary>Initializes a new instance of the <see cref="SpatialFormComponent" /> class.</summary>
		public SpatialFormComponent()
			: this(string.Empty)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="SpatialFormComponent" /> class.</summary>
		/// <param name="spatialFormFile">The spatial form file.</param>
		public SpatialFormComponent(string spatialFormFile)
		{
			SpatialFormFile = spatialFormFile;
		}


	}
}