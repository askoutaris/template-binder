﻿using TemplateBinder.Binders;

namespace TemplateBinder.Factories
{
	public interface IBinderFactory
	{
		IBinder Create(string template);
	}
}
