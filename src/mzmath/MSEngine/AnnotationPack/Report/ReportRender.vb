Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Public Class ReportRender

    ReadOnly annotation As AnnotationPack

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' multiple <see cref="AlignmentHit"/> value for multiple precursor type
    ''' </remarks>
    ReadOnly metabolites As New Dictionary(Of String, AlignmentHit)

    Sub New(pack As AnnotationPack)
        annotation = pack

        For Each libs In pack.libraries
            For Each hit In libs.Value
                Dim key As String = hit.biodeep_id & "_" & hit.adducts

                If Not metabolites.ContainsKey(key) Then
                    Call metabolites.Add(key, hit)
                End If
            Next
        Next
    End Sub

    Public Function HtmlTable(biodeep_ids As IEnumerable(Of String), Optional rt_cell As Boolean = True) As String
        Dim html As New StringBuilder
        Dim lines = Tabular(biodeep_ids, rt_cell).ToArray

        Call html.AppendLine("<table>")
        Call html.AppendLine("<thead>")
        Call html.AppendLine("<th>")
        Call html.AppendLine(lines(0))
        Call html.AppendLine("</th>")
        Call html.AppendLine("</thead>")
        Call html.AppendLine("<tbody>")

        For Each row As String In lines.Skip(1)
            Call html.AppendLine("<tr>")
            Call html.AppendLine(row)
            Call html.AppendLine("</tr>")
        Next

        Call html.AppendLine("</tbody>")
        Call html.AppendLine("</table>")

        Return html.ToString
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="biodeep_ids"></param>
    ''' <returns>
    ''' iterates the html table text, the first element is always the table header title row.
    ''' </returns>
    Public Iterator Function Tabular(biodeep_ids As IEnumerable(Of String), Optional rt_cell As Boolean = True) As IEnumerable(Of String)
        Dim metabolites = makeSubset(biodeep_ids)
        Dim ordinals = metabolites.Keys.ToArray

        ' generates the headers
        Yield ordinals _
            .Select(Function(id) metabolites(id)) _
            .Select(Function(a)
                        Dim name = a.name
                        Dim adducts = a.adducts

                        Return $"<td>{name.Replace("<", "&lt;")}<br />{adducts}</td>"
                    End Function) _
            .JoinBy("")

        For Each sample As String In annotation.samplefiles
            Yield ordinals _
                .Select(Function(id)
                            Dim annotation = metabolites(id)

                            If annotation.samplefiles.ContainsKey(sample) Then
                                If rt_cell Then
                                    Return $"<td>{(annotation(sample).rt / 60).ToString("F1")}</td>"
                                Else
                                    Return $"<td>{annotation(sample).score}</td>"
                                End If
                            Else
                                Return "<td>n/a</td>"
                            End If
                        End Function) _
                .JoinBy("")
        Next
    End Function

    Private Function makeSubset(biodeep_ids As IEnumerable(Of String)) As Dictionary(Of String, AlignmentHit)
        Dim subset As New Dictionary(Of String, AlignmentHit)

        For Each id As String In biodeep_ids.Distinct.SafeQuery
            Call subset.Add(id, metabolites.TryGetValue(id))
        Next

        Return subset
    End Function
End Class
