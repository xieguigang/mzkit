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
