Imports System.Collections.Generic
Imports System.Linq


Public Interface ILipidGenerator
        Function CanGenerate(ByVal lipid As ILipid) As Boolean
        Function Generate(ByVal lipid As Lipid) As IEnumerable(Of ILipid)
    End Interface

    Public Class LipidGenerator
        Implements ILipidGenerator
        Public Sub New(ByVal totalChainGenerator As ITotalChainVariationGenerator)
            Me.totalChainGenerator = totalChainGenerator
        End Sub

        Public Sub New()
            Me.New(New TotalChainVariationGenerator(minLength:=6, begin:=3, [end]:=3, skip:=3))

        End Sub

        Private ReadOnly totalChainGenerator As ITotalChainVariationGenerator

        Public Function CanGenerate(ByVal lipid As ILipid) As Boolean Implements ILipidGenerator.CanGenerate
            Return lipid.ChainCount >= 1
        End Function

        Public Function Generate(ByVal lipid As Lipid) As IEnumerable(Of ILipid) Implements ILipidGenerator.Generate
            Return lipid.Chains.GetCandidateSets(totalChainGenerator).[Select](Function(chains) New Lipid(lipid.LipidClass, lipid.Mass, chains))
        End Function
    End Class


