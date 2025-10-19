using TemplateBinder.Templates;

namespace TemplateBinder.Services
{
	public interface ITemplateFactory
	{
		ITemplate Create(string template);
	}

	class TemplateFactory : ITemplateFactory
	{
		private readonly ITemplateParser _parser;
		private readonly ITemplateTokensFactory _tokensFactory;

		public TemplateFactory(ITemplateParser parser, ITemplateTokensFactory tokensFactory)
		{
			_parser = parser;
			_tokensFactory = tokensFactory;
		}

		public ITemplate Create(string template)
		{
			var stringTokens = _parser.SplitTokens(template);

			var tokens = _tokensFactory.Create(stringTokens);

			return new Template(tokens);
		}
	}
}
