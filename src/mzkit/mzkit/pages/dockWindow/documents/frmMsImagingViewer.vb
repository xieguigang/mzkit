Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmMsImagingViewer
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Dim render As Drawer
    Dim params As MsImageProperty
    Dim WithEvents checks As ToolStripMenuItem

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "Image mzML", .FileExt = ".imzML", .MIMEType = "text/xml", .Name = "Image mzML"}
            }
        End Get
    End Property

    Private Sub frmMsImagingViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text
        MyApplication.host.msImageParameters.DockState = DockState.DockLeft
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
    End Sub

    Public Sub LoadRender(render As Drawer)
        Dim checks As CheckedListBox = MyApplication.host.msImageParameters.CheckedListBox1

        Me.checks = MyApplication.host.msImageParameters.RenderingToolStripMenuItem
        Me.render = render
        Me.params = New MsImageProperty(render)

        MyApplication.host.msImageParameters.PropertyGrid1.SelectedObject = params
        MyApplication.host.msImageParameters.CheckedListBox1.Items.Clear()

        For Each mz As Double In render.LoadMzArray(20)
            Call checks.Items.Add(mz.ToString("F4"))
            Call Application.DoEvents()
        Next
    End Sub

    Private Sub checks_Click(sender As Object, e As EventArgs) Handles checks.Click
        Dim mz = MyApplication.host.msImageParameters.CheckedListBox1.SelectedItems

        If mz.Count = 0 Then
            Call MyApplication.host.showStatusMessage("No ions selected for rendering!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Dim selectedMz As New List(Of Double)
            Dim progress As New frmProgressSpinner

            For i As Integer = 0 To mz.Count - 1
                selectedMz.Add(Val(CStr(mz.Item(i))))
            Next

            MyApplication.host.showStatusMessage($"Run MS-Image rendering for {mz.Count} selected ions...")

            Call New Thread(
                Sub()
                    Call Invoke(Sub() PictureBox1.BackgroundImage = render.DrawLayer(selectedMz.ToArray))
                    Call progress.Invoke(Sub() progress.Close())
                End Sub).Start()

            Call progress.ShowDialog()
            Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
        End If
    End Sub
End Class