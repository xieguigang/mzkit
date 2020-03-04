Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text.Xml

Module MRMLinearReport

    Private Function getStandardCurve(obj As Object) As StandardCurve()
        If obj.GetType Is GetType(MRMDataSet) Then
            Return DirectCast(obj, MRMDataSet).StandardCurve
        Else
            Return DirectCast(obj, StandardCurve())
        End If
    End Function

    Public Function CreateHtml(obj As Object) As String
        Dim standardCurves As StandardCurve() = getStandardCurve(obj)
        Dim report As ScriptBuilder = getBlankReport(title:="MRM Quantification Linear Models")
        Dim samples As QuantifyScan() = Nothing

        If obj.GetType Is GetType(MRMDataSet) Then
            samples = DirectCast(obj, MRMDataSet).Samples
        End If

        Return report.doReport(standardCurves, samples)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Friend Function getBlankReport(title As String) As ScriptBuilder
        Return New ScriptBuilder(
            <html lang="zh-CN">
                <head>
                    <meta charset="utf-8"/>
                    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no"/>

                    <title><%= title %> | BioNovoGene</title>

                    <!-- Bootstrap CSS -->
                    <link rel="stylesheet" href="http://cdn.biodeep.cn:8848/styles/bootstrap-4.3.1-dist/css/bootstrap.min.css" crossorigin="anonymous"/>

                    <style type="text/css">
                        .even td{/*必须加td，代表的是一行进行*/
	                      background-color: #f5f5f5;
                        }

                        .critical td {
                          background-color: #e91e63;
                        }

                        .warning td {
                          background-color: #f3ea9c;
                        }
                    </style>
                </head>
                <body class="container">
                    <h1><%= title %></h1>
                    <hr/>
                    <h2>Table Of Content</h2>
                    {$TOC}
                    <hr/>

                    {$linears}
                </body>

                <!-- Optional JavaScript -->
                <!-- jQuery first, then Popper.js, then Bootstrap JS -->
                <script src="http://cdn.biodeep.cn:8848/vendor/jquery-3.2.1.min.js" crossorigin="anonymous"></script>
                <script src="http://cdn.biodeep.cn:8848/vendor/popper.min.js" crossorigin="anonymous"></script>
                <script src="http://cdn.biodeep.cn:8848/styles/bootstrap-4.3.1-dist/js/bootstrap.min.js" crossorigin="anonymous"></script>
            </html>)
    End Function

    <Extension>
    Private Function doReport(report As ScriptBuilder, standardCurves As StandardCurve(), samples As QuantifyScan()) As String
        Dim linears As New List(Of XElement)
        Dim image As Image
        Dim title$
        Dim R2#
        Dim isWeighted As Boolean

        For Each line As StandardCurve In standardCurves
            title = line.points(Scan0).Name
            image = Visual.DrawStandardCurve(line, title).AsGDIImage
            R2 = line.linear.CorrelationCoefficient
            isWeighted = line.isWeighted
            linears +=
                <div class="row" id=<%= line.name %>>
                    <div class="col-xl-10">
                        <h2><%= title %></h2>

                        <div class="panel panel-success">
                            <div class="panel-heading">Linear</div>
                            <div style="padding:10px">
                                <ul>
                                    <li>ID: <%= line.name %></li>
                                    <li>Linear: <i>f(x)</i>=%s</li>
                                    <li>Weighted: <%= isWeighted.ToString.ToUpper %></li>
                                    <li>R<sup>2</sup>: <%= R2 %></li>
                                </ul>
                            </div>
                        </div>

                        <p>
                            <img src=<%= New DataURI(image) %> style="width: 65%;"/>
                        </p>

                        <h3>Reference Points</h3>

                        <p>%s</p>
                        <p>{$samples}</p>

                        <hr/>
                    </div>
                </div>
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
                        Dim R2 = line.linear.CorrelationCoefficient

                        Return <tr class=<%= If(R2 < 0.99, If(R2 < 0.9, "critical", "warning"), "") %>>
                                   <td>
                                       <a href=<%= "#" & line.name %>><%= line.name %></a>
                                   </td>
                                   <td><%= line.points(Scan0).Name %></td>
                                   <td><%= line.linear.Polynomial.ToString("G5", False) %></td>
                                   <td><%= line.linear.CorrelationCoefficient %></td>
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
