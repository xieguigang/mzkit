Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces


Public Module LPCEadMsCharacterization
        Public Function Characterize(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 2, 1, 1, 0.5)
            Return GetDefaultCharacterizationResultForGlycerophospholipid(molecule, defaultResult)
        End Function
    End Module

