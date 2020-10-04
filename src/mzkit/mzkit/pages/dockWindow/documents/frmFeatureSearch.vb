Imports System.Windows.Forms.ListViewItem
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Language
Imports Task

Public Class frmFeatureSearch

    Public Sub AddFileMatch(file As String, matches As ParentMatch())
        Dim matchHeaders = {
            New ColumnHeader() With {.Text = "Precursor Type"},
            New ColumnHeader() With {.Text = "Adducts"},
            New ColumnHeader() With {.Text = "M"}
        }
        Me.TreeListView1.Columns.AddRange(matchHeaders)

        Dim row As New TreeListViewItem With {.Text = file.FileName, .ImageIndex = 0, .ToolTipText = file}
        Dim i As i32 = 1

        For Each member As ParentMatch In matches
            Dim ion As New TreeListViewItem(member.id) With {.ImageIndex = 1, .ToolTipText = member.id}

            ion.SubItems.Add(New ListViewSubItem With {.Text = $"#{++i}"})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.mz})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.rt})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.ppm})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.polarity})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.charge})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.BPC})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.TIC})

            ion.SubItems.Add(New ListViewSubItem With {.Text = member.precursor_type})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.adducts})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.M})

            row.Items.Add(ion)
        Next

        row.SubItems.Add(New ListViewSubItem With {.Text = matches.Length})

        TreeListView1.Items.Add(row)
    End Sub

    Public Sub AddFileMatch(file As String, targetMz As Double, matches As ScanEntry())
        Dim row As New TreeListViewItem With {.Text = file.FileName, .ImageIndex = 0, .ToolTipText = file}
        Dim i As i32 = 1

        For Each member As ScanEntry In matches
            Dim ion As New TreeListViewItem(member.id) With {.ImageIndex = 1, .ToolTipText = member.id}

            ion.SubItems.Add(New ListViewSubItem With {.Text = $"#{++i}"})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.mz})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.rt})
            ion.SubItems.Add(New ListViewSubItem With {.Text = PPMmethod.PPM(member.mz, targetMz).ToString("F2")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.polarity})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.charge})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.BPC})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.TIC})

            row.Items.Add(ion)
        Next

        row.SubItems.Add(New ListViewSubItem With {.Text = matches.Length})

        TreeListView1.Items.Add(row)
    End Sub

End Class