Imports System.ComponentModel
Imports RibbonLib

Public Class ExportData
    Inherits ContextMenuStrip

    Private _contextPopupID As UInteger
    Private _ribbon As Ribbon

    Public Sub New(ribbon As Ribbon, contextPopupID As UInteger)
        MyBase.New()
        _contextPopupID = contextPopupID
        _ribbon = ribbon
    End Sub

    Protected Overrides Sub OnOpening(ByVal e As CancelEventArgs)
        _ribbon.ShowContextPopup(_contextPopupID, Cursor.Position.X, Cursor.Position.Y)
        e.Cancel = True
    End Sub
End Class