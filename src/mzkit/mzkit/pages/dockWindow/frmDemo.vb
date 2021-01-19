Imports mzkit.My
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmDemo

    Private Sub frmDemo_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Icon = My.Resources.chemistry

        Me.ShowIcon = True
        Me.TabText = "MS Demo Data"
    End Sub

    Public Sub ShowPage()
        Me.Show(MyApplication.host.dockPanel)
        DockState = DockState.Document
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

    End Sub

    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        If ListView1.SelectedItems.Count = 0 Then
            Return
        End If

        Dim i As Integer = ListView1.SelectedIndices.Item(0)
        Dim info As New DemoItem

        Select Case i
            Case 0 : Call Process.Start("https://ms-imaging.org/wp/imzml/example-files-test/")
            Case 1

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

            Case 2


                ' LC-MSMS
                Dim demoPath As String = $"{App.HOME}/demo/MRM-Data20190222-QCH.mzML"
                MyApplication.host.ShowMRMIons(demoPath)

            Case 3

                ' GC-MS
                Dim demoPath As String = $"{App.HOME}/demo/5ppm.CDF"
                MyApplication.host.ShowGCMSSIM(demoPath)

        End Select

        PropertyGrid1.SelectedObject = info
        PropertyGrid1.Refresh()
    End Sub
End Class