﻿#Region "Microsoft.VisualBasic::1c7059c03e013d176abd762ab7fddbe1, mzmath\ms2_math-core\Ms1\Tolerance\Tolerance.vb"

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

    '   Total Lines: 335
    '    Code Lines: 161 (48.06%)
    ' Comment Lines: 135 (40.30%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 39 (11.64%)
    '     File Size: 12.41 KB


    '     Enum MassToleranceType
    ' 
    '         Da, Ppm
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class Tolerance
    ' 
    '         Properties: [Interface], DefaultTolerance
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AddPPM, CheckScriptText, Compares, DeltaMass, Equals
    '                   GetHashCode, GetScript, MatchTolerance, ParseScript, PPM
    '                   SubPPM, (+2 Overloads) ToScript
    '         Operators: *, /, <, >
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Math
Imports std = System.Math

Namespace Ms1

    ''' <summary>
    ''' Type of the mass <see cref="Tolerance"/>
    ''' </summary>
    Public Enum MassToleranceType
        ''' <summary>
        ''' <see cref="DAmethod"/>
        ''' </summary>
        Da
        ''' <summary>
        ''' <see cref="PPMmethod"/>
        ''' </summary>
        Ppm
    End Enum

    ''' <summary>
    ''' The m/z tolerance methods, spectrum equality comparer.
    ''' </summary>
    ''' <remarks>
    ''' (可以直接使用这个对象的索引属性来进行计算判断,索引属性表示两个``m/z``值之间是否相等)
    ''' </remarks>
    Public MustInherit Class Tolerance : Inherits NumberEqualityComparer
        Implements IEqualityComparer(Of ISpectrumPeak)

        ''' <summary>
        ''' <see cref="DeltaTolerance"/>(分子质量误差的上限值)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property [Interface] As Tolerance
            Get
                Return Me
            End Get
        End Property

        ''' <summary>
        ''' ppm method or dalton method
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property Type As MassToleranceType

        Default Public ReadOnly Property IsEquals(mz1#, mz2#) As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Equals(mz1, mz2)
            End Get
        End Property

        Default Public ReadOnly Property IsEquals(mz1 As Spectra.ms2, mz2 As Spectra.ms2) As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Equals(mz1.mz, mz2.mz)
            End Get
        End Property

        ''' <summary>
        ''' 默认的误差计算是小于0.3个道尔顿以内
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property DefaultTolerance As [Default](Of Tolerance)

        Shared Sub New()
            DefaultTolerance = New DAmethod(0.3).Interface
        End Sub

        ''' <summary>
        ''' 判断两个分子质量是否一致
        ''' </summary>
        ''' <param name="mz1"></param>
        ''' <param name="mz2"></param>
        ''' <returns></returns>
        Public MustOverride Overrides Function Equals(mz1 As Double, mz2 As Double) As Boolean

        Protected Shared ReadOnly sample_mz As Double() = New Double() {
            50, 100, 200, 300,
            400, 500, 600, 700, 800, 900,
            1000,
            2000
        }

        ''' <summary>
        ''' try to convert the mass dalton error as ppm error for 
        ''' compares the da tolerance with the ppm tolerance in 
        ''' some of the situation
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetErrorPPM() As Double
        Public MustOverride Function GetErrorDalton() As Double

        ''' <summary>
        ''' 将分子质量误差值转换为百分比得分
        ''' </summary>
        ''' <param name="mz1#"></param>
        ''' <param name="mz2#"></param>
        ''' <returns></returns>
        Public MustOverride Function AsScore(mz1#, mz2#) As Double
        ''' <summary>
        ''' 计算出两个分子质量之间的误差值的大小
        ''' </summary>
        ''' <param name="mz1#"></param>
        ''' <param name="mz2#"></param>
        ''' <returns></returns>
        Public MustOverride Function MassError(mz1#, mz2#) As Double
        Public MustOverride Function MassErrorDescription(mz1#, mz2#) As String

        Protected MustOverride Function Scale(scaler As Double) As Tolerance

        ''' <summary>
        ''' Represents the method that compares two mass value.
        ''' </summary>
        ''' <param name="mass1"></param>
        ''' <param name="mass2"></param>
        ''' <returns>
        ''' A signed integer that indicates the relative values of x and y, as shown in the
        ''' following table.
        ''' 
        ''' |Value         |Meaning             |
        ''' |--------------|--------------------|
        ''' |Less than 0   |x is less than y.   |
        ''' |0             |x equals y.         |
        ''' |Greater than 0|x is greater than y.|
        ''' </returns>
        Public Function Compares(mass1 As Double, mass2 As Double) As Integer
            If Equals(mass1, mass2) Then
                Return 0
            ElseIf mass1 > mass2 Then
                Return 1
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' 判断目标分子质量误差是否符合当前的误差要求
        ''' </summary>
        ''' <param name="error"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function MatchTolerance([error] As Double) As Boolean
            Return std.Abs([error]) <= DeltaTolerance
        End Function

        Public Function GetScript() As String
            Return ToScript(Me)
        End Function

        <DebuggerStepThrough>
        Public Shared Function DeltaMass(da#) As DAmethod
            Return New DAmethod(da)
        End Function

        ''' <summary>
        ''' create the ppm tolerance error object
        ''' </summary>
        ''' <param name="value#"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function PPM(value#) As PPMmethod
            Return New PPMmethod(value)
        End Function

        ''' <summary>
        ''' 将当前的质量值减去给定的ppm质量差
        ''' </summary>
        ''' <param name="mass#"></param>
        ''' <param name="ppm#"></param>
        ''' <returns></returns>
        Public Shared Function SubPPM(mass#, ppm#) As Double
            Dim ppmd As Double = ppm / 1000000
            ' sys.Abs(measured - actualValue) 
            ppmd = ppmd * mass
            mass = mass - ppmd

            Return mass
        End Function

        ''' <summary>
        ''' 将当前的质量值加上给定的ppm质量差
        ''' </summary>
        ''' <param name="mass#"></param>
        ''' <param name="ppm#"></param>
        ''' <returns></returns>
        Public Shared Function AddPPM(mass#, ppm#) As Double
            Dim ppmd As Double = ppm / 1000000
            ' sys.Abs(measured - actualValue) 
            ppmd = ppmd * mass
            mass = mass + ppmd

            Return mass
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(tolerance As Tolerance) As Comparison(Of Double)
            Return Function(mz1, mz2) As Integer
                       If tolerance(mz1, mz2) Then
                           Return 0
                       ElseIf mz1 > mz2 Then
                           Return 1
                       Else
                           Return -1
                       End If
                   End Function
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(tolerance As Tolerance) As GenericLambda(Of Double).IEquals
            Return AddressOf tolerance.Equals
        End Operator

        ''' <summary>
        ''' Parse the given string value as the tolerance error object
        ''' 
        ''' + da:xxx
        ''' + ppm:xxx
        ''' </summary>
        ''' <param name="script">
        ''' the tolerance script value text also could be a simple number:
        ''' 
        ''' 1. if less than 1, means da:xxx
        ''' 2. if greater than or equals to 1, means ppm:xxx
        ''' </param>
        ''' <returns>
        ''' this function will returns nothing if the given script string is in in-correct format.
        ''' </returns>
        Public Shared Function ParseScript(script As String) As Tolerance
            Dim tokens = script.GetTagValue(":", trim:=True)
            Dim method = tokens.Name.ToLower
            Dim tolerance# = tokens.Value.ParseDouble

            If method.StringEmpty Then
                If tolerance = 0.0 Then
                    ' method is empty andalso tolerance is zero
                    ' means string in invalid format
                    ' returns nothing directly
                    Call $"the given string({script}) that represent the tolerance error in invalid format!".Warning
                    Return Nothing
                End If

                ' 20230608 when the given script text value just a number, then
                '
                ' + less than 1, means da tolerance error
                ' + greater than or equals to 1, means ppm tolerance error
                '
                If tolerance < 1 Then
                    Return DeltaMass(tolerance)
                Else
                    Return PPM(value:=tolerance)
                End If
            End If

            If method = "da" Then
                Return DeltaMass(da:=tolerance)
            Else
                Return PPM(value:=tolerance)
            End If
        End Function

        ''' <summary>
        ''' check of the input script text is in valid format or not
        ''' </summary>
        ''' <param name="script"></param>
        ''' <returns></returns>
        Public Shared Function CheckScriptText(script As String) As Boolean
            Dim tokens = script.GetTagValue(":", trim:=True)
            Dim method = tokens.Name.ToLower
            Dim tolerance# = tokens.Value.ParseDouble

            If (method = "da" OrElse method = "ppm") AndAlso tolerance > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Overloads Shared Function ToScript(err As Tolerance) As String
            If TypeOf err Is DAmethod Then
                Return $"da:{err.DeltaTolerance}"
            Else
                Return $"ppm:{err.DeltaTolerance}"
            End If
        End Function

        Public Overloads Function ToScript() As String
            Return ToScript(Me)
        End Function

        Public Shared Operator *(mzdiff As Tolerance, scale As Double) As Tolerance
            Return mzdiff.Scale(scale)
        End Operator

        Public Shared Operator /(mzdiff As Tolerance, scale As Double) As Tolerance
            Return mzdiff.Scale(1 / scale)
        End Operator

        ''' <summary>
        ''' the tolerance error of <paramref name="d1"/> is greater than the tolerance error of <paramref name="d2"/>?
        ''' </summary>
        ''' <param name="d1"></param>
        ''' <param name="d2"></param>
        ''' <returns></returns>
        Public Shared Operator >(d1 As Tolerance, d2 As Tolerance) As Boolean
            Return d1.GetErrorPPM > d2.GetErrorPPM
        End Operator

        Public Shared Operator <(d1 As Tolerance, d2 As Tolerance) As Boolean
            Return Not d1 > d2
        End Operator

        ''' <summary>
        ''' SpectrumEqualityComparer
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' could be used for group the peaks data via the .net framework internal groupby function by implements this interface.
        ''' </remarks>
        Public Overloads Function Equals(x As ISpectrumPeak, y As ISpectrumPeak) As Boolean Implements IEqualityComparer(Of ISpectrumPeak).Equals
            Return Equals(x.Mass, y.Mass)
        End Function

        Friend Overloads Function GetHashCode(obj As ISpectrumPeak) As Integer Implements IEqualityComparer(Of ISpectrumPeak).GetHashCode
            Return std.Round(obj.Mass, 6).GetHashCode()
        End Function
    End Class
End Namespace
