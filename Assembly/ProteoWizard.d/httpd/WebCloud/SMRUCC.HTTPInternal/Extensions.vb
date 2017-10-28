#Region "Microsoft.VisualBasic::21b378ae6317e38617588a6e60a435fd, ..\httpd\WebCloud\SMRUCC.HTTPInternal\Extensions.vb"

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
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments

Public Module Extensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path$">The file path of the local file that will be transfer to the client browser.</param>
    ''' <param name="MIMEtype$"><see cref="MIME"/></param>
    ''' <param name="out"></param>
    ''' <param name="buffer_size%"></param>
    <Extension>
    Public Sub TransferBinary(path$, MIMEtype$, ByRef out As HttpResponse, Optional buffer_size% = 4096)
        Dim buffer As Byte() = New Byte(buffer_size) {}

        Using reader As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
            Call out.WriteHeader(MIMEtype, reader.Length)

            Do While reader.Position <= reader.Length
                Call reader.Read(buffer, Scan0, buffer.Length)
                Call out.Write(buffer)
            Loop
        End Using
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub SuccessMsg(rep As HttpResponse, message$)
        Call rep.WriteJSON(New JsonResponse With {.code = 0, .message = message})
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub FailureMsg(rep As HttpResponse, message$, Optional code& = HTTP_RFC.RFC_UNKNOWN_ERROR)
        Call rep.WriteJSON(New JsonResponse With {.code = code, .message = message})
    End Sub
End Module

Public Structure JsonResponse

    <XmlAttribute>
    Public Property code As Integer
    <XmlText>
    Public Property message As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure