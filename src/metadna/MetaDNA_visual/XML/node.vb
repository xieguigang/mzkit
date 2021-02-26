
Imports System.Xml.Serialization

Public Class node

    <XmlAttribute> Public Property kegg As String
    <XmlAttribute> Public Property ms1 As String

    ''' <summary>
    ''' The ms2 index
    ''' </summary>
    ''' <returns></returns>
    <XmlText>
    Public Property ms2 As String

    Public Overrides Function ToString() As String
        Return $"Dim {kegg} = ""{ms1}|{ms2}"""
    End Function

End Class