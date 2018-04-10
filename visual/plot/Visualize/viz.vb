Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics
Imports Microsoft.VisualBasic.Imaging.Driver

Public Module viz

    <Extension>
    Public Function StandardCurves(model As NamedValue(Of IFitted), Optional samples As IEnumerable(Of NamedValue(Of Double)) = Nothing, Optional name$ = "") As GraphicsData
        Return model _
            .Value _
            .Plot(xLabel:="Peak area ratio (AIS/Ati)",
                  yLabel:="(CIS/Cti u mol/L) ratio",
                  size:="1600,1100",
                  predictedX:=samples,
                  xAxisTickDecimal:=-1,
                  yAxisTickDecimal:=-1,
                  showErrorBand:=False,
                  title:=name
             )
    End Function
End Module
