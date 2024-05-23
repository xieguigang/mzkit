#Region "Microsoft.VisualBasic::77e3645060badd25abdb5bd98b6a837f, assembly\assembly\mzPack\Binary\BinaryStreamReader.vb"

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

    '   Total Lines: 311
    '    Code Lines: 186 (59.81%)
    ' Comment Lines: 74 (23.79%)
    '    - Xml Docs: 64.86%
    ' 
    '   Blank Lines: 51 (16.40%)
    '     File Size: 11.33 KB


    '     Class BinaryStreamReader
    ' 
    '         Properties: EnumerateIndex, filepath, magic, mzmax, mzmin
    '                     rtmax, rtmin, source
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: GetMetadata, hasMs2, LoadAllScans, pointTo, populateMs2Products
    '                   ReadScan, ReadScan2, ToString
    ' 
    '         Sub: (+2 Overloads) Dispose, loadIndex, ReadChromatogramTick
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace mzData.mzWebCache

    ''' <summary>
    ''' the binary mzpack data reader
    ''' </summary>
    Public Class BinaryStreamReader : Implements IMagicBlock, IMzPackReader
        Implements IDisposable

        Dim disposedValue As Boolean
        ''' <summary>
        ''' stream offset index of the scan id
        ''' </summary>
        Dim index As New Dictionary(Of String, Long)
        Dim metadata As New Dictionary(Of String, Dictionary(Of String, String))

        Protected file As BinaryDataReader
        Protected MSscannerIndex As BufferRegion

        Public ReadOnly Property rtmin As Double
        Public ReadOnly Property rtmax As Double Implements IMzPackReader.rtmax
        Public ReadOnly Property mzmin As Double
        Public ReadOnly Property mzmax As Double

        ''' <summary>
        ''' "BioNovoGene/mzWebStream"
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property magic As String Implements IMagicBlock.magic
            Get
                Return BinaryStreamWriter.Magic
            End Get
        End Property

        ''' <summary>
        ''' get index key of all ms1 scan
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property EnumerateIndex As IEnumerable(Of String) Implements IMzPackReader.EnumerateIndex
            Get
                Return index.Keys
            End Get
        End Property

        Public ReadOnly Property filepath As String

        Public ReadOnly Property source As String Implements IMzPackReader.source
            Get
                If filepath.StringEmpty Then
                    Return "n/a"
                Else
                    Return filepath.FileName
                End If
            End Get
        End Property

        ''' <summary>
        ''' 以只读的形式打开文件
        ''' </summary>
        ''' <param name="file"></param>
        Sub New(file As String)
            Call Me.New(
                file:=file.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True)
            )

            Me.filepath = file
        End Sub

        Sub New(file As Stream)
            Me.file = New BinaryDataReader(
                input:=file,
                encoding:=Encodings.ASCII
            )
            Me.file.ByteOrder = ByteOrder.LittleEndian

            If TypeOf file Is FileStream Then
                Me.filepath = DirectCast(file, FileStream).Name
            End If

            If Not Me.VerifyMagicSignature(Me.file) Then
                Throw New InvalidProgramException("invalid magic header!")
            Else
                Call loadIndex()
            End If
        End Sub

        ''' <summary>
        ''' get meta data of a specific MS1 scan
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns>
        ''' returns NULL if the meta data is not found
        ''' </returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMetadata(index As String) As Dictionary(Of String, String) Implements IMzPackReader.GetMetadata
            Return metadata.TryGetValue(index)
        End Function

        ''' <summary>
        ''' load MS scanner index
        ''' </summary>
        Protected Overridable Sub loadIndex()
            Dim nsize As Integer
            Dim scanPos As Long
            Dim scanId As String
            Dim range As Double() = file.ReadDoubles(4)
            Dim start As Long

            ' the first 32 Bytes is the summary of the MS1
            ' data which is followd the magic header
            _mzmin = range(0)
            _mzmax = range(1)
            _rtmin = range(2)
            _rtmax = range(3)

            Using file.TemporarySeek()
                start = file.ReadInt64
                file.Seek(start, SeekOrigin.Begin)
                ' read count n
                nsize = file.ReadInt32

                ' read data index
                For i As Integer = 0 To nsize - 1
                    scanPos = file.ReadInt64
                    scanId = file.ReadString(BinaryStringFormat.ZeroTerminated)
                    index(scanId) = scanPos
                Next

                ' read meta data after index data
                If file.Position + 20 <= file.Length AndAlso file.ReadInt64 = 0 Then
                    Dim byteSize As Long = file.ReadInt64
                    Dim n As Integer = file.ReadInt32

                    If n <= index.Count Then
                        For i As Integer = 1 To n
                            Dim key As String = file.ReadString(BinaryStringFormat.ZeroTerminated)
                            Dim json As String = file.ReadString(BinaryStringFormat.ZeroTerminated)

                            metadata(key) = json.LoadJSON(Of Dictionary(Of String, String))
                        Next
                    End If
                End If

                MSscannerIndex = New BufferRegion With {
                    .position = start,
                    .size = file.Position - start
                }
            End Using
        End Sub

        ''' <summary>
        ''' read all ms2 scan products inside a given scan in ms1 level
        ''' </summary>
        ''' <param name="scanId">
        ''' the scan id which could be unsed for point to the target ms1 scan data
        ''' </param>
        ''' <returns></returns>
        Public Function ReadScan2(scanId As String) As ScanMS2()
            Dim size As Integer = pointTo(scanId)
            Dim data As ScanMS2()
            Dim nsize As Integer = file.ReadInt32

            file.Seek(size + index(scanId), SeekOrigin.Begin)

            If nsize > 0 Then
                data = populateMs2Products(nsize).ToArray
            Else
                data = {}
            End If

            Return data
        End Function

        ''' <summary>
        ''' move to target offset and then read the
        ''' data buffer size integer value
        ''' </summary>
        ''' <param name="scanId"></param>
        ''' <returns>
        ''' the data size of the current ms1 scan data
        ''' </returns>
        Private Function pointTo(scanId As String) As Integer
            Dim dataSize As Integer

            ' move to target offset
            ' and then read the data buffer size integer value
            file.Seek(offset:=index(scanId), origin:=SeekOrigin.Begin)
            dataSize = file.ReadInt32

            ' this function also read the scan id string for verify
            ' the scan id parameter value
            If file.ReadString(BinaryStringFormat.ZeroTerminated) <> scanId Then
                Throw New InvalidProgramException("unsure why these two scan id mismatch?")
            End If

            Return dataSize
        End Function

        ''' <summary>
        ''' chekc if there is any scan in ms2 level exists in current data file?
        ''' </summary>
        ''' <param name="sampling"></param>
        ''' <returns></returns>
        Public Function hasMs2(Optional sampling As Integer = 64) As Boolean Implements IMzPackReader.hasMs2
            For Each scanId As String In EnumerateIndex.Take(sampling)
                Call pointTo(scanId)

                ' rt BPC TIC
                Call file.ReadDouble()
                Call file.ReadDouble()
                Call file.ReadDouble()

                ' skip mz/into
                Call file.Seek(file.ReadInt32 * 8 * 2)

                If file.ReadInt32 > 0 Then
                    ' has ms2
                    Return True
                End If
            Next

            Return False
        End Function

        Public Sub ReadChromatogramTick(scanId As String,
                                        <Out> ByRef scan_time As Double,
                                        <Out> ByRef BPC As Double,
                                        <Out> ByRef TIC As Double) Implements IMzPackReader.ReadChromatogramTick
            Call pointTo(scanId)

            scan_time = file.ReadDouble
            BPC = file.ReadDouble
            TIC = file.ReadDouble
        End Sub

        Public Iterator Function LoadAllScans(Optional skipProducts As Boolean = False) As IEnumerable(Of ScanMS1)
            For Each scan_id As String In EnumerateIndex
                Yield ReadScan(scan_id, skipProducts)
            Next
        End Function

        Public Function ReadScan(scanId As String, Optional skipProducts As Boolean = False) As ScanMS1 Implements IMzPackReader.ReadScan
            Dim ms1 As New ScanMS1 With {.scan_id = scanId}

            ' metadata of a ms1 scan has already been read
            ' in the pointTo function
            ' skip of the meta data parser at here
            Call pointTo(scanId)
            Call Serialization.ReadScan1(ms1, file, readmeta:=False)

            If Not skipProducts Then
                Dim nsize2 As Integer = file.ReadInt32

                If nsize2 > 0 Then
                    ms1.products = populateMs2Products(nsize2).ToArray
                Else
                    ms1.products = {}
                End If
            End If

            If metadata.ContainsKey(ms1.scan_id) Then
                ms1.meta = metadata(ms1.scan_id)
            End If

            Return ms1
        End Function

        Public Overrides Function ToString() As String
            Return $"{filepath} [{EnumerateIndex.Count} ms1_scans]"
        End Function

        Private Iterator Function populateMs2Products(nsize As Integer) As IEnumerable(Of ScanMS2)
            For i As Integer = 0 To nsize - 1
                Yield file.ReadScanMs2
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
