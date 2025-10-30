Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Runtime

Public Class QuantifyValue : Inherits Value(Of String)

    Public ReadOnly Property NumberValue As Double
        Get
            Select Case Strings.UCase(Value)
                Case "NA", "N/A" : Return Double.NaN
                Case "ND", "NF", "N/D", "N/F", "" : Return 0
                Case Else
                    Return Value.ParseDouble
            End Select
        End Get
    End Property

    Sub New(data As String)
        Value = data
    End Sub

    Public Overrides Function ToString() As String
        Return Value
    End Function

    Public Overloads Shared Narrowing Operator CType(val As QuantifyValue) As Double
        Return val.NumberValue
    End Operator

End Class

Public Class QuantifyStringParser : Implements IParser

    Public Overloads Function ToString(obj As Object) As String Implements IParser.ToString
        Return If(obj Is Nothing, "", obj.ToString)
    End Function

    Public Function TryParse(content As String) As Object Implements IParser.TryParse
        Return New QuantifyValue(content)
    End Function
End Class