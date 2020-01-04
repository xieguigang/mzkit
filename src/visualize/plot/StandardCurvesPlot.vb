#Region "Microsoft.VisualBasic::5a1279991f01f3ee9b8d1ed4a6bc0ceb, src\visualize\plot\StandardCurvesPlot.vb"

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

' Module StandardCurvesPlot
' 
'     Function: StandardCurves
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics
Imports Microsoft.VisualBasic.Imaging.Driver

Public Module StandardCurvesPlot

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function StandardCurves(model As StandardCurve,
                                   Optional samples As IEnumerable(Of NamedValue(Of Double)) = Nothing,
                                   Optional name$ = "",
                                   Optional size$ = "1600,1200",
                                   Optional margin$ = "padding: 100px 100px 100px 200px",
                                   Optional factorFormat$ = "G4") As GraphicsData

        If model.requireISCalibration Then
            ' 如果进行内标校正的话，则应该是[峰面积比, 浓度比]之间的线性关系
            Return model _
                .linear _
                .Plot(xLabel:="(CIS/Cti u mol/L) ratio",
                      yLabel:="Peak area ratio (AIS/Ati)",
                      size:=size,
                      predictedX:=samples,
                      xAxisTickFormat:="F2",
                      yAxisTickFormat:="F2",
                      showErrorBand:=False,
                      title:=name,
                      margin:=margin,
                      factorFormat:=factorFormat
                )
        Else
            ' 如果不做内标校正的话，则是直接[峰面积, 浓度]之间的线性关系了
            Return model _
                .linear _
                .Plot(xLabel:="Cti u mol/L",
                      yLabel:="Peak area(Ati)",
                      size:=size,
                      predictedX:=samples,
                      xAxisTickFormat:="G2",
                      yAxisTickFormat:="G2",
                      showErrorBand:=False,
                      title:=name,
                      margin:=margin,
                      factorFormat:=factorFormat
                )
        End If
    End Function
End Module
