using TemplateBinder.Parameters;

namespace TemplateBinder.Binders
{
	public interface IBinder
	{
		string Bind(Parameter[] parameters);
	}
}
