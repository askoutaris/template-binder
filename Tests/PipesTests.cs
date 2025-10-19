using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

namespace Tests
{
	public class PipesTests
	{
		[Fact]
		public void DateTimePipe_WithFormat_FormatsCorrectly()
		{
			var pipe = new DateTimePipe("yyyy-MM-dd");
			var param = new DateTimeParameter("Date", new DateTime(2024, 1, 15));

			var result = pipe.Transform(param);

			Assert.Equal("Date", result.Name);
			Assert.Equal("2024-01-15", result.GetText());
		}

		[Fact]
		public void DateTimePipe_WithNullFormat_UsesDefault()
		{
			var pipe = new DateTimePipe(null);
			var param = new DateTimeParameter("Date", new DateTime(2024, 1, 15, 10, 30, 45));

			var result = pipe.Transform(param);

			Assert.Equal("Date", result.Name);
			Assert.Contains("2024", result.GetText());
		}

		[Fact]
		public void DateTimePipe_WithNullValue_ReturnsNull()
		{
			var pipe = new DateTimePipe("yyyy-MM-dd");
			var param = new DateTimeParameter("Date", null);

			var result = pipe.Transform(param);

			Assert.Equal("Date", result.Name);
			Assert.Null(((TextParameter)result).Value);
		}

		[Fact]
		public void DateTimePipe_WithWrongParameterType_ThrowsException()
		{
			var pipe = new DateTimePipe("yyyy-MM-dd");
			var param = new TextParameter("Name", "John");

			var ex = Assert.Throws<ArgumentException>(() => pipe.Transform(param));
			Assert.Contains("Date pipe can only be use with DateTime parameters", ex.Message);
			Assert.Contains("Name", ex.Message);
		}

		[Fact]
		public void NumberPipe_WithFormat_FormatsCorrectly()
		{
			var pipe = new NumberPipe("N2");
			var param = new NumberParameter("Price", 1234.567m);

			var result = pipe.Transform(param);

			Assert.Equal("Price", result.Name);
			Assert.Equal(1234.567m.ToString("N2"), result.GetText());
		}

		[Fact]
		public void NumberPipe_WithNullFormat_UsesDefault()
		{
			var pipe = new NumberPipe(null);
			var param = new NumberParameter("Count", 42m);

			var result = pipe.Transform(param);

			Assert.Equal("Count", result.Name);
			Assert.Equal("42", result.GetText());
		}

		[Fact]
		public void NumberPipe_WithNullValue_ReturnsNull()
		{
			var pipe = new NumberPipe("N2");
			var param = new NumberParameter("Price", null);

			var result = pipe.Transform(param);

			Assert.Equal("Price", result.Name);
			Assert.Null(((TextParameter)result).Value);
		}

		[Fact]
		public void NumberPipe_WithWrongParameterType_ThrowsException()
		{
			var pipe = new NumberPipe("N2");
			var param = new TextParameter("Name", "John");

			var ex = Assert.Throws<ArgumentException>(() => pipe.Transform(param));
			Assert.Contains("Number pipe can only be use with NumberParameter", ex.Message);
			Assert.Contains("Name", ex.Message);
		}

		[Fact]
		public void BooleanPipe_WithTrue_ReturnsTrueValue()
		{
			var pipe = new BooleanPipe("Yes", "No");
			var param = new BooleanParameter("IsActive", true);

			var result = pipe.Transform(param);

			Assert.Equal("IsActive", result.Name);
			Assert.Equal("Yes", result.GetText());
		}

		[Fact]
		public void BooleanPipe_WithFalse_ReturnsFalseValue()
		{
			var pipe = new BooleanPipe("Yes", "No");
			var param = new BooleanParameter("IsActive", false);

			var result = pipe.Transform(param);

			Assert.Equal("IsActive", result.Name);
			Assert.Equal("No", result.GetText());
		}

		[Fact]
		public void BooleanPipe_WithNull_ReturnsNull()
		{
			var pipe = new BooleanPipe("Yes", "No");
			var param = new BooleanParameter("IsActive", null);

			var result = pipe.Transform(param);

			Assert.Equal("IsActive", result.Name);
			Assert.Null(((TextParameter)result).Value);
		}

		[Fact]
		public void BooleanPipe_WithWrongParameterType_ThrowsException()
		{
			var pipe = new BooleanPipe("Yes", "No");
			var param = new TextParameter("Name", "John");

			var ex = Assert.Throws<ArgumentException>(() => pipe.Transform(param));
			Assert.Contains("Boolean pipe can only be use with BooleanParameter", ex.Message);
			Assert.Contains("Name", ex.Message);
		}
	}
}
