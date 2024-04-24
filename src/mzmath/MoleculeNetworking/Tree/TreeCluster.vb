Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' A tuple object that wrap the <see cref="ClusterTree"/> and
''' spectrum data <see cref="PeakMs2"/>.
''' </summary>
Public Class TreeCluster

    Public Property tree As ClusterTree
    Public Property spectrum As PeakMs2()
    Public Property clusters As String()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return tree.ToString
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="rank">
    ''' the score ranking function, higher score is better
    ''' </param>
    ''' <returns></returns>
    Public Function GetTopCluster(rank As Func(Of PeakMs2(), Double)) As IEnumerable(Of PeakMs2)
        Dim tree = GetTree()
        Dim specIndex = spectrum.ToDictionary(Function(s) s.lib_guid)
        Dim rank_desc = tree _
            .OrderByDescending(Function(c)
                                   Return rank(c.Value.Select(Function(a) specIndex(a)).ToArray)
                               End Function) _
            .First

        Return rank_desc _
            .Value _
            .Select(Function(id) specIndex(id))
    End Function

    Public Function GetTree() As Dictionary(Of String, String())
        Dim pull As New Dictionary(Of String, String())
        Call GetTree(tree, pull)
        Return pull
    End Function

    Public Shared Sub GetTree(tree As ClusterTree, ByRef pull As Dictionary(Of String, String()))
        If pull.ContainsKey(tree.Data) Then
            ' duplicated key may be found in spectrum
            ' taxonomy tree union operation
            pull(tree.Data) = tree.Members _
                .JoinIterates(pull.TryGetValue(tree.Data)) _
                .Distinct _
                .ToArray
        Else
            Call pull.Add(tree.Data, tree.Members.ToArray)
        End If

        If tree.Childs.Any Then
            For Each subTree As Tree(Of String) In tree.Childs.Values
                Call GetTree(DirectCast(subTree, ClusterTree), pull)
            Next
        End If
    End Sub

    Public Shared Function Union(trees As IEnumerable(Of TreeCluster), args As ClusterTree.Argument) As ClusterTree
        Dim unionTree As ClusterTree = Nothing
        Dim align As MSScoreGenerator = args.alignment
        Dim c As ClusterTree = Nothing

        For Each part As TreeCluster In Tqdm.Wrap(trees.ToArray, useColor:=True)
            If part Is Nothing Then
                Continue For
            End If
            For Each spec As PeakMs2 In part.spectrum.SafeQuery
                Call align.Add(spec)
            Next

            If unionTree Is Nothing Then
                unionTree = part.tree
                Continue For
            End If

            Dim clusters As Dictionary(Of String, String()) = part.GetTree
            Dim subs As String()

            If Not clusters.IsNullOrEmpty Then
                For Each cluster_id As String In clusters.Keys
                    Call ClusterTree.Add(unionTree, args.SetTargetKey(cluster_id), find:=c)

                    subs = clusters(cluster_id)

                    If subs Is Nothing Then
                        subs = {}
                    End If

                    If c.Members.IsNullOrEmpty Then
                        c.Members = New List(Of String)(subs)
                    Else
                        c.Members.AddRange(subs)
                    End If
                Next
            End If
        Next

        Return unionTree
    End Function

End Class
