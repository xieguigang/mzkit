#Region "Microsoft.VisualBasic::378238dabfb5f9c93c835b6b955842c1, mzkit\src\mzkit\mzkit\application\WindowModules.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 123
'    Code Lines: 97
' Comment Lines: 0
'   Blank Lines: 26
'     File Size: 4.66 KB


' Class WindowModules
' 
'     Properties: ribbon
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: initializeVSPanel, OpenFile
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.DockSample
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.RibbonLib.Controls
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
    Friend Shared ReadOnly MSIPixelProperty As New MSIPixelPropertyWindow

    Public Shared ReadOnly Property ribbon As RibbonItems
        Get
            Return MyApplication.host.ribbonItems
        End Get
    End Property

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

        MSIPixelProperty.Show(dockPanel)
        MSIPixelProperty.DockState = DockState.Hidden

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
            "All Raw Data Files(*.mzXML;*.mzML;*.mzPack;*.imzML;*.cdf;*.netcdf;*.raw;*.wiff)|*.mzXML;*.mzML;*.mzPack;*.imzML;*.cdf;*.netcdf;*.raw;*.wiff",
            "Untargetted Raw Data(*.mzXML;*.mzML;*.mzPack)|*.mzXML;*.mzML;*.mzPack",
            "Image mzML(*.imzML)|*.imzML",
            "GC-MS Targeted(*.cdf)|*.cdf;*.netcdf",
            "GC-MS / LC-MS/MS Targeted(*.mzML)|*.mzML",
            "Thermo Raw File(*.raw)|*.raw",
            "Ab Sciex Wiff(*.wiff)|*.wiff",
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
