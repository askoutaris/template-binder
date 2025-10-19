using TemplateBinder.Parameters;

namespace TemplateBinder.TemplateTokens
{
	/// <summary>
	/// Represents a token in a template that can generate text output.
	/// </summary>
	public interface ITemplateToken
	{
		/// <summary>
		/// Generates text output for this token.
		/// </summary>
		/// <param name="parameters">Dictionary of available parameters keyed by name.</param>
		/// <returns>Text representation of this token.</returns>
		string GetText(IReadOnlyDictionary<string, IParameter> parameters);
	}
}
