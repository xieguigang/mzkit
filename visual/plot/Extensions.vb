Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.MassSpectrum.Math.MSMS
Imports SMRUCC.MassSpectrum.Visualization.DATA.SpectrumJSON

Public Module Extensions

    ''' <summary>
    ''' Creates plot data from matrix
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function SpectrumFromMatrix(matrix As IEnumerable(Of LibraryMatrix)) As IEnumerable(Of SpectrumData)
        For Each group As LibraryMatrix In matrix
            Yield New SpectrumData With {
                .name = group.Name,
                .data = group _
                    .Select(Function(l)
                                Return New IntensityCoordinate With {
                                    .x = l.mz,
                                    .y = l.intensity
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
