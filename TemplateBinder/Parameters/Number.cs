namespace TemplateBinder.Parameters
{
	public abstract partial class Parameter
	{
		public class Number : Parameter
		{
			public decimal? Value { get; }

			private Number()
			{

			}

			public Number(string name, decimal? value) : base(name)
			{
				Value = value;
			}

			public override object? GetValue()
				=> Value;
		}
	}
}
