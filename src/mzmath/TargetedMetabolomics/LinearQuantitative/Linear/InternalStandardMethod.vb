
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content

Namespace LinearQuantitative.Linear

    ''' <summary>
    ''' 普通标准曲线计算算法模块
    ''' </summary>
    Public Module InternalStandardMethod

        <Extension>
        Public Iterator Function ToLinears(points As IEnumerable(Of TargetPeakPoint), contents As ContentTable) As IEnumerable(Of StandardCurve)
            For Each feature As IEnumerable(Of TargetPeakPoint) In points.GroupBy(Function(p) p.Name)
                Yield feature.ToFeatureLinear(contents)
            Next
        End Function

        <Extension>
        Private Function ToFeatureLinear(points As IEnumerable(Of TargetPeakPoint), contents As ContentTable) As StandardCurve

        End Function
    End Module
End Namespace