#Region "Microsoft.VisualBasic::7645a0bbe07c6dd2ddf1a1aa2e64d241, src\assembly\assembly\MarkupData\MS1.vb"

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

    '     Module MS1Extensions
    ' 
    '         Function: Ms1Chromatogram, QuantileBaseline
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile

Namespace MarkupData

    Public Module MS1Extensions

        ReadOnly ppm50 As [Default](Of Tolerance) = New PPMmethod(50).Interface

        ''' <summary>
        ''' 将质谱之中的ms1的结果，按照mz进行分组，之后再按照时间排序即可得到随时间变化的信号曲线
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Iterator Function Ms1Chromatogram(data As IEnumerable(Of (scan_time#, mz#, intensity#)), Optional tolerance As Tolerance = Nothing) As IEnumerable(Of (mz#, chromatogram As ChromatogramTick()))
            Dim mzGroup = data.GroupBy(Function(d) d.mz, equals:=tolerance Or ppm50)

            For Each mz As IGrouping(Of String, (scan_time#, mz#, intensity#)) In mzGroup
                Dim mzValue# = Val(mz.Key)
                Dim ticks = mz _
                    .Select(Function(tick)
                                Return New ChromatogramTick With {
                                    .Time = tick.scan_time,
                                    .Intensity = tick.intensity
                                }
                            End Function) _
                    .OrderBy(Function(tick) tick.Time) _
                    .ToArray

                Yield (mzValue, ticks)
            Next
        End Function

        <Extension>
        Public Function QuantileBaseline(data As IEnumerable(Of (mz#, chromatogram As ChromatogramTick())), Optional quantile# = 0.6) As IEnumerable(Of (mz#, chromatogram As ChromatogramTick()))
            Dim metabolites = data.ToArray
            Dim gkquantile = metabolites _
                .Select(Function(m) m.chromatogram) _
                .IteratesALL _
                .Shadows!Intensity _
                .GKQuantile
            Dim baseline# = gkquantile.Query(quantile)

            Return metabolites _
                .Where(Function(m) m.chromatogram.Length > 2) _
                .Select(Function(m)
                            Dim removes As ChromatogramTick() = m _
                                .chromatogram _
                                .Where(Function(c) c.Intensity >= baseline) _
                                .ToArray
                            Return (m.mz, removes)
                        End Function) _
                .Where(Function(m)
                           Return Not m.Item2.Length = 0
                       End Function)
        End Function
    End Module
End Namespace
