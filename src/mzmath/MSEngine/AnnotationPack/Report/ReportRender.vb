﻿#Region "Microsoft.VisualBasic::410dcc33bc6765af76bc6e4554041ef5, mzmath\MSEngine\AnnotationPack\Report\ReportRender.vb"

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

    '   Total Lines: 308
    '    Code Lines: 226 (73.38%)
    ' Comment Lines: 35 (11.36%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 47 (15.26%)
    '     File Size: 13.82 KB


    ' Class ReportRender
    ' 
    '     Properties: annotation, colorSet, DefaultColortSet, samplefiles
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetIon, GetPeak, HtmlTable, makeSubset, MakeZAreas
    '               Ms1ReportTable, Ms2ReportTable, (+2 Overloads) Tabular, ZAreaRange
    ' 
    '     Sub: SetIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions.pnorm
Imports std = System.Math

Public Class ReportRender : Implements IReportRender

    Public ReadOnly Property annotation As AnnotationPack Implements IReportRender.annotation

    ''' <summary>
    ''' metabolite indexed via the biodeep id 
    ''' </summary>
    ''' <remarks>
    ''' multiple <see cref="AlignmentHit"/> value for multiple precursor type
    ''' </remarks>
    ReadOnly metabolites As New Dictionary(Of String, AlignmentHit)
    ''' <summary>
    ''' metabolite indexed via the xcms ion id
    ''' </summary>
    ReadOnly ions As New Dictionary(Of String, AlignmentHit)
    ''' <summary>
    ''' indexed via the xcms id
    ''' </summary>
    ReadOnly peaks As New Dictionary(Of String, xcms2)

    Public Property colorSet As String() = DefaultColortSet Implements IReportRender.colorSet

    ''' <summary>
    ''' ordinal of the sample files or the sample file display selection list
    ''' </summary>
    ''' <returns></returns>
    Public Property samplefiles As String() Implements IReportRender.samplefiles

    Public Shared ReadOnly Property DefaultColortSet As String()
        Get
            Return {"#0D0887FF", "#3E049CFF", "#6300A7FF", "#8707A6FF", "#A62098FF", "#C03A83FF", "#D5546EFF", "#E76F5AFF", "#F58C46FF", "#FDAD32FF", "#FCD225FF", "#F0F921FF"}
        End Get
    End Property

    Sub New(pack As AnnotationPack)
        annotation = pack

        Call SetIndex(pack, ions, metabolites)

        For Each peak As xcms2 In pack.peaks
            peaks(peak.ID) = peak
        Next
    End Sub

    ''' <summary>
    ''' Create object index from the annotation result.
    ''' </summary>
    ''' <param name="pack"></param>
    ''' <param name="ions">indexed via the <see cref="AlignmentHit.xcms_id"/></param>
    ''' <param name="metabolites">indexed via the generated key via the <see cref="AlignmentHit.biodeep_id"/> and <see cref="AlignmentHit.adducts"/></param>
    Public Shared Sub SetIndex(pack As AnnotationPack, ByRef ions As Dictionary(Of String, AlignmentHit), ByRef metabolites As Dictionary(Of String, AlignmentHit))
        For Each libs In pack.libraries
            For Each hit As AlignmentHit In libs.Value
                Dim key As String = hit.biodeep_id & "_" & hit.adducts

                If Not metabolites.ContainsKey(key) Then
                    Call metabolites.Add(key, hit)
                End If
                If Not ions.ContainsKey(hit.xcms_id) Then
                    Call ions.Add(hit.xcms_id, hit)
                End If
            Next
        Next
    End Sub

    Public Function GetIon(xcms_id As String) As AlignmentHit Implements IReportRender.GetIon
        If ions.ContainsKey(xcms_id) Then
            Return ions(xcms_id)
        Else
            Return Nothing
        End If
    End Function

    Public Function HtmlTable(biodeep_ids As IEnumerable(Of String), Optional rt_cell As Boolean = True) As String
        Dim html As New StringBuilder
        Dim lines = Tabular(biodeep_ids, AddressOf VBDebugger.EchoLine, rt_cell).ToArray

        Call html.AppendLine("<table>")
        Call html.AppendLine("<thead>")
        Call html.AppendLine("<tr>")
        Call html.AppendLine(lines(0))
        Call html.AppendLine("</tr>")
        Call html.AppendLine("</thead>")
        Call html.AppendLine("<tbody>")

        For Each row As String In lines.Skip(1)
            Call html.AppendLine("<tr>")
            Call html.AppendLine(row)
            Call html.AppendLine("</tr>")
        Next

        Call html.AppendLine("</tbody>")
        Call html.AppendLine("</table>")

        Return html.ToString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Tabular(biodeep_ids As IEnumerable(Of String), println As Action(Of String),
                            Optional rt_cell As Boolean = False,
                            Optional sortMz As Boolean = False) As IEnumerable(Of String) Implements IReportRender.Tabular

        Return Tabular(biodeep_ids, rt_cell, ms1:=True, sortMz:=sortMz)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="biodeep_ids"></param>
    ''' <returns>
    ''' iterates the html table text, the first element is always the table header title row.
    ''' </returns>
    Public Iterator Function Tabular(biodeep_ids As IEnumerable(Of String), rt_cell As Boolean, ms1 As Boolean, sortMz As Boolean) As IEnumerable(Of String)
        Dim metabolites = makeSubset(biodeep_ids, sortMz)
        Dim ordinals = metabolites.Keys.ToArray
        Dim levels As Integer = colorSet.Length
        Dim index As New DoubleRange(0, levels)

        ' generates the headers
        Yield "<th>Samples</th>" & ordinals _
            .Select(Function(id) metabolites(id)) _
            .Select(Function(a)
                        Dim name = a.name
                        Dim adducts = a.adducts

                        ' metabolite name
                        Return $"<th><a href='#' class='meta_header' xcms_id='{a.xcms_id}'>{name.Replace("<", "&lt;")}</a></th>"
                    End Function) _
            .JoinBy("")

        ' generates the adducts row
        Yield "<td></td>" & ordinals.Select(Function(id) metabolites(id)).Select(Function(a) $"<td>{a.adducts}</td>").JoinBy("")
        ' generates the mz@rt row
        Yield "<td></td>" & ordinals.Select(Function(id) metabolites(id)).Select(Function(a) $"<td><a href='#' class='ROI' mz='{a.theoretical_mz}' rt='{a.rt}'>{a.theoretical_mz.ToString("F4")}@{(a.rt / 60).ToString("F1")}min</a></td>").JoinBy("")

        If ms1 Then
            ' ranges of the ms1 peak area for scale color in a column
            Dim z_areas As Dictionary(Of String, Dictionary(Of String, Double?)) = ordinals _
                .ToDictionary(Function(id) metabolites(id).xcms_id,
                              Function(xcms_id)
                                  Return MakeZAreas(xcms_id)
                              End Function)
            Dim ranges = z_areas.ToDictionary(Function(i) i.Key,
                                              Function(i)
                                                  Return ZAreaRange(i.Value)
                                              End Function)

            For Each sample As String In Me.samplefiles
                Yield Ms1ReportTable(sample, rt_cell, ordinals, z_areas, ranges, levels, index)
            Next
        Else
            ' ranges of the ms2 score for scale color in a column
            Dim ranges = ordinals _
                .ToDictionary(Function(key) key,
                              Function(id)
                                  Dim result = metabolites(id)
                                  Dim data = result.samplefiles.Values

                                  If rt_cell Then
                                      Return data.Select(Function(a) a.rt / 60).Range
                                  Else
                                      Return data.Select(Function(a) a.score).Range
                                  End If
                              End Function)

            For Each sample As String In Me.samplefiles
                Yield Ms2ReportTable(sample, rt_cell, ordinals, ranges, levels, index)
            Next
        End If
    End Function

    Private Shared Function ZAreaRange(i As Dictionary(Of String, Double?)) As DoubleRange
        Return i.Values.Where(Function(a) Not a Is Nothing).Select(Function(a) CDbl(a)).Range
    End Function

    Private Function MakeZAreas(xcms_id As String) As Dictionary(Of String, Double?)
        Dim ROI = peaks(metabolites(xcms_id).xcms_id)
        Dim missing = ROI.Properties.Where(Function(r) r.Value <= 0.0).ToArray
        Dim samples = ROI.Properties.Where(Function(r) r.Value > 0).ToArray
        Dim z = samples.Select(Function(r) r.Value).ToArray.Z
        Dim sample As New Dictionary(Of String, Double?)

        For i As Integer = 0 To z.Length - 1
            sample(samples(i).Key) = z(i)
        Next

        For Each name In missing
            sample(name.Key) = Nothing
        Next

        Return sample
    End Function

    Private Function Ms1ReportTable(sample As String, rt_cell As Boolean, ordinals As String(),
                                    area As Dictionary(Of String, Dictionary(Of String, Double?)),
                                    ranges As Dictionary(Of String, DoubleRange),
                                    levels As Integer,
                                    index As DoubleRange) As String

        Return $"<td><a href=""#"" class='sample_name' data_name='{sample}'>{sample}</a></td>" & ordinals _
            .Select(Function(id)
                        Dim annotation = metabolites(id)
                        Dim ROI = area(annotation.xcms_id)
                        Dim z_area As Double? = ROI(sample)
                        Dim area_data As Double? = If(peaks(annotation.xcms_id).HasProperty(sample), peaks(annotation.xcms_id)(sample), Nothing)
                        Dim range As DoubleRange = ranges(annotation.xcms_id)
                        Dim offset As Integer = If(z_area Is Nothing, -1, range.ScaleMapping(z_area, index))
                        Dim color As String
                        Dim foreColor As String

                        If offset < 0 Then
                            offset = 0
                        ElseIf offset > levels - 1 Then
                            offset = levels - 1
                        End If

                        If z_area Is Nothing Then
                            offset = -1
                            color = "white"
                            foreColor = "darkblue"
                        Else
                            z_area = std.Round(CDbl(z_area), 1)
                            color = colorSet(offset)
                            foreColor = "white"
                        End If

                        If annotation.samplefiles.ContainsKey(sample) Then
                            Dim score As Double = If(rt_cell,
                                std.Round(annotation(sample).rt / 60, 2),
                                std.Round(annotation(sample).score, 2))
                            Dim label As String = If(z_area Is Nothing, "NA", $"{CLng(area_data)} ({score})")

                            Return $"<td style='background-color:{color}; color:{foreColor};'>
<a href='#' class='score' data_id='{annotation.xcms_id}' data_sample='{sample}' biodeep_id='{annotation.biodeep_id}'>
{label}
</a>
</td>"
                        Else
                            Dim label = If(z_area Is Nothing, "NA", $"{CDbl(CLng(area_data))} (NA)")

                            Return $"<td style='background-color:{color}; color:{foreColor};'>{label}</td>"
                        End If
                    End Function) _
            .JoinBy("")
    End Function

    Private Function Ms2ReportTable(sample As String, rt_cell As Boolean, ordinals As String(), ranges As Dictionary(Of String, DoubleRange), levels As Integer, index As DoubleRange) As String
        Return $"<td><a href=""#"" class='sample_name' data_name='{sample}'>{sample}</a></td>" & ordinals _
            .Select(Function(id)
                        Dim annotation = metabolites(id)

                        If annotation.samplefiles.ContainsKey(sample) Then
                            Dim score As Double = If(rt_cell,
                        std.Round(annotation(sample).rt / 60, 2),
                        std.Round(annotation(sample).score, 2))
                            Dim range As DoubleRange = ranges(id)
                            Dim offset As Integer = range.ScaleMapping(score, index)

                            If offset < 0 Then
                                offset = 0
                            ElseIf offset > levels - 1 Then
                                offset = levels - 1
                            End If

                            Return $"<td style='background-color:{colorSet(offset)};'>
<a href='#' class='score' data_id='{annotation.xcms_id}' data_sample='{sample}' biodeep_id='{annotation.biodeep_id}'>
{score}
</a>
</td>"
                        Else
                            Return "<td>NA</td>"
                        End If
                    End Function) _
            .JoinBy("")
    End Function

    Private Function makeSubset(biodeep_ids As IEnumerable(Of String), sortMz As Boolean) As Dictionary(Of String, AlignmentHit)
        Dim subset As New Dictionary(Of String, AlignmentHit)

        For Each id As String In biodeep_ids.Distinct.SafeQuery
            Call subset.Add(id, metabolites.TryGetValue(id))
        Next

        Return subset _
            .Where(Function(a) peaks.ContainsKey(a.Value.xcms_id)) _
            .OrderBy(Function(i)
                         Return If(sortMz, i.Value.theoretical_mz, i.Value.rt)
                     End Function) _
            .ToDictionary
    End Function

    Public Function GetPeak(xcms_id As String) As xcms2 Implements IReportRender.GetPeak
        If peaks.ContainsKey(xcms_id) Then
            Return peaks(xcms_id)
        Else
            Return Nothing
        End If
    End Function
End Class
