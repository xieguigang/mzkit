Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Public Module SavePack

    <Extension>
    Public Function PackAlignment(align As AlignmentHit) As MemoryStream
        Dim ms As New MemoryStream
        Dim bin As New BinaryWriter(ms)

        bin.Write(align.xcms_id)
        bin.Write(align.libname)
        bin.Write(align.mz)
        bin.Write(align.rt)
        bin.Write(align.RI)
        bin.Write(align.theoretical_mz)
        bin.Write(align.exact_mass)
        bin.Write(align.adducts)
        bin.Write(align.ppm)
        bin.Write(align.occurrences)
        bin.Write(align.biodeep_id)
        bin.Write(align.name)
        bin.Write(align.formula)
        bin.Write(align.npeaks)
        bin.Write(align.samplefiles.TryCount)

        For Each sample In align.samplefiles.SafeQuery
            Using buf As MemoryStream = sample.Value.PackAlignment
                Call bin.Write(sample.Key)
                Call bin.Write(buf.Length)
                Call bin.Write(buf.ToArray)
            End Using
        Next

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
