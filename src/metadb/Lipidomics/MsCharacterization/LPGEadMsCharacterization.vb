Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces


Public Module LPGEadMsCharacterization
        Public Function Characterize(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())
            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 3, 1, 1, 0.5)
            Return GetDefaultCharacterizationResultForGlycerophospholipid(molecule, defaultResult)
        End Function
    End Module

