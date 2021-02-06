Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports any = Microsoft.VisualBasic.Scripting

Public Class frmMRMLibrary
    Implements ISaveHandle
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "MRM Ion Pairs", .FileExt = ".csv", .MIMEType = "application/csv", .Name = "MRM Ion Pairs"}
            }
        End Get
    End Property

    ' HMDB0000097	Choline	103.765	60

    Private Sub TabPage1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown, DataGridView1.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub

    Private Sub frmMRMLibrary_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MRM ions Library"

        If Not Globals.Settings.MRMLibfile.FileExists Then
            Globals.Settings.MRMLibfile = New Configuration.Settings().MRMLibfile
        End If

        Dim ions As IonPair() = Globals.Settings.MRMLibfile.LoadCsv(Of IonPair)

        FilePath = Globals.Settings.MRMLibfile

        For Each ion As IonPair In ions
            DataGridView1.Rows.Add(ion.accession, ion.name, ion.rt, ion.precursor, ion.product)
        Next
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim ions As New List(Of IonPair)
        Dim row As DataGridViewRow

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            row = DataGridView1.Rows.Item(i)
            ions += New IonPair With {
                .accession = any.ToString(row.Cells(0).Value),
                .name = any.ToString(row.Cells(1).Value),
                .rt = any.ToString(row.Cells(2).Value).ParseDouble,
                .precursor = any.ToString(row.Cells(3).Value).ParseDouble,
                .product = any.ToString(row.Cells(4).Value).ParseDouble
            }
        Next

        FilePath = path
        Globals.Settings.MRMLibfile = path.GetFullPath

        Return ions.SaveTo(path)
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        If DataGridView1.SelectedRows.Count > 0 Then
            For Each row As DataGridViewRow In DataGridView1.SelectedRows
                DataGridView1.Rows.RemoveAt(row.Index)
            Next
        End If
    End Sub
End Class