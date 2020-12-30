# template-binder
A C# angular like text template binder

### IBinder/BinderFactory Usage
```csharp
IBinderFactory binderFactory = GetBinderFactory();

var template = @"
  The user {FirstName} {LastName} 
  born {DateOfBirth|date:format=o} 
  has logged in {LoginTimes},
  has account balance {AccountBalance|decimal:format=N2},
  {IsActive|booleantext:falseValue=no,trueValue=yes},
  {IsLockedOut|booleantext:falseValue=no,trueValue=yes}";

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

//The user David Parker 
//born 1980-08-15T12:30:00.0000000 
//has logged in 85,
//has account balance 1.750,45,
//is active: yes,
//is locked out: no
```

### ASP.NET Core

In order to use TemplateBinder with ASP.Net Core you have to install <a href="https://www.nuget.org/packages/TemplateBinder.Extensions.DependencyInjection/" target="_blank">TemplateBinder.Extensions.DependencyInjection</a> nuget package

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddTemplateBinder() // you can create your own pipes and add them here
}
```

### Microsoft Dependency Injection

In order to use TemplateBinder with Microsoft Dependency Injection you have to install <a href="https://www.nuget.org/packages/TemplateBinder.Extensions.DependencyInjection/" target="_blank">TemplateBinder.Extensions.DependencyInjection</a> nuget package

```csharp
// setup our DI
var serviceProvider = new ServiceCollection()
  .AddTemplateBinder() // you can create your own pipes and add them here
  .BuildServiceProvider();

// resolve IBinderFactory
var binderFactory = serviceProvider.GetService<IBinderFactory>();
```

### Simple
```csharp
// you can create your own pipes and add them here
var pipeTypes = new Type[] {
  typeof(BooleanTextPipe),
  typeof(DatePipe),
  typeof(DecimalPipe),
  typeof(TextPipe),
};

var pipeFactory = new PipeFactoryDefault(pipeTypes);
var binderFactory = new BinderFactoryDefault(pipeFactory);
```
