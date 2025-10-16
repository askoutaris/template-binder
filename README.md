# TemplateBinder

A lightweight, Angular-like text template binding library for C# that enables powerful string interpolation with transformation pipelines.

## Features

- **Simple Syntax**: Angular-inspired placeholder syntax `{ParameterName|pipe:param1=value1}`
- **Type-Safe Parameters**: Strongly-typed parameter system (Text, Number, Date, Boolean)
- **Transformation Pipes**: Built-in pipes for common formatting needs (date, decimal, boolean-to-text)
- **Extensible**: Easy-to-implement custom pipes for domain-specific transformations
- **Dependency Injection**: First-class support for Microsoft.Extensions.DependencyInjection
- **.NET Standard 2.0**: Compatible with .NET Core, .NET Framework, and .NET 5+
- **Reusable Binders**: Parse once, bind multiple times for performance
- **Flexible Error Handling**: Choose to throw exceptions or silently handle missing parameters

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

```csharp
using TemplateBinder.Binders;
using TemplateBinder.Factories;
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

// Create pipe and binder factories
var pipeTypes = new Type[] {
    typeof(BooleanTextPipe),
    typeof(DatePipe),
    typeof(DecimalPipe),
    typeof(TextPipe),
};

var pipeFactory = new PipeFactoryDefault(pipeTypes);
var binderFactory = new BinderFactoryDefault(pipeFactory, throwOnMissingParameters: true);

// Define your template
var template = "Hello {FirstName} {LastName}!";

// Create a binder (parsing happens once)
var binder = binderFactory.Create(template);

// Bind parameters to the template
var parameters = new Parameter[] {
    new Parameter.Text("FirstName", "John"),
    new Parameter.Text("LastName", "Doe")
};

var result = binder.Bind(parameters);
// Output: "Hello John Doe!"
```

## Template Syntax

Templates use placeholders with the following syntax:

```
{ParameterName|pipeName:param1=value1,param2=value2}
```

- **ParameterName**: The name of the parameter to bind (required)
- **|pipeName**: Optional pipe to transform the parameter (defaults to `text`)
- **:param1=value1**: Optional comma-separated pipe parameters

### Examples

```csharp
{FirstName}                                           // Simple text binding
{DateOfBirth|date:format=yyyy-MM-dd}                 // Date formatting
{AccountBalance|decimal:format=N2}                   // Decimal with 2 decimals
{IsActive|booleantext:trueValue=Yes,falseValue=No}  // Boolean to text
```

## Usage Examples

### Complete Example with All Parameter Types

```csharp
var template = @"
    User Report
    -----------
    Name: {FirstName} {LastName}
    Born: {DateOfBirth|date:format=yyyy-MM-dd}
    Login Count: {LoginTimes}
    Balance: ${AccountBalance|decimal:format=N2}
    Active: {IsActive|booleantext:trueValue=Yes,falseValue=No}
    Locked: {IsLockedOut|booleantext:trueValue=Yes,falseValue=No}";

var binder = binderFactory.Create(template);

var parameters = new Parameter[] {
    new Parameter.Text("FirstName", "David"),
    new Parameter.Text("LastName", "Parker"),
    new Parameter.Date("DateOfBirth", new DateTime(1980, 08, 15)),
    new Parameter.Number("LoginTimes", 85),
    new Parameter.Number("AccountBalance", 1750.45M),
    new Parameter.Boolean("IsActive", true),
    new Parameter.Boolean("IsLockedOut", false)
};

var result = binder.Bind(parameters);
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
    services.AddTemplateBinder(throwOnMissingParameters: true);

    // Or with custom pipes
    services.AddTemplateBinder(
        throwOnMissingParameters: true,
        additionalPipeTypes: new[] { typeof(MyCustomPipe) }
    );
}
```

Then inject `IBinderFactory` into your services:

```csharp
public class EmailService
{
    private readonly IBinderFactory _binderFactory;

    public EmailService(IBinderFactory binderFactory)
    {
        _binderFactory = binderFactory;
    }

    public string GenerateEmailBody(User user)
    {
        var template = "Dear {FirstName}, your account balance is {Balance|decimal:format=C}.";
        var binder = _binderFactory.Create(template);

        var parameters = new Parameter[] {
            new Parameter.Text("FirstName", user.FirstName),
            new Parameter.Number("Balance", user.Balance)
        };

        return binder.Bind(parameters);
    }
}
```

### Microsoft Dependency Injection (Console Apps)

```csharp
var serviceProvider = new ServiceCollection()
    .AddTemplateBinder(throwOnMissingParameters: true)
    .BuildServiceProvider();

var binderFactory = serviceProvider.GetService<IBinderFactory>();
```

