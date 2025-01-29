
# NationalRegisterNumber

![Package Icon](./WilvanBil.NationalRegisterNumber/images/banner.png)

[![NuGet](https://img.shields.io/nuget/v/WilvanBil.NationalRegisterNumber.svg)](https://www.nuget.org/packages/WilvanBil.NationalRegisterNumber)
[![GitHub release](https://img.shields.io/github/v/release/WilvanBil/WilvanBil.NationalRegisterNumber)](https://github.com/WilvanBil/WilvanBil.NationalRegisterNumber/releases)
[![Build](https://github.com/WilvanBil/WilvanBil.NationalRegisterNumber/actions/workflows/NugetPush.yml/badge.svg)](https://github.com/WilvanBil/WilvanBil.NationalRegisterNumber/actions)
[![Tests](https://github.com/WilvanBil/WilvanBil.NationalRegisterNumber/actions/workflows/tests.yml/badge.svg)](https://github.com/WilvanBil/WilvanBil.NationalRegisterNumber/actions)

**NationalRegisterNumber** is a .NET package for generating and validating Belgian national register numbers. The logic is based on the [Official Documentation by the Belgian Government](https://www.ibz.rrn.fgov.be/fileadmin/user_upload/nl/rr/instructies/IT-lijst/IT000_Rijksregisternummer.pdf).

---

## Features

- **Validation**: Verify if a given national register number is valid.
- **Generation**: Generate valid national register numbers with flexible parameters such as birth date and biological sex.
- **Formatting**: Can format a `string` to `YY.MM.DD-XXX.CC` format using `ToFormattedNationalRegisterNumber()` string extension method.
- **Extraction**: Can extract `DateOnly` birthdate and `BiologicalSex` using `TryExtractBirthDate` and `TryExtractBiologicalSex` string extension method.

---

## Installation

Add the package to your project via the .NET CLI:

```bash
dotnet add package WilvanBil.NationalRegisterNumber
```

Alternatively, find it on NuGet Package Manager by searching for `WilvanBil.NationalRegisterNumber`.

---

## Usage

After installation, use the static class `NationalRegisterNumberGenerator` to generate or validate national register numbers.

### **Validation**

The `IsValid` method checks whether a given national register number is valid and returns a boolean (`true`/`false`).

```csharp
bool isValid = NationalRegisterNumberGenerator.IsValid("90022742191");
```

### **Generation**

The `Generate` method creates valid national register numbers. The following overloads are available:

```csharp
string number = NationalRegisterNumberGenerator.Generate();
string number = NationalRegisterNumberGenerator.Generate(DateOnly birthDate);
string number = NationalRegisterNumberGenerator.Generate(BiologicalSex sex);
string number = NationalRegisterNumberGenerator.Generate(DateOnly birthDate, BiologicalSex sex);
string number = NationalRegisterNumberGenerator.Generate(DateOnly minDate, DateOnly maxDate);
string number = NationalRegisterNumberGenerator.Generate(DateOnly minDate, DateOnly maxDate, BiologicalSex sex);
string number = NationalRegisterNumberGenerator.Generate(DateOnly birthDate, int followNumber);
```

#### **Parameters**

- **`followNumber`**: A number between `1` and `998` (inclusive). This parameter helps ensure uniqueness when generating numbers for the same date.
- `minDate` must be greater than or equal to `1900/01/01`.

##### **Exceptions**

If the input parameters are invalid, the following exceptions may be thrown:

- `ArgumentException`: Indicates an invalid parameter with an explanatory message.

### **Formatting**

You can format Belgian National Register Numbers into the official format (`YY.MM.DD-XXX.CC`) using the `ToFormattedNationalRegisterNumber` string extension method.

```csharp
string nationalRegisterNumber = "90020200395";
string formattedNumber = nationalRegisterNumber.ToFormattedNationalRegisterNumber();
Console.WriteLine(formattedNumber); // Output: 90.02.02-003.95
```

#### Notes

- **Input Validation**: The method will return the original string if:
  - The input is null, empty, or not exactly 11 characters.
  - You can combine this with the `IsValid` method from `NationalRegisterNumberGenerator` for additional validation.

### **Extraction**

You can extract `DateOnly` or `BiologicalSex` using `TryExtractBiologicalSex` and `TryExtractBirthDate`, however it will not check for full validity of the given input.
For this you can use the `IsValid` method.

```csharp
var nationalRegisterNumber = "90020200395";

if (!NationalRegisterNumberGenerator.IsValid(nationalRegisterNumber))
    return;

if (nationalRegisterNumber.TryExtractBiologicalSex(out var sex))
    Console.WriteLine($"Biological Sex: {sex}"); 
else
    Console.WriteLine("Invalid biological sex.");

if (nationalRegisterNumber.TryExtractBirthDate(out var birthDate))
    Console.WriteLine($"Birth Date: {birthDate}"); 
else
    Console.WriteLine("Invalid birth date.");

```

---

## Example Usage

### Validate a National Register Number

```csharp
bool isValid = NationalRegisterNumberGenerator.IsValid("90022742191");
Console.WriteLine($"Is valid: {isValid}");
```

### Generate and format a Random National Register Number

```csharp
string randomNumber = NationalRegisterNumberGenerator.Generate();
Console.WriteLine($"Generated number: {randomNumber.ToFormattedNationalRegisterNumber()}");
```

### Generate with Specific Parameters

```csharp
string numberByDate = NationalRegisterNumberGenerator.Generate(new DateOnly(1990, 1, 1));
string numberBySex = NationalRegisterNumberGenerator.Generate(BiologicalSex.Male);
string numberByDateAndSex = NationalRegisterNumberGenerator.Generate(new DateOnly(1990, 1, 1), BiologicalSex.Female);
```

### Extract birthdate

```csharp
var nationalRegisterNumber = "90020200395";

if (nationalRegisterNumber.TryExtractBirthDate(out var birthDate))
    Console.WriteLine($"Birth Date: {birthDate}"); 
else
    Console.WriteLine("Invalid birth date.");

```

---

## Contributing

We welcome contributions to this package! To get started:

1. Fork the repository: [WilvanBil/NationalRegisterNumber](https://github.com/WilvanBil/NationalRegisterNumber).
2. Clone your fork:

   ```bash
   git clone https://github.com/YourUsername/NationalRegisterNumber.git
   ```

3. Create a new branch for your feature or bug fix:

   ```bash
   git checkout -b feature/my-new-feature
   ```

4. Make your changes and ensure all tests pass.
5. Submit a pull request with a clear description of your changes.

### Issues

For any issues, requests or any other kind of feedback, please consider creating an [issue](https://github.com/WilvanBil/WilvanBil.NationalRegisterNumber/issues/new?template=Blank+issue).

### Running Tests

Before submitting your changes, run the test suite:

```bash
dotnet test
```

---

## !! WARNING

This package is intended for **testing and research purposes only**. Do **not** use it in a live or production environment. It is specifically designed for **unit and integration testing** scenarios.

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.
