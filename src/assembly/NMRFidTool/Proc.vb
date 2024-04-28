#Region "Microsoft.VisualBasic::bca6d5413637dd8cfeaced06463e0444, G:/mzkit/src/assembly/NMRFidTool//Proc.vb"

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

    '   Total Lines: 330
    '    Code Lines: 241
    ' Comment Lines: 38
    '   Blank Lines: 51
    '     File Size: 11.36 KB


    ' Class Proc
    ' 
    ' 
    '     Enum WindowFunctions
    ' 
    '         EXPONENTIAL, GAUSSIAN, LORENTZGAUS, SINE, SINESQUARED
    '         TRAF, TRAFS
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: ByteOrder, DspPhase, DwellTime, F1DetectionMode, FirstOrderPhase
    '                 GbFactor, Increment, LeftShift, LineBroadening, PhasingType
    '                 Shift, Ssb, SsbSine, SsbSineSquared, TdEffective
    '                 TransformSize, WindowFunction, WindowFunctionType, ZeroFrequency, ZeroOrderPhase
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: is32Bit
    ' 
    '     Sub: set32Bit
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.NMRFidTool.Acqu
Imports Microsoft.VisualBasic.Data.IO

' 
'  Copyright (c) 2013. EMBL, European Bioinformatics Institute
' 
'  This program is free software: you can redistribute it and/or modify
'  it under the terms of the GNU Lesser General Public License as published by
'  the Free Software Foundation, either version 3 of the License, or
'  (at your option) any later version.
' 
'  This program is distributed in the hope that it will be useful,
'  but WITHOUT ANY WARRANTY; without even the implied warranty of
'  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'  GNU Lesser General Public License for more details.
' 
'  You should have received a copy of the GNU Lesser General Public License
'  along with this program.  If not, see <http://www.gnu.org/licenses/>.
' 

