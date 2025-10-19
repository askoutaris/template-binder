using TemplateBinder.Parameters;

namespace TemplateBinder.TemplateTokens
{
	interface ITemplateToken
	{
		string GetText(IReadOnlyDictionary<string, IParameter> parameters);
	}
}
