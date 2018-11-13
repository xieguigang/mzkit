#Region "Microsoft.VisualBasic::32fa6085cc3dc57912378d13337b9126, plot\Extensions.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module Extensions
    ' 
    '     Function: ReleaseD3js, SpectrumFromMatrix
    ' 
    ' /********************************************************************************/

#End Region

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

