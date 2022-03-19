#Region "Microsoft.VisualBasic::1867328fc7c9c7254069ef9f9852ab74, mzkit\src\visualize\MsImaging\IndexedCache\XICWriter.vb"

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

    '   Total Lines: 149
    '    Code Lines: 104
    ' Comment Lines: 19
    '   Blank Lines: 26
    '     File Size: 5.61 KB


    '     Class XICWriter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Allocates, ToString
    ' 
    '         Sub: Clear, (+2 Overloads) Dispose, Flush, WritePixels
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Math
Imports stdNum = System.Math

Namespace IndexedCache

    Public Class XICWriter : Implements IDisposable

        ReadOnly bufferSize As Long
        ReadOnly cachefile As BinaryDataWriter
        ReadOnly centroid As Tolerance = Tolerance.DeltaMass(0.3)
        ReadOnly intocutoff As LowAbundanceTrimming = LowAbundanceTrimming.Default

        ''' <summary>
        ''' 会极大的影响文件的缓存大小以及缓存的生成速度
        ''' </summary>
        Friend ReadOnly tolerance As Tolerance = Tolerance.PPM(50)
        Friend ReadOnly offsets As New SortedDictionary(Of Double, BufferRegion)
        Friend ReadOnly length As New Dictionary(Of Double, Integer)

        ''' <summary>
        ''' temp file path
        ''' </summary>
        Friend ReadOnly cache As String
        Friend ReadOnly width As Integer
        Friend ReadOnly height As Integer
        Friend ReadOnly src As String

        Friend ReadOnly centroidPixels As New List(Of ibdPixel)

        Private disposedValue As Boolean

        ''' <summary>
        ''' [x,y]intensity
        ''' </summary>
        Public Const delta As Integer = 4 + 4 + 8

        Sub New(width As Integer, height As Integer, Optional sourceName As String = "n/a")
            Me.bufferSize = delta * width * height
            Me.width = width
            Me.height = height
            Me.src = sourceName.FileName
            Me.cache = TempFileSystem.GetAppSysTempFile(, App.PID.ToHexString, "MSI_XIC_")
            Me.cachefile = New BinaryDataWriter(cache.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
        End Sub

        Public Overrides Function ToString() As String
            Return $"buffer size for each m/z: {Lanudry(bufferSize)}"
        End Function

        Public Sub WritePixels(pixel As PixelScan)
            Dim xy As Integer() = {pixel.X, pixel.Y}
            Dim rawMsMatrix As ms2() = pixel.GetMs
            Dim dataGroup = rawMsMatrix.GroupBy(Function(i) i.mz, tolerance).ToArray

            For Each mz As NamedCollection(Of ms2) In dataGroup
                Dim mzi As Double = Val(mz.name)
                Dim offset As Long
                Dim into As Double = Aggregate i As ms2
                                     In mz
                                     Into Max(i.intensity)

                If into = 0.0 Then
                    Continue For
                Else
                    Dim find = offsets.Keys.Where(Function(mzz) tolerance(mzz, mzi)).FirstOrDefault

                    If find > 0 Then
                        mzi = find
                    End If
                End If

                If offsets.ContainsKey(mzi) Then
                    offset = offsets(mzi).position
                Else
                    offset = Allocates()

                    length(mzi) = 0
                    offsets(mzi) = New BufferRegion With {
                        .position = offset,
                        .size = bufferSize
                    }
                End If

                offset += length(mzi) * delta
                length(mzi) += 1

                cachefile.Seek(offset, SeekOrigin.Begin)
                cachefile.Write(xy)
                cachefile.Write(into)
            Next

            Call centroidPixels.Add(New ibdPixel(pixel.X, pixel.Y, rawMsMatrix.Centroid(centroid, intocutoff)))
            Call cachefile.Flush()
        End Sub

        Public Sub Clear()
            Call "".SaveTo(cache)
        End Sub

        Public Sub Flush()
            Call cachefile.Flush()
        End Sub

        Private Function Allocates() As Long
            If offsets.Count = 0 Then
                Return 0
            Else
                Return offsets _
                    .Select(Function(b) b.Value.nextBlock) _
                    .OrderByDescending(Function(b) b) _
                    .First
            End If
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call cachefile.Flush()
                    Call cachefile.Close()
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
