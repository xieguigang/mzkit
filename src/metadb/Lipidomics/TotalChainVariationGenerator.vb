﻿Imports CompMs.Common.Utility
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq

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
            Return From sphingo In sphingos From acyl In acyls From alkyl In alkyls Select New PositionLevelChains(sphingo.Concat(Of IChain)(acyl).Concat(alkyl).ToArray())
        Else
            Dim acyls = RecurseGenerate(candidates.AcylCandidate, New Func(Of Integer, Integer, Integer, AcylChain)(AddressOf CreateAcylChain)).ToArray()
            Dim alkyls = RecurseGenerate(candidates.AlkylCandidate, New Func(Of Integer, Integer, Integer, AlkylChain)(AddressOf CreateAlkylChain)).ToArray()
            Return From acyl In acyls From alkyl In alkyls Select New MolecularSpeciesLevelChains(alkyl.Concat(Of IChain)(acyl).ToArray())
        End If
    End Function

    Private Function InternalSeparate(chains As TotalChain) As IEnumerable(Of ChainSet)
        Dim minAcylCarbonMinAlkylCarbonMinSphingoCarbon As (minAcylCarbon As Integer, minAlkylCarbon As Integer, minSphingoCarbon As Integer) = Nothing
        minAcylCarbonMinAlkylCarbonMinSphingoCarbon = (MinLength * chains.AcylChainCount, MinLength * chains.AlkylChainCount, MinLength * chains.SphingoChainCount)
        Dim carbonRemain = chains.CarbonCount - minAcylCarbon - minAlkylCarbon - minSphingoCarbon
        If carbonRemain < 0 Then
            Return Enumerable.Empty(Of ChainSet)()
        End If

        Dim maxAcylCarbonMaxAlkylCarbonMaxSphingoCarbon As (maxAcylCarbon As Integer, maxAlkylCarbon As Integer, maxSphingoCarbon As Integer) = Nothing
        maxAcylCarbonMaxAlkylCarbonMaxSphingoCarbon = ((carbonRemain + minAcylCarbon) * Math.Sign(chains.AcylChainCount), (carbonRemain + minAlkylCarbon) * Math.Sign(chains.AlkylChainCount), (carbonRemain + minSphingoCarbon) * Math.Sign(chains.SphingoChainCount))
        Dim minAcylDbMinAlkylDbMinSphingoDb As (minAcylDb As Integer, minAlkylDb As Integer, minSphingoDb As Integer) = Nothing
        minAcylDbMinAlkylDbMinSphingoDb = (0, 0, 0)
        Dim dbRemain = chains.DoubleBondCount - minAcylDb - minAlkylDb - minSphingoDb
        Dim maxAcylDbMaxAlkylDbMaxSphingoDb As (maxAcylDb As Integer, maxAlkylDb As Integer, maxSphingoDb As Integer) = Nothing
        maxAcylDbMaxAlkylDbMaxSphingoDb = ((dbRemain + minAcylDb) * Math.Sign(chains.AcylChainCount), (dbRemain + minAlkylDb) * Math.Sign(chains.AlkylChainCount), (dbRemain + minSphingoDb) * Math.Sign(chains.SphingoChainCount))
        Dim minAcylOxMinAlkylOxMinSphingoOx As (minAcylOx As Integer, minAlkylOx As Integer, minSphingoOx As Integer) = Nothing
        minAcylOxMinAlkylOxMinSphingoOx = (0, 0, chains.SphingoChainCount * 2)
        Dim oxRemain = chains.OxidizedCount - minAcylOx - minAlkylOx - minSphingoOx
        Dim maxAcylOxMaxAlkylOxMaxSphingoOx As (maxAcylOx As Integer, maxAlkylOx As Integer, maxSphingoOx As Integer) = Nothing
        maxAcylOxMaxAlkylOxMaxSphingoOx = ((dbRemain + minAcylOx) * Math.Sign(chains.AcylChainCount), (dbRemain + minAlkylOx) * Math.Sign(chains.AlkylChainCount), (dbRemain + minSphingoOx) * Math.Sign(chains.SphingoChainCount))

        Dim css = Me.Distribute(chains.CarbonCount, minAcylCarbon, maxAcylCarbon, minAlkylCarbon, maxAlkylCarbon, minSphingoCarbon, maxSphingoCarbon).ToArray()
        Dim dbss = Me.Distribute(chains.DoubleBondCount, minAcylDb, maxAcylDb, minAlkylDb, maxAlkylDb, minSphingoDb, maxSphingoDb).ToArray()
        Dim oxss = Me.Distribute(chains.OxidizedCount, minAcylOx, maxAcylOx, minAlkylOx, maxAlkylOx, minSphingoOx, maxSphingoOx).ToArray()

        Return From cs In css From dbs In dbss From oxs In oxss Select New ChainSet(New ChainCandidate(chains.AcylChainCount, cs(0), dbs(0), oxs(0), 0), New ChainCandidate(chains.AlkylChainCount, cs(1), dbs(1), oxs(1), 0), New ChainCandidate(chains.SphingoChainCount, cs(2), dbs(2), oxs(2), 2))
    End Function

    Private Function Distribute(count As Integer, acylMin As Integer, acylMax As Integer, alkylMin As Integer, alkylMax As Integer, sphingoMin As Integer, sphingoMax As Integer) As IEnumerable(Of Integer())
        Return From i In Enumerable.Range(acylMin, acylMax - acylMin + 1) Where count - i <= alkylMax + sphingoMax From j In Enumerable.Range(alkylMin, alkylMax - alkylMin + 1) Let k = count - i - j Where sphingoMin <= k AndAlso k <= sphingoMax Select {i, j, k}
    End Function

    Public Function Permutate(chains As MolecularSpeciesLevelChains) As IEnumerable(Of ITotalChain) Implements ITotalChainVariationGenerator.Permutate
        Return Permutations(chains.GetDeterminedChains()).[Select](Of IChain(), ITotalChain)(Function([set]) New PositionLevelChains([set])).Distinct(ChainsComparer)
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
        ''' Cannot convert LocalFunctionStatementSyntax, CONVERSION ERROR: Conversion for LocalFunctionStatement not implemented, please report this issue in 'System.Collections.Generic....' at character 10242
        ''' 
        ''' 
        ''' Input:
        '''             System.Collections.Generic.IEnumerable<T[]> rec(int carbon_, int db_, int ox_, int minCarbon_, int minDb_, int minOx_, int chain_) {
        If (chain_ == 1) Then {
                    If (this.DoubleBondIsValid(carbon_, db_) && this.IsLexicographicOrder(minCarbon_, minDb_, minOx_, carbon_, db_, ox_)) Then {
                        @set[candidate.ChainCount - 1] = create(carbon_, db_, ox_);
                        yield return @set.ToArray();
                    }
                    yield break;
                }
                For (var c = minCarbon_; c <= carbon_ / chain_; c++) {
                    If (!this.CarbonNumberValid(c)) Then {
                        Continue For;
                    }
                    For (var d = 0; d <= db_ && this.DoubleBondIsValid(c, d); d++) {
                        If (!this.IsLexicographicOrder(minCarbon_, minDb_, c, d)) Then {
                            Continue For;
                        }
                        For (var o = candidate.MinimumOxidizedCount; o <= ox_; o++) {
                            If (!this.IsLexicographicOrder(minCarbon_, minDb_, minOx_, c, d, o)) Then {
                                Continue For;
                            }
                            @set[candidate.ChainCount - chain_] = create(c, d, o);
                            foreach(var res In rec(carbon_ - c, db_ - d, ox_ - o, c, d, o, chain_ - 1)) {
                                yield return res;
                            }
                        }
                    }
                }
            }

''' 

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
        Public Function Equals(x As ITotalChain, y As ITotalChain) As Boolean Implements IEqualityComparer(Of ITotalChain).Equals
            Return Equals(x.ToString(), y.ToString())
        End Function

        Public Function GetHashCode(obj As ITotalChain) As Integer Implements IEqualityComparer(Of ITotalChain).GetHashCode
            Return obj.ToString().GetHashCode()
        End Function
    End Class
End Class