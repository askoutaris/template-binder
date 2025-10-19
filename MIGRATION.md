# Migration Guide: v2.x → v3.0.0

This guide helps you migrate from TemplateBinder v2.x to v3.0.0.

## ⚠️ Important Notice

Version 3.0.0 is a **complete rewrite** with breaking changes. This is not a drop-in replacement for v2.x.

**Major Changes:**
- Template syntax changed from `{...}` to `{{...}}`
- Complete API redesign (IBinder → ITemplate)
- Parameter types changed from classes to structs
- Pipe names changed
- Requires .NET 8.0 (was .NET Standard 2.0)

## Quick Reference

| Aspect | v2.x | v3.0.0 |
|--------|------|--------|
| Template Syntax | `{Name}` | `{{Name}}` |
| Main Interface | `IBinder` | `ITemplate` |
| Factory Interface | `IBinderFactory` | `ITemplateFactory` |
| Text Parameter | `Parameter.Text` | `TextParameter` |
| Number Parameter | `Parameter.Number` | `NumberParameter` |
| Date Parameter | `Parameter.Date` | `DateTimeParameter` |
| Boolean Parameter | `Parameter.Boolean` | `BooleanParameter` |
| Date Pipe | `date` | `datetime` |
| Number Pipe | `decimal` | `number` |
| Boolean Pipe | `booleantext` | `boolean` |
| DI Method | `AddTemplateBinder(bool)` | `AddTemplateBinder(params Type[])` |
| Target Framework | .NET Standard 2.0 | .NET 8.0 |

## Step-by-Step Migration

### 1. Update Template Strings

**v2.x:**
```csharp
var template = "Hello {FirstName} {LastName}!";
```

**v3.0.0:**
```csharp
var template = "Hello {{FirstName}} {{LastName}}!";
```

**Find & Replace:**
- `{` → `{{`
- `}` → `}}`

### 2. Update Pipe Names in Templates

**v2.x:**
```csharp
var template = @"
    Date: {Date|date:format=yyyy-MM-dd}
    Price: {Price|decimal:format=N2}
    Active: {IsActive|booleantext:trueValue=Yes,falseValue=No}
";
```

**v3.0.0:**
```csharp
var template = @"
    Date: {{Date|datetime:format=yyyy-MM-dd}}
    Price: {{Price|number:format=N2}}
    Active: {{IsActive|boolean:trueValue=Yes,falseValue=No}}
";
```

**Find & Replace:**
- `|date:` → `|datetime:`
- `|decimal:` → `|number:`
- `|booleantext:` → `|boolean:`

### 3. Update Parameter Creation

**v2.x:**
```csharp
var parameters = new Parameter[] {
    new Parameter.Text("FirstName", "John"),
    new Parameter.Number("Age", 25),
    new Parameter.Date("BirthDate", new DateTime(1990, 1, 15)),
    new Parameter.Boolean("IsActive", true)
};
```

**v3.0.0:**
```csharp
var parameters = new IParameter[] {
    new TextParameter("FirstName", "John"),
    new NumberParameter("Age", 25),
    new DateTimeParameter("BirthDate", new DateTime(1990, 1, 15)),
    new BooleanParameter("IsActive", true)
};
```

### 4. Update Dependency Injection

**v2.x:**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddTemplateBinder(
        throwOnMissingParameters: true,
        additionalPipeTypes: new[] { typeof(MyCustomPipe) }
    );
}

public class MyService
{
    private readonly IBinderFactory _binderFactory;

    public MyService(IBinderFactory binderFactory)
    {
        _binderFactory = binderFactory;
    }

    public string Generate()
    {
        var binder = _binderFactory.Create("{Name}");
        return binder.Bind(new[] { new Parameter.Text("Name", "John") });
    }
}
```

**v3.0.0:**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddTemplateBinder(typeof(MyCustomPipe));
}

public class MyService
{
    private readonly ITemplate _template;

    public MyService(ITemplateFactory templateFactory)
    {
        // ✅ GOOD: Create template once in constructor for better performance
        _template = templateFactory.Create("{{Name}}");
    }

    public string Generate(string name)
    {
        // ✅ GOOD: Reuse the same template with different parameters
        return _template.Bind(new IParameter[] { new TextParameter("Name", name) });
    }
}
```

