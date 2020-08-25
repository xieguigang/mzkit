#Region "Microsoft.VisualBasic::7306d27116a194ea7049b68a1f5a2682, src\mzkit\mzkit\pages\PageMzCalculator.vb"

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

    ' Class PageMzCalculator
    ' 
    '     Sub: Button1_Click, Button2_Click, ExportToolStripMenuItem_Click, PageMzCalculator_VisibleChanged, Update
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports RibbonLib.Interop

Public Class PageMzCalculator

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim exact_mass As Double

        If TextBox1.Text.StringEmpty Then
            Return
        ElseIf TextBox1.Text.IsNumeric Then
            exact_mass = Val(TextBox1.Text)
        Else
            exact_mass = Task.Math.EvaluateFormula(TextBox1.Text)
        End If

        Call Update(exact_mass, Provider.GetCalculator("+").Values, DataGridView1)
        Call Update(exact_mass, Provider.GetCalculator("-").Values, DataGridView2)
    End Sub

    Overloads Sub Update(exact_mass As Double, mode As IEnumerable(Of MzCalculator), show As DataGridView)
        Dim mzText As String

        Call show.Rows.Clear()

        For Each type As MzCalculator In mode
            mzText = type.CalcMZ(exact_mass)
            mzText = If(Val(mzText) < 0, "n/a", mzText)

            Call show.Rows.Add(type.ToString, type.adducts, type.M, type.charge, mzText)
        Next
    End Sub

    Public Sub ExportToolStripMenuItem_Click()
        Using file As New SaveFileDialog With {.Filter = "Excel Table|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Dim currentTab As DataGridView = (From ctrl As Control In TabControl1.SelectedTab.Controls Where TypeOf ctrl Is DataGridView).First
                Dim table As New List(Of PrecursorInfo)

                For Each row As DataGridViewRow In currentTab.Rows
                    If row.Cells(0).Value Is Nothing Then
                        Exit For
                    End If

                    Call New PrecursorInfo With {
                        .precursor_type = row.Cells(0).Value.ToString,
                        .adduct = CDbl(row.Cells(1).Value.ToString),
                        .M = CInt(row.Cells(2).Value.ToString),
                        .charge = CDbl(row.Cells(3).Value),
                        .mz = row.Cells(4).Value.ToString
                    }.DoCall(AddressOf table.Add)
                Next

                Call table.SaveTo(file.FileName)
                Call MessageBox.Show($"Export table to [{file.FileName}] success!", "Table Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim exact_mass As Double

        If TextBox1.Text.StringEmpty Then
            Return
        ElseIf TextBox1.Text.IsNumeric Then
            exact_mass = Val(TextBox1.Text)
        Else
            exact_mass = Task.Math.EvaluateFormula(TextBox1.Text)
        End If

        Process.Start($"https://query.biodeep.cn/search?expression=[mass]~0.3&category=metabolite&mass={exact_mass}")
    End Sub

    Private Sub PageMzCalculator_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        Dim host As frmMain = ParentForm

        If Me.Visible Then
            host.ribbonItems.TabGroupCalculatorTools.ContextAvailable = ContextAvailability.Active
            host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable
        Else
            host.ribbonItems.TabGroupCalculatorTools.ContextAvailable = ContextAvailability.NotAvailable
            host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
        End If
    End Sub
End Class

