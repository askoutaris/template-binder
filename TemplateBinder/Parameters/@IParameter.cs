namespace TemplateBinder.Parameters
{
	public interface IParameter
	{
		public string Name { get; }
		public string GetText();
	}
}
