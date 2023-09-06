Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public Class OadDefaultSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Private ReadOnly spectrumGenerator As IOadSpectrumPeakGenerator
        Public Sub New()
            spectrumGenerator = New OadSpectrumPeakGenerator()
        End Sub

        Public Sub New(ByVal spectrumGenerator As IOadSpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of IOadSpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If Equals(adduct.AdductIonName, "[M+H]+") OrElse Equals(adduct.AdductIonName, "[M+Na]+") OrElse Equals(adduct.AdductIonName, "[M+NH4]+") OrElse Equals(adduct.AdductIonName, "[M+H-H2O]+") OrElse Equals(adduct.AdductIonName, "[M-H2O+H]+") OrElse Equals(adduct.AdductIonName, "[M-H]-") OrElse Equals(adduct.AdductIonName, "[M+HCOO]-") OrElse Equals(adduct.AdductIonName, "[M+CH3COO]-") Then
                Return True
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim oadLipidSpectrumGenerator = New OadLipidSpectrumGenerator()
            Dim abundance = 40.0

            Dim oadClassFragment = oadLipidSpectrumGenerator.GetClassFragmentSpectrum(lipid, adduct)
            Dim spectrum = New List(Of SpectrumPeak)(oadClassFragment.spectrum)
            Dim nlMass = oadClassFragment.nlMass
            Dim oadId = New String() {"OAD01", "OAD02", "OAD02+O", "OAD03", "OAD04", "OAD05", "OAD06", "OAD07", "OAD08", "OAD09", "OAD10", "OAD11", "OAD12", "OAD13", "OAD14", "OAD15", "OAD15+O", "OAD16", "OAD17", "OAD12+O", "OAD12+O+H", "OAD12+O+2H", "OAD01+H"}


            Dim plChains As PositionLevelChains = Nothing, acyl As AcylChain = Nothing, alkyl As AlkylChain = Nothing, sphingo As SphingoChain = Nothing

            If CSharpImpl.__Assign(plChains, TryCast(lipid.Chains, PositionLevelChains)) IsNot Nothing Then
                For Each chain In lipid.Chains.GetDeterminedChains()

                    If CSharpImpl.__Assign(acyl, TryCast(chain, AcylChain)) IsNot Nothing Then
                        spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, acyl, adduct, nlMass, abundance, oadId))
                    ElseIf CSharpImpl.__Assign(alkyl, TryCast(chain, AlkylChain)) IsNot Nothing Then
                        spectrum.AddRange(spectrumGenerator.GetAlkylDoubleBondSpectrum(lipid, alkyl, adduct, nlMass, abundance, oadId))
                    End If

                    If CSharpImpl.__Assign(sphingo, TryCast(chain, SphingoChain)) IsNot Nothing Then
                        spectrum.AddRange(spectrumGenerator.GetSphingoDoubleBondSpectrum(lipid, sphingo, adduct, nlMass, abundance, oadId))
                    End If
                Next
            End If
            spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).Mass, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Comment)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.Mass).ToList()
            Return CreateReference(lipid, adduct, spectrum, molecule)
        End Function

        Private Function CreateReference(ByVal lipid As ILipid, ByVal adduct As AdductIon, ByVal spectrum As List(Of SpectrumPeak), ByVal molecule As IMoleculeProperty) As MoleculeMsReference
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
        Public Sub Add(ByVal lipidClass As LbmClass, ByVal generator As ILipidSpectrumGenerator)
            If Not map.ContainsKey(lipidClass) Then
                map.Add(lipidClass, New List(Of ILipidSpectrumGenerator)())
            End If
            map(lipidClass).Add(generator)
        End Sub

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function <Obsolete("Please refactor calling code to use normal throw statements")>
            Shared Function __Throw(Of T)(ByVal e As Exception) As T
                Throw e
            End Function
        End Class

    End Class
    Public Class OadClassFragment
        Public Property nlMass As Double
        Public Property spectrum As List(Of SpectrumPeak)
    End Class

End Namespace
