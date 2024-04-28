#Region "Microsoft.VisualBasic::8f70fc70b0eea16af875d3229663c398, G:/mzkit/src/assembly/SpectrumTree//Tree/ReferenceBinaryTree.vb"

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

    '   Total Lines: 46
    '    Code Lines: 30
    ' Comment Lines: 10
    '   Blank Lines: 6
    '     File Size: 1.63 KB


    '     Class ReferenceBinaryTree
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Push
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports std = System.Math

Namespace Tree

    ''' <summary>
    ''' the spectrum tree library data structure that organized in binary tree format
    ''' </summary>
    Public Class ReferenceBinaryTree : Inherits ReferenceTree

        ''' <summary>
        ''' construct a reference tree library that save reference data in binary tree cluster structure
        ''' </summary>
        ''' <param name="file"></param>
        Public Sub New(file As Stream)
            MyBase.New(file, nbranchs:=2)
        End Sub

        Protected Overrides Sub Push(centroid() As ms2, node As BlockNode, raw As PeakMs2)
            Dim score = GlobalAlignment.TwoDirectionSSM(centroid, node.centroid, da)
            Dim min = std.Min(score.forward, score.reverse)
            Dim i As Integer = BlockNode.GetBinaryIndex(min)

            If i = 0 Then
                ' add to current cluster members
                node.Members.Add(tree.Append(raw, centroid, isMember:=True, spectrum))
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
                node.childs(i) = tree.Append(raw, centroid, isMember:=False, spectrum)
            End If
        End Sub
    End Class
End Namespace
