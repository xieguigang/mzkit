Imports Microsoft.VisualBasic.Data.IO

Public Class BlockNode

    Public Property Id As String
    Public Property Block As BufferRegion
    ''' <summary>
    ''' 得分0.9以上的都算作为当前节点的等价谱图
    ''' </summary>
    ''' <returns></returns>
    Public Property Members As List(Of Integer)

    ''' <summary>
    ''' 总共10个元素，分别表示[0,1]区间内的10个阈值等级
    ''' 0,0.1,0.2,0.3,0.4,0.5,0.6,0.7,0.8,0.9
    ''' </summary>
    ''' <returns></returns>
    Public Property childs As Integer()

End Class
