using TemplateBinder.Parameters;
using TemplateBinder.Pipes;
using TemplateBinder.Services;

namespace Workbench
{
	class Program
	{
		static void Main(string[] _)
		{
			Type[] knownPipes = [
				typeof(NumberPipe),
				typeof(DateTimePipe),
				typeof(BooleanPipe),
				typeof(AgeGenerationPipe),
			];

			var placeholderParser = new PlaceholderParser();
			var pipeActivator = new PipeActivator(knownPipes);
			var tokensFactory = new TemplateTokensFactory(placeholderParser, pipeActivator);
			var parser = new TemplateParser();
			var factory = new TemplateFactory(parser, tokensFactory);

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

	[PipeName("agegeneration")]
	public class AgeGenerationPipe : IPipe
	{
		public IParameter Transform(IParameter parameter)
		{
			if (parameter is not DateTimeParameter datetime)
				throw new ArgumentException($"Date pipe can only be use with DateTime parameters. Parameter {parameter.Name} is {parameter.GetType()}");

			var age = DateTime.UtcNow - datetime.Value;

			if (age is null)
				return new TextParameter(parameter.Name, "Not born yet!");

			var years = age.Value.TotalDays / 365;

			if (years < 22)
				return new TextParameter(parameter.Name, "Gen Z");
			else
				return new TextParameter(parameter.Name, "Millenian");
		}
	}
}
