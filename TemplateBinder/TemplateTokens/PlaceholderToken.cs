using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

namespace TemplateBinder.TemplateTokens
{
	/// <summary>
	/// Represents a placeholder token that looks up parameters and applies optional pipe transformations.
	/// </summary>
	class PlaceholderToken : ITemplateToken
	{
		private readonly string _parameterName;
		private readonly IPipe? _pipe;

		/// <summary>
		/// Initializes a new placeholder token.
		/// </summary>
		/// <param name="parameterName">Name of the parameter to bind.</param>
		/// <param name="pipe">Optional pipe to transform the parameter value.</param>
		public PlaceholderToken(string parameterName, IPipe? pipe)
		{
			_parameterName = parameterName;
			_pipe = pipe;
		}

		/// <summary>
		/// Looks up the parameter, applies optional pipe transformation, and returns formatted text.
		/// </summary>
		/// <param name="parameters">Dictionary of available parameters keyed by name.</param>
		/// <returns>The parameter value after optional pipe transformation.</returns>
		/// <exception cref="ArgumentException">Thrown when the parameter is not found.</exception>
		public string GetText(IReadOnlyDictionary<string, IParameter> parameters)
		{
			if (!parameters.TryGetValue(_parameterName, out var parameter))
				throw new ArgumentException($"Parameter {_parameterName} was not found");

			var transformedParameter = _pipe?.Transform(parameter) ?? parameter;

			return transformedParameter.GetText();
		}
	}
}
