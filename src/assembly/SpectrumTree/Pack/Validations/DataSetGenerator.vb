Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math

Namespace PackLib.Validation

    ''' <summary>
    ''' Helper module for create validation dataset
    ''' </summary>
    Public Class DataSetGenerator

        ReadOnly libs As SpectrumReader
        ReadOnly args As DataSetParameters
        ReadOnly ions As New List(Of ms1_scan)

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
                    .name = i,
                    .value = CreateOneDataSet() _
                        .OrderBy(Function(si) si.rt) _
                        .ToArray
                }
            Next
        End Function

        Private Iterator Function CreateOneDataSet() As IEnumerable(Of ScanMS1)
            Dim libnames As String() = libs.ListAllSpectrumId _
                .Shuffles _
                .Take(args.AverageNumberOfSpectrum) _
                .ToArray
            Dim spectrums As New List(Of BlockNode)

            For Each id As String In libnames
                Dim data = libs.GetSpectrum(id)

                If data.rt >= args.rtmin AndAlso data.rt <= args.rtmax Then
                    Call spectrums.Add(data)
                    Call ions.Add(New ms1_scan With {
                        .mz = data.mz.First,
                        .scan_time = data.rt,
                        .intensity = data.centroid.Sum(Function(s) s.intensity)
                    })
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