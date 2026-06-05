# ItalianFiscalCode.Toolkit

A lightweight C# (.NET Framework) library for managing, validating, cross-checking, and building the Italian Fiscal Code.

## Functionality
* **Formal validation:** Verifies the correctness of the check digit based on the official algorithm.
* **Personal Data Correspondence:** Check if a tax code corresponds to a person's real data (Name, Surname, Date of Birth, Gender, Municipality).
* **Build Fiscal Code:** Build of Fiscal Code with person data

## How to use it

### 1. Validation of the last letter (Check Digit)
If you just want to check that the code is written mathematically correctly:

```csharp
using ItalianFiscalCode.Toolkit;

bool isValid=FiscalCode.CheckDigit("RSSMRA70C01H501W")
// Return True if OK, False if KO
```

### 2. Check correspondence of personal data
If you want verify Fiscal Code with Personal Data

You must set the personal data in the `PersonInfo` class

```csharp
PersonInfo info = new PersonInfo() { 
	Name="MARIO",
	Surname="ROSSI",
	BirthDate="01/03/1970",
	BirthPlaceCode="H501",
	Gender="M"
};
```
Then call the `MatchPerson` method

```csharp
string fiscalCode = "RSSMRA70C01H501W";
bool isValid=FiscalCode.MatchPerson(info, fiscalCode);
// return True if OK, False if KO
```

### 3. Build Fiscal Code
For build Fiscal Code with person data:

You must set the personal data in the `PersonInfo` class

```csharp
PersonInfo info = new PersonInfo() { 
	Name="MARIO",
	Surname="ROSSI",
	BirthDate="01/03/1970",
	BirthPlaceCode="H501",
	Gender="M"
};
```

Then call the `BuildFiscalCode` method

```csharp
OutcomeOp outcome = FiscalCode.BuildFiscalCode(info);
// outcome is :
bool success=outcome.Outcome;
// Error (if outcome.Outcome is false) :
string error=outcome.Response;
```

