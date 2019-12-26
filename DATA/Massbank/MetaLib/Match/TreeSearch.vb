Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports SMRUCC.MassSpectrum.DATA.MetaLib.Models

Namespace MetaLib

    ''' <summary>
    ''' 在数据库的编号定义比较模糊的情况下, 会需要使用这个模块进行快速匹配搜索
    ''' </summary>
    Public Class TreeSearch

        ReadOnly metaTree As AVLTree(Of MetaInfo, MetaInfo)
        ReadOnly cutoff#
        ReadOnly metaEquals As New MetaEquals

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="score">认为两个物质注释指的是相同的物质的最低得分</param>
        Sub New(Optional score# = 0.4)
            metaTree = New AVLTree(Of MetaInfo, MetaInfo)(AddressOf CompareAnnotation, Function(meta) meta.name)
            cutoff = score
        End Sub

        Public Function CompareAnnotation(a As MetaInfo, b As MetaInfo) As Integer
            Dim score = metaEquals.Agreement(a, b)

            If score >= cutoff Then
                Return 0
            ElseIf score <= 0 Then
                Return -1
            Else
                Return 1
            End If
        End Function

        Public Function BuildTree(pubchem As IEnumerable(Of MetaInfo)) As TreeSearch
            For Each meta As MetaInfo In pubchem
                Call metaTree.Add(meta, meta, False)
            Next

            Return Me
        End Function

        Public Function Search(term As MetaInfo) As MetaInfo()
            Dim result = metaTree.Find(term)

            If result Is Nothing Then
                Return {}
            Else
                Return result.Members
            End If
        End Function

    End Class
End Namespace