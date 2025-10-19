using TemplateBinder.TemplateTokens;

namespace TemplateBinder.Services
{
	public interface ITemplateTokensFactory
	{
		IReadOnlyCollection<ITemplateToken> Create(IReadOnlyCollection<string> stringTokens);
	}

	public class TemplateTokensFactory : ITemplateTokensFactory
	{
		private readonly IPlaceholderParser _placeholderParser;
		private readonly IPipeActivator _pipeActivator;

		public TemplateTokensFactory(IPlaceholderParser placeholderParser, IPipeActivator pipeActivator)
		{
			_placeholderParser = placeholderParser;
			_pipeActivator = pipeActivator;
		}

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
