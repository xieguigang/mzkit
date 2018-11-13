#Region "Microsoft.VisualBasic::6b3f81e890211b92d7a09293cb85fd34, ms2_math-core\Ms1\PrecursorType\MzCalculator.vb"

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
    '         Properties: adducts, charge, M, mode, name
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AdductMass, CalcMass, CalcPrecursorMZ, ReverseMass, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

        Sub New(type$, charge%, M#, adducts#)
            Me.name = type
            Me.charge = charge
            Me.M = M
            Me.adducts = adducts
        End Sub

        Public Function CalcMass(precursorMZ#) As Double
            Return (ReverseMass(precursorMZ, M, charge, adducts))
        End Function

        Public Function CalcPrecursorMZ(mass#) As Double
            Return (AdductMass(mass, adducts, charge))
        End Function

        Public Overrides Function ToString() As String
            If InStr(name, "[") < InStr(name, "]") AndAlso InStr(name, "[") > 0 Then
                Return name
            Else
                Return $"[{name}]{If(charge = 1, "", charge)}{mode}"
            End If
        End Function

        ''' <summary>
        ''' 返回加和物的m/z数据
        ''' </summary>
        ''' <param name="mass#"></param>
        ''' <param name="adduct#"></param>
        ''' <param name="charge%"></param>
        ''' <returns></returns>
        Public Shared Function AdductMass(mass#, adduct#, charge%) As Double
            Return (mass / sys.Abs(charge) + adduct)
        End Function

        ''' <summary>
        ''' 从质谱的MS/MS的前体的m/z结果反推目标分子的mass结果
        ''' </summary>
        ''' <param name="precursorMZ#"></param>
        ''' <param name="M#"></param>
        ''' <param name="charge%"></param>
        ''' <param name="adduct#"></param>
        ''' <returns></returns>
        Public Shared Function ReverseMass(precursorMZ#, M#, charge%, adduct#) As Double
            Return ((precursorMZ - adduct) * sys.Abs(charge) / M)
        End Function
    End Structure
End Namespace
