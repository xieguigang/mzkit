Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics
Imports Microsoft.VisualBasic.Imaging.Driver

Module viz

    <Extension>
    Public Function StandardCurves(model As NamedValue(Of FitResult), Optional samples As IEnumerable(Of NamedValue(Of Double)) = Nothing) As GraphicsData
        Return model _
            .Value _
            .Plot(xLabel:="Peak area ratio (AIS/Ati)",
                  yLabel:="(CIS/Cti u mol/L) ratio",
                  size:="1600,1100",
                  predictedX:=samples,
                  xAxisTickDecimal:=-1,
                  yAxisTickDecimal:=-1,
                  showErrorBand:=True
             )
    End Function
End Module
