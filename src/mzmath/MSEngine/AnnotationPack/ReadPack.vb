Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Module ReadPack

    Public Function ReadMs2Annotation(file As Stream) As AlignmentHit

    End Function

    Public Function ReadMs2Alignment(file As Stream) As Ms2Score
        Dim bin As New BinaryReader(file)
        Dim details As New Ms2Score With {
            .precursor = bin.ReadDouble,
            .rt = bin.ReadDouble,
            .intensity = bin.ReadDouble,
            .score = bin.ReadDouble,
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
