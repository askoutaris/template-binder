using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

namespace TemplateBinder.TemplateTokens
{
	class PlaceholderToken : ITemplateToken
	{
		private readonly string _parameterName;
		private readonly IPipe? _pipe;

		public PlaceholderToken(string parameterName, IPipe? pipe)
		{
			_parameterName = parameterName;
			_pipe = pipe;
		}

		public string GetText(IReadOnlyDictionary<string, IParameter> parameters)
		{
			if (!parameters.TryGetValue(_parameterName, out var parameter))
				throw new ArgumentException($"Parameter {_parameterName} was not found");

			var transformedParameter = _pipe?.Transform(parameter) ?? parameter;

			return transformedParameter.GetText();
		}
	}
}
