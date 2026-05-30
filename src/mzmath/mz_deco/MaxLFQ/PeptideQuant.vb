Namespace MaxLFQ

    ' 定义数据结构
    Public Class PeptideQuant
        Public Property PeptideID As String       ' 多肽唯一标识
        Public Property ProteinID As String       ' 所属蛋白质ID
        Public Property Intensities As Double()  ' 各样本中该多肽的强度值（数组索引=样本ID）
    End Class
End Namespace