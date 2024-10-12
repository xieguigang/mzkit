Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Serialization.BinaryDumping

''' <summary>
''' A binary data cache helper of the <see cref="ms1_scan"/> data point collection.
''' </summary>
Public Module Ms1ScatterCache

    ReadOnly network As New NetworkByteOrderBuffer

    <Extension>
    Public Sub SaveDataFrame(scatter As IEnumerable(Of ms1_scan), file As Stream)
        Dim pool As ms1_scan() = scatter.ToArray
        Dim s As New BinaryWriter(file)
        Dim mz As Double() = pool.Select(Function(i) i.mz).ToArray
        Dim rt As Double() = pool.Select(Function(i) i.scan_time).ToArray
        Dim into As Double() = pool.Select(Function(i) i.intensity).ToArray

        Call s.Write(pool.Length)
        Call s.Write(network.GetBytes(mz))
        Call s.Write(network.GetBytes(rt))
        Call s.Write(network.GetBytes(into))
        Call s.Flush()
    End Sub

    <Extension>
    Public Iterator Function LoadDataFrame(file As Stream) As IEnumerable(Of ms1_scan)
        Dim s As New BinaryReader(file)
        Dim size As Integer = s.ReadInt32
        Dim buffer_size As Integer = size * HeapSizeOf.double
        Dim mz = network.ParseDouble(s.ReadBytes(buffer_size))
        Dim rt = network.ParseDouble(s.ReadBytes(buffer_size))
        Dim into = network.ParseDouble(s.ReadBytes(buffer_size))

        For i As Integer = 0 To size - 1
            Yield New ms1_scan(mz(i), rt(i), into(i))
        Next
    End Function
End Module
