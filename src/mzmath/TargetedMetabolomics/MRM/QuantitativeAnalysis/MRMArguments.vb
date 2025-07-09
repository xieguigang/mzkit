#Region "Microsoft.VisualBasic::0980615ad532529230e7898d0a48fb44, mzmath\TargetedMetabolomics\MRM\QuantitativeAnalysis\MRMArguments.vb"

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

    '   Total Lines: 137
    '    Code Lines: 110 (80.29%)
    ' Comment Lines: 9 (6.57%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 18 (13.14%)
    '     File Size: 5.63 KB


    '     Class MRMArguments
    ' 
    '         Properties: angleThreshold, baselineQuantile, bspline, bspline_degree, bspline_density
    '                     integratorTicks, joint_peaks, peakAreaMethod, peakwidth, sn_threshold
    '                     strict, timeWindowSize, tolerance, TPAFactors
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: FromJSON, GetDefaultArguments, ToJSON, ToString
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

    Public Interface IArgumentSet
        Function GetArgument(id As String) As MRMArguments
    End Interface

    Public Class ArgumentSet : Implements IArgumentSet

        Public Property args As Dictionary(Of String, MRMArguments)
        ''' <summary>
        ''' unify globals argument set
        ''' </summary>
        ''' <returns></returns>
        Public Property globals As MRMArguments

        Public Function GetArgument(id As String) As MRMArguments Implements IArgumentSet.GetArgument
            Return args.TryGetValue(id, [default]:=globals)
        End Function
    End Class

    Public Class MRMArguments : Implements IArgumentSet

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
        Public Property bspline_density As Integer = 5
        Public Property bspline As Boolean = False

        Public Property joint_peaks As Boolean = True
        Public Property strict As Boolean = False
        Public Property time_shift_method As Boolean = False

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
                joint_peaks As Boolean,
                time_shift_method As Boolean)

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
            Me.time_shift_method = time_shift_method
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
                joint_peaks:=True,
                time_shift_method:=False
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
                {"strict", strict.ToString},
                {"bspline", bspline.ToString},
                {"time_shift_method", time_shift_method.ToString}
            }

            For Each factor In TPAFactors.SafeQuery
                json($"factor:{factor.Key}") = factor.Value
            Next

            Return json.GetJson
        End Function

        Public Shared Function FromJSON(json_str As String) As MRMArguments
            Dim json As Dictionary(Of String, String) = json_str.LoadJSON(Of Dictionary(Of String, String))
            Dim args As New MRMArguments

            args.tolerance = Tolerance.ParseScript(json!tolerance)
            args.timeWindowSize = json!timeWindowSize
            args.angleThreshold = json!angleThreshold
            args.baselineQuantile = json!baselineQuantile
            args.integratorTicks = json!integratorTicks
            args.peakAreaMethod = [Enum].Parse(GetType(PeakAreaMethods), json!peakAreaMethod)
            args.peakwidth = New DoubleRange(json!peakmin, json!peakmax)
            args.sn_threshold = json!sn_threshold
            args.bspline_degree = json!bspline_degree
            args.bspline_density = json!bspline_density
            args.bspline = json!bspline.ParseBoolean
            args.joint_peaks = json!joint_peaks.ParseBoolean
            args.strict = json!strict.ParseBoolean
            args.time_shift_method = json!time_shift_method.ParseBoolean

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

        ''' <summary>
        ''' unify globals argument set
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function GetArgument(id As String) As MRMArguments Implements IArgumentSet.GetArgument
            Return Me
        End Function
    End Class
End Namespace
