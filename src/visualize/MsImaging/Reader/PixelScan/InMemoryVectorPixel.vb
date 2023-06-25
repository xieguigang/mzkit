#Region "Microsoft.VisualBasic::68c836f40d6fce6f1e63b999ba570d55, mzkit\src\visualize\MsImaging\Reader\PixelScan\InMemoryVectorPixel.vb"

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

'   Total Lines: 173
'    Code Lines: 133
' Comment Lines: 4
'   Blank Lines: 36
'     File Size: 6.21 KB


'     Class InMemoryVectorPixel
' 
'         Properties: intensity, mz, sampleTag, scanId, X
'                     Y
' 
'         Constructor: (+5 Overloads) Sub New
' 
'         Function: (+2 Overloads) GetBuffer, GetMsPipe, GetMzIonIntensity, (+2 Overloads) HasAnyMzIon, Parse
'                   ParseVector
' 
'         Sub: release
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Pixel

    Public Class InMemoryVectorPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
        Public Overrides ReadOnly Property Y As Integer

        Public ReadOnly Property mz As Double()
        Public ReadOnly Property intensity As Double()
        Public ReadOnly Property annotations As String()

        Public Overrides ReadOnly Property scanId As String

        Public Overrides ReadOnly Property sampleTag As String = "in-memory cache"

        <DebuggerStepThrough>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(x As Integer,
                y As Integer,
                mz As Double(),
                into As Double(),
                Optional scanId As String = "unknown scan")

            Call Me.New(scanId, x, y, mz, into)
        End Sub

        <DebuggerStepThrough>
        Sub New(scanId As String, x As Integer, y As Integer, mz As Double(), into As Double(),
                Optional sampleTag As String = Nothing,
                Optional annos As String() = Nothing)

            Me.scanId = scanId
            Me.X = x
            Me.Y = y
            Me.mz = mz
            Me.intensity = into
            Me.annotations = annos

            If Not sampleTag.StringEmpty Then
                Me.sampleTag = sampleTag
            End If
        End Sub

        ''' <summary>
        ''' create from mzpack scan data
        ''' </summary>
        ''' <param name="scan"></param>
        Sub New(scan As ScanMS1)
            Call Me.New(scan.scan_id, scan.meta!x, scan.meta!y, scan.mz, scan.into)

            If scan.hasMetaKeys(mzStreamWriter.SampleMetaName) Then
                sampleTag = scan.meta(mzStreamWriter.SampleMetaName)
            End If
        End Sub

        Sub New(pixel As PixelScan, Optional annos As String() = Nothing)
            annotations = annos
            X = pixel.X
            Y = pixel.Y
            scanId = pixel.scanId
            sampleTag = pixel.sampleTag

            If TypeOf pixel Is mzPackPixel Then
                Dim raw As mzPackPixel = DirectCast(pixel, mzPackPixel)

                mz = raw.scan.mz
                intensity = raw.scan.into
            ElseIf TypeOf pixel Is ibdPixel Then
                Dim ibd As ibdPixel = DirectCast(pixel, ibdPixel)
                Dim data As ms2() = ibd.GetMs

                mz = data.Select(Function(i) i.mz).ToArray
                intensity = data.Select(Function(i) i.intensity).ToArray
            Else
                Throw New NotImplementedException(pixel.GetType.FullName)
            End If
        End Sub

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function SetXY(x As Integer, y As Integer) As mzPackPixel
            Return New mzPackPixel(mz, intensity, scanId, sampleTag, x, y)
        End Function

        Public Overrides Function HasAnyMzIon() As Boolean
            Return Not mz.IsNullOrEmpty
        End Function

        Public Function GetBuffer() As Byte()
            Using buf As New MemoryStream, file As New BinaryDataWriter(buf)
                file.Write(scanId, BinaryStringFormat.ZeroTerminated)
                file.Write(Strings.Trim(sampleTag), BinaryStringFormat.ZeroTerminated)
                file.Write(X)
                file.Write(Y)
                file.Write(mz.Length)
                file.Write(mz)
                file.Write(intensity)

                If annotations.IsNullOrEmpty OrElse annotations.All(Function(si) si.StringEmpty) Then
                    file.Write(0)
                Else
                    Dim anno_json As String = annotations.GetJson

                    ' please notices that, this size is char count, not byte size
                    file.Write(anno_json.Length)
                    file.Write(anno_json, BinaryStringFormat.ZeroTerminated)
                End If

                file.Flush()

                Return buf.ToArray
            End Using
        End Function

        Public Shared Function Parse(buffer As Byte()) As InMemoryVectorPixel
            If buffer.Length = 0 Then
                Return Nothing
            End If

            Using file As New BinaryDataReader(New MemoryStream(buffer))
                Dim scanId As String = file.ReadString(BinaryStringFormat.ZeroTerminated)
                Dim sampletag As String = file.ReadString(BinaryStringFormat.ZeroTerminated)
                Dim x As Integer = file.ReadInt32
                Dim y As Integer = file.ReadInt32
                Dim size As Integer = file.ReadInt32
                Dim mz As Double() = file.ReadDoubles(size)
                Dim into As Double() = file.ReadDoubles(size)
                Dim annos As String() = Nothing

                If (Not file.EndOfStream) AndAlso file.ReadInt32 > 0 Then
                    annos = file.ReadString(BinaryStringFormat.ZeroTerminated).LoadJSON(Of String())()
                End If

                Return New InMemoryVectorPixel(scanId, x, y, mz, into, sampletag, annos)
            End Using
        End Function

        Public Shared Iterator Function ParseVector(buffer As Byte()) As IEnumerable(Of InMemoryVectorPixel)
            Using file As New BinaryDataReader(New MemoryStream(buffer))
                Dim n As Integer = file.ReadInt32

                For i As Integer = 0 To n - 1
                    Dim byts As Byte() = file.ReadBytes(file.ReadInt32)

                    Yield Parse(byts)
                Next
            End Using
        End Function

        Public Shared Function GetBuffer(data As InMemoryVectorPixel()) As Byte()
            Using buf As New MemoryStream, file As New BinaryDataWriter(buf)
                Call file.Write(data.Length)

                For Each item In data
                    Dim byts As Byte() = item.GetBuffer

                    Call file.Write(byts.Length)
                    Call file.Write(byts)
                Next

                Call file.Flush()

                Return buf.ToArray
            End Using
        End Function

        Protected Friend Overrides Sub release()
            Erase _mz
            Erase _intensity
        End Sub

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Return mz.Any(Function(mzi) Me.mz.Any(Function(mz2) tolerance(mzi, mz2)))
        End Function

        Protected Friend Overrides Iterator Function GetMsPipe() As IEnumerable(Of ms2)
            For i As Integer = 0 To mz.Length - 1
                Dim anno As String = Nothing

                If Not annotations.IsNullOrEmpty Then
                    anno = annotations(i)
                End If

                Yield New ms2 With {
                    .mz = mz(i),
                    .intensity = intensity(i),
                    .Annotation = anno
                }
            Next
        End Function

        Public Overrides Function GetMzIonIntensity() As Double()
            Return intensity
        End Function
    End Class

End Namespace
