Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
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
            .doReport(ref, data.result)

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
    Private Function doReport(report As ScriptBuilder, ref As Dictionary(Of String, StandardCurve), result As QuantifyScan()) As String
        ' QC RSD for each metabolites
        ' QC plot on the linear
        Dim QCResult As DataSet() = result _
            .Select(Function(sample) sample.quantify) _
            .Where(Function(data)
                       Return Not data.ID.Match("QC[-]\d+").StringEmpty
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

        For Each metabolite In QCResult
            RSD = metabolite.Values.RSD
            linear = ref(metabolite.ID)
            title = $"QC scatter of {metabolite.ID}(RSD: {stdNum.Round(RSD, 3)})"
            samples = metabolite.Properties.NamedValues
            image = Visual.DrawStandardCurve(linear, title, samples, labelerIterations:=2000).AsGDIImage
            mean = metabolite.Values.Average
            TOCData += New DataSet With {
                .ID = metabolite.ID,
                .Properties = New Dictionary(Of String, Double) From {
                    {"RSD", RSD},
                    {"Mean", mean},
                    {"SD", metabolite.Values.SD}
                }
            }

            rows *= 0

            For Each sample In metabolite.Properties
                rows += (<tr>
                             <td><%= sample.Key %></td>
                             <td><%= sample.Value %></td>
                             <td><%= stdNum.Abs(sample.Value - mean) / mean %></td>
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
                                </div>, rows.JoinBy(vbCrLf))
        Next

        report("TOC") = TOCData.reportTOC
        report("linears") = contents.JoinBy(vbCrLf)

        Return report.ToString
    End Function

    <Extension>
    Private Function reportTOC(TOCData As IEnumerable(Of DataSet)) As String
        Dim rows As New List(Of String)
        Dim RSD#

        For Each compound In TOCData
            RSD = compound!RSD
            rows += sprintf(<tr class=<%= If(RSD >= 0.3, "warning", "") %>>
                                <td><a href="#%s"><%= compound.ID %></a></td>
                                <td><%= compound!Mean %></td>
                                <td><%= compound!SD %></td>
                                <td><%= stdNum.Round(compound!RSD * 100, 2) %></td>
                            </tr>, compound.ID)
        Next

        Return sprintf(<table class="table">
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
                       </table>, rows.JoinBy(vbCrLf))
    End Function
End Module

Public Class QCData
    Public Property model As StandardCurve()
    Public Property result As QuantifyScan()
End Class