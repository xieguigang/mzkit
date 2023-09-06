Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces

Namespace CompMs.Common.Lipidomics
    Public Module SHexCerEadMsCharacterization
        Public Function Characterize(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())
            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 2, 2, 1, 0.5)
            Return GetDefaultCharacterizationResultForCeramides(molecule, defaultResult)
        End Function
    End Module
End Namespace
