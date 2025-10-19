using Microsoft.Extensions.DependencyInjection;
using TemplateBinder.Pipes;
using TemplateBinder.Services;

namespace TemplateBinder.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods for configuring TemplateBinder services.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers TemplateBinder services with the dependency injection container.
		/// Includes built-in pipes (NumberPipe, DateTimePipe, BooleanPipe) and optional custom pipes.
		/// </summary>
		/// <param name="services">The service collection to add services to.</param>
		/// <param name="customPipeTypes">Optional custom pipe types to register in addition to built-in pipes.</param>
		/// <returns>The service collection for chaining.</returns>
		/// <example>
		/// services.AddTemplateBinder();
		/// services.AddTemplateBinder(typeof(MyCustomPipe));
		/// </example>
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
