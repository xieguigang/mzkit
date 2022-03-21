#Region "Microsoft.VisualBasic::b9f432d3e0b7e65399833eb6aca543bf, mzkit\src\mzkit\Task\Properties\UVScanProperty.vb"

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

    '   Total Lines: 45
    '    Code Lines: 39
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.57 KB


    ' Class UVScanProperty
    ' 
    '     Properties: peak1, peak2, peak3, scan_time, total_ion_current
    '                 waveMax, waveMin
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Math.SignalProcessing.PeakFinding

Public Class UVScanProperty

    <Category("wavelength(nm)")>
    <DisplayName("min")>
    Public Property waveMin As Double
    <Category("wavelength(nm)")>
    <DisplayName("max")>
    Public Property waveMax As Double
    Public Property total_ion_current As Double
    Public Property scan_time As Double

    <Category("UV Spectrums")> Public Property peak1 As Double
    <Category("UV Spectrums")> Public Property peak2 As Double
    <Category("UV Spectrums")> Public Property peak3 As Double

    Sub New(scan As UVScan)
        waveMin = scan.wavelength.Min
        waveMax = scan.wavelength.Max
        total_ion_current = scan.total_ion_current
        scan_time = scan.scan_time

        Dim signal As GeneralSignal = scan.GetSignalModel
        Dim peaks = New ElevationAlgorithm(5, 0.65) _
            .FindAllSignalPeaks(signal.GetTimeSignals) _
            .OrderByDescending(Function(a)
                                   Return a.integration
                               End Function) _
            .ToArray

        If peaks.Length >= 1 Then
            peak1 = peaks(0).rtmax
        End If
        If peaks.Length >= 2 Then
            peak2 = peaks(1).rtmax
        End If
        If peaks.Length >= 3 Then
            peak3 = peaks(2).rtmax
        End If
    End Sub
End Class
