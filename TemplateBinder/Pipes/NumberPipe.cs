using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	[PipeName("number")]
	public class NumberPipe : IPipe
	{
		private readonly string? _format;

		public NumberPipe(string? format)
		{
			_format = format;
		}

		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not NumberParameter number)
				throw new ArgumentException($"Number pipe can only be use with NumberParameter. Parameter {parameter.Name} is {parameter.GetType()}");

			var value = number.Value?.ToString(_format);

			return new TextParameter(parameter.Name, value);
		}
	}

}
