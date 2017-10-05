#Region "Microsoft.VisualBasic::0d4e47cf8b861fc0af045537057dfff3, ..\httpd\WebCloud\SMRUCC.HTTPInternal\DebuggerAPI.vb"

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

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Public Module DebuggerAPI

    ''' <summary>
    ''' Run start the httpd web server.
    ''' </summary>
    ''' <param name="port%">The server port of this httpd web server to listen.</param>
    ''' <param name="wwwroot$">The website html root directory path.</param>
    ''' <param name="threads%">The number of threads of this web server its thread pool.(为了方便进行调试，这个参数的默认值是一个很小的数)</param>
    ''' <param name="cacheMode">Is this server running in file system cache mode? Not recommended for open.</param>
    ''' <returns></returns>
    <ExportAPI("/start")>
    <Description("Run start the httpd web server.")>
    <Usage("/start [/port 80 /wwwroot <wwwroot_DIR> /threads -1 /cache]")>
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
    Public Function Start(Optional port% = 80, Optional wwwroot$ = "./wwwroot", Optional threads% = 3, Optional cacheMode As Boolean = False) As Integer
        Return New PlatformEngine(wwwroot, port, True, threads:=threads, cache:=cacheMode).Run
    End Function
End Module
