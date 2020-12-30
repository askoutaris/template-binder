using System;
using Microsoft.Extensions.DependencyInjection;
using TemplateBinder.Extensions.DependencyInjection;
using TemplateBinder.Factories;
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

namespace Workbench
{
	class Program
	{
		static void Main(string[] args)
		{
			DependencyInjectionExample();
			SimpleExample();
		}

		private static void DependencyInjectionExample()
		{
			//setup our DI
			var serviceProvider = new ServiceCollection()
				.AddTemplateBinder()
				.BuildServiceProvider();

			//do the actual work here
			var binderFactory = serviceProvider.GetService<IBinderFactory>();
			BindText(binderFactory);
		}

		private static void SimpleExample()
		{
			var pipeTypes = new Type[] {
				typeof(BooleanTextPipe),
				typeof(DatePipe),
				typeof(DecimalPipe),
				typeof(TextPipe),
			};

			var pipeFactory = new PipeFactoryDefault(pipeTypes);
			var binderFactory = new BinderFactoryDefault(pipeFactory);
			BindText(binderFactory);
		}

		private static void BindText(IBinderFactory binderFactory)
		{
			var template = @"
The user {FirstName} {LastName} 
born {DateOfBirth|date:format=o} 
has logged in {LoginTimes},
has account balance {AccountBalance|decimal:format=N2},
is active: {IsActive|booleantext:falseValue=no,trueValue=yes},
is locked out: {IsLockedOut|booleantext:falseValue=no,trueValue=yes}";

			var binder = binderFactory.Create(template);

			var parameters = new Parameter[] {
				new Parameter.Text("FirstName", "David"),
				new Parameter.Text("LastName", "Parker"),
				new Parameter.Date("DateOfBirth", new DateTime(1980,08,15,12,30,0)),
				new Parameter.Number("LoginTimes", 85),
				new Parameter.Number("AccountBalance", 1750.45M),
				new Parameter.Boolean("IsActive",true),
				new Parameter.Boolean("IsLockedOut", false)
			};

			var text = binder.Bind(parameters);

			Console.WriteLine(text);
			Console.ReadLine();
		}
	}
}
