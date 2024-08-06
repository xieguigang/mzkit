Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices

Public Module ReadPack

    Public Function ReadMs2Annotation(file As Stream) As AlignmentHit
        Dim bin As New BinaryReader(file)
        Dim result As New AlignmentHit With {
            .xcms_id = bin.ReadString,
            .libname = bin.ReadString,
            .mz = bin.ReadDouble,
            .rt = bin.ReadDouble,
            .RI = bin.ReadDouble,
            .theoretical_mz = bin.ReadDouble,
            .exact_mass = bin.ReadDouble,
            .adducts = bin.ReadString,
            .ppm = bin.ReadDouble,
            .occurrences = bin.ReadInt32,
            .biodeep_id = bin.ReadString,
            .name = bin.ReadString,
            .formula = bin.ReadString,
            .npeaks = bin.ReadInt32,
            .samplefiles = New Dictionary(Of String, Ms2Score),
            .pvalue = bin.ReadDouble
        }
        Dim n As Integer = bin.ReadInt32

        For i As Integer = 0 To n - 1
            result(bin.ReadString) = ReadScore(bin)
        Next

        Return result
    End Function

    Private Function ReadScore(bin As BinaryReader) As Ms2Score
        Dim size As Integer = bin.ReadInt32
        Dim s As Stream = bin.BaseStream
        Dim buf As Stream = New SubStream(s, s.Position, size)
        Dim result As Ms2Score = ReadMs2Alignment(file:=buf)
        Return result
    End Function

    Public Function ReadMs2Alignment(file As Stream) As Ms2Score
        Dim bin As New BinaryReader(file)
        Dim details As New Ms2Score With {
            .precursor = bin.ReadDouble,
            .rt = bin.ReadDouble,
            .intensity = bin.ReadDouble,
            .score = bin.ReadDouble,
            .forward = bin.ReadDouble,
            .reverse = bin.ReadDouble,
            .jaccard = bin.ReadDouble,
            .entropy = bin.ReadDouble,
            .libname = bin.ReadString,
            .source = bin.ReadString
        }
        Dim n As Integer = bin.ReadInt32
        Dim peaks As ms2() = New ms2(n - 1) {}

        For i As Integer = 0 To n - 1
            peaks(i) = New ms2 With {
                .mz = bin.ReadDouble,
                .intensity = bin.ReadDouble,
                .Annotation = bin.ReadString
            }
        Next

        details.ms2 = peaks

        Return details
    End Function

End Module
