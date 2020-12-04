Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML

Namespace MarkupData.imzML

    Public Class ScanData

        Public Property totalIon As Double
        Public Property x As Integer
        Public Property y As Integer

        Public Property MzPtr As ibdScan
        Public Property IntPtr As ibdScan

        Sub New(scan As spectrum)

        End Sub
    End Class

    Public Class ibdScan

        Public Property offset As Long
        Public Property arrayLength As Integer
        Public Property encodedLength As Integer

    End Class
End Namespace