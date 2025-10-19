namespace TemplateBinder.Parameters
{
	/// <summary>
	/// Represents a boolean parameter with optional bool value.
	/// </summary>
	public readonly struct BooleanParameter : IParameter
	{
		/// <summary>
		/// Gets the parameter name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the boolean value, or null if not provided.
		/// </summary>
		public bool? Value { get; }

		/// <summary>
		/// Initializes a new boolean parameter.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="value">The boolean value, or null.</param>
		public BooleanParameter(string name, bool? value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Gets the text representation as "True" or "False".
		/// </summary>
		/// <returns>The value as string if not null, otherwise the parameter name.</returns>
		public string GetText()
			=> Value?.ToString() ?? Name;
	}
}
