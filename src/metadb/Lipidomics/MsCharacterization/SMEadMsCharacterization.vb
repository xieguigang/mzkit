Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces

Public Module SMEadMsCharacterization
    Public Function Characterize(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 1, 0, 0.5)
        Return GetDefaultCharacterizationResultForCeramides(molecule, defaultResult)
    End Function
End Module
