#Region "Microsoft.VisualBasic::e5aec44abf7cc5ea1f166ce43b970ceb, src\metadb\Massbank\MetaLib\Match\SynonymIndex.vb"

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

    '     Interface ICompoundNames
    ' 
    '         Function: GetSynonym
    ' 
    '     Class SynonymIndex
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: BuildIndex, (+2 Overloads) FindCandidateCompounds, GetEnumerator, IEnumerable_GetEnumerator
    ' 
    '         Sub: Add
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Trinity
Imports Microsoft.VisualBasic.Linq

Namespace MetaLib

    Public Interface ICompoundNames

        Function GetSynonym() As IEnumerable(Of String)

    End Interface

    Public Class SynonymIndex(Of T As ICompoundNames) : Implements IEnumerable(Of T)

        ReadOnly bin As WordSimilarityIndex(Of T)

        Sub New(Optional equalsName As Double = 0.9)
            bin = New WordSimilarityIndex(Of T)(New WordSimilarity(equalsName))
        End Sub

        Public Sub Add(compound As T)
            For Each name As String In compound.GetSynonym
                If Not bin.HaveKey(name) Then
                    Call bin.AddTerm(name, compound)
                Else
                    Call $"{name} ({compound})".Warning
                End If
            Next
        End Sub

        Public Function BuildIndex(compounds As IEnumerable(Of T)) As SynonymIndex(Of T)
            For Each compound As T In compounds
                Call Add(compound)
            Next

            Return Me
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function FindCandidateCompounds(name As String) As IEnumerable(Of T)
            Return bin.FindMatches(name)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function FindCandidateCompounds(names As IEnumerable(Of String)) As IEnumerable(Of T)
            Return names.Distinct.Select(Function(name) bin.FindMatches(name)).IteratesALL
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each item As T In bin.AllValues
                Yield item
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
