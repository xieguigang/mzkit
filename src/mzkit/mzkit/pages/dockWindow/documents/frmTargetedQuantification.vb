#Region "Microsoft.VisualBasic::f6e50c5ca8e25ec192141f4521d1c52e, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmTargetedQuantification.vb"

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

    '   Total Lines: 1145
    '    Code Lines: 901
    ' Comment Lines: 30
    '   Blank Lines: 214
    '     File Size: 48.07 KB


    ' Class frmTargetedQuantification
    ' 
    '     Function: createGCMSLinears, createLinear, createMRMLinears, GetContentTable, GetGCMSFeatureReader
    '               GetGCMSFeatures, GetScans, GetStandardReference, GetTableLevelKeys, isValidLinearRow
    '               linearProfileNames, LoadGCMSIonLibrary, unifyGetStandards
    ' 
    '     Sub: applyNewParameters, DataGridView1_CellDoubleClick, DataGridView1_CellEndEdit, DataGridView1_DragDrop, DataGridView1_DragEnter
    '          DataGridView1_DragOver, DataGridView1_KeyDown, DeleteIonFeatureToolStripMenuItem_Click, deleteProfiles, doLoadSampleFiles
    '          ExportImageToolStripMenuItem_Click, ExportLinearTableToolStripMenuItem_Click, ExportTableToolStripMenuItem_Click, frmTargetedQuantification_Closed, frmTargetedQuantification_FormClosing
    '          frmTargetedQuantification_Load, ImportsLinearReferenceToolStripMenuItem_Click, loadGCMSReference, loadLinearRaw, loadLinears
    '          loadMRMReference, loadReferenceData, loadSampleFiles, LoadSamplesToolStripMenuItem_Click, reload
    '          reloadProfileNames, runLinearFileImports, SaveAsToolStripMenuItem_Click, SaveDocument, saveLinearPack
    '          saveLinearsTable, SetGCMSKeys, SetMRMKeys, showIonPeaksTable, showLinear
    '          showQuanifyTable, showRawXTable, ToolStripComboBox2_SelectedIndexChanged, unifyLoadLinears, ViewLinearReportToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.QuantifyAnalysis
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports BioNovoGene.mzkit_win32.My
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports SMRUCC.Rsharp.Runtime.Components
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports any = Microsoft.VisualBasic.Scripting
Imports Rlist = SMRUCC.Rsharp.Runtime.Internal.Object.list
Imports stdNum = System.Math

