Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Drawing.Design
Imports System.Windows.Forms
Imports System.IO
Imports System.Drawing

Namespace SmileWei.EmbeddedApp
    ''' <summary>
    ''' 可以把其他窗体应用程序嵌入此容器
    ''' </summary>
    <ToolboxBitmap(GetType(AppContainer), "AppControl.bmp")>
    Public Partial Class AppContainer
        Inherits Panel

        Private appIdleAction As Action(Of Object, EventArgs) = Nothing
        Private appIdleEvent As EventHandler = Nothing

        Dim window As IntPtr

        ''' <summary>
        ''' for winform designer.
        ''' </summary>
        Private Sub New()
            Me.New(False)
        End Sub

        Public Sub New(ByVal Optional showEmbedResult As Boolean = False)
            InitializeComponent()
            Me.ShowEmbedResult = showEmbedResult
            appIdleAction = New Action(Of Object, EventArgs)(AddressOf Application_Idle)
            appIdleEvent = New EventHandler(AddressOf Application_Idle)
        End Sub

        Public Sub New(ByVal container As IContainer, ByVal Optional showEmbedResult As Boolean = False)
            container.Add(Me)
            InitializeComponent()
            Me.ShowEmbedResult = showEmbedResult
            appIdleAction = New Action(Of Object, EventArgs)(AddressOf Application_Idle)
            appIdleEvent = New EventHandler(AddressOf Application_Idle)
        End Sub

        ''' <summary>
        ''' 确保应用程序嵌入此容器
        ''' </summary>
        Private Sub Application_Idle(ByVal sender As Object, ByVal e As EventArgs)
            If AppProcess Is Nothing OrElse AppProcess.HasExited Then
                AppProcess = Nothing
                RemoveHandler Application.Idle, appIdleEvent
                Return
            End If

            If window = IntPtr.Zero Then Return
            'Application.Idle -= appIdleEvent;
            If EmbedProcess(AppProcess, window, Me) Then
                RemoveHandler Application.Idle, appIdleEvent
            End If
            'ShowWindow(AppProcess.MainWindowHandle, SW_SHOWNORMAL);
            'var parent = GetParent(AppProcess.MainWindowHandle);//你妹，不管用，全是0
            'if (parent == this.Handle)
            '{
            '    Application.Idle -= appIdleEvent;
            '}
        End Sub
        ''' <summary>
        ''' 应用程序结束运行时要清除这里的标识
        ''' </summary>
        Private Sub AppProcess_Exited(ByVal sender As Object, ByVal e As EventArgs)
            AppProcess = Nothing
        End Sub

        ''' <summary>
        ''' Close <code>AppFilename</code> 
        ''' <para>将属性<code>AppFilename</code>指向的应用程序关闭</para>
        ''' </summary>
        Public Sub [Stop]()
            If AppProcess IsNot Nothing Then ' && AppProcess.MainWindowHandle != IntPtr.Zero)
                Try
                    If Not AppProcess.HasExited Then AppProcess.Kill()
                Catch __unusedException1__ As Exception
                End Try

                AppProcess = Nothing
                embedResult = 0
            End If
        End Sub

        Protected Overrides Sub OnHandleDestroyed(ByVal e As EventArgs)
            [Stop]()
            MyBase.OnHandleDestroyed(e)
        End Sub

        Protected Overrides Sub OnResize(ByVal eventargs As EventArgs)
            If AppProcess IsNot Nothing Then
                Win32API.MoveWindow(window, 0, 0, Width, Height, True)
            End If

            MyBase.OnResize(eventargs)
        End Sub

        Protected Overrides Sub OnSizeChanged(ByVal e As EventArgs)
            Invalidate()
            MyBase.OnSizeChanged(e)
        End Sub

#Region "属性"
        ''' <summary>
        ''' Embedded application's process
        ''' </summary>
        Public Property AppProcess As Process

        ''' <summary>
        ''' 标识内嵌程序是否已经启动
        ''' </summary>
        Public ReadOnly Property IsStarted As Boolean
            Get
                Return AppProcess IsNot Nothing
            End Get
        End Property

#End Region

        ''' <summary>
        ''' 如果函数成功，返回值为子窗口的原父窗口句柄；如果函数失败，返回值为NULL。若想获得多错误信息，请调用GetLastError函数。
        ''' </summary>
        Public embedResult As Integer = 0

        ''' <summary>
        ''' 将指定的程序嵌入指定的控件
        ''' </summary>
        Public Function EmbedProcess(ByVal app As Process, window As IntPtr, Optional control As Control = Nothing) As Boolean

            If control Is Nothing Then
                control = Me
            End If

            ' Get the main handle
            If app Is Nothing OrElse window = IntPtr.Zero OrElse control Is Nothing Then Return False
            embedResult = 0

            Try
                ' Put it into this container
                embedResult = Win32API.SetParent(window, control.Handle)
            Catch __unusedException1__ As Exception
            End Try

            Try
                ' Remove border and whatnot               
                Win32API.SetWindowLong(New HandleRef(Me, window), Win32API.GWL_STYLE, Win32API.WS_VISIBLE)
            Catch __unusedException1__ As Exception
            End Try

            Try
                ' Move the window to overlay it on this window
                Win32API.MoveWindow(window, 0, 0, control.Width, control.Height, True)
            Catch __unusedException1__ As Exception
            End Try

            If ShowEmbedResult Then
                Dim errorString = Win32API.GetLastError()
                MessageBox.Show(errorString)
            End If

            Return embedResult <> 0
        End Function

        ''' <summary>
        ''' Show a MessageBox to tell whether the embedding is successfully done.
        ''' </summary>
        Public Property ShowEmbedResult As Boolean
    End Class
End Namespace
