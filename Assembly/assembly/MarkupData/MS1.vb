Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Namespace MarkupData

    Public Module MS1Extensions

        ReadOnly ppm50 As DefaultValue(Of Tolerance) = New PPMmethod(50).Interface

        ''' <summary>
        ''' 将质谱之中的ms1的结果，按照mz进行分组，之后再按照时间排序即可得到随时间变化的信号曲线
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Iterator Function Ms1Chromatogram(data As IEnumerable(Of (scan_time#, mz#, intensity#)), Optional tolerance As Tolerance = Nothing) As IEnumerable(Of (mz#, chromatogram As ChromatogramTick()))
            Dim mzGroup = data.GroupBy(Function(d) d.mz, equals:=AddressOf (tolerance Or ppm50).Assert)

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
