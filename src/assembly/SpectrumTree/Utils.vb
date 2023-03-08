Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions

Public Module Utils

    <Extension>
    Private Iterator Function PopulatePeaks(mz_groups As NamedCollection(Of PeakMs2)(),
                                            rt1 As Double,
                                            i As i32,
                                            totalIons As Double,
                                            sample As MassIndex) As IEnumerable(Of Peaktable)

        For Each mzi As NamedCollection(Of PeakMs2) In mz_groups
            Yield New Peaktable With {
                .annotation = mzi.Select(Function(a) a.lib_guid).JoinBy("+"),
                .rt = rt1,
                .rtmax = rt1,
                .rtmin = rt1,
                .index = i,
                .into = mzi.Select(Function(p) p.intensity).Sum,
                .intb = totalIons,
                .maxo = mzi.Select(Function(p) p.intensity).Max,
                .name = sample.name,
                .sample = sample.name,
                .scan = ++i,
                .sn = 1,
                .energy = "NA",
                .ionization = "HCD",
                .mz = mzi.Average(Function(a) a.mz),
                .mzmax = mzi.Select(Function(a) a.mz).Max,
                .mzmin = mzi.Select(Function(a) a.mz).Min
            }
        Next
    End Function

    <Extension>
    Private Iterator Function PopulateScan2Products(data As PeakMs2(), rt1 As Double) As IEnumerable(Of ScanMS2)
        For Each ms As PeakMs2 In data
            Yield New ScanMS2 With {
                .intensity = ms.intensity,
                .activationMethod = ms.activation,
                .centroided = True,
                .charge = 0,
                .collisionEnergy = 30,
                .into = ms.mzInto.Select(Function(a) a.intensity).ToArray,
                .mz = ms.mzInto.Select(Function(a) a.mz).ToArray,
                .parentMz = ms.mz,
                .polarity = 1,
                .rt = rt1,
                .scan_id = ms.lib_guid
            }
        Next
    End Function

    ''' <summary>
    ''' Create samples for run annotation workflow test
    ''' </summary>
    ''' <param name="libpack"></param>
    ''' <param name="N">
    ''' Take sample of N metabolites
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function GetTestSample(libpack As SpectrumReader, Optional N As Integer = 100) As (peaks As Peaktable(), Ms As ScanMS1())
        Dim test As MassIndex() = libpack.LoadMass.Sample(N, replace:=False)
        Dim peaks As New List(Of Peaktable)
        Dim spectrum As New List(Of ScanMS1)
        Dim i As i32 = 1
        Dim groupErr As Tolerance = Tolerance.DeltaMass(0.5)

        For Each sample As MassIndex In test
            Dim data As PeakMs2() = libpack.GetSpectrum(sample).ToArray
            Dim rt1 As Double = data.Select(Function(d) d.rt).Average
            Dim mz_groups As NamedCollection(Of PeakMs2)() = data.GroupBy(Function(a) a.mz, groupErr).ToArray
            Dim totalIons = data.Select(Function(a) a.intensity).Sum
            Dim scan2 As ScanMS2() = data.PopulateScan2Products(rt1).ToArray

            Call peaks.AddRange(mz_groups.PopulatePeaks(rt1, i, totalIons, sample))
            Call spectrum.Add(New ScanMS1 With {
                .scan_id = sample.name,
                .BPC = data.Select(Function(m) m.intensity).Max,
                .into = data.Select(Function(m) m.intensity).ToArray,
                .mz = data.Select(Function(m) m.mz).ToArray,
                .products = scan2,
                .rt = rt1,
                .TIC = totalIons
            })
        Next

        Return (peaks.ToArray, spectrum.ToArray)
    End Function
End Module
