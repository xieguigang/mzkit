Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports stdNum = System.Math

''' <summary>
''' the spectrum tree library data structure that organized in binary tree format
''' </summary>
Public Class ReferenceBinaryTree : Inherits ReferenceTree

    Public Sub New(file As Stream)
        MyBase.New(file, nbranchs:=2)
    End Sub

    Protected Overrides Sub Push(centroid() As ms2, node As BlockNode, raw As PeakMs2)
        Dim score = GlobalAlignment.TwoDirectionSSM(centroid, node.centroid, da)
        Dim min = stdNum.Min(score.forward, score.reverse)
        Dim i As Integer = BlockNode.GetBinaryIndex(min)

        If i = 0 Then
            ' add to current cluster members
            node.Members.Add(Append(raw, centroid, isMember:=True))
            Return
        Else
            If i = -1 Then
                i = 0
            Else
                i = 1
            End If
        End If

        If node.childs(i) > 0 Then
            ' align to next node
            Push(centroid, tree(node.childs(i)), raw)
        Else
            ' create new node
            node.childs(i) = Append(raw, centroid, isMember:=False)
        End If
    End Sub
End Class
