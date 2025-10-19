using System.Collections.Frozen;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using TemplateBinder.Pipes;

namespace TemplateBinder.Services
{
	public interface IPipeActivator
	{
		IPipe Create(string name, NameValueCollection parameters);
	}

	public class PipeActivator : IPipeActivator
	{
		private readonly FrozenDictionary<string, Type> _pipes;

		public PipeActivator(IReadOnlyCollection<Type> pipeTypes)
		{
			_pipes = InitializePipeMapping(pipeTypes);
		}

		public IPipe Create(string pipeName, NameValueCollection parameters)
		{
			if (!_pipes.TryGetValue(pipeName, out var pipeType))
				throw new InvalidOperationException($"No registered pipe with name {pipeName}");

			var ctorParams = GetConstructorValues(pipeType, parameters);

			return (IPipe)(Activator.CreateInstance(pipeType, ctorParams)
				?? throw new InvalidOperationException($"Pipe with name {pipeName} could not be activated"));
		}

		private object?[] GetConstructorValues(Type pipeType, NameValueCollection parameterValues)
		{
			var ctorParameters = GetConstructorParameters(pipeType);

			var values = new List<object?>(parameterValues.Count);
			foreach (var param in ctorParameters)
			{
				var parameterName = parameterValues.AllKeys.FirstOrDefault(key => string.Compare(key, param.Name, StringComparison.OrdinalIgnoreCase) == 0);

				object? value;
				if (parameterName != null)
				{
					var parameterValue = parameterValues[parameterName]
						?? throw new InvalidOperationException($"No value for pipe parameter {parameterName}");

					value = TypeDescriptor.GetConverter(param.ParameterType).ConvertFromString(parameterValue);
				}
				else
					value = GetDefaultValue(param.ParameterType);

				values.Add(value);
			}

			return [.. values];
		}

		private ParameterInfo[] GetConstructorParameters(Type pipeType)
		{
			var ctors = pipeType.GetConstructors();

			if (ctors.Length != 1)
				throw new Exception($"Pipe {pipeType.Name} can have only one ctor");

			return [.. ctors[0].GetParameters().OrderBy(x => x.Position)];
		}

		private object? GetDefaultValue(Type type)
		{
			if (type.IsValueType)
				return Activator.CreateInstance(type);
			else
				return null;
		}

		private FrozenDictionary<string, Type> InitializePipeMapping(IReadOnlyCollection<Type> pipeTypes)
		{
			var pipes = new Dictionary<string, Type>();

			foreach (var type in pipeTypes)
			{
				var nameAttribute = type.GetCustomAttributes().FirstOrDefault(x => x.GetType() == (typeof(PipeNameAttribute)))
					?? throw new Exception($"Type {type.FullName} must be decorated with {nameof(PipeNameAttribute)} in order to be used as pipe");

				if (!typeof(IPipe).IsAssignableFrom(type))
					throw new Exception($"Type {type.FullName} must be implement {nameof(IPipe)} interface in order to be used as pipe");

				pipes.Add(((PipeNameAttribute)nameAttribute).PipeName, type);
			}

			return pipes.ToFrozenDictionary();
		}
	}
}
