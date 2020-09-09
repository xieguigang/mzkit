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

Imports Microsoft.VisualBasic.ApplicationServices
Imports mzkit.DockSample
Imports SMRUCC.Rsharp.Interpreter
Imports REnv = SMRUCC.Rsharp.Runtime.Internal

Namespace My

    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Public Shared ReadOnly Property host As frmMain
        Public Shared ReadOnly Property LogForm As DummyOutputWindow
        Public Shared ReadOnly Property REngine As RInterpreter

        Public Shared Sub RegisterOutput(log As DummyOutputWindow)
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

        Public Shared Sub InitializeREngine()
            _REngine = New RInterpreter

            _REngine.LoadLibrary("base")
            _REngine.LoadLibrary("utils")
            _REngine.LoadLibrary("grDevices")
            _REngine.LoadLibrary("stats")

            _REngine.LoadLibrary(GetType(MyApplication))
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
        End Sub
    End Class
End Namespace
