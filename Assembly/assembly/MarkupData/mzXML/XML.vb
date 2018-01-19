Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.Math

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

        Public Shared Function ExportPeaktable(mzXML As String) As Peaktable()
            Dim ms1 As New List(Of scan)   ' peaktable
            Dim msms As New List(Of scan)  ' ms1 scan为msms scan的母离子
            Dim sample$ = mzXML.BaseName

            For Each scan As scan In LoadScans(mzXML)
                If scan.msLevel = "1" Then
                    ms1 += scan
                ElseIf scan.msLevel = "2" Then
                    msms += scan
                End If
            Next

            Dim ms1Peaktable As Dictionary(Of Integer, Peaktable) = ms1 _
                .Select(Function(s)
                            Return New Peaktable With {
                                .scan = s.num,
                                .rt = Val(s.retentionTime.Replace("PT", "")),
                                .sample = sample
                            }
                        End Function) _
                .ToDictionary(Function(peak) peak.scan)

        End Function
    End Class
End Namespace