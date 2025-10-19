# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TemplateBinder is a C# library that provides Angular-like template binding for text templates. It uses a placeholder syntax `{{ParameterName|pipeName:param1=value1,param2=value2}}` to transform parameters into formatted text output.

The library targets .NET 8.0 and uses the latest C# version with nullable reference types enabled.

## Solution Structure

- **TemplateBinder**: Core library containing the template binding engine
- **TemplateBinder.Extensions.DependencyInjection**: Microsoft.Extensions.DependencyInjection integration
- **Tests**: xUnit test project with comprehensive unit tests
- **Workbench**: Console application for testing and examples

## Build and Test Commands

### Build the solution
```bash
dotnet build TemplateBinder.sln
```

### Build specific project
```bash
dotnet build TemplateBinder/TemplateBinder.csproj
dotnet build TemplateBinder.Extensions.DependencyInjection/TemplateBinder.Extensions.DependencyInjection.csproj
```

### Run tests
```bash
dotnet test Tests/Tests.csproj
```

### Run tests with code coverage
```bash
dotnet test Tests/Tests.csproj --collect:"XPlat Code Coverage"
```

### Run the Workbench (examples)
```bash
dotnet run --project Workbench/Workbench.csproj
```

### Build for Release
```bash
dotnet build TemplateBinder.sln -c Release
```

### Pack NuGet packages
```bash
dotnet pack TemplateBinder/TemplateBinder.csproj -c Release
dotnet pack TemplateBinder.Extensions.DependencyInjection/TemplateBinder.Extensions.DependencyInjection.csproj -c Release
```

## Core Architecture

### Template Parsing Flow

1. **ITemplateFactory** creates **ITemplate** instances from template strings
2. **TemplateFactory** orchestrates the parsing pipeline:
   - **TemplateParser** splits the template into string tokens (text and placeholders using regex)
   - **TemplateTokensFactory** converts string tokens into **ITemplateToken** objects:
     - Text tokens (literal strings) → **TextToken**
     - Placeholder tokens (`{{...}}`) → **PlaceholderToken**
   - **PlaceholderParser** extracts parameter name and pipe information from placeholders
   - **PipeActivator** creates **IPipe** instances using reflection and constructor injection
3. When `Bind(IReadOnlyCollection<IParameter>)` is called:
   - Template converts parameters to dictionary for O(1) lookup
   - Each token generates its text representation:
     - TextToken returns literal text
     - PlaceholderToken looks up parameter, applies pipe transformation, returns result
   - All token texts are concatenated using StringBuilder

### Template Syntax

Templates use the format: `{{ParameterName|pipeName:param1=value1,param2=value2}}`

- `ParameterName`: The parameter to bind (required)
- `|pipeName`: Optional pipe to transform the parameter (no default, uses parameter's GetText() if omitted)
- `:param1=value1,param2=value2`: Optional comma-separated pipe parameters

Example: `{{DateOfBirth|datetime:format=yyyy-MM-dd}}` applies the datetime pipe with format parameter "yyyy-MM-dd"

### Parsing Implementation

**TemplateParser** (TemplateBinder/Services/TemplateParser.cs):
- Uses regex `({{.*?}})` to split template into tokens
- Returns list of strings (text and placeholders)

**PlaceholderParser** (TemplateBinder/Services/PlaceholderParser.cs):
- Removes `{{` and `}}` from placeholder
- Splits by `|` to separate parameter name from pipe context
- Splits pipe context by `:` to separate pipe name from parameters
- Parses parameters as comma-separated `name=value` pairs into NameValueCollection
- Trims whitespace from all parts

### Parameter Types

All parameter types are readonly structs implementing `IParameter`:

- **TextParameter**: String values (returns value or parameter name if null)
- **NumberParameter**: Numeric values (decimal) - returns formatted number or parameter name if null
- **DateTimeParameter**: DateTime values - returns ISO 8601 format or parameter name if null
- **BooleanParameter**: Boolean values - returns "True"/"False" or parameter name if null

Each parameter has:
- `Name` property (string)
- Type-specific `Value` property (nullable)
- `GetText()` method returning string representation

### Pipe System

**IPipe** implementations transform `IParameter` to formatted output:

- **DateTimePipe**: Formats DateTime with optional `format` parameter (e.g., `format=yyyy-MM-dd`)
- **NumberPipe**: Formats numbers with optional `format` parameter (e.g., `format=N2`)
- **BooleanPipe**: Converts boolean to text with required `trueValue` and `falseValue` parameters

Pipes are identified by `[PipeName("name")]` attribute and must have exactly one public constructor.

**PipeActivator** (TemplateBinder/Services/PipeActivator.cs):
- Uses FrozenDictionary for O(1) pipe type lookup
- Matches constructor parameters to pipe parameters (case-insensitive)
- Uses TypeDescriptor.GetConverter for parameter type conversion
- Provides default values for missing optional parameters

### Token System

**ITemplateToken** implementations generate text output:

- **TextToken**: Returns literal text unchanged
- **PlaceholderToken**: Looks up parameter by name, applies optional pipe transformation, returns formatted text

### Dependency Injection Integration

The `AddTemplateBinder(params Type[] customPipeTypes)` extension method registers:
- `IPipeActivator` as singleton with built-in pipes (NumberPipe, DateTimePipe, BooleanPipe) + custom pipes
- `ITemplateTokensFactory` as singleton
- `IPlaceholderParser` as singleton
- `ITemplateParser` as singleton
- `ITemplateFactory` as singleton

Custom pipes can be registered by passing their types to the extension method.

**Note**: Missing parameters will throw `ArgumentException` when binding. There is no silent fallback option.

## Adding Custom Pipes

1. Implement `IPipe` interface with `Transform(IParameter parameter)` method
2. Add `[PipeName("yourpipename")]` attribute to the class
3. Create exactly one public constructor accepting pipe parameters
4. When using DI: pass custom pipe type to `AddTemplateBinder(typeof(YourPipe))`
5. When using manually: include type in `PipeActivator` constructor

Example pattern (see DateTimePipe in TemplateBinder/Pipes/DateTimePipe.cs):
```csharp
[PipeName("yourpipe")]
public class YourPipe : IPipe
{
    private readonly string? _param;

    public YourPipe(string? param)
    {
        _param = param;
    }

    public IParameter Transform(IParameter parameter)
    {
        // Type check the parameter
        if (parameter is not YourParameterType typed)
            throw new ArgumentException($"YourPipe requires YourParameterType");

        // Transform logic
        var formattedValue = typed.Value?.ToString(_param);
        return new TextParameter(parameter.Name, formattedValue);
    }
}
```

## Key Implementation Details

- Template parsing happens once during `ITemplate` creation via factory, templates are reusable
- Token-based architecture allows efficient rendering without regex replacement
- Parameters stored in dictionary (O(1) lookup) during binding
- Missing parameters throw `ArgumentException` during binding
- Pipe parameters extracted as `NameValueCollection` from template syntax
- All pipes must return `IParameter`, typically `TextParameter` with formatted value
- PipeActivator uses FrozenDictionary for fast pipe type lookup
- Parameter types are readonly structs for better performance
- PlaceholderParser trims whitespace from parameter names and pipe context
