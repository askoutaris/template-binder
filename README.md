# TemplateBinder

A lightweight, Angular-like text template binding library for C# that enables powerful string interpolation with transformation pipelines.

## ⚠️ Version 3.0 - Breaking Changes

**If you're upgrading from v2.x, please read the [MIGRATION GUIDE](MIGRATION.md).**

Version 3.0.0 is a complete rewrite with breaking changes:
- Template syntax changed from `{...}` to `{{...}}`
- API changed from `IBinder` to `ITemplate`
- Parameter types changed from classes to structs
- Requires .NET 8.0 (was .NET Standard 2.0)

See [CHANGELOG.md](CHANGELOG.md) for full details.

## Features

- **Simple Syntax**: Angular-inspired placeholder syntax `{{ParameterName|pipe:param1=value1}}`
- **Type-Safe Parameters**: Strongly-typed parameter system (TextParameter, NumberParameter, DateTimeParameter, BooleanParameter)
- **Transformation Pipes**: Built-in pipes for common formatting needs (datetime, number, boolean)
- **Extensible**: Easy-to-implement custom pipes for domain-specific transformations
- **Dependency Injection**: First-class support for Microsoft.Extensions.DependencyInjection
- **.NET 8.0**: Modern .NET platform with latest C# features
- **High Performance**: Token-based architecture with O(1) lookups and readonly structs
- **Reusable Templates**: Parse once, bind multiple times for maximum performance

## Installation

### NuGet Package Manager

```bash
Install-Package TemplateBinder
```

### .NET CLI

```bash
dotnet add package TemplateBinder
```

### For Dependency Injection Support

```bash
dotnet add package TemplateBinder.Extensions.DependencyInjection
```

## Quick Start

### Using Dependency Injection (Recommended)

```csharp
using Microsoft.Extensions.DependencyInjection;
using TemplateBinder.Extensions.DependencyInjection;
using TemplateBinder.Parameters;
using TemplateBinder.Services;

// Setup Dependency Injection
var services = new ServiceCollection();
services.AddTemplateBinder(); // Registers built-in pipes

var serviceProvider = services.BuildServiceProvider();

// Resolve ITemplateFactory
var templateFactory = serviceProvider.GetRequiredService<ITemplateFactory>();

// Define your template
var templateString = "Hello {{FirstName}} {{LastName}}!";

// Create a template (parsing happens once)
var template = templateFactory.Create(templateString);

// Bind parameters to the template
var parameters = new IParameter[] {
    new TextParameter("FirstName", "John"),
    new TextParameter("LastName", "Doe")
};

var result = template.Bind(parameters);
// Output: "Hello John Doe!"
```

### Without Dependency Injection

```csharp
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;
using TemplateBinder.Services;

// Setup services manually
var parser = new TemplateParser();
var placeholderParser = new PlaceholderParser();
var pipeActivator = new PipeActivator([
    typeof(DateTimePipe),
    typeof(NumberPipe),
    typeof(BooleanPipe)
]);
var tokensFactory = new TemplateTokensFactory(placeholderParser, pipeActivator);
var templateFactory = new TemplateFactory(parser, tokensFactory);

// Create and use template
var template = templateFactory.Create("Hello {{FirstName}} {{LastName}}!");
var result = template.Bind([
    new TextParameter("FirstName", "John"),
    new TextParameter("LastName", "Doe")
]);
```

## Template Syntax

Templates use placeholders with the following syntax:

```
{{ParameterName|pipeName:param1=value1,param2=value2}}
```

