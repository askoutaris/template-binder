namespace TemplateBinder.Parameters
{
	public interface IParameter
	{
		string Name { get; }
		object? GetValue();
	}

	public abstract partial class Parameter : IParameter
	{
		public string Name { get; }

		protected Parameter()
		{
			Name = string.Empty;
		}

		protected Parameter(string name)
		{
			Name = name;
		}

		public abstract object? GetValue();
	}
}
