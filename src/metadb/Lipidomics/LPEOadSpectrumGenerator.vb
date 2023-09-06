Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public Class LPEOadSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Private Shared ReadOnly C2H8NO4P As Double = {CarbonMass * 2, HydrogenMass * 8, NitrogenMass, OxygenMass * 4, PhosphorusMass}.Sum()

        Private Shared ReadOnly C3H5O2 As Double = {CarbonMass * 3, HydrogenMass * 5, OxygenMass * 2}.Sum()

        Private Shared ReadOnly H2O As Double = {HydrogenMass * 2, OxygenMass}.Sum()

        Private Shared ReadOnly C2H2O As Double = {HydrogenMass * 2, CarbonMass * 2, OxygenMass}.Sum()
        Private Shared ReadOnly Electron As Double = 0.00054858026

        Private ReadOnly spectrumGenerator As IOadSpectrumPeakGenerator
        Public Sub New()
            spectrumGenerator = New OadSpectrumPeakGenerator()
        End Sub

        Public Sub New(ByVal spectrumGenerator As IOadSpectrumPeakGenerator)
            Me.spectrumGenerator = If(spectrumGenerator, CSharpImpl.__Throw(Of IOadSpectrumPeakGenerator)(New ArgumentNullException(NameOf(spectrumGenerator))))
        End Sub

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            If Equals(adduct.AdductIonName, "[M+H]+") Then
                Return True
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim abundance = 30
            Dim nlMass = 0.0
            Dim spectrum = New List(Of SpectrumPeak)()
            spectrum.AddRange(GetLPEOadSpectrum(lipid, adduct))
            '"OAD02+O",
            '"OAD05",
            '"OAD07",
            '"OAD09",
            '"OAD10",
            '"OAD11",
            '"OAD12",
            '"OAD15+O",
            '"OAD12+O+H",
            '"OAD12+O+2H",
            '"OAD01+H"
            Dim oadId = New String() {"OAD01", "OAD02", "OAD03", "OAD04", "OAD06", "OAD08", "OAD13", "OAD14", "OAD15", "OAD16", "OAD17", "OAD12+O"}

            Dim plChains As PositionLevelChains = Nothing

            If CSharpImpl.__Assign(plChains, TryCast(lipid.Chains, PositionLevelChains)) IsNot Nothing Then
                For Each chain As AcylChain In lipid.Chains.GetDeterminedChains()
                    spectrum.AddRange(spectrumGenerator.GetAcylDoubleBondSpectrum(lipid, chain, adduct, nlMass, abundance, oadId))
                Next
            End If
            spectrum = spectrum.GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).Mass, specs.Sum(Function(n) n.Intensity), String.Join(", ", specs.[Select](Function(spec) spec.Comment)), specs.Aggregate(SpectrumComment.none, Function(a, b) a Or b.SpectrumComment))).OrderBy(Function(peak) peak.Mass).ToList()
            Return CreateReference(lipid, adduct, spectrum, molecule)
        End Function

        Private Function GetLPEOadSpectrum(ByVal lipid As Lipid, ByVal adduct As AdductIon) As SpectrumPeak()
            Dim spectrum = New List(Of SpectrumPeak)()
            Dim Chains As SeparatedChains = Nothing

            If Equals(adduct.AdductIonName, "[M+H]+") Then
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - H2O), 100R, "Precursor -H2O") With {
.SpectrumComment = SpectrumComment.metaboliteclass
}, New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - C2H8NO4P), 100R, "Precursor -C2H8NO4P") With {
.SpectrumComment = SpectrumComment.metaboliteclass,
.IsAbsolutelyRequiredFragmentForAnnotation = True
}})

                If CSharpImpl.__Assign(Chains, TryCast(lipid.Chains, SeparatedChains)) IsNot Nothing Then
                    For Each chain As AcylChain In lipid.Chains.GetDeterminedChains()
                        If chain.CarbonCount <> 0 Then
                            spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass - chain.Mass + HydrogenMass), 30R, $"-{chain}") With {
.SpectrumComment = SpectrumComment.acylchain
}, New SpectrumPeak(adduct.ConvertToMz(chain.Mass + C2H2O), 30R, $"-{chain} +C2H2O") With {
.SpectrumComment = SpectrumComment.acylchain
}})
                        End If
                    Next
                End If
            Else
                spectrum.AddRange({New SpectrumPeak(adduct.ConvertToMz(lipid.Mass), 999R, "Precursor") With {
.SpectrumComment = SpectrumComment.precursor
}})
            End If
            Return spectrum.ToArray()
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
End Namespace
