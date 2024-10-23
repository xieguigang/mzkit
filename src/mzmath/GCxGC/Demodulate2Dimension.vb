Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports std = System.Math

Public Module Demodulate2Dimension

    ''' <summary>
    ''' Function To calculate 2D RT from the 1D RT
    ''' </summary>
    ''' <param name="rt"></param>
    ''' <param name="mod_time"></param>
    ''' <param name="delay_time"></param>
    ''' <returns></returns>
    Public Function Convert2dRT(rt As Double, mod_time As Double, Optional delay_time As Double = 0) As Double
        Dim rt_adj = rt - delay_time
        Dim rt_2d = rt_adj - (mod_time * std.Floor(rt_adj / mod_time))

        Return rt_2d
    End Function

    ''' <summary>
    ''' converts 1D signal into 2D with xp x yp dimensions
    ''' </summary>
    ''' <param name="sig">
    ''' data should be re-order by scan time
    ''' </param>
    ''' <param name="modtime">
    ''' the modulation time of the chromatographic run. 
    ''' modulation period in time unit 'seconds'.
    ''' </param>
    ''' <param name="sampleRate">
    ''' the sampling rate of the equipment.
    ''' If sam_rate is missing, the sampling rate is calculated by the dividing 1 by
    ''' the difference of two adjacent scan time.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function Demodulate2DShape(Of Tick As ITimeSignal)(sig As Tick(), modtime As Double, Optional sampleRate As Double = Double.NaN) As Size
        Dim numpoints As Integer = sig.Length
        Dim runtime As Double = Aggregate t As Tick
                                In sig
                                Let time As Double = t.time
                                Into Max(time)
        Dim rate As Double = If(sampleRate.IsNaNImaginary OrElse sampleRate <= 0, 1 / sig.tickInternal, sampleRate)

        Call VBDebugger.EchoLine($"Found {numpoints} data points")
        Call VBDebugger.EchoLine($"Runtime is {(runtime / 60).ToString("F2")} minutes")
        Call VBDebugger.EchoLine($"GCxGC modulation period is {modtime} second")
        Call VBDebugger.EchoLine($"Acquisition rate is {rate} Hz")

        ' Pad matrix so it can be resized
        Dim xp = CInt(modtime * rate)
        Dim yp = CInt(std.Ceiling(numpoints / xp))

        Call VBDebugger.EchoLine($"Dimension1: {xp}")
        Call VBDebugger.EchoLine($"Dimension2: {yp}")
        Call VBDebugger.EchoLine("")

        Dim delta As Integer = std.Abs(xp * yp - numpoints)

        If delta <> 0 Then
            Dim msg As String = $"the last {delta} signals points will be omitted."

            Call msg.Warning
            Call VBDebugger.EchoLine(msg)
        End If

        Return New Size(xp, yp)
    End Function

    <Extension>
    Private Function tickInternal(Of Tick As ITimeSignal)(sig As Tick()) As Double
        Dim diff As New List(Of Double)

        sig = (From t As Tick In sig Order By t.time).ToArray

        For i As Integer = 1 To sig.Length - 1
            Call diff.Add(sig(i).time - sig(i - 1).time)
        Next

        Return diff.Average
    End Function
End Module
