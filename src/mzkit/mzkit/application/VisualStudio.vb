Imports mzkit.DockSample
Imports mzkit.My
Imports WeifenLuo.WinFormsUI.Docking

Public Class VisualStudio

    Public Shared Sub Dock(win As ToolWindow, prefer As DockState)
        Select Case win.DockState
            Case DockState.Hidden, DockState.Unknown
                win.DockState = prefer
            Case DockState.Float, DockState.Document,
                 DockState.DockTop,
                 DockState.DockRight,
                 DockState.DockLeft,
                 DockState.DockBottom

                ' do nothing 
            Case DockState.DockBottomAutoHide
                win.DockState = DockState.DockBottom
            Case DockState.DockLeftAutoHide
                win.DockState = DockState.DockLeft
            Case DockState.DockRightAutoHide
                win.DockState = DockState.DockRight
            Case DockState.DockTopAutoHide
                win.DockState = DockState.DockTop
        End Select
    End Sub

    Public Shared Sub ShowProperties(item As Object)
        Dim propertyWin = MyApplication.host.propertyWin

        propertyWin.propertyGrid.SelectedObject = item
        propertyWin.propertyGrid.Refresh()
    End Sub

    Public Shared Sub ShowSingleDocument(Of T As {New, DockContent})(Optional showExplorer As Action = Nothing)
        Dim DockPanel As DockPanel = MyApplication.host.dockPanel
        Dim targeted As T = DockPanel.Documents _
            .Where(Function(doc) TypeOf doc Is T) _
            .FirstOrDefault

        If targeted Is Nothing Then
            targeted = New T
        End If

        If Not showExplorer Is Nothing Then
            Call showExplorer()
        End If

        targeted.Show(DockPanel)
        targeted.DockState = DockState.Document
    End Sub

    Public Shared Function ShowDocument(Of T As {New, DocumentWindow})() As T
        Dim newDoc As New T()

        newDoc.Show(MyApplication.host.dockPanel)
        newDoc.DockState = DockState.Document

        Return newDoc
    End Function
End Class
