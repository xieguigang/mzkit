#Region "Microsoft.VisualBasic::6e51c796322bdb6d567054cfc0cdd9a3, assembly\mzPackExtensions\MZWork\WorkspaceAccess.vb"

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

    '   Total Lines: 146
    '    Code Lines: 108 (73.97%)
    ' Comment Lines: 11 (7.53%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 27 (18.49%)
    '     File Size: 5.89 KB


    '     Class WorkspaceAccess
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: EnumerateBlocks, (+2 Overloads) GetByFileName, ListAllFileNames, ReleaseCache
    ' 
    '         Sub: (+2 Overloads) Dispose, LoadIndexInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.MessagePack

Namespace MZWork

    Public Class WorkspaceAccess : Implements IDisposable

        Private disposedValue As Boolean

        ReadOnly zip As ZipArchive
        ReadOnly cache As New Dictionary(Of String, NamedValue(Of Raw)())
        ReadOnly println As Action(Of String) = AddressOf Console.WriteLine

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(zip As String, Optional msg As Action(Of String) = Nothing)
            Call Me.New(zip.Open(FileMode.Open, doClear:=False, [readOnly]:=True), msg)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(file As Stream, Optional msg As Action(Of String) = Nothing)
            Call Me.New(New ZipArchive(stream:=file, mode:=ZipArchiveMode.Read), msg)
        End Sub

        Sub New(zip As ZipArchive, Optional msg As Action(Of String) = Nothing)
            Me.zip = zip
            Me.LoadIndexInternal()

            If Not msg Is Nothing Then
                Me.println = msg
            End If
        End Sub

        Private Sub LoadIndexInternal()
            Dim filelist As ZipArchiveEntry() = zip.Entries _
                .Where(Function(f) f.FullName.StartsWith("meta/")) _
                .ToArray
            Dim rawList As New List(Of NamedValue(Of Raw))

            For Each metafile As ZipArchiveEntry In filelist
                Dim key As String = metafile.FullName.Replace("meta/", "").BaseName
                Dim content As Raw() = MsgPackSerializer.Deserialize(Of Raw())(metafile.Open)

                Call rawList.AddRange(From cache As Raw
                                      In content
                                      Select New NamedValue(Of Raw)(key, cache))
            Next

            For Each group In rawList.GroupBy(Function(file) file.Value.source)
                Call cache.Add(group.Key, group.ToArray)
            Next
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ListAllFileNames() As String()
            Return cache.Keys.ToArray
        End Function

        Public Function GetByFileName(fileName As String, Optional verbose As Boolean = False) As IEnumerable(Of mzPack)
            If Not cache.ContainsKey(fileName) Then
                Return Nothing
            Else
                Return GetByFileName(cache(fileName), verbose)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Iterator Function EnumerateBlocks() As IEnumerable(Of NamedValue(Of Raw)())
            For Each item In cache.Values
                Yield item
            Next
        End Function

        Friend Iterator Function ReleaseCache(raws As NamedValue(Of Raw)()) As IEnumerable(Of NamedValue(Of Raw))
            For Each cache As NamedValue(Of Raw) In raws
                Dim meta As Raw = cache.Value
                Dim tempfile As String = getTempref(meta)

                If tempfile.FileLength <= 0 Then
                    Dim zipfile As ZipArchiveEntry = zip.Entries _
                        .Where(Function(f)
                                   Return f.FullName = $"mzpack/{meta.cache}"
                               End Function) _
                        .First

                    Call println($"unpack raw data [{cache.Name}/{tempfile.FileName}]...")
                    Call zipfile.Open.FlushStream(tempfile)
                End If

                ' save mzpack to temp and then modify cache path
                meta = New Raw(meta)
                meta.cache = tempfile.Replace("\", "/")

                Yield New NamedValue(Of Raw) With {
                    .Name = cache.Name,
                    .Description = cache.Description,
                    .Value = meta
                }
            Next
        End Function

        Private Iterator Function GetByFileName(raws As NamedValue(Of Raw)(), verbose As Boolean) As IEnumerable(Of mzPack)
            For Each cache As NamedValue(Of Raw) In ReleaseCache(raws)
                Dim meta As Raw = cache.Value
                Dim tempfile As String = meta.cache
                Dim pack As mzPack = mzPack.Read(
                    filepath:=tempfile,
                    ignoreThumbnail:=True,
                    verbose:=verbose
                )

                pack.source = meta.source.FileName

                Yield pack
            Next
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call zip.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