## Built-in Pipes

### Text Pipe (default)

Returns the parameter value as-is.

```csharp
{Name}              // Uses text pipe by default
{Name|text}         // Explicit text pipe
```

### Date Pipe

Formats `DateTime` values using standard .NET format strings.

```csharp
{BirthDate|date:format=yyyy-MM-dd}              // 2024-03-15
{Timestamp|date:format=o}                       // ISO 8601 format
{EventDate|date:format=MMMM dd\, yyyy}          // March 15, 2024
```

### Decimal Pipe

Formats numeric values using standard .NET numeric format strings.

```csharp
{Price|decimal:format=N2}         // 1,234.56 (with thousand separators)
{Amount|decimal:format=C}          // $1,234.56 (currency)
{Percentage|decimal:format=P2}     // 12.34% (percentage)
```

### BooleanText Pipe

Converts boolean values to custom text representations.

```csharp
{IsActive|booleantext:trueValue=Active,falseValue=Inactive}
{HasAccess|booleantext:trueValue=✓,falseValue=✗}
```

## Creating Custom Pipes

Implement the `IPipe` interface and add the `[PipeName]` attribute:

```csharp
using TemplateBinder.Attributes;
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

[PipeName("uppercase")]
public class UppercasePipe : IPipe
{
    public UppercasePipe()
    {
    }

    public IParameter Transform(IParameter parameter)
    {
        if (parameter is not Parameter.Text text)
            throw new ArgumentException("Uppercase pipe requires Text parameter");

        var value = text.Value?.ToUpperInvariant();
        return new Parameter.Text(parameter.Name, value);
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

    public TruncatePipe(string? maxLength, string? suffix)
    {
        _maxLength = int.Parse(maxLength ?? "10");
        _suffix = suffix ?? "...";
    }

    public IParameter Transform(IParameter parameter)
    {
        if (parameter is not Parameter.Text text)
            throw new ArgumentException("Truncate pipe requires Text parameter");

        var value = text.Value;
        if (value != null && value.Length > _maxLength)
        {
            value = value.Substring(0, _maxLength) + _suffix;
        }

        return new Parameter.Text(parameter.Name, value);
    }
}
```

Usage:
```csharp
{Description|truncate:maxLength=50,suffix=...}
```

### Registering Custom Pipes

**With Dependency Injection:**
```csharp
services.AddTemplateBinder(
    throwOnMissingParameters: true,
    additionalPipeTypes: new[] { typeof(UppercasePipe), typeof(TruncatePipe) }
);
```

**Without Dependency Injection:**
```csharp
var pipeTypes = new Type[] {
    typeof(TextPipe),
    typeof(DatePipe),
    typeof(DecimalPipe),
    typeof(BooleanTextPipe),
    typeof(UppercasePipe),      // Your custom pipe
    typeof(TruncatePipe)         // Your custom pipe
};

var pipeFactory = new PipeFactoryDefault(pipeTypes);
var binderFactory = new BinderFactoryDefault(pipeFactory, throwOnMissingParameters: true);
```

## Parameter Types

### Parameter.Text
```csharp
new Parameter.Text("Name", "John Doe")
```

### Parameter.Number
```csharp
new Parameter.Number("Age", 25)
new Parameter.Number("Price", 99.99M)
```

### Parameter.Date
```csharp
new Parameter.Date("BirthDate", new DateTime(1990, 1, 15))
new Parameter.Date("Timestamp", DateTime.Now)
```

### Parameter.Boolean
```csharp
new Parameter.Boolean("IsActive", true)
new Parameter.Boolean("HasAccess", false)
```

## Error Handling

Control how missing parameters are handled using the `throwOnMissingParameters` flag:

```csharp
// Throw exception when parameter is missing
var binderFactory = new BinderFactoryDefault(pipeFactory, throwOnMissingParameters: true);

// Silently skip missing parameters (placeholder remains in output)
var binderFactory = new BinderFactoryDefault(pipeFactory, throwOnMissingParameters: false);
```

## Performance

- **Parse Once, Bind Multiple Times**: Template parsing happens once during binder creation. Reuse binders for repeated operations with different parameter values.
- **StringBuilder-based**: Uses `StringBuilder.Replace()` for efficient text manipulation
- **No Reflection During Binding**: Pipe resolution happens during template parsing, not during binding

```csharp
// Good: Parse once, bind many times
var binder = binderFactory.Create(emailTemplate);
foreach (var user in users)
{
    var email = binder.Bind(CreateParameters(user));
    SendEmail(email);
}
```

## Requirements

- **.NET Standard 2.0** or higher
- **C# 9.0** language features (nullable reference types)

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
