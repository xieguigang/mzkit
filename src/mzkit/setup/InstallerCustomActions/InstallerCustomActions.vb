#Region "Microsoft.VisualBasic::da420d83450f2f01c0395bd86f86b004, mzkit\src\mzkit\setup\InstallerCustomActions\InstallerCustomActions.vb"

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

    '   Total Lines: 22
    '    Code Lines: 14
    ' Comment Lines: 2
    '   Blank Lines: 6
    '     File Size: 739.00 B


    ' Class InstallerCustomActions
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: InstallerCustomActions_Committed
    ' 
    ' /********************************************************************************/

#End Region

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
