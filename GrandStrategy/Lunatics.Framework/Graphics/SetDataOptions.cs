﻿namespace Lunatics.Framework.Graphics
{
	/// <summary>
	/// Defines how vertex or index buffer data will be flushed during a SetData operation.
	/// </summary>
	public enum SetDataOptions
	{
		/// <summary>
		/// The SetData can overwrite the portions of existing data.
		/// </summary>
		None = 0,
		/// <summary>
		/// The SetData will discard the entire buffer. A pointer to a new memory area is returned and rendering from the previous area do not stall.
		/// </summary>
		Discard = 1,
		/// <summary>
		/// The SetData operation will not overwrite existing data. This allows the driver to return immediately from a SetData operation and continue rendering.
		/// </summary>
		NoOverwrite = 2
	}
}