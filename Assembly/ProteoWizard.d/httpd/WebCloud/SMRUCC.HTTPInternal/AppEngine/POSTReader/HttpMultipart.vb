#Region "Microsoft.VisualBasic::c33f46900aa976aa812581ff7d4a2dcb, ..\httpd\WebCloud\SMRUCC.HTTPInternal\AppEngine\POSTReader\HttpMultipart.vb"

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
Imports System.IO
Imports Microsoft.VisualBasic.Language

Namespace AppEngine.POSTParser

    ''' <summary>
    ''' Stream-based multipart handling.
    '''
    ''' In this incarnation deals with an HttpInputStream as we are now using
    ''' IntPtr-based streams instead of byte [].   In the future, we will also
    ''' send uploads above a certain threshold into the disk (to implement
    ''' limit-less HttpInputFiles). 
    ''' </summary>
    Public Class HttpMultipart

        Public Class Element
            Public ContentType As String
            Public Name As String
            Public Filename As String
            Public Start As Long
            Public Length As Long

            Public Overrides Function ToString() As String
                Return "ContentType " & ContentType & ", Name " & Name & ", Filename " & Filename & ", Start " & Start.ToString() & ", Length " & Length.ToString()
            End Function
        End Class

        Private data As Stream
        Private boundary As String
        Private boundary_bytes As Byte()
        Private buffer As Byte()
        Private at_eof As Boolean
        Private encoding As Encoding
        Private sb As StringBuilder

        Const HYPHEN As Byte = CByte(AscW("-"c)), LF As Byte = CByte(AscW(ControlChars.Lf)), CR As Byte = CByte(AscW(ControlChars.Cr))

        ' See RFC 2046 
        ' In the case of multipart entities, in which one or more different
        ' sets of data are combined in a single body, a "multipart" media type
        ' field must appear in the entity's header.  The body must then contain
        ' one or more body parts, each preceded by a boundary delimiter line,
        ' and the last one followed by a closing boundary delimiter line.
        ' After its boundary delimiter line, each body part then consists of a
        ' header area, a blank line, and a body area.  Thus a body part is
        ' similar to an RFC 822 message in syntax, but different in meaning.

        Public Sub New(data As Stream, b As String, encoding As Encoding)
            Me.data = data
            boundary = b
            boundary_bytes = encoding.GetBytes(b)
            buffer = New Byte(boundary_bytes.Length + 1) {}
            ' CRLF or '--'
            Me.encoding = encoding
            sb = New StringBuilder()
        End Sub

        Private Function ReadLine() As String
            ' CRLF or LF are ok as line endings.
            Dim got_cr As Boolean = False
            Dim b As Integer = 0
            sb.Length = 0
            While True
                b = data.ReadByte()
                If b = -1 Then
                    Return Nothing
                End If

                If b = LF Then
                    Exit While
                End If
                got_cr = (b = CR)
                sb.Append(ChrW(b))
            End While

            If got_cr Then
                sb.Length -= 1
            End If

            Return sb.ToString()

        End Function

        Private Shared Function GetContentDispositionAttribute(l As String, name As String) As String
            Dim idx As Integer = l.IndexOf(name & "=""")
            If idx < 0 Then
                Return Nothing
            End If
            Dim begin As Integer = idx + name.Length + "=""".Length
            Dim [end] As Integer = l.IndexOf(""""c, begin)
            If [end] < 0 Then
                Return Nothing
            End If
            If begin = [end] Then
                Return ""
            End If
            Return l.Substring(begin, [end] - begin)
        End Function

        Private Function GetContentDispositionAttributeWithEncoding(l As String, name As String) As String
            Dim idx As Integer = l.IndexOf(name & "=""")
            If idx < 0 Then
                Return Nothing
            End If
            Dim begin As Integer = idx + name.Length + "=""".Length
            Dim [end] As Integer = l.IndexOf(""""c, begin)
            If [end] < 0 Then
                Return Nothing
            End If
            If begin = [end] Then
                Return ""
            End If

            Dim temp As String = l.Substring(begin, [end] - begin)
            Dim source As Byte() = New Byte(temp.Length - 1) {}
            For i As Integer = temp.Length - 1 To 0 Step -1
                source(i) = CByte(AscW(temp(i)))
            Next

            Return encoding.GetString(source)
        End Function

        Private Function ReadBoundary() As Boolean
            Try
                Dim line As String = ReadLine()
                While line = ""
                    line = ReadLine()
                End While
                If line(0) <> "-"c OrElse line(1) <> "-"c Then
                    Return False
                End If

                If Not StrUtils.EndsWith(line, boundary, False) Then
                    Return True
                End If
            Catch
            End Try

            Return False
        End Function

        Private Function ReadHeaders() As String
            Dim s As String = ReadLine()
            If s = "" Then
                Return Nothing
            End If

            Return s
        End Function

        Private Function CompareBytes(orig As Byte(), other As Byte()) As Boolean
            For i As Integer = orig.Length - 1 To 0 Step -1
                If orig(i) <> other(i) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Function MoveToNextBoundary() As Long
            Dim retval As Long = 0
            Dim got_cr As Boolean = False

            Dim state As Integer = 0
            Dim c As Integer = data.ReadByte()
            While True
                If c = -1 Then
                    Return -1
                End If

                If state = 0 AndAlso c = LF Then
                    retval = data.Position - 1
                    If got_cr Then
                        retval -= 1
                    End If
                    state = 1
                    c = data.ReadByte()
                ElseIf state = 0 Then
                    got_cr = (c = CR)
                    c = data.ReadByte()
                ElseIf state = 1 AndAlso c = HYPHEN Then
                    c = data.ReadByte()
                    If c = -1 Then
                        Return -1
                    End If

                    If c <> HYPHEN Then
                        state = 0
                        got_cr = False
                        ' no ReadByte() here
                        Continue While
                    End If

                    Dim nread As Integer = data.Read(buffer, 0, buffer.Length)
                    Dim bl As Integer = buffer.Length
                    If nread <> bl Then
                        Return -1
                    End If

                    If Not CompareBytes(boundary_bytes, buffer) Then
                        state = 0
                        data.Position = retval + 2
                        If got_cr Then
                            data.Position += 1
                            got_cr = False
                        End If
                        c = data.ReadByte()
                        Continue While
                    End If

                    If buffer(bl - 2) = HYPHEN AndAlso buffer(bl - 1) = HYPHEN Then
                        at_eof = True
                    ElseIf buffer(bl - 2) <> CR OrElse buffer(bl - 1) <> LF Then
                        state = 0
                        data.Position = retval + 2
                        If got_cr Then
                            data.Position += 1
                            got_cr = False
                        End If
                        c = data.ReadByte()
                        Continue While
                    End If
                    data.Position = retval + 2
                    If got_cr Then
                        data.Position += 1
                    End If
                    Exit While
                Else
                    ' state == 1
                    ' no ReadByte() here
                    state = 0
                End If
            End While

            Return retval
        End Function

        Public Function ReadNextElement() As Element
            If at_eof OrElse ReadBoundary() Then
                Return Nothing
            End If

            Dim elem As New Element()
            Dim header As New Value(Of String)
            While (header = ReadHeaders()) IsNot Nothing
                If StrUtils.StartsWith(header.value, "Content-Disposition:", True) Then
                    elem.Name = GetContentDispositionAttribute(header.value, "name")
                    elem.Filename = StripPath(GetContentDispositionAttributeWithEncoding(header.value, "filename"))
                ElseIf StrUtils.StartsWith(header.value, "Content-Type:", True) Then
                    elem.ContentType = header.value.Substring("Content-Type:".Length).Trim()
                End If
            End While

            Dim start As Long = data.Position
            elem.Start = start
            Dim pos As Long = MoveToNextBoundary()
            If pos = -1 Then
                Return Nothing
            End If

            elem.Length = pos - start
            Return elem
        End Function

        Private Shared Function StripPath(path As String) As String
            If path Is Nothing OrElse path.Length = 0 Then
                Return path
            End If

            If path.IndexOf(":\") <> 1 AndAlso Not path.StartsWith("\\") Then
                Return path
            End If
            Return path.Substring(path.LastIndexOf("\"c) + 1)
        End Function
    End Class
End Namespace
