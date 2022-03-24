#Region "Microsoft.VisualBasic::87eb8bc135369d03c1440626b2bf48fd, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmSMILESViewer.vb"

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

'   Total Lines: 51
'    Code Lines: 39
' Comment Lines: 0
'   Blank Lines: 12
'     File Size: 1.82 KB


' Class frmSMILESViewer
' 
'     Sub: Button1_Click, Button2_Click, Canvas1_Load, frmSMILESViewer_Load, Label1_Click
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Drawing
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce
Imports Microsoft.VisualBasic.Imaging

Public Class frmSMILESViewer

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim smilesStr As String = Strings.Trim(TextBox1.Text)
        Dim graph As ChemicalFormula = ParseChain.ParseGraph(smilesStr)
        Dim network As New NetworkGraph

        For Each v In graph.vertex
            network.CreateNode(v.label, New NodeData With {.label = v.elementName, .color = Brushes.Black})
        Next
        For Each l In graph.graphEdges
            Dim url = network.CreateEdge(
                  u:=l.U.label,
                  v:=l.V.label,
                  weight:=l.bond
              )

            url.data.style = New Pen(Color.Red, 2)
            network.AddEdge(url)
        Next

        Canvas1.Graph() = network
        Canvas1.ShowLabel = True
    End Sub

    Private Sub frmSMILESViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Canvas1.SetFDGParams(New ForceDirectedArgs With {.Repulsion = 10000.0!})
    End Sub

    Private Sub Canvas1_Load(sender As Object, e As EventArgs) Handles Canvas1.Load

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim kcf As String = TextBox2.Text
        Dim model As KCF

        If kcf.LineTokens.Length = 1 Then
            model = ParseChain.ParseGraph(Strings.Trim(TextBox1.Text)).ToKCF
        Else
            model = IO.LoadKCF(kcf)
        End If

        Dim visual As Image = model.Draw().AsGDIImage

        PictureBox1.BackgroundImage = visual
    End Sub
End Class
