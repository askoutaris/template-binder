using System;
using TemplateBinder.Attributes;
using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	[PipeName(Constants.Pipes.BooleanText)]
	public class BooleanTextPipe : IPipe
	{
		private readonly string? _falseValue;
		private readonly string? _trueValue;

		public BooleanTextPipe(string? falseValue, string? trueValue)
		{
			_falseValue = falseValue;
			_trueValue = trueValue;
		}

		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not Parameter.Boolean boolean)
				throw new ArgumentException($"BooleanText pipe can only be use with Boolean parameters. Parameter {parameter.Name} is {parameter.GetType()}");

			var value = boolean.Value == false ? _falseValue : boolean.Value == true ? _trueValue : null;
			return new Parameter.Text(parameter.Name, value);
		}
	}
}
