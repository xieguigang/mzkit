#Region "Microsoft.VisualBasic::95bb4f336f614a19bd618a31c19bc2e3, mzmath\mz_deco\XcmsTable.vb"

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

    '   Total Lines: 146
    '    Code Lines: 104 (71.23%)
    ' Comment Lines: 23 (15.75%)
    '    - Xml Docs: 82.61%
    ' 
    '   Blank Lines: 19 (13.01%)
    '     File Size: 5.59 KB


    ' Module XcmsTable
    ' 
    '     Function: Ms1Scatter, XcmsTable, XicTable
    ' 
    ' /********************************************************************************/

#End Region

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
                                      Optional rt_shifts As List(Of RtShift) = Nothing,
                                      Optional assign_samples As Boolean = True) As IEnumerable(Of xcms2)

        Dim pool As New List(Of PeakFeature)

        If assign_samples Then
            For Each sample As NamedCollection(Of PeakFeature) In samples
                For Each peak As PeakFeature In sample
                    peak.rawfile = sample.name
                    pool.Add(peak)
                Next
            Next
        Else
            For Each sample As NamedCollection(Of PeakFeature) In samples
                Call pool.AddRange(sample.AsEnumerable)
            Next
        End If

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

    ''' <summary>
    ''' Extract of the scatter data
    ''' </summary>
    ''' <param name="peakset"></param>
    ''' <param name="dimension"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Ms1Scatter(peakset As PeakSet,
                                        Optional dimension As String = "default|sum|mean|max|npeaks|<sample_name>") As IEnumerable(Of ms1_scan)

        Dim getter As Func(Of xcms2, Double) = Nothing
        Dim dimName As String = Strings.Trim(dimension).Split("|"c).FirstOrDefault

        Select Case dimName
            Case "sum", "default", "" : getter = Function(a) a.Properties.Values.Sum
            Case "mean" : getter = Function(a) If(a.npeaks = 0, 0, a.Properties.Values.Average)
            Case "max" : getter = Function(a) If(a.npeaks = 0, 0, a.Properties.Values.Max)
            Case "npeaks" : getter = Function(a) a.npeaks

            Case Else
                ' get from sample name
                getter = Function(a) a(dimName)
        End Select

        For Each peak As xcms2 In peakset.AsEnumerable
            Yield New ms1_scan(peak.mz, peak.rt, getter(peak))
        Next
    End Function
End Module
