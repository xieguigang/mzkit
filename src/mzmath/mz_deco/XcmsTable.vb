Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports std = System.Math

''' <summary>
''' helper function for generates the xcms peaktable liked table object
''' </summary>
Public Module XcmsTable

    ''' <summary>
    ''' A general method for create xcms peaktable
    ''' </summary>
    ''' <param name="samples"></param>
    ''' <returns></returns>
    <Extension>
    Public Function XcmsTable(samples As IEnumerable(Of NamedCollection(Of PeakFeature))) As IEnumerable(Of xcms2)
        Dim peak2 As xcms2
        Dim xcms As New Dictionary(Of String, xcms2)

        For Each sample As NamedCollection(Of PeakFeature) In samples
            For Each peak As PeakFeature In sample
                If Not xcms.ContainsKey(peak.xcms_id) Then
                    xcms(peak.xcms_id) = New xcms2 With {
                        .ID = peak.xcms_id,
                        .mz = peak.mz,
                        .mzmax = peak.mz,
                        .mzmin = peak.mz,
                        .Properties = New Dictionary(Of String, Double),
                        .rt = peak.rt,
                        .rtmax = peak.rtmax,
                        .rtmin = peak.rtmin
                    }
                End If

                peak2 = xcms(peak.xcms_id)
                peak2.Add(sample.name, peak.area)

                If peak.mz < peak2.mzmin Then
                    peak2.mzmin = peak.mz
                End If
                If peak.mz > peak2.mzmax Then
                    peak2.mzmax = peak.mz
                End If
            Next
        Next

        Return xcms.Values
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="samples">XIC data between different samples</param>
    ''' <param name="rtwin"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function XicTable(samples As IEnumerable(Of NamedCollection(Of PeakFeature)),
                                      Optional rtwin As Double = 20,
                                      Optional rt_shifts As List(Of RtShift) = Nothing) As IEnumerable(Of xcms2)

        Dim pool As New List(Of PeakFeature)

        For Each sample As NamedCollection(Of PeakFeature) In samples
            For Each peak As PeakFeature In sample
                peak.rawfile = sample.name
                pool.Add(peak)
            Next
        Next

        ' group by rt
        Dim rt_groups = pool.GroupBy(Function(a) a.rt, offsets:=rtwin).ToArray

        If rt_shifts Is Nothing Then
            rt_shifts = New List(Of RtShift)
        End If

        For Each group As NamedCollection(Of PeakFeature) In rt_groups
            Dim mz As Double() = group.Select(Function(a) a.mz).ToArray
            Dim rt As Double() = group.Select(Function(a) a.rt).ToArray
            ' the reference rt
            Dim max_rt As Double = rt(which.Max(group.Select(Function(a) a.maxInto)))
            Dim rt_quart As DataQuartile = rt.Quartile
            Dim xcms As New xcms2 With {
                .mz = mz.Average,
                .mzmin = mz.Min,
                .mzmax = mz.Max,
                .rt = max_rt,
                .rtmax = rt_quart.Q3,
                .rtmin = rt_quart.Q1,
                .ID = $"M{std.Round(.mz)}T{std.Round(.rt)}",
                .Properties = New Dictionary(Of String, Double)
            }

            For Each sample As PeakFeature In group
                xcms(sample.rawfile) = xcms(name:=sample.rawfile) + sample.area
                rt_shifts.Add(New RtShift With {
                    .refer_rt = max_rt,
                    .sample = sample.rawfile,
                    .sample_rt = sample.rt,
                    .xcms_id = xcms.ID
                })
            Next

            Yield xcms
        Next
    End Function
End Module
