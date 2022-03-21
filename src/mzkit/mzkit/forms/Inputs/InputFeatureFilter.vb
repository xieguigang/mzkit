#Region "Microsoft.VisualBasic::e4b647aeaa001b9730d8bb6bd8dd8d94, mzkit\src\mzkit\mzkit\forms\Inputs\InputFeatureFilter.vb"

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

    '   Total Lines: 39
    '    Code Lines: 27
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 1.18 KB


    ' Class InputFeatureFilter
    ' 
    '     Function: GetTypes
    ' 
    '     Sub: AddTypes, Button1_Click, Button2_Click, Label3_Click, txtPPM_TextChanged
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class InputFeatureFilter

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Public Sub AddTypes(types As Dictionary(Of String, Boolean))
        CheckedListBox1.Items.Clear()

        For Each item In types
            Call CheckedListBox1.Items.Add(item.Key)
            Call CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, item.Value)
        Next
    End Sub

    Public Function GetTypes() As Index(Of String)
        Dim types As New List(Of String)

        For Each item In CheckedListBox1.CheckedItems
            types.Add(item.ToString)
        Next

        Return types.Indexing
    End Function

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub txtPPM_TextChanged(sender As Object, e As EventArgs) Handles txtPPM.TextChanged

    End Sub
End Class
