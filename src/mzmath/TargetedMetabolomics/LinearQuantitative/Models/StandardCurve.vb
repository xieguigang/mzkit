#Region "Microsoft.VisualBasic::8cf89ebdba210387bf2780af3bcdbaea, mzmath\TargetedMetabolomics\LinearQuantitative\Models\StandardCurve.vb"

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
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
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
        ''' <summary>
        ''' the weight expression string of the linear regression
        ''' </summary>
        ''' <returns></returns>
        Public Property weight As String = "n/a"
        Public Property blankControls As Double()

        ''' <summary>
        ''' The internal standards
        ''' </summary>
        ''' <returns></returns>
        Public Property [IS] As [IS]

        Public Property arguments As Object

        Public ReadOnly Property range As DoubleRange
            Get
                Return New DoubleRange(From pt As ReferencePoint In points Select pt.Cti)
            End Get
        End Property

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

        Shared ReadOnly weights As New Dictionary(Of String, Weights) From {
            {"1/x2", New Weights(Function(X) 1 / (X ^ 2))},
            {"1/x", New Weights(Function(X) 1 / X)},
            {"exp(-x)", New Weights(Function(X) Vector.Exp(-X))}
        }

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateLinearRegression(points As IEnumerable(Of PointF), maxDeletions%,
                                                      ByRef removed As List(Of PointF),
                                                      ByRef weight As String,
                                                      Optional range As DoubleRange = Nothing) As IFitted

            Dim rawPoints As PointF() = points.ToArray
            Dim best As IFitted = Nothing

            weight = "n/a"

            ' test for each weight and pick for the best
            For Each w As KeyValuePair(Of String, Weights) In weights
                Dim result As IFitted = CreateLinearRegression(rawPoints, maxDeletions, w.Value, removed, range)

                If best Is Nothing OrElse best.R2 < result.R2 Then
                    best = result
                    weight = w.Key

                    If best.R2 > 0.999 Then
                        Exit For
                    End If
                End If
            Next

            Return best
        End Function

        Private Shared Function CreateLinearRegression(rawPoints As PointF(), maxDeletions%, w As Weights,
                                                       ByRef removed As List(Of PointF),
                                                       ByRef range As DoubleRange) As IFitted

            Dim deletes As New List(Of PointF)(removed.SafeQuery)
            Dim fit As IFitted = rawPoints.AutoPointDeletion(
                weighted:=w,
                max:=maxDeletions,
                removed:=deletes,
                keepsLowestPoint:=True,
                removesZeroY:=True,
                range:=range
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
                    weighted:=w,
                    max:=maxDeletions,
                    removed:=deletes,
                    keepsLowestPoint:=False,
                    removesZeroY:=True,
                    range:=range
                )
            End If

            removed = deletes

            Return fit
        End Function
    End Class
End Namespace
