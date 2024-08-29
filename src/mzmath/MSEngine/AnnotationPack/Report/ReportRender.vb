Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Public Class ReportRender

    Public ReadOnly Property annotation As AnnotationPack

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' multiple <see cref="AlignmentHit"/> value for multiple precursor type
    ''' </remarks>
    ReadOnly metabolites As New Dictionary(Of String, AlignmentHit)

    Public Property colorSet As String() = {"#0D0887FF", "#3E049CFF", "#6300A7FF", "#8707A6FF", "#A62098FF", "#C03A83FF", "#D5546EFF", "#E76F5AFF", "#F58C46FF", "#FDAD32FF", "#FCD225FF", "#F0F921FF"}

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
        Call html.AppendLine("<tr>")
        Call html.AppendLine(lines(0))
        Call html.AppendLine("</tr>")
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
        Dim ranges = ordinals _
            .ToDictionary(Function(key) key,
                          Function(id)
                              Dim result = metabolites(id)
                              Dim data = result.samplefiles.Values

                              If rt_cell Then
                                  Return data.Select(Function(a) a.rt / 60).Range
                              Else
                                  Return data.Select(Function(a) a.score).Range
                              End If
                          End Function)
        Dim levels As Integer = colorSet.Length
        Dim index As New DoubleRange(0, levels)

        ' generates the headers
        Yield "<th>Samples</th>" & ordinals _
            .Select(Function(id) metabolites(id)) _
            .Select(Function(a)
                        Dim name = a.name
                        Dim adducts = a.adducts

                        ' metabolite name
                        Return $"<th><a href='#' class='meta_header' xcms_id='{a.xcms_id}'>{name.Replace("<", "&lt;")}<br />{adducts}</a></th>"
                    End Function) _
            .JoinBy("")

        For Each sample As String In annotation.samplefiles
            Yield $"<td>{sample}</td>" & ordinals _
                .Select(Function(id)
                            Dim annotation = metabolites(id)

                            If annotation.samplefiles.ContainsKey(sample) Then
                                Dim score As Double = If(rt_cell,
                                    std.Round(annotation(sample).rt / 60, 2),
                                    std.Round(annotation(sample).score, 2))
                                Dim range As DoubleRange = ranges(id)
                                Dim offset As Integer = range.ScaleMapping(score, index)

                                If offset < 0 Then
                                    offset = 0
                                ElseIf offset > levels - 1 Then
                                    offset = levels - 1
                                End If

                                Return $"<td data_id='{id}' data_sample='{sample}' style='background-color:{colorSet(offset)}'>{score}</td>"
                            Else
                                Return "<td>NA</td>"
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
