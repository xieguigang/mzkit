#Region "Microsoft.VisualBasic::6b006fd397620a211600b717a2fadc91, E:/mzkit/src/assembly/SpectrumTree//Utils.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 110
    '    Code Lines: 91
    ' Comment Lines: 8
    '   Blank Lines: 11
    '     File Size: 4.56 KB


    ' Module Utils
    ' 
    '     Function: GetTestSample, PopulatePeaks, PopulateScan2Products
    ' 
    ' /********************************************************************************/

#End Region

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
                .rtmax = rt1 + 5,
                .rtmin = rt1 - 5,
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
    Public Function GetTestSample(libpack As SpectrumReader,
                                  Optional N As Integer = 100,
                                  Optional rtmax As Double = 840) As (peaks As Peaktable(), Ms As ScanMS1())

        Dim test As MassIndex() = libpack.LoadMass.Sample(N, replace:=False)
        Dim peaks As New List(Of Peaktable)
        Dim spectrum As New List(Of ScanMS1)
        Dim i As i32 = 1
        Dim groupErr As Tolerance = Tolerance.DeltaMass(0.5)

        For Each sample As MassIndex In test
            Dim data As PeakMs2() = libpack.GetSpectrum(sample).ToArray
            Dim rt1 As Double = data.Select(Function(d) d.rt).Average

            If rt1 = 0.0 Then
                rt1 = randf(30, rtmax)
            End If

            Dim totalIons = data.Select(Function(a) a.intensity).Sum
            Dim scan2 As ScanMS2() = data.PopulateScan2Products(rt1).ToArray
            Dim mz_groups As NamedCollection(Of PeakMs2)() = data _
                .GroupBy(Function(a) a.mz, groupErr) _
                .ToArray

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

        Return (peaks.ToArray, spectrum.OrderBy(Function(s) s.rt).ToArray)
    End Function
End Module