**⚠️ Performance Note:** Always create templates once (in constructor) and reuse them. Template parsing is expensive, binding is fast.

### 5. Update Manual Setup (Without DI)

**v2.x:**
```csharp
var pipeTypes = new[] {
    typeof(TextPipe),
    typeof(DatePipe),
    typeof(DecimalPipe),
    typeof(BooleanTextPipe)
};

var pipeFactory = new PipeFactoryDefault(pipeTypes);
var binderFactory = new BinderFactoryDefault(pipeFactory, throwOnMissingParameters: true);

var binder = binderFactory.Create("{Name}");
var result = binder.Bind(new[] { new Parameter.Text("Name", "John") });
```

**v3.0.0:**
```csharp
var parser = new TemplateParser();
var placeholderParser = new PlaceholderParser();
var pipeActivator = new PipeActivator([
    typeof(DateTimePipe),
    typeof(NumberPipe),
    typeof(BooleanPipe)
]);
var tokensFactory = new TemplateTokensFactory(placeholderParser, pipeActivator);
var templateFactory = new TemplateFactory(parser, tokensFactory);

var template = templateFactory.Create("{{Name}}");
var result = template.Bind([new TextParameter("Name", "John")]);
```

### 6. Update Custom Pipes

**v2.x:**
```csharp
using TemplateBinder.Attributes;
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

[PipeName("uppercase")]
public class UppercasePipe : IPipe
{
    public UppercasePipe() { }

    public IParameter Transform(IParameter parameter)
    {
        if (parameter is not Parameter.Text text)
            throw new ArgumentException("Requires Text parameter");

        var value = text.Value?.ToUpperInvariant();
        return new Parameter.Text(parameter.Name, value);
    }
}
```

**v3.0.0:**
```csharp
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

[PipeName("uppercase")]
public class UppercasePipe : IPipe
{
    public UppercasePipe() { }

    public IParameter Transform(IParameter parameter)
    {
        if (parameter is not TextParameter text)
            throw new ArgumentException("Requires TextParameter");

        var value = text.Value?.ToUpperInvariant();
        return new TextParameter(parameter.Name, value);
    }
}
```

### 7. Error Handling Changes

**v2.x:**
```csharp
// Could configure silent fallback for missing parameters
var binderFactory = new BinderFactoryDefault(pipeFactory, throwOnMissingParameters: false);
var binder = binderFactory.Create("{Name} {Missing}");
var result = binder.Bind(new[] { new Parameter.Text("Name", "John") });
// Result: "John {Missing}" (missing parameter left as-is)
```

**v3.0.0:**
```csharp
// Always throws on missing parameters
var template = templateFactory.Create("{{Name}} {{Missing}}");
try
{
    var result = template.Bind([new TextParameter("Name", "John")]);
}
catch (ArgumentException ex)
{
    // ex.Message: "Parameter Missing was not found"
    // Must handle explicitly or provide all required parameters
}
```

## Complete Example: Before & After

### v2.x Complete Example

```csharp
using TemplateBinder.Binders;
using TemplateBinder.Factories;
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

// Setup
var pipeTypes = new Type[] {
    typeof(BooleanTextPipe),
    typeof(DatePipe),
    typeof(DecimalPipe),
    typeof(TextPipe)
};

var pipeFactory = new PipeFactoryDefault(pipeTypes);
var binderFactory = new BinderFactoryDefault(pipeFactory, throwOnMissingParameters: true);

// Create template
var templateStr = @"
User Report
-----------
Name: {FirstName} {LastName}
Born: {DateOfBirth|date:format=yyyy-MM-dd}
Balance: ${AccountBalance|decimal:format=N2}
Active: {IsActive|booleantext:trueValue=Yes,falseValue=No}
";

var binder = binderFactory.Create(templateStr);

// Bind parameters
var parameters = new Parameter[] {
    new Parameter.Text("FirstName", "David"),
    new Parameter.Text("LastName", "Parker"),
    new Parameter.Date("DateOfBirth", new DateTime(1980, 08, 15)),
    new Parameter.Number("AccountBalance", 1750.45M),
    new Parameter.Boolean("IsActive", true)
};

var result = binder.Bind(parameters);
Console.WriteLine(result);
```

