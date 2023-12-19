#Region "Microsoft.VisualBasic::56d523fbae356833ab64a665d8201b0b, mzkit\src\mzmath\ms2_math-core\Chromatogram\ChromatogramTick.vb"

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

'   Total Lines: 70
'    Code Lines: 44
' Comment Lines: 11
'   Blank Lines: 15
'     File Size: 2.05 KB


'     Class ChromatogramTick
' 
'         Properties: Intensity, Time
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: ToString
' 
'     Class Chromatogram
' 
'         Properties: Chromatogram, Name, rtmax, rtmin
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.TagData

Namespace Chromatogram

    ''' <summary>
    ''' The chromatogram signal ticks (``{time => intensity}``)
    ''' </summary>
    Public Class ChromatogramTick : Implements ITimeSignal

        ''' <summary>
        ''' The signal tick time in second
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Time As Double Implements ITimeSignal.time

        ''' <summary>
        ''' number of detector counts
        ''' </summary>
        ''' <returns></returns>
        <XmlText>
        Public Property Intensity As Double Implements ITimeSignal.intensity

        Sub New()
        End Sub

        Sub New(time#, into#)
            Me.Time = time
            Me.Intensity = into
        End Sub

        ''' <summary>
        ''' make signal tick data copy
        ''' </summary>
        ''' <param name="tick"></param>
        Sub New(tick As ITimeSignal)
            Me.Time = tick.time
            Me.Intensity = tick.intensity
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Intensity.ToString("G4")}@{Time.ToString("F2")}s"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(tick As ChromatogramTick) As PointF
            Return New PointF(tick.Time, tick.Intensity)
        End Operator

        Public Shared Iterator Function Zip(rt As Double(), intensity As Double()) As IEnumerable(Of ChromatogramTick)
            For i As Integer = 0 To rt.Length - 1
                Yield New ChromatogramTick(rt(i), If(intensity(i) < 0, 0, intensity(i)))
            Next
        End Function
    End Class

End Namespace
