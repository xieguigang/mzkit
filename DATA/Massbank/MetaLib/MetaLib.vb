Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

''' <summary>
''' 对chebi/kegg/pubchem/HMDB/metlin的物质注释信息整合库
''' </summary>
Public Class MetaLib : Implements INamedValue

    ''' <summary>
    ''' 该物质在整合库之中的唯一标识符
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String Implements IKeyedEntity(Of String).Key

    ''' <summary>
    ''' chebi主编号
    ''' </summary>
    ''' <returns></returns>
    Public Property chebi As String


End Class

Public Class xref

End Class