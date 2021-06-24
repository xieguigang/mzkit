Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace MarkupData.imzML

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