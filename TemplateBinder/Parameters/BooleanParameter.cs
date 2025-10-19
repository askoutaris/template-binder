namespace TemplateBinder.Parameters
{
	public readonly struct BooleanParameter : IParameter
	{
		public string Name { get; }
		public bool? Value { get; }

		public BooleanParameter(string name, bool? value)
		{
			Name = name;
			Value = value;
		}

		public string GetText()
			=> Value?.ToString() ?? Name;
	}
}
