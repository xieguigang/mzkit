Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class OadDefaultSpectrumGenerator
    Implements ILipidSpectrumGenerator
    Private ReadOnly spectrumGenerator As IOadSpectrumPeakGenerator
    Public Sub New()
        spectrumGenerator = New OadSpectrumPeakGenerator()
    End Sub

    Public Sub New(spectrumGenerator As IOadSpectrumPeakGenerator)
        Me.spectrumGenerator = spectrumGenerator
    End Sub

    Public Function CanGenerate(lipid As ILipid, adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
        If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") OrElse Equals(adduct.AdductIonName, "[M+H-H2O]+") OrElse Equals(adduct.AdductIonName, "[M-H2O+H]+") OrElse Equals(adduct.AdductIonName, "[M-H]-") OrElse Equals(adduct.AdductIonName, "[M+HCOO]-") OrElse Equals(adduct.AdductIonName, "[M+CH3COO]-") Then
            Return True
        End If
        Return False
    End Function

    Public Function Generate(lipid As Lipid, adduct As AdductIon, Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
        Dim oadLipidSpectrumGenerator = New OadLipidSpectrumGenerator()
        Dim abundance = 40.0

        Dim oadClassFragment = oadLipidSpectrumGenerator.GetClassFragmentSpectrum(lipid, adduct)
        Dim spectrum = New List(Of SpectrumPeak)(oadClassFragment.spectrum)
        Dim nlMass = oadClassFragment.nlMass
        Dim oadId = New String() {"OAD01", "OAD02", "OAD02+O", "OAD03", "OAD04", "OAD05", "OAD06", "OAD07", "OAD08", "OAD09", "OAD10", "OAD11", "OAD12", "OAD13", "OAD14", "OAD15", "OAD15+O", "OAD16", "OAD17", "OAD12+O", "OAD12+O+H", "OAD12+O+2H", "OAD01+H"}


        Dim plChains As PositionLevelChains = TryCast(lipid.Chains, PositionLevelChains)
        Dim acyl As AcylChain = Nothing
        Dim alkyl As AlkylChain = Nothing
        Dim sphingo As SphingoChain = Nothing

        If plChains IsNot Nothing Then
            For Each chain In lipid.Chains.GetDeterminedChains()
                acyl = TryCast(chain, AcylChain)
                alkyl = TryCast(chain, AlkylChain)
                If acyl IsNot Nothing Then
                    spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlMass, abundance, oadId))
                ElseIf alkyl IsNot Nothing Then
                    spectrum.AddRange(spectrumGenerator.GetAlkylDoubleBondSpectrum(lipid, alkyl, adduct, nlMass, abundance, oadId))
                End If
                sphingo = TryCast(chain, SphingoChain)
                If sphingo IsNot Nothing Then
                    spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlMass, abundance, oadId))
                End If
            Next
        End If
        spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).mz, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Annotation)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.Mass).ToList()
        Return CreateReference(lipid, adduct, spectrum, molecule)
    End Function

    Private Function CreateReference(lipid As ILipid, adduct As AdductIon, spectrum As List(Of SpectrumPeak), molecule As IMoleculeProperty) As MoleculeMsReference
        Return New MoleculeMsReference With {
    .PrecursorMz = adduct.ConvertToMz(lipid.Mass),
    .IonMode = adduct.IonMode,
    .Spectrum = spectrum,
    .Name = lipid.Name,
    .Formula = molecule?.Formula,
    .Ontology = molecule?.Ontology,
    .SMILES = molecule?.SMILES,
    .InChIKey = molecule?.InChIKey,
    .AdductType = adduct,
    .CompoundClass = lipid.LipidClass.ToString(),
    .Charge = adduct.ChargeNumber
}
    End Function

    Private Shared ReadOnly comparer As IEqualityComparer(Of SpectrumPeak) = New SpectrumEqualityComparer()

    Private ReadOnly map As Dictionary(Of LbmClass, List(Of ILipidSpectrumGenerator)) = New Dictionary(Of LbmClass, List(Of ILipidSpectrumGenerator))()
    Public Sub Add(lipidClass As LbmClass, generator As ILipidSpectrumGenerator)
        If Not map.ContainsKey(lipidClass) Then
            map.Add(lipidClass, New List(Of ILipidSpectrumGenerator)())
        End If
        map(lipidClass).Add(generator)
    End Sub

End Class

Public Class OadClassFragment
    Public Property nlMass As Double
    Public Property spectrum As List(Of SpectrumPeak)
End Class

