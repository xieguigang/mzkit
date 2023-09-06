Imports CompMs.Common.Extension
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Class ChainGenerator
    Implements IChainGenerator
    Public Sub New(Optional begin As Integer = 3, Optional [end] As Integer = 3, Optional skip As Integer = 3)
        Me.Begin = begin
        Me.End = [end]
        Me.Skip = skip
    End Sub

    Public ReadOnly Property Begin As Integer ' if begin is 3, first double bond is 3-4 at the earliest counting from ketone carbon.
    Public ReadOnly Property [End] As Integer ' if end is 3 and number of carbon is 18, last double bond is 15-16 at latest.
    Public ReadOnly Property Skip As Integer ' if skip is 3 and 6-7 is double bond, next one is 9-10 at the earliest.

    Public Function Generate(chain As AcylChain) As IEnumerable(Of IChain) Implements IChainGenerator.Generate
        Dim bs = EnumerateBonds(chain.CarbonCount, chain.DoubleBond).ToArray()
        Dim os = EnumerateOxidized(chain.CarbonCount, chain.Oxidized, 2).ToArray()
        Return bs.SelectMany(Function(__) os, Function(b, o) New AcylChain(chain.CarbonCount, b, o))
    End Function

    Public Function Generate(chain As AlkylChain) As IEnumerable(Of IChain) Implements IChainGenerator.Generate
        Dim bs = EnumerateBonds(chain.CarbonCount, chain.DoubleBond).ToArray()
        Dim os = EnumerateOxidized(chain.CarbonCount, chain.Oxidized, 2).ToArray()
        Return bs.SelectMany(Function(__) os, Function(b, o) New AlkylChain(chain.CarbonCount, b, o))
    End Function

    Public Function Generate(chain As SphingoChain) As IEnumerable(Of IChain) Implements IChainGenerator.Generate
        Dim bs = EnumerateBonds(chain.CarbonCount, chain.DoubleBond).ToArray()
        Dim os = EnumerateOxidized(chain.CarbonCount, chain.Oxidized, 4).ToArray()
        Return bs.SelectMany(Function(__) os, Function(b, o) New SphingoChain(chain.CarbonCount, b, o))
    End Function

    Private Function EnumerateBonds(carbon As Integer, doubleBond As IDoubleBond) As IEnumerable(Of IDoubleBond)
        If doubleBond.UnDecidedCount = 0 Then
            Return [Return](doubleBond)
        End If
        Dim used = New Boolean(carbon - 1) {}
        For i = 1 To Begin - 1
            used(i - 1) = True
        Next
        For Each bond In doubleBond.Bonds
            For i = Math.Max(1, bond.Position - Skip + 1) To bond.Position + Skip - 1
                used(i - 1) = True
            Next
        Next

        ''' Cannot convert LocalFunctionStatementSyntax, CONVERSION ERROR: Conversion for LocalFunctionStatement not implemented, please report this issue in 'System.Collections.Generic....' at character 2815
        ''' 
        ''' 
        ''' Input:
        ''' 
        System.Collections.Generic.IEnumerable<CompMs.Common.Lipidomics.IDoubleBond> rec(int i, System.Collections.Generic.List<CompMs.Common.Lipidomics.IDoubleBondInfo> infos) {
                if (infos.Count == doubleBond.UnDecidedCount) {
                    yield return new CompMs.Common.Lipidomics.DoubleBond(doubleBond.Bonds.Concat(infos).OrderBy(b => b.Position).ToArray());
                    yield break;
                }
                for (var j = i; j <= carbon - this.End; j++){
                    if (used[j - 1]) {
                        continue;
                    }
                    infos.Add(CompMs.Common.Lipidomics.DoubleBondInfo.Create(j));
                    foreach(var res In rec(j + this.Skip, infos)) {
                        yield return res;
                    }
                    infos.RemoveAt(infos.Count - 1);
                }
            }

''' 

            Return rec(Begin, New List(Of IDoubleBondInfo)(doubleBond.UnDecidedCount))
    End Function

    Private Function EnumerateOxidized(carbon As Integer, oxidized As IOxidized, begin As Integer) As IEnumerable(Of IOxidized)
        If oxidized.UnDecidedCount = 0 Then
            Return [Return](oxidized)
        End If

        ''' Cannot convert LocalFunctionStatementSyntax, CONVERSION ERROR: Conversion for LocalFunctionStatement not implemented, please report this issue in 'System.Collections.Generic....' at character 4193
        ''' 
        ''' 
        ''' Input:
        ''' 
        System.Collections.Generic.IEnumerable<CompMs.Common.Lipidomics.IOxidized> rec(int i, System.Collections.Generic.List<int> infos) {
                if (infos.Count == oxidized.UnDecidedCount) {
                    yield return CompMs.Common.Lipidomics.Oxidized.CreateFromPosition(oxidized.Oxidises.Concat(infos).OrderBy(p => p).ToArray());
                    yield break;
                }
                for (var j = i; j <carbon + 1; j++){
                    if (oxidized.Oxidises.Contains(j)) {
                        continue;
                    }
                    infos.Add(j);
                    foreach(var res In rec(j + 1, infos)) {
                        yield return res;
                    }
                    infos.RemoveAt(infos.Count - 1);
                }
            }

''' 

            Return rec(begin, New List(Of Integer)(oxidized.UnDecidedCount))
    End Function

    Public Function CarbonIsValid(carbon As Integer) As Boolean Implements IChainGenerator.CarbonIsValid
        Return True
    End Function

    Public Function DoubleBondIsValid(carbon As Integer, db As Integer) As Boolean Implements IChainGenerator.DoubleBondIsValid
        Return db = 0 OrElse carbon >= Begin + Skip * (db - 1) + [End]
    End Function
End Class
