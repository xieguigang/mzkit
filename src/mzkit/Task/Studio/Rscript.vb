#Region "Microsoft.VisualBasic::d5a6a1641d8693bb3199036962b927fd, mzkit\src\mzkit\Task\Studio\Rscript.vb"

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

    '   Total Lines: 227
    '    Code Lines: 115
    ' Comment Lines: 91
    '   Blank Lines: 21
    '     File Size: 10.95 KB


    '     Class Rscript
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Check, Compile, FromEnvironment, GetCheckCommandLine, GetCompileCommandLine
    '                   GetparallelModeCommandLine, GetslaveModeCommandLine, parallelMode, slaveMode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\App\Rscript.exe

' 
'  // 
'  // R# scripting host
'  // 
'  // VERSION:   1.99.7833.40504
'  // ASSEMBLY:  Rscript, Version=1.99.7833.40504, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics  2020
'  // GUID:      16d477b1-e7fb-41eb-9b61-7ea75c5d2939
'  // BUILT:     6/11/2021 10:05:36 AM
'  // 
' 
' 
'  < RscriptCommandLine.CLI >
' 
' 
' SYNOPSIS
' Rscript command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  --build:        build R# package
'  --check:        Verify a packed R# package is damaged or not?
'  --parallel:     Create a new parallel thread process for running a new parallel task.
'  --slave:        Create a R# cluster node for run background or parallel task. This IPC command will
'                  run a R# script file that specified by the ``/exec`` argument, and then post back
'                  the result data json to the specific master listener.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Rscript ??<commandName>" for getting more details command help.
'    2. Using command "Rscript /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Rscript /i" for enter interactive console mode.

