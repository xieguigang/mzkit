Imports mzkit.My
Imports Task

Public Class DemoDataPage

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Call Process.Start("https://ms-imaging.org/wp/imzml/example-files-test/")
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        Dim findRaw = MyApplication.fileExplorer.findRawFileNode("003_Ex2_Orbitrap_CID.mzXML")
        Dim demoPath As String = $"{App.HOME}/demo/003_Ex2_Orbitrap_CID.mzXML"

        If findRaw Is Nothing Then
            If Not demoPath.FileExists Then
                MyApplication.host.showStatusMessage("the demo data file is missing!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return
            End If
            MyApplication.fileExplorer.addFileNode(MyApplication.fileExplorer.getRawCache(demoPath))
            findRaw = MyApplication.fileExplorer.findRawFileNode("003_Ex2_Orbitrap_CID.mzXML")
        End If

        MyApplication.fileExplorer.treeView1.SelectedNode = findRaw
        MyApplication.fileExplorer.showRawFile(DirectCast(findRaw.Tag, Raw))
        MyApplication.host.ShowMzkitToolkit()
    End Sub
End Class
