Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports System.Runtime.CompilerServices

Namespace mzData.mzWebCache

    Public Module ScanModel

        <Extension>
        Public Function Scan2(i As PeakMs2) As ScanMS2
            Return New ScanMS2 With {
                .centroided = True,
                .mz = i.mzInto.Select(Function(mzi) mzi.mz).ToArray,
                .into = i.mzInto.Select(Function(mzi) mzi.intensity).ToArray,
                .parentMz = i.mz,
                .intensity = i.intensity,
                .rt = i.rt,
                .scan_id = $"{i.file}#{i.lib_guid}",
                .collisionEnergy = i.collisionEnergy
            }
        End Function

        <Extension>
        Public Function Scan1(list As NamedCollection(Of PeakMs2)) As ScanMS1
            Dim scan2 As ScanMS2() = list _
                .Select(Function(i)
                            Return i.Scan2
                        End Function) _
                .ToArray

            Return New ScanMS1 With {
               .into = scan2 _
                   .Select(Function(i) i.intensity) _
                   .ToArray,
               .mz = scan2 _
                   .Select(Function(i) i.parentMz) _
                   .ToArray,
               .products = scan2,
               .rt = Val(list.name),
               .scan_id = list.name,
               .TIC = .into.Sum,
               .BPC = .into.Max
            }
        End Function
    End Module
End Namespace