# Belgian National Register Number Library - AI Coding Instructions

## Project Overview

This is a .NET library for generating, validating, formatting, and extracting data from Belgian national register numbers. The validation logic follows the [official Belgian government specification](https://www.ibz.rrn.fgov.be/fileadmin/user_upload/nl/rr/instructies/IT-lijst/IT000_Rijksregisternummer.pdf).

## Architecture

### Core Components

- **`NationalRegisterNumberGenerator`**: Static class containing all generation and validation logic
- **`NationalRegisterNumberStringExtensions`**: Extension methods for formatting and data extraction
- **`BiologicalSex`**: Simple enum (Male/Female) used in generation and extraction

### Critical Business Logic

The Belgian national register number format is `YYMMDDXXXcc`:

- `YYMMDD`: Birth date (2-digit year, month, day)
- `XXX`: Follow number (001-998), where **odd = male, even = female**
- `cc`: Checksum calculated as `97 - (dividend % 97)` where dividend is the first 9 digits
- **Year 2000 ambiguity**: For births after 1999, prepend `2` to the 9-digit string before calculating the checksum

### Key Invariants

- Minimum birth date: `1900-01-01` (enforced in `_absoluteMinDate`)
- Follow numbers: Must be between 1-998 inclusive
- National register numbers are **always 11 digits** when unformatted
- Validation accepts both formatted (`YY.MM.DD-XXX.CC`) and unformatted strings

## Testing Standards

### Test Framework Stack

- **xUnit**: Primary test framework
- **FluentAssertions**: For readable assertions (use `.Should()` syntax)
- **FsCheck**: Property-based testing for exhaustive validation (see `ValidationTests.cs`)

### Testing Patterns

```csharp
// Standard test structure (AAA pattern)
// Arrange
var birthDate = new DateOnly(1990, 1, 1);

// Act
var result = NationalRegisterNumberGenerator.Generate(birthDate);

// Assert
result.Should().BeTrue();
```

### Property-Based Testing

When validating logic against all possible inputs, use FsCheck's `[Property]` attribute:

```csharp
[Property(Verbose = true)]
public void GenerateShouldWorkForAnyDateTime(DateTime birthDate)
```

### Test Coverage Requirements

- All public methods require tests
- Include edge cases: date boundaries (1900, 1999/2000 transition), follow number limits
- Test invalid inputs throw `ArgumentException` with descriptive messages

## Development Workflows

### Building and Testing

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test
```

### NuGet Package Creation

- Multi-target: `net8.0` and `net9.0`
- Auto-generated on build (`GeneratePackageOnBuild=true`)
- Includes XML documentation and symbol packages (`.snupkg`)
- Icon and README included in package assets

### Versioning

Uses [Versionize](https://github.com/versionize/versionize) with Conventional Commits:

- `feat:` → Minor version bump, appears in Features section
- `fix:` → Patch version bump, appears in Bug Fixes section
- `perf:` → Patch version bump, appears in Performance section
- Commit message format matters for CHANGELOG.md generation

### CI/CD

- **Tests**: Run automatically on push/PR to `master` (`tests.yml`)
- **Publishing**: Manual workflow dispatch requiring tag input (`NugetPush.yml`)
- All tests must pass before publishing to NuGet

## Code Conventions

### Nullable Reference Types

- **Enabled globally** (`<Nullable>enable</Nullable>`)
- Use `out` parameters with nullable types: `out DateOnly? birthDate`, `out BiologicalSex? sex`
- Return `false` from `Try*` methods and set out params to `null` on failure

### Extension Method Patterns

```csharp
// Always check length first
if (string.IsNullOrWhiteSpace(input) || input.Length != NationalRegisterNumberLength)
    return fallbackValue;

// Use Span<T> for performance when parsing substrings
if (!int.TryParse(nationalRegisterNumber.AsSpan(6, 3), out int followNumber))
```

### Exception Handling

Throw `ArgumentException` with **specific messages** explaining the constraint:

```csharp
throw new ArgumentException($"Birthdate can't be before {_absoluteMinDate.ToShortDateString()}", nameof(birthDate));
```

## Important Design Decisions

### Why Random.Shared?

Uses `Random.Shared` for thread-safe random number generation in modern .NET.

### Why No Full Validation in Extraction Methods?

`TryExtractBirthDate` and `TryExtractBiologicalSex` deliberately **skip checksum validation** for performance. Callers should use `IsValid()` first if full validation is needed.

### FluentAssertions Version Lock

Test project locks FluentAssertions to `[7.1.0,8.0.0)` to avoid version 8's licensing changes.

## Common Pitfalls

1. **Don't forget the Y2K adjustment**: Numbers born after 1999 require prepending `2` to the dividend for checksum calculation
2. **Sex encoding is in the last digit of follow number**: Check `result[8]` (9th character), not the entire follow number
3. **Formatted strings are accepted**: `IsValid()` filters to digits only, so `90.02.27-421.91` works
4. **Date ranges**: When generating with min/max dates, ensure `minDate <= maxDate` or throw `ArgumentException`

## Quick Reference

### Adding New Overloads

Follow existing patterns in `NationalRegisterNumberGenerator.cs`:

1. Create public overload with specific parameters
2. Delegate to `Generate(DateOnly birthDate, int followNumber)` base method
3. Add comprehensive xUnit tests with valid/invalid cases
4. Use FluentAssertions for assertions

### Adding New Extension Methods

Add to `NationalRegisterNumberStringExtensions.cs` and follow the `Try*` pattern for data extraction with `out` parameters.
