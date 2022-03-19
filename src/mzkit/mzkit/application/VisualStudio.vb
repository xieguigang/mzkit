#Region "Microsoft.VisualBasic::d121be5fe83107dc141341d5fb1c2186, mzkit\src\mzkit\mzkit\application\VisualStudio.vb"

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

    '   Total Lines: 92
    '    Code Lines: 68
    ' Comment Lines: 6
    '   Blank Lines: 18
    '     File Size: 3.07 KB


    ' Class VisualStudio
    ' 
    '     Properties: DockPanel
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: ShowDocument
    ' 
    '     Sub: Dock, ShowProperties, ShowPropertyWindow, ShowRTerm, ShowSingleDocument
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.DockSample
Imports BioNovoGene.mzkit_win32.My
Imports WeifenLuo.WinFormsUI.Docking

Public Class VisualStudio

    Public Shared ReadOnly Property DockPanel As DockPanel
        Get
            Return MyApplication.host.dockPanel
        End Get
    End Property

    Sub New()
    End Sub

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

    Public Shared Sub ShowPropertyWindow()
        Call Dock(WindowModules.propertyWin, DockState.DockRight)
    End Sub

    Public Shared Sub ShowProperties(item As Object)
        Dim propertyWin = WindowModules.propertyWin

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

    ''' <summary>
    ''' create a new document tab page
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Shared Function ShowDocument(Of T As {New, DocumentWindow})(Optional status As DockState = DockState.Document,
                                                                       Optional title As String = Nothing) As T
        Dim newDoc As New T()

        newDoc.Show(MyApplication.host.dockPanel)
        newDoc.DockState = status

        If Not title.StringEmpty Then
            newDoc.TabText = title
        End If

        Return newDoc
    End Function

    Public Shared Sub ShowRTerm()
        WindowModules.RtermPage.Show(MyApplication.host.dockPanel)
        WindowModules.RtermPage.DockState = DockState.Document

        MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.RtermPage.Text}]"
    End Sub
End Class
