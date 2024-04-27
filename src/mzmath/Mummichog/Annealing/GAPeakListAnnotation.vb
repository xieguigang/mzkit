#Region "Microsoft.VisualBasic::1d6903880dc6fd69b673ba941fc059f7, G:/mzkit/src/mzmath/Mummichog//Annealing/GAPeakListAnnotation.vb"

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

    '   Total Lines: 54
    '    Code Lines: 35
    ' Comment Lines: 13
    '   Blank Lines: 6
    '     File Size: 2.60 KB


    ' Module GAPeakListAnnotation
    ' 
    '     Function: PeakListAnnotation
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Helper
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Population.SubstitutionStrategy

Public Module GAPeakListAnnotation

    ''' <summary>
    ''' Do ms1 peak list annotation based on the given biological context information, Run annotation via the genetic algorithm
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
