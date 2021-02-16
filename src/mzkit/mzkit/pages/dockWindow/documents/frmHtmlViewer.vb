Imports WkHtmlToPdf.Arguments

Public Class frmHtmlViewer

    Public Sub PDF(filepath As String)
        Dim env As New PdfConvertEnvironment With {
            .Debug = False,
            .TempFolderPath = App.GetAppSysTempFile,
            .Timeout = 60000,
            .WkHtmlToPdfPath = $"{App.HOME}/wkhtmltopdf.exe"
        }

        Call WkHtmlToPdf.PdfConvert.ConvertHtmlToPdf(url:=WebBrowser1.Url.OriginalString, filepath, env)
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

        TabText = "Document Viewer"
        Icon = My.Resources.IE
    End Sub

    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        TabText = WebBrowser1.DocumentTitle
    End Sub
End Class