﻿#Region "Microsoft.VisualBasic::4fc0a7bd1ff992a2320591db6bb022ac, mzmath\ms2_math-core\Ms1\PrecursorType\MzCalculator.vb"

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

    '   Total Lines: 187
    '    Code Lines: 97 (51.87%)
    ' Comment Lines: 67 (35.83%)
    '    - Xml Docs: 94.03%
    ' 
    '   Blank Lines: 23 (12.30%)
    '     File Size: 7.16 KB


    '     Class MzCalculator
    ' 
    '         Properties: adducts, charge, IsEmpty, M, mode
    '                     name
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: AdductMZ, CalcMass, CalcMZ, (+2 Overloads) EvaluateAll, GetIonMode
    '                   ReverseMass, (+2 Overloads) ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports std = System.Math

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' m/z calculator for a given ion precursor type
    ''' </summary>
    ''' <remarks>
    ''' this precursor adduct data model could be used for construct 
    ''' the <see cref="PrecursorInfo"/> data.
    ''' </remarks>
    Public Class MzCalculator

        Const ElectronMassInDalton = 0.0005485799

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
        ''' only one of the char +/-
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' equals to the <see cref="IonModes"/>
        ''' </remarks>
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
        ''' <param name="type"></param>
        ''' <param name="charge"></param>
        ''' <param name="M"></param>
        ''' <param name="adducts"></param>
        ''' <param name="mode">只允许``+/-``这两种符号出现</param>
        Sub New(type$, charge%, M#, adducts#, Optional mode As Char = Nothing)
            Me.name = type
            Me.charge = charge
            Me.M = M
            Me.adducts = adducts
            Me.mode = mode
        End Sub

        ''' <summary>
        ''' Evaluate the exact mass from m/z based on current precursor adducts data
        ''' </summary>
        ''' <param name="mz#"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CalcMass(mz#) As Double
            Return (ReverseMass(mz, M, charge, adducts, ionMode:=If(mode = "+"c, IonModes.Positive, IonModes.Negative)))
        End Function

        ''' <summary>
        ''' 通过所给定的精确分子质量计算出``m/z``
        ''' </summary>
        ''' <param name="mass#"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CalcMZ(mass#) As Double
            Return AdductMZ(mass, adducts, charge, IonMode:=If(mode = "+"c, IonModes.Positive, IonModes.Negative), M:=M)
        End Function

        Public Function GetIonMode() As IonModes
            Return Provider.ParseIonMode(mode)
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
        ''' <param name="exactMass">the exact mass of the metabolite</param>
        ''' <param name="AdductIonAccurateMass">the exact mass of the adduct ion</param>
        ''' <param name="chargeNumber">the charge value of the adduct ion</param>
        ''' <param name="M">
        ''' Adduct Ion Xmer
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function AdductMZ(exactMass#, AdductIonAccurateMass#, chargeNumber%,
                                        IonMode As IonModes,
                                        Optional M As Integer = 1) As Double

            Dim precursorMz = std.abs((exactMass * M + AdductIonAccurateMass) / chargeNumber)

            If IonMode = IonModes.Positive Then
                precursorMz -= ElectronMassInDalton * chargeNumber
            Else
                precursorMz += ElectronMassInDalton * chargeNumber
            End If

            Return precursorMz
        End Function

        ''' <summary>
        ''' 从质谱的MS/MS的前体的m/z结果反推目标分子的mass结果
        ''' </summary>
        ''' <param name="mz#"></param>
        ''' <param name="M#"></param>
        ''' <param name="chargeNumber%"></param>
        ''' <param name="adductIonAccurateMass#"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ReverseMass(mz#, M#, chargeNumber%, adductIonAccurateMass#, ionMode As IonModes) As Double
            Dim monoIsotopicMass = (mz * chargeNumber - adductIonAccurateMass) / M

            If ionMode = IonModes.Positive Then
                monoIsotopicMass += ElectronMassInDalton * chargeNumber
            Else
                monoIsotopicMass -= ElectronMassInDalton * chargeNumber
            End If

            Return monoIsotopicMass
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mass">
        ''' mass value or ``m/z``
        ''' </param>
        ''' <param name="mode"><see cref="ParseIonMode"/></param>
        ''' <returns></returns>
        Public Shared Function EvaluateAll(mass#, mode As String, Optional exact_mass As Boolean = False) As IEnumerable(Of PrecursorInfo)
            Return EvaluateAll(mass, Provider.GetCalculator(mode).Values, exact_mass)
        End Function

        Public Shared Iterator Function EvaluateAll(mass As Double,
                                                    ions As IEnumerable(Of MzCalculator),
                                                    Optional exact_mass As Boolean = False) As IEnumerable(Of PrecursorInfo)
            For Each type As MzCalculator In ions
                Yield New PrecursorInfo With {
                    .adduct = type.adducts,
                    .charge = type.charge,
                    .M = type.M,
                    .mz = If(exact_mass, type.CalcMass(mass), type.CalcMZ(mass)),
                    .precursor_type = type.name,
                    .ionMode = type.GetIonMode
                }
            Next
        End Function
    End Class

End Namespace
