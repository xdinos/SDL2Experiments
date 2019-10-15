using System.Collections;
using System.Collections.Generic;

namespace Lunatics.Framework.Graphics
{
	public sealed class EffectParameterCollection : IEnumerable<EffectParameter>
	{
		public int Count => elements.Count;

		public EffectParameter this[int index] => elements[index];

		public EffectParameter this[string name]
		{
			get
			{
				foreach (EffectParameter elem in elements)
				{
					if (name.Equals(elem.Name))
					{
						return elem;
					}
				}
				return null; // FIXME: ArrayIndexOutOfBounds? -flibit
			}
		}

		internal EffectParameterCollection(List<EffectParameter> value)
		{
			elements = value;
		}

		public List<EffectParameter>.Enumerator GetEnumerator()
		{
			return elements.GetEnumerator();
		}

		public EffectParameter GetParameterBySemantic(string semantic)
		{
			foreach (EffectParameter elem in elements)
			{
				if (semantic.Equals(elem.Semantic))
				{
					return elem;
				}
			}
			return null;
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return elements.GetEnumerator();
		}

		IEnumerator<EffectParameter> System.Collections.Generic.IEnumerable<EffectParameter>.GetEnumerator()
		{
			return elements.GetEnumerator();
		}


		private List<EffectParameter> elements;
	}
}