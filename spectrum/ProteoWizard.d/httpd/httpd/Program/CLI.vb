#Region "Microsoft.VisualBasic::70724f99bda8818bffdc912e415d0d1f, ..\httpd\httpd\Program\CLI.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Parallel.Threads
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<GroupingDefine(CLI.httpdServerCLI, Description:="Server CLI for running this httpd web server.")>
<CLI> Module CLI

    Public Const httpdServerCLI$ = NameOf(httpdServerCLI)
    Public Const Utility$ = NameOf(Utility)

    <ExportAPI("/start",
               Info:="Run start the httpd web server.",
               Usage:="/start [/port 80 /wwwroot <wwwroot_DIR> /threads -1 /cache]")>
    <Argument("/port", True, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="The server port of this httpd web server to listen.")>
    <Argument("/wwwroot", True, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Description:="The website html root directory path.")>
    <Argument("/threads", True, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="The number of threads of this web server its thread pool.")>
    <Argument("/cache", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Is this server running in file system cache mode? Not recommended for open.")>
    <Group(httpdServerCLI)>
    Public Function Start(args As CommandLine) As Integer
        Dim port As Integer = args.GetValue("/port", 80)
        Dim HOME As String = args.GetValue("/wwwroot", App.CurrentDirectory)
        Dim threads As Integer = args.GetValue("/threads", -1)
        Dim cacheMode As Boolean = args.GetBoolean("/cache")

        Return New PlatformEngine(HOME, port,
                                  True,
                                  threads:=threads,
                                  cache:=cacheMode).Run
    End Function

    <ExportAPI("/run",
               Info:="Run start the web server with specific Web App.",
               Usage:="/run /dll <app.dll> [/port <80> /wwwroot <wwwroot_DIR>]")>
    <Group(httpdServerCLI)>
    Public Function RunApp(args As CommandLine) As Integer
        Dim port As Integer = args.GetValue("/port", 80)
        Dim HOME As String = args.GetValue("/wwwroot", App.CurrentDirectory)
        Dim dll As String = args.GetValue("/dll", "")
        Return New PlatformEngine(HOME, port, True, dll).Run
    End Function

    <ExportAPI("/GET",
               Info:="Tools for http get request the content of a specific url.",
               Usage:="/GET /url [<url>/std_in] [/out <file/std_out>]")>
    <Argument("/url", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Description:="The resource URL on the web.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
              AcceptTypes:={GetType(String)},
              Description:="The save location of your requested data file.")>
    Public Function [GET](args As CommandLine) As Integer
        Dim url As String = args.ReadInput("/url")

        VBDebugger.ForceSTDError = True

        Using out As StreamWriter = args.OpenStreamOutput("/out")
            Dim html As String = url.GET
            Call out.Write(html)
        End Using

        Return 0
    End Function

    <ExportAPI("/Stress.Testing",
               Info:="Using Ctrl + C to stop the stress testing.",
               Usage:="/Stress.Testing /url <target_url> [/out <out.txt>]")>
    Public Function StressTest(args As CommandLine) As Integer
        Dim url$ = args <= "/url"
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/" & url.NormalizePathString & ".txt")
        Dim test As Func(Of Integer, String) = AddressOf New __test With {
            .url = url
        }.Run

        Using result As StreamWriter = out.OpenWriter
            Do While True
                Dim pack%() = SeqRandom(10000)
                Dim returns = BatchTasks.BatchTask(pack, getTask:=test, numThreads:=1000, TimeInterval:=0)
                For Each line In returns
                    Call result.WriteLine(line)
                Next

                Call result.WriteLine()
                Call result.WriteLine("==========================================================")
                Call result.WriteLine()
                Call result.Flush()
            Loop
        End Using

        Return 0
    End Function

    Private Structure __test
        Dim url$

        Public Function Run(n%) As String
            Try
                Dim request$ = url & "?random=" & UrlEncode(StrUtils.RandomASCIIString(len:=n))
                Dim response& = Time(Sub() Call request.GET)
                Return {"len=" & n, $"response={response}ms"}.JoinBy(ASCII.TAB)
            Catch ex As Exception
                Return "error"
            End Try
        End Function
    End Structure

    ''' <summary>
    ''' 可以使用这个API来运行内部的配置API，例如调用内部的函数配置mysql链接
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/run.dll", Usage:="/run.dll /api <namespace::apiName> [....]")>
    Public Function RunDll(args As CommandLine) As Integer
        Dim api$ = args <= "/api"
        Dim run As Boolean = False

        For Each dll As String In ls - l - r - "*.dll" <= App.HOME
            Dim method As MethodInfo = RunDllEntryPoint.GetDllMethod(Assembly.LoadFile(dll), api)
#If DEBUG Then
            Call dll.__INFO_ECHO
#End If
            If Not method Is Nothing Then
                run = True
                Call method.Invoke(Nothing, Nothing)
            End If
        Next

        If Not run Then
            Call $"No dll api which is named {api} was found!".Warning
        End If

        Return 0
    End Function
End Module
