using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

namespace Workbench
{
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
