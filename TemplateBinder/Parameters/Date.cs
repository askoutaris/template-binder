using System;

namespace TemplateBinder.Parameters
{
	public abstract partial class Parameter
	{
		public class Date : Parameter
		{
			public DateTime? Value { get; }

			private Date()
			{

			}

			public Date(string name, DateTime? value) : base(name)
			{
				Value = value;
			}

			public override object? GetValue()
				=> Value;
		}
	}
}
