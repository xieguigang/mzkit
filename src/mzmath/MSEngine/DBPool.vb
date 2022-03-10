Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' database pool of the metabolites database
''' </summary>
Public Class DBPool

    Protected ReadOnly metadb As New Dictionary(Of String, IMzQuery)

    Public Sub Register(name As String, database As IMzQuery)
        If Not database Is Nothing Then
            metadb(name) = database
        End If
    End Sub

    Public Function getAnnotation(uniqueId As String) As (name As String, formula As String)
        For Each db In metadb.Values
            Dim result = db.GetAnnotation(uniqueId)

            If Not (result.formula.StringEmpty AndAlso result.name.StringEmpty) Then
                Return result
            End If
        Next

        Return Nothing
    End Function

    Public Iterator Function QueryByMz(mz As Double) As IEnumerable(Of NamedCollection(Of MzQuery))
        For Each xrefDb In metadb
            Yield New NamedCollection(Of MzQuery) With {
                .name = xrefDb.Key,
                .value = xrefDb.Value.QueryByMz(mz).ToArray
            }
        Next
    End Function

    Public Iterator Function MSetAnnotation(mzlist As IEnumerable(Of Double)) As IEnumerable(Of NamedCollection(Of MzQuery))
        Dim allMz As Double() = mzlist.ToArray

        For Each xrefDb In metadb
            Yield New NamedCollection(Of MzQuery) With {
                .name = xrefDb.Key,
                .value = xrefDb.Value _
                    .MSetAnnotation(allMz) _
                    .ToArray
            }
        Next
    End Function
End Class
