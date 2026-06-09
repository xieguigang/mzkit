Namespace PeakAlignment

    ''' <summary>
    ''' 内部使用的对齐峰组，表示一个对齐后的离子特征
    ''' </summary>
    Public Class AlignedPeakGroup
        ''' <summary>组内所有峰的m/z平均值</summary>
        Public Property avgMz As Double
        ''' <summary>组内所有峰的m/z最小值</summary>
        Public Property minMz As Double
        ''' <summary>组内所有峰的m/z最大值</summary>
        Public Property maxMz As Double
        ''' <summary>组内所有峰的RT平均值</summary>
        Public Property avgRt As Double
        ''' <summary>组内所有峰的RT最小值</summary>
        Public Property minRt As Double
        ''' <summary>组内所有峰的RT最大值</summary>
        Public Property maxRt As Double
        ''' <summary>当前组中各样本的峰面积，键名为样本文件名</summary>
        Public Property sampleAreas As New Dictionary(Of String, Double)
        ''' <summary>当前组中各样本的峰对象引用，用于后续缺失值填充</summary>
        Public Property samplePeaks As New Dictionary(Of String, PeakFeature)
    End Class
End Namespace