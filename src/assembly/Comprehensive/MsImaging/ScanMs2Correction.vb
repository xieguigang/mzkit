
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache

Namespace MsImaging

    ''' <summary>
    ''' processing the MSI raw data contains with ms2 scan data
    ''' </summary>
    Public Class ScanMs2Correction : Inherits Correction

        Public Overrides Function GetPixelRowX(scanMs1 As ScanMS1) As Integer
            Throw New NotImplementedException()
        End Function

        Public Shared Function GetTotalScanNumbers(scan As ScanMS1) As Integer
            If scan.products.IsNullOrEmpty Then
                Return 1
            Else
                Return scan.products.Length + 1
            End If
        End Function

        Public Shared Function GetTotalScanNumbers(raw As mzPack) As Integer
            Return Aggregate scan As ScanMS1
                   In raw.MS
                   Let total As Integer = GetTotalScanNumbers(scan)
                   Into Sum(total)
        End Function
    End Class
End Namespace