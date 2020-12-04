Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML

Namespace MarkupData.imzML

    <XmlType("indexedmzML", [Namespace]:=mzML.Xml.xmlns)>
    Public Class XML

        <XmlAttribute>
        Public Property version As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadScans(file As String) As IEnumerable(Of ScanData)
            Return mzML.Xml.LoadScans(file).Select(Function(scan) New ScanData(scan))
        End Function
    End Class
End Namespace