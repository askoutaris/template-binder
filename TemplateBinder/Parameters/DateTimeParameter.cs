namespace TemplateBinder.Parameters
{
	/// <summary>
	/// Represents a date/time parameter with optional DateTime value.
	/// </summary>
	public readonly struct DateTimeParameter : IParameter
	{
		/// <summary>
		/// Gets the parameter name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the date/time value, or null if not provided.
		/// </summary>
		public DateTime? Value { get; }

		/// <summary>
		/// Initializes a new date/time parameter.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="value">The date/time value, or null.</param>
		public DateTimeParameter(string name, DateTime? value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Gets the text representation in ISO 8601 format.
		/// </summary>
		/// <returns>The value in ISO 8601 format if not null, otherwise the parameter name.</returns>
		public string GetText()
			=> Value?.ToString("O") ?? Name;
	}
}
