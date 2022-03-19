#Region "Microsoft.VisualBasic::a3c3a8a51471fa789759ba9dcba6b66e, mzkit\src\mzkit\mzkit\forms\frmTweaks\frmMsImagingTweaks.vb"

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

    '   Total Lines: 339
    '    Code Lines: 258
    ' Comment Lines: 23
    '   Blank Lines: 58
    '     File Size: 12.19 KB


    ' Class frmMsImagingTweaks
    ' 
    '     Properties: Parameters
    ' 
    '     Function: GetSelectedIons
    ' 
    '     Sub: AddIonMzLayer, checkNode, ClearIons, frmMsImagingTweaks_Load, LoadBasePeakIonsToolStripMenuItem_Click
    '          loadBasePeakMz, LoadPinnedIons, loadRenderFromCDF, PropertyGrid1_DragDrop, PropertyGrid1_DragEnter
    '          RGBLayers, ToolStripButton1_Click, ToolStripButton2_Click, ToolStripSpringTextBox1_Click, uncheckNode
    '          Win7StyleTreeView1_AfterCheck
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports BioNovoGene.mzkit_win32.My
Imports RibbonLib.Interop
Imports Task
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Public Class frmMsImagingTweaks

    Friend checkedMz As New List(Of TreeNode)
    Friend viewer As frmMsImagingViewer

    Public Iterator Function GetSelectedIons() As IEnumerable(Of Double)
        If Not Win7StyleTreeView1.SelectedNode Is Nothing Then
            If Not Win7StyleTreeView1.SelectedNode.Checked Then
                If Win7StyleTreeView1.SelectedNode.Tag Is Nothing Then
                    For Each node As TreeNode In Win7StyleTreeView1.SelectedNode.Nodes
                        Yield DirectCast(node.Tag, Double)
                    Next
                Else
                    Yield DirectCast(Win7StyleTreeView1.SelectedNode.Tag, Double)
                End If
            Else
                GoTo UseCheckedList
            End If
        Else
