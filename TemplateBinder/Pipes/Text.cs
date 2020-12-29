using TemplateBinder.Attributes;
using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	[PipeName(Constants.Pipes.Text)]
	public class TextPipe : IPipe
	{
		public IParameter Transform(IParameter parameter)
		{
			var value = parameter.GetValue()?.ToString();
			return new Parameter.Text(parameter.Name, value);
		}
	}
}
