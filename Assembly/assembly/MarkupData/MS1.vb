Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Namespace MarkupData

    Public Module MS1Extensions

        ReadOnly ppm20 As DefaultValue(Of Tolerance) = New PPMmethod(20).Interface

        ''' <summary>
        ''' 将质谱之中的ms1的结果，按照mz进行分组，之后再按照时间排序即可得到随时间变化的信号曲线
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Iterator Function Ms1Chromatogram(data As IEnumerable(Of (scan_time#, mz#, intensity#)), Optional tolerance As Tolerance = Nothing) As IEnumerable(Of (mz#, chromatogram As ChromatogramTick()))
            Dim mzGroup = data.GroupBy(Function(d) d.mz, equals:=AddressOf (tolerance Or ppm20).Assert)

            For Each mz As NamedCollection(Of (scan_time#, mz#, intensity#)) In mzGroup
                Dim ticks = mz _
                    .Select(Function(tick)
                                Return New ChromatogramTick With {
                                    .Time = tick.scan_time,
                                    .Intensity = tick.intensity
                                }
                            End Function) _
                    .OrderBy(Function(tick) tick.Time) _
                    .ToArray

                Yield (Val(mz.Name), ticks)
            Next
        End Function
    End Module
End Namespace