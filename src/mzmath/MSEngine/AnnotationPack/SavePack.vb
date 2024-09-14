Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Linq

Public Module SavePack

    <Extension>
    Public Function PackAlignment(align As AlignmentHit) As MemoryStream
        Dim ms As New MemoryStream
        Dim bin As New BinaryWriter(ms)

        bin.Write(If(align.xcms_id, ""))
        bin.Write(If(align.libname, ""))
        bin.Write(align.mz)
        bin.Write(align.rt)
        bin.Write(align.RI)
        bin.Write(align.theoretical_mz)
        bin.Write(align.exact_mass)
        bin.Write(If(align.adducts, ""))
        bin.Write(align.ppm)
        bin.Write(align.occurrences)
        bin.Write(If(align.biodeep_id, ""))
        bin.Write(If(align.name, ""))
        bin.Write(If(align.formula, ""))
        bin.Write(align.npeaks)
        bin.Write(align.pvalue)
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
        bin.Write(align.forward)
        bin.Write(align.reverse)
        bin.Write(align.jaccard)
        bin.Write(align.entropy)
        bin.Write(If(align.libname, ""))
        bin.Write(If(align.source, ""))
        bin.Write(align.ms2.TryCount)

        For Each peak As SSM2MatrixFragment In align.ms2.SafeQuery
            bin.Write(peak.mz)
            bin.Write(peak.query)
            bin.Write(peak.ref)
            bin.Write(If(peak.da, ""))
        Next

        bin.Flush()
        ms.Seek(Scan0, SeekOrigin.Begin)

        Return ms
    End Function

End Module
