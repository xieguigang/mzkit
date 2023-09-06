Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces

Namespace CompMs.Common.Lipidomics
    Public NotInheritable Class EidDefaultCharacterization
        Private Sub New()
        End Sub

        Public Shared Function Characterize4AlkylAcylGlycerols(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
            Return GetDefaultCharacterizationResultForAlkylAcylGlycerols(molecule, defaultResult)
        End Function

        Public Shared Function Characterize4Ceramides(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
            Return GetDefaultCharacterizationResultForCeramides(molecule, defaultResult)
        End Function

        Public Shared Function Characterize4DiacylGlycerols(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
            Return GetDefaultCharacterizationResultForGlycerophospholipid(molecule, defaultResult)
        End Function

        Public Shared Function Characterize4MonoacylGlycerols(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
            Return GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule, defaultResult)
        End Function

        Public Shared Function Characterize4SingleAcylChain(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
            Return GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule, defaultResult)
        End Function


        Public Shared Function Characterize4TriacylGlycerols(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
            Return GetDefaultCharacterizationResultForTriacylGlycerols(molecule, defaultResult)
        End Function
    End Class
End Namespace
