#Region "Microsoft.VisualBasic::ab51f7b28199f3fb5d1bb0b25f05cd71, E:/mzkit/src/mzmath/ms2_math-core//Ms1/Tolerance/Tolerance.vb"

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

    '   Total Lines: 284
    '    Code Lines: 138
    ' Comment Lines: 112
    '   Blank Lines: 34
    '     File Size: 10.14 KB


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
    '         Function: AddPPM, Compares, DeltaMass, GetScript, MatchTolerance
    '                   ParseScript, PPM, SubPPM, (+2 Overloads) ToScript
    '         Operators: *, /, <, >
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
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
    ''' The m/z tolerance methods.
    ''' (可以直接使用这个对象的索引属性来进行计算判断,索引属性表示两个``m/z``值之间是否相等)
    ''' </summary>
    Public MustInherit Class Tolerance : Inherits NumberEqualityComparer

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
        ''' + da:xxx
        ''' + ppm:xxx
        ''' </summary>
        ''' <param name="script">
        ''' the tolerance script value text also could be a simple number:
        ''' 
        ''' 1. if less than 1, means da:xxx
        ''' 2. if greater than or equals to 1, means ppm:xxx
        ''' </param>
        ''' <returns></returns>
        Public Shared Function ParseScript(script As String) As Tolerance
            Dim tokens = script.GetTagValue(":", trim:=True)
            Dim method = tokens.Name.ToLower
            Dim tolerance# = tokens.Value.ParseDouble

            If method.StringEmpty Then
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
    End Class
End Namespace
