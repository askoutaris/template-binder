using Microsoft.Extensions.DependencyInjection;
using TemplateBinder.Extensions.DependencyInjection;
using TemplateBinder.Parameters;
using TemplateBinder.Services;

namespace Workbench
{
	class Program
	{
		static void Main(string[] _)
		{
			// Setup Dependency Injection
			var services = new ServiceCollection();
			services.AddTemplateBinder(typeof(AgeGenerationPipe));

			var serviceProvider = services.BuildServiceProvider();

			// Resolve ITemplateFactory from DI container
			var factory = serviceProvider.GetRequiredService<ITemplateFactory>();

			var templateStr = @"
				User Report
				-----------
				Name: {{FirstName}} {{LastName}}
				Born: {{DateOfBirth|datetime:format=yyyy-MM-dd}}
				Age Generation: {{DateOfBirth|agegeneration}}
				Login Count: {{LoginTimes}}
				Balance: ${{AccountBalance|number:format=N2}}
				Active: {{IsActive|boolean:trueValue=Yes,falseValue=No}}
				Locked: {{IsLockedOut|boolean:trueValue=Yes,falseValue=No}}
			";

			var template = factory.Create(templateStr);

			var parameters = new IParameter[] {
				new TextParameter("FirstName", "David"),
				new TextParameter("LastName", "Parker"),
				new DateTimeParameter("DateOfBirth", new DateTime(1980, 08, 15)),
				new NumberParameter("LoginTimes", 85),
				new NumberParameter("AccountBalance", 1750.45M),
				new BooleanParameter("IsActive", true),
				new BooleanParameter("IsLockedOut", false)
			};

			var message = template.Bind(parameters);

			Console.WriteLine(message);

			Console.ReadLine();
		}
	}
}
