#Region "Microsoft.VisualBasic::4fb492bd01d14dca45b52821878e588f, src\mzmath\ms2_math-core\Ms1\PrecursorType\MzCalculator.vb"

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

    '     Class MzCalculator
    ' 
    '         Properties: adducts, charge, IsEmpty, M, mode
    '                     name
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: AdductMZ, CalcMass, CalcMZ, EvaluateAll, ReverseMass
    '                   (+2 Overloads) ToString
    ' 
    '     Class PrecursorInfo
    ' 
    '         Properties: adduct, charge, ionMode, M, mz
    '                     precursor_type
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports System.Data.Linq.Mapping
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
#End If

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace Ms1.PrecursorType

    Public Class MzCalculator

        Public Property name As String
        ''' <summary>
        ''' 电荷量的符号无所谓,计算出来的m/z结果值总是正数
        ''' </summary>
        ''' <returns></returns>
        Public Property charge As Integer
        Public Property M As Integer

        ''' <summary>
        ''' 是可能会出现负数的加和结果，例如[M-H2O]的adducts为-18
        ''' </summary>
        Public Property adducts As Double
        ''' <summary>
        ''' +/-
        ''' </summary>
        ''' <returns></returns>
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

        Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="type$"></param>
        ''' <param name="charge%"></param>
        ''' <param name="M#"></param>
        ''' <param name="adducts#"></param>
        ''' <param name="mode">只允许``+/-``这两种符号出现</param>
        Sub New(type$, charge%, M#, adducts#, Optional mode As Char = Nothing)
            Me.name = type
            Me.charge = charge
            Me.M = M
            Me.adducts = adducts
            Me.mode = mode
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

        <DebuggerStepThrough>
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
            Return (mass / stdNum.Abs(charge) + adduct)
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
            Return ((mz - adduct) * stdNum.Abs(charge) / M)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mass">
        ''' mass value or ``m/z``
        ''' </param>
        ''' <param name="mode"><see cref="ParseIonMode"/></param>
        ''' <returns></returns>
        Public Shared Iterator Function EvaluateAll(mass#, mode As String, Optional exact_mass As Boolean = False) As IEnumerable(Of PrecursorInfo)
            For Each type In Provider.Calculator(mode).Values
                Yield New PrecursorInfo With {
                    .adduct = type.adducts,
                    .charge = type.charge,
                    .M = type.M,
                    .mz = If(exact_mass, type.CalcMass(mass), type.CalcMZ(mass)),
                    .precursor_type = type.name
                }
            Next
        End Function
    End Class

    Public Class PrecursorInfo

        <XmlAttribute>
        Public Property precursor_type As String
        Public Property charge As Double
        Public Property M As Double
        Public Property adduct As Double

        ''' <summary>
        ''' mz or exact mass
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="m/z")>
        Public Property mz As String
        Public Property ionMode As Integer

        Sub New()
        End Sub

        Sub New(mz As MzCalculator)
            precursor_type = mz.ToString
            charge = mz.charge
            M = mz.M
            adduct = mz.adducts
            ionMode = ParseIonMode(mz.mode)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{precursor_type} m/z={mz}"
        End Function
    End Class
End Namespace
