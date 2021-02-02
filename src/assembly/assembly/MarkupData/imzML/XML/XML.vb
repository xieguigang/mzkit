Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace MarkupData.imzML

    <XmlType("indexedmzML", [Namespace]:=mzML.indexedmzML.xmlns)>
    Public Class XML

        <XmlAttribute>
        Public Property version As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadScans(file As String) As IEnumerable(Of ScanData)
            Return mzML.indexedmzML.LoadScans(file).Select(Function(scan) New ScanData(scan))
        End Function

        Public Shared Iterator Function LoadScanData(imzML As String) As IEnumerable(Of ScanReader)
            Dim ibd As ibdReader = ibdReader.Open(imzML.ChangeSuffix("ibd"))

            For Each scan As ScanData In LoadScans(file:=imzML)
                Yield New ScanReader(scan, ibd)
            Next
        End Function
    End Class

    Public Class ScanReader : Inherits ScanData

        ReadOnly ibd As ibdReader

        Sub New(scan As ScanData, ibd As ibdReader)
            Call MyBase.New(scan)

            Me.ibd = ibd
        End Sub

        Public Function LoadMsData() As ms2()
            Return ibd.GetMSMS(Me)
        End Function

        Public Overrides Function ToString() As String
            Return $"{ibd.UUID} {MyBase.ToString}"
        End Function
    End Class
End Namespace