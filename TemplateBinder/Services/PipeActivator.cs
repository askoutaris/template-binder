using System.Collections.Frozen;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using TemplateBinder.Pipes;

namespace TemplateBinder.Services
{
	/// <summary>
	/// Factory for creating pipe instances by name with reflection-based activation.
	/// </summary>
	public interface IPipeActivator
	{
		/// <summary>
		/// Creates a pipe instance by name with the specified parameters.
		/// </summary>
		/// <param name="name">The pipe name as defined by <see cref="PipeNameAttribute"/>.</param>
		/// <param name="parameters">The parameters to pass to the pipe constructor.</param>
		/// <returns>A new pipe instance configured with the provided parameters.</returns>
		/// <exception cref="InvalidOperationException">Thrown when no pipe is registered with the specified name or when the pipe cannot be activated.</exception>
		IPipe Create(string name, NameValueCollection parameters);
	}

	/// <summary>
	/// Reflection-based pipe activator that creates pipe instances from registered types.
	/// Uses <see cref="FrozenDictionary{TKey,TValue}"/> for O(1) pipe type lookups by name.
	/// Validates that all pipe types have exactly one constructor and implement <see cref="IPipe"/>.
	/// </summary>
	public class PipeActivator : IPipeActivator
	{
		private readonly FrozenDictionary<string, Type> _pipes;

		/// <summary>
		/// Initializes a new instance of <see cref="PipeActivator"/> with the specified pipe types.
		/// </summary>
		/// <param name="pipeTypes">The collection of pipe types to register. Each type must be decorated with <see cref="PipeNameAttribute"/> and implement <see cref="IPipe"/>.</param>
		/// <exception cref="Exception">Thrown when a pipe type is missing <see cref="PipeNameAttribute"/>, does not implement <see cref="IPipe"/>, or has multiple constructors.</exception>
		public PipeActivator(IReadOnlyCollection<Type> pipeTypes)
		{
			_pipes = InitializePipeMapping(pipeTypes);
		}

		/// <summary>
		/// Creates a pipe instance by name with the specified parameters.
		/// Maps parameter names to constructor parameters case-insensitively and converts values using <see cref="TypeDescriptor"/>.
		/// </summary>
		/// <param name="pipeName">The pipe name as defined by <see cref="PipeNameAttribute"/>.</param>
		/// <param name="parameters">The parameters to pass to the pipe constructor.</param>
		/// <returns>A new pipe instance configured with the provided parameters.</returns>
		/// <exception cref="InvalidOperationException">Thrown when no pipe is registered with the specified name or when the pipe cannot be activated.</exception>
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
