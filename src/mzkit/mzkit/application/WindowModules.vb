Imports mzkit.DockSample
Imports mzkit.My
Imports WeifenLuo.WinFormsUI.Docking

Friend MustInherit Class WindowModules

    Friend Shared ReadOnly viewer As New frmMsImagingViewer

    Friend Shared ReadOnly fileExplorer As New frmFileExplorer
    Friend Shared ReadOnly rawFeaturesList As New frmRawFeaturesList
    Friend Shared ReadOnly UVScansList As New frmUVScans
    Friend Shared ReadOnly spectrumTreeExplorer As New frmTreeExplorer

    Friend Shared ReadOnly output As New OutputWindow
    Friend Shared WithEvents panelMain As New frmDockDocument
    Friend Shared startPage As New frmStartPage
    Friend Shared ReadOnly settingsPage As New frmSettings
    Friend Shared ReadOnly RtermPage As New frmRsharp
    Friend Shared ReadOnly propertyWin As New PropertyWindow
    Friend Shared ReadOnly taskWin As New TaskListWindow
    Friend Shared ReadOnly plotParams As New frmTweaks
    Friend Shared ReadOnly msImageParameters As New frmMsImagingTweaks
    Friend Shared ReadOnly msDemo As New frmDemo
    Friend Shared ReadOnly MRMIons As New frmSRMIonsExplorer
    Friend Shared ReadOnly GCMSPeaks As New frmGCMSPeaks
    Friend Shared ReadOnly parametersTool As New AdjustParameters

    Private Sub New()
    End Sub

    Public Shared Sub initializeVSPanel()
        Dim dockPanel As DockPanel = MyApplication.host.dockPanel

        output.Show(dockPanel)
        MyApplication.RegisterOutput(output)
        fileExplorer.Show(dockPanel)

        UVScansList.Show(dockPanel)
        UVScansList.DockState = DockState.Hidden

        spectrumTreeExplorer.Show(dockPanel)
        spectrumTreeExplorer.DockState = DockState.Hidden

        plotParams.Show(dockPanel)
        plotParams.DockState = DockState.Hidden

        rawFeaturesList.Show(dockPanel)
        propertyWin.Show(dockPanel)

        startPage.Show(dockPanel)
        startPage.DockState = DockState.Document

        panelMain.Show(dockPanel)
        panelMain.DockState = DockState.Document

        settingsPage.Show(dockPanel)
        settingsPage.DockState = DockState.Hidden

        GCMSPeaks.Show(dockPanel)
        GCMSPeaks.DockState = DockState.Hidden

        MRMIons.Show(dockPanel)
        MRMIons.DockState = DockState.Hidden

        RtermPage.Show(dockPanel)
        RtermPage.DockState = DockState.Hidden

        taskWin.Show(dockPanel)
        taskWin.DockState = DockState.DockBottomAutoHide

        msImageParameters.Show(dockPanel)
        msImageParameters.DockState = DockState.Hidden

        parametersTool.Show(dockPanel)
        parametersTool.DockState = DockState.Hidden

        msDemo.Show(dockPanel)
        msDemo.DockState = DockState.Hidden

        If Globals.Settings.ui.rememberLayouts Then
            fileExplorer.DockState = Globals.Settings.ui.fileExplorerDock
            rawFeaturesList.DockState = Globals.Settings.ui.featureListDock
            output.DockState = Globals.Settings.ui.OutputDock
            propertyWin.DockState = Globals.Settings.ui.propertyWindowDock
        Else
            fileExplorer.DockState = DockState.DockLeftAutoHide
            rawFeaturesList.DockState = DockState.DockLeftAutoHide
            output.DockState = DockState.DockBottomAutoHide
            propertyWin.DockState = DockState.DockRightAutoHide
        End If
    End Sub

    Public Shared Sub OpenFile()
        Dim filters As String() = {
            "Untargetted Raw Data(*.mzXML;*.mzML;*.mzPack)|*.mzXML;*.mzML;*.mzPack",
            "Image mzML(*.imzML)|*.imzML",
            "GC-MS Targeted(*.cdf)|*.cdf;*.netcdf",
            "GC-MS / LC-MS/MS Targeted(*.mzML)|*.mzML",
            "R# Script(*.R)|*.R"
        }

        Using file As New OpenFileDialog With {
            .Filter = filters.JoinBy("|")
        }
            If file.ShowDialog = DialogResult.OK Then
                Call MyApplication.host.OpenFile(file.FileName, showDocument:=True)
            End If
        End Using
    End Sub
End Class
