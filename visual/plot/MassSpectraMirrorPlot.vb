Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Spectra

Public Module MassSpectra

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function MirrorPlot(library As LibraryMatrix,
                               Optional size$ = "1200,800",
                               Optional margin$ = "padding: 100px 30px 50px 100px;",
                               Optional intoCutoff# = 0.05) As GraphicsData
        Return AlignMirrorPlot(
            library, library,
            size:=size,
            intoCutoff:=intoCutoff,
            margin:=margin
        )
    End Function

    Public Function AlignMirrorPlot(query As LibraryMatrix, ref As LibraryMatrix,
                                    Optional size$ = "1200,800",
                                    Optional margin$ = "padding: 100px 30px 50px 100px;",
                                    Optional intoCutoff# = 0.05) As GraphicsData

        Dim mzRange As DoubleRange = query _
            .Trim(intoCutoff) _
            .Join(ref.Trim(intoCutoff)) _
            .Select(Function(mass) mass.mz) _
            .CreateAxisTicks
        Dim qMatrix As (x#, into#)() = query.Select(Function(q) (q.mz, q.intensity)).ToArray
        Dim sMatrix As (x#, into#)() = ref.Select(Function(s) (s.mz, s.intensity)).ToArray

        Return AlignmentPlot.PlotAlignment(
            qMatrix, sMatrix,
            queryName:=query.Name,
            subjectName:=ref.Name,
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
