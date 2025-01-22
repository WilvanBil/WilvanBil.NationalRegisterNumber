
# National Register Number

**National Register Number** is a .NET package for generating and validating Belgian national register numbers. The logic is based on the [Official Documentation by the Belgian Government](https://www.ibz.rrn.fgov.be/fileadmin/user_upload/nl/rr/instructies/IT-lijst/IT000_Rijksregisternummer.pdf).

---

## Features
- **Validation**: Verify if a given national register number is valid.
- **Generation**: Generate valid national register numbers with flexible parameters such as birth date and biological sex.

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

#### **Exceptions**

If the input parameters are invalid, the following exceptions may be thrown:
- `ArgumentException`: Indicates an invalid parameter with an explanatory message.

---

## Example Usage

### Validate a National Register Number
```csharp
bool isValid = NationalRegisterNumberGenerator.IsValid("90022742191");
Console.WriteLine($"Is valid: {isValid}");
```

### Generate a Random National Register Number
```csharp
string randomNumber = NationalRegisterNumberGenerator.Generate();
Console.WriteLine($"Generated number: {randomNumber}");
```

### Generate with Specific Parameters
```csharp
string numberByDate = NationalRegisterNumberGenerator.Generate(new DateOnly(1990, 1, 1));
string numberBySex = NationalRegisterNumberGenerator.Generate(BiologicalSex.Male);
string numberByDateAndSex = NationalRegisterNumberGenerator.Generate(new DateOnly(1990, 1, 1), BiologicalSex.Female);
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

### Running Tests

Before submitting your changes, run the test suite:
```bash
dotnet test
```

---

## !! WARNING !!
This package is intended for **testing and research purposes only**. Do **not** use it in a live or production environment. It is specifically designed for **unit and integration testing** scenarios.

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.
