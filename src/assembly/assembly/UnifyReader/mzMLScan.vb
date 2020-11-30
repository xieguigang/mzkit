Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML

Namespace DataReader

    Public Class mzMLScan : Inherits MsDataReader(Of spectrum)

        Public Overrides Function GetScanTime(scan As spectrum) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetScanId(scan As spectrum) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function IsEmpty(scan As spectrum) As Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetMsMs(scan As spectrum) As Math.Spectra.ms2()
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetMsLevel(scan As spectrum) As Integer
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetBPC(scan As spectrum) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetTIC(scan As spectrum) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetParentMz(scan As spectrum) As Double
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace