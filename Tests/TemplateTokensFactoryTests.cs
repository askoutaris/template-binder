using TemplateBinder.Pipes;
using TemplateBinder.Services;
using TemplateBinder.TemplateTokens;

namespace Tests
{
	public class TemplateTokensFactoryTests
	{
		private readonly PlaceholderParser _placeholderParser = new();
		private readonly PipeActivator _pipeActivator = new([
			typeof(DateTimePipe),
			typeof(NumberPipe),
			typeof(BooleanPipe)
		]);
		private readonly TemplateTokensFactory _factory;

		public TemplateTokensFactoryTests()
		{
			_factory = new TemplateTokensFactory(_placeholderParser, _pipeActivator);
		}

		[Fact]
		public void Create_TextOnly_CreatesTextToken()
		{
			var tokens = _factory.Create(["Hello World"]);

			Assert.Single(tokens);
			var token = tokens.First();
			Assert.IsType<TextToken>(token);
		}

		[Fact]
		public void Create_PlaceholderOnly_CreatesPlaceholderToken()
		{
			var tokens = _factory.Create(["{{Name}}"]);

			Assert.Single(tokens);
			var token = tokens.First();
			Assert.IsType<PlaceholderToken>(token);
		}

		[Fact]
		public void Create_MixedTokens_CreatesCorrectTypes()
		{
			var tokens = _factory.Create(["Hello ", "{{Name}}", "!"]);

			Assert.Equal(3, tokens.Count);
			Assert.IsType<TextToken>(tokens.ElementAt(0));
			Assert.IsType<PlaceholderToken>(tokens.ElementAt(1));
			Assert.IsType<TextToken>(tokens.ElementAt(2));
		}

		[Fact]
		public void Create_PlaceholderWithPipe_CreatesTokenWithPipe()
		{
			var tokens = _factory.Create(["{{Date|datetime:format=yyyy-MM-dd}}"]);

			Assert.Single(tokens);
			var token = tokens.First();
			Assert.IsType<PlaceholderToken>(token);
		}

		[Fact]
		public void Create_MultiplePlaceholders_CreatesAllTokens()
		{
			var tokens = _factory.Create([
				"{{FirstName}}",
				" ",
				"{{LastName}}"
			]);

			Assert.Equal(3, tokens.Count);
			Assert.IsType<PlaceholderToken>(tokens.ElementAt(0));
			Assert.IsType<TextToken>(tokens.ElementAt(1));
			Assert.IsType<PlaceholderToken>(tokens.ElementAt(2));
		}

		[Fact]
		public void Create_EmptyList_ReturnsEmptyList()
		{
			var tokens = _factory.Create([]);

			Assert.Empty(tokens);
		}

		[Fact]
		public void Create_ComplexTemplate_CreatesCorrectTokens()
		{
			var tokens = _factory.Create([
				"Name: ",
				"{{Name}}",
				", Age: ",
				"{{Age|number}}",
				", Active: ",
				"{{IsActive|boolean:trueValue=Yes,falseValue=No}}"
			]);

			Assert.Equal(6, tokens.Count);
			Assert.IsType<TextToken>(tokens.ElementAt(0));
			Assert.IsType<PlaceholderToken>(tokens.ElementAt(1));
			Assert.IsType<TextToken>(tokens.ElementAt(2));
			Assert.IsType<PlaceholderToken>(tokens.ElementAt(3));
			Assert.IsType<TextToken>(tokens.ElementAt(4));
			Assert.IsType<PlaceholderToken>(tokens.ElementAt(5));
		}
	}
}
