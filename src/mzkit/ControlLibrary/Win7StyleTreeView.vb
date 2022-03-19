#Region "Microsoft.VisualBasic::df8bfd896274aa6266677470123bcced, mzkit\src\mzkit\ControlLibrary\Win7StyleTreeView.vb"

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

    '   Total Lines: 63
    '    Code Lines: 50
    ' Comment Lines: 3
    '   Blank Lines: 10
    '     File Size: 2.61 KB


    '     Class Win7StyleTreeView
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Class Helper
    ' 
    '             Function: SendMessage, SetWindowTheme, TreeViewGetExtendedStyle
    ' 
    '             Sub: ApplyTreeViewThemeStyles, TreeViewSetExtendedStyle
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace Kesoft.Windows.Forms.Win7StyleTreeView
    ''' <summary>
    ''' Windows 7 style TreeView control.
    ''' </summary>
    Public Partial Class Win7StyleTreeView
        Inherits TreeView

        Public Sub New()
            Me.InitializeComponent()
            Helper.ApplyTreeViewThemeStyles(Me)
        End Sub

        Public Sub New(container As IContainer)
            container.Add(Me)
            Me.InitializeComponent()
            Helper.ApplyTreeViewThemeStyles(Me)
        End Sub

        Friend Class Helper
            Private Const TvFirst As Integer = &H1100
            Private Const TvmSetextendedstyle As Integer = TvFirst + 44
            Private Const TvmGetextendedstyle As Integer = TvFirst + 45
            Private Const TvsExFadeinoutexpandos As Integer = &H0040
            Private Const TvsExDoublebuffer As Integer = &H0004

            <DllImport("uxtheme.dll", CharSet:=CharSet.Auto)>
            Private Shared Function SetWindowTheme(hWnd As IntPtr, subAppName As String, subIdList As String) As Integer
            End Function

            <DllImport("user32.dll", CharSet:=CharSet.Auto)>
            Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
            End Function

            Private Shared Function TreeViewGetExtendedStyle(handle As IntPtr) As Integer
                Dim ptr = SendMessage(handle, TvmGetextendedstyle, IntPtr.Zero, IntPtr.Zero)
                Return ptr.ToInt32()
            End Function

            Private Shared Sub TreeViewSetExtendedStyle(handle As IntPtr, extendedStyle As Integer, mask As Integer)
                SendMessage(handle, TvmSetextendedstyle, New IntPtr(mask), New IntPtr(extendedStyle))
            End Sub

            Public Shared Sub ApplyTreeViewThemeStyles(treeView As TreeView)
                If treeView Is Nothing Then
                    Throw New ArgumentNullException("treeView")
                End If

                treeView.HotTracking = True
                treeView.ShowLines = False
                Dim hwnd = treeView.Handle
                SetWindowTheme(hwnd, "Explorer", Nothing)
                Dim exstyle = TreeViewGetExtendedStyle(hwnd)
                exstyle = exstyle Or TvsExDoublebuffer Or TvsExFadeinoutexpandos
                TreeViewSetExtendedStyle(hwnd, exstyle, 0)
            End Sub
        End Class
    End Class
End Namespace
