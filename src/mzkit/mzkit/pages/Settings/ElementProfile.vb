Public Class ElementProfile : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings

    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Throw New NotImplementedException()
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call DirectCast(ParentForm, frmMain).ShowPage(DirectCast(ParentForm, frmMain).mzkitSearch)
    End Sub
End Class
