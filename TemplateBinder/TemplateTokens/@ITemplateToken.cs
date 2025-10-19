using TemplateBinder.Parameters;

namespace TemplateBinder.TemplateTokens
{
	public interface ITemplateToken
	{
		string GetText(IReadOnlyDictionary<string, IParameter> parameters);
	}
}
