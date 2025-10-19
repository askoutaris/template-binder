using System.Text.RegularExpressions;

namespace TemplateBinder.Services
{
	public interface ITemplateParser
	{
		IReadOnlyCollection<string> SplitTokens(string input);
	}

	public class TemplateParser : ITemplateParser
	{
		private static readonly Regex _regex = new Regex("({{.*?}})", RegexOptions.Singleline);

		public IReadOnlyCollection<string> SplitTokens(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return [];

			var parts = _regex.Split(input);

			var result = new List<string>();
			foreach (var part in parts)
				if (!string.IsNullOrEmpty(part))
					result.Add(part);

			return [.. result];
		}
	}
}
