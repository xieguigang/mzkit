Imports CompMs.Common.Components
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports CompMs.Common.Interfaces
Imports System.Collections.Generic
Imports System.Linq

Namespace CompMs.Common.Lipidomics
    Public Class LipidSpectrumGeneratorFactory
        Public Function Create(ByVal lipidClass As LbmClass, ByVal adduct As AdductIon, ByVal rules As ISpectrumGenerationRule()) As ILipidSpectrumGenerator
            Return New RuleBaseSpectrumGenerator(lipidClass, adduct, rules)
        End Function
    End Class

    Friend Class RuleBaseSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Public Sub New(ByVal lipidClass As LbmClass, ByVal adduct As AdductIon, ByVal rules As ISpectrumGenerationRule())
            Me.LipidClass = lipidClass
            Me.Adduct = adduct
            Me.Rules = rules
        End Sub

        Public ReadOnly Property LipidClass As LbmClass
        Public ReadOnly Property Adduct As AdductIon
        Public ReadOnly Property Rules As ISpectrumGenerationRule()

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            Return lipid.LipidClass = LipidClass AndAlso Equals(adduct.AdductIonName, adduct.AdductIonName)
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            If Not CanGenerate(lipid, adduct) Then
                Return Nothing
            End If
            Dim spectrum = Rules.SelectMany(Function(rule) rule.Create(lipid, adduct)).GroupBy(Function(spec) spec, comparer).[Select](Function(specs) New SpectrumPeak(Enumerable.First(specs).Mass, Enumerable.First(specs).Intensity, String.Join(", ", specs.[Select](Function(spec) spec.Comment)))).OrderBy(Function(peak) peak.Mass).ToList()

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
    End Class

    Public Interface ISpectrumGenerationRule
        Function Create(ByVal lipid As ILipid, ByVal adduct As AdductIon) As SpectrumPeak()
    End Interface

    Public Class MzVariableRule
        Implements ISpectrumGenerationRule
        Public Sub New(ByVal mz As IMzVariable, ByVal intensity As Double, ByVal comment As String)
            Me.Mz = mz
            Me.Intensity = intensity
            Me.Comment = comment
        End Sub

        Public ReadOnly Property Mz As IMzVariable
        Public ReadOnly Property Intensity As Double
        Public ReadOnly Property Comment As String

        Public Function Create(ByVal lipid As ILipid, ByVal adduct As AdductIon) As SpectrumPeak() Implements ISpectrumGenerationRule.Create
            Return Mz.Evaluate(lipid, adduct).[Select](Function(mz) New SpectrumPeak(mz, Intensity, Comment)).ToArray()
        End Function
    End Class

    Public Interface IMzVariable
        Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double)
    End Interface

    Public Class EmptyMz
        Implements IMzVariable
        Public Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double) Implements IMzVariable.Evaluate
            Return Enumerable.Empty(Of Double)()
        End Function
    End Class

    Public Class ConstantMz
        Implements IMzVariable
        Public Sub New(ByVal exactMass As Double)
            Me.ExactMass = exactMass
        End Sub

        Public ReadOnly Property ExactMass As Double

        Public Iterator Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double) Implements IMzVariable.Evaluate
            Yield ExactMass
        End Function

        Public Overrides Function ToString() As String
            Return $"Const: {ExactMass}"
        End Function
    End Class

    Public Class PrecursorMz
        Implements IMzVariable
        Public Iterator Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double) Implements IMzVariable.Evaluate
            Yield adduct.ConvertToMz(lipid.Mass)
        End Function

        Public Overrides Function ToString() As String
            Return $"Precursor m/z"
        End Function
    End Class

    Public Class MolecularLevelChains
        Implements IMzVariable
        Public Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double) Implements IMzVariable.Evaluate
            Dim lipid_ As Lipid = Nothing, chains As SeparatedChains = Nothing

            If CSharpImpl.__Assign(lipid_, TryCast(lipid, Lipid)) IsNot Nothing AndAlso CSharpImpl.__Assign(chains, TryCast(lipid_.Chains, SeparatedChains)) IsNot Nothing Then
                Return lipid.Chains.GetDeterminedChains().[Select](Function(chain) chain.Mass)
            End If
            Return Enumerable.Empty(Of Double)()
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class

    Public Class PositionChainMz
        Implements IMzVariable
        Public Sub New(ByVal position As Integer)
            Me.Position = position
        End Sub

        Public ReadOnly Property Position As Integer

        Public Iterator Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double) Implements IMzVariable.Evaluate
            Dim lipid_ As Lipid = Nothing, chains As PositionLevelChains = Nothing

            If CSharpImpl.__Assign(lipid_, TryCast(lipid, Lipid)) IsNot Nothing AndAlso CSharpImpl.__Assign(chains, TryCast(lipid_.Chains, PositionLevelChains)) IsNot Nothing AndAlso chains.ChainCount >= Position Then
                Dim chain = lipid.Chains.GetChainByPosition(Position)
                Yield chain.Mass
            End If
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class

    Public Class ChainDesorptionMz
        Implements IMzVariable
        Public Sub New(ByVal position As Integer)
            Me.Position = position
        End Sub

        Public ReadOnly Property Position As Integer

        Public Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double) Implements IMzVariable.Evaluate
            Dim lipid_ As Lipid = Nothing, chains As SeparatedChains = Nothing

            If CSharpImpl.__Assign(lipid_, TryCast(lipid, Lipid)) IsNot Nothing AndAlso CSharpImpl.__Assign(chains, TryCast(lipid_.Chains, SeparatedChains)) IsNot Nothing AndAlso chains.ChainCount >= Position Then
                Return CreateSpectrum(lipid.Chains.GetChainByPosition(Position))
            End If
            Return Enumerable.Empty(Of Double)()
        End Function

        Private Function CreateSpectrum(ByVal chain As IChain) As IEnumerable(Of Double)
            If chain.CarbonCount = 0 OrElse chain.DoubleBond.UnDecidedCount <> 0 OrElse chain.Oxidized.UnDecidedCount <> 0 Then
                Return New Double(-1) {}
            End If
            Dim diffs = New Double(chain.CarbonCount - 1) {}
            For i = 0 To chain.CarbonCount - 1
                diffs(i) = HydrogenMass * 2 + CarbonMass
            Next
            For Each bond In chain.DoubleBond.Bonds
                diffs(bond.Position - 1) -= HydrogenMass
                diffs(bond.Position) -= HydrogenMass
            Next
            For Each ox In chain.Oxidized.Oxidises
                diffs(ox - 1) += OxygenMass
            Next
            Dim acylChain As AcylChain = Nothing

            If CSharpImpl.__Assign(acylChain, TryCast(chain, AcylChain)) IsNot Nothing Then
                diffs(0) += OxygenMass - HydrogenMass * 2
            End If
            For i = 1 To chain.CarbonCount - 1
                diffs(i) += diffs(i - 1)
            Next
            Return diffs.Take(chain.CarbonCount - 1).[Select](Function(diff) chain.Mass - diff)
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class

    Public Class LossMz
        Implements IMzVariable
        Public Sub New(ByVal left As IMzVariable, ByVal right As IMzVariable)
            Me.Left = left
            Me.Right = right
        End Sub

        Public ReadOnly Property Left As IMzVariable
        Public ReadOnly Property Right As IMzVariable

        Public Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double) Implements IMzVariable.Evaluate
            Return Left.Evaluate(lipid, adduct).SelectMany(Function(__) Right.Evaluate(lipid, adduct), Function(left, right) left - right)
        End Function
    End Class

    Public Class MzVariableProxy
        Implements IMzVariable
        Public Sub New(ByVal mz As IMzVariable)
            Me.Mz = mz
        End Sub

        Public ReadOnly Property Mz As IMzVariable

        Private cache As Double() = New Double(-1) {}

        Private lipid As ILipid
        Private adduct As AdductIon

        Public Function Evaluate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As IEnumerable(Of Double) Implements IMzVariable.Evaluate
            If lipid Is Me.lipid AndAlso adduct Is Me.adduct Then
                Return cache
            End If
            Me.lipid = lipid
            Me.adduct = adduct
            Return CSharpImpl.__Assign(cache, Mz.Evaluate(lipid, adduct).ToArray())
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
