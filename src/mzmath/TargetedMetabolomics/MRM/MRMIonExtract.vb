Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace MRM

    Public Class MRMIonExtract : Inherits FeatureExtract(Of indexedmzML)

        ReadOnly ionpairs As IsomerismIonPairs()
        ReadOnly ms1ppm As Tolerance

        Public Sub New(ionpairs As IEnumerable(Of IonPair), ms1ppm As Tolerance, peakwidth As DoubleRange)
            MyBase.New(peakwidth)

            Me.ms1ppm = ms1ppm
            Me.ionpairs = IonPair _
                .GetIsomerism(ionpairs.ToArray, ms1ppm) _
                .ToArray
        End Sub

        Public Overrides Iterator Function GetSamplePeaks(sample As indexedmzML) As IEnumerable(Of TargetPeakPoint)
            Dim sampleName As String = DirectCast(sample, IFileReference).FilePath.BaseName

            For Each ionData In sample.mzML.run.chromatogramList.list.MRMSelector(ionpairs, ms1ppm)
                Yield New TargetPeakPoint With {
                    .Name = ionData.ion.target.accession,
                    .SampleName = sampleName
                }
            Next
        End Function
    End Class
End Namespace