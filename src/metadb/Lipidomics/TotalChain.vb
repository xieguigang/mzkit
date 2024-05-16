#Region "Microsoft.VisualBasic::487fd2d1882a6f3d0ab8dc0579ced4de, metadb\Lipidomics\TotalChain.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 109
    '    Code Lines: 89
    ' Comment Lines: 0
    '   Blank Lines: 20
    '     File Size: 5.35 KB


    ' Class TotalChain
    ' 
    '     Properties: AcylChainCount, AlkylChainCount, CarbonCount, ChainCount, Description
    '                 DoubleBondCount, Mass, OxidizedCount, SphingoChainCount
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Accept, CalculateSubLevelMass, Equals, EtherSymbol, GetCandidateSets
    '               GetChainByPosition, GetDeterminedChains, Includes, OxidizeSymbol, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass

Public Class TotalChain : Implements ITotalChain

    Public Sub New(carbonCount As Integer, doubleBondCount As Integer, oxidizedCount As Integer, acylChainCount As Integer, alkylChainCount As Integer, sphingoChainCount As Integer)
        Me.CarbonCount = carbonCount
        Me.DoubleBondCount = doubleBondCount
        Me.OxidizedCount = oxidizedCount
        Me.AcylChainCount = acylChainCount
        Me.AlkylChainCount = alkylChainCount
        Me.SphingoChainCount = sphingoChainCount
        Description = LipidDescription.Class
    End Sub

    Public ReadOnly Property CarbonCount As Integer Implements ITotalChain.CarbonCount
    Public ReadOnly Property DoubleBondCount As Integer Implements ITotalChain.DoubleBondCount
    Public ReadOnly Property OxidizedCount As Integer Implements ITotalChain.OxidizedCount
    Public ReadOnly Property ChainCount As Integer Implements ITotalChain.ChainCount
        Get
            Return AcylChainCount + AlkylChainCount + SphingoChainCount
        End Get
    End Property
    Public ReadOnly Property AcylChainCount As Integer Implements ITotalChain.AcylChainCount
    Public ReadOnly Property AlkylChainCount As Integer Implements ITotalChain.AlkylChainCount
    Public ReadOnly Property SphingoChainCount As Integer Implements ITotalChain.SphingoChainCount

    Public ReadOnly Property Description As LipidDescription Implements ITotalChain.Description

    Public ReadOnly Property Mass As Double Implements ITotalChain.Mass
        Get
            Return CalculateSubLevelMass(CarbonCount, DoubleBondCount, OxidizedCount, ChainCount, AcylChainCount, AlkylChainCount, SphingoChainCount)
        End Get
    End Property

    Private Shared Function CalculateSubLevelMass(carbon As Integer, doubleBond As Integer, oxidize As Integer, chain As Integer, acyl As Integer, alkyl As Integer, sphingo As Integer) As Double
        Dim carbonGain = carbon * CarbonMass
        Dim hydrogenGain = (2 * carbon - 2 * doubleBond + chain) * HydrogenMass
        Dim oxygenGain = oxidize * OxygenMass
        Dim acylGain = acyl * TotalChain.AcylGain
        Dim alkylGain = alkyl * TotalChain.AlkylGain
        Dim sphingoGain = sphingo * TotalChain.SphingoGain
        Dim result = carbonGain + hydrogenGain + oxygenGain + acylGain + alkylGain + sphingoGain
        Return result
    End Function

    Private Shared ReadOnly AcylGain As Double = OxygenMass - 2 * HydrogenMass

    Private Shared ReadOnly AlkylGain As Double = 0R

    Private Shared ReadOnly SphingoGain As Double = NitrogenMass + HydrogenMass

    Private Function GetChainByPosition(position As Integer) As IChain Implements ITotalChain.GetChainByPosition
        Return Nothing
    End Function

    Private Function GetDeterminedChains() As IChain() Implements ITotalChain.GetDeterminedChains
        Return Array.Empty(Of IChain)()
    End Function

    Private Function Includes(chains As ITotalChain) As Boolean Implements ITotalChain.Includes
        Return CarbonCount = chains.CarbonCount AndAlso DoubleBondCount = chains.DoubleBondCount AndAlso OxidizedCount = chains.OxidizedCount
    End Function

    Private Function GetCandidateSets(totalChainGenerator As ITotalChainVariationGenerator) As IEnumerable(Of ITotalChain) Implements ITotalChain.GetCandidateSets
        Return totalChainGenerator.Separate(Me)
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0}{1}:{2}{3}", EtherSymbol(AlkylChainCount), CarbonCount, DoubleBondCount, OxidizeSymbol(OxidizedCount))
    End Function

    Private Shared Function EtherSymbol(ether As Integer) As String
        Select Case ether
            Case 0
                Return ""
            Case 2
                Return "dO-"
            Case 4
                Return "eO-"
            Case Else
                Return "O-"
        End Select
    End Function

    Private Shared Function OxidizeSymbol(oxidize As Integer) As String
        If oxidize = 0 Then
            Return ""
        End If
        If oxidize = 1 Then
            Return ";O"
        End If
        Return $";{oxidize}O"
    End Function

    Public Overloads Function Equals(other As ITotalChain) As Boolean Implements IEquatable(Of ITotalChain).Equals
        Dim tChains As TotalChain = TryCast(other, TotalChain)
        Return tChains IsNot Nothing AndAlso ChainCount = other.ChainCount AndAlso CarbonCount = other.CarbonCount AndAlso DoubleBondCount = other.DoubleBondCount AndAlso OxidizedCount = other.OxidizedCount AndAlso Description = other.Description AndAlso AcylChainCount = tChains.AcylChainCount AndAlso AlkylChainCount = tChains.AlkylChainCount AndAlso SphingoChainCount = tChains.SphingoChainCount
    End Function

    Public Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult Implements IVisitableElement.Accept
        Dim decomposer_ As IDecomposer(Of TResult, TotalChain) = TryCast(decomposer, IDecomposer(Of TResult, TotalChain))

        If decomposer_ IsNot Nothing Then
            Return decomposer_.Decompose(visitor, Me)
        End If
        Return Nothing
    End Function

End Class
