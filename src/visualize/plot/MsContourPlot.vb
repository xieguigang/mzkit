Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Driver

Public Module MsContourPlot

    Public Function Plot(ms1 As IEnumerable(Of ms1_scan), Optional size$ = "2700,2100", Optional colorSet$ = "Jet") As GraphicsData
        Dim theme As New Theme With {
            .colorSet = colorSet
        }
        Dim app As New ScanContour(ms1.ToArray, theme)

        Return app.Plot(size, 300)
    End Function
End Module
