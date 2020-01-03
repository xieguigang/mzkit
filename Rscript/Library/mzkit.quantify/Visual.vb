
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Visualization

<Package("mzkit.quantify.visual")>
Module Visual

    ''' <summary>
    ''' Draw standard curve
    ''' </summary>
    ''' <param name="model">The linear model of the targeted metabolism model data.</param>
    ''' <param name="title">The plot title</param>
    ''' <param name="samples">The point data of samples</param>
    ''' <returns></returns>
    <ExportAPI("standard_curve")>
    Public Function DrawStandardCurve(model As StandardCurve,
                                      Optional title$ = "",
                                      Optional samples As NamedValue(Of Double)() = Nothing,
                                      Optional size$ = "1600,1200",
                                      Optional margin$ = "padding: 200px 100px 200px 200px") As GraphicsData

        Return StandardCurvesPlot.StandardCurves(
            model:=model,
            samples:=samples,
            name:=title,
            size:=size,
            margin:=margin
        )
    End Function

    <ExportAPI("chromatogram.plot")>
    Public Function chromatogramPlot(mzML$, ions As IonPair()) As GraphicsData
        Return ions.MRMChromatogramPlot(mzML)
    End Function

    <ExportAPI("MRM.chromatogramPeaks.plot")>
    Public Function MRMchromatogramPeakPlot(chromatogram As ChromatogramTick(), Optional title$ = "MRM Chromatogram Peak Plot") As GraphicsData
        Return chromatogram.Plot(
            title:=title,
            showMRMRegion:=True,
            showAccumulateLine:=True
        )
    End Function
End Module
