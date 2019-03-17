#Region "Microsoft.VisualBasic::2e985b8ccb25e935c4990fd4ced6fb5d, ms2_math-core\Ms1\PrecursorType\MzCalculator.vb"

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

    '     Structure MzCalculator
    ' 
    '         Properties: adducts, charge, IsEmpty, M, mode
    '                     name
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AdductMass, CalcMass, CalcPrecursorMZ, ReverseMass, (+2 Overloads) ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports sys = System.Math

Namespace Ms1.PrecursorType

    Public Structure MzCalculator

        Public Property name As String
        Public Property charge As Integer
        Public Property M As Integer

        ''' <summary>
        ''' 是可能会出现负数的加和结果，例如[M-H2O]的adducts为-18
        ''' </summary>
        Public Property adducts As Double
        Public Property mode As Char

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return name.StringEmpty AndAlso
                             charge = 0 AndAlso
                                  M = 0 AndAlso
                            adducts = 0 AndAlso
                               mode = ASCII.NUL
            End Get
        End Property

        Sub New(type$, charge%, M#, adducts#)
            Me.name = type
            Me.charge = charge
            Me.M = M
            Me.adducts = adducts
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CalcMass(precursorMZ#) As Double
            Return (ReverseMass(precursorMZ, M, charge, adducts))
        End Function

        ''' <summary>
        ''' 通过所给定的精确分子质量计算出``m/z``
        ''' </summary>
        ''' <param name="mass#"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CalcMZ(mass#) As Double
            Return (AdductMZ(mass * M, adducts, charge))
        End Function

        Public Overrides Function ToString() As String
            If InStr(name, "[") < InStr(name, "]") AndAlso InStr(name, "[") > 0 Then
                Return name
            Else
                Return $"[{name}]{If(charge = 1, "", charge)}{mode}"
            End If
        End Function

        Public Overloads Function ToString(ionization_mode As Long) As String
            If IsEmpty Then
                Return "[M]+" Or "[M]-".When(ionization_mode < 0)
            Else
                Return Me.ToString
            End If
        End Function

        ''' <summary>
        ''' 返回加和物的m/z数据
        ''' </summary>
        ''' <param name="mass#"></param>
        ''' <param name="adduct#"></param>
        ''' <param name="charge%"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function AdductMZ(mass#, adduct#, charge%) As Double
            Return (mass / sys.Abs(charge) + adduct)
        End Function

        ''' <summary>
        ''' 从质谱的MS/MS的前体的m/z结果反推目标分子的mass结果
        ''' </summary>
        ''' <param name="mz#"></param>
        ''' <param name="M#"></param>
        ''' <param name="charge%"></param>
        ''' <param name="adduct#"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ReverseMass(mz#, M#, charge%, adduct#) As Double
            Return ((mz - adduct) * sys.Abs(charge) / M)
        End Function

        Public Shared Iterator Function CalculateMode(mass#, mode As String) As IEnumerable(Of MzReport)
            For Each type In Provider.Calculator(mode).Values
                Yield New MzReport With {
                    .Adduct = type.adducts,
                    .Charge = type.charge,
                    .M = type.M,
                    .mz = type.CalcMZ(mass),
                    .PrecursorType = type.name
                }
            Next
        End Function
    End Structure

    Public Structure MzReport
        Dim PrecursorType As String
        Dim Charge As Double
        Dim M As Double
        Dim Adduct As Double
        Dim mz As Double
    End Structure
End Namespace