- **ParameterName**: The name of the parameter to bind (required)
- **|pipeName**: Optional pipe to transform the parameter (uses parameter's default text representation if omitted)
- **:param1=value1**: Optional comma-separated pipe parameters

### Examples

```csharp
{{FirstName}}                                            // Simple text binding
{{DateOfBirth|datetime:format=yyyy-MM-dd}}               // Date formatting
{{AccountBalance|number:format=N2}}                      // Number with 2 decimals
{{IsActive|boolean:trueValue=Yes,falseValue=No}}        // Boolean to text
```

## Usage Examples

### Complete Example with All Parameter Types

```csharp
var templateString = @"
User Report
-----------
Name: {{FirstName}} {{LastName}}
Born: {{DateOfBirth|datetime:format=yyyy-MM-dd}}
Login Count: {{LoginTimes}}
Balance: ${{AccountBalance|number:format=N2}}
Active: {{IsActive|boolean:trueValue=Yes,falseValue=No}}
Locked: {{IsLockedOut|boolean:trueValue=Yes,falseValue=No}}";

var template = templateFactory.Create(templateString);

var parameters = new IParameter[] {
    new TextParameter("FirstName", "David"),
    new TextParameter("LastName", "Parker"),
    new DateTimeParameter("DateOfBirth", new DateTime(1980, 08, 15)),
    new NumberParameter("LoginTimes", 85),
    new NumberParameter("AccountBalance", 1750.45M),
    new BooleanParameter("IsActive", true),
    new BooleanParameter("IsLockedOut", false)
};

var result = template.Bind(parameters);
```

**Output:**
```
User Report
-----------
Name: David Parker
Born: 1980-08-15
Login Count: 85
Balance: $1,750.45
Active: Yes
Locked: No
```

### ASP.NET Core Integration

Install the `TemplateBinder.Extensions.DependencyInjection` package and register services:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddTemplateBinder();

    // Or with custom pipes
    services.AddTemplateBinder(typeof(MyCustomPipe), typeof(AnotherCustomPipe));
}
```

Then inject `ITemplateFactory` into your services and **create templates once, reuse many times**:

```csharp
public class EmailService
{
    private readonly ITemplate _emailTemplate;

    public EmailService(ITemplateFactory templateFactory)
    {
        // ✅ GOOD: Create template once in constructor
        _emailTemplate = templateFactory.Create(
            "Dear {{FirstName}}, your account balance is {{Balance|number:format=C}}."
        );
    }

    public string GenerateEmailBody(User user)
    {
        // ✅ GOOD: Reuse the same template with different parameters
        var parameters = new IParameter[] {
            new TextParameter("FirstName", user.FirstName),
            new NumberParameter("Balance", user.Balance)
        };

        return _emailTemplate.Bind(parameters);
    }
}
```

**⚠️ Performance Tip:** Always create templates once (in constructor or static field) and reuse them. Template parsing is expensive, binding is fast.

### Microsoft Dependency Injection (Console Apps)

```csharp
var serviceProvider = new ServiceCollection()
    .AddTemplateBinder()
    .BuildServiceProvider();

var templateFactory = serviceProvider.GetRequiredService<ITemplateFactory>();
```

## Built-in Pipes

### DateTime Pipe

Formats `DateTime` values using standard .NET format strings.

```csharp
{{BirthDate|datetime:format=yyyy-MM-dd}}              // 2024-03-15
{{Timestamp|datetime:format=o}}                       // ISO 8601 format
{{EventDate|datetime:format=MMMM dd\, yyyy}}          // March 15, 2024
```

### Number Pipe

Formats numeric values using standard .NET numeric format strings.

```csharp
{{Price|number:format=N2}}         // 1,234.56 (with thousand separators)
{{Amount|number:format=C}}         // $1,234.56 (currency)
{{Percentage|number:format=P2}}    // 12.34% (percentage)
```

### Boolean Pipe

Converts boolean values to custom text representations.

```csharp
{{IsActive|boolean:trueValue=Active,falseValue=Inactive}}
{{HasAccess|boolean:trueValue=✓,falseValue=✗}}
```

### No Pipe (Default Parameter Text)

If no pipe is specified, the parameter's `GetText()` method is used:

```csharp
{{Name}}              // Returns TextParameter.Value
{{Count}}             // Returns NumberParameter.Value.ToString()
{{Date}}              // Returns DateTimeParameter.Value in ISO 8601 format
{{IsActive}}          // Returns "True" or "False"
```

## Creating Custom Pipes

Implement the `IPipe` interface and add the `[PipeName]` attribute:

```csharp
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

[PipeName("uppercase")]
public class UppercasePipe : IPipe
{
    // Must have exactly one public constructor
    public UppercasePipe()
    {
    }

    public IParameter Transform(IParameter parameter)
    {
        if (parameter is not TextParameter text)
            throw new ArgumentException("Uppercase pipe requires TextParameter");

        var value = text.Value?.ToUpperInvariant();
        return new TextParameter(parameter.Name, value);
    }
}
```

### Custom Pipe with Parameters

```csharp
[PipeName("truncate")]
public class TruncatePipe : IPipe
{
    private readonly int _maxLength;
    private readonly string _suffix;

    // Constructor parameters match template syntax
    // {{Text|truncate:maxLength=50,suffix=...}}
    public TruncatePipe(string? maxLength, string? suffix)
    {
        _maxLength = int.Parse(maxLength ?? "10");
        _suffix = suffix ?? "...";
    }

