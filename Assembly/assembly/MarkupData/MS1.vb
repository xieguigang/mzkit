Imports System.Runtime.CompilerServices
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Namespace MarkupData

    Public Module MS1Extensions

        ''' <summary>
        ''' 将质谱之中的ms1的结果，按照mz进行分组，之后再按照时间排序即可得到随时间变化的信号曲线
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function Ms1Chromatogram(data As IEnumerable(Of (scan_time#, mz#, intensity#))) As (mz#, chromatogram As ChromatogramTick())

        End Function
    End Module
End Namespace