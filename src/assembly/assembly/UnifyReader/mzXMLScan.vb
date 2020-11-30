Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public Class mzXMLScan : Inherits MsDataReader(Of scan)

        Public Overrides Function GetScanTime(scan As scan) As Double
            Return PeakMs2.RtInSecond(scan.retentionTime)
        End Function

        Public Overrides Function GetScanId(scan As scan) As Double
            Return scan.getName
        End Function

        Public Overrides Function IsEmpty(scan As scan) As Boolean
            Return scan.peaks Is Nothing OrElse
                scan.peaks.compressedLen = 0 OrElse
                scan.peaks.value.StringEmpty
        End Function

        Public Overrides Function GetMsMs(scan As scan) As ms2()
            Return scan.peaks _
                .ExtractMzI _
                .Where(Function(p) p.intensity > 0) _
                .Select(Function(p)
                            Return New ms2 With {
                                .mz = p.mz,
                                .quantity = p.intensity,
                                .intensity = p.intensity
                            }
                        End Function) _
                .ToArray
        End Function

        Public Overrides Function GetMsLevel(scan As scan) As Integer
            Return scan.msLevel
        End Function

        Public Overrides Function GetBPC(scan As scan) As Double
            Return scan.basePeakIntensity
        End Function

        Public Overrides Function GetTIC(scan As scan) As Double
            Return scan.totIonCurrent
        End Function

        Public Overrides Function GetParentMz(scan As scan) As Double
            Return scan.precursorMz.value
        End Function

        Public Overrides Function GetPolarity(scan As scan) As String
            Return scan.polarity
        End Function
    End Class
End Namespace