using TemplateBinder.TemplateTokens;

namespace TemplateBinder.Services
{
	/// <summary>
	/// Factory for creating typed template tokens from string tokens.
	/// Converts string tokens into <see cref="TextToken"/> or <see cref="PlaceholderToken"/> instances based on placeholder syntax.
	/// </summary>
	public interface ITemplateTokensFactory
	{
		/// <summary>
		/// Creates a collection of typed template tokens from string tokens.
		/// </summary>
		/// <param name="stringTokens">The string tokens to convert. Tokens starting with "{{" and ending with "}}" are treated as placeholders.</param>
		/// <returns>A read-only collection of typed template tokens (<see cref="TextToken"/> or <see cref="PlaceholderToken"/>).</returns>
		IReadOnlyCollection<ITemplateToken> Create(IReadOnlyCollection<string> stringTokens);
	}

	/// <summary>
	/// Factory that creates typed template tokens from string tokens.
	/// Uses <see cref="IPlaceholderParser"/> to extract placeholder parameters and <see cref="IPipeActivator"/> to create pipe instances.
	/// Determines token type based on placeholder syntax: tokens starting with "{{" and ending with "}}" become <see cref="PlaceholderToken"/>, others become <see cref="TextToken"/>.
	/// </summary>
	public class TemplateTokensFactory : ITemplateTokensFactory
	{
		private readonly IPlaceholderParser _placeholderParser;
		private readonly IPipeActivator _pipeActivator;

		/// <summary>
		/// Initializes a new instance of <see cref="TemplateTokensFactory"/> with the specified dependencies.
		/// </summary>
		/// <param name="placeholderParser">The parser for extracting placeholder parameters and pipe information.</param>
		/// <param name="pipeActivator">The activator for creating pipe instances from pipe names and parameters.</param>
		public TemplateTokensFactory(IPlaceholderParser placeholderParser, IPipeActivator pipeActivator)
		{
			_placeholderParser = placeholderParser;
			_pipeActivator = pipeActivator;
		}

		/// <summary>
		/// Creates a collection of typed template tokens from string tokens.
		/// Tokens starting with "{{" and ending with "}}" are parsed as placeholders with optional pipes.
		/// </summary>
		/// <param name="stringTokens">The string tokens to convert.</param>
		/// <returns>A read-only collection of typed template tokens.</returns>
		public IReadOnlyCollection<ITemplateToken> Create(IReadOnlyCollection<string> stringTokens)
		{
			return [.. stringTokens.Select(CreateToken)];
		}

		private ITemplateToken CreateToken(string token)
		{
			if (IsPlaceholder(token))
				return CreatePlaceholderToken(token);
			else
				return new TextToken(token);
		}

		private PlaceholderToken CreatePlaceholderToken(string placeholder)
		{
			var parameters = _placeholderParser.Parse(placeholder);

			var pipe = parameters.Pipe is not null
				? _pipeActivator.Create(parameters.Pipe.Name, parameters.Pipe.Parameters)
				: null;

			return new PlaceholderToken(parameters.ParameterName, pipe);
		}

		private bool IsPlaceholder(string token) =>
			token.StartsWith("{{") && token.EndsWith("}}");
	}
}
