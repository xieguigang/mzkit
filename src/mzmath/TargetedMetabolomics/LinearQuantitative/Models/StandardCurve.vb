﻿#Region "Microsoft.VisualBasic::8cf89ebdba210387bf2780af3bcdbaea, mzmath\TargetedMetabolomics\LinearQuantitative\Models\StandardCurve.vb"

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

    '   Total Lines: 128
    '    Code Lines: 83 (64.84%)
    ' Comment Lines: 27 (21.09%)
    '    - Xml Docs: 85.19%
    ' 
    '   Blank Lines: 18 (14.06%)
    '     File Size: 4.75 KB


    '     Class StandardCurve
    ' 
    '         Properties: [IS], arguments, blankControls, isValid, isWeighted
    '                     linear, name, points, requireISCalibration
    ' 
    '         Function: CreateLinearRegression, GetModelFlip, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace LinearQuantitative

    ''' <summary>
    ''' The linear model of the targeted metabolism model data.
    ''' </summary>
    ''' <remarks>
    ''' (标准曲线模型)
    ''' </remarks>
    Public Class StandardCurve : Implements INamedValue

        ''' <summary>
        ''' The metabolite name or database id
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String Implements IKeyedEntity(Of String).Key
        ''' <summary>
        ''' 该代谢物的线性回归模型
        ''' </summary>
        ''' <returns></returns>
        Public Property linear As IFitted
        Public Property points As ReferencePoint()

        Public Property blankControls As Double()

        ''' <summary>
        ''' The internal standards
        ''' </summary>
        ''' <returns></returns>
        Public Property [IS] As [IS]

        Public Property arguments As Object

        ''' <summary>
        ''' This linear model is required calibration by internal standards or not?
        ''' (在进行线性回归计算的时候是否需要内标校正？)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property requireISCalibration As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                ' 20211123 当忘记填写浓度值的时候
                ' 样本定量过程中会出现浓度值异常高的bug
                ' 所以在这里将CIS大于零的判断删除
                Return Not [IS] Is Nothing AndAlso Not [IS].ID.StringEmpty ' AndAlso [IS].CIS > 0
            End Get
        End Property

        Public ReadOnly Property isWeighted As Boolean
            Get
                Return TypeOf linear Is WeightedFit
            End Get
        End Property

        Public ReadOnly Property isValid As Boolean
            Get
                Return Not linear.CorrelationCoefficient.IsNaNImaginary
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{name}] {linear}"
        End Function

        Public Function GetModelFlip() As Formula
            Dim fx = linear.Polynomial
            Dim a = fx.Factors(0)
            Dim b = fx.Factors(1)
            Dim flip_a = -a / b
            Dim flip_b = 1 / b
            Dim flip_linear As New Polynomial(flip_a, flip_b)

            Return flip_linear
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateLinearRegression(points As IEnumerable(Of PointF), maxDeletions%, ByRef removed As List(Of PointF)) As IFitted
            Dim deletes As New List(Of PointF)(removed.SafeQuery)
            Dim rawPoints As PointF() = points.ToArray
            Dim fit As IFitted = rawPoints.AutoPointDeletion(
                weighted:=Function(X) 1 / (X ^ 2),
                max:=maxDeletions,
                removed:=deletes,
                keepsLowestPoint:=True,
                removesZeroY:=True
            )

            If fit Is Nothing Then

                ' 完全没有线性
                Return New FitResult With {
                    .ErrorTest = rawPoints _
                        .Select(Function(p)
                                    Return DirectCast(New TestPoint With {.X = p.X, .Y = p.Y, .Yfit = 99999999}, IFitError)
                                End Function) _
                        .ToArray,
                    .RMSE = 9999999,
                    .SSE = 9999999,
                    .SSR = 9999999,
                    .Polynomial = New Polynomial With {.Factors = New Double() {0, 0}}
                }

            ElseIf fit.R2 < 0.95 Then
                deletes = New List(Of PointF)(removed.SafeQuery)
                fit = rawPoints.AutoPointDeletion(
                    weighted:=Function(X) 1 / (X ^ 2),
                    max:=maxDeletions,
                    removed:=deletes,
                    keepsLowestPoint:=False,
                    removesZeroY:=True
                )
            End If

            removed = deletes

            Return fit
        End Function
    End Class
End Namespace
