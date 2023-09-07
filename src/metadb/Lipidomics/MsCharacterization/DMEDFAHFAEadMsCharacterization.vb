Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine

Friend Class DMEDFAHFAEadMsCharacterization
    Public Shared Function Characterize(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())
        Dim snPositionMzValues = reference.Spectrum.Where(Function(n) n.SpectrumComment = SpectrumComment.snposition).ToList()
        Dim dions4position As List(Of DiagnosticIon) = Nothing
        If Not snPositionMzValues.IsNullOrEmpty Then
            dions4position = New List(Of DiagnosticIon)() From {
                    New DiagnosticIon() With {
                    .MzTolerance = 0.05,
                    .IonAbundanceCutOff = 10,
                    .Mz = snPositionMzValues(0).mz
                }
                }
        Else
            Console.WriteLine()
        End If

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 1, 1, 0.5, dIons4position:=dions4position)
        Return GetDefaultCharacterizationResultForGlycerophospholipid(molecule, defaultResult)
    End Function
End Class
