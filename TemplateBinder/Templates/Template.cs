using System.Text;
using TemplateBinder.Parameters;
using TemplateBinder.TemplateTokens;

namespace TemplateBinder.Templates
{
	public interface ITemplate
	{
		string Bind(IReadOnlyCollection<IParameter> parameters);
	}

	class Template : ITemplate
	{
		private readonly IReadOnlyCollection<ITemplateToken> _tokens;

		public Template(IReadOnlyCollection<ITemplateToken> tokens)
		{
			_tokens = tokens;
		}

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
