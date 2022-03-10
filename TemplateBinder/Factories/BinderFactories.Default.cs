using TemplateBinder.Binders;

namespace TemplateBinder.Factories
{
	public class BinderFactoryDefault : IBinderFactory
	{
		private readonly IPipeFactory _pipeFactory;

		public BinderFactoryDefault(IPipeFactory pipeFactory)
		{
			_pipeFactory = pipeFactory;
		}

		public IBinder Create(string template)
			=> new BinderDefault(_pipeFactory, template);
	}
}
