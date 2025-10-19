namespace TemplateBinder.Parameters
{
	/// <summary>
	/// Represents a named parameter that can be bound to a template placeholder.
	/// </summary>
	public interface IParameter
	{
		/// <summary>
		/// Gets the parameter name used for binding to template placeholders.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the text representation of this parameter.
		/// </summary>
		/// <returns>The formatted text value, or the parameter name if value is null.</returns>
		public string GetText();
	}
}
