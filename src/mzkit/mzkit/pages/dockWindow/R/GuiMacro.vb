Imports System
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace ConEmuInside
    Public Class GuiMacroException
        Inherits Exception

        Public Sub New(ByVal asMessage As String)
            MyBase.New(asMessage)
        End Sub
    End Class

    Public Class GuiMacro
        Public Enum GuiMacroResult
            ' Succeeded
            gmrOk = 0
            ' Reserved for .Net control module
            gmrPending = 1
            gmrDllNotLoaded = 2
            gmrException = 3
            ' Bad PID or ConEmu HWND was specified
            gmrInvalidInstance = 4
            ' Unknown macro execution error in ConEmu
            gmrExecError = 5
        End Enum

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Private Shared Function LoadLibrary(ByVal libname As String) As IntPtr
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto)>
        Private Shared Function FreeLibrary(ByVal hModule As IntPtr) As Boolean
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Ansi)>
        Private Shared Function GetProcAddress(ByVal hModule As IntPtr, ByVal lpProcName As String) As IntPtr
        End Function

        <UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet:=CharSet.Unicode)>
        Private Delegate Function FConsoleMain3(ByVal anWorkMode As Integer, ByVal asCommandLine As String) As Integer
        Public Delegate Sub ExecuteResult(ByVal code As GuiMacroResult, ByVal data As String)
        <UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet:=CharSet.Unicode)>
        Private Delegate Function FGuiMacro(ByVal asWhere As String, ByVal asMacro As String, <Out> ByRef bstrResult As IntPtr) As Integer
        Private libraryPathField As String
        Private ConEmuCD As IntPtr
        Private fnConsoleMain3 As FConsoleMain3
        Private fnGuiMacro As FGuiMacro

        Public ReadOnly Property LibraryPath As String
            Get
                Return libraryPathField
            End Get
        End Property

        Protected Function ExecuteLegacy(ByVal asWhere As String, ByVal asMacro As String) As String
            If ConEmuCD = IntPtr.Zero Then
                Throw New GuiMacroException("ConEmuCD was not loaded")
            End If

            If fnConsoleMain3 Is Nothing Then
                Throw New GuiMacroException("ConsoleMain3 function was not found")
            End If

            Dim cmdLine = " -GuiMacro"
            If Not String.IsNullOrEmpty(asWhere) Then cmdLine += ":" & asWhere
            cmdLine += " " & asMacro
            Environment.SetEnvironmentVariable("ConEmuMacroResult", Nothing)
            Dim result As String
            Dim iRc = fnConsoleMain3.Invoke(3, cmdLine)

            Select Case iRc
                Case 200, 201 ' CERR_CMDLINEEMPTY
                    ' CERR_CMDLINE
                    Throw New GuiMacroException("Bad command line was passed to ConEmuCD")
                Case 0, 133 ' This is expected
                    ' CERR_GUIMACRO_SUCCEEDED: not expected, but...
                    result = Environment.GetEnvironmentVariable("ConEmuMacroResult")
                    If Equals(result, Nothing) Then Throw New GuiMacroException("ConEmuMacroResult was not set")
                Case 134 ' CERR_GUIMACRO_FAILED
                    Throw New GuiMacroException("GuiMacro execution failed")
                Case Else
                    Throw New GuiMacroException(String.Format("Internal ConEmuCD error: {0}", iRc))
            End Select

            Return result
        End Function

        Protected Sub ExecuteHelper(ByVal asWhere As String, ByVal asMacro As String, ByVal aCallbackResult As ExecuteResult)
            If aCallbackResult Is Nothing Then
                Throw New GuiMacroException("aCallbackResult was not specified")
            End If

            Dim result As String
            Dim code As GuiMacroResult

            ' New ConEmu builds exports "GuiMacro" function
            If fnGuiMacro IsNot Nothing Then
                Dim fnCallback = Marshal.GetFunctionPointerForDelegate(aCallbackResult)

                If fnCallback = IntPtr.Zero Then
                    Throw New GuiMacroException("GetFunctionPointerForDelegate failed")
                End If

                Dim bstrPtr = IntPtr.Zero
                Dim iRc = fnGuiMacro.Invoke(asWhere, asMacro, bstrPtr)

                Select Case iRc
                    Case 0, 133 ' This is not expected for `GuiMacro` exported funciton, but JIC
                        ' CERR_GUIMACRO_SUCCEEDED: expected
                        code = GuiMacroResult.gmrOk
                    Case 134 ' CERR_GUIMACRO_FAILED
                        code = GuiMacroResult.gmrExecError
                    Case Else
                        Throw New GuiMacroException(String.Format("Internal ConEmuCD error: {0}", iRc))
                End Select

                If bstrPtr = IntPtr.Zero Then
                    ' Not expected, `GuiMacro` must return some BSTR in any case
                    Throw New GuiMacroException("Empty bstrPtr was returned")
                End If

                result = Marshal.PtrToStringBSTR(bstrPtr)
                Marshal.FreeBSTR(bstrPtr)

                If Equals(result, Nothing) Then
                    ' Not expected, `GuiMacro` must return some BSTR in any case
                    Throw New GuiMacroException("Marshal.PtrToStringBSTR failed")
                End If
            Else
                result = ExecuteLegacy(asWhere, asMacro)
                code = GuiMacroResult.gmrOk
            End If

            aCallbackResult(code, result)
        End Sub

        Public Function Execute(ByVal asWhere As String, ByVal asMacro As String, ByVal aCallbackResult As ExecuteResult) As GuiMacroResult
            If ConEmuCD = IntPtr.Zero Then Return GuiMacroResult.gmrDllNotLoaded
            Call New Thread(Sub()
                                ' Don't block application termination
                                Thread.CurrentThread.IsBackground = True
                                ' Start GuiMacro execution
                                Try
                                    ExecuteHelper(asWhere, asMacro, aCallbackResult)
                                Catch e As GuiMacroException
                                    aCallbackResult(GuiMacroResult.gmrException, e.Message)
                                End Try
                            End Sub).Start()
            Return GuiMacroResult.gmrPending
        End Function

        Public Sub New(ByVal asLibrary As String)
            ConEmuCD = IntPtr.Zero
            fnConsoleMain3 = Nothing
            fnGuiMacro = Nothing
            libraryPathField = asLibrary
            LoadConEmuDll(asLibrary)
        End Sub

        Protected Overrides Sub Finalize()
            UnloadConEmuDll()
        End Sub

        Private Sub LoadConEmuDll(ByVal asLibrary As String)
            If ConEmuCD <> IntPtr.Zero Then
                Return
            End If

            ConEmuCD = LoadLibrary(asLibrary)

            If ConEmuCD = IntPtr.Zero Then
                Dim errorCode As Integer = Marshal.GetLastWin32Error()
                Throw New GuiMacroException(String.Format("Can't load library, ErrCode={0}" & Microsoft.VisualBasic.Constants.vbLf & "{1}", errorCode, asLibrary))
            End If

            ' int __stdcall ConsoleMain3(int anWorkMode/*0-Server&ComSpec,1-AltServer,2-Reserved*/, LPCWSTR asCmdLine)
            Const fnNameOld = "ConsoleMain3"
            Dim ptrConsoleMain = GetProcAddress(ConEmuCD, fnNameOld)
            Const fnNameNew = "GuiMacro"
            Dim ptrGuiMacro = GetProcAddress(ConEmuCD, fnNameNew)

            If ptrConsoleMain = IntPtr.Zero AndAlso ptrGuiMacro = IntPtr.Zero Then
                UnloadConEmuDll()
                Throw New GuiMacroException(String.Format("Function {0} not found in library" & Microsoft.VisualBasic.Constants.vbLf & "{1}" & Microsoft.VisualBasic.Constants.vbLf & "Update ConEmu modules", fnNameOld, asLibrary))
            End If

            If ptrGuiMacro <> IntPtr.Zero Then
                ' To call: ExecGuiMacro.Invoke(asWhere, asCommand, callbackDelegate);
                fnGuiMacro = CType(Marshal.GetDelegateForFunctionPointer(ptrGuiMacro, GetType(FGuiMacro)), FGuiMacro)
            End If

            If ptrConsoleMain <> IntPtr.Zero Then
                ' To call: ConsoleMain3.Invoke(0, cmdline);
                fnConsoleMain3 = CType(Marshal.GetDelegateForFunctionPointer(ptrConsoleMain, GetType(FConsoleMain3)), FConsoleMain3)
            End If
        End Sub

        Private Sub UnloadConEmuDll()
            If ConEmuCD <> IntPtr.Zero Then
                FreeLibrary(ConEmuCD)
                ConEmuCD = IntPtr.Zero
            End If
        End Sub
    End Class
End Namespace
