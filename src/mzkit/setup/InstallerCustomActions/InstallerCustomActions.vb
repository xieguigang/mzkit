Imports System.Configuration.Install
Imports System.IO
Imports System.Reflection

Public Class InstallerCustomActions

    Public Sub New()
        MyBase.New()

        '组件设计器需要此调用。
        InitializeComponent()

        '调用 InitializeComponent 后添加初始化代码

    End Sub

    Private Sub InstallerCustomActions_Committed(sender As Object, e As InstallEventArgs) Handles Me.Committed
        Call Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
        Call Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\mzkit_win32.exe")
        Call Process.Start("http://www.biodeep.cn/")
    End Sub
End Class
