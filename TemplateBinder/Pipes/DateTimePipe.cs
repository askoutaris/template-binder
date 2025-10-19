using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	/// <summary>
	/// Formats DateTime values using standard .NET format strings.
	/// </summary>
	/// <example>{{Date|datetime:format=yyyy-MM-dd}}</example>
	[PipeName("datetime")]
	public class DateTimePipe : IPipe
	{
		private readonly string? _format;

		/// <summary>
		/// Initializes a new DateTime pipe.
		/// </summary>
		/// <param name="format">Optional .NET DateTime format string (e.g., "yyyy-MM-dd", "o").</param>
		public DateTimePipe(string? format)
		{
			_format = format;
		}

		/// <summary>
		/// Transforms a DateTimeParameter into formatted text.
		/// </summary>
		/// <param name="parameter">The DateTimeParameter to format.</param>
		/// <returns>TextParameter with formatted date/time value.</returns>
		/// <exception cref="ArgumentException">Thrown when parameter is not DateTimeParameter.</exception>
		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not DateTimeParameter datetime)
				throw new ArgumentException($"Date pipe can only be use with DateTime parameters. Parameter {parameter.Name} is {parameter.GetType()}");

			var value = datetime.Value?.ToString(_format);

			return new TextParameter(parameter.Name, value);
		}
	}
}
