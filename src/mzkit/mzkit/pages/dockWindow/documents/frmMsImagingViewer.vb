Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports mzkit.My
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmMsImagingViewer
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Dim render As Drawer

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
    End Sub

    Public Sub LoadRender(render As Drawer)
        Me.render = render

    End Sub

End Class