Namespace CLI


    ''' <summary>
    ''' RscriptCommandLine.CLI
    ''' </summary>
    '''
    Public Class Rscript : Inherits InteropService

        Public Const App$ = "Rscript.exe"

        Sub New(App$)
            MyBase._executableAssembly = App$
        End Sub

        ''' <summary>
        ''' Create an internal CLI pipeline invoker from a given environment path. 
        ''' </summary>
        ''' <param name="directory">A directory path that contains the target application</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromEnvironment(directory As String) As Rscript
            Return New Rscript(App:=directory & "/" & Rscript.App)
        End Function

        ''' <summary>
        ''' ```bash
        ''' --build [/src &lt;folder, default=./&gt; /save &lt;Rpackage.zip&gt;]
        ''' ```
        ''' build R# package
        ''' </summary>
        '''
        ''' <param name="src"> A folder path that contains the R source files and meta data files of the target R package, 
        '''               a folder that exists in this folder path which is named &apos;R&apos; is required!
        ''' </param>
        Public Function Compile(Optional src As String = "./", Optional save As String = "") As Integer
            Dim cli = GetCompileCommandLine(src:=src, save:=save, internal_pipelineMode:=True)
            Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
            Return proc.Run()
        End Function
        Public Function GetCompileCommandLine(Optional src As String = "./", Optional save As String = "", Optional internal_pipelineMode As Boolean = True) As String
            Dim CLI As New StringBuilder("--build")
            Call CLI.Append(" ")
            If Not src.StringEmpty Then
                Call CLI.Append("/src " & """" & src & """ ")
            End If
            If Not save.StringEmpty Then
                Call CLI.Append("/save " & """" & save & """ ")
            End If
            Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


            Return CLI.ToString()
        End Function

        ''' <summary>
        ''' ```bash
        ''' --check --target &lt;package.zip&gt;
        ''' ```
        ''' Verify a packed R# package is damaged or not?
        ''' </summary>
        '''

        Public Function Check(target As String) As Integer
            Dim cli = GetCheckCommandLine(target:=target, internal_pipelineMode:=True)
            Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
            Return proc.Run()
        End Function
        Public Function GetCheckCommandLine(target As String, Optional internal_pipelineMode As Boolean = True) As String
            Dim CLI As New StringBuilder("--check")
            Call CLI.Append(" ")
            Call CLI.Append("--target " & """" & target & """ ")
            Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


            Return CLI.ToString()
        End Function

        ''' <summary>
        ''' ```bash
        ''' --parallel --master &lt;master_port&gt; [--delegate &lt;delegate_name&gt;]
        ''' ```
        ''' Create a new parallel thread process for running a new parallel task.
        ''' </summary>
        '''
        ''' <param name="master"> the TCP port of the master node.
        ''' </param>
        Public Function parallelMode(master As String, Optional [delegate] As String = "") As Integer
            Dim cli = GetparallelModeCommandLine(master:=master, [delegate]:=[delegate], internal_pipelineMode:=True)
            Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
            Return proc.Run()
        End Function
        Public Function GetparallelModeCommandLine(master As String, Optional [delegate] As String = "", Optional internal_pipelineMode As Boolean = True) As String
            Dim CLI As New StringBuilder("--parallel")
            Call CLI.Append(" ")
            Call CLI.Append("--master " & """" & master & """ ")
            If Not [delegate].StringEmpty Then
                Call CLI.Append("--delegate " & """" & [delegate] & """ ")
            End If
            Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


            Return CLI.ToString()
        End Function

        ''' <summary>
        ''' ```bash
        ''' --slave /exec &lt;script.R&gt; /args &lt;json_base64&gt; /request-id &lt;request_id&gt; /PORT=&lt;port_number&gt; [--debug /timeout=&lt;timeout in ms, default=1000&gt; /retry=&lt;retry_times, default=5&gt; /MASTER=&lt;ip, default=localhost&gt; /entry=&lt;function_name, default=NULL&gt;]
        ''' ```
        ''' Create a R# cluster node for run background or parallel task. This IPC command will run a R# script file that specified by the ``/exec`` argument, and then post back the result data json to the specific master listener.
        ''' </summary>
        '''
        ''' <param name="exec"> a specific R# script for run
        ''' </param>
        ''' <param name="args"> The base64 text of the input arguments for running current R# script file, this is a json encoded text of the arguments. the json object should be a collection of [key =&gt; value[]] pairs.
        ''' </param>
        ''' <param name="entry"> the entry function name, by default is running the script from the begining to ends.
        ''' </param>
        ''' <param name="request_id"> the unique id for identify current slave progress in the master node when invoke post data callback.
        ''' </param>
        ''' <param name="MASTER"> the ip address of the master node, by default this parameter value is ``localhost``.
        ''' </param>
        ''' <param name="PORT"> the port number for master node listen to this callback post data.
        ''' </param>
        ''' <param name="retry"> How many times that this cluster node should retry to send callback data if the TCP request timeout.
        ''' </param>
        Public Function slaveMode(exec As String,
                                     args As String,
                                     request_id As String,
                                     PORT As String,
                                     Optional timeout As String = "1000",
                                     Optional retry As String = "5",
                                     Optional master As String = "localhost",
                                     Optional entry As String = "NULL",
                                     Optional debug As Boolean = False) As Integer
            Dim cli = GetslaveModeCommandLine(exec:=exec,
                                         args:=args,
                                         request_id:=request_id,
                                         PORT:=PORT,
                                         timeout:=timeout,
                                         retry:=retry,
                                         master:=master,
                                         entry:=entry,
                                         debug:=debug, internal_pipelineMode:=True)
            Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
            Return proc.Run()
        End Function
        Public Function GetslaveModeCommandLine(exec As String,
                                     args As String,
                                     request_id As String,
                                     PORT As String,
                                     Optional timeout As String = "1000",
                                     Optional retry As String = "5",
                                     Optional master As String = "localhost",
                                     Optional entry As String = "NULL",
                                     Optional debug As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
            Dim CLI As New StringBuilder("--slave")
            Call CLI.Append(" ")
            Call CLI.Append("/exec " & """" & exec & """ ")
            Call CLI.Append("/args " & """" & args & """ ")
            Call CLI.Append("/request-id " & """" & request_id & """ ")
            Call CLI.Append("/PORT " & """" & PORT & """ ")
            If Not timeout.StringEmpty Then
                Call CLI.Append("/timeout " & """" & timeout & """ ")
            End If
            If Not retry.StringEmpty Then
                Call CLI.Append("/retry " & """" & retry & """ ")
            End If
            If Not master.StringEmpty Then
                Call CLI.Append("/master " & """" & master & """ ")
            End If
            If Not entry.StringEmpty Then
                Call CLI.Append("/entry " & """" & entry & """ ")
            End If
            If debug Then
                Call CLI.Append("--debug ")
            End If
            Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


            Return CLI.ToString()
        End Function
    End Class
End Namespace
