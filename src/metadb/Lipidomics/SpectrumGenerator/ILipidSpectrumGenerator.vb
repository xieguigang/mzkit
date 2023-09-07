Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Interface ILipidSpectrumGenerator
        Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean
        Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty
    End Interface

