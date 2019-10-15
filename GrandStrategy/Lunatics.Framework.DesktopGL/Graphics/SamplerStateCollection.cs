using Lunatics.Framework.Graphics;

namespace Lunatics.Framework.DesktopGL.Graphics
{
	public sealed class SamplerStateCollection : ISamplerStates
	{
		#region Public Array Access Property

		public SamplerState this[int index]
		{
			get => _samplers[index];
			set
			{
				_samplers[index] = value;
				_modifiedSamplers[index] = true;
			}
		}

		#endregion

		#region Private Variables

		private readonly SamplerState[] _samplers;
		private readonly bool[] _modifiedSamplers;

		#endregion

		#region Internal Constructor

		internal SamplerStateCollection(int slots, bool[] modSamplers)
		{
			_samplers = new SamplerState[slots];
			_modifiedSamplers = modSamplers;
			for (var i = 0; i < _samplers.Length; i += 1)
			{
				_samplers[i] = SamplerState.LinearWrap;
			}
		}

		#endregion
	}
}