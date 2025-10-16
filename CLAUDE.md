# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TemplateBinder is a C# library that provides Angular-like template binding for text templates. It uses a placeholder syntax `{ParameterName|pipeName:param1=value1,param2=value2}` to transform parameters into formatted text output.

The library targets .NET Standard 2.0 and uses C# 9 with nullable reference types enabled.

## Solution Structure

- **TemplateBinder**: Core library containing the template binding engine
- **TemplateBinder.Extensions.DependencyInjection**: Microsoft.Extensions.DependencyInjection integration
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

1. **IBinderFactory** creates **IBinder** instances from template strings
2. **BinderDefault** (implements IBinder) parses the template during construction:
   - Uses regex patterns from `Constants.Regex` to find placeholders matching `{...}`
   - Extracts parameter name, pipe name, and pipe parameters from each placeholder
   - Creates **Placeholder** objects containing the original text, parameter name, and configured **IPipe**
3. When `Bind(Parameter[])` is called:
   - Matches each placeholder's parameter name against provided parameters
   - Applies the pipe transformation to convert parameter to text
   - Replaces placeholder text with transformed value

### Template Syntax

Templates use the format: `{ParameterName|pipeName:param1=value1,param2=value2}`

- `ParameterName`: The parameter to bind (required)
- `|pipeName`: Optional pipe to transform the parameter (defaults to `text`)
- `:param1=value1,param2=value2`: Optional comma-separated pipe parameters

Example: `{DateOfBirth|date:format=o}` applies the date pipe with format parameter "o"

### Regex Patterns (Constants.cs:14-19)

- **Placeholder**: `(\{)(.*?(\})` - Matches `{...}`
- **ParameterName**: `[^{][^(||})]*` - Extracts parameter name from placeholder
- **PipeName**: `(?<=\|)(.*?)(?=\:)` - Extracts pipe name after `|` before `:`
- **PipeParameters**: `(?<=\:)(.*)[^}]` - Extracts parameters after `:`

### Parameter Types

All parameters inherit from abstract `Parameter` class which implements `IParameter`:

- **Parameter.Text**: String values
- **Parameter.Number**: Numeric values (decimal)
- **Parameter.Date**: DateTime values
- **Parameter.Boolean**: Boolean values

### Pipe System

**IPipe** implementations transform `IParameter` to formatted output:

- **TextPipe**: Default pipe, returns parameter value as-is
- **DatePipe**: Formats DateTime with optional `format` parameter (e.g., `format=o`)
- **DecimalPipe**: Formats numbers with optional `format` parameter (e.g., `format=N2`)
- **BooleanTextPipe**: Converts boolean to text with `trueValue` and `falseValue` parameters

Pipes are registered with **IPipeFactory** and identified by `[PipeName]` attribute.

### Dependency Injection Integration

The `AddTemplateBinder(throwOnMissingParameters)` extension method registers:
- `IPipeFactory` as singleton with default pipe types
- `IBinderFactory` as singleton

Optional `additionalPipeTypes` parameter allows registering custom pipes.

The `throwOnMissingParameters` parameter controls whether missing parameters throw exceptions or are silently ignored during binding.

## Adding Custom Pipes

1. Implement `IPipe` interface with `Transform(IParameter parameter)` method
2. Add `[PipeName("yourpipename")]` attribute to the class
3. Accept pipe parameters via constructor (extracted from template syntax)
4. When using DI: pass custom pipe type in `AddTemplateBinder(throwOnMissingParameters, new[] { typeof(YourPipe) })`
5. When using manually: include type in `PipeFactoryDefault` constructor

Example pattern (see DatePipe in TemplateBinder/Pipes/Date.cs:6-26):
```csharp
[PipeName(Constants.Pipes.YourPipe)]
public class YourPipe : IPipe
{
    private readonly string? _param;

    public YourPipe(string? param)
    {
        _param = param;
    }

    public IParameter Transform(IParameter parameter)
    {
        // Transform logic
        return new Parameter.Text(parameter.Name, transformedValue);
    }
}
```

## Key Implementation Details

- Template parsing happens once during `IBinder` construction, binding is reusable
- Missing parameters behavior controlled by `throwOnMissingParameters` flag in `BinderFactoryDefault`
- Placeholder replacement uses `StringBuilder.Replace()` for performance
- Pipe parameters extracted as `NameValueCollection` from template syntax
- All pipes must return `IParameter`, typically `Parameter.Text` with formatted value
