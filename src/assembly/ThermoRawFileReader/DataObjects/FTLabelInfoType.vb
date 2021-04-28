Imports System.Runtime.InteropServices
Imports ThermoFisher.CommonCore.Data.Business

Namespace DataObjects

    ''' <summary>
    ''' Type for storing FT Label Information
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure FTLabelInfoType
        ''' <summary>
        ''' Peak m/z
        ''' </summary>
        ''' <remarks>This is observed m/z; it is not monoisotopic mass</remarks>
        Public Mass As Double

        ''' <summary>
        ''' Peak Intensity
        ''' </summary>
        Public Intensity As Double

        ''' <summary>
        ''' Peak Resolution
        ''' </summary>
        Public Resolution As Single

        ''' <summary>
        ''' Peak Baseline
        ''' </summary>
        Public Baseline As Single

        ''' <summary>
        ''' Peak Noise
        ''' </summary>
        ''' <remarks>For signal/noise ratio, see SignalToNoise</remarks>
        Public Noise As Single

        ''' <summary>
        ''' Peak Charge
        ''' </summary>
        ''' <remarks>Will be 0 if the charge could not be determined</remarks>
        Public Charge As Integer

        ''' <summary>
        ''' Signal to noise ratio
        ''' </summary>
        ''' <returns>Intensity divided by noise, or 0 if Noise is 0</returns>
        Public ReadOnly Property SignalToNoise As Double
            Get
                If Noise > 0 Then Return Intensity / Noise
                Return 0
            End Get
        End Property

        ''' <summary>
        ''' Return a summary of this object
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Format("m/z {0,9:F3}, S/N {1,7:F1}, intensity {2,12:F0}", Mass, SignalToNoise, Intensity)
        End Function
    End Structure
End Namespace