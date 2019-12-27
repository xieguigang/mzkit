
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Visualization

<Package("mzkit.quantify.visual")>
Module Visual

    <ExportAPI("standard_curve")>
    Public Function StandardCurve(model As FitModel, Optional name$ = "", Optional samples As NamedValue(Of Double)() = Nothing) As GraphicsData
        Return StandardCurvesPlot.StandardCurves(model, samples, name)
    End Function
End Module
