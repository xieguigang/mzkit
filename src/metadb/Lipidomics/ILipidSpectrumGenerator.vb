Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Interfaces

Namespace CompMs.Common.Lipidomics
    Public Interface ILipidSpectrumGenerator
        Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean
        Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty
    End Interface
End Namespace
