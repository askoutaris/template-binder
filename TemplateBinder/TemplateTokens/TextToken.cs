using TemplateBinder.Parameters;

namespace TemplateBinder.TemplateTokens
{
	class TextToken : ITemplateToken
	{
		private readonly string _text;

		public TextToken(string text)
		{
			_text = text;
		}

		public string GetText(IReadOnlyDictionary<string, IParameter> parameters)
			=> _text;
	}
}
