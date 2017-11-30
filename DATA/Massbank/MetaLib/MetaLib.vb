Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MetaLib

    ''' <summary>
    ''' 对``chebi/kegg/pubchem/HMDB/metlin``的物质注释信息整合库，这个数据库只要为了生成编号，名称之类的注释信息而构建的
    ''' </summary>
    Public Class MetaLib : Implements INamedValue

        ''' <summary>
        ''' 该物质在整合库之中的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property ID As String Implements IKeyedEntity(Of String).Key
        Public Property xref As xref
        Public Property name As String
        Public Property formula As String
        Public Property mass As Double

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    Public Class xref

        ''' <summary>
        ''' chebi主编号
        ''' </summary>
        ''' <returns></returns>
        Public Property chebi As String
        Public Property KEGG As String
        Public Property pubchem As String
        Public Property HMDB As String
        Public Property metlin As String
        Public Property CAS As String
        Public Property InChIkey As String
        Public Property InChI As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace