using System;

namespace TemplateBinder.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PipeNameAttribute : Attribute
	{
		public string PipeName { get; }

		public PipeNameAttribute(string pipeName)
		{
			PipeName = pipeName;
		}
	}
}
