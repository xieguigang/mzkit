#Region "Microsoft.VisualBasic::133460564e7859efafc19743891b6154, src\assembly\SpectrumTree\Reader.vb"

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

    ' Class Reader
    ' 
    '     Properties: filepath, root, rootCluster
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: OpenParallelReadFile, ReadNextNode, ReadSpectra
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq

Public Class Reader : Implements IDisposable

    Dim infile As BinaryDataReader
    Dim disposedValue As Boolean

    Public ReadOnly Property root As BlockNode
    Public ReadOnly Property filepath As String
    ''' <summary>
    ''' 因为进行树搜索必须要经过根节点，所以在这里将其数据缓存了下来
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property rootCluster As PeakMs2()

    Sub New(file As String)
        infile = New BinaryDataReader(OpenParallelReadFile(file))
        filepath = file

        If infile.ReadString(Writer.magic.Length) <> Writer.magic Then
            Throw New InvalidDataException("invalid magic header!")
        End If

        root = ReadNextNode(infile.BaseStream.Position)
        rootCluster = root.ReadNode(infile.BaseStream).ToArray
    End Sub

    Public Function ReadNextNode(pos As Long) As BlockNode
        infile.Seek(pos, SeekOrigin.Begin)

        If infile.ReadByte = 0 Then
            Return Nothing
        End If

        Dim scan0 = infile.BaseStream.Position  ' data size entry
        Dim left = scan0 + infile.ReadInt64     ' offset + data size = left
        Dim right As Long

        infile.Seek(left, SeekOrigin.Begin)
        right = left + infile.ReadInt64         ' left_offset + data size = right
        infile.Seek(right, SeekOrigin.Begin)

        Return New BlockNode With {
            .scan0 = scan0,
            .left = New BlockNode With {.scan0 = left},
            .right = New BlockNode With {.scan0 = right}
        }
    End Function

    Public Shared Function ReadSpectra(reader As BinaryDataReader) As PeakMs2
        Dim libid As String = reader.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim file As String = reader.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim scan As Integer = reader.ReadInt32
        Dim precursor_type As String = reader.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim activation As String = reader.ReadString(BinaryStringFormat.ZeroTerminated)
        Dim collisionEnergy As Double = reader.ReadDouble
        Dim mz As Double = reader.ReadDouble
        Dim rt As Double = reader.ReadDouble
        Dim nmeta As Integer = reader.ReadInt32
        Dim meta As New Dictionary(Of String, String)
        Dim key As String
        Dim value As String

        For i As Integer = 0 To nmeta - 1
            key = reader.ReadString(BinaryStringFormat.ZeroTerminated)
            value = reader.ReadString(BinaryStringFormat.ZeroTerminated)

            meta(key) = value
        Next

        Dim products As New List(Of ms2)
        Dim nproducts As Integer = reader.ReadInt32

        For i As Integer = 0 To nproducts - 1
            Call New ms2 With {
                .mz = reader.ReadDouble,
                .intensity = reader.ReadDouble,
                .quantity = reader.ReadDouble,
                .Annotation = reader.ReadString(BinaryStringFormat.ZeroTerminated)
            }.DoCall(AddressOf products.Add)
        Next

        Return New PeakMs2 With {
            .lib_guid = libid,
            .file = file,
            .scan = scan,
            .precursor_type = precursor_type,
            .activation = activation,
            .collisionEnergy = collisionEnergy,
            .mz = mz,
            .rt = rt,
            .meta = meta,
            .mzInto = products.ToArray
        }
    End Function

    Public Shared Function OpenParallelReadFile(filepath As String) As Stream
        Return File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call infile.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并替代终结器
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
