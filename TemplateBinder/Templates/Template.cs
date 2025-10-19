using System.Text;
using TemplateBinder.Parameters;
using TemplateBinder.TemplateTokens;

namespace TemplateBinder.Templates
{
	/// <summary>
	/// Represents a parsed template that can be bound with parameter values.
	/// </summary>
	public interface ITemplate
	{
		/// <summary>
		/// Binds parameters to the template and generates output text.
		/// </summary>
		/// <param name="parameters">Collection of parameters to bind.</param>
		/// <returns>The template with all placeholders replaced by parameter values.</returns>
		/// <exception cref="ArgumentException">Thrown when a required parameter is missing.</exception>
		string Bind(IReadOnlyCollection<IParameter> parameters);
	}

	/// <summary>
	/// Default template implementation using token-based rendering.
	/// </summary>
	class Template : ITemplate
	{
		private readonly IReadOnlyCollection<ITemplateToken> _tokens;

		/// <summary>
		/// Initializes a new template with parsed tokens.
		/// </summary>
		/// <param name="tokens">Tokens representing text and placeholders.</param>
		public Template(IReadOnlyCollection<ITemplateToken> tokens)
		{
			_tokens = tokens;
		}

		/// <summary>
		/// Binds parameters to the template and generates output text.
		/// </summary>
		/// <param name="parameters">Collection of parameters to bind.</param>
		/// <returns>The template with all placeholders replaced by parameter values.</returns>
		/// <exception cref="ArgumentException">Thrown when a required parameter is missing.</exception>
		public string Bind(IReadOnlyCollection<IParameter> parameters)
		{
			var parametersMap = parameters.ToDictionary(x => x.Name);

			var sb = new StringBuilder();

			foreach (var token in _tokens)
				sb.Append(token.GetText(parametersMap));

			return sb.ToString();
		}
	}
}
