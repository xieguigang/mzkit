Imports System.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Models

    Public Class Coordinate

        Public Property HMDB As String
        Public Property Name As String
        ''' <summary>
        ''' 标准曲线的浓度梯度信息
        ''' </summary>
        ''' <returns></returns>
        Public Property C As Dictionary(Of String, Double)
        Public Property ISTD As String
        ''' <summary>
        ''' 内标的编号，需要使用这个编号来分别找到离子对和浓度信息
        ''' </summary>
        ''' <returns></returns>
        Public Property [IS] As String
        ''' <summary>
        ''' 系数因子，值位于[0,1]区间，默认是1
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <[DefaultValue](1)>
        <[Default](1)>
        Public Property Factor As Double

        Public Overrides Function ToString() As String
            Return $"[{[IS]}] {HMDB}: {Name}, {C.GetJson}"
        End Function
    End Class
End Namespace