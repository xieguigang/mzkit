#Region "Microsoft.VisualBasic::69fbfc63d2409093a4aa8572ede27807, mzkit\src\assembly\assembly\mzPack\Binary\Serialization.vb"

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

'   Total Lines: 100
'    Code Lines: 65
' Comment Lines: 24
'   Blank Lines: 11
'     File Size: 3.64 KB


'     Module Serialization
' 
'         Function: ReadScanMs2
' 
'         Sub: ReadScan1, WriteBuffer, WriteScan1
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO

Namespace mzData.mzWebCache

    Public Module Serialization

        ''' <summary>
        ''' the given file input data should be in <see cref="ByteOrder.LittleEndian"/>
        ''' </summary>
        ''' <param name="ms1"></param>
        ''' <param name="file">should be in little endian byte order</param>
        Public Sub ReadScan1(ms1 As ScanMS1, file As BinaryDataReader, Optional readmeta As Boolean = False)
            If readmeta Then
                Dim size As Integer = file.ReadInt32()
                ms1.scan_id = file.ReadString(BinaryStringFormat.ZeroTerminated)
            End If

            ms1.rt = file.ReadDouble
            ms1.BPC = file.ReadDouble
            ms1.TIC = file.ReadDouble

            Dim nsize As Integer = file.ReadInt32
            Dim mz As Double() = file.ReadDoubles(nsize)
            Dim into As Double() = file.ReadDoubles(nsize)

            ms1.mz = mz
            ms1.into = into
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scan"></param>
        ''' <param name="file">should be in little endian byte order</param>
        <Extension>
        Public Sub WriteScan1(scan As ScanMS1, file As BinaryDataWriter)
            ' write MS1 scan information
            ' this first zero int32 is a 
            ' placeholder for indicate the byte size
            ' of this ms1 data region
            Call file.Write(0)
            Call file.Write(scan.scan_id, BinaryStringFormat.ZeroTerminated)
            Call file.Write(scan.rt)
            Call file.Write(scan.BPC)
            Call file.Write(scan.TIC)
            Call file.Write(scan.mz.Length)
            Call file.Write(scan.mz)
            Call file.Write(scan.into)
            Call file.Flush()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scan"></param>
        ''' <param name="file">should be in little endian byte order</param>
        <Extension>
        Public Sub WriteBuffer(scan As ScanMS2, file As BinaryDataWriter)
            Call file.Write(scan.scan_id, BinaryStringFormat.ZeroTerminated)
            Call file.Write(scan.parentMz)
            Call file.Write(scan.rt)
            Call file.Write(scan.intensity)
            Call file.Write(scan.polarity)
            Call file.Write(scan.charge)
            Call file.Write(scan.activationMethod)
            Call file.Write(scan.collisionEnergy)
            Call file.Write(CByte(If(scan.centroided, 1, 0)))
            Call file.Write(scan.mz.Length)
            Call file.Write(scan.mz)
            Call file.Write(scan.into)
        End Sub

        Public Function ParseScan2(buf As Byte()) As ScanMS2
            Using ms As New MemoryStream(buf)
                Dim rd As New BinaryDataReader(ms)
                Dim ms2 As ScanMS2 = rd.ReadScanMs2

                Return ms2
            End Using
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file">should be in little endian byte order</param>
        ''' <returns></returns>
        <Extension>
        Public Function ReadScanMs2(file As BinaryDataReader) As ScanMS2
            Dim ms2 As New ScanMS2 With {
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
            Dim productSize As Integer = file.ReadInt32

            ms2.mz = file.ReadDoubles(productSize)
            ms2.into = file.ReadDoubles(productSize)

            Return ms2
        End Function
    End Module
End Namespace
