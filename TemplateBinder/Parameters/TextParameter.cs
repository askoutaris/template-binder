namespace TemplateBinder.Parameters
{
	/// <summary>
	/// Represents a text parameter with optional string value.
	/// </summary>
	public readonly struct TextParameter : IParameter
	{
		/// <summary>
		/// Gets the parameter name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the text value, or null if not provided.
		/// </summary>
		public string? Value { get; }

		/// <summary>
		/// Initializes a new text parameter.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="value">The text value, or null.</param>
		public TextParameter(string name, string? value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Gets the text representation.
		/// </summary>
		/// <returns>The value if not null, otherwise the parameter name.</returns>
		public string GetText()
			=> Value ?? Name;
	}
}
