Public Class frmDockDocument

    Friend pages As New List(Of Control)

    Public Sub addPage(ParamArray pageList As Control())
        For Each page As Control In pageList
            Controls.Add(page)
            pages.Add(page)
            page.Dock = DockStyle.Fill
        Next
    End Sub
End Class