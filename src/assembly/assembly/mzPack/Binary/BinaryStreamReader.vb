#Region "Microsoft.VisualBasic::cc074580b5c61c76836b2ec36924906e, assembly\mzPack\Binary\BinaryStreamReader.vb"

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

'     Class BinaryStreamReader
' 
'         Properties: EnumerateIndex, filepath, magic, mzmax, mzmin
'                     rtmax, rtmin
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: pointTo, populateMs2Products, ReadScan, ReadScan2
' 
'         Sub: (+2 Overloads) Dispose, loadIndex
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text

Namespace mzData.mzWebCache

    Public Class BinaryStreamReader : Implements IMagicBlock
        Implements IDisposable

        Dim disposedValue As Boolean
        Dim index As New Dictionary(Of String, Long)

        Protected file As BinaryDataReader
        Protected MSscannerIndex As BufferRegion

        Public ReadOnly Property rtmin As Double
        Public ReadOnly Property rtmax As Double
        Public ReadOnly Property mzmin As Double
        Public ReadOnly Property mzmax As Double

        Public ReadOnly Property magic As String Implements IMagicBlock.magic
            Get
                Return BinaryStreamWriter.Magic
            End Get
        End Property

        Public ReadOnly Property EnumerateIndex As IEnumerable(Of String)
            Get
                Return index.Keys
            End Get
        End Property

        Public ReadOnly Property filepath As String

        Sub New(file As String)
            Me.file = New BinaryDataReader(
                input:=file.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True),
                encoding:=Encodings.ASCII
            )
            Me.file.ByteOrder = ByteOrder.LittleEndian
            Me.filepath = file

            If Not Me.VerifyMagicSignature(Me.file) Then
                Throw New InvalidProgramException("invalid magic header!")
            Else
                Call loadIndex()
            End If
        End Sub

        ''' <summary>
        ''' load MS scanner index
        ''' </summary>
        Protected Overridable Sub loadIndex()
            Dim nsize As Integer
            Dim scanPos As Long
            Dim scanId As String
            Dim range As Double() = file.ReadDoubles(4)
            Dim start As Long

            _mzmin = range(0)
            _mzmax = range(1)
            _rtmin = range(2)
            _rtmax = range(3)

            Using file.TemporarySeek()
                start = file.ReadInt64
                file.Seek(start, SeekOrigin.Begin)
                ' read count n
                nsize = file.ReadInt32

                For i As Integer = 0 To nsize - 1
                    scanPos = file.ReadInt64
                    scanId = file.ReadString(BinaryStringFormat.ZeroTerminated)
                    index(scanId) = scanPos
                Next

                MSscannerIndex = New BufferRegion With {
                    .position = start,
                    .size = file.Position - start
                }
            End Using
        End Sub

        Public Function ReadScan2(scanId As String) As ScanMS2()
            Dim size As Integer = pointTo(scanId)
            Dim data As ScanMS2()

            file.Seek(size + index(scanId), SeekOrigin.Begin)
            data = populateMs2Products.ToArray

            Return data
        End Function

        Private Function pointTo(scanId As String) As Integer
            Dim dataSize As Integer

            file.Seek(offset:=index(scanId), origin:=SeekOrigin.Begin)
            dataSize = file.ReadInt32

            If file.ReadString(BinaryStringFormat.ZeroTerminated) <> scanId Then
                Throw New InvalidProgramException("unsure why these two scan id mismatch?")
            End If

            Return dataSize
        End Function

        Public Function ReadScan(scanId As String, Optional skipProducts As Boolean = False) As ScanMS1
            Dim ms1 As New ScanMS1 With {.scan_id = scanId}

            Call pointTo(scanId)

            ms1.rt = file.ReadDouble
            ms1.BPC = file.ReadDouble
            ms1.TIC = file.ReadDouble

            Dim nsize As Integer = file.ReadInt32
            Dim mz As Double() = file.ReadDoubles(nsize)
            Dim into As Double() = file.ReadDoubles(nsize)

            If Not skipProducts Then
                ms1.products = populateMs2Products.ToArray
            End If

            ms1.mz = mz
            ms1.into = into

            Return ms1
        End Function

        Private Iterator Function populateMs2Products() As IEnumerable(Of ScanMS2)
            Dim nsize As Integer = file.ReadInt32
            Dim ms2 As ScanMS2
            Dim productSize As Integer

            For i As Integer = 0 To nsize - 1
                ms2 = New ScanMS2 With {
                    .scan_id = file.ReadString(BinaryStringFormat.ZeroTerminated),
                    .parentMz = file.ReadDouble,
                    .rt = file.ReadDouble,
                    .intensity = file.ReadDouble,
                    .polarity = file.ReadInt32,
                    .charge = file.ReadInt32,
                    .activationMethod = file.ReadByte,
                    .collisionEnergy = file.ReadDouble,
                    .centroided = file.ReadByte = 1
                }
                productSize = file.ReadInt32

                ms2.mz = file.ReadDoubles(productSize)
                ms2.into = file.ReadDoubles(productSize)

                Yield ms2
            Next
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call file.Dispose()
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
End Namespace
