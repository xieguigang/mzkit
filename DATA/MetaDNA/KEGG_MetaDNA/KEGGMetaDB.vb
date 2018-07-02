''' <summary>
''' MetaDNA算法所需要的KEGG注释信息的结构
''' </summary>
Public Class KEGGMetaDB

    Public Property keggID As String
    Public Property name As String
    Public Property formula As String
    Public Property exact_mass As Double

    ''' <summary>
    ''' 与其他的数据库的编号的外键连接
    ''' </summary>
    ''' <returns></returns>
    Public Property xref As Dictionary(Of String, String)

    ''' <summary>
    ''' 指向来源的数据集的唯一编号
    ''' </summary>
    ''' <returns></returns>
    Public Property libname As String
    Public Property [class] As String

    Public Overrides Function ToString() As String
        Return $"[{libname}] {keggID}={name}"
    End Function

End Class
