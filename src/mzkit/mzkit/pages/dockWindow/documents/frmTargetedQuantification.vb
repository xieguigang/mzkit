Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
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

                CheckedListBox1.Items.Clear()

                DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
                DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

                For Each file As NamedValue(Of String) In files
                    Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = file.Name})
                Next

                If Not Globals.Settings.MRMLibfile.FileExists Then
                    Globals.Settings.MRMLibfile = New Configuration.Settings().MRMLibfile
                End If

                Dim ionsLib As New IonLibrary(Globals.Settings.MRMLibfile.LoadCsv(Of IonPair))
                Dim allFeatures As IonPair() = files _
                    .Select(Function(file) file.Value) _
                    .GetAllFeatures

                For Each ion As IonPair In allFeatures
                    Dim refId As String = ionsLib.GetDisplay(ion)
                    Dim i As Integer = DataGridView1.Rows.Add(refId)
                    Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

                    Call CheckedListBox1.Items.Add(refId)

                    For Each IS_candidate As IonPair In allFeatures
                        comboxBox.Items.Add(ionsLib.GetDisplay(IS_candidate))
                    Next
                Next
            End If
        End Using
    End Sub

    Private Sub DeleteIonFeatureToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteIonFeatureToolStripMenuItem.Click

    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Dim ref As New List(Of Standards)
        Dim levelKeys As New List(Of String)

        For i As Integer = 2 To DataGridView1.Columns.Count - 1
            levelKeys.Add(DataGridView1.Columns(i).HeaderText)
        Next

        For Each row As DataGridViewRow In DataGridView1.Rows
            Dim rid As String = any.ToString(row.Cells(0).Value)
            Dim IS_id As String = any.ToString(row.Cells(1).Value)
            Dim levels As New Dictionary(Of String, Double)

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

        Dim file As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "/mzkit/standards.csv"

        Call ref.SaveTo(file)
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub
End Class