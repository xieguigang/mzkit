Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions

Namespace PackLib.Validation

    ''' <summary>
    ''' Helper module for create validation dataset
    ''' </summary>
    Public Class DataSetGenerator

        ReadOnly libs As SpectrumReader
        ReadOnly args As DataSetParameters
        ReadOnly ions As New List(Of NamedValue(Of ms1_scan))

        Sub New(libs As SpectrumReader, args As DataSetParameters)
            Me.libs = libs
            Me.args = args
        End Sub

        Public Function Initial() As DataSetGenerator
            ions.Clear()
            Return Me
        End Function

        Public Iterator Function ExportRawDatas() As IEnumerable(Of NamedCollection(Of ScanMS1))
            For i As Integer = 0 To args.RawFiles
                Yield New NamedCollection(Of ScanMS1) With {
                    .name = $"{args.rawname}_{i + 1}",
                    .value = CreateOneDataSet() _
                        .OrderBy(Function(si) si.rt) _
                        .ToArray
                }
            Next
        End Function

        Public Iterator Function GetPeaktable() As IEnumerable(Of Peaktable)
            Dim metabo = ions.GroupBy(Function(a) a.Name).ToArray
            Dim i As i32 = 1

            For Each mset In metabo
                Dim mzset = mset.GroupBy(Function(a) a.Value.mz, offsets:=0.65)
                Dim n As i32 = 1

                For Each ion In mzset
                    Dim rt As Double() = ion.Select(Function(a) a.Value.scan_time).TabulateBin
                    Dim mz As Double() = ion.Select(Function(a) a.Value.mz).ToArray

                    Yield New Peaktable With {
                        .annotation = mset.Key,
                        .name = mset.Key & $"_{++n}",
                        .index = ++i,
                        .mz = mz.Average,
                        .maxo = 0,
                        .energy = "NA",
                        .intb = 0,
                        .into = 0,
                        .ionization = "CID",
                        .mzmax = mz.Max,
                        .mzmin = mz.Min,
                        .rt = rt.Average,
                        .rtmax = rt.Max,
                        .rtmin = rt.Min,
                        .sample = "NA",
                        .scan = .index,
                        .sn = 999
                    }
                Next
            Next
        End Function

        Private Iterator Function CreateOneDataSet() As IEnumerable(Of ScanMS1)
            Dim libnames As String() = libs.ListAllSpectrumId _
                .Shuffles _
                .Take(args.AverageNumberOfSpectrum) _
                .ToArray
            Dim spectrums As New List(Of BlockNode)
            Dim ion As ms1_scan

            For Each id As String In libnames
                Dim data As BlockNode = libs.GetSpectrum(id)

                If data.rt >= args.rtmin AndAlso data.rt <= args.rtmax Then
                    ion = New ms1_scan With {
                        .mz = data.mz.First,
                        .scan_time = data.rt,
                        .intensity = data.centroid.Sum(Function(s) s.intensity)
                    }

                    Call spectrums.Add(data)
                    Call ions.Add(New NamedValue(Of ms1_scan)(data.Id, ion))
                End If
            Next

            ' group by rt
            Dim rt_groups = spectrums.GroupBy(Function(s) s.rt, offsets:=1)

            For Each rt_scan As NamedCollection(Of BlockNode) In rt_groups
                Yield New ScanMS1 With {
                    .BPC = rt_scan.Length,
                    .into = rt_scan.Select(Function(a) CDbl(a.centroid.Length)).ToArray,
                    .mz = rt_scan.Select(Function(a) a.mz.First).ToArray,
                    .TIC = .BPC,
                    .rt = Val(rt_scan.name),
                    .scan_id = "[MS1] rt=" & rt_scan.name,
                    .products = rt_scan _
                        .Select(Function(si)
                                    Return New ScanMS2 With {
                                        .scan_id = si.Id,
                                        .centroided = True,
                                        .intensity = si.centroid.Length,
                                        .mz = si.centroid.Select(Function(m) m.mz).ToArray,
                                        .into = si.centroid.Select(Function(m) m.intensity).ToArray,
                                        .parentMz = si.mz.First,
                                        .rt = si.rt,
                                        .polarity = 1,
                                        .activationMethod = ActivationMethods.CID
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function

    End Class
End Namespace