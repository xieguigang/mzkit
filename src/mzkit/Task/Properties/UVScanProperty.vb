Imports System.ComponentModel
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

        Dim signal = scan.GetSignalModel
        Dim peaks = New ElevationAlgorithm(5, 0.65).FindAllSignalPeaks(signal.GetTimeSignals).OrderByDescending(Function(a) a.integration).ToArray

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
