#Region "Microsoft.VisualBasic::8c722b2a8ff75afa33041a5cb274fa4c, ..\httpd\WebCloud\SMRUCC.HTTPInternal\AppEngine\API\Arguments.vb"

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

Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.POSTParser
Imports SMRUCC.WebCloud.HTTPInternal.Core
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Namespace AppEngine.APIMethods.Arguments

    ''' <summary>
    ''' Data of the http request
    ''' </summary>
    Public Class HttpRequest

        ''' <summary>
        ''' GET/POST/PUT/DELETE....
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HTTPMethod As String
        Public ReadOnly Property URL As String
        ''' <summary>
        ''' <see cref="HttpProcessor.http_protocol_versionstring"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property version As String
        Public ReadOnly Property HttpHeaders As Dictionary(Of String, String)

        ''' <summary>
        ''' Remote client ip address
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Remote As String

        ''' <summary>
        ''' If current request url is indicates the HTTP root:  index.html
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsWWWRoot As Boolean
            Get
                Return String.Equals("/", URL)
            End Get
        End Property

        Public Property URLParameters As NameValueCollection

        Sub New(request As HttpProcessor)
            HTTPMethod = request.http_method
            URL = request.http_url
            version = request.http_protocol_versionstring
            HttpHeaders = request.httpHeaders
            Remote = request.socket.Client.RemoteEndPoint.ToString.Split(":"c).First
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class HttpPOSTRequest : Inherits HttpRequest

        Public ReadOnly Property POSTData As PostReader

        Sub New(request As HttpProcessor, inputData As MemoryStream)
            Call MyBase.New(request)
            POSTData = New PostReader(inputData, HttpHeaders(PlatformEngine.contentType), Encoding.UTF8)
        End Sub
    End Class

    Public Class HttpResponse
        Implements IDisposable

        ReadOnly response As StreamWriter
        ReadOnly writeFailed As Action(Of String)

        Sub New(rep As StreamWriter, write404 As Action(Of String))
            response = rep
            writeFailed = write404
        End Sub

        Dim __writeHTML As Boolean = False
        Dim __writeData As Boolean = False

        ''' <summary>
        ''' 在这里只需要将错误消息放进来就行了，页面使用自定义的模板
        ''' </summary>
        ''' <param name="message$"></param>
        Public Sub Write404(message$)
            Call writeFailed(message)
        End Sub

        Public Sub Redirect(url As String)
            Call WriteHTML(<script>window.location='%s';</script>, url)
        End Sub

        Public Sub WriteHTML(html As String)
            If Not __writeHTML AndAlso Not __writeData Then  ' 如果writeData是True，则说明在这之前已经写了其他数据，就不写http头部了
                __writeHTML = writeSuccess()
            End If
            Call response.WriteLine(html)
        End Sub

        Public Sub WriteHTML(html As StringBuilder)
            WriteHTML(html.ToString)
        End Sub

        Private Function writeSuccess() As Boolean
            Try
                Call __writeSuccess("text/html", Nothing)
            Catch ex As Exception
                Call App.LogException(ex)
            End Try

            Return True
        End Function

        ''' <summary>
        ''' 将需要保存到浏览器的数据通过response header的形式返回
        ''' </summary>
        ''' <param name="cookies"></param>
        Public Sub SetCookies(cookies As Dictionary(Of String, String))

        End Sub

        Public Sub WriteHeader(content_type$, Length&)
            ' this is the successful HTTP response line
            response.WriteLine("HTTP/1.0 200 OK")
            ' these are the HTTP headers...      
            response.WriteLine("Accept-Ranges: bytes")
            response.WriteLine("Content-Length: " & Length)
            response.WriteLine("Content-Type: " & content_type)

            response.WriteLine("") ' this terminates the HTTP headers.. everything after this is HTTP body..
            response.Flush()
        End Sub

        Private Sub __writeSuccess(content_type As String, content As Content)
            ' this is the successful HTTP response line
            response.WriteLine("HTTP/1.0 200 OK")
            ' these are the HTTP headers...          
            response.WriteLine("Content-Type: " & content_type)
            response.WriteLine("Connection: close")
            ' ..add your own headers here if you like

            Call content.WriteHeader(response)

            response.WriteLine("X-Powered-By: Microsoft VisualBasic")
            response.WriteLine("")
            ' this terminates the HTTP headers.. everything after this is HTTP body..

            response.Flush()
        End Sub

        ''' <summary>
        ''' %s %d, etc
        ''' </summary>
        ''' <param name="html">C language like printf function format usage.</param>
        ''' <param name="args"></param>
        Public Sub WriteHTML(html As XElement, ParamArray args As Object())
            Call WriteHTML(sprintf(html.ToString, args))
        End Sub

        Public Sub WriteJSON(Of T)(obj As T)
            Dim json As String = obj.GetJson
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(json)

            If Not __writeData Then
                __writeData = True
                Call WriteHeader(MIME.Json, bytes.Length)
            End If

            Call response.WriteLine(json)
        End Sub

        Public Sub WriteXML(Of T)(obj As T)
            __writeData = True
            Call response.WriteLine(obj.GetXml)
        End Sub

        Public Sub Write(byts As Byte())
            __writeData = True
            Call response.BaseStream.Write(byts, Scan0, byts.Length)
        End Sub

        Public Sub Write(byts As Byte(), offset As Integer, count As Integer)
            __writeData = True
            Call response.BaseStream.Write(byts, offset, count)
        End Sub

        ' Exceptions:
        '   T:System.Text.EncoderFallbackException:
        '     The current encoding does not support displaying half of a Unicode surrogate
        '     pair.

        ''' <summary>
        ''' Closes the current StreamWriter object and the underlying stream.
        ''' </summary>
        Public Sub Close()
            Call response.Close()
            Call response.Dispose()
        End Sub

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The current writer is closed.
        '
        '   T:System.IO.IOException:
        '     An I/O error has occurred.
        '
        '   T:System.Text.EncoderFallbackException:
        '     The current encoding does not support displaying half of a Unicode surrogate
        '     pair.

        ''' <summary>
        ''' Clears all buffers for the current writer and causes any buffered data to be
        ''' written to the underlying stream.
        ''' </summary>
        Public Sub Flush()
            Call response.Flush()
        End Sub

        ' Exceptions:
        '   T:System.IO.IOException:
        '     An I/O error occurs.
        '
        '   T:System.ObjectDisposedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and current writer is closed.
        '
        '   T:System.NotSupportedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and the contents of the buffer cannot be written to the underlying fixed
        '     size stream because the System.IO.StreamWriter is at the end the stream.

        ''' <summary>
        ''' Writes a character to the stream.
        ''' </summary>
        ''' <param name="value">The character to write to the stream.</param>
        Public Sub Write(value As Char)
            __writeData = True
            Call response.Write(value)
        End Sub

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and current writer is closed.
        '
        '   T:System.NotSupportedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and the contents of the buffer cannot be written to the underlying fixed
        '     size stream because the System.IO.StreamWriter is at the end the stream.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurs.

        ''' <summary>
        ''' Writes a string to the stream.
        ''' </summary>
        ''' <param name="value">The string to write to the stream. If value is null, nothing is written.</param>
        Public Sub Write(value As String)
            __writeData = True
            Call response.Write(value)
        End Sub

        Public Sub WriteLine(s As String)
            __writeData = True
            Call response.WriteLine(s)
        End Sub

        ' Exceptions:
        '   T:System.IO.IOException:
        '     An I/O error occurs.
        '
        '   T:System.ObjectDisposedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and current writer is closed.
        '
        '   T:System.NotSupportedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and the contents of the buffer cannot be written to the underlying fixed
        '     size stream because the System.IO.StreamWriter is at the end the stream.

        ''' <summary>
        ''' Writes a character array to the stream.
        ''' </summary>
        ''' <param name="buffer">A character array containing the data to write. If buffer is null, nothing is
        ''' written.</param>
        Public Sub Write(buffer() As Char)
            __writeData = True
            Call response.Write(buffer)
        End Sub

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     buffer is null.
        '
        '   T:System.ArgumentException:
        '     The buffer length minus index is less than count.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     index or count is negative.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurs.
        '
        '   T:System.ObjectDisposedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and current writer is closed.
        '
        '   T:System.NotSupportedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and the contents of the buffer cannot be written to the underlying fixed
        '     size stream because the System.IO.StreamWriter is at the end the stream.

        ''' <summary>
        ''' Writes a subarray of characters to the stream.
        ''' </summary>
        ''' <param name="buffer">A character array that contains the data to write.</param>
        ''' <param name="index">The character position in the buffer at which to start reading data.</param>
        ''' <param name="count">The maximum number of characters to write.</param>
        Public Sub Write(buffer() As Char, index As Integer, count As Integer)
            __writeData = True
            Call response.Write(buffer, index, count)
        End Sub

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream has been disposed.

        ''' <summary>
        ''' Clears all buffers for this stream asynchronously and causes any buffered data
        ''' to be written to the underlying device.
        ''' </summary>
        ''' <returns>A task that represents the asynchronous flush operation.</returns>
        <ComVisible(False)>
        Public Function FlushAsync() As Tasks.Task
            Return response.FlushAsync
        End Function

        '
        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a character to the stream asynchronously.
        ''' </summary>
        ''' <param name="value">The character to write to the stream.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteAsync(value As Char) As Tasks.Task
            __writeData = True
            Return response.WriteAsync(value)
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a string to the stream asynchronously.
        ''' </summary>
        ''' <param name="value">The string to write to the stream. If value is null, nothing is written.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteAsync(value As String) As Tasks.Task
            __writeData = True
            Return response.WriteAsync(value)
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     buffer is null.
        '
        '   T:System.ArgumentException:
        '     The index plus count is greater than the buffer length.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     index or count is negative.
        '
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a subarray of characters to the stream asynchronously.
        ''' </summary>
        ''' <param name="buffer">A character array that contains the data to write.</param>
        ''' <param name="index">The character position in the buffer at which to begin reading data.</param>
        ''' <param name="count">The maximum number of characters to write.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteAsync(buffer() As Char, index As Integer, count As Integer) As Tasks.Task
            __writeData = True
            Return response.WriteAsync(buffer, index, count)
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a line terminator asynchronously to the stream.
        ''' </summary>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteLineAsync() As Tasks.Task
            __writeData = True
            Return response.WriteLineAsync
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a character followed by a line terminator asynchronously to the stream.
        ''' </summary>
        ''' <param name="value">The character to write to the stream.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteLineAsync(value As Char) As Tasks.Task
            __writeData = True
            Return response.WriteLineAsync(value)
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.
        ''' <summary>
        ''' Writes a string followed by a line terminator asynchronously to the stream.
        ''' </summary>
        ''' <param name="value">The string to write. If the value is null, only a line terminator is written.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteLineAsync(value As String) As Tasks.Task
            __writeData = True
            Return response.WriteLineAsync(value)
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     buffer is null.
        '
        '   T:System.ArgumentException:
        '     The index plus count is greater than the buffer length.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     index or count is negative.
        '
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a subarray of characters followed by a line terminator asynchronously
        ''' to the stream.
        ''' </summary>
        ''' <param name="buffer">The character array to write data from.</param>
        ''' <param name="index">The character position in the buffer at which to start reading data.</param>
        ''' <param name="count">The maximum number of characters to write.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteLineAsync(buffer() As Char, index As Integer, count As Integer) As Tasks.Task
            __writeData = True
            Return response.WriteLineAsync(buffer, index, count)
        End Function

        ''' <summary>
        ''' url重定向跳转操作
        ''' </summary>
        ''' <param name="rep"></param>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Shared Operator <=(rep As HttpResponse, url As String) As Boolean
            Call rep.Redirect(url)
            Return True
        End Operator

        Public Shared Operator >=(rep As HttpResponse, url As String) As Boolean
            Throw New NotSupportedException
        End Operator

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
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
