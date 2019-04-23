Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree

Namespace MetaLib

    ''' <summary>
    ''' 在数据库的编号定义比较模糊的情况下, 会需要使用这个模块进行快速匹配搜索
    ''' </summary>
    Public Class TreeSearch

        ReadOnly metaTree As AVLTree(Of MetaLib, MetaLib)
        ReadOnly cutoff#

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="score">认为两个物质注释指的是相同的物质的最低得分</param>
        Sub New(Optional score# = 0.4)
            metaTree = New AVLTree(Of MetaLib, MetaLib)(AddressOf CompareAnnotation, Function(meta) meta.name)
            cutoff = score
        End Sub

        Public Function CompareAnnotation(a As MetaLib, b As MetaLib) As Integer
            Dim score = MetaEquals.Agreement(a, b)

            If score >= cutoff Then
                Return 0
            ElseIf score <= 0 Then
                Return -1
            Else
                Return 1
            End If
        End Function

        Public Function BuildTree(pubchem As IEnumerable(Of MetaLib)) As TreeSearch
            For Each meta As MetaLib In pubchem
                Call metaTree.Add(meta, meta, False)
            Next

            Return Me
        End Function

    End Class
End Namespace