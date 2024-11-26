Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace LinearQuantitative

    Public Class DataFile

        Public Property filename As String
        Public Property compounds As IonPeakTableRow()

        Sub New(filename As String, compounds As IEnumerable(Of IonPeakTableRow))
            _filename = filename
            _compounds = compounds.ToArray
        End Sub

        Public Overrides Function ToString() As String
            Return $"{compounds.Count} compounds@{filename}: {compounds.Keys.JoinBy("; ")}"
        End Function

    End Class
End Namespace