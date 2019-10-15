﻿namespace Lunatics.Framework.Graphics
{
	/// <summary>
	/// Defines modes for addressing texels using texture coordinates that are outside of the range of 0.0 to 1.0.
	/// </summary>
	public enum TextureAddressMode
	{
		/// <summary>
		/// Texels outside range will form the tile at every integer junction.
		/// </summary>
		Wrap,
		/// <summary>
		/// Texels outside range will be setted to color of 0.0 or 1.0 texel.
		/// </summary>
		Clamp,
		/// <summary>
		/// Same as <see cref="TextureAddressMode.Wrap"/> but tiles will also flipped at every integer junction.
		/// </summary>
		Mirror,
	}
}