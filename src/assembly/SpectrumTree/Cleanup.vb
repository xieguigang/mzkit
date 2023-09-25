Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq

Public Module Cleanup

    ''' <summary>
    ''' Make union and compress the spectrum which associated a same metabolite.
    ''' </summary>
    ''' <param name="spec"></param>
    ''' <param name="n"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Compress(spec As IEnumerable(Of PeakMs2), n As Integer) As IEnumerable(Of PeakMs2)
        Dim da3 As Tolerance = Tolerance.DeltaMass(0.3)
        Dim rawdata As Dictionary(Of String, PeakMs2) = spec _
            .Select(Function(s, i)
                        s.lib_guid = $"{s.lib_guid}.{i + 1}"
                        Return s
                    End Function) _
            .ToDictionary(Function(s) s.lib_guid)
        Dim allMz As Double() = rawdata.Values _
            .Select(Function(m) m.mzInto) _
            .IteratesALL _
            .ToArray _
            .Centroid(da3, New RelativeIntensityCutoff(0.0)) _
            .Select(Function(a) a.mz) _
            .ToArray
        Dim specData As EntityClusterModel() = rawdata.Values _
            .CreateSpecRows(allMz, da3) _
            .Kmeans(expected:=n)

        For Each group In specData.GroupBy(Function(a) a.Cluster)
            Yield group _
                .ToArray _
                .Union(group.Key, rawdata, da3)
        Next
    End Function

    <Extension>
    Private Function Union(norm_spec As EntityClusterModel(), clusterId As String, rawdata As Dictionary(Of String, PeakMs2), da As Tolerance) As PeakMs2
        If norm_spec.Length = 1 Then
            Return rawdata(norm_spec(Scan0).ID)
        End If

        Dim spec As ms2() = norm_spec _
            .Select(Function(r) r.Properties.Select(Function(n) New ms2(n.Key, n.Value))) _
            .IteratesALL _
            .ToArray _
            .Centroid(da, New RelativeIntensityCutoff(0)) _
            .ToArray

        Return New PeakMs2 With {
            .mzInto = spec
        }
    End Function

    <Extension>
    Private Iterator Function CreateSpecRows(rawdata As IEnumerable(Of PeakMs2), allMz As Double(), da As Tolerance) As IEnumerable(Of EntityClusterModel)
        Dim keys As String() = allMz.Select(Function(m) m.ToString("F4")).ToArray
        Dim feature_mz As Double

        For Each spec As PeakMs2 In rawdata
            Dim v As New Dictionary(Of String, Double)
            Dim maxinto As Double = spec.mzInto.Max(Function(m) m.intensity)
            Dim norm As ms2() = spec.mzInto _
                .Select(Function(mzi)
                            Return New ms2 With {
                                .mz = mzi.mz,
                                .intensity = mzi.intensity / maxinto
                            }
                        End Function) _
                .ToArray

            For i As Integer = 0 To keys.Length - 1
                feature_mz = allMz(i)
                v(keys(i)) = Aggregate mzi As ms2
                             In norm
                             Where da(mzi.mz, feature_mz)
                             Into Sum(mzi.intensity)
            Next

            Yield New EntityClusterModel With {
                .ID = spec.lib_guid,
                .Properties = v
            }
        Next
    End Function
End Module
