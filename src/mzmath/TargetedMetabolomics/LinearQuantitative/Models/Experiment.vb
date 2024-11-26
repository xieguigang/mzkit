Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq

Namespace LinearQuantitative

    ''' <summary>
    ''' a experiment project
    ''' </summary>
    ''' 
    <XmlType("Experiment", [Namespace]:="https://mzkit.org")>
    Public Class Experiment : Inherits XmlDataModel

        <XmlElement("DataFile")> Public Property DataFiles As DataFile()

        <XmlAttribute>
        Public Property ProjectId As String

        Sub New()
        End Sub

        Sub New(files As IEnumerable(Of DataFile))
            DataFiles = files.SafeQuery.ToArray
        End Sub

    End Class
End Namespace