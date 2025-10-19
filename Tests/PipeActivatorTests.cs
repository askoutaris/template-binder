using System.Collections.Specialized;
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;
using TemplateBinder.Services;

namespace Tests
{
	public class PipeActivatorTests
	{
		[Fact]
		public void Create_DateTimePipe_WithFormat_CreatesCorrectly()
		{
			var activator = new PipeActivator([typeof(DateTimePipe)]);
			var parameters = new NameValueCollection { { "format", "yyyy-MM-dd" } };

			var pipe = activator.Create("datetime", parameters);

			Assert.NotNull(pipe);
			Assert.IsType<DateTimePipe>(pipe);

			// Verify it works
			var result = pipe.Transform(new DateTimeParameter("Date", new DateTime(2024, 1, 15)));
			Assert.Equal("2024-01-15", result.GetText());
		}

		[Fact]
		public void Create_NumberPipe_WithFormat_CreatesCorrectly()
		{
			var activator = new PipeActivator([typeof(NumberPipe)]);
			var parameters = new NameValueCollection { { "format", "N2" } };

			var pipe = activator.Create("number", parameters);

			Assert.NotNull(pipe);
			Assert.IsType<NumberPipe>(pipe);

			// Verify it works
			var result = pipe.Transform(new NumberParameter("Price", 1234.567m));
			Assert.Equal(1234.567m.ToString("N2"), result.GetText());
		}

		[Fact]
		public void Create_BooleanPipe_WithTwoParameters_CreatesCorrectly()
		{
			var activator = new PipeActivator([typeof(BooleanPipe)]);
			var parameters = new NameValueCollection
			{
				{ "trueValue", "Yes" },
				{ "falseValue", "No" }
			};

			var pipe = activator.Create("boolean", parameters);

			Assert.NotNull(pipe);
			Assert.IsType<BooleanPipe>(pipe);

			// Verify it works
			var result = pipe.Transform(new BooleanParameter("IsActive", true));
			Assert.Equal("Yes", result.GetText());
		}

		[Fact]
		public void Create_WithoutParameters_UsesDefaultValues()
		{
			var activator = new PipeActivator([typeof(DateTimePipe)]);
			var parameters = new NameValueCollection();

			var pipe = activator.Create("datetime", parameters);

			Assert.NotNull(pipe);
			Assert.IsType<DateTimePipe>(pipe);
		}

		[Fact]
		public void Create_CaseInsensitiveParameterMatching_Works()
		{
			var activator = new PipeActivator([typeof(DateTimePipe)]);
			var parameters = new NameValueCollection { { "FORMAT", "yyyy-MM-dd" } };

			var pipe = activator.Create("datetime", parameters);

			var result = pipe.Transform(new DateTimeParameter("Date", new DateTime(2024, 1, 15)));
			Assert.Equal("2024-01-15", result.GetText());
		}

		[Fact]
		public void Create_UnknownPipeName_ThrowsInvalidOperationException()
		{
			var activator = new PipeActivator([typeof(DateTimePipe)]);
			var parameters = new NameValueCollection();

			var ex = Assert.Throws<InvalidOperationException>(() =>
				activator.Create("unknown", parameters));

			Assert.Contains("No registered pipe with name unknown", ex.Message);
		}

		[Fact]
		public void Create_MultiplePipeTypes_RegistersAll()
		{
			var activator = new PipeActivator([
				typeof(DateTimePipe),
				typeof(NumberPipe),
				typeof(BooleanPipe)
			]);

			var datePipe = activator.Create("datetime", []);
			var numberPipe = activator.Create("number", []);
			var booleanPipe = activator.Create("boolean", new NameValueCollection
			{
				{ "trueValue", "T" },
				{ "falseValue", "F" }
			});

			Assert.IsType<DateTimePipe>(datePipe);
			Assert.IsType<NumberPipe>(numberPipe);
			Assert.IsType<BooleanPipe>(booleanPipe);
		}

		[Fact]
		public void Constructor_TypeWithoutPipeNameAttribute_ThrowsException()
		{
			var ex = Assert.Throws<Exception>(() =>
				new PipeActivator([typeof(InvalidPipeWithoutAttribute)]));

			Assert.Contains("must be decorated with PipeNameAttribute", ex.Message);
		}

		[Fact]
		public void Constructor_TypeNotImplementingIPipe_ThrowsException()
		{
			var ex = Assert.Throws<Exception>(() =>
				new PipeActivator([typeof(InvalidTypeNotIPipe)]));

			Assert.Contains("must be implement IPipe interface", ex.Message);
		}

		// Test helper classes
		private class InvalidPipeWithoutAttribute : IPipe
		{
			public IParameter Transform(IParameter parameter) => parameter;
		}

		[PipeName("invalid")]
		private class InvalidTypeNotIPipe
		{
		}
	}
}
