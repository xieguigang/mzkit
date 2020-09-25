#Region "Microsoft.VisualBasic::829e095c071c5a4ac89724293d4a78ec, src\mzkit\mzkit\pages\dockWindow\frmRsharp.vb"

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

' Class frmRsharp
' 
'     Sub: frmRsharp_Closing, frmRsharp_Load
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.Language.UnixBash
Imports mzkit.ConEmuInside
Imports mzkit.SmileWei.EmbeddedApp

Public Class frmRsharp

    Protected ConEmu As Process
    Protected guiMacro As GuiMacro

    Private Sub frmRsharp_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmRsharp_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "R# Terminal"

        Me.Icon = My.Resources.Rscript

        Me.ShowIcon = True


    End Sub

    Private Function GetConEmu() As String
        Dim sOurDir As String = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
        Dim sSearchIn As String() = {Directory.GetCurrentDirectory(), sOurDir, Path.Combine(sOurDir, ".."), Path.Combine(sOurDir, "ConEmu"), "%PATH%", "%REG%"}
        Dim sNames As String()
        sNames = New String() {"ConEmu.exe", "ConEmu64.exe"}

        For Each sd In sSearchIn

            For Each sn In sNames
                Dim spath As String

                If Equals(sd, "%PATH%") OrElse Equals(sd, "%REG%") Then
                    spath = sn 'TODO
                Else
                    spath = Path.Combine(sd, sn)
                End If

                If File.Exists(spath) Then Return spath
            Next
        Next

        ' Default
        Return "ConEmu.exe"
    End Function

    Private Function GetConEmuExe() As String
        Dim bExeLoaded = False
        Dim lsConEmuExe As String = Nothing

        While Not bExeLoaded AndAlso ConEmu IsNot Nothing AndAlso Not ConEmu.HasExited

            Try
                lsConEmuExe = ConEmu.Modules(0).FileName
                bExeLoaded = True
            Catch __unusedWin32Exception1__ As ComponentModel.Win32Exception
                Thread.Sleep(50)
            End Try
        End While

        Return lsConEmuExe
    End Function

    ' Returns Path to "ConEmuCD[64].dll" (to GuiMacro execution)
    Private Function GetConEmuCD() As String
        ' Query real (full) path of started executable
        Dim lsConEmuExe As String = GetConEmuExe()
        If Equals(lsConEmuExe, Nothing) Then Return Nothing

        ' Determine bitness of **our** process
        Dim lsDll = If(IntPtr.Size = 8, "ConEmuCD64.dll", "ConEmuCD.dll")

        ' Ready to find the library
        Dim lsExeDir, ConEmuCD As String
        lsExeDir = Path.GetDirectoryName(lsConEmuExe)
        ConEmuCD = Path.Combine(lsExeDir, "ConEmu\" & lsDll)

        If Not File.Exists(ConEmuCD) Then
            ConEmuCD = Path.Combine(lsExeDir, lsDll)

            If Not File.Exists(ConEmuCD) Then
                ConEmuCD = lsDll ' Must not get here actually
            End If
        End If

        Return ConEmuCD
    End Function

    Private Sub ExecuteGuiMacro(ByVal asMacro As String)
        ' conemuc.exe -silent -guimacro:1234 print("\e","git"," --version","\n")
        Dim ConEmuCD As String = GetConEmuCD()

        If Equals(ConEmuCD, Nothing) Then
            Throw New GuiMacroException("ConEmuCD must not be null")
        End If

        If guiMacro IsNot Nothing AndAlso Not Equals(guiMacro.LibraryPath, ConEmuCD) Then
            guiMacro = Nothing
        End If

        Try
            If guiMacro Is Nothing Then guiMacro = New GuiMacro(ConEmuCD)
            guiMacro.Execute(ConEmu.Id.ToString(), asMacro, Sub(ByVal code, ByVal data) Debugger.Log(0, "GuiMacroResult", "code=" & code.ToString() & "; data=" & data & Microsoft.VisualBasic.Constants.vbLf))
        Catch e As GuiMacroException
            MessageBox.Show(e.Message, "GuiMacroException", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub startBtn_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim sRunAs, sRunArgs As String
        Dim argDirectory = App.HOME
        Dim argConEmuExe As String = GetConEmu()
        Dim argXmlFile As String = $"{argConEmuExe.ParentPath}/ConEmu.xml"

        ' Show terminal panel, hide start options
        'RefreshControls(true);

        sRunAs = ""
        '" -cmd " + // This one MUST be the last switch
        'argCmdLine.Text + sRunAs // And the shell command line itself
        sRunArgs = " -NoKeyHooks" & " -InsideWnd 0x" & termPanel.Handle.ToString("X") & " -LoadCfgFile """ & argXmlFile & """" & " -Dir """ & argDirectory & """" & "" & " -detached"

        Try
            ' Start ConEmu
            ConEmu = Process.Start(argConEmuExe, sRunArgs)
            ExecuteGuiMacro("Shell(""new_console"", """", """ & ("R#" & sRunAs).Replace("""", "\""") & """)")
        Catch ex As ComponentModel.Win32Exception
            MessageBox.Show(ex.Message & Microsoft.VisualBasic.Constants.vbCrLf & Microsoft.VisualBasic.Constants.vbCrLf & "Command:" & Microsoft.VisualBasic.Constants.vbCrLf & argConEmuExe & Microsoft.VisualBasic.Constants.vbCrLf & Microsoft.VisualBasic.Constants.vbCrLf & "Arguments:" & Microsoft.VisualBasic.Constants.vbCrLf & sRunArgs, ex.GetType().FullName & " (" & ex.NativeErrorCode.ToString() & ")", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
    End Sub
End Class

