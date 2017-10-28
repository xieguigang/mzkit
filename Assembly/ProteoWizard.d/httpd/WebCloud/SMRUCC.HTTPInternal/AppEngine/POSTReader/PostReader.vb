#Region "Microsoft.VisualBasic::96de06690f54732919bc93acb5a5111b, ..\httpd\WebCloud\SMRUCC.HTTPInternal\AppEngine\POSTReader\PostReader.vb"

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

Imports System.Text
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace AppEngine.POSTParser

    ''' <summary>
    ''' POST参数的解析工具
    ''' </summary>
    Public Class PostReader

        ''' <summary>
        ''' Get value from <see cref="Form"/>
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property param(name As String) As String
            Get
                Return Form(name)
            End Get
        End Property

        Private Shared Function GetParameter(header As String, attr As String) As String
            Dim ap As Integer = header.IndexOf(attr)
            If ap = -1 Then
                Return Nothing
            End If

            ap += attr.Length
            If ap >= header.Length Then
                Return Nothing
            End If

            Dim ending As Char = header(ap)
            If ending <> """"c Then
                ending = " "c
            End If

            Dim [end] As Integer = header.IndexOf(ending, ap + 1)
            If [end] = -1 Then
                Return If((ending = """"c), Nothing, header.Substring(ap))
            End If

            Return header.Substring(ap + 1, [end] - ap - 1)
        End Function

        Public ReadOnly Property ContentType() As String
        Public ReadOnly Property InputStream() As MemoryStream
        Public ReadOnly Property ContentEncoding As Encoding
        Public ReadOnly Property Form As New NameValueCollection
        Public ReadOnly Property Files As New Dictionary(Of String, List(Of HttpPostedFile))

        Sub New(input As MemoryStream, contentType As String, encoding As Encoding)
            Me.InputStream = input
            Me.ContentType = contentType
            Me.ContentEncoding = encoding

            Call LoadMultiPart()
        End Sub

        ' GetSubStream returns a 'copy' of the InputStream with Position set to 0.
        Private Shared Function GetSubStream(stream As Stream) As Stream
            Dim other As MemoryStream = DirectCast(stream, MemoryStream)
            Return New MemoryStream(other.GetBuffer(), 0, CInt(other.Length), False, True)
        End Function

        ''' <summary>
        ''' Loads the data on the form for multipart/form-data
        ''' </summary>
        Private Sub LoadMultiPart()
            Dim boundary As String = GetParameter(ContentType, "; boundary=")

            If boundary Is Nothing Then
                ' probably is a jquery post
                Dim byts As Byte() = DirectCast(InputStream, MemoryStream).ToArray
                Dim s As String = ContentEncoding.GetString(byts)

                _Form = s.PostUrlDataParser
            Else
                Call __loadMultiPart(boundary)
            End If

            Call Files _
                .ToDictionary(Function(f) f.Key,
                              Function(names) names.Value.Select(Function(x) x.Summary).ToArray) _
                .GetJson(indent:=True) _
                .Warning
        End Sub

        Private Sub __loadMultiPart(boundary$)
            Dim input As Stream = GetSubStream(InputStream)
            Dim multi_part As New HttpMultipart(input, boundary, ContentEncoding)
            Dim read As New Value(Of HttpMultipart.Element)
            Dim str As String

            While (read = multi_part.ReadNextElement()) IsNot Nothing
                Dim data As HttpMultipart.Element = +read

                If data.Filename Is Nothing Then
                    Dim copy As Byte() = New Byte(data.Length - 1) {}

                    input.Position = data.Start
                    input.Read(copy, 0, CInt(data.Length))

                    str = ContentEncoding.GetString(copy)
                    Call Form.Add(data.Name, str)
                Else
                    '
                    ' We use a substream, as in 2.x we will support large uploads streamed to disk,
                    '
                    Dim [sub] As New HttpPostedFile(
                        data.Filename,
                        data.ContentType,
                        input,
                        data.Start,
                        data.Length)

                    If Not Files.ContainsKey(data.Name) Then
                        Files.Add(data.Name, New List(Of HttpPostedFile))
                    End If

                    Files(data.Name) += [sub]
                End If
            End While
        End Sub
    End Class
End Namespace
