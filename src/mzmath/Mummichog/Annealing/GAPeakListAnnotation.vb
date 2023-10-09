Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Helper
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Population.SubstitutionStrategy

Public Module GAPeakListAnnotation

    ''' <summary>
    ''' Run annotation via the genetic algorithm
    ''' </summary>
    ''' <param name="candidates"></param>
    ''' <param name="background"></param>
    ''' <param name="minhit"></param>
    ''' <param name="popsize"></param>
    ''' <param name="permutation"></param>
    ''' <param name="modelSize"></param>
    ''' <param name="pinned"></param>
    ''' <param name="ignoreTopology"></param>
    ''' <param name="mutation_rate"></param>
    ''' <returns></returns>
    Public Function PeakListAnnotation(candidates As IEnumerable(Of MzSet),
                                       background As IEnumerable(Of NamedValue(Of NetworkGraph)),
                                       Optional minhit As Integer = 3,
                                       Optional popsize As Integer = 100,
                                       Optional permutation As Integer = 1000,
                                       Optional modelSize As Integer = -1,
                                       Optional pinned As String() = Nothing,
                                       Optional ignoreTopology As Boolean = False,
                                       Optional mutation_rate As Double = 0.3) As ActivityEnrichment()

        Dim context As New ContextFitness(background, modelSize, pinned, ignoreTopology)
        Dim pop0 As New AnnotationSet With {
            .IonSet = candidates.ToArray,
            .i = Replicate(0, .IonSet.Length).ToArray,
            .MutationRate = mutation_rate
        }
        Dim pops = pop0.InitialPopulation(popSize:=popsize)
        Dim ga As New GeneticAlgorithm(Of AnnotationSet)(pops, context, Strategies.EliteCrossbreed)
        Dim snapshot As Action(Of AnnotationSet, Double) =
            Sub()

            End Sub
        Dim driver As New EnvironmentDriver(Of AnnotationSet)(ga, snapshot, iterations:=permutation)
        Dim best As AnnotationSet
        Dim result As ActivityEnrichment()

        driver.Train()
        best = driver.BestModel
        result = context.Enrich(best)

        Return result
    End Function
End Module
