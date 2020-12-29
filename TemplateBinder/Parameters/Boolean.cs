namespace TemplateBinder.Parameters
{
	public abstract partial class Parameter
	{
		public class Boolean : Parameter
		{
			public bool? Value { get; }

			private Boolean()
			{

			}

			public Boolean(string name, bool? value) : base(name)
			{
				Value = value;
			}

			public override object? GetValue()
				=> Value;
		}
	}
}
