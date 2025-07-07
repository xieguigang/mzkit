﻿#Region "Microsoft.VisualBasic::5c09e126e2f65d09c196df47728f0c3d, mzmath\MoleculeNetworking\Tree\TreeCluster.vb"

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

    '   Total Lines: 171
    '    Code Lines: 109 (63.74%)
    ' Comment Lines: 33 (19.30%)
    '    - Xml Docs: 87.88%
    ' 
    '   Blank Lines: 29 (16.96%)
    '     File Size: 5.60 KB


    ' Delegate Function
    ' 
    ' 
    ' Class TreeCluster
    ' 
    '     Properties: clusters, spectrum, tree
    ' 
    '     Function: Any, (+2 Overloads) GetTopCluster, GetTree, ToString, Union
    ' 
    '     Sub: GetTree
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq

Public Delegate Function IRankCluster(cluster As PeakMs2()) As Double

''' <summary>
''' A tuple object that wrap the <see cref="ClusterTree"/> and
''' spectrum data <see cref="PeakMs2"/>.
''' </summary>
Public Class TreeCluster

    Public Property tree As ClusterTree

    ''' <summary>
    ''' the input spectrum data, keeps the original order with the input sequence 
    ''' </summary>
    ''' <returns></returns>
    Public Property spectrum As PeakMs2()
    Public Property clusters As String()

    ''' <summary>
    ''' does current spectrum cluster tree contains any spectrum data?
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' not empty?
    ''' </remarks>
    Public Function Any() As Boolean
        Return Not spectrum.IsNullOrEmpty
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return tree.ToString
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ranking"></param>
    ''' <param name="n">get top n clusters</param>
    ''' <returns></returns>
    Public Iterator Function GetTopCluster(ranking As IRankCluster, n As Integer) As IEnumerable(Of SeqValue(Of PeakMs2()))
        Dim tree = GetTree()

        If tree.IsNullOrEmpty Then
            Return
        End If

        Dim specIndex = spectrum.ToDictionary(Function(s) s.lib_guid)
        Dim rank_desc = tree _
            .OrderByDescending(Function(c)
                                   Return ranking(c.Value.Select(Function(a) specIndex(a)).ToArray)
                               End Function)
        Dim i As Integer = 0

        For Each top As KeyValuePair(Of String, String()) In rank_desc
            Yield New SeqValue(Of PeakMs2()) With {
                .i = i,
                .value = top.Value _
                    .Select(Function(id) specIndex(id)) _
                    .ToArray
            }

            i += 1

            If i >= n Then
                Exit For
            End If
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ranking">
    ''' the score ranking function, higher score is better
    ''' </param>
    ''' <returns>
    ''' this function may returns empty collection if the tree
    ''' data is nothing or empty tree data.
    ''' </returns>
    Public Function GetTopCluster(ranking As IRankCluster) As PeakMs2()
        Dim top = GetTopCluster(ranking, n:=1).FirstOrDefault

        If top.value.IsNullOrEmpty Then
            Return {}
        Else
            Return top.value
        End If
    End Function

    Public Function GetTree() As Dictionary(Of String, String())
        Dim pull As New Dictionary(Of String, String())

        If tree.Data Is Nothing Then
            Call "empty tree data!".Warning
        Else
            Call GetTree(tree, pull)
        End If

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

        For Each part As TreeCluster In Tqdm.Wrap(trees.ToArray, useColor:=True, wrap_console:=App.EnableTqdm)
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
