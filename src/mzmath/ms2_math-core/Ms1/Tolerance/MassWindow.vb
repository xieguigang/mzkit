﻿#Region "Microsoft.VisualBasic::d90ad661d053e029dc71a17d76b45bd6, mzmath\ms2_math-core\Ms1\Tolerance\MassWindow.vb"

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

    '   Total Lines: 164
    '    Code Lines: 104 (63.41%)
    ' Comment Lines: 30 (18.29%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 30 (18.29%)
    '     File Size: 5.19 KB


    '     Interface IMassBin
    ' 
    '         Properties: mass, max, min
    ' 
    '     Interface IMassSet
    ' 
    '         Properties: mass, max, min
    ' 
    '     Module MassExtensions
    ' 
    '         Function: AverageMzDiff, Mass, MassList, MzWindowDescription
    ' 
    '     Class MassWindow
    ' 
    '         Properties: annotation, mass, mzmax, mzmin
    ' 
    '         Constructor: (+5 Overloads) Sub New
    '         Function: GetMzDiff, (+2 Overloads) ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace Ms1

    ''' <summary>
    ''' an abstract mass window model
    ''' </summary>
    Public Interface IMassBin

        Property mass As Double
        Property min As Double
        Property max As Double

    End Interface

    ''' <summary>
    ''' an abstract model of the mass collection
    ''' </summary>
    Public Interface IMassSet

        Property mass As Double()
        Property min As Double()
        Property max As Double()

    End Interface

    <HideModuleName>
    Public Module MassExtensions

        ''' <summary>
        ''' get all mass value from the mass window data
        ''' </summary>
        ''' <param name="masslist"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Mass(masslist As IEnumerable(Of MassWindow)) As Double()
            Return masslist.Select(Function(mzi) mzi.mass).ToArray
        End Function

        <Extension>
        Public Function AverageMzDiff(masslist As IEnumerable(Of MassWindow)) As Double
            Return masslist.Select(Function(mzi) mzi.GetMzDiff).IteratesALL.Average
        End Function

        <Extension>
        Public Iterator Function MassList(massSet As IMassSet) As IEnumerable(Of MassWindow)
            For i As Integer = 0 To massSet.mass.Length - 1
                Yield New MassWindow With {
                    .mass = massSet.mass(i),
                    .mzmin = massSet.min(i),
                    .mzmax = massSet.max(i)
                }
            Next
        End Function

        Public Function MzWindowDescription(mzmax As Double, mzmin As Double, Optional ppm As Double = 30) As String
            If PPMmethod.PPM(mzmin, mzmax) > 30 Then
                Return $"da:{(mzmax - mzmin).ToString("F3")}"
            Else
                Return $"ppm:{PPMmethod.PPM(mzmin, mzmax).ToString("F1")}"
            End If
        End Function

    End Module

    ''' <summary>
    ''' the m/z bin data
    ''' </summary>
    Public Class MassWindow : Implements IMassBin, Value(Of Double).IValueOf

        ''' <summary>
        ''' the real mass value
        ''' </summary>
        ''' <returns></returns>
        <XmlText>
        Public Property mass As Double Implements IMassBin.mass, Value(Of Double).IValueOf.Value

        ''' <summary>
        ''' the left of current mass window
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mzmin As Double Implements IMassBin.min
        ''' <summary>
        ''' the right of current mass window
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mzmax As Double Implements IMassBin.max

        Public Property annotation As String

        Sub New()
        End Sub

        ''' <summary>
        ''' create from a single mass value
        ''' </summary>
        ''' <param name="mass"></param>
        Sub New(mass As Double)
            Me.mass = mass
        End Sub

        Sub New(mass As Double, ppm As Double)
            Me.mass = mass

            With MzWindow(mass, ppm)
                mzmin = .lowerMz
                mzmax = .upperMz
            End With
        End Sub

        Sub New(mass As Double, mzdiff As Tolerance)
            Me.mass = mass

            If mzdiff.Type = MassToleranceType.Da Then
                mzmin = mass - mzdiff.DeltaTolerance
                mzmax = mass + mzdiff.DeltaTolerance
            Else
                With MzWindow(mass, ppm:=mzdiff.DeltaTolerance)
                    mzmin = .lowerMz
                    mzmax = .upperMz
                End With
            End If
        End Sub

        Sub New(massdata As IEnumerable(Of Double), Optional anno As String = Nothing)
            Dim allmass As Double() = massdata.ToArray

            mass = allmass.Average
            mzmin = allmass.Min
            mzmax = allmass.Max
            annotation = If(anno, $"based on {allmass.Length} ions")
        End Sub

        Public Overrides Function ToString() As String
            Dim ppm_desc As String = MzWindowDescription(mzmax, mzmin, ppm:=100)
            Dim str As String = $"{mass.ToString("F4")} [{ppm_desc}]{annotation}"

            Return str
        End Function

        Public Iterator Function GetMzDiff() As IEnumerable(Of Double)
            If mzmin = 0.0 Then
                Yield 0
            Else
                Yield mass - mzmin
            End If
            If mzmax = 0.0 Then
                Yield 0
            Else
                Yield mzmax - mass
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function ToString(mzmax As Double, mzmin As Double, Optional ppm As Double = 30) As String
            Return MzWindowDescription(mzmax, mzmin, ppm)
        End Function

    End Class
End Namespace
