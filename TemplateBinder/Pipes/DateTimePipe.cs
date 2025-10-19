using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	[PipeName("datetime")]
	public class DateTimePipe : IPipe
	{
		private readonly string? _format;

		public DateTimePipe(string? format)
		{
			_format = format;
		}

		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not DateTimeParameter datetime)
				throw new ArgumentException($"Date pipe can only be use with DateTime parameters. Parameter {parameter.Name} is {parameter.GetType()}");

			var value = datetime.Value?.ToString(_format);

			return new TextParameter(parameter.Name, value);
		}
	}
}