Public Class frmTargetedQuantification

    ReadOnly args As New QuantifyParameters

    Private Sub frmTargetedQuantification_Load(sender As Object, e As EventArgs) Handles Me.Load
        WindowModules.ribbon.TargetedContex.ContextAvailable = ContextAvailability.Active

        AddHandler WindowModules.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw
        AddHandler WindowModules.ribbon.SaveLinears.ExecuteEvent, AddressOf saveLinearsTable

        TabText = "Targeted Quantification"

        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False

        Call reloadProfileNames()
        Call ApplyVsTheme(ToolStrip1, ToolStrip2, ContextMenuStrip1, ContextMenuStrip2, ContextMenuStrip3)

        Call VisualStudio.Dock(WindowModules.parametersTool, DockState.DockRight)
        Call WindowModules.parametersTool.SetParameterObject(args, AddressOf applyNewParameters)
    End Sub

    ''' <summary>
    ''' 调整参数后重新计算标准曲线
    ''' </summary>
    ''' <param name="args"></param>
    Private Sub applyNewParameters(args As QuantifyParameters)
        If rowIndex >= 0 Then
            ' 这个可能是因为之前的一批标准曲线计算留下来的
            If DataGridView1.Rows.Count <= rowIndex Then
                Return
            End If

            showLinear(args)
        End If
    End Sub

    Private Sub reloadProfileNames()
        cbProfileNameSelector.Items.Clear()

        For Each key As String In linearProfileNames()
            cbProfileNameSelector.Items.Add(key)
        Next
    End Sub

    Private Sub frmTargetedQuantification_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler WindowModules.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw
        RemoveHandler WindowModules.ribbon.SaveLinears.ExecuteEvent, AddressOf saveLinearsTable
    End Sub

    Sub loadLinearRaw(sender As Object, e As ExecuteEventArgs)
        Call ImportsLinearReferenceToolStripMenuItem_Click(Nothing, Nothing)
    End Sub

    Dim linearPack As LinearPack
    Dim linearFiles As NamedValue(Of String)()
    Dim allFeatures As String()
    Dim isGCMS As Boolean = False

    Sub saveLinearsTable(sender As Object, e As ExecuteEventArgs)
        If linearPack Is Nothing OrElse linearPack.linears.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("No linears for save!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else

        End If
    End Sub

    Private Sub ImportsLinearReferenceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsLinearReferenceToolStripMenuItem.Click
        Using importsFile As New OpenFileDialog With {
            .Filter = "LC-MSMS / GC-MS Targeted(*.mzML)|*.mzML|GC-MS Targeted(*.cdf)|*.cdf",
            .Multiselect = True,
            .Title = "Select linears"
        }

            If importsFile.ShowDialog = DialogResult.OK Then
                Call runLinearFileImports(importsFile.FileNames)
            End If
        End Using
    End Sub

    Private Sub runLinearFileImports(fileNames As String())
        Dim files As NamedValue(Of String)() = ContentTable.StripMaxCommonNames(fileNames)
        Dim fakeLevels As Dictionary(Of String, Double)
        Dim directMapName As Boolean = False

        If files.All(Function(name) name.Value.BaseName.IsContentPattern) Then
            files = files _
                .Select(Function(file)
                            Return New NamedValue(Of String) With {
                                .Name = file.Value.BaseName,
                                .Value = file.Value,
                                .Description = file.Description
                            }
                        End Function) _
                .ToArray
            fakeLevels = files _
                .ToDictionary(Function(file) file.Value.BaseName,
                              Function(file)
                                  Return file.Value _
                                      .BaseName _
                                      .ParseContent _
                                      .ScaleTo(ContentUnits.ppb) _
                                      .Value
                              End Function)
            directMapName = True
        Else
            fakeLevels = files _
                .ToDictionary(Function(file) file.Name,
                              Function()
                                  Return 0.0
                              End Function)
        End If

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        For Each file As NamedValue(Of String) In files
            Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = file.Name})

            If file.Value.ExtensionSuffix("CDF") OrElse RawScanParser.IsSIMData(file.Value) Then
                isGCMS = True
                Call MyApplication.host.ShowGCMSSIM(file.Value, isBackground:=False, showExplorer:=False)
            Else
                isGCMS = False
                Call MyApplication.host.ShowMRMIons(file.Value)
            End If
        Next

        Me.linearFiles = files
        Me.linearPack = New LinearPack With {
            .reference = New Dictionary(Of String, SampleContentLevels) From {
                {"n/a", New SampleContentLevels(fakeLevels, directMapName)}
            }
        }

        If isGCMS Then
            Call loadGCMSReference(files, directMapName)
        Else
            Call loadMRMReference(files, directMapName)
        End If
    End Sub

    Private Function LoadGCMSIonLibrary() As QuantifyIon()
        Dim filePath = Globals.Settings.QuantifyIonLibfile

        If filePath.FileLength > 0 Then
            Try
                Using file As Stream = filePath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Return MsgPackSerializer.Deserialize(Of QuantifyIon())(file)
                End Using
            Catch ex As Exception
                Call App.LogException(ex)
                Call MyApplication.host.showStatusMessage("Error while load GCMS reference: " & ex.Message, My.Resources.StatusAnnotations_Warning_32xLG_color)

                Return {}
            End Try
        Else
            Return {}
        End If
    End Function

    Private Sub loadGCMSReference(files As NamedValue(Of String)(), directMapName As Boolean)
        Dim ions As QuantifyIon() = LoadGCMSIonLibrary()
        Dim extract As SIMIonExtract = GetGCMSFeatureReader(ions)
        Dim allFeatures = files _
            .Select(Function(file) GetGCMSFeatures(file, extract)) _
            .IteratesALL _
            .GroupBy(Function(p) p.rt, Function(x, y) stdNum.Abs(x - y) <= 15) _
            .ToArray
        Dim contentLevels = linearPack.reference("n/a")

        Me.allFeatures = allFeatures.Select(Function(p) $"{p.value.First.time.Min}/{p.value.First.time.Max}").ToArray

        For Each group As NamedCollection(Of ROI) In allFeatures
            Dim ion As QuantifyIon = extract.FindIon(group.First)
            Dim i As Integer = DataGridView1.Rows.Add(ion.name)
            Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            comboxBox.Items.Add("")

            For Each IS_candidate In allFeatures
                comboxBox.Items.Add(extract.FindIon(IS_candidate.First).name)
            Next

            If directMapName Then
                Dim row As DataGridViewRow = DataGridView1.Rows(i)

                For index As Integer = 2 To DataGridView1.Columns.Count - 1
                    row.Cells(index).Value = contentLevels.Content(DataGridView1.Columns(index).HeaderText)
                Next
            End If
        Next
    End Sub

    Private Function GetGCMSFeatures(file As String, extract As SIMIonExtract) As IEnumerable(Of ROI)
        Dim gcms As GCMS.Raw

        If file.ExtensionSuffix("cdf") Then
            gcms = netCDFReader.Open(file).ReadData(showSummary:=False)
        Else
            gcms = mzMLReader.LoadFile(file)
        End If

        Return extract.GetAllFeatures(gcms)
    End Function

    Private Sub loadMRMReference(files As NamedValue(Of String)(), directMapName As Boolean)
        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim allFeatures As IonPair() = files _
            .Select(Function(file) file.Value) _
            .GetAllFeatures
        Dim contentLevels As SampleContentLevels = linearPack.reference("n/a")

        Me.allFeatures = allFeatures.Select(AddressOf ionsLib.GetDisplay).ToArray

        For Each ion As IonPair In allFeatures
            Dim refId As String = ionsLib.GetDisplay(ion)
            Dim i As Integer = DataGridView1.Rows.Add(refId)
            Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            comboxBox.Items.Add("")

            For Each IS_candidate As IonPair In allFeatures
                comboxBox.Items.Add(ionsLib.GetDisplay(IS_candidate))
            Next

            If directMapName Then
                Dim row As DataGridViewRow = DataGridView1.Rows(i)

                For index As Integer = 2 To DataGridView1.Columns.Count - 1
                    row.Cells(index).Value = contentLevels.Content(DataGridView1.Columns(index).HeaderText)
                Next
            End If
        Next
    End Sub

    Private Function isValidLinearRow(r As DataGridViewRow) As Boolean
        Dim allKeys = linearPack.GetLevelKeys

        For i As Integer = 2 To allKeys.Length - 1 + 2
            If (Not any.ToString(r.Cells(i).Value).IsNumeric) OrElse (any.ToString(r.Cells(i).Value) = "0") Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Sub DeleteIonFeatureToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteIonFeatureToolStripMenuItem.Click

    End Sub

    Private Iterator Function GetTableLevelKeys() As IEnumerable(Of String)
        For i As Integer = 2 To DataGridView1.Columns.Count - 1
            Yield DataGridView1.Columns(i).HeaderText
        Next
    End Function

    Private Iterator Function unifyGetStandards() As IEnumerable(Of Standards)
        Dim levelKeys As String() = GetTableLevelKeys.ToArray
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons As Dictionary(Of String, QuantifyIon) = LoadGCMSIonLibrary.ToDictionary(Function(i) i.name)
        Dim ref As New Value(Of Standards)

        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not ref = GetStandardReference(row, GCMSIons, ionLib, levelKeys) Is Nothing Then
                Yield CType(ref, Standards)
            End If
        Next
    End Function

    Private Function GetStandardReference(row As DataGridViewRow, GCMSIons As Dictionary(Of String, QuantifyIon), ionLib As IonLibrary, levelKeys As String()) As Standards
        Dim rid As String = any.ToString(row.Cells(0).Value)
        Dim IS_id As String = any.ToString(row.Cells(1).Value)
        Dim levels As New Dictionary(Of String, Double)

        If rid.StringEmpty AndAlso IS_id.StringEmpty Then
            Return Nothing
        End If

        If isGCMS Then
            Dim ion As QuantifyIon = GCMSIons.GetIon(rid)

            If Not ion Is Nothing Then
                rid = $"{ion.rt.Min}/{ion.rt.Max}"
            End If

            ion = GCMSIons.GetIon(IS_id)

            If Not ion Is Nothing Then
                IS_id = $"{ion.rt.Min}/{ion.rt.Max}"
            End If

        Else
            Dim ion As IonPair = ionLib.GetIonByKey(rid)

            If Not ion Is Nothing Then
                rid = $"{ion.precursor}/{ion.product}"
            ElseIf rid.IsPattern("Ion \[.+?\]") Then
                rid = rid.GetStackValue("[", "]")
            End If

            ion = ionLib.GetIonByKey(IS_id)

            If Not ion Is Nothing Then
                IS_id = $"{ion.precursor}/{ion.product}"
            ElseIf IS_id.IsPattern("Ion \[.+?\]") Then
                IS_id = IS_id.GetStackValue("[", "]")
            End If
        End If

        For i As Integer = 2 To DataGridView1.Columns.Count - 1
            levels(levelKeys(i - 2)) = any.ToString(row.Cells(i).Value).ParseDouble
        Next

        If levels.Values.All(Function(x) x = 0.0) Then
            Return Nothing
        Else
            Return New Standards() With {
                .ID = rid,
                .Name = rid,
                .[IS] = IS_id,
                .ISTD = IS_id,
                .Factor = 1,
                .C = levels
            }
        End If
    End Function

    Protected Overrides Sub SaveDocument() Handles SaveToolStripMenuItem.Click, ToolStripButton1.Click
        ' Dim ref As New List(Of Standards)(getStandards)
        Dim profileName As String = cbProfileNameSelector.Text

        If profileName.StringEmpty Then
            Call MessageBox.Show("Empty profile name!", "Targeted Quantification Linear", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim file As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.linearPack"

        Call frmTaskProgress.RunAction(
            Sub()
                Call Me.Invoke(Sub() Call saveLinearPack(profileName, file))
                Call Me.Invoke(Sub() Call reloadProfileNames())
            End Sub, "Save Linear Reference Models", "...")

        Call MyApplication.host.showStatusMessage($"linear model profile '{profileName}' saved!")
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        Using savefile As New SaveFileDialog With {.Title = "Select location for save linear pack data.", .Filter = "Mzkit Linear Models(*.linearPack)|*.linearPack"}
            If savefile.ShowDialog = DialogResult.OK Then
                Call frmTaskProgress.RunAction(
                    Sub()
                        Call Me.Invoke(Sub() saveLinearPack(savefile.FileName.BaseName, savefile.FileName))
                    End Sub, "Save Linear Reference Models", "...")
            End If
        End Using
    End Sub

    Private Function linearProfileNames() As String()
        Return (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/") _
            .ListFiles("*.linearPack") _
            .Select(AddressOf BaseName) _
            .ToArray
    End Function

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub

    Private Sub unifyLoadLinears()
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim quantifyIons As SIMIonExtract = GetGCMSFeatureReader(LoadGCMSIonLibrary)

        isGCMS = linearPack.targetted = TargettedData.SIM

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        Dim levelKeys As String() = linearPack.GetLevelKeys

        For Each level As String In levelKeys
            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = level})
        Next

        Dim islist As String() = linearPack.IS _
            .Select(Function(i)
                        Dim ionpairtext = i.ID.Split("/"c).Select(AddressOf Val).ToArray
                        Dim name As String

                        If isGCMS Then
                            name = quantifyIons.FindIon(ionpairtext.Min, ionpairtext.Max).name
                        Else
                            name = New IonPair With {
                                .precursor = ionpairtext(0),
                                .product = ionpairtext(1)
                            } _
                            .DoCall(AddressOf ionLib.GetDisplay)
                        End If

                        Return name
                    End Function) _
            .ToArray

        allFeatures = islist

        For Each linear As KeyValuePair(Of String, SampleContentLevels) In linearPack.reference
            Call loadReferenceData(quantifyIons, islist, levelKeys, ionLib, linear)
        Next
    End Sub

    Private Sub loadReferenceData(quantifyIons As SIMIonExtract, islist As String(), levelKeys As String(), ionLib As IonLibrary, linear As KeyValuePair(Of String, SampleContentLevels))
        Dim ionpairtext = linear.Key.Split("/"c).Select(AddressOf Val).ToArray
        Dim ionID As String
        Dim [is] As [IS] = linearPack.GetLinear(linear.Key)?.IS

        If [is] Is Nothing Then
            [is] = New [IS]
        End If

        If isGCMS Then
            ionID = quantifyIons.FindIon(ionpairtext.Min, ionpairtext.Max).name
        Else
            ionID = New IonPair With {
                .precursor = ionpairtext(0),
                .product = ionpairtext(1)
            } _
            .DoCall(AddressOf ionLib.GetDisplay)
        End If

        If Not [is].ID.StringEmpty Then
            ionpairtext = [is].ID.Split("/"c).Select(AddressOf Val).ToArray

            If isGCMS Then
                [is].name = quantifyIons.FindIon(ionpairtext.Min, ionpairtext.Max).name
            Else
                [is].name = New IonPair With {
                    .precursor = ionpairtext(0),
                    .product = ionpairtext(1)
                } _
                .DoCall(AddressOf ionLib.GetDisplay)
            End If
        End If

        Dim i As Integer = DataGridView1.Rows.Add(ionID)
        Dim IScandidate As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

        IScandidate.Items.Add("")

        For Each id As String In islist
            IScandidate.Items.Add(id)
        Next

        IScandidate.Value = [is].name

        For j As Integer = 0 To levelKeys.Length - 1
            DataGridView1.Rows(i).Cells(j + 2).Value = CStr(linear.Value(levelKeys(j)))
        Next
    End Sub

    Dim linearEdit As Boolean = False

    Private Sub loadLinears(sender As Object, e As EventArgs) Handles cbProfileNameSelector.SelectedIndexChanged
        If linearEdit AndAlso MessageBox.Show("Current linear profiles has been edited, do you want continute to load new linear profiles data?", "Linear Profile Unsaved", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
            Return
        End If

        If cbProfileNameSelector.SelectedIndex = -1 Then
            Return
        End If

        Dim profileName As String = any.ToString(cbProfileNameSelector.Items(cbProfileNameSelector.SelectedIndex))
        Dim file As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.linearPack"

        linearPack = LinearPack.OpenFile(file)

        Call unifyLoadLinears()
    End Sub

    Private Sub reload(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Call reloadProfileNames()
        Call loadLinears(Nothing, Nothing)
    End Sub

    Private Function GetContentTable(row As DataGridViewRow) As ContentTable
        Dim ionId As String = any.ToString(row.Cells(0).Value)
        Dim isId As String = any.ToString(row.Cells(1).Value)
        Dim contentLevel As New Dictionary(Of String, Double)

        For Each id As SeqValue(Of String) In GetTableLevelKeys().SeqIterator
            contentLevel(id.value) = any _
                .ToString(row.Cells(id + 2).Value) _
                .ParseDouble
        Next

        Dim directMap As Boolean

        If Me.linearFiles Is Nothing Then
            directMap = Me.linearPack _
                .GetLevelKeys _
                .All(Function(name)
                         Return name.BaseName.IsContentPattern
                     End Function)
        Else
            directMap = Me.linearFiles _
                .All(Function(name)
                         Return name.Value.BaseName.IsContentPattern
                     End Function)
        End If

        Dim contentSampleLevel As New SampleContentLevels(contentLevel, directMap)
        Dim ref As New Standards With {
            .C = New Dictionary(Of String, Double),
            .ID = ionId,
            .[IS] = isId,
            .ISTD = isId,
            .Name = ionId
        }
        Dim levels As New Dictionary(Of String, SampleContentLevels) From {{ionId, contentSampleLevel}}
        Dim refs As New Dictionary(Of String, Standards) From {{ionId, ref}}
        Dim ISlist As New Dictionary(Of String, [IS]) From {{isId, New [IS] With {.ID = isId, .name = isId, .CIS = 5}}}

        Return New ContentTable(levels, refs, ISlist)
    End Function

    Dim standardCurve As StandardCurve
    Dim rowIndex As Integer = -1

    ''' <summary>
    ''' 鼠标点击参考线性表格重新计算线性方程
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.ColumnIndex <> 0 OrElse e.RowIndex < 0 Then
            Return
        Else
            rowIndex = e.RowIndex
            showLinear(args)
        End If
    End Sub

    Private Sub showLinear(args As QuantifyParameters)
        ' 计算出线性方程
        standardCurve = createLinear(DataGridView1.Rows(rowIndex), args)

        If standardCurve Is Nothing Then
            Return
        End If

        ' 进行线性方程的可视化
        PictureBox1.BackgroundImage = standardCurve _
            .StandardCurves(
                size:="1920,1200",
                name:=$"Linear of {standardCurve.name}",
                margin:="padding: 100px 100px 200px 200px;",
                gridFill:="white"
            ) _
            .AsGDIImage

        Call DataGridView2.Rows.Clear()

        ' 显示出线性方程的线性拟合建模表格
        For Each point As ReferencePoint In standardCurve.points
            Call DataGridView2.Rows.Add(point.ID, point.Name, point.AIS, point.Ati, point.cIS, point.Cti, point.Px, point.yfit, point.error, point.variant, point.valid, point.level)
        Next
    End Sub

    ''' <summary>
    ''' unify save linear pack data
    ''' </summary>
    ''' <param name="title"></param>
    ''' <param name="file"></param>
    Private Sub saveLinearPack(title As String, file As String)
        Dim ref As Standards() = unifyGetStandards.ToArray
        Dim linears As New List(Of StandardCurve)
        Dim points As TargetPeakPoint() = Nothing
        Dim refPoints As New List(Of TargetPeakPoint)
        Dim refLevels As New Dictionary(Of String, SampleContentLevels)
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons As Dictionary(Of String, QuantifyIon) = LoadGCMSIonLibrary.ToDictionary(Function(i) i.name)
        Dim directMap As Boolean = ref(Scan0).C.Keys.All(Function(name) name.IsContentPattern)

        For Each i As Standards In ref
            refLevels(i.ID) = New SampleContentLevels(i.C, directMap:=directMap)
        Next

        For Each row As DataGridViewRow In DataGridView1.Rows
            If isValidLinearRow(row) Then
                Dim line = createLinear(row, args, points)

                If Not line Is Nothing Then
                    linears.Add(line)
                    refPoints.AddRange(points)
                End If
            End If
        Next

        refPoints = refPoints _
            .GroupBy(Function(p) $"{p.SampleName}\{p.Name}") _
            .Select(Function(pg) pg.First) _
            .AsList

        If isGCMS Then
            Call SetGCMSKeys(refPoints, linears, GCMSIons)
        Else
            Call SetMRMKeys(refPoints, linears, ionLib)
        End If

        Dim linearPack As New LinearPack With {
            .linears = linears.ToArray,
            .peakSamples = refPoints.ToArray,
            .time = Now,
            .title = title,
            .reference = refLevels,
            .[IS] = allFeatures _
                .Select(Function(name)
                            If isGCMS Then
                                ' do nothing
                            Else
                                Dim nameIon As IonPair = ionLib.GetIonByKey(name)
                                name = $"{nameIon.precursor}/{nameIon.product}"
                            End If

                            Return New [IS] With {
                                .ID = name,
                                .name = name,
                                .CIS = 5
                            }
                        End Function) _
                .ToArray,
            .targetted = If(isGCMS, TargettedData.SIM, TargettedData.MRM)
        }

        Call linearPack.Write(file)
    End Sub

    Private Sub SetGCMSKeys(refPoints As List(Of TargetPeakPoint), linears As List(Of StandardCurve), GCMSIons As Dictionary(Of String, QuantifyIon))
        Dim ion As QuantifyIon

        For Each point As TargetPeakPoint In refPoints
            ion = GCMSIons.GetIon(point.Name)

            If Not ion Is Nothing Then
                point.Name = $"{ion.rt.Min}/{ion.rt.Max}"
            End If
        Next

        For Each line As StandardCurve In linears
            ion = GCMSIons.GetIon(line.name)

            If Not ion Is Nothing Then
                line.name = $"{ion.rt.Min}/{ion.rt.Max}"
            End If

            If Not line.IS Is Nothing AndAlso Not line.IS.ID.StringEmpty Then
                ion = GCMSIons.GetIon(line.IS.ID)

                If Not ion Is Nothing Then
                    line.IS.ID = $"{ion.rt.Min}/{ion.rt.Max}"
                End If

                line.IS.name = line.IS.ID
            End If
        Next
    End Sub

    Private Sub SetMRMKeys(refPoints As List(Of TargetPeakPoint), linears As List(Of StandardCurve), ionLib As IonLibrary)
        Dim ion As IonPair

        For Each point As TargetPeakPoint In refPoints
            ion = ionLib.GetIonByKey(point.Name)
            point.Name = $"{ion.precursor}/{ion.product}"
        Next

        For Each line As StandardCurve In linears
            ion = ionLib.GetIonByKey(line.name)
            line.name = $"{ion.precursor}/{ion.product}"

            If Not line.IS Is Nothing AndAlso Not line.IS.ID.StringEmpty Then
                ion = ionLib.GetIonByKey(line.IS.ID)
                line.IS.ID = $"{ion.precursor}/{ion.product}"
                line.IS.name = line.IS.ID
            End If
        Next
    End Sub

    Private Function GetGCMSFeatureReader(ionLib As IEnumerable(Of QuantifyIon)) As SIMIonExtract
        Return New SIMIonExtract(ionLib, {5, 15}, Tolerance.DeltaMass(0.3), 20, 0.65)
    End Function

    ''' <summary>
    ''' unify create linear reference
    ''' </summary>
    ''' <param name="refRow"></param>
    ''' <param name="refPoints"></param>
    ''' <returns></returns>
    Private Function createLinear(refRow As DataGridViewRow, args As QuantifyParameters, Optional ByRef refPoints As TargetPeakPoint() = Nothing) As StandardCurve
        Dim id As String = any.ToString(refRow.Cells(0).Value)
        Dim isid As String = any.ToString(refRow.Cells(1).Value)
        Dim chr As New List(Of TargetPeakPoint)

        If isGCMS Then
            chr.AddRange(createGCMSLinears(id, isid))
        Else
            chr.AddRange(createMRMLinears(id, isid))
        End If

        Dim algorithm As New InternalStandardMethod(GetContentTable(refRow), PeakAreaMethods.NetPeakSum)

        refPoints = chr.ToArray

        If chr = 0 OrElse chr.All(Function(p) p.Name <> id) Then
            Call MyApplication.host.showStatusMessage($"No sample data was found of ion '{id}'!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return Nothing
        Else
            Return algorithm.ToLinears(chr).FirstOrDefault
        End If
    End Function

    Private Function createGCMSLinears(id As String, isid As String) As IEnumerable(Of TargetPeakPoint)
        Dim ionLib = LoadGCMSIonLibrary.ToDictionary(Function(a) a.name)
        Dim quantifyIon = ionLib.GetIon(id)
        Dim quantifyIS = ionLib.GetIon(isid)
        Dim SIMIonExtract = GetGCMSFeatureReader(ionLib.Values)
        Dim chr As New List(Of TargetPeakPoint)

        If linearFiles.IsNullOrEmpty Then
            Call linearPack.peakSamples _
                .Select(Function(p)
                            Dim t = p.Name.Split("/"c).Select(AddressOf Val).ToArray

                            If stdNum.Abs(t(0) - quantifyIon.rt.Min) <= 10 AndAlso stdNum.Abs(t(1) - quantifyIon.rt.Max) <= 10 Then
                                Return New TargetPeakPoint With {
                                    .Name = quantifyIon.name,
                                    .ChromatogramSummary = p.ChromatogramSummary,
                                    .Peak = p.Peak,
                                    .SampleName = p.SampleName
                                }
                            ElseIf stdNum.Abs(t(0) - quantifyIS.rt.Min) <= 10 AndAlso stdNum.Abs(t(1) - quantifyIS.rt.Max) <= 10 Then
                                Return New TargetPeakPoint With {
                                    .Name = quantifyIS.name,
                                    .ChromatogramSummary = p.ChromatogramSummary,
                                    .Peak = p.Peak,
                                    .SampleName = p.SampleName
                                }
                            Else
                                Return Nothing
                            End If
                        End Function) _
                .Where(Function(p) Not p Is Nothing) _
                .DoCall(AddressOf chr.AddRange)
        Else
            Call SIMIonExtract.LoadSamples(linearFiles, quantifyIon, keyByName:=True).DoCall(AddressOf chr.AddRange)

            If Not isid.StringEmpty Then
                Call SIMIonExtract.LoadSamples(linearFiles, quantifyIS, keyByName:=True).DoCall(AddressOf chr.AddRange)
            End If
        End If

        Return chr
    End Function

    Private Function createMRMLinears(id As String, isid As String) As IEnumerable(Of TargetPeakPoint)
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim quantifyIon = ionLib.GetIonByKey(id)
        Dim quantifyIS = ionLib.GetIonByKey(isid)
        Dim dadot3 As Tolerance = args.GetTolerance
        Dim chr As New List(Of TargetPeakPoint)

        If linearFiles.IsNullOrEmpty Then
            ' load from model files
            Call linearPack.peakSamples _
                .Select(Function(p)
                            Dim t = p.Name.Split("/"c).Select(AddressOf Val).ToArray

                            If dadot3(t(0), quantifyIon.precursor) AndAlso dadot3(t(1), quantifyIon.product) Then
                                Return New TargetPeakPoint With {
                                    .Name = quantifyIon.name,
                                    .ChromatogramSummary = p.ChromatogramSummary,
                                    .Peak = p.Peak,
                                    .SampleName = p.SampleName
                                }
                            ElseIf dadot3(t(0), quantifyIS.precursor) AndAlso dadot3(t(1), quantifyIS.product) Then
                                Return New TargetPeakPoint With {
                                    .Name = quantifyIS.name,
                                    .ChromatogramSummary = p.ChromatogramSummary,
                                    .Peak = p.Peak,
                                    .SampleName = p.SampleName
                                }
                            Else
                                Return Nothing
                            End If
                        End Function) _
                .Where(Function(p) Not p Is Nothing) _
                .DoCall(AddressOf chr.AddRange)
        Else
            Dim arguments As MRMArguments = args.GetMRMArguments

            ' load from raw data files
            Call MRMIonExtract.LoadSamples(linearFiles, quantifyIon, arguments).DoCall(AddressOf chr.AddRange)

            If Not isid.StringEmpty Then
                Call MRMIonExtract.LoadSamples(linearFiles, quantifyIS, arguments).DoCall(AddressOf chr.AddRange)
            End If
        End If

        Return chr
    End Function

    Private Sub ExportImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportImageToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export Standard Curve Image",
            .Filter = "Plot Image(*.png)|*.png"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    Private Sub ExportLinearTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportLinearTableToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export Reference Points",
            .Filter = "Reference Point Table(*.csv)|*.csv"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call standardCurve.points.SaveTo(file.FileName)
            End If
        End Using
    End Sub

    Dim scans As New List(Of QuantifyScan)

    Private Sub LoadSamplesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadSamplesToolStripMenuItem.Click
        Using importsFile As New OpenFileDialog With {
            .Filter = "LC-MSMS / GC-MS Targeted(*.mzML)|*.mzML|GC-MS Targeted(*.cdf)|*.cdf",
            .Multiselect = True,
            .Title = "Select linears"
        }
            If importsFile.ShowDialog = DialogResult.OK Then
                Call doLoadSampleFiles(importsFile.FileNames)
            End If
        End Using
    End Sub

    Private Sub doLoadSampleFiles(FileNames As String())
        Dim files As NamedValue(Of String)() = FileNames _
            .Select(Function(file)
                        Return New NamedValue(Of String) With {
                            .Name = file.BaseName,
                            .Value = file
                        }
                    End Function) _
            .ToArray

        ' add files to viewer
        For Each file As NamedValue(Of String) In files
            Call MyApplication.host.showStatusMessage($"open raw data file '{file.Value}'...")
            Call MyApplication.host.OpenFile(file.Value, showDocument:=linearPack Is Nothing)
            Call Application.DoEvents()
        Next

        ' and then do quantify if the linear is exists
        If Not linearPack Is Nothing Then
            Call loadSampleFiles(files)
        Else
            Call MyApplication.host.showStatusMessage("no linear model for run quantification, just open raw files viewer...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If

        ToolStripComboBox2.SelectedIndex = 1

        Call showQuanifyTable()
    End Sub

    Private Sub loadSampleFiles(files As NamedValue(Of String)())
        Dim points As New List(Of TargetPeakPoint)
        Dim linears As New List(Of StandardCurve)
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons = LoadGCMSIonLibrary.ToDictionary(Function(a) a.name)
        Dim extract = GetGCMSFeatureReader(GCMSIons.Values)
        Dim massError As MRMArguments = args.GetMRMArguments

        Call scans.Clear()

        For Each refRow As DataGridViewRow In DataGridView1.Rows
            If isValidLinearRow(refRow) Then
                Dim id As String = any.ToString(refRow.Cells(0).Value)
                Dim isid As String = any.ToString(refRow.Cells(1).Value)

                linears.Add(createLinear(refRow, args))

                If isGCMS Then
                    Dim ion As QuantifyIon = GCMSIons.GetIon(id)
                    Dim ISion As QuantifyIon = GCMSIons.GetIon(isid)

                    points.AddRange(extract.LoadSamples(files, ion, keyByName:=True))
                    points.AddRange(extract.LoadSamples(files, ion, keyByName:=True))
                Else
                    Dim ion As IonPair = ionLib.GetIonByKey(id)
                    Dim ISion As IonPair = ionLib.GetIonByKey(isid)

                    points.AddRange(MRMIonExtract.LoadSamples(files, ion, massError))

                    If Not ISion Is Nothing Then
                        points.AddRange(MRMIonExtract.LoadSamples(files, ISion, massError))
                    End If
                End If
            End If
        Next

        With linears.Where(Function(l) Not l Is Nothing).ToArray
            For Each file In points.GroupBy(Function(p) p.SampleName)
                Dim uniqueIons = file.GroupBy(Function(p) p.Name).Select(Function(p) p.First).ToArray
                Dim quantify As QuantifyScan = .SampleQuantify(uniqueIons, PeakAreaMethods.SumAll, fileName:=file.Key)

                If Not quantify Is Nothing Then
                    scans.Add(quantify)
                End If
            Next
        End With
    End Sub

    Private Sub showQuanifyTable()
        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify = scans.Select(Function(q) q.quantify).ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub showRawXTable()
        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify = scans.Select(Function(q) q.rawX).ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub showIonPeaksTable()
        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify As EntityObject() = scans.Select(Function(q) q.ionPeaks).IteratesALL.DataFrame.ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Me.linearEdit = True
    End Sub

    Private Sub deleteProfiles(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim profileName As String = cbProfileNameSelector.Text

        If MessageBox.Show($"Going to delete current linear profile '{cbProfileNameSelector.Text}'?", "Delete current profiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return
        Else
            Call (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.linearPack").DeleteFile
        End If

        linearEdit = False
        linearFiles = Nothing
        linearPack = Nothing

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        Call reloadProfileNames()
    End Sub

    Private Sub ToolStripComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox2.SelectedIndexChanged
        Select Case ToolStripComboBox2.SelectedIndex
            Case 0 : Call showIonPeaksTable()
            Case 1 : Call showQuanifyTable()
            Case 2 : Call showRawXTable()

        End Select
    End Sub

    Private Sub ExportTableToolStripMenuItem_Click() Handles ExportTableToolStripMenuItem.Click, ToolStripButton4.Click
        Call DataGridView3.SaveDataGrid("Export sample result table [%s] success!")
    End Sub

    Private Sub DataGridView1_DragDrop(sender As Object, e As DragEventArgs) Handles DataGridView1.DragDrop
        Dim path As String = CType(e.Data.GetData(DataFormats.FileDrop), String())(Scan0)

        If Not path.ExtensionSuffix("linearpack") Then
            MessageBox.Show($"[{path}] is not a mzkit linear model file...", "Not a linearPack file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Else
            cbProfileNameSelector.Text = path.BaseName
            linearPack = LinearPack.OpenFile(path)

            Call unifyLoadLinears()
        End If
    End Sub

    Private Sub DataGridView1_DragEnter(sender As Object, e As DragEventArgs) Handles DataGridView1.DragEnter
        e.Effect = DragDropEffects.Copy
    End Sub

    Private Sub DataGridView1_DragOver(sender As Object, e As DragEventArgs) Handles DataGridView1.DragOver
        e.Effect = DragDropEffects.Copy
    End Sub

    Private Iterator Function GetScans() As IEnumerable(Of QuantifyScan)

    End Function

    Private Sub ViewLinearReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewLinearReportToolStripMenuItem.Click
        If linearPack Is Nothing OrElse linearPack.linears.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("no linear model was loaded!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".html", sessionID:=App.PID.ToHexString, "linear_report")
        Dim samples As QuantifyScan() = {}
        Dim ionsRaw As New Rlist With {
            .slots = linearPack.peakSamples _
                .GroupBy(Function(sample) sample.Name) _
                .ToDictionary(Function(ion) ion.Key,
                              Function(ionGroup)
                                  Dim innerList As New Rlist With {
                                      .slots = ionGroup _
                                          .ToDictionary(Function(ion) ion.SampleName,
                                                        Function(ion)
                                                            Return CObj(ion.Peak.ticks)
                                                        End Function)
                                  }

                                  Return CObj(innerList)
                              End Function)
        }

        For Each line In linearPack.linears
            line.linear.ErrorTest = line.points _
                .Select(Function(p)
                            Return CType(New TestPoint With {.X = p.Px, .Y = p.Cti, .Yfit = p.yfit}, IFitError)
                        End Function) _
                .ToArray
        Next

        Call MyApplication.REngine.LoadLibrary("mzkit")
        Call MyApplication.REngine.Evaluate("imports 'Linears' from 'mzkit.quantify';")
        Call MyApplication.REngine.Set("$temp_report", MyApplication.REngine.Invoke("report.dataset", linearPack.linears, samples, Nothing, ionsRaw))
        Call MyApplication.REngine.Invoke("html", MyApplication.REngine("$temp_report"), MyApplication.REngine.globalEnvir).ToString.SaveTo(tempfile)

        If TypeOf MyApplication.REngine.globalEnvir.last Is Message Then
            Call MessageBox.Show(MyApplication.REngine.globalEnvir.last.ToString, "View Linear Report", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Call VisualStudio.ShowDocument(Of frmHtmlViewer)().LoadHtml(tempfile)
        End If
    End Sub

    Private Sub frmTargetedQuantification_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        WindowModules.ribbon.TargetedContex.ContextAvailable = ContextAvailability.NotAvailable
    End Sub
End Class