UseCheckedList:
            If checkedMz.Count > 0 Then
                For Each node In checkedMz
                    Yield DirectCast(node.Tag, Double)
                Next
            Else

            End If
        End If
    End Function

    Public Const Ion_Layers As String = "Ion Layers"
    Public Const Pinned_Pixels As String = "Pinned Pixels"

    Shared ReadOnly channelNames As New Dictionary(Of String, String) From {
        {"r", "Red"},
        {"g", "Green"},
        {"b", "Blue"}
    }

    ''' <summary>
    ''' negative value or zero means no ion selected
    ''' </summary>
    ''' <remarks>
    ''' [r,g,b] => m/z[]
    ''' </remarks>
    ReadOnly rgb As New Dictionary(Of String, TreeNode) From {
        {"r", Nothing},
        {"g", Nothing},
        {"b", Nothing}
    }

    Public ReadOnly Property Parameters As MsImageProperty
        Get
            Return PropertyGrid1.SelectedObject
        End Get
    End Property

    Private Sub frmMsImagingTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "MsImage Parameters"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)

        Win7StyleTreeView1.Nodes.Add(Ion_Layers)
        Win7StyleTreeView1.Nodes.Add(Pinned_Pixels)
        RibbonEvents.ribbonItems.TabGroupMSI.ContextAvailable = ContextAvailability.Active
    End Sub

    Public Sub ClearIons() Handles ClearSelectionToolStripMenuItem.Click, ToolStripButton3.Click
        checkedMz.Clear()
        Win7StyleTreeView1.Nodes.Item(0).Nodes.Clear()
        ' Win7StyleTreeView1.Nodes.Item(1).Nodes.Clear()
    End Sub

    Public Sub LoadPinnedIons(ions As IEnumerable(Of ms2))
        Win7StyleTreeView1.Nodes.Item(1).Nodes.Clear()

        For Each i As ms2 In ions _
            .ToArray _
            .Centroid(Tolerance.DeltaMass(0.0001), New RelativeIntensityCutoff(0.001)) _
            .OrderByDescending(Function(m) m.intensity)

            Call AddIonMzLayer(i.mz, index:=1)
        Next
    End Sub

    Private Sub checkNode(node As TreeNode)
        Static checked As TreeNode = Nothing

        If checked Is Nothing OrElse checked IsNot node Then
            checked = node
            node.Checked = True
        Else
            Return
        End If

        checkedMz.Add(node)

        If rgb.Any(Function(i) i.Value Is Nothing) Then
            For Each C As KeyValuePair(Of String, TreeNode) In rgb.ToArray
                If C.Value Is Nothing Then
                    rgb(C.Key) = node

                    If node.Text.IsNumeric OrElse node.Text.IsPattern(".+ \([rgb]\)") Then
                        node.Text = $"{CDbl(node.Tag).ToString("F4")} ({channelNames(C.Key)})"
                    End If

                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub uncheckNode(node As TreeNode)
        Static unchecked As TreeNode = Nothing

        If unchecked Is Nothing OrElse unchecked IsNot node Then
            unchecked = node
            node.Checked = False
        Else
            Return
        End If

        checkedMz.Remove(node)

        For Each C As KeyValuePair(Of String, TreeNode) In rgb.ToArray
            If C.Value Is node Then
                rgb(C.Key) = Nothing
                node.Text = CDbl(node.Tag).ToString("F4")
            End If
        Next
    End Sub

    Private Sub Win7StyleTreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterCheck
        If e.Node.Checked Then
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    Call checkNode(mz)
                Next
            Else
                Call checkNode(e.Node)
            End If
        Else
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    Call uncheckNode(mz)
                Next
            Else
                Call uncheckNode(e.Node)
            End If
        End If
    End Sub

    ''' <summary>
    ''' load m/z layer
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ViewLayerButton.Click
        If ToolStripSpringTextBox1.Text.StringEmpty Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf ToolStripSpringTextBox1.Text.IsNumeric(True) Then
            Dim mz As Double = Val(ToolStripSpringTextBox1.Text)
            Dim viewer = WindowModules.viewer

            If TypeOf viewer Is frmMsImagingViewer Then
                Call DirectCast(viewer, frmMsImagingViewer).renderByMzList({mz})
            End If
        Else
            ' formula
            Dim formula As String = ToolStripSpringTextBox1.Text
            Dim exactMass As Double = Math.EvaluateFormula(formula)

            Call Win7StyleTreeView1.Nodes.Item(0).Nodes.Clear()

            For Each type As MzCalculator In Provider.Positives
                Dim mz As Double = type.CalcMZ(exactMass)

                If mz <= 0 Then
                    Continue For
                End If

                Dim node As TreeNode = Win7StyleTreeView1.Nodes.Item(0).Nodes.Add($"{mz.ToString("F4")} {type.ToString}")

                node.Tag = mz
            Next
        End If
    End Sub

    ''' <summary>
    ''' add ion layer by m/z
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If ToolStripSpringTextBox1.Text.StringEmpty Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call AddIonMzLayer(mz:=Val(ToolStripSpringTextBox1.Text))
        End If

        ToolStripSpringTextBox1.Text = ""
    End Sub

    Private Sub AddIonMzLayer(mz As Double, Optional index As Integer = 0)
        Dim node As TreeNode = Win7StyleTreeView1.Nodes.Item(index).Nodes.Add(mz.ToString("F4"))
        node.Tag = mz
    End Sub

    ''' <summary>
    ''' 只渲染前三个选中的m/z
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RGBLayers(sender As Object, e As EventArgs) Handles RenderLayerCompositionModeToolStripMenuItem.Click
        Dim mz3 As Double() = New Double(2) {}

        If rgb.All(Function(i) i.Value Is Nothing) Then
            mz3 = {}
        Else
            Dim rgb As String() = {"r", "g", "b"}

            For i As Integer = 0 To rgb.Length - 1
                If Not Me.rgb(rgb(i)) Is Nothing Then
                    mz3(i) = Me.rgb(rgb(i)).Tag
                End If
            Next
        End If

        If mz3.Length = 0 Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf Parameters Is Nothing Then
            Call MyApplication.host.warning("MS-imaging data is not loaded yet!")
            Return
        End If

        Dim r As Double = mz3.ElementAtOrDefault(0, [default]:=-1)
        Dim g As Double = mz3.ElementAtOrDefault(1, [default]:=-1)
        Dim b As Double = mz3.ElementAtOrDefault(2, [default]:=-1)
        Dim viewer = WindowModules.viewer

        Call viewer.renderRGB(r, g, b)
    End Sub

    Private Sub PropertyGrid1_DragDrop(sender As Object, e As DragEventArgs) Handles PropertyGrid1.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim firstFile As String = files.ElementAtOrDefault(Scan0)

        If Not firstFile Is Nothing Then
            If firstFile.ExtensionSuffix("imzML") Then
                Call MyApplication.host.OpenFile(firstFile, showDocument:=True)
            ElseIf firstFile.ExtensionSuffix("CDF") Then
                Call loadRenderFromCDF(firstFile)
            Else
                Call MyApplication.host.showStatusMessage("invalid file type!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            End If
        End If
    End Sub

    Public Sub loadRenderFromCDF(firstFile As String)
        Dim pixels As PixelData()
        Dim size As Size
        Dim tolerance As Tolerance

        If viewer Is Nothing Then
            viewer = WindowModules.viewer
        End If

        Using cdf As New netCDFReader(firstFile)
            size = cdf.GetMsiDimension
            pixels = cdf.LoadPixelsData.ToArray
            tolerance = cdf.GetMzTolerance
        End Using

        viewer.LoadRender(firstFile, firstFile)
        viewer.renderByPixelsData(pixels, size)

        For Each mz As Double In pixels _
            .GroupBy(Function(p) p.mz, tolerance) _
            .Select(Function(a)
                        Return Val(a.name)
                    End Function)

            Call AddIonMzLayer(mz)
        Next
    End Sub

    Private Sub PropertyGrid1_DragEnter(sender As Object, e As DragEventArgs) Handles PropertyGrid1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub LoadBasePeakIonsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadBasePeakIonsToolStripMenuItem.Click
        Call ClearIons()

        If ServiceHub.MSIEngineRunning Then
            Dim progress As New frmProgressSpinner

            Call New Thread(Sub()
                                Call Me.Invoke(Sub() Call loadBasePeakMz())
                                Call progress.Invoke(Sub() progress.Close())
                            End Sub).Start()

            Call progress.ShowDialog()
        Else
            Call MyApplication.host.showStatusMessage("No MSI raw data file was loaded!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub loadBasePeakMz()
        Dim layers As TreeNode = Win7StyleTreeView1.Nodes.Item(0)
        Dim data As Double() = ServiceHub.LoadBasePeakMzList

        For Each p As Double In data
            layers.Nodes.Add(p.ToString("F4")).Tag = p
            Application.DoEvents()
        Next
    End Sub

    Private Sub ToolStripSpringTextBox1_Click(sender As Object, e As EventArgs) Handles ToolStripSpringTextBox1.Click
        If ToolStripSpringTextBox1.Text.IsNumeric(True) Then
            ViewLayerButton.Text = "View MS-Imaging Layer"
        Else
            ViewLayerButton.Text = "List formula ions at here"
        End If
    End Sub
End Class
