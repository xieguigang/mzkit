#Region "Microsoft.VisualBasic::6d0043c276ddd7865c0ad219cc533889, mzmath\Mummichog\Annealing\Permutation.vb"

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

    '   Total Lines: 67
    '    Code Lines: 47
    ' Comment Lines: 9
    '   Blank Lines: 11
    '     File Size: 2.30 KB


    ' Module Permutation
    ' 
    '     Function: CreateCombinations, GetRandom
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' A helper module for generates the combination
''' </summary>
Public Module Permutation

    ''' <summary>
    ''' Populate n annotation candidate combination result, the candidate list size n is equals to <paramref name="permutations"/>.
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="permutations"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function CreateCombinations(input As IEnumerable(Of MzSet), permutations As Integer) As IEnumerable(Of MzQuery())
        Dim raw As MzSet() = input.ToArray
        Dim block As New List(Of MzQuery)
        Dim target As MzQuery
        Dim filter As Index(Of String) = {}
        Dim delta As Integer = permutations / 10

        For i As Integer = 0 To permutations
            Call filter.Clear()
            Call block.Clear()

            For Each mz As MzSet In raw
                target = mz.GetRandom(filter)

                If target Is Nothing Then
                    Continue For
                End If

                If Not target.unique_id Is Nothing Then
                    Call block.Add(target)
                    Call filter.Add(target.unique_id)
                End If
            Next

            Yield block _
                .GroupBy(Function(a) a.unique_id) _
                .Select(Function(a) a.OrderBy(Function(v) v.ppm).First) _
                .ToArray

            If i Mod delta = 0 Then
                Call VBDebugger.EchoLine($" -- {(i / permutations * 100).ToString("F2")}% [{i}/{permutations}]")
            End If
        Next
    End Function

    <Extension>
    Private Function GetRandom(mzset As MzSet, filter As Index(Of String)) As MzQuery
        Dim filters = mzset.query _
            .Where(Function(a) Not a.unique_id Like filter) _
            .ToArray

        If filters.Length = 0 Then
            Return Nothing
        Else
            Dim i As Integer = randf.NextInteger(mzset.size)
            Dim target As MzQuery = mzset(i)

            Return target
        End If
    End Function
End Module
