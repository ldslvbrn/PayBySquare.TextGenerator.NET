
Imports System.ComponentModel
Imports System.Net

Public Class PayBySquareOverkill

    Public InvoiceID As String
    Public Payments As New List(Of Payment)
    Public Version As Byte = 0               ' 1.0.0 = 0x00 = 0, 1.1.0 = 1 = 0x01

    Public Sub New()
    End Sub

    Public Sub New(P As Payment)
        Payments.Add(P)
    End Sub

    Public Sub New(P As Payment, V As Byte)
        Payments.Add(P)
        Version = V
    End Sub

    Public Sub New(IBAN As String, Amount As Decimal, CurrencyCode As String, VariableSymbol As String, PaymentNote As String)
        Me.New(New Payment(IBAN, Amount, CurrencyCode, VariableSymbol, PaymentNote))
    End Sub

    Public Function GeneratePayBySquareOverkillString() As String
        Dim TS As New TabSerializer, Crc As New Crc32, Enc As New LZMA.Lzma1Encoder, Value(), DataToCompress(), Buff() As Byte
        TS.Append(InvoiceID)
        TS.Append(Payments.Count)
        For Each P As Payment In Payments
            TS.Append(If(P.IsStandingOrder, 2, 1))  'PaymentOptions= paymentorder=1, standingorder=2, directdebit=4
            TS.Append(P.Amount.ToString.Replace(",", "."))
            TS.Append(P.CurrencyCode)
            TS.Append(P.PaymentDueDate)
            TS.Append(P.VariableSymbol)
            TS.Append(P.ConstantSymbol)
            TS.Append(P.SpecificSymbol)
            TS.Append(CStr(Nothing))                'OriginatorsReferenceInformation (VS, SS a KS in SEPA format) - only if VS, KS and SS are empty
            TS.Append(If(P.PaymentNote IsNot Nothing AndAlso P.PaymentNote.Length > 140, P.PaymentNote.Substring(0, 140), P.PaymentNote))
            TS.Append(P.BankAccounts.Count)
            For Each BA As BankAccount In P.BankAccounts
                TS.Append(BA.IBAN)
                TS.Append(BA.BIC)
            Next
            If P.IsStandingOrder Then               'StandingOrderExt structure
                TS.Append(1)
                TS.Append(CStr(Nothing))            'Day only used when different from the day in PaymentDueDate
                'TS.Append(P.StandingOrderDay.Value)
                TS.Append(CStr(Nothing))            'Month only used when specifiyng selection
                'If (P.StandingOrderMonth.HasValue) Then
                '    TS.Append(CInt(Math.Pow(2, P.StandingOrderMonth.Value)))
                'Else
                '    TS.Append(0)
                'End If
                TS.Append(EncodePeriodicity(P.Periodicity.Value))
                TS.Append(P.LastDate)
            Else                                    'No StandingOrderExt structure
                TS.Append(0)
            End If
            TS.Append(0)                            'No DirectDebitExt structure, refer to XSD schema for implementation
            TS.Append(P.BeneficiaryName)
            TS.Append(P.BeneficiaryAddressLine1)
            TS.Append(P.BeneficiaryAddressLine2)
        Next
        Value = Text.Encoding.UTF8.GetBytes(TS.ToString) 'TrimEnd tabs
        DataToCompress = Crc.ComputeHash(Value).Concat(Value).ToArray
        Buff = New Byte() {0, Version}.Concat(BitConverter.GetBytes(CShort(DataToCompress.Length))).Concat(Enc.Encode(DataToCompress)).ToArray    '4x4 bits (Type=0, Version=0, DocumentType=0, Reserved=0) & 16 bit little endian length of DataToCompress (crc & value) & LzmaCompressedData
        Return ToBase32Hex(Buff)
    End Function

    Private Function ToBase32Hex(Buff() As Byte) As String
        Const Base32HexCharset As String = "0123456789ABCDEFGHIJKLMNOPQRSTUV"
        Dim CharIndex As Integer, sb As New Text.StringBuilder, B As Byte, bi, ByteIdx, BitIdx As Integer
        For BitPos As Integer = 0 To Buff.Length * 8 Step 5 'Cist jako bitstream po 5 bitech
            CharIndex = 0
            For bi = 0 To 4
                ByteIdx = (BitPos + bi) \ 8
                BitIdx = 7 - (BitPos + bi) Mod 8
                If ByteIdx = Buff.Length Then
                    B = 0   'Do nasobku 5 bitu se doplnuje nulami na konci
                Else
                    B = Buff(ByteIdx)
                End If
                If (B And (1 << BitIdx)) <> 0 Then CharIndex += 1 << (4 - bi)
            Next
            sb.Append(Base32HexCharset(CharIndex))
        Next
        Return sb.ToString
    End Function

    Private Function EncodePeriodicity(Periodicity As Periodicity) As String
        Dim perStr As String = Nothing
        Select Case Periodicity
            Case Periodicity.Daily
                perStr = "d"
            Case Periodicity.Weekly
                perStr = "w"
            Case Periodicity.Biweekly
                perStr = "b"
            Case Periodicity.Monthly
                perStr = "m"
            Case Periodicity.Bimonthly
                perStr = "B"
            Case Periodicity.Quarterly
                perStr = "q"
            Case Periodicity.Semiannually
                perStr = "s"
            Case Periodicity.Annually
                perStr = "a"
        End Select
        Return perStr
    End Function

End Class
