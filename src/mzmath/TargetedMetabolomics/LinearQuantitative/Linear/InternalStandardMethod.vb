﻿#Region "Microsoft.VisualBasic::05f86e9099b77002c915cb6d7c4801d7, mzmath\TargetedMetabolomics\LinearQuantitative\Linear\InternalStandardMethod.vb"

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

    '   Total Lines: 189
    '    Code Lines: 135 (71.43%)
    ' Comment Lines: 31 (16.40%)
    '    - Xml Docs: 90.32%
    ' 
    '   Blank Lines: 23 (12.17%)
    '     File Size: 7.93 KB


    '     Class InternalStandardMethod
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateModel, (+2 Overloads) ToFeatureLinear, ToLinears, TPA
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports std = System.Math

Namespace LinearQuantitative.Linear

    ''' <summary>
    ''' 普通标准曲线计算算法模块
    ''' </summary>
    Public Class InternalStandardMethod

        ReadOnly contents As ContentTable
        ReadOnly baselineQuantile As Double = 0.65
        ReadOnly maxDeletions As Integer = 1
        ReadOnly integrator As PeakAreaMethods

        Sub New(contents As ContentTable, integrator As PeakAreaMethods,
                Optional baselineQuantile As Double = 0.65,
                Optional maxDeletions As Integer = 1)

            Me.maxDeletions = maxDeletions
            Me.contents = contents
            Me.baselineQuantile = baselineQuantile
            Me.integrator = integrator
        End Sub

        ''' <summary>
        ''' linear regression
        ''' </summary>
        ''' <param name="points"></param>
        ''' <returns></returns>
        Public Iterator Function ToLinears(points As IEnumerable(Of TargetPeakPoint)) As IEnumerable(Of StandardCurve)
            Dim ionGroups As Dictionary(Of String, TargetPeakPoint()) = points _
                .GroupBy(Function(p) p.Name) _
                .ToDictionary(Function(ion) ion.Key,
                              Function(samples)
                                  Return samples.ToArray
                              End Function)
            Dim linearSamples As String() = ionGroups.Values _
                .IteratesALL _
                .Select(Function(p) p.SampleName) _
                .Distinct _
                .ToArray

            For Each featureId As String In ionGroups.Keys
                If contents.hasDefined(featureId) Then
                    Yield ToFeatureLinear(ionGroups, featureId, linearSamples)
                ElseIf contents.hasISDefined(featureId) Then
                    ' 跳过内标
                Else
                    Throw New MissingPrimaryKeyException($"missing linear information of the metabolite: {featureId}!")
                End If
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ionGroups">
        ''' single compound reference points data
        ''' </param>
        ''' <param name="ionKey"></param>
        ''' <returns></returns>
        Public Function ToFeatureLinear(ionGroups As IEnumerable(Of IonPeakTableRow), ionKey As String) As StandardCurve
            Dim define As Standards = contents.GetStandards(ionKey)
            Dim compound As IonPeakTableRow() = ionGroups.ToArray
            Dim A As Double() = compound.Select(Function(i) i.TPA).ToArray
            Dim ISTPA As Double() = compound.Select(Function(i) i.TPA_IS).ToArray
            Dim linearSamples As String() = compound.Select(Function(i) i.raw).ToArray

            Return CreateModel(linearSamples, A, ISTPA, define)
        End Function

        Private Function CreateModel(linearSamples As String(), A#(), ISTPA#(), define As Standards) As StandardCurve
            Dim ionKey As String = define.ID
            Dim C As Double() = linearSamples.Select(Function(level) contents(level, ionKey)).ToArray
            Dim CIS As Double = 1
            Dim invalids As New List(Of PointF)
            Dim points As New List(Of ReferencePoint)
            Dim line As PointF() = QuantificationWorker _
                .CreateModelPoints(C, A, ISTPA, CIS, ionKey, define.Name, linearSamples, points) _
                .ToArray
            Dim fit As IFitted = StandardCurve.CreateLinearRegression(line, maxDeletions, removed:=invalids)

            ' get points that removed from linear modelling
            For Each ptRef As ReferencePoint In points
                For Each invalid In invalids
                    If std.Abs(invalid.X - ptRef.Cti) <= 0.0001 AndAlso std.Abs(invalid.Y - ptRef.Px) <= 0.0001 Then
                        ptRef.valid = False
                        Exit For
                    End If
                Next
            Next

            Dim out As New StandardCurve With {
                .name = ionKey,
                .linear = fit,
                .points = points _
                    .OrderBy(Function(p) contents(p.level, ionKey)) _
                    .ToArray,
                .[IS] = contents.GetIS(define.ISTD)
            }
            Dim fy As Func(Of Double, Double) = out.ReverseModelFunction
            Dim ptY#

            For Each pt As ReferencePoint In out.points
                If pt.AIS > 0 Then
                    ptY = pt.Ati / pt.AIS
                Else
                    ptY = pt.Ati
                End If

                pt.yfit = std.Round(fy(ptY), 5)
            Next

            Return out
        End Function

        ''' <summary>
        ''' linear regression
        ''' </summary>
        ''' <param name="ionGroups"></param>
        ''' <param name="ionKey"></param>
        ''' <param name="linearSamples"></param>
        ''' <returns></returns>
        Private Function ToFeatureLinear(ionGroups As Dictionary(Of String, TargetPeakPoint()),
                                         ionKey As String,
                                         linearSamples As String()) As StandardCurve

            Dim define As Standards = contents.GetStandards(ionKey)
            Dim rawPoints As TargetPeakPoint() = ionGroups(ionKey)
            Dim points As New List(Of ReferencePoint)
            Dim A As Double() = TPA(linearSamples, rawPoints)
            Dim ISTPA As Double()

            If (Not define.ISTD.StringEmpty(, True)) AndAlso Not define.ISTD.TextEquals("None") Then
                If Not ionGroups.ContainsKey(define.ISTD) Then
                    Call $"target linear required IS reference '{define.ISTD}', but ion is missing in sample data!".Warning
                    ISTPA = Nothing
                Else
                    ISTPA = TPA(linearSamples, ionGroups(define.ISTD))
                End If
            Else
                ISTPA = Nothing
            End If

            Return CreateModel(linearSamples, A, ISTPA, define)
        End Function

        ''' <summary>
        ''' 计算峰面积
        ''' </summary>
        ''' <param name="linearSamples"></param>
        ''' <param name="points"></param>
        ''' <returns></returns>
        Private Function TPA(linearSamples As String(), points As IEnumerable(Of TargetPeakPoint)) As Double()
            Dim getByLevels As Dictionary(Of String, TargetPeakPoint) = points.ToDictionary(Function(p) p.SampleName)
            Dim vec As New List(Of Double)
            Dim deconv As (area#, baseline#, maxPeakHeight#)
            Dim target As TargetPeakPoint
            Dim dataPeak As ChromatogramTick()

            For Each sampleLevel As String In linearSamples
                If Not getByLevels.ContainsKey(sampleLevel) Then
                    vec.Add(0)
                Else
                    target = getByLevels(sampleLevel)
                    dataPeak = target.Peak.ticks
                    deconv = dataPeak _
                        .Shadows _
                        .TPAIntegrator(
                            peak:=target.Peak,
                            baselineQuantile:=baselineQuantile,
                            peakAreaMethod:=integrator
                        )

                    vec.Add(deconv.area)
                End If
            Next

            Return vec.ToArray
        End Function
    End Class
End Namespace
