using TemplateBinder.Parameters;

namespace TemplateBinder.TemplateTokens
{
	/// <summary>
	/// Represents a literal text token that outputs fixed text.
	/// </summary>
	class TextToken : ITemplateToken
	{
		private readonly string _text;

		/// <summary>
		/// Initializes a new text token.
		/// </summary>
		/// <param name="text">The literal text to output.</param>
		public TextToken(string text)
		{
			_text = text;
		}

		/// <summary>
		/// Returns the literal text unchanged.
		/// </summary>
		/// <param name="parameters">Dictionary of parameters (not used by this token).</param>
		/// <returns>The literal text.</returns>
		public string GetText(IReadOnlyDictionary<string, IParameter> parameters)
			=> _text;
	}
}
