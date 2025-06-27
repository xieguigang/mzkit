﻿#Region "Microsoft.VisualBasic::39f9c3bf2e03852713a3beac853f104e, mzmath\TargetedMetabolomics\MRM\QuantitativeAnalysis\MRMArguments.vb"

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


    ' Code Statistics:

    '   Total Lines: 81
    '    Code Lines: 62 (76.54%)
    ' Comment Lines: 8 (9.88%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (13.58%)
    '     File Size: 3.19 KB


    '     Class MRMArguments
    ' 
    '         Properties: angleThreshold, baselineQuantile, bspline_degree, bspline_density, integratorTicks
    '                     peakAreaMethod, peakwidth, sn_threshold, strict, timeWindowSize
    '                     tolerance, TPAFactors
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetDefaultArguments, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MRM

    Public Class MRMArguments

        ''' <summary>
        ''' ``{<see cref="Standards.ID"/>, <see cref="Standards.Factor"/>}``，
        ''' 这个是为了计算亮氨酸和异亮氨酸这类无法被区分的物质的峰面积所需要的
        ''' </summary>
        ''' <returns></returns>
        Public Property TPAFactors As Dictionary(Of String, Double)
        ''' <summary>
        ''' m/z tolerance for match ion pair
        ''' </summary>
        ''' <returns></returns>
        Public Property tolerance As Tolerance
        Public Property timeWindowSize As Double
        Public Property angleThreshold#
        Public Property baselineQuantile# = 0.65
        Public Property integratorTicks% = 5000
        Public Property peakAreaMethod As PeakAreaMethods = PeakAreaMethods.Integrator
        Public Property peakwidth As DoubleRange = Nothing
        Public Property sn_threshold As Double = 3

        Public Property bspline_degree As Integer = 2
        Public Property bspline_density As Integer = 100

        Public Property joint_peaks As Boolean = True
        Public Property strict As Boolean = False

        Sub New()
        End Sub

        <DebuggerStepThrough>
        Sub New(TPAFactors As Dictionary(Of String, Double),
                tolerance As Tolerance,
                timeWindowSize#,
                angleThreshold#,
                baselineQuantile#,
                integratorTicks%,
                peakAreaMethod As PeakAreaMethods,
                peakwidth As DoubleRange,
                sn_threshold As Double,
                joint_peaks As Boolean)

            Me.TPAFactors = TPAFactors
            Me.tolerance = tolerance
            Me.timeWindowSize = timeWindowSize
            Me.angleThreshold = angleThreshold
            Me.baselineQuantile = baselineQuantile
            Me.integratorTicks = integratorTicks
            Me.peakAreaMethod = peakAreaMethod
            Me.peakwidth = peakwidth
            Me.sn_threshold = sn_threshold
            Me.joint_peaks = joint_peaks
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
                peakwidth:=New Double() {8, 30},
                sn_threshold:=3,
                joint_peaks:=True
            )
        End Function

        Public Function ToJSON() As String
            Dim json As New Dictionary(Of String, String) From {
                {"tolerance", tolerance.GetScript},
                {"timeWindowSize", timeWindowSize},
                {"angleThreshold", angleThreshold},
                {"baselineQuantile", baselineQuantile},
                {"integratorTicks", integratorTicks},
                {"peakAreaMethod", peakAreaMethod.ToString},
                {"peakmin", peakwidth.Min},
                {"peakmax", peakwidth.Max},
                {"sn_threshold", sn_threshold},
                {"bspline_degree", bspline_degree},
                {"bspline_density", bspline_density},
                {"joint_peaks", joint_peaks.ToString},
                {"strict", strict.ToString}
            }

            For Each factor In TPAFactors.SafeQuery
                json($"factor:{factor.Key}") = factor.Value
            Next

            Return json.GetJson
        End Function

        Public Shared Function FromJSON(json_str As String) As MRMArguments
            Dim json As Dictionary(Of String, String) = json_str.LoadJSON(Of Dictionary(Of String, String))
            Dim args As New MRMArguments



            Return args
        End Function

        Public Overrides Function ToString() As String
            Dim porperties As PropertyInfo() = GetType(MRMArguments).GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            Dim json As New Dictionary(Of String, Object)

            For Each p In porperties
                json.Add(p.Name, p.GetValue(Me))
            Next

            Return json.GetJson
        End Function
    End Class
End Namespace
