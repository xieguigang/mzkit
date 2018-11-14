Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.MassSpectrum.Math.MSMS

Public Module MassSpectra

    <Extension>
    Public Function MirrorPlot(library As LibraryMatrix,
                               Optional size$ = "1200,800",
                               Optional margin$ = "padding: 100px 30px 50px 100px;",
                               Optional intoCutoff# = 0.05) As GraphicsData

        Dim mzRange As DoubleRange = library _
            .Select(Function(mass) mass.mz) _
            .ToArray

        library = library / library.Max
        library = library(library!intensity >= intoCutoff)
        library = library * 100

        Dim matrix As (x#, value#)() = library _
            .Select(Function(mass) (mass.mz, mass.intensity)) _
            .ToArray

        Return AlignmentPlot.PlotAlignment(
            matrix, matrix,
            queryName:=library.Name,
            subjectName:=library.Name,
            xrange:=$"{mzRange.Min},{mzRange.Max}",
            yrange:="0,100",
            size:=size, padding:=margin,
            xlab:="M/Z ratio",
            ylab:="Relative Intensity(%)",
            title:="BioDeep™ MS/MS alignment Viewer",
            titleCSS:=CSSFont.Win7Large,
            format:="F0",
            yAxislabelPosition:=YlabelPosition.LeftCenter,
            labelPlotStrength:=0.3
        )
    End Function
End Module
