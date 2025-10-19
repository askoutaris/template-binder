using System.Text.RegularExpressions;

namespace TemplateBinder.Services
{
	/// <summary>
	/// Parses template strings into tokens (text and placeholders).
	/// </summary>
	public interface ITemplateParser
	{
		/// <summary>
		/// Splits a template string into text and placeholder tokens.
		/// </summary>
		/// <param name="input">The template string to parse.</param>
		/// <returns>Collection of string tokens (text and placeholders like {{Name}}).</returns>
		IReadOnlyCollection<string> SplitTokens(string input);
	}

	/// <summary>
	/// Default implementation that uses regex to split templates by {{...}} placeholders.
	/// </summary>
	public class TemplateParser : ITemplateParser
	{
		private static readonly Regex _regex = new Regex("({{.*?}})", RegexOptions.Singleline);

		/// <summary>
		/// Splits a template string into text and placeholder tokens.
		/// </summary>
		/// <param name="input">The template string to parse.</param>
		/// <returns>Collection of tokens, alternating between text and placeholders.</returns>
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
