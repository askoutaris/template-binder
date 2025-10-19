namespace TemplateBinder.Services
{
	public class PlaceholderParameters
	{
		public string ParameterName { get; }
		public PipeParameters? Pipe { get; }

		public PlaceholderParameters(string parameterName, PipeParameters? pipe)
		{
			ParameterName = parameterName;
			Pipe = pipe;
		}
	}
}
