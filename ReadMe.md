# PayBySquare generátor QR textů pro .NET

Projekt obsahuje nezávislou knihovnu pro slovenský PayBySquare standard QR plateb.

PayBySquare standard byl vytvořen společností ADELANTE, s.r.o. s hlavním důrazem na zbytečnou složitost a obtížnou implementaci tak, aby běžní uživatelé QR plateb museli platit výpalné této společnosti. PayBySquare.TextGenerator.NET řeší tento problém pro implementace v .NET.

Vycházeno z tohoto projektu: https://github.com/PavlinII/PayBySquare.TextGenerator.NET

# PayBySquare standalone QR text generator for .NET

This project provides standalone library for Slovak PayBySquare QR payment standard.

PayBySquare standard was created by ADELANTE, s.r.o. company with main focus on unneeded complexity and difficult implementation. Main purpose is to collect ransom fees from common users of QR payments. PayBySquare.TextGenerator.NET deals with this problem for .NET implementation.

Fork and extension of this project: https://github.com/PavlinII/PayBySquare.TextGenerator.NET

## Consume library

There are different options to consume this library:

* Reference [NuGet package](https://www.nuget.org/packages/PayBySquare.TextGenerator.NET/1.0.0) in your project.
* Download DLL from [Releases](https://github.com/PavlinII/PayBySquare.TextGenerator.NET/releases)
* Clone this repository into your project as a submodule or just copy it's content as a project reference.

## Getting started

Library is prepared as .NET Standard 2.0. Code can be easily adjusted to any other project type.

Simple payment:
```cs
var generator = new PayBySquareTextGenerator.PayBySquareOverkill("CZ1720100000002800266981", 1235.80m, "EUR", "654321", "PayBySquareOverkill");
var text = generator.GeneratePayBySquareOverkillString();
```

Payment can be decorated with other informations:
```cs
var payment = new Payment
{
	Amount = 112.35m,
	CurrencyCode = "EUR",
	BankAccounts = new List<BankAccount>
	{
		new BankAccount("CZ1720100000002800266981", "FIOBCZPPXXX")
	},
	VariableSymbol = "654321",
	ConstantSymbol = "0308",
	SpecificSymbol = "998877",
	PaymentNote = "PayBySquareOverkill note",
	PaymentDueDate = new DateTime(2019,1,1),
};
var generator = new PayBySquareOverkill(payment);
var text = generator.GeneratePayBySquareOverkillString();
```
Standing order example:
```cs
var paymentDay = DateTime.Today.AddDays(2);

var payment = new Payment
{
	Amount = 112.35m,
	CurrencyCode = "EUR",
	BankAccounts = new List<BankAccount>
	{
		new BankAccount("CZ1720100000002800266981", "FIOBCZPPXXX")
	},
	VariableSymbol = "654321",
	ConstantSymbol = "0308",
	SpecificSymbol = "998877",
	BeneficiaryName = "Beneficiary name",
	PaymentDueDate = paymentDay,
	IsStandingOrder = true,
	Periodicity = Periodicity.Monthly,
	LastDate = paymentDay.AddMonths(12)
};

var generator = new PayBySquareOverkill(payment);
var text = generator.GeneratePayBySquareOverkillString();
```

## Next step?

Transfer generated text to QR code using free online generators or custom library.

QR code Error correction level "Medium" is suggested.

## C# implementation

Everything except LZMA namespace can be easily rewriten for C#.

LZMA namespace is based on C# managed-lzma project: [https://github.com/weltkante/managed-lzma](https://github.com/weltkante/managed-lzma)

Do NOT rewrite this LZMA namespace back to C#, that would be waste of time. Link to managed-lzma project directly (start with ManagedLzma.LZMA.AsyncEncoder) or use it's content to replace local code structure.

