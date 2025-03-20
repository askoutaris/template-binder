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

		public static IServiceCollection AddTemplateBinder(this IServiceCollection services, bool throwOnMissingParameters)
			=> services.AddTemplateBinderInternal(_pipeTypes, throwOnMissingParameters);

		public static IServiceCollection AddTemplateBinder(this IServiceCollection services, bool throwOnMissingParameters, Type[] additionalPipeTypes)
		{
			var pipeTypes = _pipeTypes
				.Concat(additionalPipeTypes)
				.Distinct()
				.ToArray();

			return services.AddTemplateBinderInternal(pipeTypes, throwOnMissingParameters);
		}

		private static IServiceCollection AddTemplateBinderInternal(this IServiceCollection services, Type[] pipeTypes, bool throwOnMissingParameters)
		{
			services.AddSingleton<IPipeFactory>(new PipeFactoryDefault(pipeTypes));
			services.AddSingleton<IBinderFactory>(ctx =>
			{
				var pipeFactory = ctx.GetRequiredService<IPipeFactory>();

				return new BinderFactoryDefault(pipeFactory, throwOnMissingParameters);
			});
			return services;
		}
	}
}
