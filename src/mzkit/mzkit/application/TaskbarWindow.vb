Imports Microsoft.Windows.Taskbar
Imports mzkit.My
Imports WeifenLuo.WinFormsUI.Docking

Module TaskBarWindow

    Private Function GetTabPages() As DocumentWindow()
        Return MyApplication.host.dockPanel.Documents _
            .Where(Function(page) TypeOf page Is DocumentWindow) _
            .Select(Function(doc) DirectCast(doc, DocumentWindow)) _
            .ToArray
    End Function

    Friend Sub preview_TabbedThumbnailActivated(ByVal sender As Object, ByVal e As TabbedThumbnailEventArgs)
        ' User selected a tab via the thumbnail preview
        ' Select the corresponding control in our app
        For Each page As DocumentWindow In GetTabPages()
            If page.Handle = e.WindowHandle Then
                ' Select the tab in the application UI as well as taskbar tabbed thumbnail list
                page.Show(MyApplication.host.dockPanel)
                TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(page.preview)
            End If
        Next

        ' Also activate our parent form (incase we are minimized, this will restore it)
        If MyApplication.host.WindowState = FormWindowState.Minimized Then
            MyApplication.host.WindowState = FormWindowState.Normal
        End If
    End Sub

    Friend Sub preview_TabbedThumbnailClosed(ByVal sender As Object, ByVal e As TabbedThumbnailClosedEventArgs)
        Dim pageClosed As DocumentWindow = Nothing

        ' Find the tabpage that was "closed" by the user (via the taskbar tabbed thumbnail)
        For Each page As DocumentWindow In GetTabPages()
            If page.Handle = e.WindowHandle Then
                pageClosed = page
                Exit For
            End If
        Next

        If pageClosed IsNot Nothing Then
            ' Remove the event handlers


            ' Dispose the tab
            pageClosed.Close()


        End If

        Dim tabbedThumbnail As TabbedThumbnail = TryCast(sender, TabbedThumbnail)
        If tabbedThumbnail IsNot Nothing Then
            ' Remove the event handlers from the tab preview
            RemoveHandler tabbedThumbnail.TabbedThumbnailActivated, AddressOf preview_TabbedThumbnailActivated
            RemoveHandler tabbedThumbnail.TabbedThumbnailClosed, AddressOf preview_TabbedThumbnailClosed
            RemoveHandler tabbedThumbnail.TabbedThumbnailMaximized, AddressOf preview_TabbedThumbnailMaximized
            RemoveHandler tabbedThumbnail.TabbedThumbnailMinimized, AddressOf preview_TabbedThumbnailMinimized
        End If
    End Sub

    Friend Sub preview_TabbedThumbnailMaximized(ByVal sender As Object, ByVal e As TabbedThumbnailEventArgs)
        ' User clicked on the maximize button on the thumbnail's context menu
        ' Maximize the app
        MyApplication.host.WindowState = FormWindowState.Maximized

        ' If there is a selected tab, take it's screenshot
        ' invalidate the tab's thumbnail
        ' update the "preview" object with the new thumbnail
        If MyApplication.host.dockPanel.ActiveDocument IsNot Nothing Then
            UpdatePreviewBitmap(MyApplication.host.dockPanel.ActiveDocument)
        End If
    End Sub

    Friend Sub preview_TabbedThumbnailMinimized(ByVal sender As Object, ByVal e As TabbedThumbnailEventArgs)
        ' User clicked on the minimize button on the thumbnail's context menu
        ' Minimize the app
        MyApplication.host.WindowState = FormWindowState.Minimized
    End Sub

    ''' <summary>
    ''' Helper method to update the thumbnail preview for a given tab page.
    ''' </summary>
    ''' <param name="tabPage"></param>
    Friend Sub UpdatePreviewBitmap(ByVal tabPage As DocumentWindow)
        If tabPage IsNot Nothing Then
            Dim preview As TabbedThumbnail = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(tabPage)

            If preview IsNot Nothing Then
                Dim bitmap As Bitmap = TabbedThumbnailScreenCapture.GrabWindowBitmap(tabPage.Handle, tabPage.Size)
                preview.SetImage(bitmap)
            End If


        End If
    End Sub
End Module
