Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public Class imzMLScan : Inherits MsDataReader(Of ScanReader)

        Public Overrides Function GetScanTime(scan As ScanReader) As Double
            Return 0
        End Function

        Public Overrides Function GetScanId(scan As ScanReader) As String
            Return $"[{scan.x},{scan.y}]"
        End Function

        Public Overrides Function IsEmpty(scan As ScanReader) As Boolean
            Return scan.MzPtr Is Nothing OrElse scan.IntPtr Is Nothing
        End Function

        Public Overrides Function GetMsMs(scan As ScanReader) As ms2()
            Return scan.LoadMsData
        End Function

        Public Overrides Function GetMsLevel(scan As ScanReader) As Integer
            Return 1
        End Function

        Public Overrides Function GetBPC(scan As ScanReader) As Double
            Return 0
        End Function

        Public Overrides Function GetTIC(scan As ScanReader) As Double
            Return scan.totalIon
        End Function

        Public Overrides Function GetParentMz(scan As ScanReader) As Double
            Return 0
        End Function

        Public Overrides Function GetPolarity(scan As ScanReader) As String
            Return "+"
        End Function
    End Class
End Namespace