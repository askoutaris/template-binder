using TemplateBinder.Parameters;
using TemplateBinder.Pipes;
using TemplateBinder.TemplateTokens;

namespace Tests
{
	public class TokensTests
	{
		[Fact]
		public void TextToken_GetText_ReturnsText()
		{
			var token = new TextToken("Hello World");
			var parameters = new Dictionary<string, IParameter>();

			var result = token.GetText(parameters);

			Assert.Equal("Hello World", result);
		}

		[Fact]
		public void TextToken_GetText_WithEmptyParameters_ReturnsText()
		{
			var token = new TextToken("Static text");
			var parameters = new Dictionary<string, IParameter>();

			var result = token.GetText(parameters);

			Assert.Equal("Static text", result);
		}

		[Fact]
		public void PlaceholderToken_WithoutPipe_ReturnsParameterText()
		{
			var token = new PlaceholderToken("Name", null);
			var parameters = new Dictionary<string, IParameter>
			{
				{ "Name", new TextParameter("Name", "John") }
			};

			var result = token.GetText(parameters);

			Assert.Equal("John", result);
		}

		[Fact]
		public void PlaceholderToken_WithPipe_ReturnsTransformedText()
		{
			var pipe = new DateTimePipe("yyyy-MM-dd");
			var token = new PlaceholderToken("Date", pipe);
			var parameters = new Dictionary<string, IParameter>
			{
				{ "Date", new DateTimeParameter("Date", new DateTime(2024, 1, 15)) }
			};

			var result = token.GetText(parameters);

			Assert.Equal("2024-01-15", result);
		}

		[Fact]
		public void PlaceholderToken_MissingParameter_ThrowsArgumentException()
		{
			var token = new PlaceholderToken("Name", null);
			var parameters = new Dictionary<string, IParameter>();

			var ex = Assert.Throws<ArgumentException>(() => token.GetText(parameters));
			Assert.Contains("Parameter Name was not found", ex.Message);
		}

		[Fact]
		public void PlaceholderToken_WithNumberPipe_FormatsNumber()
		{
			var pipe = new NumberPipe("N2");
			var token = new PlaceholderToken("Price", pipe);
			var parameters = new Dictionary<string, IParameter>
			{
				{ "Price", new NumberParameter("Price", 1234.567m) }
			};

			var result = token.GetText(parameters);

			Assert.Equal(1234.567m.ToString("N2"), result);
		}

		[Fact]
		public void PlaceholderToken_WithBooleanPipe_ConvertsToText()
		{
			var pipe = new BooleanPipe("Yes", "No");
			var token = new PlaceholderToken("IsActive", pipe);
			var parameters = new Dictionary<string, IParameter>
			{
				{ "IsActive", new BooleanParameter("IsActive", true) }
			};

			var result = token.GetText(parameters);

			Assert.Equal("Yes", result);
		}

		[Fact]
		public void PlaceholderToken_WithNullParameter_HandlesCorrectly()
		{
			var pipe = new DateTimePipe("yyyy-MM-dd");
			var token = new PlaceholderToken("Date", pipe);
			var parameters = new Dictionary<string, IParameter>
			{
				{ "Date", new DateTimeParameter("Date", null) }
			};

			var result = token.GetText(parameters);

			Assert.Equal("Date", result);
		}
	}
}
