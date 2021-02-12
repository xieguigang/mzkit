Public Class frmHtmlViewer

    Public Sub PDF(filepath As String)
        Call WkHtmlToPdf.PdfConvert.ConvertHtmlToPdf(url:=WebBrowser1.Url.OriginalString, filepath)
    End Sub

    Public Sub LoadHtml(url As String)
        Call WebBrowser1.Navigate(url)
    End Sub

    Private Sub SavePDFToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SavePDFToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export page as pdf file.",
            .Filter = "PDF file(*.pdf)|*.pdf"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call PDF(file.FileName)
            End If
        End Using
    End Sub

    Private Sub frmHtmlViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub
End Class