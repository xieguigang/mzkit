Imports System.IO
Imports System.Runtime.CompilerServices
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

    Public Iterator Function ReadSample(file As Stream) As IEnumerable(Of PeakFeature)
        Dim rd As New BinaryReader(file)
        rd.BaseStream.Seek(file.Length - 4, SeekOrigin.Begin)
        Dim n As Integer = rd.ReadInt32
        rd.BaseStream.Seek(Scan0, SeekOrigin.Begin)

        For i As Integer = 1 To n
            Yield New PeakFeature With {
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
        Next
    End Function

End Module
