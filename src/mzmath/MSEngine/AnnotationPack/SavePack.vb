Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Module SavePack

    Public Function PackAlignment(align As AlignmentHit) As MemoryStream
        Dim ms As New MemoryStream
        Dim bin As New BinaryWriter(ms)

        bin.Write(align.xcms_id)


        Return ms
    End Function

    <Extension>
    Public Function PackAlignment(align As Ms2Score) As MemoryStream
        Dim ms As New MemoryStream
        Dim bin As New BinaryWriter(ms)

        bin.Write(align.precursor)
        bin.Write(align.rt)
        bin.Write(align.intensity)
        bin.Write(align.score)
        bin.Write(align.libname)
        bin.Write(align.source)
        bin.Write(align.ms2.Length)

        For Each peak As ms2 In align.ms2
            bin.Write(peak.mz)
            bin.Write(peak.intensity)
            bin.Write(peak.Annotation)
        Next

        bin.Flush()
        ms.Seek(Scan0, SeekOrigin.Begin)

        Return ms
    End Function

End Module
