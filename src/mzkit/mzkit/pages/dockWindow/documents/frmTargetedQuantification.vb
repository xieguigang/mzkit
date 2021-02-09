Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports mzkit.My
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task
Imports any = Microsoft.VisualBasic.Scripting

Public Class frmTargetedQuantification

    Private Sub frmTargetedQuantification_Load(sender As Object, e As EventArgs) Handles Me.Load
        MyApplication.ribbon.TargetedContex.ContextAvailable = ContextAvailability.Active
        AddHandler MyApplication.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw

        TabText = "Targeted Quantification"

        Call reloadProfileNames()
        Call ApplyVsTheme(ToolStrip1, ToolStrip2, ContextMenuStrip1, ContextMenuStrip2, ContextMenuStrip3)
    End Sub

    Private Sub reloadProfileNames()
        ToolStripComboBox1.Items.Clear()

        For Each key As String In linearProfileNames()
            ToolStripComboBox1.Items.Add(key)
        Next
    End Sub

    Private Sub frmTargetedQuantification_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler MyApplication.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw
    End Sub

    Sub loadLinearRaw(sender As Object, e As ExecuteEventArgs)
        Call ImportsLinearReferenceToolStripMenuItem_Click(Nothing, Nothing)
    End Sub

    Private Shared Function StripMaxCommonNames(files As String()) As NamedValue(Of String)()
        Dim names As String() = files.Select(AddressOf BaseName).ToArray
        Dim minName As String = names.MinLengthString
        Dim index As Integer

        For i As Integer = 0 To minName.Length - 1
            index = i

            If names.Select(Function(str) str(index)).Distinct.Count > 1 Then
                names = names _
                    .Select(Function(str)
                                Return str.Substring(index).Trim(" "c, ASCII.TAB, "-"c, "_"c)
                            End Function) _
                    .ToArray

                Exit For
            End If
        Next

        If names.All(Function(str) Char.IsDigit(str.First)) Then
            names = names _
                .Select(Function(str) $"L{str}") _
                .ToArray
        End If

        Return names _
            .Select(Function(nameStr, i)
                        Return New NamedValue(Of String) With {
                            .Name = nameStr,
                            .Value = files(i)
                        }
                    End Function) _
            .OrderBy(Function(file)
                         Return file.Name.Match("\d+").ParseInteger
                     End Function) _
            .ToArray
    End Function

    Dim linearFiles As NamedValue(Of String)()

    Private Sub ImportsLinearReferenceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsLinearReferenceToolStripMenuItem.Click
        Using importsFile As New OpenFileDialog With {
            .Filter = "LC-MSMS / GC-MS Targeted(*.mzML)|*.mzML|GC-MS Targeted(*.cdf)|*.cdf",
            .Multiselect = True,
            .Title = "Select linears"
        }

            If importsFile.ShowDialog = DialogResult.OK Then
                Dim files = StripMaxCommonNames(importsFile.FileNames)

                DataGridView1.Rows.Clear()
                DataGridView1.Columns.Clear()



                DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
                DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

                For Each file As NamedValue(Of String) In files
                    Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = file.Name})
                Next

                Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
                Dim allFeatures As IonPair() = files _
                    .Select(Function(file) file.Value) _
                    .GetAllFeatures

                linearFiles = files

                For Each ion As IonPair In allFeatures
                    Dim refId As String = ionsLib.GetDisplay(ion)
                    Dim i As Integer = DataGridView1.Rows.Add(refId)
                    Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)



                    For Each IS_candidate As IonPair In allFeatures
                        comboxBox.Items.Add(ionsLib.GetDisplay(IS_candidate))
                    Next
                Next
            End If
        End Using
    End Sub

    Private Function isValidLinearRow(r As DataGridViewRow) As Boolean
        For i As Integer = 2 To linearFiles.Length - 1 + 2
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

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click, ToolStripButton1.Click
        Dim ref As New List(Of Standards)
        Dim levelKeys As String() = GetTableLevelKeys.ToArray
        Dim profileName As String = ToolStripComboBox1.Text

        If profileName.StringEmpty Then
            Call MessageBox.Show("Empty profile name!", "Targeted Quantification Linear", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim ionLib As IonLibrary = Globals.LoadIonLibrary

        For Each row As DataGridViewRow In DataGridView1.Rows
            Dim rid As String = any.ToString(row.Cells(0).Value)
            Dim IS_id As String = any.ToString(row.Cells(1).Value)
            Dim levels As New Dictionary(Of String, Double)
            Dim ion As IonPair

            If rid.StringEmpty AndAlso IS_id.StringEmpty Then
                Continue For
            End If

            ion = ionLib.GetIonByKey(rid)

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

            For i As Integer = 2 To DataGridView1.Columns.Count - 1
                levels(levelKeys(i - 2)) = any.ToString(row.Cells(i).Value).ParseDouble
            Next

            Call New Standards() With {
                .ID = rid,
                .Name = rid,
                .[IS] = IS_id,
                .ISTD = IS_id,
                .Factor = 1,
                .C = levels
            }.DoCall(AddressOf ref.Add)
        Next

        Dim file As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.csv"

        Call ref.SaveTo(file)
        Call reloadProfileNames()
    End Sub

    Private Function linearProfileNames() As String()
        Return (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/").ListFiles("*.csv").Select(AddressOf BaseName).ToArray
    End Function

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub

    Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
        Dim profileName As String = any.ToString(ToolStripComboBox1.Items(ToolStripComboBox1.SelectedIndex))
        Dim file As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.csv"
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim ref As Standards() = file.LoadCsv(Of Standards)

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        Dim levelKeys As String() = ref(Scan0).C.Keys.ToArray

        For Each level As String In levelKeys
            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = level})
        Next

        Dim islist As New List(Of String)

        For Each ion As Standards In ref
            Dim ionpairtext = ion.ID.Split("/"c)
            Dim ionpair As New IonPair With {.precursor = ionpairtext(0), .product = ionpairtext(1)}

            islist.Add(ionLib.GetDisplay(ionpair))
        Next

        For Each ion As Standards In ref
            Dim ionpairtext = ion.ID.Split("/"c)
            Dim ionpair As New IonPair With {.precursor = ionpairtext(0), .product = ionpairtext(1)}

            ion.ID = ionLib.GetDisplay(ionpair)

            If Not ion.IS.StringEmpty Then
                ionpairtext = ion.IS.Split("/"c)
                ionpair = New IonPair With {.precursor = ionpairtext(0), .product = ionpairtext(1)}
                ion.IS = ionLib.GetDisplay(ionpair)
            End If

            Dim i As Integer = DataGridView1.Rows.Add(ion.ID)
            Dim IScandidate As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            For Each id As String In islist
                IScandidate.Items.Add(id)
            Next

            IScandidate.Value = ion.IS

            For j As Integer = 0 To levelKeys.Length - 1
                DataGridView1.Rows(i).Cells(j + 2).Value = CStr(ion.C(levelKeys(j)))
            Next
        Next
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Call reloadProfileNames()
    End Sub

    Private Function GetContentTable(row As DataGridViewRow) As ContentTable
        Dim ionId As String = any.ToString(row.Cells(0).Value)
        Dim isId As String = any.ToString(row.Cells(1).Value)
        Dim contentLevel As New Dictionary(Of String, Double)

        For Each id As SeqValue(Of String) In GetTableLevelKeys().SeqIterator
            contentLevel(id.value) = any.ToString(row.Cells(id + 2).Value).ParseDouble
        Next

        Dim contentSampleLevel As New SampleContentLevels(contentLevel)
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

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.ColumnIndex <> 0 Then
            Return
        End If

        standardCurve = createLinear(DataGridView1.Rows(e.RowIndex))
        PictureBox1.BackgroundImage = standardCurve _
            .StandardCurves(
                size:="1920,1200",
                name:=$"Linear of {standardCurve.name}",
                margin:="padding: 100px 100px 200px 200px;",
                gridFill:="white"
            ) _
            .AsGDIImage

        Call DataGridView2.Rows.Clear()

        For Each point As ReferencePoint In standardCurve.points
            Call DataGridView2.Rows.Add(point.ID, point.Name, point.AIS, point.Ati, point.cIS, point.Cti, point.Px, point.yfit, point.error, point.variant, point.valid, point.level)
        Next
    End Sub

    Private Function createLinear(refRow As DataGridViewRow,
                                  Optional ByRef ion As IonPair = Nothing,
                                  Optional ByRef isIon As IonPair = Nothing) As StandardCurve

        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim id As String = any.ToString(refRow.Cells(0).Value)
        Dim isid As String = any.ToString(refRow.Cells(1).Value)
        Dim chr As New List(Of TargetPeakPoint)

        ion = ionLib.GetIonByKey(id)
        isIon = ionLib.GetIonByKey(isid)

        Dim quantifyIon = ion
        Dim quantifyIS = isIon

        Call loadSamples(linearFiles, quantifyIon).DoCall(AddressOf chr.AddRange)

        If Not isid.StringEmpty Then
            Call loadSamples(linearFiles, quantifyIS).DoCall(AddressOf chr.AddRange)
        End If

        Dim algorithm As New InternalStandardMethod(GetContentTable(refRow), PeakAreaMethods.NetPeakSum)

        Return algorithm.ToLinears(chr).First
    End Function

    Private Function loadSamples(files As IEnumerable(Of NamedValue(Of String)), quantifyIon As IonPair) As IEnumerable(Of TargetPeakPoint)
        Dim da3 As Tolerance = Tolerance.DeltaMass(0.3)

        Return files _
            .Select(Function(file)
                        Dim ionLine As chromatogram = indexedmzML.LoadFile(file).mzML.run.chromatogramList.list _
                            .Where(Function(c) quantifyIon.Assert(c, da3)) _
                            .FirstOrDefault
                        Dim peakTicks = MRMIonExtract.GetTargetPeak(quantifyIon, ionLine, preferName:=True)

                        peakTicks.SampleName = file.Name

                        Return peakTicks
                    End Function)
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
                Dim files As NamedValue(Of String)() = importsFile.FileNames _
                    .Select(Function(file)
                                Return New NamedValue(Of String) With {
                                    .Name = file.BaseName,
                                    .Value = file
                                }
                            End Function) _
                    .ToArray

                Call scans.Clear()

                Dim points As New List(Of TargetPeakPoint)
                Dim linears As New List(Of StandardCurve)

                For Each row As DataGridViewRow In DataGridView1.Rows
                    If isValidLinearRow(row) Then
                        Dim ion As IonPair = Nothing
                        Dim ISion As IonPair = Nothing

                        linears.Add(createLinear(row, ion, ISion))
                        points.AddRange(loadSamples(files, ion))

                        If Not ISion Is Nothing Then
                            points.AddRange(loadSamples(files, ISion))
                        End If
                    End If
                Next

                With linears.ToArray
                    For Each file In points.GroupBy(Function(p) p.SampleName)
                        scans.Add(.SampleQuantify(file.ToArray, PeakAreaMethods.SumAll, fileName:=file.Key))
                    Next
                End With

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
            End If
        End Using
    End Sub

    Private Sub ExportTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportTableToolStripMenuItem.Click

    End Sub
End Class