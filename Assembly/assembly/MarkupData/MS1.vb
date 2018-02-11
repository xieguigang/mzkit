Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.Default
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
        Public Function Ms1Chromatogram(data As IEnumerable(Of (scan_time#, mz#, intensity#)), Optional tolerance As Tolerance = Nothing) As (mz#, chromatogram As ChromatogramTick())

        End Function
    End Module
End Namespace