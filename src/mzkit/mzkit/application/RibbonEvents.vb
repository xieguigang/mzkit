Imports System.Runtime.CompilerServices

Module RibbonEvents

    <Extension>
    Public Sub AddHandlers(ribbonItems As RibbonItems)
        AddHandler ribbonItems.ButtonExit.ExecuteEvent, AddressOf ExitToolsStripMenuItem_Click
        AddHandler ribbonItems.ButtonOpenRaw.ExecuteEvent, AddressOf OpenFile
        AddHandler ribbonItems.ButtonImportsRawFiles.ExecuteEvent, AddressOf ImportsFiles
        AddHandler ribbonItems.ButtonAbout.ExecuteEvent, AddressOf About_Click
        AddHandler ribbonItems.ButtonPageNavBack.ExecuteEvent, AddressOf NavBack_Click
        AddHandler ribbonItems.ButtonNew.ExecuteEvent, AddressOf CreateNewScript

        AddHandler ribbonItems.TweaksImage.ExecuteEvent, AddressOf mzkitTool.ShowPlotTweaks
        AddHandler ribbonItems.ShowProperty.ExecuteEvent, AddressOf ShowProperties

        AddHandler ribbonItems.ButtonMzCalculator.ExecuteEvent, Sub(sender, e) Call ShowPage(mzkitCalculator)
        AddHandler ribbonItems.ButtonSettings.ExecuteEvent, AddressOf ShowSettings
        AddHandler ribbonItems.ButtonMzSearch.ExecuteEvent, Sub(sender, e) Call ShowPage(mzkitSearch)
        AddHandler ribbonItems.ButtonRsharp.ExecuteEvent, AddressOf showRTerm

        AddHandler ribbonItems.ButtonDropA.ExecuteEvent, Sub(sender, e) ShowPage(mzkitTool)
        AddHandler ribbonItems.ButtonDropB.ExecuteEvent, Sub(sender, e) ShowPage(mzkitCalculator)
        AddHandler ribbonItems.ButtonFormulaSearch.ExecuteEvent, Sub(sender, e) ShowPage(mzkitSearch)
        AddHandler ribbonItems.ButtonDropD.ExecuteEvent, Sub(sender, e) ShowPage(mzkitMNtools)
        AddHandler ribbonItems.ButtonShowSpectrumSearchPage.ExecuteEvent, Sub(sender, e) Call New frmSpectrumSearch().Show(dockPanel)

        AddHandler ribbonItems.ButtonCalculatorExport.ExecuteEvent, Sub(sender, e) Call mzkitCalculator.ExportToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExactMassSearchExport.ExecuteEvent, Sub(sender, e) Call mzkitTool.ExportExactMassSearchTable()
        AddHandler ribbonItems.ButtonSave.ExecuteEvent, Sub(sender, e) Call saveCurrentFile()
        AddHandler ribbonItems.ButtonNetworkExport.ExecuteEvent, Sub(sender, e) Call mzkitMNtools.saveNetwork()
        AddHandler ribbonItems.ButtonFormulaSearchExport.ExecuteEvent, Sub(sender, e) Call mzkitSearch.SaveSearchResultTable()

        AddHandler ribbonItems.ButtonBioDeep.ExecuteEvent, Sub(sender, e) Call Process.Start("http://www.biodeep.cn/")
        AddHandler ribbonItems.ButtonLicense.ExecuteEvent, Sub(sender, e) Call New frmLicense().ShowDialog()

        AddHandler ribbonItems.ButtonExportImage.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExportMatrix.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveMatrixToolStripMenuItem_Click()

        AddHandler ribbonItems.ButtonLayout1.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonLayout2.ExecuteEvent, Sub(sender, e) Call mzkitTool.SaveMatrixToolStripMenuItem_Click()

        AddHandler ribbonItems.ButtonShowStartPage.ExecuteEvent, AddressOf showStartPage
        AddHandler ribbonItems.ButtonShowLogWindow.ExecuteEvent, AddressOf showLoggingWindow

        AddHandler ribbonItems.ButtonShowExplorer.ExecuteEvent, AddressOf ShowExplorer
        AddHandler ribbonItems.ButtonShowSearchList.ExecuteEvent, AddressOf ShowSearchList
        AddHandler ribbonItems.ButtonShowProperties.ExecuteEvent, AddressOf ShowProperties

        AddHandler ribbonItems.ButtonShowPlotViewer.ExecuteEvent, Sub(sender, e) Call mzkitTool.ShowTabPage(mzkitTool.TabPage5)
        AddHandler ribbonItems.ButtonShowMatrixViewer.ExecuteEvent, Sub(sender, e) Call mzkitTool.ShowTabPage(mzkitTool.TabPage6)
        AddHandler ribbonItems.ButtonNetworkRender.ExecuteEvent, Sub(sender, e) Call mzkitMNtools.RenderNetwork()
        AddHandler ribbonItems.ButtonRefreshNetwork.ExecuteEvent, Sub(sender, e) Call mzkitMNtools.RefreshNetwork()

        AddHandler ribbonItems.ButtonRunScript.ExecuteEvent, AddressOf RunCurrentScript
        AddHandler ribbonItems.ButtonSaveScript.ExecuteEvent, AddressOf saveCurrentScript

        AddHandler ribbonItems.HelpButton.ExecuteEvent, AddressOf showHelp

        AddHandler ribbonItems.ButtonTIC.ExecuteEvent, Sub(sender, e) Call mzkitTool.TIC(isBPC:=False)
        AddHandler ribbonItems.ButtonBPC.ExecuteEvent, Sub(sender, e) Call mzkitTool.TIC(isBPC:=True)
        AddHandler ribbonItems.ButtonXIC.ExecuteEvent, AddressOf rawFeaturesList.ShowXICToolStripMenuItem_Click

        AddHandler ribbonItems.ButtonResetLayout.ExecuteEvent, AddressOf resetLayout

        AddHandler ribbonItems.RecentItems.ExecuteEvent, AddressOf _recentItems_ExecuteEvent
        ' AddHandler ribbonItems.ButtonMsImaging.ExecuteEvent, AddressOf showMsImaging
        AddHandler ribbonItems.ButtonMsDemo.ExecuteEvent, Sub() msDemo.ShowPage()
        AddHandler ribbonItems.Targeted.ExecuteEvent, Sub() Call ConnectToBioDeep.OpenAdvancedFunction(AddressOf VisualStudio.ShowSingleDocument(Of frmTargetedQuantification))

        AddHandler ribbonItems.MRMLibrary.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmMRMLibrary)(Nothing)
        AddHandler ribbonItems.QuantifyIons.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmQuantifyIons)(Nothing)

        AddHandler ribbonItems.LogInBioDeep.ExecuteEvent, Sub() Call New frmLogin().ShowDialog()

        AddHandler ribbonItems.ButtonInstallMzkitPackage.ExecuteEvent, AddressOf MyApplication.InstallPackageRelease
        AddHandler ribbonItems.ShowGCMSExplorer.ExecuteEvent, Sub() Call VisualStudio.Dock(GCMSPeaks, DockState.DockLeft)
        AddHandler ribbonItems.ShowMRMExplorer.ExecuteEvent, Sub() Call VisualStudio.Dock(MRMIons, DockState.DockLeft)

        AddHandler ribbonItems.Tutorials.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmVideoList)()

        AddHandler ribbonItems.AdjustParameters.ExecuteEvent, Sub() Call VisualStudio.Dock(parametersTool, DockState.DockRight)
    End Sub
End Module
