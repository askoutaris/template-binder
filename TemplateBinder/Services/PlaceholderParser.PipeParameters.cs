using System.Collections.Specialized;

namespace TemplateBinder.Services
{
	/// <summary>
	/// Represents pipe configuration extracted from a placeholder.
	/// Contains the pipe name and its parameters as key-value pairs.
	/// </summary>
	public class PipeParameters
	{
		/// <summary>
		/// Gets the pipe name as defined in the placeholder (e.g., "datetime", "number", "boolean").
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the pipe parameters as key-value pairs extracted from the placeholder syntax (e.g., "format=yyyy-MM-dd").
		/// </summary>
		public NameValueCollection Parameters { get; }

		/// <summary>
		/// Initializes a new instance of <see cref="PipeParameters"/> with the specified name and parameters.
		/// </summary>
		/// <param name="name">The pipe name.</param>
		/// <param name="parameters">The pipe parameters as key-value pairs.</param>
		public PipeParameters(string name, NameValueCollection parameters)
		{
			Name = name;
			Parameters = parameters;
		}
	}
}
