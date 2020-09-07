Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text

Public Class frmPlotViewer : Implements ISaveHandle, IFileReference

    ''' <summary>
    ''' no used
    ''' </summary>
    ''' <returns></returns>
    Private Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {New ContentType With {.Details = "plot image", .FileExt = ".png", .MIMEType = "plot image", .Name = "plot image"}}
        End Get
    End Property

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        If PictureBox1.BackgroundImage Is Nothing Then
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "plot image(*.png)|*.png"}
            If file.ShowDialog = DialogResult.OK Then
                Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Return PictureBox1.BackgroundImage.SaveAs(path)
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return PictureBox1.BackgroundImage.SaveAs(path)
    End Function

    Private Sub frmPlotViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
    End Sub
End Class