﻿namespace Lunatics.Framework.Graphics
{
	/// <summary>
	/// Defines a function for color blending.
	/// </summary>
	public enum BlendFunction
	{
		/// <summary>
		/// The function will add destination to the source. (srcColor * srcBlend) + (destColor * destBlend)
		/// </summary>
		Add,
		/// <summary>
		/// The function will subtract destination from source. (srcColor * srcBlend) - (destColor * destBlend)
		/// </summary>
		Subtract,
		/// <summary>
		/// The function will subtract source from destination. (destColor * destBlend) - (srcColor * srcBlend)
		/// </summary>
		ReverseSubtract,
		/// <summary>
		/// The function will extract minimum of the source and destination. min((srcColor * srcBlend),(destColor * destBlend))
		/// </summary>
		Max,
		/// <summary>
		/// The function will extract maximum of the source and destination. max((srcColor * srcBlend),(destColor * destBlend))
		/// </summary>
		Min,
	}
}