    public IParameter Transform(IParameter parameter)
    {
        if (parameter is not TextParameter text)
            throw new ArgumentException("Truncate pipe requires TextParameter");

        var value = text.Value;
        if (value != null && value.Length > _maxLength)
        {
            value = value.Substring(0, _maxLength) + _suffix;
        }

        return new TextParameter(parameter.Name, value);
    }
}
```

Usage:
```csharp
{{Description|truncate:maxLength=50,suffix=...}}
```

### Registering Custom Pipes

**With Dependency Injection:**
```csharp
services.AddTemplateBinder(typeof(UppercasePipe), typeof(TruncatePipe));
```

**Without Dependency Injection:**
```csharp
var pipeActivator = new PipeActivator([
    typeof(DateTimePipe),
    typeof(NumberPipe),
    typeof(BooleanPipe),
    typeof(UppercasePipe),      // Your custom pipe
    typeof(TruncatePipe)         // Your custom pipe
]);
```

## Parameter Types

All parameter types are readonly structs implementing `IParameter` with a `Name`, type-specific `Value`, and `GetText()` method.

### TextParameter
```csharp
new TextParameter("Name", "John Doe")
new TextParameter("Description", null)  // Returns parameter name if value is null
```

### NumberParameter
```csharp
new NumberParameter("Age", 25)
new NumberParameter("Price", 99.99M)
new NumberParameter("Count", null)  // Returns parameter name if value is null
```

### DateTimeParameter
```csharp
new DateTimeParameter("BirthDate", new DateTime(1990, 1, 15))
new DateTimeParameter("Timestamp", DateTime.Now)
new DateTimeParameter("Date", null)  // Returns parameter name if value is null
```

### BooleanParameter
```csharp
new BooleanParameter("IsActive", true)
new BooleanParameter("HasAccess", false)
new BooleanParameter("Flag", null)  // Returns parameter name if value is null
```

## Error Handling

Missing parameters will throw an `ArgumentException` during binding:

```csharp
var template = templateFactory.Create("Hello {{Name}}!");
var result = template.Bind([]);  // Throws ArgumentException: "Parameter Name was not found"
```

**Important**: Always ensure all required parameters are provided. There is no silent fallback option.

## Performance

TemplateBinder is designed for high performance:

- **Parse Once, Bind Multiple Times**: Template parsing happens once during template creation. Reuse templates for maximum performance.
- **Token-Based Architecture**: Uses pre-parsed token objects instead of regex replacement during binding
- **O(1) Lookups**: Parameters stored in dictionary, pipe types in FrozenDictionary
- **Readonly Structs**: Parameter types are readonly structs for better memory efficiency
- **No Reflection During Binding**: Pipe instances created during template parsing, not during binding

```csharp
// ✅ GOOD: Parse once, bind many times
var template = templateFactory.Create(emailTemplate);
foreach (var user in users)
{
    var email = template.Bind(CreateParameters(user));
    SendEmail(email);
}

// ❌ BAD: Parsing inside the loop
foreach (var user in users)
{
    var template = templateFactory.Create(emailTemplate);  // DON'T DO THIS!
    var email = template.Bind(CreateParameters(user));
    SendEmail(email);
}
```

### Performance Tips

1. **✅ ALWAYS create templates once, reuse many times**
   ```csharp
   // ✅ GOOD: Parse once
   private readonly ITemplate _template;
   public MyService(ITemplateFactory factory)
   {
       _template = factory.Create("Hello {{Name}}!");
   }
   public string Generate(string name) => _template.Bind([new TextParameter("Name", name)]);

   // ❌ BAD: Parsing on every call
   public string Generate(string name)
   {
       var template = _factory.Create("Hello {{Name}}!");  // Expensive!
       return template.Bind([new TextParameter("Name", name)]);
   }
   ```

2. **Use dependency injection**: Services are registered as singletons
3. **For static templates**: Consider static readonly fields
   ```csharp
   public class Constants
   {
       private static readonly ITemplateFactory _factory = CreateFactory();
       public static readonly ITemplate WelcomeEmail = _factory.Create("Welcome {{Name}}!");
   }
   ```

## Requirements

- **.NET 8.0** or higher
- Latest C# version with nullable reference types enabled

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**Alkiviadis Skoutaris**

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## Links

- [NuGet Package - TemplateBinder](https://www.nuget.org/packages/TemplateBinder/)
- [NuGet Package - Dependency Injection Extension](https://www.nuget.org/packages/TemplateBinder.Extensions.DependencyInjection/)
- [GitHub Repository](https://github.com/askoutaris/template-binder)
