using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	/// <summary>
	/// Converts boolean values to custom text representations.
	/// </summary>
	/// <example>{{IsActive|boolean:trueValue=Yes,falseValue=No}}</example>
	[PipeName("boolean")]
	public class BooleanPipe : IPipe
	{
		private readonly string _trueValue;
		private readonly string _falseValue;

		/// <summary>
		/// Initializes a new Boolean pipe.
		/// </summary>
		/// <param name="trueValue">Text to display when value is true.</param>
		/// <param name="falseValue">Text to display when value is false.</param>
		public BooleanPipe(string trueValue, string falseValue)
		{
			_trueValue = trueValue;
			_falseValue = falseValue;
		}

		/// <summary>
		/// Transforms a BooleanParameter into custom text.
		/// </summary>
		/// <param name="parameter">The BooleanParameter to transform.</param>
		/// <returns>TextParameter with custom true/false text.</returns>
		/// <exception cref="ArgumentException">Thrown when parameter is not BooleanParameter.</exception>
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
