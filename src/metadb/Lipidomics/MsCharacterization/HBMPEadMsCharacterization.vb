Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces


Public Module HBMPEadMsCharacterization
        Public Function Characterize(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

            Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 2, 1, 0.5)
            If reference.Name.Contains("/") Then
                defaultResult.IsPositionIonsExisted = True
            End If

            Return GetDefaultCharacterizationResultForTriacylGlycerols(molecule, defaultResult)
        End Function
    End Module

