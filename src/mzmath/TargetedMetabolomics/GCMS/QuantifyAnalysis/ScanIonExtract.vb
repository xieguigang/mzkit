Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace GCMS.QuantifyAnalysis

    Public Class ScanIonExtract : Inherits QuantifyIonExtract

        Public Sub New(ions As IEnumerable(Of QuantifyIon), peakwidth As DoubleRange, centroid As Tolerance)
            MyBase.New(ions, peakwidth, centroid)
        End Sub

        Public Overrides Function GetSamplePeaks(sample As Raw) As IEnumerable(Of TargetPeakPoint)
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace