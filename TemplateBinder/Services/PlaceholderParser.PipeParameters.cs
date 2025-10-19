using System.Collections.Specialized;

namespace TemplateBinder.Services
{
	class PipeParameters
	{
		public string Name { get; }
		public NameValueCollection Parameters { get; }

		public PipeParameters(string name, NameValueCollection parameters)
		{
			Name = name;
			Parameters = parameters;
		}
	}
}
