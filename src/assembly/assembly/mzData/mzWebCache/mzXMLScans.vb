Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace mzData.mzWebCache

    Public Class mzXMLScans : Inherits ScanPopulator(Of scan)

        Protected Overrides Sub readScan(scan As scan)
            Throw New NotImplementedException()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function loadScans(rawfile As String) As IEnumerable(Of scan)
            Return mzXML.XML.LoadScans(rawfile)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function isMs1(scan As scan) As Boolean
            Return scan.msLevel = 1
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function isValid(scan As scan) As Boolean
            Return True
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function getScanTime(scan As scan) As Double
            Return PeakMs2.RtInSecond(scan.retentionTime)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function getScanId(scan As scan) As String
            Return scan.getName
        End Function
    End Class
End Namespace