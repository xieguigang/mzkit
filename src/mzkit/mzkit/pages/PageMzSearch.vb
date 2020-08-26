#Region "Microsoft.VisualBasic::d934041670f9828af641220a18c6edf7, src\mzkit\mzkit\pages\PageMzSearch.vb"

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

' Class PageMzSearch
' 
'     Function: FormulaFinderTest4
' 
'     Sub: doMzSearch, PageMzSearch_Load, runSearchInternal, ShowFormulaFinderResults
' 
' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.BioDeep
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports RibbonLib.Interop
Imports stdNum = System.Math

Public Class PageMzSearch

    Dim host As frmMain

    Public Sub doMzSearch(mz As Double, ppm As Double)
        Dim progress As New frmTaskProgress

        Call New Thread(Sub() Call runSearchInternal(mz, ppm, progress)).Start()
        Call progress.ShowDialog()
    End Sub

    Private Sub runSearchInternal(mz As Double, ppm As Double, progress As frmTaskProgress)
        Thread.Sleep(100)
        progress.Invoke(Sub() progress.Label2.Text = "initialize workspace...")

        Dim opts = Chemoinformatics.Formula.SearchOption.DefaultMetaboliteProfile
        Dim oMwtWin As New FormulaSearch(
            opts:=opts,
            progress:=Sub(msg) progress.Invoke(Sub() progress.Label1.Text = msg)
        )

        progress.Invoke(Sub() progress.Label2.Text = "running formula search...")

        Dim searchResults = oMwtWin.SearchByExactMass(mz).ToArray

        progress.Invoke(Sub() progress.Label2.Text = "output search result...")
        host.Invoke(Sub() host.ToolStripStatusLabel1.Text = $"Run formula search for m/z {mz} with tolerance error {ppm} ppm, have {searchResults.Length} formula found!")

        Call ShowFormulaFinderResults(searchResults)

        Call progress.Invoke(Sub() Call progress.Close())
    End Sub

    Private Sub ShowFormulaFinderResults(lstResults As IEnumerable(Of FormulaComposition))
        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        ' Add coluns to the table
        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Formula"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Exact Mass"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "PPM"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "Charge"})
        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, .ValueType = GetType(String), .HeaderText = "m/z"})

        For Each result As FormulaComposition In lstResults
            DataGridView1.Rows.Add(result.EmpiricalFormula, result.exact_mass, result.ppm, result.charge, stdNum.Abs(result.exact_mass / result.charge))
        Next
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = Scan0 AndAlso e.RowIndex >= 0 Then
            Dim formula As String = DataGridView1.Rows(e.RowIndex).Cells(0).Value?.ToString

            If Not formula.StringEmpty Then
                Call Process.Start($"https://query.biodeep.cn/search?expression=[formula]&category=metabolite&formula={formula}")
            End If
        End If
    End Sub

    Public Sub SaveSearchResultTable()
        Call DataGridView1.SaveDataGrid
    End Sub

    Private Sub PageMzSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        host = DirectCast(ParentForm, frmMain)
    End Sub

    Private Sub PageMzSearch_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Visible Then
            host.ribbonItems.TabGroupFormulaSearchTools.ContextAvailable = ContextAvailability.Active
        Else
            host.ribbonItems.TabGroupFormulaSearchTools.ContextAvailable = ContextAvailability.NotAvailable
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim mz As Double = Val(TextBox1.Text)
        Dim ppm As Double = 30

        Call doMzSearch(mz, ppm)
    End Sub
End Class

