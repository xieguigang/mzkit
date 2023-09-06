Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces

Namespace CompMs.Common.Lipidomics
    Friend Class DMEDFAEadMsCharacterization
        Public Shared Function Characterize(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 1, 0, 0.5)
            Return GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule, defaultResult)
        End Function
    End Class
End Namespace
