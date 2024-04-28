#Region "Microsoft.VisualBasic::785a236242ad57fac072b38525012c8c, E:/mzkit/src/mzmath/mz_deco//Save/SaveSample.vb"

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

    '   Total Lines: 118
    '    Code Lines: 98
    ' Comment Lines: 0
    '   Blank Lines: 20
    '     File Size: 4.27 KB


    ' Module SaveSample
    ' 
    '     Function: ReadGCSample, ReadSample, readSingle
    ' 
    '     Sub: DumpGCMSPeaks, DumpSample
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.BinaryDumping

Public Module SaveSample

    <Extension>
    Public Sub DumpSample(sample As IEnumerable(Of PeakFeature), file As Stream)
        Dim bin As New BinaryWriter(file)
        Dim n As Integer = 0

        For Each point As PeakFeature In sample
            Call bin.Write(If(point.xcms_id, ""))
            Call bin.Write(If(point.rawfile, ""))
            Call bin.Write(point.mz)
            Call bin.Write(point.rt)
            Call bin.Write(point.RI)
            Call bin.Write(point.rtmin)
            Call bin.Write(point.rtmax)
            Call bin.Write(point.maxInto)
            Call bin.Write(point.baseline)
            Call bin.Write(point.integration)
            Call bin.Write(point.area)
            Call bin.Write(point.noise)
            Call bin.Write(point.nticks)

            n += 1
        Next

        Call bin.Write(n)
        Call bin.BaseStream.Flush()
    End Sub

    <Extension>
    Public Sub DumpGCMSPeaks(sample As IEnumerable(Of GCMSPeak), file As Stream)
        Dim bin As New BinaryWriter(file)
        Dim pool As GCMSPeak() = sample.SafeQuery.ToArray
        Dim encoder As New NetworkByteOrderBuffer

        Call DumpSample(pool, file)

        Dim offset As Long = file.Position

        For i As Integer = 0 To pool.Length - 1
            Dim mz As Double() = pool(i).Spectrum.Select(Function(a) a.mz).ToArray
            Dim into As Double() = pool(i).Spectrum.Select(Function(a) a.intensity).ToArray

            Call bin.Write(mz.Length)
            Call bin.Write(encoder.GetBytes(mz))
            Call bin.Write(encoder.GetBytes(into))
        Next

        Call bin.Write(offset)
        Call bin.Write(pool.Length)
        Call bin.BaseStream.Flush()
    End Sub

    Public Iterator Function ReadGCSample(file As Stream) As IEnumerable(Of GCMSPeak)
        Dim rd As New BinaryReader(file)
        rd.BaseStream.Seek(file.Length - 4, SeekOrigin.Begin)
        Dim n As Integer = rd.ReadInt32
        rd.BaseStream.Seek(file.Length - 4 - 8, SeekOrigin.Begin)
        Dim offset As Long = rd.ReadInt64
        Dim peak_offset As Long = Scan0
        Dim peak As PeakFeature
        Dim decoder As New NetworkByteOrderBuffer

        For i As Integer = 1 To n
            rd.BaseStream.Seek(peak_offset, SeekOrigin.Begin)
            peak = readSingle(rd)
            peak_offset = rd.BaseStream.Position
            rd.BaseStream.Seek(offset, SeekOrigin.Begin)

            Dim len As Integer = rd.ReadInt32
            Dim mzBuf As Byte() = rd.ReadBytes(len * 8)
            Dim intoBuf As Byte() = rd.ReadBytes(len * 8)
            Dim mz As Double() = decoder.ParseDouble(mzBuf)
            Dim into As Double() = decoder.ParseDouble(intoBuf)
            Dim spectrum As ms2() = mz.Select(Function(mzi, index) New ms2(mzi, into(index))).ToArray

            offset = rd.BaseStream.Position

            Yield New GCMSPeak(peak) With {.Spectrum = spectrum}
        Next
    End Function

    Private Function readSingle(rd As BinaryReader) As PeakFeature
        Return New PeakFeature With {
            .xcms_id = rd.ReadString,
            .rawfile = rd.ReadString,
            .mz = rd.ReadDouble,
            .rt = rd.ReadDouble,
            .RI = rd.ReadDouble,
            .rtmin = rd.ReadDouble,
            .rtmax = rd.ReadDouble,
            .maxInto = rd.ReadDouble,
            .baseline = rd.ReadDouble,
            .integration = rd.ReadDouble,
            .area = rd.ReadDouble,
            .noise = rd.ReadDouble,
            .nticks = rd.ReadInt32
        }
    End Function

    Public Iterator Function ReadSample(file As Stream) As IEnumerable(Of PeakFeature)
        Dim rd As New BinaryReader(file)
        rd.BaseStream.Seek(file.Length - 4, SeekOrigin.Begin)
        Dim n As Integer = rd.ReadInt32
        rd.BaseStream.Seek(Scan0, SeekOrigin.Begin)

        For i As Integer = 1 To n
            Yield readSingle(rd)
        Next
    End Function

End Module
