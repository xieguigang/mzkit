#Region "Microsoft.VisualBasic::b76a7466309f21b2cfa7d9168391a378, mzkit\src\mzkit\mzkit\forms\frmSplashScreen.vb"

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

    '   Total Lines: 86
    '    Code Lines: 57
    ' Comment Lines: 11
    '   Blank Lines: 18
    '     File Size: 3.47 KB


    ' Class frmSplashScreen
    ' 
    '     Properties: isAboutScreen
    ' 
    '     Sub: frmSplashScreen_Deactivate, frmSplashScreen_KeyDown, frmSplashScreen_Load, frmSplashScreen_LostFocus, frmSplashScreen_Paint
    '          LinkLabel1_LinkClicked, UpdateInformation
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports BioNovoGene.mzkit_win32.My

Public Class frmSplashScreen

    Public Property isAboutScreen As Boolean = False

    Private Sub frmSplashScreen_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        If isAboutScreen Then
            Call Me.Close()
        End If
    End Sub

    Private Sub frmSplashScreen_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus

    End Sub

    Public Sub UpdateInformation(message As String)
        Invoke(Sub() Information.Text = message)
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Process.Start("http://mzkit.org/")
    End Sub

    Private Sub frmSplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DoubleBuffered = True

        Label3.Text = Label3.Text.Replace("%s", My.User.Name)
        Label4.Text = "Built: " & GetType(MyApplication).Assembly.FromAssembly.AssemblyVersion
        Label4.Location = New Point(Width - Label4.Width - 5, Label4.Location.Y)

        If (Not isAboutScreen) AndAlso (Not Globals.Settings.licensed) Then
            Globals.loadedSettings = True

            If New frmUserAgreement().ShowDialog() = DialogResult.Cancel Then
                App.Exit()
            End If

            Globals.loadedSettings = False
        End If
    End Sub

    Private Sub frmSplashScreen_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawRectangle(New Pen(Color.Black, 1), New Rectangle(0, 0, Width - 1, Height - 1))
    End Sub

    ''' <summary>
    ''' photoshop怎么重置？ps使用的时候出现了问题，想要恢复默认设置，ps该怎么初始化呢？下面我们就来看看ps恢复默认设置的教程，需要的朋友可以参考下
    ''' 
    ''' 很多photoshop用着用着会出现很多问题，一时两时设置不好，最简单的办法就是把PS初始化，这样PS就会回归的安装时的设置。
    ''' 
    ''' 1、点启动PS时，双击PS打开程序之时，快速的按住shift+ctrl+alt
    ''' 2、会弹出【是否要删除Adobe Photoshop设置文件】窗口，点击【是（Y）】
    ''' 3，然后会继续进入PS，进入后的PS就是回归到刚安装好的PS界面，包括插件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub frmSplashScreen_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown,
        Information.KeyDown,
        Label3.KeyDown,
        Label4.KeyDown,
        Panel1.KeyDown,
        PictureBox1.KeyDown,
        PictureBox2.KeyDown

        If isAboutScreen Then
            Return
        ElseIf Globals.loadedSettings Then
            Return
        End If

        If e.Alt AndAlso e.Control AndAlso e.Shift Then
            Globals.loadedSettings = True

            If MessageBox.Show("Delete the BioNovoGene Mzkit_Win32 Settings File?", "BioNovoGene Mzkit_Win32", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.OK Then
                Globals.defaultWorkspace.DeleteFile
                Globals.Settings?.workspaceFile?.DeleteFile
                Globals.Settings.Reset()
                Globals.Settings.Save()
            End If

            Globals.loadedSettings = False
        End If
    End Sub
End Class
