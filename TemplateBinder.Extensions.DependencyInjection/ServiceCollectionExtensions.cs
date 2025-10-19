using Microsoft.Extensions.DependencyInjection;
using TemplateBinder.Pipes;
using TemplateBinder.Services;

namespace TemplateBinder.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTemplateBinder(this IServiceCollection services, params Type[] customPipeTypes)
		{
			Type[] buitInPipeType = [
				typeof(NumberPipe),
				typeof(DateTimePipe),
				typeof(BooleanPipe)
			];

			services.AddSingleton<IPipeActivator>(new PipeActivator([.. buitInPipeType, .. customPipeTypes]));
			services.AddSingleton<ITemplateTokensFactory, TemplateTokensFactory>();
			services.AddSingleton<IPlaceholderParser, PlaceholderParser>();
			services.AddSingleton<ITemplateParser, TemplateParser>();
			services.AddSingleton<ITemplateFactory, TemplateFactory>();

			return services;
		}
	}
}
