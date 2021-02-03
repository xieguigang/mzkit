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
            Call pasteData()
        End If
    End Sub

    Private Sub pasteData()
        Dim text As String = Clipboard.GetText

        If DataGridView1.SelectedCells.Count = 0 Then
            Return
        End If

        Dim i As Integer = DataGridView1.SelectedCells.Item(Scan0).RowIndex
        Dim j As Integer = DataGridView1.SelectedCells.Item(Scan0).ColumnIndex

        If text.Contains(vbCr) OrElse text.Contains(vbLf) Then
            Dim colCells As String() = text.LineTokens

            For ii As Integer = 0 To colCells.Length - 1
                DataGridView1.Rows(ii + i).Cells(j).Value = colCells(ii)
            Next
        Else
            Dim rowCells As String() = text.Split(ASCII.TAB)


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
End Class