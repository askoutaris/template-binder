using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	/// <summary>
	/// Formats numeric values using standard .NET numeric format strings.
	/// </summary>
	/// <example>{{Price|number:format=N2}}</example>
	[PipeName("number")]
	public class NumberPipe : IPipe
	{
		private readonly string? _format;

		/// <summary>
		/// Initializes a new Number pipe.
		/// </summary>
		/// <param name="format">Optional .NET numeric format string (e.g., "N2", "C", "P2").</param>
		public NumberPipe(string? format)
		{
			_format = format;
		}

		/// <summary>
		/// Transforms a NumberParameter into formatted text.
		/// </summary>
		/// <param name="parameter">The NumberParameter to format.</param>
		/// <returns>TextParameter with formatted numeric value.</returns>
		/// <exception cref="ArgumentException">Thrown when parameter is not NumberParameter.</exception>
		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not NumberParameter number)
				throw new ArgumentException($"Number pipe can only be use with NumberParameter. Parameter {parameter.Name} is {parameter.GetType()}");

			var value = number.Value?.ToString(_format);

			return new TextParameter(parameter.Name, value);
		}
	}

}
