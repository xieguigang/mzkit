#Region "Microsoft.VisualBasic::4d3f6ba5b3e9471ead1cc0621e43f93b, src\mzkit\mzkit\application\ApplicationEvents.vb"

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

'     Class MyApplication
' 
'         Properties: host, LogForm, REngine
' 
'         Sub: InitializeREngine, LogText, MyApplication_UnhandledException, RegisterHost, RegisterOutput
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Windows.Forms
Imports mzkit.DockSample
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RProgram = SMRUCC.Rsharp.Interpreter.Program

Namespace My

    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Public Shared ReadOnly Property host As frmMain
        Public Shared ReadOnly Property LogForm As OutputWindow
        Public Shared ReadOnly Property REngine As RInterpreter

        Shared WithEvents console As Console
        Shared Rtask As Thread
        Shared cancel As New ManualResetEvent(initialState:=False)

        Public Shared ReadOnly TaskQueue As New ThreadQueue

#Region "mzkit"
        Public Shared ReadOnly Property mzkitRawViewer As PageMzkitTools
            Get
                Return _host.mzkitTool
            End Get
        End Property
#End Region

#Region "dock windows"
        Public Shared ReadOnly Property fileExplorer As frmFileExplorer
            Get
                Return _host.fileExplorer
            End Get
        End Property

        Public Shared ReadOnly Property featureExplorer As frmRawFeaturesList
            Get
                Return _host.rawFeaturesList
            End Get
        End Property

        Public Shared ReadOnly Property RtermPage As frmRsharp
            Get
                Return _host.RtermPage
            End Get
        End Property

#End Region

        Public Shared Sub RegisterOutput(log As OutputWindow)
            _LogForm = log

            Microsoft.VisualBasic.My.Log4VB.redirectError =
                Sub(header, message, level)
                    log.Invoke(Sub() log.AppendMessage($"[{header} {level.ToString}] {message}"))
                End Sub
            Microsoft.VisualBasic.My.Log4VB.redirectWarning =
                Sub(header, message, level)
                    log.Invoke(Sub() log.AppendMessage($"[{header} {level.ToString}] {message}"))
                End Sub
        End Sub

        Public Shared Sub RegisterConsole(console As Console)
            _console = console
        End Sub

        Public Shared Function GetSplashScreen() As frmSplashScreen
            For i As Integer = 0 To Application.OpenForms.Count - 1
                If TypeOf Application.OpenForms(i) Is frmSplashScreen Then
                    Return DirectCast(Application.OpenForms(i), frmSplashScreen)
                End If
            Next

            Return Nothing
        End Function

        Public Shared Sub ExecuteRScript(scriptText As String, isFile As Boolean)
            Dim result As Object

            Using buffer As New MemoryStream
                Using writer As New StreamWriter(buffer)
                    MyApplication.REngine.RedirectOutput(writer, OutputEnvironments.Html)

                    If isFile Then
                        result = MyApplication.REngine.Source(scriptText)
                    Else
                        result = MyApplication.REngine.Evaluate(scriptText)
                    End If

                    Call REngine.Print(RInterpreter.lastVariableName)

                    writer.Flush()
                    console.WriteLine(Encoding.UTF8.GetString(buffer.ToArray))
                End Using
            End Using

            Call handleResult(result)
        End Sub

        Private Shared Sub handleResult(result As Object)
            If TypeOf result Is Message AndAlso DirectCast(result, Message).level = MSG_TYPES.ERR Then
                Dim err As Message = result

                console.WriteLine(err.ToString & vbCrLf)

                For i As Integer = 0 To err.message.Length - 1
                    console.WriteLine((i + 1) & ". " & err.message(i))
                Next

                console.WriteLine(vbCrLf)

                For Each stack In err.environmentStack
                    console.WriteLine(stack.ToString)
                Next

                console.WriteLine(vbCrLf)
            End If
        End Sub

        Public Shared Sub InitializeREngine()
            Dim Rcore = GetType(RInterpreter).Assembly.FromAssembly
            Dim framework = GetType(App).Assembly.FromAssembly

            Call console.WriteLine($"
   , __           | 
  /|/  \  |  |    | Documentation: https://r_lang.dev.SMRUCC.org/
   |___/--+--+--  |
   | \  --+--+--  | Version {Rcore.AssemblyVersion} ({Rcore.BuiltTime.ToString})
   |  \_/ |  |    | sciBASIC.NET Runtime: {framework.AssemblyVersion}         
                  
Welcome to the R# language
")
            Call console.WriteLine("Type 'demo()' for some demos, 'help()' for on-line help, or
'help.start()' for an HTML browser interface to help.
Type 'q()' to quit R.
")

            _REngine = New RInterpreter

            _REngine.strict = False

            _REngine.LoadLibrary("base")
            _REngine.LoadLibrary("utils")
            _REngine.LoadLibrary("grDevices")
            _REngine.LoadLibrary("stats")

            _REngine.LoadLibrary(GetType(MyApplication))

            AddHandler console.CancelKeyPress,
                Sub()
                    ' ctrl + C just break the current executation
                    ' not exit program running
                    cancel.Set()

                    If Not Rtask Is Nothing Then
                        Rtask.Abort()
                    End If
                End Sub

            Call New Thread(AddressOf New Shell(New PS1("> "), AddressOf doRunScriptWithSpecialCommand, dev:=console) With {.Quite = "!.R#::quit" & Rnd()}.Run).Start()
        End Sub

        Private Shared Sub doRunScriptWithSpecialCommand(script As String)
            Select Case script
                Case "CLS"
                    Call console.Clear()
                Case Else
                    If Not script.StringEmpty Then
                        Rtask = New Thread(Sub() Call doRunScript(script))
                        Rtask.Start()

                        ' block the running task thread at here
                        cancel.Reset()
                        cancel.WaitOne()
                    Else
                        console.WriteLine()
                    End If
            End Select
        End Sub

        Private Shared Sub doRunScript(script As String)
            Call ExecuteRScript(script, isFile:=False)
            Call cancel.Set()
        End Sub

        Public Overloads Shared Sub LogText(msg As String)
            LogForm.AppendMessage(msg)
        End Sub

        Public Shared Sub RegisterHost(host As frmMain)
            MyApplication._host = host
        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs) Handles Me.UnhandledException
            Call App.LogException(e.Exception)
            Call MessageBox.Show(e.Exception.ToString, "Unknown Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)

            Try
                Call host.SaveSettings()
                Call fileExplorer.SaveFileCache(Sub()
                                                    ' do nothing
                                                End Sub)
            Catch ex As Exception

            End Try

            End
        End Sub
    End Class
End Namespace
