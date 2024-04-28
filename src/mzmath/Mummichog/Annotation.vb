#Region "Microsoft.VisualBasic::668f1dbcee2d5fdc52caaa66d74f0fec, E:/mzkit/src/mzmath/Mummichog//Annotation.vb"

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

    '   Total Lines: 164
    '    Code Lines: 110
    ' Comment Lines: 33
    '   Blank Lines: 21
    '     File Size: 6.44 KB


    ' Module Annotation
    ' 
    '     Function: GetCandidateSet, (+2 Overloads) PeakListAnnotation, Score
    '     Class PeakListAnnotationTask
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Solve
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports std = System.Math

Public Module Annotation

    Private Class PeakListAnnotationTask : Inherits VectorTask

        ReadOnly background As NamedValue(Of NetworkGraph)()
        ReadOnly input As Dictionary(Of String, MzQuery)

        Public ReadOnly result As ActivityEnrichment()

        Public modelSize As Integer
        Public ignoreTopology As Boolean
        Public pinList As Index(Of String)

        Sub New(candidateList As MzQuery(), allsubgraph As NamedValue(Of NetworkGraph)())
            Call MyBase.New(allsubgraph.Length)

            input = candidateList.ToDictionary(Function(a) a.unique_id)
            background = allsubgraph
            result = New ActivityEnrichment(background.Length - 1) {}
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            For i As Integer = start To ends
                result(i) = ActivityEnrichment.Evaluate(
                    input:=input,
                    background:=background(i),
                    modelSize:=modelSize,
                    pinList:=pinList,
                    ignoreTopology:=ignoreTopology
                )
            Next
        End Sub
    End Class

    ''' <summary>
    ''' Run network graph module enrichment
    ''' </summary>
    ''' <param name="candidateList"></param>
    ''' <param name="allsubgraph"></param>
    ''' <param name="pinList"></param>
    ''' <param name="modelSize"></param>
    ''' <param name="ignoreTopology"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 
    ''' </remarks>
    <Extension>
    Friend Function PeakListAnnotation(candidateList As MzQuery(), allsubgraph As NamedValue(Of NetworkGraph)(), pinList As Index(Of String),
                                       Optional modelSize As Integer = -1,
                                       Optional ignoreTopology As Boolean = False,
                                       Optional parallel As Boolean = True) As ActivityEnrichment()

        Dim task As New PeakListAnnotationTask(candidateList, allsubgraph) With {
            .ignoreTopology = ignoreTopology,
            .modelSize = modelSize,
            .pinList = pinList
        }

        If parallel Then
            Call task.Run()
        Else
            Call task.Solve()
        End If

        Dim scores = From query As ActivityEnrichment
                     In task.result
                     Where query.Background > 0 AndAlso
                         query.Input > 0 AndAlso
                         query.Activity > 0 AndAlso
                         Not query.Q.IsNaNImaginary
                     Select query
                     Order By query.Activity Descending

        Return scores.ToArray
    End Function

    ''' <summary>
    ''' Do ms1 peak list annotation based on the given biological context information
    ''' </summary>
    ''' <param name="candidates"></param>
    ''' <param name="background"></param>
    ''' <param name="minhit"></param>
    ''' <param name="permutation"></param>
    ''' <param name="modelSize"></param>
    ''' <param name="pinned"></param>
    ''' <param name="ignoreTopology"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function is based on the Monte-Carlo method for run candidate search
    ''' </remarks>
    <Extension>
    Public Function PeakListAnnotation(candidates As IEnumerable(Of MzSet),
                                       background As IEnumerable(Of NamedValue(Of NetworkGraph)),
                                       Optional minhit As Integer = 3,
                                       Optional permutation As Integer = 100,
                                       Optional modelSize As Integer = -1,
                                       Optional pinned As String() = Nothing,
                                       Optional ignoreTopology As Boolean = False,
                                       Optional mutation_rate As Double = 0.3) As ActivityEnrichment()

        Dim allsubgraph As NamedValue(Of NetworkGraph)() = background.ToArray

        If modelSize <= 0 Then
            modelSize = allsubgraph _
                .Select(Function(g) g.Value.vertex) _
                .IteratesALL _
                .Select(Function(v) v.label) _
                .Distinct _
                .Count
        End If

        Dim monteCarlo As New MonteCarlo(allsubgraph, modelSize, pinned, ignoreTopology)
        Dim result As ActivityEnrichment() = monteCarlo.Solve(candidates, permutation, mutation_rate)

        Return result
    End Function

    <Extension>
    Public Function Score(tmp1 As ActivityEnrichment(), Optional ignoreTopology As Boolean = False) As Double
        If ignoreTopology Then
            Return Aggregate v As ActivityEnrichment
                   In tmp1
                   Let pscore As Double = If(
                       v.Fisher.two_tail_pvalue < 1.0E-100,
                       100,
                       -std.Log10(v.Fisher.two_tail_pvalue)
                   )
                   Into Sum(pscore * v.Input)
        Else
            Return Aggregate v As ActivityEnrichment
                   In tmp1
                   Into Sum(v.Activity * v.Input)
        End If
    End Function

    ''' <summary>
    ''' Populate a sequence of the annotation search candidate set
    ''' </summary>
    ''' <param name="MsDb"></param>
    ''' <param name="peaks"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetCandidateSet(MsDb As IMzQuery, peaks As IEnumerable(Of Double)) As IEnumerable(Of MzSet)
        Return From mzi As Double
               In peaks
               Let candidates = MsDb.QueryByMz(mzi).ToArray
               Where candidates.Count > 0
               Select New MzSet With {
                   .mz = mzi,
                   .query = candidates
               }
    End Function

End Module
