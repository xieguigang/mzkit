Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Interfaces


Public Interface ILipidSpectrumGenerator
        Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean
        Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty
    End Interface

