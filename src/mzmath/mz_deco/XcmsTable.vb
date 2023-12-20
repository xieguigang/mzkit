Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' helper function for generates the xcms peaktable liked table object
''' </summary>
Public Module XcmsTable

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
                        .npeaks = 0,
                        .Properties = New Dictionary(Of String, Double),
                        .rt = peak.rt,
                        .rtmax = peak.rtmax,
                        .rtmin = peak.rtmin
                    }
                End If

                peak2 = xcms(peak.xcms_id)
                peak2.Add(sample.name, peak.area)

                If peak.area > 0 Then
                    peak2.npeaks += 1
                End If

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
End Module
