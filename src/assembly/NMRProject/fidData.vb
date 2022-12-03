''' <summary>
''' NMR方法检测到的与时间相关的原子震荡数据
''' </summary>
Public Class fidData

    Public Property time As Double()
    Public Property amplitude As Double()

    ''' <summary>
    ''' 基于傅里叶变换将时域数据转换为频域数据
    ''' </summary>
    ''' <returns></returns>
    Public Function FourierTransform() As FrequencyData

    End Function

End Class

Public Class FrequencyData

    Public Property frequency As Double()
    Public Property amplitude As Double()

End Class
