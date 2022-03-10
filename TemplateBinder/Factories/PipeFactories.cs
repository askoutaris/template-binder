using System.Collections.Specialized;
using TemplateBinder.Pipes;

namespace TemplateBinder.Factories
{
	public interface IPipeFactory
	{
		IPipe Create(string pipeName, NameValueCollection parameterValues);
	}
}
