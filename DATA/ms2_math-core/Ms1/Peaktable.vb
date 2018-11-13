
''' <summary>
''' 一级信息表
''' </summary>
Public Class Peaktable
    Implements IMs1
    Implements IRetentionTime

    ''' <summary>
    ''' 可以是差异代谢物的编号
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String
    Public Property mz As Double Implements IMs1.mz
    Public Property mzmin As Double
    Public Property mzmax As Double
    Public Property rt As Double Implements IMs1.rt
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property into As Double
    Public Property intb As Double
    Public Property maxo As Double
    Public Property sn As Double
    Public Property sample As String
    Public Property index As Double
    ''' <summary>
    ''' The scan number
    ''' </summary>
    ''' <returns></returns>
    Public Property scan As Integer
    ''' <summary>
    ''' CID/HCD
    ''' </summary>
    ''' <returns></returns>
    Public Property ionization As String
    Public Property energy As String

    Public Overrides Function ToString() As String
        Return $"{mz}@{rt}#{scan}-{ionization}-{energy}"
    End Function
End Class

Public Class ROITable : Implements IRetentionTime

    Public Property ID As String

    Public Property rtmin As Double
    Public Property rtmax As Double

    Public Property rt As Double Implements IRetentionTime.rt
    ''' <summary>
    ''' 保留指数
    ''' </summary>
    ''' <returns></returns>
    Public Property ri As Double

    ''' <summary>
    ''' 这个区域的最大峰高度
    ''' </summary>
    ''' <returns></returns>
    Public Property maxInto As Double
    ''' <summary>
    ''' 所计算出来的基线的响应强度
    ''' </summary>
    ''' <returns></returns>
    Public Property baseline As Double
    ''' <summary>
    ''' 当前的这个ROI的峰面积积分值
    ''' </summary>
    ''' <returns></returns>
    Public Property integration As Double

End Class