using TemplateBinder.Binders;

namespace TemplateBinder.Factories
{
	public class BinderFactoryDefault : IBinderFactory
	{
		private readonly IPipeFactory _pipeFactory;
		private readonly bool _throwOnMissingParameters;

		public BinderFactoryDefault(IPipeFactory pipeFactory, bool throwOnMissingParameters)
		{
			_pipeFactory = pipeFactory;
			_throwOnMissingParameters = throwOnMissingParameters;
		}

		public IBinder Create(string template)
			=> new BinderDefault(_pipeFactory, template, _throwOnMissingParameters);
	}
}
