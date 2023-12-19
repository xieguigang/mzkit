Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

Public Module SaveXIC

    <Extension>
    Public Sub DumpSample(sample As IEnumerable(Of MzGroup), file As Stream)
        Dim bin As New BinaryWriter(file)
        Dim n As Integer = 0

        For Each ion As MzGroup In sample
            Call bin.Write(ion.mz)
            Call bin.Write(ion.size)

            For Each point In ion.XIC
                Call bin.Write(point.Time)
                Call bin.Write(point.Intensity)
            Next
        Next

        Call bin.Write(n)
        Call bin.Flush()
    End Sub

    Public Iterator Function ReadSample(file As Stream) As IEnumerable(Of MzGroup)
        Dim bin As New BinaryReader(file)
        Dim n As Integer

        file.Seek(file.Length - 4, SeekOrigin.Begin)
        n = bin.ReadInt32
        file.Seek(0, SeekOrigin.Begin)

        For i As Integer = 1 To n
            Dim mz As Double = bin.ReadDouble
            Dim size As Integer = bin.ReadInt32
            Dim ticks As ChromatogramTick() = New ChromatogramTick(size - 1) {}

            For offset As Integer = 0 To size - 1
                ticks(offset) = New ChromatogramTick(bin.ReadDouble, bin.ReadDouble)
            Next

            Yield New MzGroup With {.mz = mz, .XIC = ticks}
        Next
    End Function
End Module
