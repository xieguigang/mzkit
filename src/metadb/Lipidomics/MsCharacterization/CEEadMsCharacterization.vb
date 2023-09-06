Imports CompMs.Common.Components
Imports CompMs.Common.Interfaces

Friend Class CEEadMsCharacterization
    Public Shared Function Characterize(ByVal scan As IMSScanProperty, ByVal molecule As ILipid, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 1, 0, 2) ' doublebond position cannot be determined
        Return GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule, defaultResult)
    End Function
End Class
