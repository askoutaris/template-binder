using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	[PipeName("boolean")]
	public class BooleanPipe : IPipe
	{
		private readonly string _trueValue;
		private readonly string _falseValue;

		public BooleanPipe(string trueValue, string falseValue)
		{
			_trueValue = trueValue;
			_falseValue = falseValue;
		}

		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not BooleanParameter boolean)
				throw new ArgumentException($"Boolean pipe can only be use with BooleanParameter. Parameter {parameter.Name} is {parameter.GetType()}");

			var value = boolean.Value.HasValue
				? boolean.Value == true ? _trueValue : _falseValue
				: null;

			return new TextParameter(parameter.Name, value);
		}
	}
}
