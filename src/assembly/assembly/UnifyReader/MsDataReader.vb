Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public MustInherit Class MsDataReader(Of Scan)

        Public MustOverride Function GetScanTime(scan As Scan) As Double
        Public MustOverride Function GetScanId(scan As Scan) As Double
        Public MustOverride Function IsEmpty(scan As Scan) As Boolean
        Public MustOverride Function GetMsMs(scan As Scan) As ms2()
        Public MustOverride Function GetMsLevel(scan As Scan) As Integer
        Public MustOverride Function GetBPC(scan As Scan) As Double
        Public MustOverride Function GetTIC(scan As Scan) As Double
        Public MustOverride Function GetParentMz(scan As Scan) As Double
        Public MustOverride Function GetPolarity(scan As Scan) As String

    End Class
End Namespace