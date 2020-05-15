#Region "Microsoft.VisualBasic::3a8e617de4ac5315a7a69687da3f705a, Rscript\Library\mzkit.quantify\MRM\MRMQCReport.vb"

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

    ' Module MRMQCReport
    ' 
    '     Function: CreateHtml, doReport, reportTOC
    ' 
    ' Class QCData
    ' 
    '     Properties: matchQC, model, result
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Histogram
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text.Xml
Imports stdNum = System.Math

Module MRMQCReport

    Public Function CreateHtml(obj As Object) As String
        Dim data As QCData = DirectCast(obj, QCData)
        Dim ref As Dictionary(Of String, StandardCurve) = data.model.ToDictionary(Function(r) r.name)
        Dim html As String = MRMLinearReport _
            .getBlankReport(title:="MRM QC Report") _
            .doReport(
                ref:=ref,
                result:=data.result,
                QCSampleNamePattern:=data.matchQC
            )

        Return html
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="report">
    ''' 1. TOC
    ''' 2. linears
    ''' </param>
    ''' <param name="ref"></param>
    ''' <param name="result"></param>
    ''' <returns></returns>
    <Extension>
    Private Function doReport(report As ScriptBuilder, ref As Dictionary(Of String, StandardCurve), result As QuantifyScan(), QCSampleNamePattern$) As String
        ' QC RSD for each metabolites
        ' QC plot on the linear
        Dim QCResult As DataSet() = result _
            .Select(Function(sample) sample.quantify) _
            .Where(Function(data)
                       Return Not data.ID.Match(pattern:=QCSampleNamePattern).StringEmpty
                   End Function) _
            .Transpose
        Dim RSD As Double
        Dim linear As StandardCurve
        Dim TOCData As New List(Of DataSet)
        Dim image As Image
        Dim title$
        Dim samples As NamedValue(Of Double)()
        Dim contents As New List(Of String)
        Dim rows As New List(Of String)
        Dim mean#
        Dim RSD_dist As New List(Of Double)
        Dim QC_variants As New List(Of Double)
        Dim tabulateMean As Double

        For Each metabolite In QCResult
            RSD = metabolite.Values.RSD
            linear = ref(metabolite.ID)
            title = $"QC scatter of {metabolite.ID}(RSD: {stdNum.Round(RSD, 3)})"
            samples = metabolite.Properties.NamedValues
            image = Visual.DrawStandardCurve(linear, title, samples, labelerIterations:=2000).AsGDIImage
            mean = metabolite.Values.Average
            tabulateMean = metabolite.Values.TabulateMode
            TOCData += New DataSet With {
                .ID = metabolite.ID,
                .Properties = New Dictionary(Of String, Double) From {
                    {"RSD", RSD},
                    {"Mean", mean},
                    {"SD", metabolite.Values.SD}
                }
            }

            If Not RSD.IsNaNImaginary Then
                RSD_dist += RSD
            End If

            rows *= 0

            For Each sample As KeyValuePair(Of String, Double) In metabolite.Properties
                QC_variants += (stdNum.Abs(sample.Value - tabulateMean) / tabulateMean)
                rows += (<tr>
                             <td><%= sample.Key %></td>
                             <td><%= sample.Value %></td>
                             <td><%= stdNum.Abs(sample.Value - tabulateMean) / tabulateMean %></td>
                         </tr>).ToString
            Next

            contents += sprintf(<div id=<%= metabolite.ID %>>
                                    <h2><%= metabolite.ID %></h2>
                                    <hr/>
                                    <ul>
                                        <li>Mean: <%= mean %></li>
                                        <li>SD: <%= metabolite.Values.SD %></li>
                                        <li>RSD : <strong><%= stdNum.Round(RSD * 100, 2) %></strong></li>
                                    </ul>
                                    <img src=<%= New DataURI(image).ToString %> style="width: 70%"/>
                                    <h3>QC samples</h3>
                                    <table class="table">
                                        <thead>
                                            <tr>
                                                <th>Sample Name</th>
                                                <th>Quantify</th>
                                                <th>Variant</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            %s
                                        </tbody>
                                    </table>
                                    <div style="page-break-after: always;"></div>
                                </div>, rows.JoinBy(vbCrLf))
        Next

        QC_variants = QC_variants _
            .Where(Function(n) Not n.IsNaNImaginary AndAlso n <= 5) _
            .AsList

        report("TOC") = TOCData.reportTOC(RSD_dist, QC_variants.ToArray)
        report("linears") = contents.JoinBy(vbCrLf)

        Return report.ToString
    End Function

    <Extension>
    Private Function reportTOC(TOCData As IEnumerable(Of DataSet), RSD_dist As Double(), QC_variants As Double()) As String
        Dim rows As New List(Of String)
        Dim RSD#
        Dim RSD_steps = 0.1
        Dim hist As Image = RSD_dist _
            .Hist([step]:=RSD_steps) _
            .HistogramPlot([step]:=RSD_steps, serialsTitle:="QC RSD", xLabel:="RSD", yLabel:="Number of Metabolite") _
            .AsGDIImage
        Dim histVariants As Image = QC_variants _
            .Hist([step]:=RSD_steps) _
            .HistogramPlot([step]:=RSD_steps, serialsTitle:="QC variants", xLabel:="Variants", yLabel:="Number of Sample") _
            .AsGDIImage

        For Each compound In TOCData
            RSD = compound!RSD
            rows += sprintf(<tr class=<%= If(RSD >= 0.3, If(RSD >= 0.6, "critical", "warning"), "") %>>
                                <td><a href="#%s"><%= compound.ID %></a></td>
                                <td><%= compound!Mean %></td>
                                <td><%= compound!SD %></td>
                                <td><%= stdNum.Round(compound!RSD * 100, 2) %></td>
                            </tr>, compound.ID)
        Next

        Return sprintf(<div>
                           <img src=<%= New DataURI(hist).ToString %> style="width: 50%;"/>
                           <img src=<%= New DataURI(histVariants).ToString %> style="width: 50%; float: right"/>
                           <br/>
                           <hr/>
                           <table class="table">
                               <thead>
                                   <tr>
                                       <th>Metabolite</th>
                                       <th>Mean</th>
                                       <th>SD</th>
                                       <th>RSD</th>
                                   </tr>
                               </thead>
                               <tbody>
                               %s
                               </tbody>
                           </table>
                       </div>, rows.JoinBy(vbCrLf))
    End Function
End Module

Public Class QCData
    Public Property model As StandardCurve()
    Public Property result As QuantifyScan()
    Public Property matchQC As String
End Class
