Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text
Imports mzkit.My
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task

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

                For Each file As NamedValue(Of String) In files
                    Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = file.Name})
                Next

                If Not Globals.Settings.MRMLibfile.FileExists Then
                    Globals.Settings.MRMLibfile = New Configuration.Settings().MRMLibfile
                End If

                Dim ionsLib As New IonLibrary(Globals.Settings.MRMLibfile.LoadCsv(Of IonPair))

                For Each ion As IonPair In files _
                    .Select(Function(file) file.Value) _
                    .GetAllFeatures

                    Dim refId As String = ionsLib.GetDisplay(ion)

                    Call DataGridView1.Rows.Add(refId)
                    Call CheckedListBox1.Items.Add(refId)
                Next
            End If
        End Using
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
End Class