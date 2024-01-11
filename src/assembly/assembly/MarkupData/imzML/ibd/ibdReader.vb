#Region "Microsoft.VisualBasic::cbb91f6795524757ab04ae4ab2681900, mzkit\src\assembly\assembly\MarkupData\imzML\ibd\ibdReader.vb"

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

    '   Total Lines: 163
    '    Code Lines: 107
    ' Comment Lines: 31
    '   Blank Lines: 25
    '     File Size: 5.91 KB


    '     Class ibdReader
    ' 
    '         Properties: fileName, size, UUID
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetMSMS, GetMSMSPipe, Open, (+2 Overloads) ReadArray, ToString
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO

Namespace MarkupData.imzML

    ''' <summary>
    ''' the binary data reader of the mass spectrum data
    ''' </summary>
    Public Class ibdReader : Implements IDisposable

        ReadOnly stream As BinaryDataReader
        ReadOnly filepath As String

        Dim disposedValue As Boolean

        ''' <summary>
        ''' The first 16 bytes of the binary file are reserved for an Universally Unique Identifier. 
        ''' It is also saved in the imzML file so that a correct assignment of ibd and imzML file 
        ''' is possible even if the names of both files are different.
        ''' </summary>
        Dim magic As Guid
        Dim format As Format

        ''' <summary>
        ''' Universal Unique Identifier
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UUID As String
            Get
                Return magic.ToString
            End Get
        End Property

        ''' <summary>
        ''' the original source file name
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property fileName As String
            Get
                If filepath.StringEmpty Then
                    Return ""
                Else
                    Return filepath.FileName
                End If
            End Get
        End Property

        ''' <summary>
        ''' the ibd file size
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Long
            Get
                If stream.BaseStream Is Nothing Then
                    If filepath.FileExists Then
                        Return filepath.FileLength
                    Else
                        Return 0
                    End If
                Else
                    Return stream.Length
                End If
            End Get
        End Property

        Sub New(file As Stream, layout As Format)
            stream = New BinaryDataReader(file)
            stream.ByteOrder = ByteOrder.LittleEndian
            format = layout
            magic = New Guid(stream.ReadBytes(16))

            If TypeOf file Is FileStream Then
                filepath = DirectCast(file, FileStream).Name
            End If
        End Sub

        ''' <summary>
        ''' Get spectrum data of a pixel point
        ''' </summary>
        ''' <param name="scan">[x, y] of a pixel point</param>
        ''' <returns>
        ''' will skip of the m/z fragments in ZERO intensity value.
        ''' </returns>
        Public Iterator Function GetMSMSPipe(scan As ScanData) As IEnumerable(Of ms2)
            Dim mz As Double() = ReadArray(scan.MzPtr)
            Dim intensity As Double() = ReadArray(scan.IntPtr)

            For i As Integer = 0 To mz.Length - 1
                If intensity(i) = 0.0 Then
                    Continue For
                End If

                Yield New ms2 With {
                    .mz = mz(i),
                    .intensity = intensity(i)
                }
            Next
        End Function

        Public Sub GetMSVector(scan As ScanData, <Out> ByRef mz As Double(), <Out> ByRef intensity As Double())
            mz = ReadArray(scan.MzPtr)
            intensity = ReadArray(scan.IntPtr)
        End Sub

        ''' <summary>
        ''' Get spectrum data of a pixel point
        ''' </summary>
        ''' <param name="scan">[x, y] of a pixel point</param>
        ''' <returns></returns>
        Public Function GetMSMS(scan As ScanData) As ms2()
            Return GetMSMSPipe(scan).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadArray(ptr As ibdPtr) As Double()
            Return ReadArray(ptr.offset, ptr.encodedLength, ptr.arrayLength)
        End Function

        Public Function ReadArray(offset As Long, encodedLength As Integer, arrayLength As Integer) As Double()
            Dim externalArray As Byte()
            Dim sizeof As Integer = encodedLength / arrayLength

            stream.Seek(offset, SeekOrigin.Begin)
            externalArray = stream.ReadBytes(encodedLength)

            If sizeof = 4 Then
                Return externalArray _
                    .Split(4) _
                    .Select(Function(bytes) CDbl(BitConverter.ToSingle(bytes, Scan0))) _
                    .ToArray
            Else
                Return externalArray _
                    .Split(8) _
                    .Select(Function(bytes) BitConverter.ToDouble(bytes, Scan0)) _
                    .ToArray
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"[{format.ToString}] " & UUID
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Open(ibd As String, Optional format As Format = Format.Processed) As ibdReader
            Dim file As Stream = ibd.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim reader As New ibdReader(file, format)

            Return reader
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call stream.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
