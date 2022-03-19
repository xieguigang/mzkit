#Region "Microsoft.VisualBasic::28be65c7c246229a44a778ca814cde45, mzkit\src\mzkit\mzkit\forms\Inputs\InputRSymbol.vb"

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

    '   Total Lines: 37
    '    Code Lines: 30
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.35 KB


    ' Class InputRSymbol
    ' 
    '     Function: GetNames
    ' 
    '     Sub: Button1_Click, Button2_Click, InputRSymbol_Load, LoadFields
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.My

Public Class InputRSymbol

    Private Sub InputRSymbol_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each symbol As String In MyApplication.REngine.globalEnvir.GetSymbolsNames
            Call ComboBox1.Items.Add(symbol)
        Next
    End Sub

    Public Iterator Function GetNames() As IEnumerable(Of String)
        For Each i As Integer In CheckedListBox1.CheckedIndices
            Yield CheckedListBox1.Items(i).ToString
        Next
    End Function

    Public Sub LoadFields(names As IEnumerable(Of String))
        Call CheckedListBox1.Items.Clear()

        For Each name As String In names
            Call CheckedListBox1.Items.Add(name)
            Call CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, True)
        Next
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Trim(ComboBox1.Text).StringEmpty Then
            MessageBox.Show("A symbol name is required!", "Create Symbol", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            DialogResult = DialogResult.OK
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub
End Class
