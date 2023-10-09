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

    Public Function Calculate(chromosome As AnnotationSet, parallel As Boolean) As Double Implements Fitness(Of AnnotationSet).Calculate

    End Function
End Class
