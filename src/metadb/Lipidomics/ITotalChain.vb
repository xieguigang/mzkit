#Region "Microsoft.VisualBasic::173b28b10d4c00117909efae37947f91, metadb\Lipidomics\ITotalChain.vb"

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

    '   Total Lines: 106
    '    Code Lines: 84 (79.25%)
    ' Comment Lines: 13 (12.26%)
    '    - Xml Docs: 92.31%
    ' 
    '   Blank Lines: 9 (8.49%)
    '     File Size: 5.04 KB


    ' Interface ITotalChain
    ' 
    '     Properties: AcylChainCount, AlkylChainCount, CarbonCount, ChainCount, Description
    '                 DoubleBondCount, Mass, OxidizedCount, SphingoChainCount
    ' 
    '     Function: GetCandidateSets, GetChainByPosition, GetDeterminedChains, Includes
    ' 
    ' Module TotalChainExtension
    ' 
    '     Function: Deconstruct, GetChains, GetEachChains, GetTypedChains
    ' 
    '     Sub: (+2 Overloads) ApplyToChain
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Public Interface ITotalChain
    Inherits IEquatable(Of ITotalChain), IVisitableElement
    ReadOnly Property CarbonCount As Integer
    ReadOnly Property DoubleBondCount As Integer
    ReadOnly Property OxidizedCount As Integer
    ReadOnly Property ChainCount As Integer
    ReadOnly Property AcylChainCount As Integer
    ReadOnly Property AlkylChainCount As Integer
    ReadOnly Property SphingoChainCount As Integer
    ReadOnly Property Mass As Double
    ReadOnly Property Description As LipidDescription

    ''' <summary>
    ''' Retrieve the determined chain by position.
    ''' The position here refers to a specific order defined for each lipid class.
    ''' It may not necessarily match the commonly used sn-position for lipids.
    ''' </summary>
    ''' <param name="position">1-indexed position</param>
    ''' <returns>IChain if the specified position chain is determined; otherwise, null.</returns>
    Function GetChainByPosition(position As Integer) As IChain
    ''' <summary>
    ''' This method returns an array of lipid chains with confirmed structures.
    ''' It only includes the chains that have their structures determined, and there is no guarantee that the position and index will match.
    ''' </summary>
    ''' <returns>IChain[]</returns>
    Function GetDeterminedChains() As IChain()
    Function Includes(chains As ITotalChain) As Boolean
    Function GetCandidateSets(totalChainGenerator As ITotalChainVariationGenerator) As IEnumerable(Of ITotalChain)
End Interface

Public Module TotalChainExtension
    <Extension()>
    Public Function GetChains(lipid As LipidMolecule) As ITotalChain
        Dim prop = LipidClassDictionary.Default.LbmItems(lipid.LipidClass)
        Select Case lipid.AnnotationLevel
            Case 1
                Return New TotalChain(lipid.TotalCarbonCount, lipid.TotalDoubleBondCount, lipid.TotalOxidizedCount, acylChainCount:=prop.AcylChain, alkylChainCount:=prop.AlkylChain, sphingoChainCount:=prop.SphingoChain)
            Case 2, 3
                Return New MolecularSpeciesLevelChains(GetEachChains(lipid, prop))
            Case Else
        End Select
        Return Nothing
    End Function

    Private Function GetEachChains(lipid As LipidMolecule, prop As LipidClassProperty) As IChain()
        Dim chains = New IChain(prop.TotalChain - 1) {}
        If prop.TotalChain >= 1 Then
            If prop.SphingoChain >= 1 Then
                ' chains[0] = SphingoParser.Parse(lipid.Sn1AcylChainString);
                chains(0) = New SphingoChain(lipid.Sn1CarbonCount, New DoubleBond(lipid.Sn1DoubleBondCount), New Oxidized(lipid.Sn1Oxidizedount))
            ElseIf prop.AlkylChain >= 1 Then
                chains(0) = New AlkylChain(lipid.Sn1CarbonCount, New DoubleBond(lipid.Sn1DoubleBondCount), New Oxidized(lipid.Sn1Oxidizedount))
            Else
                chains(0) = New AcylChain(lipid.Sn1CarbonCount, New DoubleBond(lipid.Sn1DoubleBondCount), New Oxidized(lipid.Sn1Oxidizedount))
            End If
        End If
        If prop.TotalChain >= 2 Then
            chains(1) = New AcylChain(lipid.Sn2CarbonCount, New DoubleBond(lipid.Sn2DoubleBondCount), New Oxidized(lipid.Sn2Oxidizedount))
        End If
        If prop.TotalChain >= 3 Then
            chains(2) = New AcylChain(lipid.Sn3CarbonCount, New DoubleBond(lipid.Sn3DoubleBondCount), New Oxidized(lipid.Sn3Oxidizedount))
        End If
        If prop.TotalChain >= 4 Then
            chains(3) = New AcylChain(lipid.Sn4CarbonCount, New DoubleBond(lipid.Sn4DoubleBondCount), New Oxidized(lipid.Sn4Oxidizedount))
        End If
        Return chains
    End Function

    <Extension()>
    Public Function GetTypedChains(Of T As IChain)(chain As ITotalChain) As IEnumerable(Of T)
        Return chain.GetDeterminedChains().OfType(Of T)()
    End Function

    <Extension()>
    Public Function Deconstruct(Of tT As IChain, tU As IChain)(chain As ITotalChain) As (tT, tU)
        If chain.ChainCount <> 2 OrElse GetType(tT) Is GetType(tU) Then
            Return Nothing
        End If
        Dim t = chain.GetTypedChains(Of tT)().SingleOrDefault()
        Dim u = chain.GetTypedChains(Of tU)().SingleOrDefault()
        If TypeOf t Is tT AndAlso TypeOf u Is tU Then
            Return (t, u)
        End If
        Return Nothing
    End Function

    <Extension()>
    Public Sub ApplyToChain(chains As ITotalChain, position As Integer, action As Action(Of IChain))
        Dim chain = chains.GetChainByPosition(position)
        If chain IsNot Nothing Then
            action?.Invoke(chain)
        End If
    End Sub

    <Extension()>
    Public Sub ApplyToChain(Of T As IChain)(chains As ITotalChain, position As Integer, action As Action(Of T))
        Dim chain = chains.GetChainByPosition(position)
        Dim c As T = chain

        If c IsNot Nothing Then
            action?.Invoke(c)
        End If
    End Sub
End Module
