namespace TemplateBinder.Parameters
{
	/// <summary>
	/// Represents a numeric parameter with optional decimal value.
	/// </summary>
	public readonly struct NumberParameter : IParameter
	{
		/// <summary>
		/// Gets the parameter name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the numeric value, or null if not provided.
		/// </summary>
		public decimal? Value { get; }

		/// <summary>
		/// Initializes a new numeric parameter.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="value">The numeric value, or null.</param>
		public NumberParameter(string name, decimal? value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Gets the text representation.
		/// </summary>
		/// <returns>The value as string if not null, otherwise the parameter name.</returns>
		public string GetText()
			=> Value?.ToString() ?? Name;
	}
}
