Imports System
Imports ThermoFisher.CommonCore.Data.Business

' ReSharper disable UnusedMember.Global


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
            If DeviceType = Device.MS OrElse DeviceType = Device.MSAnalog Then Return "Mass Spectrometer"
            Return String.Format("{0} device #{1}", DeviceType.ToString(), DeviceNumber)
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
            If String.IsNullOrWhiteSpace(AxisLabelY) Then Return String.Empty
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

    ''' <summary>
    ''' Display the device type and instrument model or name
    ''' </summary>
    Public Overrides Function ToString() As String
        If Not String.IsNullOrWhiteSpace(Model) Then Return String.Format("{0}: {1}", DeviceType.ToString(), Model)
        If Not String.IsNullOrWhiteSpace(InstrumentName) Then Return String.Format("{0}: {1}", DeviceType.ToString(), InstrumentName)
        Return String.Format("{0} device", DeviceType.ToString())
    End Function
End Class
