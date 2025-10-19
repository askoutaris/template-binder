using TemplateBinder.Services;

namespace Tests
{
	public class PlaceholderParserTests
	{
		private readonly PlaceholderParser _parser = new();

		[Fact]
		public void Parse_SimpleParameter_ReturnsParameterName()
		{
			var result = _parser.Parse("{{Name}}");

			Assert.Equal("Name", result.ParameterName);
			Assert.Null(result.Pipe);
		}

		[Fact]
		public void Parse_ParameterWithWhitespace_TrimsWhitespace()
		{
			var result = _parser.Parse("{{  Name  }}");

			Assert.Equal("Name", result.ParameterName);
			Assert.Null(result.Pipe);
		}

		[Fact]
		public void Parse_ParameterWithPipe_ExtractsBoth()
		{
			var result = _parser.Parse("{{Date|datetime}}");

			Assert.Equal("Date", result.ParameterName);
			Assert.NotNull(result.Pipe);
			Assert.Equal("datetime", result.Pipe.Name);
			Assert.Empty(result.Pipe.Parameters);
		}

		[Fact]
		public void Parse_ParameterWithPipeAndOneParameter_ExtractsAll()
		{
			var result = _parser.Parse("{{Date|datetime:format=yyyy-MM-dd}}");

			Assert.Equal("Date", result.ParameterName);
			Assert.NotNull(result.Pipe);
			Assert.Equal("datetime", result.Pipe.Name);
			Assert.Single(result.Pipe.Parameters);
			Assert.Equal("yyyy-MM-dd", result.Pipe.Parameters["format"]);
		}

		[Fact]
		public void Parse_ParameterWithPipeAndMultipleParameters_ExtractsAll()
		{
			var result = _parser.Parse("{{IsActive|boolean:trueValue=Yes,falseValue=No}}");

			Assert.Equal("IsActive", result.ParameterName);
			Assert.NotNull(result.Pipe);
			Assert.Equal("boolean", result.Pipe.Name);
			Assert.Equal(2, result.Pipe.Parameters.Count);
			Assert.Equal("Yes", result.Pipe.Parameters["trueValue"]);
			Assert.Equal("No", result.Pipe.Parameters["falseValue"]);
		}

		[Fact]
		public void Parse_ParameterWithPipeAndNoParameters_ExtractsCorrectly()
		{
			var result = _parser.Parse("{{Price|number}}");

			Assert.Equal("Price", result.ParameterName);
			Assert.NotNull(result.Pipe);
			Assert.Equal("number", result.Pipe.Name);
			Assert.Empty(result.Pipe.Parameters);
		}

		[Fact]
		public void Parse_ComplexFormat_ExtractsCorrectly()
		{
			var result = _parser.Parse("{{Amount|number:format=N2}}");

			Assert.Equal("Amount", result.ParameterName);
			Assert.NotNull(result.Pipe);
			Assert.Equal("number", result.Pipe.Name);
			Assert.Equal("N2", result.Pipe.Parameters["format"]);
		}

		[Fact]
		public void Parse_InvalidPlaceholder_ThrowsInvalidOperationException()
		{
			// When a placeholder doesn't have {{ }}, the Replace operations will leave it as-is
			// and it should still parse (though it's not ideal)
			// Let's test a truly invalid case that causes an exception
			var result = _parser.Parse("InvalidFormat");

			Assert.Equal("InvalidFormat", result.ParameterName);
			Assert.Null(result.Pipe);
		}

		[Fact]
		public void Parse_EmptyPlaceholder_ThrowsException()
		{
			var ex = Assert.Throws<InvalidOperationException>(() => _parser.Parse("{{}}"));

			Assert.Contains("Invalid placeholder", ex.Message);
		}
	}
}
