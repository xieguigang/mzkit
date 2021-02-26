Imports System.Xml.Serialization

Public Class unknown

    <XmlAttribute> Public Property name As String
    <XmlAttribute> Public Property Msn As String
    <XmlAttribute> Public Property length As Integer
    <XmlAttribute> Public Property intensity As Double
    <XmlAttribute> Public Property scores As Double()

    ''' <summary>
    ''' 请注意，第一个推断节点肯定是metaDNA的最初的通过标准品库所鉴定出来的seed数据
    ''' 所以这个seed的ms1名称肯定是在unknown之中找不到的
    ''' </summary>
    ''' <returns></returns>
    <XmlElement("node")>
    Public Property edges As node()

    Public Overrides Function ToString() As String
        Return $"{name}: {edges.Select(Function(n) n.kegg).JoinBy(" -> ")}"
    End Function
End Class
