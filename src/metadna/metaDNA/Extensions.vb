Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MetaDNA.Infer

<HideModuleName> Public Module Extensions

    <Extension>
    Public Iterator Function MgfSeeds(mgf As IEnumerable(Of PeakMs2)) As IEnumerable(Of AnnotatedSeed)
        For Each ion As PeakMs2 In mgf
            Yield ion.MgfSeed
        Next
    End Function

    <Extension>
    Private Function MgfSeed(ion As PeakMs2) As AnnotatedSeed
        Dim ms1 As New ms1_scan With {
            .mz = ion.mz,
            .scan_time = ion.rt,
            .intensity = ion.intensity
        }
        Dim ms2 As New LibraryMatrix With {
            .name = ion.lib_guid,
            .ms2 = ion.mzInto
        }

        Return New AnnotatedSeed With {
            .inferSize = 1,
            .kegg_id = ion.meta!KEGG,
            .id = ion.lib_guid,
            .parent = ms1,
            .parentTrace = 100,
            .products = New Dictionary(Of String, LibraryMatrix) From {
                {ion.lib_guid, ms2}
            }
        }
    End Function

    <Extension>
    Public Function ExportInferRaw(raw As IEnumerable(Of CandidateInfer), result As IEnumerable(Of MetaDNAResult)) As MetaDNARawSet
        Return New MetaDNARawSet With {
            .Inference = raw.GetRaw(result).ToArray
        }
    End Function

    <Extension>
    Private Iterator Function GetRaw(raw As IEnumerable(Of CandidateInfer), result As IEnumerable(Of MetaDNAResult)) As IEnumerable(Of CandidateInfer)

    End Function
End Module
