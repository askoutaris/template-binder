namespace TemplateBinder.Parameters
{
	public readonly struct TextParameter : IParameter
	{
		public string Name { get; }
		public string? Value { get; }

		public TextParameter(string name, string? value)
		{
			Name = name;
			Value = value;
		}

		public string GetText()
			=> Value ?? Name;
	}
}
