#Region "Microsoft.VisualBasic::7d0a23b1903dd26c94b8d0a5ab522573, G:/mzkit/src/assembly/ThermoRawFileReader//DeviceInfo.vb"

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

    '   Total Lines: 144
    '    Code Lines: 73
    ' Comment Lines: 49
    '   Blank Lines: 22
    '     File Size: 4.59 KB


    ' Class DeviceInfo
    ' 
    '     Properties: AxisLabelX, AxisLabelY, DeviceDescription, DeviceNumber, DeviceType
    '                 InstrumentName, Model, SerialNumber, SoftwareVersion, Units
    '                 YAxisLabelWithUnits
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetYAxisLabelWithUnits, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports ThermoFisher.CommonCore.Data.Business

''' <summary>
''' Tracks information on the source device for data stored in a Thermo .raw file
''' </summary>
<CLSCompliant(True)>
Public Class DeviceInfo
    ''' <summary>
    ''' Device type
    ''' </summary>
    Public ReadOnly Property DeviceType As Device

    ''' <summary>
    ''' Device number (for this device type)
    ''' </summary>
    ''' <remarks>Each device type starts with device number 1</remarks>
    Public ReadOnly Property DeviceNumber As Integer

    ''' <summary>
    ''' Returns a human-readable description of the device
    ''' </summary>
    ''' <returns>
    ''' "Mass Spectrometer" if DeviceType is MS or MSAnalog
    ''' Otherwise, a description in the form "Analog Device #1"
    ''' </returns>
    Public ReadOnly Property DeviceDescription As String
        Get
            If DeviceType = Device.MS OrElse DeviceType = Device.MSAnalog Then
                Return "Mass Spectrometer"
            Else
                Return String.Format("{0} device #{1}", DeviceType.ToString(), DeviceNumber)
            End If
        End Get
    End Property

    ''' <summary>
    ''' Instrument name (device name)
    ''' </summary>
    Public Property InstrumentName As String

    ''' <summary>
    ''' Instrument model
    ''' </summary>
    Public Property Model As String

    ''' <summary>
    ''' Instrument serial number
    ''' </summary>
    Public Property SerialNumber As String

    ''' <summary>
    ''' Acquisition software version
    ''' </summary>
    Public Property SoftwareVersion As String

    ''' <summary>
    ''' Units for stored intensity data for this device
    ''' </summary>
    Public Property Units As DataUnits

    ''' <summary>
    ''' X axis label for plotting data vs. scan
    ''' </summary>
    Public Property AxisLabelX As String

    ''' <summary>
    ''' Y axis label for plotting data vs. scan
    ''' </summary>
    Public Property AxisLabelY As String

    ''' <summary>
    ''' Y axis label, along with the units in parentheses
    ''' </summary>
    Public ReadOnly Property YAxisLabelWithUnits As String
        Get
            If String.IsNullOrWhiteSpace(AxisLabelY) Then
                Return String.Empty
            Else
                Return GetYAxisLabelWithUnits()
            End If
        End Get
    End Property

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="deviceType">Device type</param>
    ''' <param name="deviceNumber">Device number (for this device type)</param>
    Public Sub New(deviceType As Device, deviceNumber As Integer)
        Me.DeviceType = deviceType
        Me.DeviceNumber = deviceNumber
    End Sub

    Private Function GetYAxisLabelWithUnits() As String
        Dim isPressure = AxisLabelY.IndexOf("pressure", StringComparison.OrdinalIgnoreCase) >= 0

        Select Case Units
            Case DataUnits.AbsorbanceUnits

                If isPressure Then
                    Return AxisLabelY
                Else
                    Return AxisLabelY & " (AU)"
                End If

            Case DataUnits.MilliAbsorbanceUnits

                If isPressure Then
                    Return AxisLabelY & " x1E-3"
                Else
                    Return AxisLabelY & " (mAU)"
                End If

            Case DataUnits.MicroAbsorbanceUnits

                If isPressure Then
                    Return AxisLabelY & " x1E-6"
                Else
                    Return AxisLabelY & " (μAU)"
                End If

            Case DataUnits.Volts
                Return AxisLabelY & " (V)"
            Case DataUnits.MilliVolts
                Return AxisLabelY & " (mV)"
            Case DataUnits.MicroVolts
                Return AxisLabelY & " (μV)"
            Case DataUnits.None
                Return AxisLabelY & " (counts)"
            Case Else
                Return AxisLabelY
        End Select
    End Function

    ''' <summary>
    ''' Display the device type and instrument model or name
    ''' </summary>
    Public Overrides Function ToString() As String
        If Not String.IsNullOrWhiteSpace(Model) Then Return String.Format("{0}: {1}", DeviceType.ToString(), Model)
        If Not String.IsNullOrWhiteSpace(InstrumentName) Then Return String.Format("{0}: {1}", DeviceType.ToString(), InstrumentName)

        Return String.Format("{0} device", DeviceType.ToString())
    End Function
End Class
