Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Text.Xml

Module MRMLinearReport

    Public Function CreateHtml(obj As Object) As String
        Dim standardCurves As StandardCurve() = DirectCast(obj, StandardCurve())
        Dim report As ScriptBuilder = getBlankReport()

        Return report.doReport(standardCurves)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function getBlankReport() As ScriptBuilder
        Return New ScriptBuilder(
            <html lang="zh-CN">
                <head>
                    <meta charset="utf-8"/>
                    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no"/>

                    <title>Linear Models</title>

                    <!-- Bootstrap CSS -->
                    <link rel="stylesheet" href="http://cdn.biodeep.cn:8848/styles/bootstrap-4.3.1-dist/css/bootstrap.min.css" crossorigin="anonymous"/>
                </head>
                <body class="container-fluid">
                    <h1>MRM Quantification Linear Models</h1>

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
    Private Function doReport(report As ScriptBuilder, standardCurves As StandardCurve()) As String
        Dim linears As New List(Of XElement)
        Dim image As Image
        Dim title$

        For Each line As StandardCurve In standardCurves
            title = line.points(Scan0).Name
            image = Visual.DrawStandardCurve(line, title).AsGDIImage
            linears +=
                <div class="row">
                    <div class="col-xl-10">
                        <h2><%= title %></h2>

                        <div class="panel panel-success">
                            <div class="panel-heading">Linear</div>
                            <div style="padding:10px">
                                <ul>
                                    <li>ID: <%= line.name %></li>
                                    <li>Linear: <i>f(x)</i>=%s</li>
                                    <li>R<sup>2</sup>: <%= line.linear.CorrelationCoefficient %></li>
                                </ul>
                            </div>
                        </div>

                        <p>
                            <img src=<%= New DataURI(image) %> style="width: 60%;"/>
                        </p>

                        <h3>Reference Points</h3>

                        <p>%s</p>

                        <hr/>
                    </div>
                </div>
        Next

        report("linears") = linears _
            .Select(Function(e, i)
                        Return e.asset(standardCurves(i))
                    End Function) _
            .JoinBy(vbCrLf)

        Return "<!doctype html>" & report.ToString
    End Function

    <Extension>
    Private Function asset(e As XElement, line As StandardCurve) As String
        Dim equation$ = line.linear.Polynomial.ToString("G4", html:=True)
        Dim title = line.points(Scan0).Name
        Dim pointTable$ = line.points.ToHTMLTable(
            className:="table",
            width:="100%",
            title:=title,
            alt:=$"Linear Model Reference Points of '{title}'"
        )

        Return sprintf(e, equation, pointTable)
    End Function
End Module
