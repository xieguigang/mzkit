Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.MassSpectrum.Math.MRM

Public Module viz

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function StandardCurves(model As FitModel,
                                   Optional samples As IEnumerable(Of NamedValue(Of Double)) = Nothing,
                                   Optional name$ = "") As GraphicsData

        If model.RequireISCalibration Then
            ' 如果进行内标校正的话，则应该是[峰面积比, 浓度比]之间的线性关系
            Return model _
                .LinearRegression _
                .Plot(xLabel:="Peak area ratio (AIS/Ati)",
                      yLabel:="(CIS/Cti u mol/L) ratio",
                      size:="1600,1100",
                      predictedX:=samples,
                      xAxisTickFormat:="F2",
                      yAxisTickFormat:="F0",
                      showErrorBand:=False,
                      title:=name,
                      margin:="padding: 100px 100px 100px 200px"
                )
        Else
            ' 如果不做内标校正的话，则是直接[峰面积, 浓度]之间的线性关系了
            Return model _
                .LinearRegression _
                .Plot(xLabel:="Peak area(Ati)",
                      yLabel:="Cti u mol/L",
                      size:="1600,1100",
                      predictedX:=samples,
                      xAxisTickFormat:="G2",
                      yAxisTickFormat:="F0",
                      showErrorBand:=False,
                      title:=name,
                      margin:="padding: 100px 100px 100px 200px"
                )
        End If
    End Function
End Module
