using TemplateBinder.Parameters;

namespace Tests
{
	public class ParametersTests
	{
		[Fact]
		public void TextParameter_WithValue_ReturnsValue()
		{
			var param = new TextParameter("Name", "John");

			Assert.Equal("Name", param.Name);
			Assert.Equal("John", param.Value);
			Assert.Equal("John", param.GetText());
		}

		[Fact]
		public void TextParameter_WithNull_ReturnsName()
		{
			var param = new TextParameter("Name", null);

			Assert.Equal("Name", param.Name);
			Assert.Null(param.Value);
			Assert.Equal("Name", param.GetText());
		}

		[Fact]
		public void NumberParameter_WithValue_ReturnsValue()
		{
			var param = new NumberParameter("Age", 25.5m);

			Assert.Equal("Age", param.Name);
			Assert.Equal(25.5m, param.Value);
			Assert.Equal(25.5m.ToString(), param.GetText());
		}

		[Fact]
		public void NumberParameter_WithNull_ReturnsName()
		{
			var param = new NumberParameter("Age", null);

			Assert.Equal("Age", param.Name);
			Assert.Null(param.Value);
			Assert.Equal("Age", param.GetText());
		}

		[Fact]
		public void DateTimeParameter_WithValue_ReturnsIso8601()
		{
			var date = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
			var param = new DateTimeParameter("CreatedAt", date);

			Assert.Equal("CreatedAt", param.Name);
			Assert.Equal(date, param.Value);
			Assert.Equal("2024-01-15T10:30:45.0000000Z", param.GetText());
		}

		[Fact]
		public void DateTimeParameter_WithNull_ReturnsName()
		{
			var param = new DateTimeParameter("CreatedAt", null);

			Assert.Equal("CreatedAt", param.Name);
			Assert.Null(param.Value);
			Assert.Equal("CreatedAt", param.GetText());
		}

		[Fact]
		public void BooleanParameter_WithTrue_ReturnsTrue()
		{
			var param = new BooleanParameter("IsActive", true);

			Assert.Equal("IsActive", param.Name);
			Assert.True(param.Value);
			Assert.Equal("True", param.GetText());
		}

		[Fact]
		public void BooleanParameter_WithFalse_ReturnsFalse()
		{
			var param = new BooleanParameter("IsActive", false);

			Assert.Equal("IsActive", param.Name);
			Assert.False(param.Value);
			Assert.Equal("False", param.GetText());
		}

		[Fact]
		public void BooleanParameter_WithNull_ReturnsName()
		{
			var param = new BooleanParameter("IsActive", null);

			Assert.Equal("IsActive", param.Name);
			Assert.Null(param.Value);
			Assert.Equal("IsActive", param.GetText());
		}
	}
}
