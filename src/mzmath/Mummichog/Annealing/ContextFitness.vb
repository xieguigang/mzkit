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
