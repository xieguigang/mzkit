Namespace MetaLib

    ''' <summary>
    ''' 主要是取自HMDB数据库之中的代谢物分类信息
    ''' </summary>
    Public Interface ICompoundClass

        Property kingdom As String
        Property super_class As String
        Property [class] As String
        Property sub_class As String
        Property molecular_framework As String

    End Interface
End Namespace