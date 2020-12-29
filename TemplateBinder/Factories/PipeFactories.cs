using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TemplateBinder.Pipes;

namespace TemplateBinder.Factories
{
	public interface IPipeFactory
	{
		IPipe Create(string pipeName, NameValueCollection parameterValues);
	}
}
