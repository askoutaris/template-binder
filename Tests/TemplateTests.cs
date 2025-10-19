using TemplateBinder.Parameters;
using TemplateBinder.Pipes;
using TemplateBinder.Services;

namespace Tests
{
	public class TemplateTests
	{
		private readonly TemplateFactory _factory;

		public TemplateTests()
		{
			var parser = new TemplateParser();
			var placeholderParser = new PlaceholderParser();
			var pipeActivator = new PipeActivator([
				typeof(DateTimePipe),
				typeof(NumberPipe),
				typeof(BooleanPipe)
			]);
			var tokensFactory = new TemplateTokensFactory(placeholderParser, pipeActivator);
			_factory = new TemplateFactory(parser, tokensFactory);
		}

		[Fact]
		public void Bind_SimpleText_ReturnsText()
		{
			var template = _factory.Create("Hello World");
			var result = template.Bind([]);

			Assert.Equal("Hello World", result);
		}

		[Fact]
		public void Bind_SingleParameter_ReplacesParameter()
		{
			var template = _factory.Create("Hello {{Name}}");
			var parameters = new IParameter[]
			{
				new TextParameter("Name", "John")
			};

			var result = template.Bind(parameters);

			Assert.Equal("Hello John", result);
		}

		[Fact]
		public void Bind_MultipleParameters_ReplacesAll()
		{
			var template = _factory.Create("{{FirstName}} {{LastName}}");
			var parameters = new IParameter[]
			{
				new TextParameter("FirstName", "John"),
				new TextParameter("LastName", "Doe")
			};

			var result = template.Bind(parameters);

			Assert.Equal("John Doe", result);
		}

		[Fact]
		public void Bind_WithDateTimePipe_FormatsDate()
		{
			var template = _factory.Create("Date: {{Date|datetime:format=yyyy-MM-dd}}");
			var parameters = new IParameter[]
			{
				new DateTimeParameter("Date", new DateTime(2024, 1, 15))
			};

			var result = template.Bind(parameters);

			Assert.Equal("Date: 2024-01-15", result);
		}

		[Fact]
		public void Bind_WithNumberPipe_FormatsNumber()
		{
			var template = _factory.Create("Price: ${{Price|number:format=N2}}");
			var parameters = new IParameter[]
			{
				new NumberParameter("Price", 1234.567m)
			};

			var result = template.Bind(parameters);

			Assert.Equal($"Price: ${1234.567m.ToString("N2")}", result);
		}

		[Fact]
		public void Bind_WithBooleanPipe_ConvertsBoolean()
		{
			var template = _factory.Create("Active: {{IsActive|boolean:trueValue=Yes,falseValue=No}}");
			var parameters = new IParameter[]
			{
				new BooleanParameter("IsActive", true)
			};

			var result = template.Bind(parameters);

			Assert.Equal("Active: Yes", result);
		}

		[Fact]
		public void Bind_ComplexTemplate_WorksCorrectly()
		{
			var templateStr = @"User Report
Name: {{FirstName}} {{LastName}}
Born: {{DateOfBirth|datetime:format=yyyy-MM-dd}}
Balance: ${{Balance|number:format=N2}}
Active: {{IsActive|boolean:trueValue=Yes,falseValue=No}}";

			var template = _factory.Create(templateStr);
			var parameters = new IParameter[]
			{
				new TextParameter("FirstName", "John"),
				new TextParameter("LastName", "Doe"),
				new DateTimeParameter("DateOfBirth", new DateTime(1990, 5, 15)),
				new NumberParameter("Balance", 1234.56m),
				new BooleanParameter("IsActive", true)
			};

			var result = template.Bind(parameters);

			Assert.Contains("John Doe", result);
			Assert.Contains("1990-05-15", result);
			Assert.Contains($"${1234.56m.ToString("N2")}", result);
			Assert.Contains("Yes", result);
		}

		[Fact]
		public void Bind_MissingParameter_ThrowsException()
		{
			var template = _factory.Create("Hello {{Name}}");

			var ex = Assert.Throws<ArgumentException>(() => template.Bind([]));
			Assert.Contains("Parameter Name was not found", ex.Message);
		}

		[Fact]
		public void Bind_WithoutPipe_UsesParameterDefault()
		{
			var template = _factory.Create("Number: {{Count}}");
			var parameters = new IParameter[]
			{
				new NumberParameter("Count", 42m)
			};

			var result = template.Bind(parameters);

			Assert.Equal("Number: 42", result);
		}

		[Fact]
		public void Bind_MultilineTemplate_HandlesCorrectly()
		{
			var templateStr = @"Line 1: {{Value1}}
Line 2: {{Value2}}
Line 3: {{Value3}}";

			var template = _factory.Create(templateStr);
			var parameters = new IParameter[]
			{
				new TextParameter("Value1", "First"),
				new TextParameter("Value2", "Second"),
				new TextParameter("Value3", "Third")
			};

			var result = template.Bind(parameters);

			Assert.Contains("Line 1: First", result);
			Assert.Contains("Line 2: Second", result);
			Assert.Contains("Line 3: Third", result);
		}

		[Fact]
		public void Bind_SameParameterMultipleTimes_ReplacesAll()
		{
			var template = _factory.Create("{{Name}} is awesome! Yes, {{Name}} is really awesome!");
			var parameters = new IParameter[]
			{
				new TextParameter("Name", "John")
			};

			var result = template.Bind(parameters);

			Assert.Equal("John is awesome! Yes, John is really awesome!", result);
		}

		[Fact]
		public void Bind_EmptyTemplate_ReturnsEmpty()
		{
			var template = _factory.Create("");

			var result = template.Bind([]);

			Assert.Equal("", result);
		}

		[Fact]
		public void Bind_NullParameterValue_UsesParameterName()
		{
			var template = _factory.Create("Name: {{Name}}");
			var parameters = new IParameter[]
			{
				new TextParameter("Name", null)
			};

			var result = template.Bind(parameters);

			Assert.Equal("Name: Name", result);
		}

		[Fact]
		public void Bind_ParameterWithPipeAndNullValue_HandlesCorrectly()
		{
			var template = _factory.Create("Date: {{Date|datetime:format=yyyy-MM-dd}}");
			var parameters = new IParameter[]
			{
				new DateTimeParameter("Date", null)
			};

			var result = template.Bind(parameters);

			Assert.Equal("Date: Date", result);
		}
	}
}
