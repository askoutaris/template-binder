using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

namespace TemplateBinder
{
	public interface IBinder
	{
		string Bind(Parameter[] parameters);
	}

	class Placeholder
	{
		public string PlaceholderText { get; }
		public string ParameterName { get; }
		public IPipe Pipe { get; }

		public Placeholder(string placeholderText, string parameterName, IPipe pipe)
		{
			PlaceholderText = placeholderText;
			ParameterName = parameterName;
			Pipe = pipe;
		}
	}
}
