#Region "Microsoft.VisualBasic::f7653597d709ddfc928974a6aab394fc, G:/mzkit/src/metadb/Lipidomics//TotalChainVariationGenerator.vb"

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

    '   Total Lines: 247
    '    Code Lines: 202
    ' Comment Lines: 1
    '   Blank Lines: 44
    '     File Size: 15.29 KB


    ' Class TotalChainVariationGenerator
    ' 
    '     Properties: MinLength
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: CarbonNumberValid, CreateAcylChain, CreateAlkylChain, CreateSphingoChain, Distribute
    '               DoubleBondIsValid, InternalSeparate, (+2 Overloads) IsLexicographicOrder, ListingCandidates, Permutate
    '               Product, RecurseGenerate, Separate
    ' 
    '     Class ChainCandidate
    ' 
    '         Properties: CarbonCount, ChainCount, DoubleBondCount, MinimumOxidizedCount, OxidizedCount
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class ChainSet
    ' 
    '         Properties: AcylCandidate, AlkylCandidate, SphingoCandidate
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class PositionLevelChainEqualityCompaerer
    ' 
    '         Function: Equals, GetHashCode
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Concurrent
Imports std = System.Math


Public Class TotalChainVariationGenerator
    Implements ITotalChainVariationGenerator
    Public Sub New(chainGenerator As IChainGenerator, minLength As Integer)
        Me.MinLength = minLength
        Me.chainGenerator = chainGenerator
    End Sub

    Public Sub New(Optional minLength As Integer = 6, Optional begin As Integer = 3, Optional [end] As Integer = 3, Optional skip As Integer = 3)
        Me.New(New ChainGenerator(begin, [end], skip), minLength)

    End Sub

    Public ReadOnly Property MinLength As Integer

    Private ReadOnly chainGenerator As IChainGenerator

    Public Function Separate(chain As TotalChain) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Separate
        Return InternalSeparate(chain).SelectMany(New Func(Of ChainSet, IEnumerable(Of ITotalChain))(AddressOf ListingCandidates))
    End Function

    Friend Class ChainCandidate
        Public Sub New(chainCount As Integer, carbonCount As Integer, doubleBondCount As Integer, oxidizedCount As Integer, minimumOxidizedCount As Integer)
            Me.ChainCount = chainCount
            Me.CarbonCount = carbonCount
            Me.DoubleBondCount = doubleBondCount
            Me.OxidizedCount = oxidizedCount
            Me.MinimumOxidizedCount = minimumOxidizedCount
        End Sub

        Public ReadOnly Property ChainCount As Integer

        Public ReadOnly Property CarbonCount As Integer

        Public ReadOnly Property DoubleBondCount As Integer

        Public ReadOnly Property OxidizedCount As Integer

        Public ReadOnly Property MinimumOxidizedCount As Integer
    End Class

    Friend Class ChainSet
        Public Sub New(acylCandidate As ChainCandidate, alkylCandidate As ChainCandidate, sphingoCandidate As ChainCandidate)
            Me.AcylCandidate = acylCandidate
            Me.AlkylCandidate = alkylCandidate
            Me.SphingoCandidate = sphingoCandidate
        End Sub

        Public ReadOnly Property AcylCandidate As ChainCandidate
        Public ReadOnly Property AlkylCandidate As ChainCandidate
        Public ReadOnly Property SphingoCandidate As ChainCandidate
    End Class

    ' TODO: refactoring
    Private Function ListingCandidates(candidates As ChainSet) As IEnumerable(Of ITotalChain)
        If candidates.SphingoCandidate.ChainCount > 0 Then
            Dim sphingos = RecurseGenerate(candidates.SphingoCandidate, New Func(Of Integer, Integer, Integer, SphingoChain)(AddressOf CreateSphingoChain)).ToArray()
            Dim acyls = RecurseGenerate(candidates.AcylCandidate, New Func(Of Integer, Integer, Integer, AcylChain)(AddressOf CreateAcylChain)).ToArray()
            Dim alkyls = RecurseGenerate(candidates.AlkylCandidate, New Func(Of Integer, Integer, Integer, AlkylChain)(AddressOf CreateAlkylChain)).ToArray()
            Return From sphingo In sphingos
                   From acyl In acyls
                   From alkyl In alkyls
                   Select New PositionLevelChains(sphingo.Select(Function(si) DirectCast(si, IChain)).Concat(acyl.Select(Function(ai) DirectCast(ai, IChain))).Concat(alkyl.Select(Function(ai) DirectCast(ai, IChain))).ToArray())
        Else
            Dim acyls = RecurseGenerate(candidates.AcylCandidate, New Func(Of Integer, Integer, Integer, AcylChain)(AddressOf CreateAcylChain)).ToArray()
            Dim alkyls = RecurseGenerate(candidates.AlkylCandidate, New Func(Of Integer, Integer, Integer, AlkylChain)(AddressOf CreateAlkylChain)).ToArray()
            Return From acyl In acyls
                   From alkyl In alkyls
                   Select New MolecularSpeciesLevelChains(alkyl.Select(Function(ai) DirectCast(ai, IChain)).Concat(acyl.Select(Function(ai) DirectCast(ai, IChain))).ToArray())
        End If
    End Function

    Private Function InternalSeparate(chains As TotalChain) As IEnumerable(Of ChainSet)
        Dim minAcylCarbonMinAlkylCarbonMinSphingoCarbon As (minAcylCarbon As Integer, minAlkylCarbon As Integer, minSphingoCarbon As Integer) = Nothing
        Dim minAcylCarbon As Integer, minAlkylCarbon As Integer, minSphingoCarbon As Integer
        minAcylCarbonMinAlkylCarbonMinSphingoCarbon = (MinLength * chains.AcylChainCount, MinLength * chains.AlkylChainCount, MinLength * chains.SphingoChainCount)

        With minAcylCarbonMinAlkylCarbonMinSphingoCarbon
            minAcylCarbon = .minAcylCarbon
            minAlkylCarbon = .minAlkylCarbon
            minSphingoCarbon = .minSphingoCarbon
        End With

        Dim carbonRemain = chains.CarbonCount - minAcylCarbon - minAlkylCarbon - minSphingoCarbon
        If carbonRemain < 0 Then
            Return Enumerable.Empty(Of ChainSet)()
        End If

        Dim maxAcylCarbonMaxAlkylCarbonMaxSphingoCarbon As (maxAcylCarbon As Integer, maxAlkylCarbon As Integer, maxSphingoCarbon As Integer) = Nothing
        Dim maxAcylCarbon As Integer, maxAlkylCarbon As Integer, maxSphingoCarbon As Integer
        maxAcylCarbonMaxAlkylCarbonMaxSphingoCarbon = ((carbonRemain + minAcylCarbon) * std.Sign(chains.AcylChainCount), (carbonRemain + minAlkylCarbon) * std.Sign(chains.AlkylChainCount), (carbonRemain + minSphingoCarbon) * std.Sign(chains.SphingoChainCount))

        With maxAcylCarbonMaxAlkylCarbonMaxSphingoCarbon
            maxAcylCarbon = .maxAcylCarbon
            maxAlkylCarbon = .maxAlkylCarbon
            maxSphingoCarbon = .maxSphingoCarbon
        End With

        Dim minAcylDbMinAlkylDbMinSphingoDb As (minAcylDb As Integer, minAlkylDb As Integer, minSphingoDb As Integer) = Nothing
        Dim minAcylDb As Integer, minAlkylDb As Integer, minSphingoDb As Integer
        minAcylDbMinAlkylDbMinSphingoDb = (0, 0, 0)
        Dim dbRemain = chains.DoubleBondCount - minAcylDb - minAlkylDb - minSphingoDb
        Dim maxAcylDbMaxAlkylDbMaxSphingoDb As (maxAcylDb As Integer, maxAlkylDb As Integer, maxSphingoDb As Integer) = Nothing
        Dim maxAcylDb As Integer, maxAlkylDb As Integer, maxSphingoDb As Integer
        maxAcylDbMaxAlkylDbMaxSphingoDb = ((dbRemain + minAcylDb) * std.Sign(chains.AcylChainCount), (dbRemain + minAlkylDb) * std.Sign(chains.AlkylChainCount), (dbRemain + minSphingoDb) * std.Sign(chains.SphingoChainCount))

        With maxAcylDbMaxAlkylDbMaxSphingoDb
            maxAcylDb = .maxAcylDb
            maxAlkylDb = .maxAlkylDb
            maxSphingoDb = .maxSphingoDb
        End With

        Dim minAcylOxMinAlkylOxMinSphingoOx As (minAcylOx As Integer, minAlkylOx As Integer, minSphingoOx As Integer) = Nothing
        Dim minAcylOx As Integer, minAlkylOx As Integer, minSphingoOx As Integer
        minAcylOxMinAlkylOxMinSphingoOx = (0, 0, chains.SphingoChainCount * 2)
        With minAcylOxMinAlkylOxMinSphingoOx
            minAcylOx = .minAcylOx
            minAlkylOx = .minAlkylOx
            minSphingoOx = .minSphingoOx
        End With
        Dim oxRemain = chains.OxidizedCount - minAcylOx - minAlkylOx - minSphingoOx
        Dim maxAcylOxMaxAlkylOxMaxSphingoOx As (maxAcylOx As Integer, maxAlkylOx As Integer, maxSphingoOx As Integer) = Nothing
        Dim maxAcylOx As Integer, maxAlkylOx As Integer, maxSphingoOx As Integer
        maxAcylOxMaxAlkylOxMaxSphingoOx = ((dbRemain + minAcylOx) * std.Sign(chains.AcylChainCount), (dbRemain + minAlkylOx) * std.Sign(chains.AlkylChainCount), (dbRemain + minSphingoOx) * std.Sign(chains.SphingoChainCount))

        With maxAcylOxMaxAlkylOxMaxSphingoOx
            maxAcylOx = .maxAcylOx
            maxAlkylOx = .maxAlkylOx
            maxSphingoOx = .maxSphingoOx
        End With

        Dim css = Me.Distribute(chains.CarbonCount, minAcylCarbon, maxAcylCarbon, minAlkylCarbon, maxAlkylCarbon, minSphingoCarbon, maxSphingoCarbon).ToArray()
        Dim dbss = Me.Distribute(chains.DoubleBondCount, minAcylDb, maxAcylDb, minAlkylDb, maxAlkylDb, minSphingoDb, maxSphingoDb).ToArray()
        Dim oxss = Me.Distribute(chains.OxidizedCount, minAcylOx, maxAcylOx, minAlkylOx, maxAlkylOx, minSphingoOx, maxSphingoOx).ToArray()

        Return From cs In css From dbs In dbss From oxs In oxss Select New ChainSet(New ChainCandidate(chains.AcylChainCount, cs(0), dbs(0), oxs(0), 0), New ChainCandidate(chains.AlkylChainCount, cs(1), dbs(1), oxs(1), 0), New ChainCandidate(chains.SphingoChainCount, cs(2), dbs(2), oxs(2), 2))
    End Function

    Private Function Distribute(count As Integer, acylMin As Integer, acylMax As Integer, alkylMin As Integer, alkylMax As Integer, sphingoMin As Integer, sphingoMax As Integer) As IEnumerable(Of Integer())
        Return From i In Enumerable.Range(acylMin, acylMax - acylMin + 1) Where count - i <= alkylMax + sphingoMax From j In Enumerable.Range(alkylMin, alkylMax - alkylMin + 1) Let k = count - i - j Where sphingoMin <= k AndAlso k <= sphingoMax Select {i, j, k}
    End Function

    Public Function Permutate(chains As MolecularSpeciesLevelChains) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Permutate
        Return Permutations(chains.GetDeterminedChains()).Select(Function([set]) New PositionLevelChains([set])).Distinct(ChainsComparer)
    End Function

    Public Function Product(chains As PositionLevelChains) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Product
        If chains.GetDeterminedChains().All(Function(chain) chain.DoubleBond.UnDecidedCount = 0 AndAlso chain.Oxidized.UnDecidedCount = 0) Then
            Return Enumerable.Empty(Of ITotalChain)()
        End If
        Return CartesianProduct(chains.GetDeterminedChains().[Select](Function(c) c.GetCandidates(chainGenerator).ToArray()).ToArray()).[Select](Function([set]) New PositionLevelChains([set])).Distinct(ChainsComparer)
    End Function

    Private Function CarbonNumberValid(curCarbon As Integer) As Boolean
        Return curCarbon >= MinLength AndAlso chainGenerator.CarbonIsValid(curCarbon)
    End Function

    Private Function DoubleBondIsValid(carbon As Integer, db As Integer) As Boolean
        Return chainGenerator.DoubleBondIsValid(carbon, db)
    End Function

    Private Function IsLexicographicOrder(prevCarbon As Integer, prevDb As Integer, curCarbon As Integer, curDb As Integer) As Boolean
        Return (prevCarbon, prevDb).CompareTo((curCarbon, curDb)) <= 0
    End Function

    Private Function IsLexicographicOrder(prevCarbon As Integer, prevDb As Integer, prevOx As Integer, curCarbon As Integer, curDb As Integer, curOx As Integer) As Boolean
        Return (prevCarbon, prevDb, prevOx).CompareTo((curCarbon, curDb, curOx)) <= 0
    End Function

    Private Function RecurseGenerate(Of T)(candidate As ChainCandidate, create As Func(Of Integer, Integer, Integer, T)) As IEnumerable(Of T())
        If candidate.ChainCount = 0 Then
            If candidate.CarbonCount = 0 AndAlso candidate.DoubleBondCount = 0 AndAlso candidate.OxidizedCount = 0 Then
                Return {New T(-1) {}}
            Else
                Return Enumerable.Empty(Of T())()
            End If
        End If

        Dim [set] = New T(candidate.ChainCount - 1) {}
        Dim rec As Func(Of Integer, Integer, Integer, Integer, Integer, Integer, Integer, IEnumerable(Of T())) =
            Iterator Function(carbon_ As Integer, db_ As Integer, ox_ As Integer, minCarbon_ As Integer, minDb_ As Integer, minOx_ As Integer, chain_ As Integer) As IEnumerable(Of T())
                If (chain_ = 1) Then
                    If (Me.DoubleBondIsValid(carbon_, db_) AndAlso Me.IsLexicographicOrder(minCarbon_, minDb_, minOx_, carbon_, db_, ox_)) Then
                        [set](candidate.ChainCount - 1) = create(carbon_, db_, ox_)
                        Yield [set].ToArray()
                    End If
                    Return
                End If
                For c = minCarbon_ To carbon_ / chain_
                    If (Not Me.CarbonNumberValid(c)) Then
                        Continue For
                    End If
                    For d = 0 To db_

                        If Not Me.DoubleBondIsValid(c, d) Then
                            Exit For
                        End If
                        If (Not Me.IsLexicographicOrder(minCarbon_, minDb_, c, d)) Then
                            Continue For
                        End If
                        For o = candidate.MinimumOxidizedCount To ox_
                            If (Not Me.IsLexicographicOrder(minCarbon_, minDb_, minOx_, c, d, o)) Then
                                Continue For
                            End If
                            [set](candidate.ChainCount - chain_) = create(c, d, o)
                            For Each res In rec(carbon_ - c, db_ - d, ox_ - o, c, d, o, chain_ - 1)
                                Yield res
                            Next
                        Next
                    Next
                Next
            End Function

        Return rec(candidate.CarbonCount, candidate.DoubleBondCount, candidate.OxidizedCount, MinLength, -1, -1, candidate.ChainCount)
    End Function

    Private ReadOnly _sphingoCache As ConcurrentDictionary(Of (Integer, Integer, Integer), SphingoChain) = New ConcurrentDictionary(Of (Integer, Integer, Integer), SphingoChain)()
    Private Function CreateSphingoChain(carbon As Integer, db As Integer, ox As Integer) As SphingoChain
        Return _sphingoCache.GetOrAdd((carbon, db, ox), Function(triple) New SphingoChain(triple.Item1, New DoubleBond(triple.Item2), New Oxidized(triple.Item3, 1, 3)))
    End Function

    Private ReadOnly _acylCache As ConcurrentDictionary(Of (Integer, Integer, Integer), AcylChain) = New ConcurrentDictionary(Of (Integer, Integer, Integer), AcylChain)()
    Private Function CreateAcylChain(carbon As Integer, db As Integer, ox As Integer) As AcylChain
        Return _acylCache.GetOrAdd((carbon, db, ox), Function(triple) New AcylChain(triple.Item1, New DoubleBond(triple.Item2), New Oxidized(triple.Item3)))
    End Function

    Private ReadOnly _alkylCache As ConcurrentDictionary(Of (Integer, Integer, Integer), AlkylChain) = New ConcurrentDictionary(Of (Integer, Integer, Integer), AlkylChain)()
    Private Function CreateAlkylChain(carbon As Integer, db As Integer, ox As Integer) As AlkylChain
        Return _alkylCache.GetOrAdd((carbon, db, ox), Function(triple) New AlkylChain(triple.Item1, New DoubleBond(triple.Item2), New Oxidized(triple.Item3)))
    End Function

    Private Shared ReadOnly ChainsComparer As PositionLevelChainEqualityCompaerer = New PositionLevelChainEqualityCompaerer()

    Friend Class PositionLevelChainEqualityCompaerer
        Implements IEqualityComparer(Of ITotalChain)
        Public Overloads Function Equals(x As ITotalChain, y As ITotalChain) As Boolean Implements IEqualityComparer(Of ITotalChain).Equals
            Return Equals(x.ToString(), y.ToString())
        End Function

        Public Overloads Function GetHashCode(obj As ITotalChain) As Integer Implements IEqualityComparer(Of ITotalChain).GetHashCode
            Return obj.ToString().GetHashCode()
        End Function
    End Class
End Class
