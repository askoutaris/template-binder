using System;
using TemplateBinder.Attributes;
using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	[PipeName(Constants.Pipes.Date)]
	public class DatePipe : IPipe
	{
		private readonly string? _format;

		public DatePipe(string? format)
		{
			_format = format;
		}

		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not Parameter.Date date)
				throw new ArgumentException($"Date pipe can only be use with Date parameters. Parameter {parameter.Name} is {parameter.GetType()}");

			var value = date.Value?.ToString(_format);
			return new Parameter.Text(parameter.Name, value);
		}
	}
}
