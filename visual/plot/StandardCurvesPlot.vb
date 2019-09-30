#Region "Microsoft.VisualBasic::79f77cb483b0aaafd3d974b50b0dab31, plot\viz.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module viz
    ' 
    '     Function: StandardCurves
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.MassSpectrum.Math

Public Module StandardCurvesPlot

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
