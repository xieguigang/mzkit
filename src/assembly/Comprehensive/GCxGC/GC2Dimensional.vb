#Region "Microsoft.VisualBasic::a81570bf398bd234d0977f08418046a6, assembly\Comprehensive\GCxGC\GC2Dimensional.vb"

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

    '   Total Lines: 201
    '    Code Lines: 124 (61.69%)
    ' Comment Lines: 58 (28.86%)
    '    - Xml Docs: 87.93%
    ' 
    '   Blank Lines: 19 (9.45%)
    '     File Size: 7.62 KB


    ' Module GC2Dimensional
    ' 
    '     Function: (+3 Overloads) Demodulate2D, IsLecoGCMS, (+2 Overloads) scan1, ToMzPack
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.GCxGC
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Linq

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
        For Each name As String In New String() {
            "mass_values", "intensity_values",
            "scan_acquisition_time", "total_intensity",
            "point_count"
        }
            If Not cdf.dataVariableExists(name) Then
                Return False
            End If
        Next

        Return True
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
        Dim raw1D As mzPack = GCMSConvertor.ConvertGCMS(agilentGC, println)

        Return raw1D.Demodulate2D(modtime, sam_rate)
    End Function

    <Extension>
    Public Function Demodulate2D(raw1D As mzPack, modtime As Double, Optional sam_rate As Double = Double.NaN) As mzPack
        ' agilentGC.ToString
        '
        ' 2022-01-12
        ' build string based on the internal data vector
        ' will cause the out of memory error
        Dim gc2D As New mzPack With {
            .MS = raw1D.MS.Demodulate2D(modtime, sam_rate),
            .Application = FileApplicationClass.GCxGC,
            .source = $"LECO GCxGC({raw1D.source})",
            .Thumbnail = raw1D.Thumbnail,
            .Scanners = raw1D.Scanners,
            .Annotations = raw1D.Annotations,
            .Chromatogram = raw1D.Chromatogram,
            .metadata = raw1D.metadata
        }

        Return gc2D
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
                            .activationMethod = ActivationMethods.AnyType,
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
End Module
