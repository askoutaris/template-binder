using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	public interface IPipe
	{
		IParameter Transform(IParameter parameter);
	}
}
