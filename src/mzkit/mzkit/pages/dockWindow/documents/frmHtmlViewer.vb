#Region "Microsoft.VisualBasic::e7d46a1e851208677fb39dec47cfa0d2, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmHtmlViewer.vb"

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

    '   Total Lines: 77
    '    Code Lines: 62
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 2.54 KB


    ' Class frmHtmlViewer
    ' 
    '     Sub: CopyFullPath, frmHtmlViewer_Load, LoadHtml, OpenContainingFolder, PDF
    '          SaveDocument, WebBrowser1_DocumentCompleted
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports BioNovoGene.mzkit_win32.My
Imports WkHtmlToPdf.Arguments

Public Class frmHtmlViewer

    Dim url As String

    Public Sub PDF(filepath As String)
        Static bin As String = Nothing

        If bin.StringEmpty Then
            bin = $"{App.HOME}/tools/wkhtmltopdf.exe"

            If Not bin.FileExists Then
                bin = $"{App.HOME}/wkhtmltopdf.exe"
            End If
        End If

        If bin.FileExists Then
            Dim env As New PdfConvertEnvironment With {
                .Debug = False,
                .TempFolderPath = TempFileSystem.GetAppSysTempFile,
                .Timeout = 60000,
                .WkHtmlToPdfPath = bin
            }
            Dim content As New PdfDocument With {.Url = {WebBrowser1.Url.OriginalString}}
            Dim pdfFile As New PdfOutput With {.OutputFilePath = filepath}

            Call WkHtmlToPdf.PdfConvert.ConvertHtmlToPdf(content, pdfFile, env)
        Else
            Call MyApplication.host.showStatusMessage("'wkhtmltopdf' tool is missing for generate PDF file...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Public Sub LoadHtml(url As String)
        Me.WebBrowser1.Navigate(url)
        Me.url = url
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.SetText(url)
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Try
            If url.FileExists Then
                Call Process.Start(url.ParentPath)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Overrides Sub SaveDocument() Handles SavePDFToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export page as pdf file.",
            .Filter = "PDF file(*.pdf)|*.pdf"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call PDF(file.FileName)
                Call Process.Start(file.FileName)
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
