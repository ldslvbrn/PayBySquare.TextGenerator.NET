
Public Class Payment

    'Mandatory
    Public Amount As Decimal
    Public CurrencyCode As String
    Public BankAccounts As New List(Of BankAccount)

    'Optional
    Public VariableSymbol, ConstantSymbol, SpecificSymbol, PaymentNote, BeneficiaryName, BeneficiaryAddressLine1, BeneficiaryAddressLine2 As String
    Public PaymentDueDate? As Date

    'Standing order
    Public IsStandingOrder As Boolean = False
    'Public StandingOrderDay? As Integer
    'Public StandingOrderMonth? As Integer
    Public Periodicity? As Periodicity = Nothing
    Public LastDate? As Date = Nothing

    Public Sub New()
    End Sub

    Public Sub New(IBAN As String, Amount As Decimal, CurrencyCode As String, VariableSymbol As String, PaymentNote As String)
        BankAccounts.Add(New BankAccount(IBAN))
        Me.Amount = Amount : Me.CurrencyCode = CurrencyCode : Me.VariableSymbol = VariableSymbol : Me.PaymentNote = PaymentNote
    End Sub

End Class
