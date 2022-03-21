#Region "Microsoft.VisualBasic::08deb42de052cd7627d7667d25555d94, mzkit\src\mzkit\mzkit\pages\toolkit\PageMzCalculator.vb"

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

    '   Total Lines: 168
    '    Code Lines: 124
    ' Comment Lines: 10
    '   Blank Lines: 34
    '     File Size: 7.05 KB


    ' Class PageMzCalculator
    ' 
    '     Sub: Button1_Click, Button2_Click, DataGridView1_CellClick, DataGridView1_CellContentClick, DataGridView2_CellClick
    '          DataGridView2_CellContentClick, ExportToolStripMenuItem_Click, PageMzCalculator_VisibleChanged, RunFeatureSearch, ToggleSlider1_CheckChanged
    '          ToggleSlider1_Load, Update
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports BioNovoGene.mzkit_win32.My
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

    ''' <summary>
    ''' update m/z evaluate
    ''' </summary>
    ''' <param name="exact_mass"></param>
    ''' <param name="mode"></param>
    ''' <param name="show"></param>
    Overloads Sub Update(exact_mass As Double, mode As IEnumerable(Of MzCalculator), show As DataGridView)
        Call show.Rows.Clear()

        If ToggleSlider1.Checked Then
            ' 通过输入的mz计算出不同的exact mass
            Dim MassText As String

            show.Columns.Item(4).HeaderText = "Exact Mass"

            For Each type As MzCalculator In mode
                MassText = type.CalcMass(exact_mass)
                MassText = If(Val(MassText) < 0, "n/a", MassText)

                Call show.Rows.Add(type.ToString, type.adducts, type.M, type.charge, MassText, "Query")
            Next
        Else
            ' 通过输入的exact mass计算出不同的m/z
            Dim mzText As String

            show.Columns.Item(4).HeaderText = "Precursor m/z"

            For Each type As MzCalculator In mode
                mzText = type.CalcMZ(exact_mass)
                mzText = If(Val(mzText) < 0, "n/a", mzText)

                Call show.Rows.Add(type.ToString, type.adducts, type.M, type.charge, mzText, "Search", "Query")
            Next
        End If
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
        If TextBox1.Text.StringEmpty Then
            Return
        ElseIf TextBox1.Text.IsNumeric Then
            Call Process.Start($"http://query.biodeep.cn/search?expression=[mass]~0.3&category=metabolite&mass={TextBox1.Text}")
        Else
            Call Process.Start($"http://query.biodeep.cn/search?expression=[formula]&category=metabolite&formula={TextBox1.Text}")
        End If
    End Sub

    Private Sub PageMzCalculator_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        Dim host As frmMain = MyApplication.host

        If Me.Visible Then
            host.ribbonItems.TabGroupCalculatorTools.ContextAvailable = ContextAvailability.Active
            host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable
        Else
            host.ribbonItems.TabGroupCalculatorTools.ContextAvailable = ContextAvailability.NotAvailable
            host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.ColumnIndex = 5 AndAlso e.RowIndex > -1 Then
            RunFeatureSearch(DataGridView1, e, 1)
        End If
    End Sub

    Private Sub RunFeatureSearch(grid As DataGridView, e As DataGridViewCellEventArgs, ionMode As Integer)
        Dim row = grid.Rows(e.RowIndex)
        Dim mz As Double = Val(row.Cells(4).Value)

        Call FeatureSearchHandler.SearchByMz(mz, WindowModules.fileExplorer.GetRawFiles(), False)
    End Sub

    Private Sub DataGridView2_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellClick
        If e.ColumnIndex = 5 AndAlso e.RowIndex > -1 Then
            RunFeatureSearch(DataGridView2, e, -1)
        ElseIf e.ColumnIndex = 6 AndAlso e.RowIndex > -1 Then

        End If
    End Sub

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub ToggleSlider1_Load(sender As Object, e As EventArgs) Handles ToggleSlider1.Load
        ToggleSlider1.Text = "Toggle M/z Search"
        ToggleSlider1.Checked = False

        DataGridView1.Columns(6).Visible = False
        DataGridView2.Columns(6).Visible = False
    End Sub

    Private Sub ToggleSlider1_CheckChanged(sender As Object, e As EventArgs) Handles ToggleSlider1.CheckChanged
        If ToggleSlider1.Checked Then
            ' 通过输入的mz计算出不同的exact mass
            ToggleSlider1.Text = "Toggle Exact Mass Search"
            Button2.Visible = False
            Button1.Text = "Exact Mass"
            Label1.Text = "Enter a m/z value:"

            DataGridView1.Columns(6).Visible = False
            DataGridView2.Columns(6).Visible = False
        Else
            ' 通过输入的exact mass计算出不同的m/z
            ToggleSlider1.Text = "Toggle M/z Search"
            Button2.Visible = True
            Button1.Text = "Evaluate M/z"
            Label1.Text = "Enter an exact mass value:"

            DataGridView1.Columns(6).Visible = True
            DataGridView2.Columns(6).Visible = True
        End If

        Call Button1_Click(Nothing, Nothing)
    End Sub
End Class
