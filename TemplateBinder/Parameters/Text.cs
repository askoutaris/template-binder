namespace TemplateBinder.Parameters
{
	public abstract partial class Parameter
	{
		public class Text : Parameter
		{
			public string? Value { get; }

			private Text()
			{

			}

			public Text(string name, string? value) : base(name)
			{
				Value = value;
			}

			public override object? GetValue()
				=> Value;
		}
	}
}
