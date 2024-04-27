#Region "Microsoft.VisualBasic::09baccea3e27fffd41fcd7ace5476559, G:/mzkit/src/assembly/NMRFidTool//Acqu.vb"

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

    '   Total Lines: 434
    '    Code Lines: 327
    ' Comment Lines: 27
    '   Blank Lines: 80
    '     File Size: 13.24 KB


    ' Class Acqu
    ' 
    ' 
    '     Enum Spectrometer
    ' 
    '         BRUKER, JEOL, VARIAN
    ' 
    ' 
    ' 
    '     Class AcquisitionMode
    ' 
    '         Properties: AquiredPoints, ByteOrder, Decoupler1Freq, Decoupler2Feq, DspDecimation
    '                     DspFirmware, DspGroupDelay, FidType, FilterType, FreqOffset
    '                     InstrumentName, NumberOfScans, ObservedNucleus, Origin, Owner
    '                     Probehead, PulseProgram, Solvent, SpectralWidth, TransmiterFreq
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: getAcquisitionMode, is32Bit
    ' 
    '         Sub: set32Bit, (+2 Overloads) setAcquisitionMode
    '         Enum InnerEnum
    ' 
    '             CUSTOM_DISP, DISP, SEQUENTIAL, SIMULTANIOUS
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: Type
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ordinal, ToString, valueOf, values
    ' 
    '     Class FidData
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Enum InnerEnum
    ' 
    '             CUSTOM_DISP, DISP, SEQUENTIAL, SIMULTANIOUS
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: Type
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ordinal, ToString, valueOf, values
    ' 
    '  
    ' 
    '     Properties: SpectralFrequency
    ' 
    '     Function: getSpectrometer
    ' 
    ' /********************************************************************************/

