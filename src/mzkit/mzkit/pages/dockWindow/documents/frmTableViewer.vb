#Region "Microsoft.VisualBasic::09e529c89b620cda995388ea15f96330, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmTableViewer.vb"

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

    '   Total Lines: 274
    '    Code Lines: 198
    ' Comment Lines: 8
    '   Blank Lines: 68
    '     File Size: 10.44 KB


    ' Class frmTableViewer
    ' 
    '     Properties: FilePath, MimeType, ViewRow
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: getCurrentTable, (+2 Overloads) getFieldVector, GetSchema, (+2 Overloads) Save
    ' 
    '     Sub: ActionsToolStripMenuItem_Click, AdvancedDataGridView1_FilterStringChanged, AdvancedDataGridViewSearchToolBar1_Search, columnVectorStat, exportTableCDF
    '          frmTableViewer_Activated, frmTableViewer_FormClosed, frmTableViewer_FormClosing, frmTableViewer_Load, LoadTable
    '          resetFilter, SaveDocument, SendToREnvironmentToolStripMenuItem_Click, ViewToolStripMenuItem_Click, VisualizeToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports RibbonLib.Interop
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports Zuby.ADGV
Imports REnv = SMRUCC.Rsharp.Runtime

Public Class frmTableViewer : Implements ISaveHandle, IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath
    Public Property ViewRow As Action(Of Dictionary(Of String, Object))

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "Excel Data Table", .FileExt = ".xls", .MIMEType = "application/xls", .Name = "Excel Data Table"}
            }
        End Get
    End Property

    Dim memoryData As New DataSet

    Public Sub LoadTable(apply As Action(Of DataTable))
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Call Me.AdvancedDataGridView1.Columns.Clear()
        Call Me.AdvancedDataGridView1.Rows.Clear()
        Call apply(table)
        Call AdvancedDataGridView1.SetDoubleBuffered()

        For Each column As DataGridViewColumn In AdvancedDataGridView1.Columns
            'Select Case table.Columns.Item(column.HeaderText).DataType
            '    Case GetType(String)
            '        AdvancedDataGridView1.SetSortEnabled(column, True)
            '    Case GetType(Double)
            '    Case GetType(Integer)
            '    Case Else
            '        ' do nothing 
            'End Select

            AdvancedDataGridView1.ShowMenuStrip(column)
        Next

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName

        AdvancedDataGridView1.DataSource = BindingSource1
        AdvancedDataGridViewSearchToolBar1.SetColumns(AdvancedDataGridView1.Columns)
    End Sub

    Protected Overrides Sub SaveDocument()
        Call AdvancedDataGridView1.SaveDataGrid("Save Table View")
    End Sub

    Private Sub frmTableViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False

        TabText = "Table View"

        ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Using writeTsv As StreamWriter = path.OpenWriter(encoding:=Encodings.UTF8WithoutBOM)
            Call AdvancedDataGridView1.WriteTableToFile(writeTsv)
        End Using

        Return True
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        If AdvancedDataGridView1.SelectedRows.Count <= 0 Then
            Call MyApplication.host.showStatusMessage("Please select a row data for view content!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf Not ViewRow Is Nothing Then
            Dim obj As New Dictionary(Of String, Object)
            Dim row As DataGridViewRow = AdvancedDataGridView1.SelectedRows(0)

            For i As Integer = 0 To AdvancedDataGridView1.Columns.Count - 1
                obj(AdvancedDataGridView1.Columns(i).HeaderText) = row.Cells(i).Value
            Next

            Call _ViewRow(obj)
        End If
    End Sub

    Private Sub SendToREnvironmentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SendToREnvironmentToolStripMenuItem.Click
        Dim form As New InputRSymbol
        Dim fieldNames As New List(Of String)

        For Each col As DataGridViewColumn In AdvancedDataGridView1.Columns
            Call fieldNames.Add(col.Name)
        Next

        Call form.LoadFields(fieldNames)

        Call InputDialog.Input(Of InputRSymbol)(
            Sub(config)
                Dim name As String = config.ComboBox1.Text.Trim
                Dim fields As String() = config.GetNames.ToArray
                Dim table As New dataframe With {
                    .columns = New Dictionary(Of String, Array)
                }

                For Each fieldRef As String In fields
                    Dim i As Integer = fieldNames.IndexOf(fieldRef)
                    Dim array As Array = getFieldVector(i)

                    Call table.add(fieldRef, array)
                Next

                Call MyApplication.REngine.Add(name, table)
                Call VisualStudio.ShowRTerm()
            End Sub, config:=form)
    End Sub

    Public Function getFieldVector(fieldRef As String) As Array
        Dim fieldNames As New List(Of String)

        For Each col As DataGridViewColumn In AdvancedDataGridView1.Columns
            Call fieldNames.Add(col.Name)
        Next

        Dim i As Integer = fieldNames.IndexOf(fieldRef)
        Dim vec = getFieldVector(i)

        Return vec
    End Function

    Public Function getFieldVector(i As Integer) As Array
        Dim array As New List(Of Object)

        For Each row As DataGridViewRow In AdvancedDataGridView1.Rows
            array.Add(row.Cells(i).Value)
        Next

        Return REnv.TryCastGenericArray(array.ToArray, MyApplication.REngine.globalEnvir)
    End Function

    Public Function GetSchema() As Dictionary(Of String, Type)
        Dim schema As New Dictionary(Of String, Type)

        For Each col As DataGridViewColumn In AdvancedDataGridView1.Columns
            Call schema.Add(col.Name, GetType(Double))
        Next

        Return schema
    End Function

    Private Sub VisualizeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VisualizeToolStripMenuItem.Click
        Dim load As New InputDataVisual

        Call load.SetAxis(GetSchema)
        Call InputDialog.Input(
            Sub(creator)
                Call creator.DoPlot(getFieldVector(creator.GetX), AddressOf getFieldVector)
            End Sub, config:=load)
    End Sub

    Private Sub ActionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ActionsToolStripMenuItem.Click
        Dim takeActions As New InputAction

        Call takeActions.SetFields(GetSchema.Keys)
        Call InputDialog.Input(Sub(input)
                                   Dim name As String = input.getTargetName
                                   Dim action As String = input.getActionName
                                   Dim data As Array = getFieldVector(name)

                                   Call Actions.RunAction(action, data)
                               End Sub, config:=takeActions)
    End Sub

    Private Sub AdvancedDataGridViewSearchToolBar1_Search(sender As Object, e As AdvancedDataGridViewSearchToolBarSearchEventArgs) Handles AdvancedDataGridViewSearchToolBar1.Search
        Dim restartsearch = True
        Dim startColumn = 0
        Dim startRow = 0

        If Not e.FromBegin Then
            Dim endcol = AdvancedDataGridView1.CurrentCell.ColumnIndex + 1 >= AdvancedDataGridView1.ColumnCount
            Dim endrow = AdvancedDataGridView1.CurrentCell.RowIndex + 1 >= AdvancedDataGridView1.RowCount

            If endcol AndAlso endrow Then
                startColumn = AdvancedDataGridView1.CurrentCell.ColumnIndex
                startRow = AdvancedDataGridView1.CurrentCell.RowIndex
            Else
                startColumn = If(endcol, 0, AdvancedDataGridView1.CurrentCell.ColumnIndex + 1)
                startRow = AdvancedDataGridView1.CurrentCell.RowIndex + If(endcol, 1, 0)
            End If
        End If

        Dim c = AdvancedDataGridView1.FindCell(e.ValueToSearch, If(e.ColumnToSearch IsNot Nothing, e.ColumnToSearch.Name, Nothing), startRow, startColumn, e.WholeWord, e.CaseSensitive)

        If c Is Nothing AndAlso restartsearch Then
            c = AdvancedDataGridView1.FindCell(e.ValueToSearch, If(e.ColumnToSearch IsNot Nothing, e.ColumnToSearch.Name, Nothing), 0, 0, e.WholeWord, e.CaseSensitive)
        End If

        If c IsNot Nothing Then
            AdvancedDataGridView1.CurrentCell = c
        End If
    End Sub

    Private Sub AdvancedDataGridView1_FilterStringChanged(sender As Object, e As AdvancedDataGridView.FilterEventArgs) Handles AdvancedDataGridView1.FilterStringChanged

    End Sub

    Private Sub frmTableViewer_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

    End Sub

    Private Sub frmTableViewer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

    End Sub

    Private Sub frmTableViewer_Activated(sender As Object, e As EventArgs) Handles Me.Activated

    End Sub

    Shared Sub New()
        AddHandler ribbonItems.ButtonResetTableFilter.ExecuteEvent,
            Sub()
                Dim table = getCurrentTable()

                If Not table Is Nothing Then
                    Call table.resetFilter()
                End If
            End Sub

        AddHandler ribbonItems.ButtonColumnStats.ExecuteEvent,
            Sub()
                Dim table = getCurrentTable()

                If Not table Is Nothing Then
                    Call table.columnVectorStat()
                End If
            End Sub

        AddHandler ribbonItems.ButtonSaveTableCDF.ExecuteEvent,
            Sub()
                Dim table = getCurrentTable()

                If Not table Is Nothing Then
                    Call table.exportTableCDF()
                End If
            End Sub
    End Sub

    Private Sub exportTableCDF()

    End Sub

    Private Sub columnVectorStat()

    End Sub

    Private Sub resetFilter()
        Call AdvancedDataGridView1.CleanFilterAndSort()
    End Sub

    Private Shared Function getCurrentTable() As frmTableViewer
        If TypeOf MyApplication.host.dockPanel.ActiveDocument Is frmTableViewer Then
            Return MyApplication.host.dockPanel.ActiveDocument
        Else
            Return Nothing
        End If
    End Function
End Class
