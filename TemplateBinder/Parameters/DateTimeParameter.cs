namespace TemplateBinder.Parameters
{
	public readonly struct DateTimeParameter : IParameter
	{
		public string Name { get; }
		public DateTime? Value { get; }

		public DateTimeParameter(string name, DateTime? value)
		{
			Name = name;
			Value = value;
		}

		public string GetText()
			=> Value?.ToString("O") ?? Name;
	}
}
