Imports System.IO
Imports System.Reflection

Public Class InstallerCustomActions

    Public Sub New()
        MyBase.New()

        '组件设计器需要此调用。
        InitializeComponent()

        '调用 InitializeComponent 后添加初始化代码

    End Sub

    Public Overrides Sub Commit(savedState As IDictionary)
        MyBase.Commit(savedState)

        Call Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
        Call Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"\{App.AssemblyName}.exe")
        Call Process.Start("http://www.biodeep.cn/")
    End Sub
End Class
