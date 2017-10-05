#Region "Microsoft.VisualBasic::8a37df07c652bb4ae3af9007b27110bb, ..\httpd\Restarter\Program.vb"

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

Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Triggers
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

    Sub New()
        Call Ini.Load()
    End Sub

    Sub Main()
        Call GetType(Program).RunCLI(App.CommandLine)
    End Sub

    <ExportAPI("/time", Usage:="/time <hh:mm>")>
    Public Function RestartByTime(args As CommandLine) As Integer
        Dim time As String = args.SingleValue
        Dim t As String() = time.Split(":"c)
        Dim timer As New DailyTimerTrigger(CInt(t(0)), CInt(t(1)), AddressOf Restart,)

        Call timer.Start()
        Call Restart()

        Do While True
            Call Console.ReadLine()
            Call Restart()
        Loop

        Return True
    End Function

    Dim process As Process

    Public Sub Restart()
        If Not process Is Nothing Then
            Try
                Call process.Kill()
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        End If

        Dim settings As Ini = Ini.Load
        process = Process.Start(settings.Exe, settings.CLI)
    End Sub

    <ExportAPI("/interval", Usage:="/interval <hh:mm>")>
    Public Function RestartByInterval(args As CommandLine) As Integer
        Dim time As String = args.SingleValue
        Dim t As String() = time.Split(":"c)
        Dim ticks As Integer = UpdateThread.GetTicks(CInt(t(0)), CInt(t(1)))
        Dim timer As New UpdateThread(ticks, AddressOf Restart)

        Call timer.Start()

        Do While True
            Call Console.ReadLine()
            Call Restart()
        Loop

        Return True
    End Function
End Module

Public Class Ini

    Public Property Exe As String
    Public Property CLI As String

    Public Shared ReadOnly Property DefaultFile As String = App.HOME & "/" & App.AssemblyName & ".json"

    Public Shared Function Load() As Ini
        If DefaultFile.FileExists Then
            Return DefaultFile.ReadAllText.LoadObject(Of Ini)
        Else
            Dim ini As New Ini With {
                .CLI = "Put the CLI argument at here",
                .Exe = "Application Path"
            }
            Call ini.GetJson.SaveTo(DefaultFile)
            Call "No profile was found, please modify the generated profile file and then run this command again.".__DEBUG_ECHO
            Call App.Exit(-10)

            Return Nothing
        End If
    End Function
End Class
