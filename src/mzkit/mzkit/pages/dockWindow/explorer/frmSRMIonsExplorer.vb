Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports mzkit.My
Imports Task

Public Class frmSRMIonsExplorer

    Public Sub LoadMRM(file As String)
        Dim list = file.LoadChromatogramList.ToArray
        Dim TIC = list.Where(Function(i) i.id.TextEquals("TIC")).First

        ' Call Win7StyleTreeView1.Nodes.Clear()

        Dim TICRoot As TreeNode = Win7StyleTreeView1.Nodes.Add(file.FileName)

        TICRoot.Tag = TIC
        TICRoot.ImageIndex = 0

        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim display As String

        For Each chr As chromatogram In list.Where(Function(i) Not i.id.TextEquals("TIC"))
            Dim ionRef As New IonPair With {
                .precursor = chr.precursor.MRMTargetMz,
                .product = chr.product.MRMTargetMz
            }

            display = ionsLib.GetDisplay(ionRef)

            With TICRoot.Nodes.Add(display)
                .Tag = chr
                .ImageIndex = 1
                .SelectedImageIndex = 1
            End With
        Next
    End Sub

    Private Sub ShowTICOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICOverlapToolStripMenuItem.Click
        Dim list As New List(Of NamedCollection(Of ChromatogramTick))

        For Each rawfile As TreeNode In Win7StyleTreeView1.Nodes
            Dim fileName As String = rawfile.Text.BaseName

            For Each obj As TreeNode In rawfile.Nodes
                If Not obj.Checked Then
                    Continue For
                End If

                With DirectCast(obj.Tag, chromatogram)
                    list += New NamedCollection(Of ChromatogramTick)($"[{fileName}] {obj.Text}", .Ticks)
                End With
            Next
        Next

        Call MyApplication.host.mzkitTool.TIC(list.ToArray)
    End Sub

    Private Sub frmSRMIonsExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MRM Ions"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        Dim chr As chromatogram = e.Node.Tag
        Dim ticks As ChromatogramTick() = chr.Ticks
        Dim proper As New MRMROIProperty(chr)

        Call MyApplication.host.mzkitTool.ShowMRMTIC(e.Node.Text, ticks)
        Call VisualStudio.ShowProperties(proper)
    End Sub

    Private Sub ShowSpectrumToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If Win7StyleTreeView1.SelectedNode Is Nothing OrElse DirectCast(Win7StyleTreeView1.SelectedNode.Tag, chromatogram).id = "TIC" Then
            Return
        End If

        Dim chr As chromatogram = Win7StyleTreeView1.SelectedNode.Tag
        Dim spectrum As ms2() = {
            New ms2 With {.mz = chr.precursor.MRMTargetMz, .intensity = 1, .quantity = 1},
            New ms2 With {.mz = chr.product.MRMTargetMz, .intensity = 0.6, .quantity = 0.6}
        }
        Dim scanData As New LibraryMatrix With {.ms2 = spectrum, .name = "SRM ions"}
        Dim q = scanData.OrderByDescending(Function(x) x.intensity).First
        Dim title1$ = $"SRM ion pair"
        Dim title2$ = $"[{spectrum(0).mz.ToString("F4")}:{spectrum(1).intensity.ToString("G3")}]"

        Call MyApplication.host.mzkitTool.showMatrix(spectrum, $"SRM ion pair [{spectrum(0).mz.ToString("F4")}:{spectrum(1).intensity.ToString("G3")}]")
        Call MyApplication.host.mzkitTool.PlotMatrx(title1, title2, scanData)
    End Sub
End Class