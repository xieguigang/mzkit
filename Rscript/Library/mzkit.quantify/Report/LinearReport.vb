#Region "Microsoft.VisualBasic::5f28af0760bb46764012b4370c33d246, Library\mzkit.quantify\Report\LinearReport.vb"

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

    ' Module LinearReport
    ' 
    '     Function: asset, CreateHtml, doReport, getStandardCurve, singleLinear
    '               TOC
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text.Xml
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports REnv = SMRUCC.Rsharp.Runtime
Imports stdNum = System.Math

Module LinearReport

    Private Function getStandardCurve(obj As Object) As StandardCurve()
        If obj.GetType Is GetType(LinearDataSet) Then
            Return DirectCast(obj, LinearDataSet).StandardCurve
        Else
            Return DirectCast(obj, StandardCurve())
        End If
    End Function

    Public Function CreateHtml(obj As Object) As String
        Dim standardCurves As StandardCurve() = getStandardCurve(obj)
        Dim report As ScriptBuilder = getBlankReport(title:="Targeted Quantification Linear Models")
        Dim samples As QuantifyScan() = Nothing
        Dim ionsRaw As list = Nothing

        If obj.GetType Is GetType(LinearDataSet) Then
            samples = DirectCast(obj, LinearDataSet).Samples
            ionsRaw = DirectCast(obj, LinearDataSet).IonsRaw
        End If

        Return report.doReport(standardCurves, samples, ionsRaw)
    End Function


    <Extension>
    Private Function doReport(report As ScriptBuilder, standardCurves As StandardCurve(), samples As QuantifyScan(), ionsRaw As list) As String
        Dim linears As New List(Of XElement)

        For Each line As StandardCurve In standardCurves
            With line.singleLinear(ionsRaw)
                If Not .IsNothing Then
                    Call .DoCall(AddressOf linears.Add)
                End If
            End With
        Next

        report("TOC") = standardCurves.TOC
        report("linears") = linears _
            .Select(Function(e, i)
                        Dim line As StandardCurve = standardCurves(i)
                        Dim reportHtml As New ScriptBuilder(e.asset(line))

                        reportHtml("samples") = ""

                        'If samples.IsNullOrEmpty Then
                        '    reportHtml("samples") = ""
                        'Else
                        '    reportHtml("samples") = samples _
                        '        .Select(Function(s)
                        '                    Return New NamedValue(Of Double) With {
                        '                        .Name = s.quantify.ID,
                        '                        .Value = s.quantify(line.name)
                        '                    }
                        '                End Function) _
                        '        .samplePlots(line) _
                        '        .ToString
                        'End If

                        Return reportHtml.ToString
                    End Function) _
            .JoinBy(vbCrLf)

        Return "<!doctype html>" & report.ToString
    End Function

    <Extension>
    Private Function singleLinear(line As StandardCurve, ionsRaw As list) As XElement
        Dim title$ = line.points(Scan0).Name
        Dim image As Image = Visual.DrawStandardCurve(line, title, gridFill:="white").AsGDIImage
        Dim R2# = line.linear.R2
        Dim isWeighted As Boolean = line.isWeighted
        Dim range As DoubleRange = line.points _
            .Where(Function(r) r.valid) _
            .Select(Function(r) r.Cti) _
            .Range
        Dim rawData As NamedCollection(Of ChromatogramTick)() = DirectCast(ionsRaw(line.name), list).slots _
            .Select(Function(sample)
                        Return New NamedCollection(Of ChromatogramTick) With {
                            .name = sample.Key,
                            .value = REnv.asVector(Of ChromatogramTick)(sample.Value)
                        }
                    End Function) _
            .ToArray
        Dim timeRanges As Double() = rawData _
            .Select(Function(r) r.value) _
            .IteratesALL _
            .Select(Function(tick) tick.Time) _
            .ToArray

        If timeRanges.Length = 0 Then
            Call $"metabolite {title} have no raw data for plot!".Warning
            Return Nothing
        End If

        Dim ionRawPlot As Image = rawData _
            .TICplot(
                size:="1600,900",
                fillCurve:=False,
                gridFill:="rgb(250,250,250)",
                penStyle:="stroke: black; stroke-width: 2px; stroke-dash: solid;",
                timeRange:=New Double() {0, timeRanges.Max},
                parallel:=False
            ).AsGDIImage

        Return <div class="row" id=<%= line.name %>>
                   <div class="col-xl-10">
                       <h2><%= title %></h2>

                       <div class="panel panel-success">
                           <div class="panel-heading">Linear</div>
                           <div style="padding:10px">
                               <ul>
                                   <li>ID: <%= line.name %></li>
                                   <li>Linear: <i>f(x)</i>=%s</li>
                                   <li>Weighted: <%= isWeighted.ToString.ToUpper %></li>
                                   <li>R<sup>2</sup>: <%= R2 %> (<%= stdNum.Sqrt(R2) %>)</li>
                                   <li>Range: <%= $"{range.Min} ~ {range.Max}" %></li>
                               </ul>
                           </div>
                       </div>

                       <p>
                           <img src=<%= New DataURI(image) %> style="width: 65%;"/>
                           <img src=<%= New DataURI(ionRawPlot) %> style="width: 65%;"/>
                       </p>

                       <h3>Reference Points</h3>

                       <p>%s</p>
                       <p>{$samples}</p>

                       <hr/>
                   </div>
                   <div style="page-break-after: always;"></div>
               </div>
    End Function

    '<Extension>
    'Private Function samplePlots(samples As IEnumerable(Of NamedValue(Of Double)), line As StandardCurve) As XElement
    '    Dim sampleData = samples.ToArray
    '    Dim title$ = line.points(Scan0).Name
    '    Dim curvesPlot = Visual.DrawStandardCurve(line, $"Samples Of {title}", sampleData).AsGDIImage

    '    Return <div>
    '               <img src=<%= New DataURI(curvesPlot) %> style="width: 100%;"/>
    '           </div>
    'End Function

    <Extension>
    Private Function TOC(lines As StandardCurve()) As String
        Dim rows As String() = lines _
            .Select(Function(line)
                        Dim R2 As Double = line.linear.R2
                        Dim range As DoubleRange = line.points _
                            .Where(Function(sp) sp.valid) _
                            .Select(Function(sp) sp.Cti) _
                            .Range

                        Return <tr class=<%= If(R2 < 0.99, If(R2 < 0.9, "critical", "warning"), "") %>>
                                   <td>
                                       <a href=<%= "#" & line.name %>><%= line.name %></a>
                                   </td>
                                   <td><%= line.points(Scan0).Name %></td>
                                   <td><%= line.linear.Polynomial.ToString("G5", False) %></td>
                                   <td><%= stdNum.Sqrt(line.linear.R2) %></td>
                                   <td><%= range.Min %> ~ <%= range.Max %></td>
                               </tr>
                    End Function) _
            .Select(Function(e) e.ToString) _
            .ToArray

        Return sprintf(<table class="table">
                           <thead>
                               <tr>
                                   <th>ID</th>
                                   <th>name</th>
                                   <th><i>f(x)</i></th>
                                   <th>R<sup>2</sup></th>
                                   <th>Linear Range</th>
                               </tr>
                           </thead>
                           <tbody>
                               %s
                           </tbody>
                       </table>, rows.JoinBy(vbCrLf))
    End Function

    <Extension>
    Private Function asset(e As XElement, line As StandardCurve) As String
        Dim equation$ = line.linear.Polynomial.ToString("G4", html:=True)
        Dim title As String = $"Linear Model Reference Points of '{line.points(Scan0).Name}'"
        Dim pointTable$ = line.points.ToHTMLTable(
            className:="table",
            width:="100%",
            title:=title,
            altClassName:="even"
        )

        Return sprintf(e, equation, pointTable)
    End Function
End Module
