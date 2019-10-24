#Region "Microsoft.VisualBasic::68ba5751df61b84dc4bc87be8683dbbb, ms2_math-core\Chromatogram\ChromatogramTick.vb"

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

    '     Class ChromatogramTick
    ' 
    '         Properties: Intensity, Time
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace Chromatogram

    ''' <summary>
    ''' The chromatogram signal ticks (``{time => intensity}``)
    ''' </summary>
    Public Class ChromatogramTick : Implements ITimePoint

        ''' <summary>
        ''' The signal tick time in second
        ''' </summary>
        ''' <returns></returns>
        Public Property Time As Double Implements ITimePoint.time
        ''' <summary>
        ''' number of detector counts
        ''' </summary>
        ''' <returns></returns>
        Public Property Intensity As Double Implements ITimePoint.intensity

        Sub New()
        End Sub

        Sub New(time#, into#)
            Me.Time = time
            Me.Intensity = into
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Intensity}@{Time}s"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(tick As ChromatogramTick) As PointF
            Return New PointF(tick.Time, tick.Intensity)
        End Operator
    End Class

    Public Interface ITimePoint
        Property time As Double
        Property intensity As Double
    End Interface
End Namespace
