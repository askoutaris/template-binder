namespace TemplateBinder.Pipes
{
	/// <summary>
	/// Identifies a pipe implementation with its template syntax name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class PipeNameAttribute : Attribute
	{
		/// <summary>
		/// Gets the pipe name used in template syntax.
		/// </summary>
		public string PipeName { get; }

		/// <summary>
		/// Initializes a new pipe name attribute.
		/// </summary>
		/// <param name="pipeName">The name used in templates (e.g., "datetime", "number").</param>
		public PipeNameAttribute(string pipeName)
		{
			PipeName = pipeName;
		}
	}
}
