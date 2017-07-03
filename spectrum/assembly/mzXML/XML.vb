Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace mzXML

    ''' <summary>
    ''' ``*.mzXML`` raw data
    ''' </summary>
    ''' 
    <XmlType("mzXML", [Namespace]:="http://sashimi.sourceforge.net/schema_revision/mzXML_3.2")> Public Class XML

        Public Property msRun As MSData
        Public Property index As index
        Public Property indexOffset As Long
        Public Property shal As String

        Public Shared Function LoadScans(mzXML$) As IEnumerable(Of scan)
            Return mzXML.LoadXmlDataSet(Of scan)(, xmlns:="http://sashimi.sourceforge.net/schema_revision/mzXML_3.2")
        End Function
    End Class
End Namespace