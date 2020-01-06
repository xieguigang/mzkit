Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder

Module MRMLinearReport

    Public Function CreateHtml(obj As Object) As String
        Dim standardCurves As StandardCurve() = DirectCast(obj, StandardCurve())
        Dim report As ScriptBuilder = getBlankReport()

        Return report.doReport(standardCurves)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function getBlankReport() As ScriptBuilder
        Return New ScriptBuilder(
            <html>
                <head>
                    <title>Linear Models</title>
                </head>
                <body>

                </body>
            </html>)
    End Function

    <Extension>
    Private Function doReport(report As ScriptBuilder, standardCurves As StandardCurve()) As String
        For Each line As StandardCurve In standardCurves

        Next

        Return report.ToString
    End Function
End Module
