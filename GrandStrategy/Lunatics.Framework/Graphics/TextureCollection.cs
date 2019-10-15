using System;

namespace Lunatics.Framework.Graphics
{
	public sealed class TextureCollection
	{
		public ITexture this[int index]
		{
			get
			{
				return _textures[index];
			}
			set
			{
#if DEBUG
				// XNA checks for disposed textures here! -flibit
				if (value != null && value.IsDisposed)
					throw new ObjectDisposedException(value.GetType().ToString());
#endif
				_textures[index] = value;
				_modifiedSamplers[index] = true;
			}
		}

		public TextureCollection(int slots, bool[] modifiedSamplers)
		{
			_textures = new ITexture[slots];
			_modifiedSamplers = modifiedSamplers;
			for (var i = 0; i < _textures.Length; i += 1)
				_textures[i] = null;
		}

		private readonly ITexture[] _textures;
		private readonly bool[] _modifiedSamplers;
	}
}