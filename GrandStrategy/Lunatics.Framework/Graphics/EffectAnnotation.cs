namespace Lunatics.Framework.Graphics
{
	public class EffectAnnotation
	{
		public string Name { get; private set; }
		public string Semantic { get; private set; }
		public int RowCount { get; private set; }
		public int ColumnCount { get; private set; }
		public EffectParameterClass ParameterClass { get; private set; }
		public EffectParameterType ParameterType { get; private set; }

		internal EffectAnnotation(string name,
		                          string semantic,
		                          int rowCount,
		                          int columnCount,
		                          EffectParameterClass parameterClass,
		                          EffectParameterType parameterType,
		                          object data)
		{

			Name = name;
			Semantic = semantic;
			RowCount = rowCount;
			ColumnCount = columnCount;

			ParameterClass = parameterClass;
			ParameterType = parameterType;
		}
	}
}