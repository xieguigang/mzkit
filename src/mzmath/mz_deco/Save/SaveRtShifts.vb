Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module SaveRtShifts

    <Extension>
    Public Sub DumpShiftsData(rt_shifts As IEnumerable(Of RtShift), file As Stream)
        Dim pool As RtShift() = rt_shifts.ToArray
        Dim all_samples As String() = pool.Select(Function(o) o.sample).Distinct.ToArray
        Dim buf As Byte() = Encoding.UTF8.GetBytes(all_samples.GetJson)
        Dim bin As New BinaryWriter(file)

        bin.Write(pool.Length)
        bin.Write(all_samples.Length)
        bin.Write(buf.Length)
        bin.Write(buf)

        Dim sample_index As Index(Of String) = all_samples.Indexing

        For Each tick As RtShift In pool
            bin.Write(tick.refer_rt)
            bin.Write(tick.sample_rt)
            bin.Write(tick.RI)
            bin.Write(sample_index(tick.sample))
            bin.Write(tick.xcms_id)
        Next

        Call bin.Flush()
    End Sub

    Public Iterator Function ParseRtShifts(file As Stream) As IEnumerable(Of RtShift)
        Dim bin As New BinaryReader(file)
        Dim size As Integer = bin.ReadInt32
        Dim nsamples As Integer = bin.ReadInt32
        Dim json_bytes As Integer = bin.ReadInt32
        Dim buf As Byte() = New Byte(json_bytes - 1) {}
        Dim sample_names As String()

        bin.Read(buf, 0, json_bytes)
        sample_names = Encoding.UTF8.GetString(buf).LoadJSON(Of String())

        For i As Integer = 1 To size
            Yield New RtShift With {
                .refer_rt = bin.ReadDouble,
                .sample_rt = bin.ReadDouble,
                .RI = bin.ReadDouble,
                .sample = sample_names(bin.ReadInt32),
                .xcms_id = bin.ReadString
            }
        Next
    End Function
End Module
