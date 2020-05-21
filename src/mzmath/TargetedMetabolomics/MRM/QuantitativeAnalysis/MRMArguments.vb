#Region "Microsoft.VisualBasic::686844ec36e318c7ce126ab4367591e9, src\mzmath\TargetedMetabolomics\MRM\QuantitativeAnalysis\MRMArguments.vb"

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

'     Class MRMArguments
' 
'         Properties: angleThreshold, baselineQuantile, integratorTicks, peakAreaMethod, timeWindowSize
'                     tolerance, TPAFactors
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetDefaultArguments
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace MRM

    Public Class MRMArguments

        ''' <summary>
        ''' ``{<see cref="Standards.ID"/>, <see cref="Standards.Factor"/>}``，这个是为了计算亮氨酸和异亮氨酸这类无法被区分的物质的峰面积所需要的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TPAFactors As Dictionary(Of String, Double)
        Public ReadOnly Property tolerance As Tolerance
        Public ReadOnly Property timeWindowSize#
        Public ReadOnly Property angleThreshold#
        Public ReadOnly Property baselineQuantile# = 0.65
        Public ReadOnly Property integratorTicks% = 5000
        Public ReadOnly Property peakAreaMethod As PeakAreaMethods = PeakAreaMethods.Integrator
        Public ReadOnly Property peakwidth As DoubleRange = Nothing

        Public ReadOnly Property bspline_degree As Integer = 2
        Public ReadOnly Property bspline_density As Integer = 100

        Sub New(TPAFactors As Dictionary(Of String, Double),
                tolerance As Tolerance,
                timeWindowSize#,
                angleThreshold#,
                baselineQuantile#,
                integratorTicks%,
                peakAreaMethod As PeakAreaMethods,
                peakwidth As DoubleRange)

            Me.TPAFactors = TPAFactors
            Me.tolerance = tolerance
            Me.timeWindowSize = timeWindowSize
            Me.angleThreshold = angleThreshold
            Me.baselineQuantile = baselineQuantile
            Me.integratorTicks = integratorTicks
            Me.peakAreaMethod = peakAreaMethod
            Me.peakwidth = peakwidth
        End Sub

        Public Shared Function GetDefaultArguments() As MRMArguments
            Return New MRMArguments(
                TPAFactors:=Nothing,
                tolerance:=Tolerance.DeltaMass(0.3),
                timeWindowSize:=5,
                angleThreshold:=5,
                baselineQuantile:=0.65,
                integratorTicks:=5000,
                peakAreaMethod:=PeakAreaMethods.NetPeakSum,
                peakwidth:={8, 30}
            )
        End Function
    End Class
End Namespace
