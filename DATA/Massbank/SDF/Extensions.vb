Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text

Namespace File

    Public Module Extensions

        ''' <summary>
        ''' Generic method for parsing the SDF meta annotation data.
        ''' </summary>
        ''' <typeparam name="MetaData"></typeparam>
        ''' <param name="sdf"></param>
        ''' <param name="properties"></param>
        ''' <returns></returns>
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