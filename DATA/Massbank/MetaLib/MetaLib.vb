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

        ''' <summary>
        ''' 化合物分类
        ''' </summary>
        ''' <returns></returns>
        Public Property compound_class As String
        ''' <summary>
        ''' 仅限于人体内环境，这个化合物所存在的组织列表
        ''' </summary>
        ''' <returns></returns>
        Public Property tissue_locations As String()
        ''' <summary>
        ''' 仅限于人体内环境，这个化合物所存在的生物体液列表
        ''' </summary>
        ''' <returns></returns>
        Public Property biofluid_locations As String()

        ''' <summary>
        ''' 包含有这个物质的KEGG pathway的编号的集合，只有当<see cref="DATA.MetaLib.xref.KEGG"/>
        ''' 存在值的时候才会存在这个属性
        ''' </summary>
        ''' <returns></returns>
        Public Property pathways As String()

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