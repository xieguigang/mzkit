Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

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
    Public Function Solve(input As IEnumerable(Of MzSet), permutations As Integer, Optional mutation_rate As Double = 0.3) As ActivityEnrichment()
        Dim candidates As New AnnotationSet With {
            .IonSet = input.ToArray,
            .i = VectorExtensions.Replicate(0, .IonSet.Length).ToArray,
            .MutationRate = mutation_rate
        }
        Dim t0 As Double = 1

        Me.best_score = Double.MinValue

        For i As Integer = 0 To permutations
            candidates = eval(candidates)
            t0 *= 0.9
            candidates.MutationRate *= t0
        Next

        Dim result As ActivityEnrichment() = background.Enrich(candidates)
        Return result
    End Function

End Class
