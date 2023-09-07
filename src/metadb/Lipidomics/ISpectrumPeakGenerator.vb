Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS


Public Interface ISpectrumPeakGenerator
    Function GetAcylDoubleBondSpectrum(lipid As ILipid, acylChain As AcylChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak)
    Function GetAlkylDoubleBondSpectrum(lipid As ILipid, acylChain As AlkylChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak)
    Function GetSphingoDoubleBondSpectrum(lipid As ILipid, acylChain As SphingoChain, adduct As AdductIon, nlMass As Double, abundance As Double) As IEnumerable(Of SpectrumPeak)
End Interface

