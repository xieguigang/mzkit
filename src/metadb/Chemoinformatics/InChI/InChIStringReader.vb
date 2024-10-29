Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports ASCII = Microsoft.VisualBasic.Text.ASCII

Namespace IUPAC.InChI

    Public Class InChIStringReader

        ReadOnly prefixData As String()

        Shared ReadOnly prefixes As Index(Of Char) = "chpqbtmsihfr"

        Sub New(tokens As String())
            prefixData = tokens
        End Sub

        Public Function GetByPrefix(c As Char) As String
            If c = ASCII.NUL Then
                Return prefixData.First(Function(t) Not t.First Like prefixes)
            Else
                Return GetStringData(prefixData.FirstOrDefault(Function(t) c = t.First))
            End If
        End Function

        Private Shared Function GetStringData(str As String) As String
            If str.StringEmpty Then
                Return ""
            Else
                Return str.Substring(1)
            End If
        End Function

        Public Function GetByPrefix(any As Char()) As String
            Return GetStringData(prefixData.FirstOrDefault(Function(t) any.Any(Function(cc) cc = t.First)))
        End Function
    End Class
End Namespace