#Region "Microsoft.VisualBasic::db3afbd293a71e92383ec7dedfd3b52c, src\mzmath\TargetedMetabolomics\QuantitativeAnalysis\StandardCurve.vb"

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

    ' Class StandardCurve
    ' 
    '     Properties: [IS], blankControls, isValid, isWeighted, linear
    '                 name, points, requireISCalibration
    ' 
    '     Function: CreateLinearRegression, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Language

''' <summary>
''' The linear model of the targeted metabolism model data.(标准曲线模型)
''' </summary>
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
    Public Property points As MRMStandards()

    Public Property blankControls As Double()

    ''' <summary>
    ''' The internal standards
    ''' </summary>
    ''' <returns></returns>
    Public Property [IS] As [IS]

    ''' <summary>
    ''' This linear model is required calibration by internal standards or not?
    ''' (在进行线性回归计算的时候是否需要内标校正？)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property requireISCalibration As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Not [IS] Is Nothing AndAlso Not [IS].ID.StringEmpty AndAlso [IS].CIS > 0
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function CreateLinearRegression(points As IEnumerable(Of PointF), weighted As Boolean, maxDeletions%, ByRef removed As List(Of PointF)) As IFitted
        Return points.AutoPointDeletion(weighted, max:=maxDeletions, removed:=removed)
    End Function

End Class