### v3.0.0 Complete Example

```csharp
using Microsoft.Extensions.DependencyInjection;
using TemplateBinder.Extensions.DependencyInjection;
using TemplateBinder.Parameters;
using TemplateBinder.Services;

// Setup with DI
var services = new ServiceCollection();
services.AddTemplateBinder();
var serviceProvider = services.BuildServiceProvider();
var templateFactory = serviceProvider.GetRequiredService<ITemplateFactory>();

// Create template (note {{ }} syntax and pipe name changes)
var templateStr = @"
User Report
-----------
Name: {{FirstName}} {{LastName}}
Born: {{DateOfBirth|datetime:format=yyyy-MM-dd}}
Balance: ${{AccountBalance|number:format=N2}}
Active: {{IsActive|boolean:trueValue=Yes,falseValue=No}}
";

var template = templateFactory.Create(templateStr);

// Bind parameters (note struct types)
var parameters = new IParameter[] {
    new TextParameter("FirstName", "David"),
    new TextParameter("LastName", "Parker"),
    new DateTimeParameter("DateOfBirth", new DateTime(1980, 08, 15)),
    new NumberParameter("AccountBalance", 1750.45M),
    new BooleanParameter("IsActive", true)
};

var result = template.Bind(parameters);
Console.WriteLine(result);
```

## Target Framework Update

**v2.x:** .NET Standard 2.0
```xml
<TargetFramework>netstandard2.0</TargetFramework>
```

**v3.0.0:** .NET 8.0
```xml
<TargetFramework>net8.0</TargetFramework>
```

Update your project's target framework to .NET 8.0 or later.

## Breaking Changes Checklist

Use this checklist to ensure complete migration:

- [ ] Updated all template strings from `{...}` to `{{...}}`
- [ ] Updated pipe names: `date`→`datetime`, `decimal`→`number`, `booleantext`→`boolean`
- [ ] Changed `Parameter.Text` to `TextParameter`
- [ ] Changed `Parameter.Number` to `NumberParameter`
- [ ] Changed `Parameter.Date` to `DateTimeParameter`
- [ ] Changed `Parameter.Boolean` to `BooleanParameter`
- [ ] Updated DI registration from `AddTemplateBinder(bool)` to `AddTemplateBinder(Type[])`
- [ ] Changed injected type from `IBinderFactory` to `ITemplateFactory`
- [ ] Changed factory method from `Create()` returning `IBinder` to `ITemplate`
- [ ] Changed binding method from `binder.Bind()` to `template.Bind()`
- [ ] Added error handling for missing parameters (no longer silent)
- [ ] Updated custom pipes to use new parameter struct types
- [ ] Updated project target framework to .NET 8.0+
- [ ] Tested all templates and parameter bindings
- [ ] Reviewed and updated unit tests

## Support

If you encounter issues during migration:

1. Review this migration guide thoroughly
2. Check the updated [README.md](README.md) for v3 examples
3. Review [CLAUDE.md](CLAUDE.md) for architecture details
4. Check [GitHub Issues](https://github.com/askoutaris/template-binder/issues)
5. Create a new issue with migration questions

## Why the Breaking Changes?

Version 3.0.0 represents a complete architectural improvement:

- **Better Performance**: Token-based architecture with O(1) lookups
- **Type Safety**: Readonly struct parameters instead of classes
- **Maintainability**: Cleaner separation of concerns
- **Extensibility**: More flexible pipe system with constructor injection
- **Modern .NET**: Takes advantage of latest C# features

While migration requires effort, the v3 architecture provides a more robust, performant, and maintainable foundation for the future.
