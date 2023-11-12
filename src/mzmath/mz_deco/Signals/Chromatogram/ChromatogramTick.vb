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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Linq

Namespace Chromatogram

    ''' <summary>
    ''' The chromatogram signal ticks (``{time => intensity}``)
    ''' </summary>
    Public Class ChromatogramTick : Implements ITimeSignal

        ''' <summary>
        ''' The signal tick time in second
        ''' </summary>
        ''' <returns></returns>
        Public Property Time As Double Implements ITimeSignal.time
        ''' <summary>
        ''' number of detector counts
        ''' </summary>
        ''' <returns></returns>
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
    End Class

    ''' <summary>
    ''' A label tagged chromatogram data
    ''' </summary>
    Public Class ChromatogramSerial

        ''' <summary>
        ''' the data label
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        Public Property Chromatogram As ChromatogramTick()

        ''' <summary>
        ''' get the tick count in current <see cref="Chromatogram"/> signal data.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                Return Chromatogram.TryCount
            End Get
        End Property

        ''' <summary>
        ''' get the min rt of current <see cref="Chromatogram"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property rtmin As Double
            Get
                Return Aggregate t As ChromatogramTick
                       In Chromatogram
                       Into Min(t.Time)
            End Get
        End Property

        ''' <summary>
        ''' get the max rt of current <see cref="Chromatogram"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property rtmax As Double
            Get
                Return Aggregate t As ChromatogramTick
                       In Chromatogram
                       Into Max(t.Time)
            End Get
        End Property

        Default Public ReadOnly Property GetTick(i As Integer) As ChromatogramTick
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Chromatogram(i)
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(name As String)
            Me.Name = name
        End Sub

        Sub New(name As String, ticks As IEnumerable(Of ChromatogramTick))
            Me.Name = name
            Me.Chromatogram = ticks.SafeQuery.ToArray
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetTime() As IEnumerable(Of Double)
            Return From t As ChromatogramTick In Chromatogram Select t.Time
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetIntensity() As IEnumerable(Of Double)
            Return From t As ChromatogramTick In Chromatogram Select t.Intensity
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetTuple() As NamedCollection(Of ChromatogramTick)
            Return New NamedCollection(Of ChromatogramTick)(Name, Chromatogram)
        End Function

        Public Overrides Function ToString() As String
            Return Name & $" [{rtmin.ToString("F4")} ~ {rtmax.ToString("F4")} sec]"
        End Function

    End Class
End Namespace
