#Region "Microsoft.VisualBasic::1bd83a5e03c210720891ac7253b9812f, mzkit\src\mzkit\mzkit\Win32API.vb"

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

    '   Total Lines: 155
    '    Code Lines: 100
    ' Comment Lines: 33
    '   Blank Lines: 22
    '     File Size: 7.40 KB


    '     Class Win32API
    ' 
    '         Function: AllocConsole, FindWindow, FormatMessage, FreeConsole, GetConsoleWindow
    '                   GetLastError, GetLastErrorString, GetParent, GetWindowLong, GetWindowThreadProcessId
    '                   MoveWindow, PostMessage, SetParent, SetWindowLong, SetWindowLongPtr32
    '                   SetWindowLongPtr64, SetWindowPos, ShowWindow
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.Runtime.InteropServices

Namespace SmileWei.EmbeddedApp
    Public Class Win32API

        <DllImport("kernel32.dll")>
        Public Shared Function AllocConsole() As Boolean
        End Function

        <DllImport("kernel32.dll")>
        Public Shared Function FreeConsole() As Boolean
        End Function

        <DllImport("kernel32.dll")>
        Public Shared Function GetConsoleWindow() As IntPtr
        End Function

#Region "Win32 API"
        <DllImport("user32.dll", EntryPoint:="GetWindowThreadProcessId", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function GetWindowThreadProcessId(ByVal hWnd As Long, ByVal lpdwProcessId As Long) As Long
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function SetParent(ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As Integer
        End Function

        <DllImport("user32.dll", EntryPoint:="GetWindowLongA", SetLastError:=True)>
        Public Shared Function GetWindowLong(ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Long
        End Function

        Public Shared Function SetWindowLong(ByVal hWnd As HandleRef, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As IntPtr
            If IntPtr.Size = 4 Then
                Return SetWindowLongPtr32(hWnd, nIndex, dwNewLong)
            End If

            Return SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
        End Function

        <DllImport("user32.dll", EntryPoint:="SetWindowLong", CharSet:=CharSet.Auto)>
        Public Shared Function SetWindowLongPtr32(ByVal hWnd As HandleRef, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As IntPtr
        End Function

        <DllImport("user32.dll", EntryPoint:="SetWindowLongPtr", CharSet:=CharSet.Auto)>
        Public Shared Function SetWindowLongPtr64(ByVal hWnd As HandleRef, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function SetWindowPos(ByVal hwnd As IntPtr, ByVal hWndInsertAfter As Long, ByVal x As Long, ByVal y As Long, ByVal cx As Long, ByVal cy As Long, ByVal wFlags As Long) As Long
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function MoveWindow(ByVal hwnd As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal repaint As Boolean) As Boolean
        End Function

        <DllImport("user32.dll", EntryPoint:="PostMessageA", SetLastError:=True)>
        Public Shared Function PostMessage(ByVal hwnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As UInteger, ByVal lParam As UInteger) As Boolean
        End Function

        ''' <summary>
        ''' 获取系统错误信息描述
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetLastError() As String
            Dim errCode = Marshal.GetLastWin32Error()
            Dim tempptr = IntPtr.Zero
            Dim msg As String = Nothing
            FormatMessage(&H1300, tempptr, errCode, 0, msg, 255, tempptr)
            Return msg
        End Function
        ''' <summary>
        ''' 获取系统错误信息描述
        ''' </summary>
        ''' <param name="errCode">系统错误码</param>
        ''' <returns></returns>
        Public Shared Function GetLastErrorString(ByVal errCode As Integer) As String
            Dim tempptr = IntPtr.Zero
            Dim msg As String = Nothing
            FormatMessage(&H1300, tempptr, errCode, 0, msg, 255, tempptr)
            Return msg
        End Function

        <DllImport("Kernel32.dll")>
        Public Shared Function FormatMessage(ByVal flag As Integer, ByRef source As IntPtr, ByVal msgid As Integer, ByVal langid As Integer, ByRef buf As String, ByVal size As Integer, ByRef args As IntPtr) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function GetParent(ByVal hwnd As IntPtr) As IntPtr
        End Function
        '''' <summary>
        '''' ShellExecute(IntPtr.Zero, "Open", "C:/Program Files/TTPlayer/TTPlayer.exe", "", "", 1);
        '''' </summary>
        '''' <param name="hwnd"></param>
        '''' <param name="lpOperation"></param>
        '''' <param name="lpFile"></param>
        '''' <param name="lpParameters"></param>
        '''' <param name="lpDirectory"></param>
        '''' <param name="nShowCmd"></param>
        '''' <returns></returns>
        '[DllImport("shell32.dll", EntryPoint = "ShellExecute")]
        'public static extern int ShellExecute(
        'IntPtr hwnd,
        'string lpOperation,
        'string lpFile,
        'string lpParameters,
        'string lpDirectory,
        'int nShowCmd
        ');
        '[DllImport("kernel32.dll")]
        'public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId); 
        <DllImport("user32.dll", EntryPoint:="ShowWindow", SetLastError:=True)>
        Public Shared Function ShowWindow(ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Boolean
        End Function

        Public Const SWP_NOOWNERZORDER As Integer = &H200
        Public Const SWP_NOREDRAW As Integer = &H8
        Public Const SWP_NOZORDER As Integer = &H4
        Public Const SWP_SHOWWINDOW As Integer = &H40
        Public Const WS_EX_MDICHILD As Integer = &H40
        Public Const SWP_FRAMECHANGED As Integer = &H20
        Public Const SWP_NOACTIVATE As Integer = &H10
        Public Const SWP_ASYNCWINDOWPOS As Integer = &H4000
        Public Const SWP_NOMOVE As Integer = &H2
        Public Const SWP_NOSIZE As Integer = &H1
        Public Const GWL_STYLE As Integer = -16
        Public Const WS_VISIBLE As Integer = &H10000000
        Public Const WM_CLOSE As Integer = &H10
        Public Const WS_CHILD As Integer = &H40000000
        Public Const SW_HIDE As Integer = 0 '{隐藏, 并且任务栏也没有最小化图标}
        Public Const SW_SHOWNORMAL As Integer = 1 '{用最近的大小和位置显示, 激活}
        Public Const SW_NORMAL As Integer = 1 '{同 SW_SHOWNORMAL}
        Public Const SW_SHOWMINIMIZED As Integer = 2 '{最小化, 激活}
        Public Const SW_SHOWMAXIMIZED As Integer = 3 '{最大化, 激活}
        Public Const SW_MAXIMIZE As Integer = 3 '{同 SW_SHOWMAXIMIZED}
        Public Const SW_SHOWNOACTIVATE As Integer = 4 '{用最近的大小和位置显示, 不激活}
        Public Const SW_SHOW As Integer = 5 '{同 SW_SHOWNORMAL}
        Public Const SW_MINIMIZE As Integer = 6 '{最小化, 不激活}
        Public Const SW_SHOWMINNOACTIVE As Integer = 7 '{同 SW_MINIMIZE}
        Public Const SW_SHOWNA As Integer = 8 '{同 SW_SHOWNOACTIVATE}
        Public Const SW_RESTORE As Integer = 9 '{同 SW_SHOWNORMAL}
        Public Const SW_SHOWDEFAULT As Integer = 10 '{同 SW_SHOWNORMAL}
        Public Const SW_MAX As Integer = 10 '{同 SW_SHOWNORMAL}

        'const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        'const int PROCESS_VM_READ = 0x0010;
        'const int PROCESS_VM_WRITE = 0x0020;     
#End Region


    End Class
End Namespace
