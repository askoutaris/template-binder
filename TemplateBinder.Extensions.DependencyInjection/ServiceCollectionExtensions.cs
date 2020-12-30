using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TemplateBinder.Factories;
using TemplateBinder.Pipes;

namespace TemplateBinder.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		private static readonly Type[] _pipeTypes = new Type[] {
			typeof(BooleanTextPipe),
			typeof(DatePipe),
			typeof(DecimalPipe),
			typeof(TextPipe),
		};

		public static IServiceCollection AddTemplateBinder(this IServiceCollection services)
			=> services.AddTemplateBinderInternal(_pipeTypes);

		public static IServiceCollection AddTemplateBinder(this IServiceCollection services, Type[] additionalPipeTypes)
		{
			var pipeTypes = _pipeTypes
				.Concat(additionalPipeTypes)
				.Distinct()
				.ToArray();

			return services.AddTemplateBinderInternal(pipeTypes);
		}

		private static IServiceCollection AddTemplateBinderInternal(this IServiceCollection services, Type[] pipeTypes)
		{
			services.AddSingleton<IPipeFactory>(new PipeFactoryDefault(pipeTypes));
			services.AddSingleton<IBinderFactory, BinderFactoryDefault>();
			return services;
		}
	}
}
