using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TemplateBinder.Attributes;
using TemplateBinder.Pipes;

namespace TemplateBinder.Factories
{
	public class PipeFactoryDefault : IPipeFactory
	{
		private readonly Dictionary<string, Type> _pipes;

		public PipeFactoryDefault(params Type[] pipeTypes)
		{
			_pipes = new Dictionary<string, Type>();

			InitializePipeMapping(pipeTypes);
		}

		public IPipe Create(string pipeName, NameValueCollection parameterValues)
		{
			var pipeType = _pipes[pipeName];
			var ctorParams = GetConstructoreValues(pipeType, parameterValues);
			return (IPipe)Activator.CreateInstance(pipeType, ctorParams);
		}

		private object?[] GetConstructoreValues(Type pipeType, NameValueCollection parameterValues)
		{
			var ctorParameters = GetConstructorParameters(pipeType);

			var values = new List<object?>(parameterValues.Count);
			foreach (var param in ctorParameters)
			{
				var valueKey = parameterValues.AllKeys.FirstOrDefault(key => string.Compare(key, param.Name, StringComparison.OrdinalIgnoreCase) == 0);

				object? value;
				if (valueKey != null)
					value = TypeDescriptor.GetConverter(param.ParameterType).ConvertFromString(parameterValues[valueKey]);
				else
					value = GetDefaultValue(param.ParameterType);

				values.Add(value);
			}

			return values.ToArray();
		}

		private ParameterInfo[] GetConstructorParameters(Type pipeType)
		{
			var ctors = pipeType.GetConstructors();
			if (ctors.Length != 1)
				throw new Exception($"Pipe {pipeType.Name} can have only one ctor");

			return ctors[0].GetParameters()
				.OrderBy(x => x.Position)
				.ToArray();
		}

		private object? GetDefaultValue(Type type)
		{
			if (type.IsValueType)
				return Activator.CreateInstance(type);
			else
				return null;
		}

		private void InitializePipeMapping(Type[] pipeTypes)
		{
			foreach (var type in pipeTypes)
			{
				var nameAttribute = type.GetCustomAttributes().FirstOrDefault(x => x.GetType() == (typeof(PipeNameAttribute)));
				if (nameAttribute == null)
					throw new Exception($"Type {type.FullName} must be decorated with {nameof(PipeNameAttribute)} in order to be used as pipe");
				if (!typeof(IPipe).IsAssignableFrom(type))
					throw new Exception($"Type {type.FullName} must be implement {nameof(IPipe)} interface in order to be used as pipe");

				_pipes.Add(((PipeNameAttribute)nameAttribute).PipeName, type);
			}
		}
	}
}
