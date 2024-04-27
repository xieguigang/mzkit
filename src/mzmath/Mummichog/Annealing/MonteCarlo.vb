#Region "Microsoft.VisualBasic::a9dd6db53d78792d93905c0db12e48af, G:/mzkit/src/mzmath/Mummichog//Annealing/MonteCarlo.vb"

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

    '   Total Lines: 73
    '    Code Lines: 47
    ' Comment Lines: 12
    '   Blank Lines: 14
    '     File Size: 2.62 KB


    ' Class MonteCarlo
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: eval, Solve
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.ApplicationServices

''' <summary>
''' Do candidate set annotation search via Monte-Carlo method
''' </summary>
Public Class MonteCarlo

    Dim background As ContextFitness
    Dim best_score As Double = Double.MinValue

    Sub New(background As IEnumerable(Of NamedValue(Of NetworkGraph)),
            Optional modelSize As Integer = -1,
            Optional pinned As String() = Nothing,
            Optional ignoreTopology As Boolean = False)

        Me.background = New ContextFitness(background, modelSize, pinned, ignoreTopology)
    End Sub

    Private Function eval(candidates As AnnotationSet) As AnnotationSet
        Dim clone As AnnotationSet = candidates.Mutate
        Dim score As Double = 1 / background.Calculate(clone, parallel:=True)
        Dim uniq As Double = clone.UniqueHitSize

        clone.Score = score
        score *= uniq

        If score > best_score Then
            best_score = score
            ' use the new candidate set
            Return clone
        Else
            ' use the old candidate set
            Return candidates
        End If
    End Function

    ''' <summary>
    ''' Solve the annotation problem
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="permutations"></param>
    ''' <param name="mutation_rate"></param>
    ''' <returns></returns>
    Public Function Solve(input As IEnumerable(Of MzSet),
                          Optional permutations As Integer = 1000,
                          Optional mutation_rate As Double = 0.3) As ActivityEnrichment()

        Dim candidates As New AnnotationSet With {
            .IonSet = input.ToArray,
            .i = VectorExtensions.Replicate(0, .IonSet.Length).ToArray,
            .MutationRate = mutation_rate
        }
        Dim t As New PerformanceCounter
        Dim d As Integer = permutations / 50

        best_score = Double.MinValue
        t.Set()

        For i As Integer = 0 To permutations
            candidates = eval(candidates)

            If i Mod d = 0 Then
                VBDebugger.EchoLine(t.Mark($"{i}/{permutations}  {(100 * i / permutations).ToString("F1")}% | {candidates}, context_score:({candidates.Score.ToString("F3")}, {best_score.ToString("F0")})").ToString)
            End If
        Next

        Dim result As ActivityEnrichment() = background.Enrich(candidates)
        Return result
    End Function

End Class
