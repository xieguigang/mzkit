Imports Microsoft.VisualBasic.CommandLine
Imports mzkit.My
Imports WeifenLuo.WinFormsUI.Docking

Module CLI

    Public Function openRawFile(filepath As String, args As CommandLine) As Integer
        MyApplication.afterLoad =
            Sub()
                Select Case filepath.ExtensionSuffix.ToLower
                    Case "mzpack"
                        MyApplication.host.OpenFile(filepath, showDocument:=True)
                        WindowModules.panelMain.Show(MyApplication.host.dockPanel)
                        WindowModules.panelMain.DockState = DockState.Document
                End Select
            End Sub

        Return 0
    End Function
End Module
