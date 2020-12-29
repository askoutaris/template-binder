using System;
using TemplateBinder.Attributes;
using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	[PipeName(Constants.Pipes.Decimal)]
	public class DecimalPipe : IPipe
	{
		private readonly string? _format;

		public DecimalPipe(string? format)
		{
			_format = format;
		}

		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not Parameter.Number number)
				throw new ArgumentException($"Decimal pipe can only be use with Number parameters. Parameter {parameter.Name} is {parameter.GetType()}");

			var value = number.Value?.ToString(_format);
			return new Parameter.Text(parameter.Name, value);
		}
	}
}
