Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' The massbank data records
''' </summary>
Public Class Record : Implements INamedValue

    Public Property ACCESSION As String Implements INamedValue.Key

End Class

Public Structure PeakData

    Dim mz As Double
    Dim int As Double
    Dim relint As Double

End Structure