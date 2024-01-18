
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' tools for spectrum taxonomy
''' </summary>
''' 
<Package("taxonomy")>
Module Taxonomy

    ''' <summary>
    ''' create a spectrum taxonomy tree object
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("tree")>
    Public Function tree(Optional mzdiff As Double = 0.3,
                         Optional intocutoff As Double = 0.05,
                         Optional equals As Double = 0.85,
                         Optional interval As Double = 0.1) As NetworkingTree

        Return New NetworkingTree(mzdiff, intocutoff, equals, interval)
    End Function

    ''' <summary>
    ''' create taxonomy tree for multiple sample data in parallel
    ''' </summary>
    ''' <param name="x">a collection of the spectrum sample data, should be a tuple list 
    ''' object that contains multiple sample data to build tree. each slot value in this 
    ''' tuple list should be a vector of the <see cref="PeakMs2"/> spectrum data.</param>
    ''' <param name="mzdiff"></param>
    ''' <param name="intocutoff"></param>
    ''' <param name="equals"></param>
    ''' <param name="interval"></param>
    ''' <param name="env"></param>
    ''' <returns>a union spectrum taxonomy tree object</returns>
    <ExportAPI("parallel_tree")>
    <RApiReturn(GetType(ClusterTree))>
    Public Function parallelTree(<RRawVectorArgument> x As list,
                                 Optional mzdiff As Double = 0.3,
                                 Optional intocutoff As Double = 0.05,
                                 Optional equals As Double = 0.85,
                                 Optional interval As Double = 0.1,
                                 Optional env As Environment = Nothing) As Object

        Dim pool As Dictionary(Of String, PeakMs2()) = x.AsGeneric(Of PeakMs2())(env)
        Dim trees As TreeCluster() = pool _
            .AsParallel _
            .Select(Function(par)
                        Return New NetworkingTree(mzdiff, intocutoff, equals, interval).Tree(par.Value.SafeQuery)
                    End Function) _
            .ToArray
        ' make union
        Dim args As New ClusterTree.Argument With {
            .diff = interval,
            .threshold = 0.95,
            .alignment = NetworkingTree.CreateAlignment(mzdiff, intocutoff, equals)
        }
        Dim union As ClusterTree = TreeCluster.Union(trees, args)

        Return union
    End Function

    ''' <summary>
    ''' get spectrum clusters
    ''' </summary>
    ''' <param name="tree"></param>
    ''' <returns></returns>
    <ExportAPI("clusters")>
    Public Function clusters(tree As ClusterTree) As Object
        Dim pull As New Dictionary(Of String, String())
        Call TreeCluster.GetTree(tree, pull)

        Return New list With {
            .slots = pull _
                .ToDictionary(Function(t) t.Key,
                              Function(t)
                                  Return CObj(t.Value)
                              End Function)
        }
    End Function

    <ExportAPI("vocabulary")>
    Public Function vocabulary(tree As ClusterTree) As SpectrumVocabulary
        Return New SpectrumVocabulary(tree)
    End Function

End Module
