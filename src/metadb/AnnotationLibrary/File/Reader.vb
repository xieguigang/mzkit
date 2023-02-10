#Region "Microsoft.VisualBasic::70abd780c4f21657291362f01d14b806, mzkit\src\metadb\AnnotationLibrary\File\Reader.vb"

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

    '   Total Lines: 96
    '    Code Lines: 69
    ' Comment Lines: 10
    '   Blank Lines: 17
    '     File Size: 3.64 KB


    ' Class Reader
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetSpectrums, QueryByMz
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Data.IO.MessagePack

Public Class Reader : Inherits LibraryFile
    Implements IDisposable

    Dim index As BlockSearchFunction(Of MassIndex)
    Dim disposedValue As Boolean

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(file As String, massDiff As Double)
        Call Me.New(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True), massDiff)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(file As Stream, massDiff As Double)
        Call Me.New(New ZipArchive(file, ZipArchiveMode.Read), massDiff)
    End Sub

    Sub New(file As ZipArchive, massDiff As Double)
        Me.file = file

        Dim rawIndex = LibraryFile _
            .LoadIndex(file) _
            .ToArray

        Me.index = New BlockSearchFunction(Of MassIndex)(
            data:=rawIndex,
            eval:=Function(i) i.mz,
            tolerance:=massDiff,
            factor:=5
        )
    End Sub

    Public Function GetSpectrums(spectrumBlockId As String) As Spectrum()
        Dim spectrumName As String = $"{spectrumBlockId.Substring(0, 2)}/{spectrumBlockId}.mat"
        Dim pack As ZipArchiveEntry = file.Entries _
            .Where(Function(i) i.FullName = spectrumName) _
            .FirstOrDefault

        Using msBuffer As Stream = pack.Open
            Return MsgPackSerializer.Deserialize(Of Spectrum())(msBuffer)
        End Using
    End Function

    Public Iterator Function QueryByMz(mz As Double, mzdiff As Tolerance) As IEnumerable(Of Metabolite)
        Dim data As Metabolite
        Dim query As IEnumerable(Of MassIndex) = index.Search(New MassIndex With {.mz = mz})

        For Each index As MassIndex In query
            If mzdiff(mz, index.mz) Then
                For Each key As String In index.referenceIds
                    Dim fullName As String = $"{LibraryFile.annotationPath}/{key.Substring(0, 2)}/{key}.dat"
                    Dim pack As ZipArchiveEntry = file.Entries _
                        .Where(Function(i) i.FullName = fullName) _
                        .FirstOrDefault

                    data = MsgPackSerializer.Deserialize(Of Metabolite)(pack.Open)
                    data.spectrumBlockId = key

                    Yield data
                Next
            End If
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call file.Dispose()
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
