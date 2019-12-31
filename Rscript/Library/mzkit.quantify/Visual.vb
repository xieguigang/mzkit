
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.MassSpectrum.Math
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
    Public Function DrawStandardCurve(model As StandardCurve, Optional title$ = "", Optional samples As NamedValue(Of Double)() = Nothing) As GraphicsData
        Return StandardCurvesPlot.StandardCurves(model, samples, title)
    End Function
End Module
