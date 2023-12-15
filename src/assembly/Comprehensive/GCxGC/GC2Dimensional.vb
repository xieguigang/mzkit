#Region "Microsoft.VisualBasic::63d3aa95872be663e82b0c9015e3636c, mzkit\src\assembly\Comprehensive\GCxGC\GC2Dimensional.vb"

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

'   Total Lines: 249
'    Code Lines: 142
' Comment Lines: 77
'   Blank Lines: 30
'     File Size: 9.60 KB


' Module GC2Dimensional
' 
'     Function: Convert2dRT, (+2 Overloads) Demodulate2D, Demodulate2DShape, (+2 Overloads) scan1, tickInternal
'               ToMzPack
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

''' <summary>
''' GCxGC assembly data
''' </summary>
Public Module GC2Dimensional

    ''' <summary>
    ''' test if the given cdf file is in gc-ms file format
    ''' </summary>
    ''' <param name="cdf"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IsLecoGCMS(cdf As netCDFReader) As Boolean

    End Function

    ''' <summary>
    ''' Function To calculate 2D RT from the 1D RT
    ''' </summary>
    ''' <param name="rt"></param>
    ''' <param name="Modtime"></param>
    ''' <param name="delay_time"></param>
    ''' <returns></returns>
    Public Function Convert2dRT(rt As Double, Modtime As Double, Optional delay_time As Double = 0) As Double
        Dim rt_adj = rt - delay_time
        Dim rt_2d = rt_adj - (Modtime * stdNum.Floor(rt_adj / Modtime))

        Return rt_2d
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="agilentGC"></param>
    ''' <param name="modtime">
    ''' the modulation time of the chromatographic run. 
    ''' modulation period in time unit 'seconds'.
    ''' </param>
    ''' <param name="sam_rate">
    ''' the sampling rate of the equipment.
    ''' If sam_rate is missing, the sampling rate is calculated by the dividing 1 by
    ''' the difference of two adjacent scan time.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function ToMzPack(agilentGC As netCDFReader,
                             modtime As Double,
                             Optional sam_rate As Double = Double.NaN) As mzPack

        Dim println As Action(Of String) = AddressOf Console.WriteLine
        Dim sig As ScanMS1() = GCMSConvertor.LoadMs1Scans(agilentGC, println).ToArray

        ' agilentGC.ToString
        '
        ' 2022-01-12
        ' build string based on the internal data vector
        ' will cause the out of memory error

        Return New mzPack With {
            .MS = sig.Demodulate2D(modtime, sam_rate),
            .Application = FileApplicationClass.GCxGC,
            .source = "LECO GCxGC CDF"
        }
    End Function

    ''' <summary>
    ''' converts 1D signal into 2D with xp x yp dimensions
    ''' </summary>
    ''' <param name="sig"></param>
    ''' <param name="modtime">
    ''' the modulation time of the chromatographic run. 
    ''' modulation period in time unit 'seconds'.
    ''' </param>
    ''' <param name="sam_rate">
    ''' the sampling rate of the equipment.
    ''' If sam_rate is missing, the sampling rate is calculated by the dividing 1 by
    ''' the difference of two adjacent scan time.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function Demodulate2D(sig As ScanMS1(),
                                 modtime As Double,
                                 Optional sam_rate As Double = Double.NaN) As ScanMS1()

        Dim size As Size = sig.Demodulate2DShape(modtime, sampleRate:=sam_rate)
        Dim matrix As ScanMS1() = sig.Split(size.Width) _
            .Select(Function(t)
                        Return t.scan1
                    End Function) _
            .ToArray
        Dim t1d As Double = matrix.Select(Function(t) t.rt).Max / 60
        Dim t2d As Double = Aggregate scan1D As ScanMS1
                            In matrix
                            Let t2 As Double = scan1D.products.Max(Function(t) t.rt)
                            Into Average(t2)

        Call Console.WriteLine($"get max runtime: {t1d.ToString("F2")} min.")
        Call Console.WriteLine($"2d modtime ({t2d.ToString("F2")}s) should be approximately equals to {modtime}s.")

        Return matrix
    End Function

    ''' <summary>
    ''' converts 1D signal into 2D with xp x yp dimensions
    ''' </summary>
    ''' <param name="sig"></param>
    ''' <param name="modtime">modulation period in seconds</param>
    ''' <param name="sampleRate">
    ''' the sampling rate of the equipment.
    ''' If sam_rate is missing, the sampling rate is calculated by the dividing 1 by
    ''' the difference of two adjacent scan time.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function Demodulate2D(sig As ChromatogramTick(),
                                 modtime As Double,
                                 Optional sampleRate As Double = Double.NaN) As D2Chromatogram()

        Dim size As Size = sig.Demodulate2DShape(modtime, sampleRate)
        Dim matrix = sig.Split(size.Width) _
            .Select(Function(t)
                        Return t.scan1
                    End Function) _
            .ToArray

        Return matrix
    End Function

    ''' <summary>
    ''' merge data
    ''' </summary>
    ''' <param name="rt1"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Private Function scan1(rt1 As ScanMS1()) As ScanMS1
        Dim t0 As Double = rt1(Scan0).rt
        Dim allMs = rt1.Select(Function(d) d.GetMs).IteratesALL.ToArray
        Dim d2 As ScanMS2() = rt1 _
            .Select(Function(t)
                        Return New ScanMS2 With {
                            .activationMethod = mzData.ActivationMethods.AnyType,
                            .centroided = False,
                            .charge = 0,
                            .collisionEnergy = 0,
                            .into = t.into,
                            .mz = t.mz,
                            .parentMz = 0,
                            .polarity = 0,
                            .intensity = 0,
                            .rt = t.rt - t0,
                            .scan_id = $"[MS1] 2D.scan_time={ .rt.ToString("F2")}, (BPC: { .into.Max.ToString("G3")}, TIC: { .into.Sum.ToString("G3")})"
                        }
                    End Function) _
            .ToArray

        Return New ScanMS1 With {
           .BPC = allMs.Select(Function(d) d.intensity).Max,
           .TIC = allMs.Select(Function(d) d.intensity).Sum,
           .scan_id = rt1(Scan0).scan_id,
           .rt = t0,
           .mz = rt1(Scan0).mz,
           .into = rt1(Scan0).into,
           .products = d2
        }
    End Function

    <Extension>
    Private Function scan1(t As ChromatogramTick()) As D2Chromatogram
        Dim t0 As Double = t(0).Time

        Return New D2Chromatogram With {
            .scan_time = t0,
            .intensity = t.Sum(Function(i) i.Intensity),
            .chromatogram = t _
                .Select(Function(i)
                            Return New ChromatogramTick With {
                                .Time = i.Time - t0,
                                .Intensity = i.Intensity
                            }
                        End Function) _
                .ToArray
        }
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
    Public Function Demodulate2DShape(Of Tick As ITimeSignal)(sig As Tick(),
                                                              modtime As Double,
                                                              Optional sampleRate As Double = Double.NaN) As Size
        Dim numpoints As Integer = sig.Length
        Dim runtime As Double = Aggregate t As Tick
                                In sig
                                Let time As Double = t.time
                                Into Max(time)
        Dim rate As Double = If(sampleRate.IsNaNImaginary OrElse sampleRate <= 0, 1 / sig.tickInternal, sampleRate)

        Call Console.WriteLine($"Found {numpoints} data points")
        Call Console.WriteLine($"Runtime is {(runtime / 60).ToString("F2")} minutes")
        Call Console.WriteLine($"GCxGC modulation period is {modtime} second")
        Call Console.WriteLine($"Acquisition rate is {rate} Hz")

        ' Pad matrix so it can be resized
        Dim xp = CInt(modtime * rate)
        Dim yp = CInt(stdNum.Ceiling(numpoints / xp))

        Call Console.WriteLine($"Dimension1: {xp}")
        Call Console.WriteLine($"Dimension2: {yp}")
        Call Console.WriteLine()

        Dim delta As Integer = stdNum.Abs(xp * yp - numpoints)

        If delta <> 0 Then
            Dim msg As String = $"the last {delta} signals points will be omitted."

            Call msg.Warning
            Call Console.WriteLine(msg)
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
