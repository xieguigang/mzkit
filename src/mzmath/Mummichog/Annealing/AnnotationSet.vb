#Region "Microsoft.VisualBasic::1a61679df5ee7eb87b8e5fb5f08629a6, mzmath\Mummichog\Annealing\AnnotationSet.vb"

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

    '   Total Lines: 98
    '    Code Lines: 61 (62.24%)
    ' Comment Lines: 22 (22.45%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (15.31%)
    '     File Size: 3.56 KB


    ' Class AnnotationSet
    ' 
    '     Properties: CandidateSet, i, IonSet, Key, MutationRate
    '                 Score, UniqueHitSize
    ' 
    '     Function: Crossover, Mutate, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Helper
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class AnnotationSet : Implements Chromosome(Of AnnotationSet)

    Public Property MutationRate As Double Implements Chromosome(Of AnnotationSet).MutationRate
    Public Property Score As Double

    Public ReadOnly Property Key As String Implements IReadOnlyId.Identity
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return FNV1a.GetHashCode(IonSet.Select(Function(m, i) m(Me.i(i)).unique_id)).ToString
        End Get
    End Property

    Public Property IonSet As MzSet()

    ''' <summary>
    ''' The chromosome composition
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' the value range of each element in this vector is based on
    ''' the <see cref="MzSet.size"/> of the corresponding element 
    ''' in <see cref="IonSet"/> collection.
    ''' </remarks>
    Public Property i As Integer()

    Public ReadOnly Property CandidateSet As MzQuery()
        Get
            Return i _
                .Select(Function(idx, j) IonSet(j)(idx)) _
                .GroupBy(Function(a) a.unique_id) _
                .Select(Function(a) a.First) _
                .ToArray
        End Get
    End Property

    ''' <summary>
    ''' the size of the result output <see cref="CandidateSet"/>.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property UniqueHitSize As Integer
        Get
            Return i _
                .Select(Function(idx, j)
                            Return IonSet(j)(idx)
                        End Function) _
                .GroupBy(Function(a) a.unique_id) _
                .Count
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"<{UInteger.Parse(Key).ToHexString}> {UniqueHitSize} uniq candidates"
    End Function

    Public Iterator Function Crossover(another As AnnotationSet) As IEnumerable(Of AnnotationSet) Implements Chromosome(Of AnnotationSet).Crossover
        Dim clone1 As New AnnotationSet With {.i = Me.i.ToArray, .IonSet = IonSet, .MutationRate = MutationRate}
        Dim clone2 As New AnnotationSet With {.i = another.i.ToArray, .IonSet = IonSet, .MutationRate = MutationRate}

        Call clone1.i.Crossover(clone2.i)

        Yield clone1
        Yield clone2
    End Function

    ''' <summary>
    ''' generate a new annotation candidate combination
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 1. this function make an object copy at first
    ''' 2. and then do the annotation candidate mutations
    ''' 3. returns the new clone one
    ''' </remarks>
    Public Function Mutate() As AnnotationSet Implements Chromosome(Of AnnotationSet).Mutate
        Dim clone As New AnnotationSet With {
            .IonSet = IonSet,
            .MutationRate = MutationRate
        }
        Dim index As Integer() = Me.i.ToArray

        For i As Integer = 0 To index.Length - 1
            If randf.NextDouble < MutationRate Then
                index(i) = randf.NextInteger(IonSet(i).size)
            End If
        Next

        clone.i = index

        Return clone
    End Function
End Class
