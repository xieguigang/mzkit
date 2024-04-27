#Region "Microsoft.VisualBasic::358d060ce62c86bbea5d51898616db99, G:/mzkit/src/mzmath/Mummichog//Annealing/ContextFitness.vb"

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

    '   Total Lines: 57
    '    Code Lines: 40
    ' Comment Lines: 9
    '   Blank Lines: 8
    '     File Size: 2.08 KB


    ' Class ContextFitness
    ' 
    '     Properties: Cacheable
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Calculate, Enrich
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF

Public Class ContextFitness : Implements Fitness(Of AnnotationSet)

    Public ReadOnly Property Cacheable As Boolean Implements Fitness(Of AnnotationSet).Cacheable
        Get
            Return True
        End Get
    End Property

    Dim background As NamedValue(Of NetworkGraph)()
    Dim pinList As Index(Of String)
    Dim modelSize As Integer = -1
    Dim ignoreTopology As Boolean = False

    Sub New(background As IEnumerable(Of NamedValue(Of NetworkGraph)),
            Optional modelSize As Integer = -1,
            Optional pinned As String() = Nothing,
            Optional ignoreTopology As Boolean = False)

        Me.background = background.ToArray
        Me.pinList = pinned.Indexing
        Me.modelSize = modelSize
        Me.ignoreTopology = ignoreTopology
    End Sub

    Public Function Enrich(obj As AnnotationSet) As ActivityEnrichment()
        Return obj.CandidateSet.PeakListAnnotation(
           background, pinList, modelSize, ignoreTopology,
           parallel:=True)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="chromosome"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the smaller fitness value is better
    ''' </remarks>
    Public Function Calculate(chromosome As AnnotationSet, parallel As Boolean) As Double Implements Fitness(Of AnnotationSet).Calculate
        Dim result As ActivityEnrichment() = chromosome.CandidateSet.PeakListAnnotation(
            background, pinList, modelSize, ignoreTopology,
            parallel:=parallel)
        Dim score As Double = result.Score(ignoreTopology)

        If score <= 0 Then
            Return Double.MaxValue
        Else
            Return 1 / score
        End If
    End Function
End Class

