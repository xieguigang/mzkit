#Region "Microsoft.VisualBasic::2a5f0f78d36826088aa2d122aa9c996e, E:/mzkit/src/assembly/NMRFidTool//Math/phasing/DSPPhaseCorrection.vb"

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

    '   Total Lines: 186
    '    Code Lines: 161
    ' Comment Lines: 15
    '   Blank Lines: 10
    '     File Size: 6.85 KB


    '     Class DSPPhaseCorrection
    ' 
    '         Function: dspPhaseCorrection
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace fidMath.Phasing

    ''' <summary>
    ''' Phase correction due to the DSP filter
    ''' 
    ''' @author Luis F. de Figueiredo
    ''' 
    ''' </summary>
    Public Class DSPPhaseCorrection


        '    public DSPPhaseCorrection() {
        '
        '    }

        ''' <summary>
        ''' Method to calculate the initial phase correction due to the digital filter. This method was based on the
        ''' information available in the dsp_phase of SpinWorks from Prof. Kirk Marat
        ''' </summary>
        Public Overridable Function dspPhaseCorrection(spectrum As Spectrum) As Spectrum
            Dim phase As Double = 0
            If spectrum.Acqu.DspFirmware = 10 Then
                Select Case spectrum.Acqu.DspDecimation
                    Case 2
                        phase = 16110.0
                    Case 3
                        phase = 12060.0
                    Case 4
                        phase = 23985.0
                    Case 6
                        phase = 21270.0
                    Case 8
                        phase = 24682.5
                    Case 12
                        phase = 21735.0
                    Case 16
                        phase = 25031.25
                    Case 24
                        phase = 21967.5
                    Case 32
                        phase = 25205.62
                    Case 48
                        phase = 22083.75
                    Case 64
                        phase = 25292.8
                    Case 96
                        phase = 22141.87
                    Case 128
                        phase = 25336.4
                    Case 192
                        phase = 22170.93
                    Case 256
                        phase = 25358.2
                    Case 384
                        phase = 22185.46
                    Case 512
                        phase = 25369.1
                    Case 768
                        phase = 22192.73
                    Case 1024
                        phase = 25374.55
                    Case 1536
                        phase = 22196.36
                    Case 2048
                        phase = 25377.27
                    Case Else
                        phase = 0

                End Select
            ElseIf spectrum.Acqu.DspFirmware = 11 Then
                Select Case spectrum.Acqu.DspDecimation
                    Case 2
                        phase = 16560.0
                    Case 3
                        phase = 13140.0
                    Case 4
                        phase = 17280.0
                    Case 6
                        phase = 18060.0
                    Case 8
                        phase = 19170.0
                    Case 12
                        phase = 25020.0
                    Case 16
                        phase = 26010.0
                    Case 24
                        phase = 25260.0
                    Case 32
                        phase = 26190.0
                    Case 48
                        phase = 25380.0
                    Case 64
                        phase = 26280.0
                    Case 96
                        phase = 25440.0
                    Case 128
                        phase = 26100.0
                    Case 192
                        phase = 25680.0
                    Case 256
                        phase = 26010.0
                    Case 384
                        phase = 25800.0
                    Case 512
                        phase = 25965.0
                    Case 768
                        phase = 25860.0
                    Case 1024
                        phase = 25942.5
                    Case 1536
                        phase = 25890.0
                    Case 2048
                        phase = 25931.25
                    Case Else
                        phase = 0

                End Select
            ElseIf spectrum.Acqu.DspFirmware = 12 Then
                Select Case spectrum.Acqu.DspDecimation
                    Case 2
                        phase = 16560.0
                    Case 3
                        phase = 13140.0
                    Case 4
                        phase = 17280.0
                    Case 6
                        phase = 18060.0
                    Case 8
                        phase = 19170.0
                    Case 12
                        phase = 25020.0
                    Case 16
                        phase = 25785.0
                    Case 24
                        phase = 25260.0
                    Case 32
                        phase = 25965.0
                    Case 48
                        phase = 25380.0
                    Case 64
                        phase = 26055.0
                    Case 96
                        phase = 25440.0
                    Case 128
                        phase = 26100.0
                    Case 192
                        phase = 25680.0
                    Case 256
                        phase = 26010.0
                    Case 384
                        phase = 25800.0
                    Case 512
                        phase = 25965.0
                    Case 768
                        phase = 25860.0
                    Case 1024
                        phase = 25942.5
                    Case 1536
                        phase = 25890.0
                    Case 2048
                        phase = 25931.25
                    Case Else
                        phase = 0

                End Select
            ElseIf spectrum.Acqu.DspFirmware >= 20 AndAlso spectrum.Acqu.DspFirmware <= 23 OrElse spectrum.Acqu.getAcquisitionMode Is (Acqu.AcquisitionMode.CUSTOM_DISP) Then
                phase = spectrum.Acqu.DspGroupDelay * 360
            Else
                phase = 0
                Console.Error.WriteLine("[WARNING] It is not possible to identify the DSP used;")
            End If
            Dim fid = New Double(spectrum.Fid.Length - 1) {}

            For i As Integer = 0 To spectrum.Fid.Length / 2 - 1
                Dim phaseAngle = 2 * Math.PI / 360 * i / (spectrum.Fid.Length / 2) * phase
                ' real channel are even positions
                fid(i * 2) = spectrum.Fid(i * 2) * Math.Cos(phaseAngle) - spectrum.Fid(i * 2 + 1) * Math.Sin(phaseAngle)
                ' imaginary channel are off positions
                fid(i * 2 + 1) = spectrum.Fid(i * 2) * Math.Sin(phaseAngle) + spectrum.Fid(i * 2 + 1) * Math.Cos(phaseAngle)

            Next

            Return New Spectrum(fid, spectrum.Acqu, spectrum.Proc)
        End Function
    End Class
End Namespace
