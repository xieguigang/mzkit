Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces


Public NotInheritable Class EidDefaultCharacterization
        Private Sub New()
        End Sub

        Public Shared Function Characterize4AlkylAcylGlycerols(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
            Return GetDefaultCharacterizationResultForAlkylAcylGlycerols(molecule, defaultResult)
        End Function

        Public Shared Function Characterize4Ceramides(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
            Return GetDefaultCharacterizationResultForCeramides(molecule, defaultResult)
        End Function

        Public Shared Function Characterize4DiacylGlycerols(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
            Return GetDefaultCharacterizationResultForGlycerophospholipid(molecule, defaultResult)
        End Function

        Public Shared Function Characterize4MonoacylGlycerols(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
            Return GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule, defaultResult)
        End Function

        Public Shared Function Characterize4SingleAcylChain(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
            Return GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule, defaultResult)
        End Function


        Public Shared Function Characterize4TriacylGlycerols(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
            Return GetDefaultCharacterizationResultForTriacylGlycerols(molecule, defaultResult)
        End Function
    End Class

