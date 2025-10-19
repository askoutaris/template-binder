namespace TemplateBinder.Parameters
{
	public readonly struct NumberParameter : IParameter
	{
		public string Name { get; }
		public decimal? Value { get; }

		public NumberParameter(string name, decimal? value)
		{
			Name = name;
			Value = value;
		}

		public string GetText()
			=> Value?.ToString() ?? Name;
	}
}
