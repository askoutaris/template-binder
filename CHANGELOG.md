# Changelog

All notable changes to TemplateBinder will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - 2025-01-XX

### ⚠️ BREAKING CHANGES - Complete Refactor

Version 3.0.0 is a complete architectural rewrite. **This is not backward compatible with v2.x.**

See [MIGRATION.md](MIGRATION.md) for detailed migration guide from v2.x to v3.0.0.

### Changed

#### Template Syntax
- **BREAKING**: Placeholder syntax changed from `{...}` to `{{...}}`
  - Old: `{Name|pipe:param=value}`
  - New: `{{Name|pipe:param=value}}`

#### API Changes
- **BREAKING**: `IBinder` → `ITemplate`
- **BREAKING**: `IBinderFactory` → `ITemplateFactory`
- **BREAKING**: `BinderFactoryDefault` removed, use DI or manual setup
- **BREAKING**: `PipeFactoryDefault` removed, use `PipeActivator`

#### Parameter Types
- **BREAKING**: Changed from nested classes to structs
  - `Parameter.Text` → `TextParameter`
  - `Parameter.Number` → `NumberParameter`
  - `Parameter.Date` → `DateTimeParameter`
  - `Parameter.Boolean` → `BooleanParameter`
- **BREAKING**: All parameters are now readonly structs (value types)
- **BREAKING**: Parameters no longer inherit from base `Parameter` class

#### Pipe Names
- **BREAKING**: Renamed built-in pipes
  - `date` → `datetime`
  - `decimal` → `number`
  - `booleantext` → `boolean`

#### Dependency Injection
- **BREAKING**: `AddTemplateBinder(throwOnMissingParameters)` → `AddTemplateBinder(customPipeTypes...)`
- **BREAKING**: Removed `throwOnMissingParameters` option - now always throws on missing parameters
- **BREAKING**: Changed from `IBinderFactory` injection to `ITemplateFactory` injection

#### Error Handling
- **BREAKING**: Missing parameters now always throw `ArgumentException`
- **BREAKING**: Removed silent fallback option for missing parameters

#### Target Framework
- **BREAKING**: Changed from .NET Standard 2.0 to .NET 8.0
- **BREAKING**: Requires latest C# version

### Added

#### New Architecture
- Token-based parsing and rendering system
- `ITemplateToken` interface with `TextToken` and `PlaceholderToken` implementations
- `TemplateParser` for regex-based template splitting
- `PlaceholderParser` for extracting parameter and pipe information
- `PipeActivator` for reflection-based pipe instantiation
- `TemplateTokensFactory` for creating typed token objects

#### Performance Improvements
- O(1) parameter lookups using dictionary
- O(1) pipe type lookups using FrozenDictionary
- Readonly struct parameters for better memory efficiency
- Token-based rendering (no regex replacement during binding)
- Pipe instances created during template parsing (not during binding)

#### Documentation
- Comprehensive XML documentation comments on all public types
- Updated README.md with v3 API examples
- Added CLAUDE.md with detailed architecture documentation

#### Testing
- 78 comprehensive unit tests
- 98%+ code coverage
- Tests for all parameter types, pipes, services, and integration scenarios

### Removed

- **BREAKING**: `IBinder` and `IBinderFactory` interfaces
- **BREAKING**: `BinderDefault` and `BinderFactoryDefault` classes
- **BREAKING**: `PipeFactoryDefault` and `IPipeFactory` interfaces
- **BREAKING**: `Parameter` abstract base class
- **BREAKING**: `TextPipe` (use parameter's GetText() instead)
- **BREAKING**: Constants.cs and regex pattern constants
- **BREAKING**: Silent error handling for missing parameters

### Migration Path

For users upgrading from v2.x:

1. **Update template syntax** from `{...}` to `{{...}}`
2. **Update parameter types** from `Parameter.Text()` to `TextParameter()`
3. **Update pipe names** in templates (date→datetime, decimal→number, booleantext→boolean)
4. **Update DI registration** from `AddTemplateBinder(throwOnMissingParameters)` to `AddTemplateBinder()`
5. **Update injected types** from `IBinderFactory` to `ITemplateFactory`
6. **Update usage** from `binder.Bind()` to `template.Bind()`
7. **Add error handling** for missing parameters (no longer silent)
8. **Update target framework** to .NET 8.0

See [MIGRATION.md](MIGRATION.md) for detailed examples.

---

## [2.x.x] - Previous Versions

For v2.x changelog, see the v2 branch.
