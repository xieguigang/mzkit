Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel

Namespace LinearQuantitative

    ''' <summary>
    ''' a experiment project
    ''' </summary>
    Public Class Experiment : Inherits XmlDataModel

        <XmlElement> Public Property DataFiles As DataFile()

    End Class
End Namespace