using TemplateBinder.Services;

namespace Tests
{
	public class TemplateParserTests
	{
		private readonly TemplateParser _parser = new();

		[Fact]
		public void SplitTokens_WithTextOnly_ReturnsSingleToken()
		{
			var result = _parser.SplitTokens("Hello World");

			Assert.Single(result);
			Assert.Equal("Hello World", result.First());
		}

		[Fact]
		public void SplitTokens_WithSinglePlaceholder_ReturnsSingleToken()
		{
			var result = _parser.SplitTokens("{{Name}}");

			Assert.Single(result);
			Assert.Equal("{{Name}}", result.First());
		}

		[Fact]
		public void SplitTokens_WithTextAndPlaceholder_ReturnsMultipleTokens()
		{
			var result = _parser.SplitTokens("Hello {{Name}}!");

			Assert.Equal(3, result.Count);
			Assert.Equal("Hello ", result.ElementAt(0));
			Assert.Equal("{{Name}}", result.ElementAt(1));
			Assert.Equal("!", result.ElementAt(2));
		}

		[Fact]
		public void SplitTokens_WithMultiplePlaceholders_ReturnsAllTokens()
		{
			var result = _parser.SplitTokens("{{FirstName}} {{LastName}}");

			Assert.Equal(3, result.Count);
			Assert.Equal("{{FirstName}}", result.ElementAt(0));
			Assert.Equal(" ", result.ElementAt(1));
			Assert.Equal("{{LastName}}", result.ElementAt(2));
		}

		[Fact]
		public void SplitTokens_WithComplexTemplate_ReturnsAllTokens()
		{
			var result = _parser.SplitTokens("Name: {{Name}}, Age: {{Age}}, Active: {{IsActive}}");

			Assert.Equal(6, result.Count);
			Assert.Equal("Name: ", result.ElementAt(0));
			Assert.Equal("{{Name}}", result.ElementAt(1));
			Assert.Equal(", Age: ", result.ElementAt(2));
			Assert.Equal("{{Age}}", result.ElementAt(3));
			Assert.Equal(", Active: ", result.ElementAt(4));
			Assert.Equal("{{IsActive}}", result.ElementAt(5));
		}

		[Fact]
		public void SplitTokens_WithEmptyString_ReturnsEmpty()
		{
			var result = _parser.SplitTokens("");

			Assert.Empty(result);
		}

		[Fact]
		public void SplitTokens_WithWhitespace_ReturnsEmpty()
		{
			var result = _parser.SplitTokens("   ");

			Assert.Empty(result);
		}

		[Fact]
		public void SplitTokens_WithNull_ReturnsEmpty()
		{
			var result = _parser.SplitTokens(null!);

			Assert.Empty(result);
		}

		[Fact]
		public void SplitTokens_WithPlaceholderAndPipe_ReturnsToken()
		{
			var result = _parser.SplitTokens("{{Date|datetime:format=yyyy-MM-dd}}");

			Assert.Single(result);
			Assert.Equal("{{Date|datetime:format=yyyy-MM-dd}}", result.First());
		}

		[Fact]
		public void SplitTokens_WithMultilineTemplate_HandlesCorrectly()
		{
			var template = @"Line 1: {{Value1}}
Line 2: {{Value2}}";

			var result = _parser.SplitTokens(template);

			Assert.Equal(4, result.Count);
			Assert.Contains("{{Value1}}", result);
			Assert.Contains("{{Value2}}", result);
		}
	}
}
