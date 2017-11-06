Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text

Namespace File

    Public Module Extensions

        <Extension>
        Public Function Data(Of MetaData As {New, Class})(sdf As SDF, properties As Dictionary(Of String, PropertyInfo)) As MetaData
            Dim table As Dictionary(Of String, String()) = sdf.MetaData
            Dim meta As Object = New MetaData

            For Each key As String In table.Keys
                Call properties(key).SetValue(meta, table(key).JoinBy(ASCII.LF))
            Next

            Return DirectCast(meta, MetaData)
        End Function
    End Module
End Namespace