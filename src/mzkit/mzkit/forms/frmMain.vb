Imports System.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports RibbonLib
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop

Public Class frmMain

    Dim pages As New List(Of Control)

    Dim mzkitTool As New PageMzkitTools
    Dim mzkitSettings As New PageSettings

    Private Sub ShowPage(page As Control)
        For Each page2 In pages
            If Not page Is page2 Then
                page2.Hide()
            End If
        Next

        page.Show()
    End Sub

    Private Sub OpenFile(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Call mzkitTool.ImportsRaw()
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim SaveFileDialog As New SaveFileDialog
        SaveFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        SaveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"

        If (SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String = SaveFileDialog.FileName
            ' TODO: Add code here to save the current contents of the form to a file.
        End If
    End Sub

    Private Sub ExitToolsStripMenuItem_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Me.Close()
    End Sub

    Private Sub CutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Use My.Computer.Clipboard to insert the selected text or images into the clipboard
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Use My.Computer.Clipboard to insert the selected text or images into the clipboard
    End Sub

    Private Sub PasteToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        'Use My.Computer.Clipboard.GetText() or My.Computer.Clipboard.GetData to retrieve information from the clipboard.
    End Sub

    Private Sub ToolBarToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Me.ToolStrip.Visible = Me.ToolBarToolStripMenuItem.Checked
    End Sub

    Private Sub StatusBarToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Me.StatusStrip.Visible = Me.StatusBarToolStripMenuItem.Checked
    End Sub

    Friend ribbonItems As RibbonItems
    Friend recentItems As List(Of RecentItemsPropertySet)

    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ribbonItems = New RibbonItems(Ribbon1)

        AddHandler ribbonItems.ButtonExit.ExecuteEvent, AddressOf ExitToolsStripMenuItem_Click
        AddHandler ribbonItems.ButtonOpenRaw.ExecuteEvent, AddressOf OpenFile
        AddHandler ribbonItems.ButtonMzCalculator.ExecuteEvent, AddressOf MzCalculatorToolStripMenuItem_Click
        AddHandler ribbonItems.ButtonAbout.ExecuteEvent, AddressOf About_Click


        AddHandler ribbonItems.ButtonSettings.ExecuteEvent, AddressOf Settings_Click
    End Sub

    Private Sub Settings_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Call ShowPage(mzkitSettings)
    End Sub

    Private Sub About_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Call New frmSplashScreen() With {.isAboutScreen = True, .TopMost = True}.Show()
    End Sub

    Private Sub MzCalculatorToolStripMenuItem_Click(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        Call New frmCalculator().ShowDialog()
    End Sub

    Private Sub addPage(ParamArray pageList As Control())
        For Each page As Control In pageList
            Panel1.Controls.Add(page)
            pages.Add(page)
            page.Dock = DockStyle.Fill
        Next
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitRecentItems()
        InitSpinner()
        ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        addPage(mzkitTool, mzkitSettings)
        ShowPage(mzkitTool)

        ToolStripStatusLabel.Text = "Ready!"
    End Sub

    Private Sub InitSpinner()
        Dim _spinner = ribbonItems.Spinner

        _spinner.TooltipTitle = "PPM"
        _spinner.TooltipDescription = "Enter ppm error for search feature by m/z."
        _spinner.MaxValue = 30D
        _spinner.MinValue = 0
        _spinner.Increment = 0.5D
        _spinner.DecimalValue = 10D
    End Sub

    Private Sub InitRecentItems()
        ' prepare list of recent items
        recentItems = New List(Of RecentItemsPropertySet)()
        recentItems.Add(New RecentItemsPropertySet() With {.Label = "Recent item 1", .LabelDescription = "Recent item 1 description", .Pinned = True})
        recentItems.Add(New RecentItemsPropertySet() With {.Label = "Recent item 2", .LabelDescription = "Recent item 2 description", .Pinned = False})

        ribbonItems.RecentItems.RecentItems = recentItems
    End Sub

    Private Sub _recentItems_ExecuteEvent(ByVal sender As Object, ByVal e As ExecuteEventArgs)
        If e.Key.PropertyKey = RibbonProperties.RecentItems Then
            ' go over recent items
            Dim objectArray() As Object = CType(e.CurrentValue.PropVariant.Value, Object())
            For i As Integer = 0 To objectArray.Length - 1
                Dim propertySet As IUISimplePropertySet = TryCast(objectArray(i), IUISimplePropertySet)

                If propertySet IsNot Nothing Then
                    Dim propLabel As PropVariant
                    propertySet.GetValue(RibbonProperties.Label, propLabel)
                    Dim label As String = CStr(propLabel.Value)

                    Dim propLabelDescription As PropVariant
                    propertySet.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
                    Dim labelDescription As String = CStr(propLabelDescription.Value)

                    Dim propPinned As PropVariant
                    propertySet.GetValue(RibbonProperties.Pinned, propPinned)
                    Dim pinned As Boolean = CBool(propPinned.Value)

                    ' update pinned value
                    recentItems(i).Pinned = pinned
                End If
            Next i
        ElseIf e.Key.PropertyKey = RibbonProperties.SelectedItem Then
            ' get selected item index
            Dim selectedItem As UInteger = CUInt(e.CurrentValue.PropVariant.Value)

            ' get selected item label
            Dim propLabel As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Label, propLabel)
            Dim label As String = CStr(propLabel.Value)

            ' get selected item label description
            Dim propLabelDescription As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
            Dim labelDescription As String = CStr(propLabelDescription.Value)

            ' get selected item pinned value
            Dim propPinned As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Pinned, propPinned)
            Dim pinned As Boolean = CBool(propPinned.Value)
        End If
    End Sub

    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        mzkitTool.SaveFileCache()
    End Sub
End Class
