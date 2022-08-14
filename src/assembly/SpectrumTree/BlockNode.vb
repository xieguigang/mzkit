Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
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
    Public Property centroid As ms2()

    Public ReadOnly Property isLeaf As Boolean
        Get
            Return childs.IsNullOrEmpty
        End Get
    End Property

    Public ReadOnly Property isBlank As Boolean
        Get
            ' ZERO always is root
            ' and root can not be a child
            ' so zero means no data at here
            Return childs.All(Function(c) c <= 0)
        End Get
    End Property

    Public Shared Function GetIndex(score As Double) As Integer
        If score > 0.9 Then
            Return -1
        ElseIf score > 0.8 Then
            Return 0
        ElseIf score > 0.7 Then
            Return 1
        ElseIf score > 0.6 Then
            Return 2
        ElseIf score > 0.5 Then
            Return 3
        ElseIf score > 0.4 Then
            Return 4
        ElseIf score > 0.3 Then
            Return 5
        ElseIf score > 0.2 Then
            Return 6
        ElseIf score > 0.1 Then
            Return 7
        ElseIf score > 0 Then
            Return 8
        Else
            Return 9
        End If
    End Function

End Class
