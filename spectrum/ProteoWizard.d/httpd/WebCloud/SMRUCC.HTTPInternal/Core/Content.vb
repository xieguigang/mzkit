#Region "Microsoft.VisualBasic::f852b63d1419c6ce58a121d8665335a4, ..\httpd\WebCloud\SMRUCC.HTTPInternal\Core\Content.vb"

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
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core

    Public Structure Content

        Public Property Length As Integer

        ''' <summary>
        ''' 不需要在这里写入http头部
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String
        Public Property attachment As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Sub WriteHeader(outputStream As StreamWriter)
            If Length > 0 Then
                Call outputStream.WriteLine("Content-Length: " & Length)
            End If
            If Not String.IsNullOrEmpty(attachment) Then
                Call outputStream.WriteLine($"Content-Disposition: attachment;filename=""{attachment}""")
            End If
        End Sub
    End Structure
End Namespace