#End Region

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
''' Data structure for the acquisition parameters
''' 
''' @author  Luis F. de Figueiredo
''' 
''' User: ldpf
''' Date: 14/01/2013
''' Time: 14:01
''' 
''' </summary>
Public Class Acqu


    'TODO use an enum for parameters that have a limited set of options such as aquisition mode
    Private transmiterFreqField As Double 'sfo1
    Private decoupler1FreqField As Double 'sfo2
    Private decoupler2FeqField As Double 'sfo3
    Private freqOffsetField As Double 'o1 (Hz)
    Private spectralFrequencyField As Double 'BF1 (Hz)
    Private spectralWidthField As Double 'sw            sweep width (ppm)
    Private aquiredPointsField As Integer 'td            acquired points (real + imaginary)
    Private dspDecimationField As Integer 'decim         DSP decimation factor
    Private dspFirmwareField As Integer 'dspfvs        DSP firmware version
    Private dspGroupDelayField As Double 'grpdly        DSP group delay
    Private filterTypeField As Integer 'digmod        filter type
    Private numberOfScansField As Integer 'ns            number of scans
    Private integerType As Boolean 'dtypa         data type (0 -> 32 bit int, 1 -> 64 bit double)
    Private pulseProgramField As String 'pulprog       pulse program
    Private observedNucleusField As String 'nuc1          observed nucleus
    Private instrumentNameField As String 'instrum       instrument name
    Private solventField As String 'solvent       solvent
    Private probeheadField As String 'probehead     probehead
    Private originField As String 'origin        origin
    Private ownerField As String 'owner         owner
    Private acquisitionModeField As AcquisitionMode 'aq_mod        acquisition mode
    Private fidTypeField As FidData 'fid_type      define in class data_par
    Private byteOrderField As ByteOrder 'bytorda       byte order (0 -> Little endian, 1 -> Big Endian)

    Private spectrometerField As Spectrometer

    Public Enum Spectrometer
        BRUKER
        VARIAN
        JEOL
    End Enum

    Public Sub New(spectrometer As Spectrometer)
        spectrometerField = spectrometer
    End Sub

    Public Overridable Property TransmiterFreq As Double
        Get
            Return transmiterFreqField
        End Get
        Set(value As Double)
            transmiterFreqField = value
        End Set
    End Property


    Public Overridable Property Decoupler1Freq As Double
        Get
            Return decoupler1FreqField
        End Get
        Set(value As Double)
            decoupler1FreqField = value
        End Set
    End Property


    Public Overridable Property Decoupler2Feq As Double
        Get
            Return decoupler2FeqField
        End Get
        Set(value As Double)
            decoupler2FeqField = value
        End Set
    End Property


    Public Overridable Property FreqOffset As Double
        Get
            Return freqOffsetField
        End Get
        Set(value As Double)
            freqOffsetField = value
        End Set
    End Property


    Public Overridable Property SpectralWidth As Double
        Get
            Return spectralWidthField
        End Get
        Set(value As Double)
            spectralWidthField = value
        End Set
    End Property


    Public Overridable Property AquiredPoints As Integer
        Get
            Return aquiredPointsField
        End Get
        Set(value As Integer)
            aquiredPointsField = value
        End Set
    End Property


    Public Overridable Property DspDecimation As Integer
        Get
            Return dspDecimationField
        End Get
        Set(value As Integer)
            dspDecimationField = value
        End Set
    End Property


    Public Overridable Property DspFirmware As Integer
        Get
            Return dspFirmwareField
        End Get
        Set(value As Integer)
            dspFirmwareField = value
        End Set
    End Property


    Public Overridable Property DspGroupDelay As Double
        Get
            Return dspGroupDelayField
        End Get
        Set(value As Double)
            dspGroupDelayField = value
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


    Public Overridable Function getAcquisitionMode() As AcquisitionMode
        Return acquisitionModeField
    End Function

    Public Overridable Sub setAcquisitionMode(acquisitionMode As Integer)
        For Each mode In Acqu.AcquisitionMode.values()
            If mode.typeField = acquisitionMode Then
                acquisitionModeField = mode
            End If
        Next
    End Sub
    Public Overridable Sub setAcquisitionMode(mode As AcquisitionMode)
        acquisitionModeField = mode

    End Sub

    Public Overridable Property FilterType As Integer
        Get
            Return filterTypeField
        End Get
        Set(value As Integer)
            filterTypeField = value
        End Set
    End Property


    Public Overridable Property NumberOfScans As Integer
        Get
            Return numberOfScansField
        End Get
        Set(value As Integer)
            numberOfScansField = value
        End Set
    End Property


    Public Overridable Property PulseProgram As String
        Get
            Return pulseProgramField
        End Get
        Set(value As String)
            pulseProgramField = value
        End Set
    End Property


    Public Overridable Property ObservedNucleus As String
        Get
            Return observedNucleusField
        End Get
        Set(value As String)
            observedNucleusField = value
        End Set
    End Property


    Public Overridable Property InstrumentName As String
        Get
            Return instrumentNameField
        End Get
        Set(value As String)
            instrumentNameField = value
        End Set
    End Property


    Public Overridable Function is32Bit() As Boolean
        Return integerType
    End Function

    Public Overridable Sub set32Bit(is32Bit As Boolean)
        fidTypeField = If(is32Bit, FidData.INT32, FidData.DOUBLE)
        integerType = is32Bit
    End Sub

    Public Overridable Property Solvent As String
        Get
            Return solventField
        End Get
        Set(value As String)
            solventField = value
        End Set
    End Property


    Public Overridable Property Probehead As String
        Get
            Return probeheadField
        End Get
        Set(value As String)
            probeheadField = value
        End Set
    End Property


    Public Overridable Property Origin As String
        Get
            Return originField
        End Get
        Set(value As String)
            originField = value
        End Set
    End Property


    Public Overridable Property Owner As String
        Get
            Return ownerField
        End Get
        Set(value As String)
            ownerField = value
        End Set
    End Property


    Public Overridable ReadOnly Property FidType As FidData
        Get
            Return fidTypeField
        End Get
    End Property

    Public NotInheritable Class AcquisitionMode
        Public Shared ReadOnly SEQUENTIAL As AcquisitionMode = New AcquisitionMode("SEQUENTIAL", InnerEnum.SEQUENTIAL, 1)
        Public Shared ReadOnly SIMULTANIOUS As AcquisitionMode = New AcquisitionMode("SIMULTANIOUS", InnerEnum.SIMULTANIOUS, 2)
        Public Shared ReadOnly DISP As AcquisitionMode = New AcquisitionMode("DISP", InnerEnum.DISP, 3)
        Public Shared ReadOnly CUSTOM_DISP As AcquisitionMode = New AcquisitionMode("CUSTOM_DISP", InnerEnum.CUSTOM_DISP, 4)

        Private Shared ReadOnly valueList As IList(Of AcquisitionMode) = New List(Of AcquisitionMode)()

        Shared Sub New()
            valueList.Add(SEQUENTIAL)
            valueList.Add(SIMULTANIOUS)
            valueList.Add(DISP)
            valueList.Add(CUSTOM_DISP)
        End Sub

        Public Enum InnerEnum
            SEQUENTIAL
            SIMULTANIOUS
            DISP
            CUSTOM_DISP
        End Enum

        Public ReadOnly innerEnumValue As InnerEnum
        Private ReadOnly nameValue As String
        Private ReadOnly ordinalValue As Integer
        Private Shared nextOrdinal As Integer = 0

        Friend ReadOnly typeField As Integer

        Friend Sub New(name As String, innerEnum As InnerEnum, type As Integer)
            typeField = type

            nameValue = name
            ordinalValue = Math.Min(Threading.Interlocked.Increment(nextOrdinal), nextOrdinal - 1)
            innerEnumValue = innerEnum
        End Sub

        Friend ReadOnly Property Type As Double
            Get
                Return typeField
            End Get
        End Property

        Public Shared Function values() As IList(Of AcquisitionMode)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As AcquisitionMode
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

    Public NotInheritable Class FidData
        Public Shared ReadOnly INT32 As FidData = New FidData("INT32", InnerEnum.INT32, 1)
        Public Shared ReadOnly [DOUBLE] As FidData = New FidData("DOUBLE", InnerEnum.DOUBLE, 2)
        Public Shared ReadOnly FLOAT As FidData = New FidData("FLOAT", InnerEnum.FLOAT, 3)
        Public Shared ReadOnly INT16 As FidData = New FidData("INT16", InnerEnum.INT16, 4)

        Private Shared ReadOnly valueList As IList(Of FidData) = New List(Of FidData)()

        Shared Sub New()
            valueList.Add(INT32)
            valueList.Add([DOUBLE])
            valueList.Add(FLOAT)
            valueList.Add(INT16)
        End Sub

        Public Enum InnerEnum
            INT32
            [DOUBLE]
            FLOAT
            INT16
        End Enum

        Public ReadOnly innerEnumValue As InnerEnum
        Private ReadOnly nameValue As String
        Private ReadOnly ordinalValue As Integer
        Private Shared nextOrdinal As Integer = 0

        Friend ReadOnly typeField As Integer

        Friend Sub New(name As String, innerEnum As InnerEnum, type As Integer)
            typeField = type

            nameValue = name
            ordinalValue = Math.Min(Threading.Interlocked.Increment(nextOrdinal), nextOrdinal - 1)
            innerEnumValue = innerEnum
        End Sub

        Friend ReadOnly Property Type As Double
            Get
                Return typeField
            End Get
        End Property

        Public Shared Function values() As IList(Of FidData)
            Return valueList
        End Function

        Public Function ordinal() As Integer
            Return ordinalValue
        End Function

        Public Overrides Function ToString() As String
            Return nameValue
        End Function

        Public Shared Function valueOf(name As String) As FidData
            For Each enumInstance In valueList
                If Equals(enumInstance.nameValue, name) Then
                    Return enumInstance
                End If
            Next
            Throw New ArgumentException(name)
        End Function
    End Class

    Public Overridable Property SpectralFrequency As Double
        Get
            Return spectralFrequencyField
        End Get
        Set(value As Double)
            spectralFrequencyField = value
        End Set
    End Property


    Public Overridable Function getSpectrometer() As Spectrometer
        Return spectrometerField
    End Function
End Class
