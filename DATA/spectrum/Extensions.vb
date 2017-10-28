Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.MassSpectrum.Assembly

Public Module Extensions

    ''' <summary>
    ''' Creates plot data from matrix
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function SpectrumFromMatrix(matrix As IEnumerable(Of LibraryMatrix)) As IEnumerable(Of spectrumData)
        Dim groups = matrix.GroupBy(Function(l) l.Name)

        For Each group As IGrouping(Of String, LibraryMatrix) In groups
            Yield New spectrumData With {
                .name = group.Key,
                .data = group _
                    .Select(Function(l)
                                Return New MSSignal With {
                                    .x = l.ProductMz,
                                    .y = l.LibraryIntensity
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function

    <Extension>
    Public Function ReleaseD3js(out$, data$) As Boolean
        Try
            Call My.Resources.App.SaveTo(out & "/App.js")
            Call My.Resources.data.Replace("$data", data).SaveTo(out & "/data.js")
            Call My.Resources.index.SaveTo(out & "/index.html")
            Call My.Resources.highcharts_src_delta.SaveTo(out & "/lib/highcharts.src.delta.js")
            Call My.Resources.excanvas_compiled.SaveTo(out & "/lib/excanvas.compiled.js")
            Call My.Resources.jquery_1_6_1_min.SaveTo(out & "/jquery-1.6.1.min.js")
            Call My.Resources.attn.SaveAs(out & "/images/attn.png")
        Catch ex As Exception
            Call App.LogException(ex)
            Return False
        End Try

        Return True
    End Function
End Module