''' <summary>
''' Data structure for the spectra processing parameters.
''' 
''' @author  Luis F. de Figueiredo
''' 
''' User: ldpf
''' Date: 14/01/2013
''' Time: 14:02
''' 
''' </summary>
Public Class Proc

    Private windowFunctionTypeField As Integer 'wdw               window function type
    Private phasingTypeField As Integer 'ph_mod            phasing type
    Private f1DetectionModeField As Integer 'mc2               F1 detection mode
    Private zeroFrequencyField As Double 'sf                frequency of 0 ppm
    Private lineBroadeningField As Double 'lb                line broadening (in Hz?)
    Private gbFactorField As Double 'gb                GB-factor
    Private zeroOrderPhaseField As Double 'phc0              zero order phase
    Private firstOrderPhaseField As Double 'phc1              first order phase
    Private ssbField As Double 'ssb               sine bell shift
    Private ssbSineField As Double 'ssbSine           sine bell shift
    Private ssbSineSquaredField As Double 'ssbSineSquared    sine bell shift
    Private byteOrderField As ByteOrder 'bytordp           byte order (0 -> Little endian, 1 -> Big Endian)
    Private integerType As Boolean 'dtypp             data type (0 -> 32 bit int, 1 -> 64 bit double)

    ' obtained after reading the FID but could it be obtained from the Bruker?
    Private transformSizeField As Integer 'si                transform size (complex)    bruker_read::get_fid
    Private dwellTimeField As Double 'dw                dwell time (in s)           bruker_read::get_fid
    Private hertzPerPoint As Double 'hzperpt                                       bruker_read::get_fid
    Private ppmPerPoint As Double 'ppmperpt                                      bruker_read::get_fid
    Private spectraWidthHertz As Double 'sw_h                                          bruker_read::get_fid

    Private offset As Double 'offset                                        bruker_read::get_fid
    'TODO consider moving this to the Fourier Transformed class or processing class....
    ' variables required later...
    Private tdEffectiveField As Integer 'td_eff        apodization::transform::do_fft
    Private leftShiftField As Integer = 0 'leftshift     ft_settings_dialog::ft_settings_dialog
    Private shiftField As Integer 'j             apodization::transform::do_fft
    Private incrementField As Integer 'i             apodization::transform::do_fft
    Private dspPhaseField As Double
    Private windowFunctionField As WindowFunctions

    Public Enum WindowFunctions
        EXPONENTIAL
        GAUSSIAN
        LORENTZGAUS
        SINE
        SINESQUARED
        TRAF
        TRAFS
    End Enum

    Public Sub New(acquisition As Acqu)

        ' set the size for the fourier Transform
        ' perhaps I should check first if I can use the data from the Proc file..??
        If acquisition.AquiredPoints < 1 * 1024 Then
            transformSizeField = 1024
        ElseIf acquisition.AquiredPoints <= 2 * 1024 Then
            transformSizeField = 2 * 1024
        ElseIf acquisition.AquiredPoints <= 4 * 1024 Then
            transformSizeField = 4 * 1024
        ElseIf acquisition.AquiredPoints <= 8 * 1024 Then
            transformSizeField = 8 * 1024
        ElseIf acquisition.AquiredPoints <= 16 * 1024 Then
            transformSizeField = 16 * 1024
        ElseIf acquisition.AquiredPoints <= 32 * 1024 Then
            transformSizeField = 32 * 1024
        ElseIf acquisition.AquiredPoints <= 64 * 1024 Then
            transformSizeField = 64 * 1024
        ElseIf acquisition.AquiredPoints <= 128 * 1024 Then
            transformSizeField = 128 * 1024
        ElseIf acquisition.AquiredPoints <= 256 * 1024 Then
            transformSizeField = 256 * 1024
        Else
            transformSizeField = 512 * 1024
        End If
        'set the dwell time (in s) to display the timeline of the fid (dw is distance between points)
        If acquisition.SpectralWidth = 0 Or acquisition.TransmiterFreq = 0 Then
            Console.WriteLine("Some acquisition parameters are null")
        End If
        dwellTimeField = 1.0 / (2 * acquisition.SpectralWidth * acquisition.TransmiterFreq)
        hertzPerPoint = acquisition.SpectralWidth * acquisition.TransmiterFreq / transformSizeField
        ppmPerPoint = acquisition.SpectralWidth / transformSizeField
        spectraWidthHertz = acquisition.SpectralWidth * acquisition.TransmiterFreq
        offset = (acquisition.TransmiterFreq - zeroFrequencyField) * 1000000.0 + acquisition.SpectralWidth * acquisition.TransmiterFreq / 2.0

        ' set the position where the shift starts???
        Dim mode = acquisition.getAcquisitionMode()

        If mode Is AcquisitionMode.DISP OrElse mode Is AcquisitionMode.SIMULTANIOUS Then
            shiftField = 2 * leftShiftField
        ElseIf mode Is AcquisitionMode.SEQUENTIAL Then
            shiftField = leftShiftField
        Else
        End If

        'set the number of acquired points we are going to work with
        tdEffectiveField = If(acquisition.AquiredPoints <= 2 * transformSizeField, acquisition.AquiredPoints, 2 * transformSizeField) ' fid data is truncated in the nonsense case -  normal case

    End Sub

    Public Overridable Property TransformSize As Integer
        Get
            Return transformSizeField
        End Get
        Set(value As Integer)
            transformSizeField = value
        End Set
    End Property


    Public Overridable Property WindowFunctionType As Integer
        Get
            Return windowFunctionTypeField
        End Get
        Set(value As Integer)
            windowFunctionTypeField = value
        End Set
    End Property


    Public Overridable Property PhasingType As Integer
        Get
            Return phasingTypeField
        End Get
        Set(value As Integer)
            phasingTypeField = value
        End Set
    End Property


    Public Overridable Property F1DetectionMode As Integer
        Get
            Return f1DetectionModeField
        End Get
        Set(value As Integer)
            f1DetectionModeField = value
        End Set
    End Property


    Public Overridable Property ZeroFrequency As Double
        Get
            Return zeroFrequencyField
        End Get
        Set(value As Double)
            zeroFrequencyField = value
        End Set
    End Property


    Public Overridable Property LineBroadening As Double
        Get
            Return lineBroadeningField
        End Get
        Set(value As Double)
            lineBroadeningField = value
        End Set
    End Property


    Public Overridable Property GbFactor As Double
        Get
            Return gbFactorField
        End Get
        Set(value As Double)
            gbFactorField = value
        End Set
    End Property


    Public Overridable Property ZeroOrderPhase As Double
        Get
            Return zeroOrderPhaseField
        End Get
        Set(value As Double)
            zeroOrderPhaseField = value
        End Set
    End Property


    Public Overridable Property FirstOrderPhase As Double
        Get
            Return firstOrderPhaseField
        End Get
        Set(value As Double)
            firstOrderPhaseField = value
        End Set
    End Property


    Public Overridable Property Ssb As Double
        Get
            Return ssbField
        End Get
        Set(value As Double)
            ssbField = value
            ' if value is given in degrees this converts it to the inverse of coefficient in front to the Pi
            ' 360 = 2 Pi => angle/180 = coefficient (e.g. 360/180 = 2 Pi)
            ' This variables are used in the apodizationTool method sine and Co.
            If value >= 1 Then 'convert Bruker convention to degrees
                value = 180.0 / value
            Else
                value = 0.0
            End If
            ' I do not get this... (see source code)
            ssbSineField = value
            ssbSineSquaredField = value
        End Set
    End Property


    Public Overridable ReadOnly Property SsbSine As Double
        Get
            Return ssbSineField
        End Get
    End Property

    Public Overridable ReadOnly Property SsbSineSquared As Double
        Get
            Return ssbSineSquaredField
        End Get
    End Property

    Public Overridable Property TdEffective As Integer
        Get
            Return tdEffectiveField
        End Get
        Set(value As Integer)
            tdEffectiveField = value
        End Set
    End Property


    Public Overridable Property LeftShift As Integer
        Get
            Return leftShiftField
        End Get
        Set(value As Integer)
            leftShiftField = value
        End Set
    End Property


    Public Overridable Property Shift As Integer
        Get
            Return shiftField
        End Get
        Set(value As Integer)
            shiftField = value
        End Set
    End Property


    Public Overridable Property Increment As Integer
        Get
            Return incrementField
        End Get
        Set(value As Integer)
            incrementField = value
        End Set
    End Property


    Public Overridable ReadOnly Property DwellTime As Double
        Get
            Return dwellTimeField
        End Get
    End Property

    Public Overridable Property DspPhase As Double
        Get
            Return dspPhaseField
        End Get
        Set(value As Double)
            dspPhaseField = value
        End Set
    End Property


    Public Overridable Property WindowFunction As WindowFunctions
        Get
            Return windowFunctionField
        End Get
        Set(value As WindowFunctions)
            windowFunctionField = value
        End Set
    End Property


    Public Overridable Property ByteOrder As ByteOrder
        Get
            Return byteOrderField
        End Get
        Set(value As ByteOrder)
            byteOrderField = value
        End Set
    End Property


    Public Overridable Function is32Bit() As Boolean
        Return integerType
    End Function

    Public Overridable Sub set32Bit(integerType As Boolean)
        Me.integerType = integerType
    End Sub
End Class
