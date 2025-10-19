using TemplateBinder.Templates;

namespace TemplateBinder.Services
{
	/// <summary>
	/// Factory for creating ITemplate instances from template strings.
	/// </summary>
	public interface ITemplateFactory
	{
		/// <summary>
		/// Creates a reusable template from a template string.
		/// </summary>
		/// <param name="template">Template string with {{...}} placeholders.</param>
		/// <returns>Parsed template ready for binding parameters.</returns>
		ITemplate Create(string template);
	}

	/// <summary>
	/// Default template factory that orchestrates parsing and token creation.
	/// </summary>
	public class TemplateFactory : ITemplateFactory
	{
		private readonly ITemplateParser _parser;
		private readonly ITemplateTokensFactory _tokensFactory;

		/// <summary>
		/// Initializes a new template factory.
		/// </summary>
		/// <param name="parser">Parser for splitting template into tokens.</param>
		/// <param name="tokensFactory">Factory for creating typed token objects.</param>
		public TemplateFactory(ITemplateParser parser, ITemplateTokensFactory tokensFactory)
		{
			_parser = parser;
			_tokensFactory = tokensFactory;
		}

		/// <summary>
		/// Creates a reusable template from a template string.
		/// </summary>
		/// <param name="template">Template string with {{...}} placeholders.</param>
		/// <returns>Parsed template ready for binding parameters.</returns>
		public ITemplate Create(string template)
		{
			var stringTokens = _parser.SplitTokens(template);

			var tokens = _tokensFactory.Create(stringTokens);

			return new Template(tokens);
		}
	}
}
