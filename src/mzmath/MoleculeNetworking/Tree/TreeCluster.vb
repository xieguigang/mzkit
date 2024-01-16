Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.DataMining.BinaryTree

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

    Public Function GetTree() As Dictionary(Of String, String())
        Dim pull As New Dictionary(Of String, String())
        Call GetTree(tree, pull)
        Return pull
    End Function

    Public Shared Sub GetTree(tree As ClusterTree, ByRef pull As Dictionary(Of String, String()))
        Call pull.Add(tree.Data, tree.Members.ToArray)

        If tree.Childs.Any Then
            For Each subTree As Tree(Of String) In tree.Childs.Values
                Call GetTree(DirectCast(subTree, ClusterTree), pull)
            Next
        End If
    End Sub

End Class
