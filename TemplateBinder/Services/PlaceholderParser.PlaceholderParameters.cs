namespace TemplateBinder.Services
{
	/// <summary>
	/// Represents placeholder configuration extracted from a template placeholder.
	/// Contains the parameter name and optional pipe configuration.
	/// </summary>
	public class PlaceholderParameters
	{
		/// <summary>
		/// Gets the parameter name from the placeholder (e.g., "FirstName" from "{{FirstName}}").
		/// </summary>
		public string ParameterName { get; }

		/// <summary>
		/// Gets the pipe configuration if a pipe is specified in the placeholder, or null if no pipe is used.
		/// </summary>
		public PipeParameters? Pipe { get; }

		/// <summary>
		/// Initializes a new instance of <see cref="PlaceholderParameters"/> with the specified parameter name and optional pipe.
		/// </summary>
		/// <param name="parameterName">The parameter name.</param>
		/// <param name="pipe">The pipe configuration, or null if no pipe is specified.</param>
		public PlaceholderParameters(string parameterName, PipeParameters? pipe)
		{
			ParameterName = parameterName;
			Pipe = pipe;
		}
	}
}
