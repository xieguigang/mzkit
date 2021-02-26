Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class compound : Implements INamedValue

    <XmlAttribute>
    Public Property kegg As String Implements INamedValue.Key
    <XmlAttribute("candidates")>
    Public Property size As Integer

    <XmlElement("unknown")>
    Public Property candidates As unknown()

    Public Overrides Function ToString() As String
        Return $"{kegg} have {candidates.Length} candidates: {candidates.Select(Function(c) c.name).GetJson}"
    End Function

End Class
