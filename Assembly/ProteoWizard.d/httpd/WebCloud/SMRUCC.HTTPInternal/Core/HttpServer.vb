#Region "Microsoft.VisualBasic::fddb628438e39866c651cfe68e32a5e8, ..\httpd\WebCloud\SMRUCC.HTTPInternal\Core\HttpServer.vb"

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
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Parallel.Linq

Namespace Core

    ''' <summary>
    ''' Internal http server core.
    ''' </summary>
    Public MustInherit Class HttpServer : Inherits BaseClass
        Implements IDisposable

        Protected Is_active As Boolean = True

        ReadOnly _httpListener As TcpListener

        ''' <summary>
        ''' The network data port of this internal http server listen.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LocalPort As Integer
        ''' <summary>
        ''' Indicates this http server is running status or not. 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsRunning As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Not _httpListener Is Nothing AndAlso _httpListener.Server.IsBound
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="port">The network data port of this internal http server listen.</param>
        Public Sub New(port As Integer, Optional threads As Integer = -1)
            Me._LocalPort = port
            Me._httpListener = New TcpListener(IPAddress.Any, _LocalPort)
            Me._threadPool = New Threads.ThreadPool(
                If(threads = -1,
                LQuerySchedule.Recommended_NUM_THREADS * 8,
                threads))
            Me.BufferSize = Val(App.GetVariable("httpserver.buffer_size"))
            Me.BufferSize = If(BufferSize <= 0, 4096, BufferSize)

            Call $"Web server threads_pool_size={_threadPool.NumOfThreads}, buffer_size={BufferSize}bytes".__INFO_ECHO
        End Sub

        ''' <summary>
        ''' 处理连接的线程池
        ''' </summary>
        Protected Friend _threadPool As Threads.ThreadPool

        ''' <summary>
        ''' Running this http server. 
        ''' NOTE: current thread will be blocked at here until the server core is shutdown. 
        ''' (请注意，在服务器开启之后，当前的线程会被阻塞在这里)
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function Run() As Integer
            Try
                Call _httpListener.Start(10240)
            Catch ex As Exception
                If ex.IsSocketPortOccupied Then
                    Call $"Could not start http services at {NameOf(_LocalPort)}:={_LocalPort}".__DEBUG_ECHO
                    Call ex.ToString.__DEBUG_ECHO
                    Call Console.WriteLine()
                    Call "Program http server thread was terminated.".__DEBUG_ECHO
                    Call Console.WriteLine()
                    Call Console.WriteLine()
                    Call Console.WriteLine()
                Else
                    ex = New Exception(CStr(LocalPort), ex)
                    Call ex.PrintException
                    Call App.LogException(ex)
                End If

                Call Pause()

                Return -1
            End Try

#Const DEBUG = 0
            Call $"Http Server Start listen at {_httpListener.LocalEndpoint.ToString}".__INFO_ECHO
#If DEBUG Then
            Call RunTask(AddressOf Me.OpenAPI_HOME)
#End If
            While Is_active
                If Not _threadPool.FullCapacity Then
                    Call _threadPool.RunTask(AddressOf __accept)
                Else
                    Thread.Sleep(1)
                End If
            End While

            Return 0
        End Function

        ''' <summary>
        ''' 向网页服务器内部的线程池之中添加执行任务
        ''' </summary>
        ''' <param name="task"></param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub RunTask(task As Action)
            Call _threadPool.RunTask(task)
        End Sub

        Private Sub __accept()
            Try
                Dim s As TcpClient = _httpListener.AcceptTcpClient
                Dim processor As HttpProcessor = getProcessor(s)

                Call $"Process client from {s.Client.RemoteEndPoint.ToString}".__DEBUG_ECHO
                Call Time(AddressOf processor.Process)
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        End Sub

        Public Property BufferSize As Integer = 4096

        ''' <summary>
        ''' 一些初始化的设置在这里
        ''' </summary>
        ''' <param name="client"></param>
        ''' <returns></returns>
        Private Function getProcessor(client As TcpClient) As HttpProcessor
            With __httpProcessor(client)
                .BUF_SIZE = BufferSize
                Return .ref
            End With
        End Function

        ''' <summary>
        ''' New HttpProcessor(Client, Me) with {._404Page = "...."}
        ''' </summary>
        ''' <param name="client"></param>
        ''' <returns></returns>
        Protected MustOverride Function __httpProcessor(client As TcpClient) As HttpProcessor

        Private Sub OpenAPI_HOME()
            Call Thread.Sleep(10 * 1000)

            If Environment.OSVersion.Platform = PlatformID.Win32NT Then
                Dim uri As String = $"http://127.0.0.1:{_LocalPort}/"
                Call Process.Start(uri)
            End If
        End Sub

        ''' <summary>
        ''' Shutdown this internal http server
        ''' </summary>
        Public Sub Shutdown()
            Is_active = False
            Call _httpListener.Stop()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p"></param>
        ''' <example>
        ''' 
        ''' If p.http_url.Equals("/Test.png") Then
        '''     Dim fs As Stream = File.Open("../../Test.png", FileMode.Open)
        '''
        '''     p.writeSuccess("image/png")
        '''     fs.CopyTo(p.outputStream.BaseStream)
        '''     p.outputStream.BaseStream.Flush()
        ''' End If
        '''
        '''  Console.WriteLine("request: {0}", p.http_url)
        ''' 
        '''  p.writeSuccess()
        '''  p.outputStream.WriteLine("&lt;html>&lt;body>&lt;h1>Shoal SystemsBiology Shell Language&lt;/h1>")
        '''  p.outputStream.WriteLine("Current Time: " &amp; DateTime.Now.ToString())
        '''  p.outputStream.WriteLine("url : {0}", p.http_url)
        '''
        '''  p.outputStream.WriteLine("&lt;form method=post action=/local_wiki>")
        '''  p.outputStream.WriteLine("&lt;input type=text name=SearchValue value=Keyword>")
        '''  p.outputStream.WriteLine("&lt;input type=submit name=Invoker value=""Search"">")
        '''  p.outputStream.WriteLine("&lt;/form>")
        ''' 
        ''' </example>
        Public MustOverride Sub handleGETRequest(p As HttpProcessor)
        Public MustOverride Sub handlePOSTRequest(p As HttpProcessor, inputData As MemoryStream)
        Public MustOverride Sub handleOtherMethod(p As HttpProcessor)

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Me.Is_active = False
                    _threadPool.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
