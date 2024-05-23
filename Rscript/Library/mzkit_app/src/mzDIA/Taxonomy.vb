#Region "Microsoft.VisualBasic::9f43e6b414601d3c9a667c974ba81202, Rscript\Library\mzkit_app\src\mzDIA\Taxonomy.vb"

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

    '   Total Lines: 157
    '    Code Lines: 108 (68.79%)
    ' Comment Lines: 26 (16.56%)
    '    - Xml Docs: 92.31%
    ' 
    '   Blank Lines: 23 (14.65%)
    '     File Size: 6.01 KB


    ' Module Taxonomy
    ' 
    '     Function: clusters, parallelTree, tree, vocabulary
    '     Class ParallelTreeTask
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Solve
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
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
        Dim task As New ParallelTreeTask(pool, mzdiff, intocutoff, equals, interval)
        Dim trees As TreeCluster() = DirectCast(task.Run, ParallelTreeTask).out
        ' make union
        Dim args As New ClusterTree.Argument With {
            .diff = interval,
            .threshold = 0.95,
            .alignment = NetworkingTree.CreateAlignment(mzdiff, intocutoff, equals)
        }
        Dim union As ClusterTree = TreeCluster.Union(trees, args)

        Return union
    End Function

    Private Class ParallelTreeTask : Inherits VectorTask

        ReadOnly pool As KeyValuePair(Of String, PeakMs2())(),
            mzdiff As Double,
            intocutoff As Double,
            equals_score As Double,
            interval As Double

        Public out As TreeCluster()

        Public Sub New(pool As Dictionary(Of String, PeakMs2()),
                       mzdiff As Double,
                       intocutoff As Double,
                       equals As Double,
                       interval As Double)

            Call MyBase.New(pool.Count, verbose:=True)

            Me.pool = pool.ToArray
            Me.mzdiff = mzdiff
            Me.intocutoff = intocutoff
            Me.equals_score = equals
            Me.interval = interval
            Me.out = Allocate(Of TreeCluster)(all:=False)
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            Dim da As Tolerance = Tolerance.DeltaMass(mzdiff)
            Dim cutoff As New RelativeIntensityCutoff(intocutoff)
            Dim tree As TreeCluster
            Dim poolData As New List(Of PeakMs2)

            For i As Integer = start To ends
                Dim data As PeakMs2() = pool(i).Value

                data = data _
                    .SafeQuery _
                    .Select(Function(spec)
                                spec.mzInto = spec.mzInto.Centroid(da, cutoff).ToArray
                                Return spec
                            End Function) _
                    .ToArray
                poolData.AddRange(data)
            Next

            tree = New NetworkingTree(mzdiff, intocutoff, equals_score, interval).Tree(poolData)

            SyncLock out
                out(cpu_id) = tree
            End SyncLock
        End Sub
    End Class

    ''' <summary>
    ''' get spectrum clusters
    ''' </summary>
    ''' <param name="tree"></param>
    ''' <returns></returns>
    <ExportAPI("clusters")>
    Public Function clusters(tree As Object, Optional env As Environment = Nothing) As Object
        Dim pull As New Dictionary(Of String, String())

        If tree Is Nothing Then
            Return Nothing
        End If

        If TypeOf tree Is ClusterTree Then
            Call TreeCluster.GetTree(tree, pull)
        ElseIf TypeOf tree Is SpectrumVocabulary Then
            pull = DirectCast(tree, SpectrumVocabulary).GetClusters
        Else
            Return Message.InCompatibleType(GetType(ClusterTree), tree.GetType, env)
        End If

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
