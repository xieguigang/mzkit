#Region "Microsoft.VisualBasic::9b27219f519d2011b68ea33d41392e38, ..\httpd\WebCloud\SMRUCC.HTTPInternal\Core\CachedFile.vb"

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
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace Core

    Public Class CachedFile

        Public ReadOnly Property Path As String
        Public ReadOnly Property bufs As Byte()
        Public ReadOnly Property content As ContentType

        Sub New(path As String)
            Me.Path = FileIO.FileSystem.GetFileInfo(path).FullName
            Me.bufs = File.ReadAllBytes(Me.Path)
            Me.Path = Me.Path.ToLower

            Dim ext As String = "." & Me.Path.Split("."c).Last

            Me.content = If(
                MIME.SuffixTable.ContainsKey(ext),
                MIME.SuffixTable(ext),
                MIME.UnknownType)
        End Sub

        Public Overrides Function ToString() As String
            Return $"({content.ToString}) {Path}"
        End Function

        ''' <summary>
        ''' 假若文件更新不频繁，可以在初始化阶段一次性的读取所有文件到内存中
        ''' </summary>
        ''' <param name="wwwroot"></param>
        ''' <returns></returns>
        Public Shared Function CacheAllFiles(wwwroot As String) As Dictionary(Of String, CachedFile)
            Dim allFiles As IEnumerable(Of String) = FileIO.FileSystem.GetFiles(
                wwwroot,
                FileIO.SearchOption.SearchAllSubDirectories,
                "*.*")
            Dim hash As New Dictionary(Of String, CachedFile)

            For Each file As String In allFiles
                hash(file) = New CachedFile(file)
            Next

            Return hash
        End Function
    End Class
End Namespace
