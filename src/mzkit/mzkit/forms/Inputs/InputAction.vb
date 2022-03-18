#Region "Microsoft.VisualBasic::302039d888b35e7c4ce39fcc2b9b48dc, mzkit\src\mzkit\mzkit\forms\Inputs\InputAction.vb"

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
    '    Code Lines: 33
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.28 KB


    ' Class InputAction
    ' 
    '     Properties: getActionName, getTargetName
    ' 
    '     Sub: Button1_Click, Button2_Click, SetFields
    ' 
    ' /********************************************************************************/

#End Region

Public Class InputAction

    Public Sub SetFields(names As IEnumerable(Of String))
        For Each name As String In names
            Call ComboBox1.Items.Add(name)
        Next

        For Each name As String In Actions.allActions
            Call ComboBox2.Items.Add(name)
        Next
    End Sub

    Public ReadOnly Property getTargetName As String
        Get
            Return ComboBox1.Text
        End Get
    End Property
    Public ReadOnly Property getActionName As String
        Get
            Return ComboBox2.Text
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedIndex = -1 Then
            MessageBox.Show("no data target is selected!", "Run Action", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        ElseIf ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("no data action is selected!", "Run Action", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub
End Class
