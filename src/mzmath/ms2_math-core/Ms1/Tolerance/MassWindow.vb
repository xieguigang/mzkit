#Region "Microsoft.VisualBasic::4147844685b5e8869765226a6678376a, G:/mzkit/src/mzmath/ms2_math-core//Ms1/Tolerance/MassWindow.vb"

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

    '   Total Lines: 76
    '    Code Lines: 45
    ' Comment Lines: 15
    '   Blank Lines: 16
    '     File Size: 2.17 KB


    '     Interface IMassBin
    ' 
    '         Properties: mass, max, min
    ' 
    '     Class MassWindow
    ' 
    '         Properties: annotation, mass, mzmax, mzmin
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports std = System.Math

Namespace Ms1

    Public Interface IMassBin

        Property mass As Double
        Property min As Double
        Property max As Double

    End Interface

    ''' <summary>
    ''' the m/z bin data
    ''' </summary>
    Public Class MassWindow : Implements IMassBin

        ''' <summary>
        ''' the real mass value
        ''' </summary>
        ''' <returns></returns>
        <XmlText>
        Public Property mass As Double Implements IMassBin.mass

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

        Public Overrides Function ToString() As String
            Dim ppm As Double = PPMmethod.PPM(mzmin, mzmax)

            If ppm > 100 Then
                Return $"{mass.ToString("F4")} [{std.Abs(mzmax - mzmin).ToString("F3")}da]{annotation}"
            Else
                Return $"{mass.ToString("F4")} [{CInt(ppm)}ppm]{annotation}"
            End If
        End Function

    End Class
End Namespace
