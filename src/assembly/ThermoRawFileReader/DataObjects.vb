Imports System
Imports System.Collections.Generic
Imports ThermoFisher.CommonCore.Data.Business
Imports System.Runtime.InteropServices

' Ignore Spelling: struct, cid, etd, hcd, EThcD, ETciD, sa

''' <summary>
''' Type for Tune Methods
''' </summary>
<CLSCompliant(True)>
Public Class TuneMethod

    ''' <summary>
    '''         /// Settings
    '''         /// </summary>
    Public ReadOnly Settings As New List(Of ThermoRawFileReader.TuneMethodSettingType)

End Class

''' <summary>
''' Type for Tune Method Settings
''' </summary>
<CLSCompliant(True)>
Public Structure TuneMethodSettingType
    ''' <summary>
    ''' Tune category
    ''' </summary>
    Public Category As String

    ''' <summary>
    ''' Tune name
    ''' </summary>
    Public Name As String

    ''' <summary>
    ''' Tune value
    ''' </summary>
    Public Value As String

    ''' <summary>
    ''' Display the category, name, and value of this setting
    ''' </summary>
    Public Overrides Function ToString() As String
        Return String.Format("{0,-20}  {1,-40} = {2}", If(Category, "Undefined") & ":", If(Name, String.Empty), If(Value, String.Empty))
    End Function
End Structure

''' <summary>
''' Type for File Information
''' </summary>
<CLSCompliant(True)>
Public Class RawFileInfo
    ''' <summary>
    ''' Date of Acquisition
    ''' </summary>
    ''' <remarks>Will often be blank</remarks>
    Public AcquisitionDate As String

    ''' <summary>
    ''' Acquisition Filename
    ''' </summary>
    ''' <remarks>Will often be blank</remarks>
    Public AcquisitionFilename As String

    ''' <summary>
    ''' First Comment
    ''' </summary>
    ''' <remarks>Will often be blank</remarks>
    Public Comment1 As String

    ''' <summary>
    ''' Second Comment
    ''' </summary>
    ''' <remarks>Will often be blank</remarks>
    Public Comment2 As String

    ''' <summary>
    ''' Sample Name
    ''' </summary>
    ''' <remarks>Will often be blank</remarks>
    Public SampleName As String

    ''' <summary>
    ''' Sample Comment
    ''' </summary>
    ''' <remarks>Will often be blank</remarks>
    Public SampleComment As String

    ''' <summary>
    ''' Creation date
    ''' </summary>
    Public CreationDate As Date

    ''' <summary>
    ''' Username of the user when the file was created
    ''' </summary>
    Public CreatorID As String

    ''' <summary>
    ''' Dictionary tracking the device data stored in the .raw file
    ''' Keys are Device type, values are the number of devices of this type
    ''' </summary>
    ''' <remarks>
    ''' Typically a .raw file has a single device, of type Device.MS
    ''' Some .raw files also have LC information, stored as Device.Analog or Device.UV
    ''' </remarks>
    Public ReadOnly Devices As Dictionary(Of Device, Integer)

    ''' <summary>
    ''' Instrument Flags
    ''' </summary>
    ''' <remarks>Values should be one of the constants in the InstFlags class (file Enums.cs)</remarks>
    Public InstFlags As String

    ''' <summary>
    ''' Instrument Hardware Version
    ''' </summary>
    Public InstHardwareVersion As String

    ''' <summary>
    ''' Instrument Software Version
    ''' </summary>
    Public InstSoftwareVersion As String

    ''' <summary>
    ''' Instrument Methods
    ''' </summary>
    ''' <remarks>Typically only have one instrument method; the length of this array defines the number of instrument methods</remarks>
    Public ReadOnly InstMethods As List(Of String)

    ''' <summary>
    ''' Instrument Model
    ''' </summary>
    Public InstModel As String

    ''' <summary>
    ''' Instrument Name
    ''' </summary>
    Public InstName As String

    ''' <summary>
    ''' Instrument Description
    ''' </summary>
    ''' <remarks>Typically only defined for instruments converted from other formats</remarks>
    Public InstrumentDescription As String

    ''' <summary>
    ''' Instrument Serial Number
    ''' </summary>
    Public InstSerialNumber As String

    ''' <summary>
    ''' Tune Methods
    ''' </summary>
    ''' <remarks>Typically have one or two tune methods; the length of this array defines the number of tune methods defined</remarks>
    Public ReadOnly TuneMethods As List(Of TuneMethod)

    ''' <summary>
    ''' File format Version Number
    ''' </summary>
    Public VersionNumber As Integer

    ''' <summary>
    ''' Mass Resolution
    ''' </summary>
    Public MassResolution As Double

    ''' <summary>
    ''' First scan number
    ''' </summary>
    Public ScanStart As Integer

    ''' <summary>
    ''' Last scan number
    ''' </summary>
    Public ScanEnd As Integer

    ''' <summary>
    ''' Flag for corrupt files
    ''' </summary>
    ''' <remarks>Auto-set to true if the file is corrupt / has no data</remarks>
    Public CorruptFile As Boolean

    ''' <summary>
    ''' Reset all data in the struct
    ''' </summary>
    Public Sub Clear()
        AcquisitionDate = String.Empty
        AcquisitionFilename = String.Empty
        Comment1 = String.Empty
        Comment2 = String.Empty
        SampleName = String.Empty
        SampleComment = String.Empty
        CreationDate = Date.MinValue
        CreatorID = String.Empty
        Devices.Clear()
        InstFlags = String.Empty
        InstHardwareVersion = String.Empty
        InstSoftwareVersion = String.Empty
        InstMethods.Clear()
        InstModel = String.Empty
        InstName = String.Empty
        InstrumentDescription = String.Empty
        InstSerialNumber = String.Empty
        TuneMethods.Clear()
        VersionNumber = 0
        MassResolution = 0
        ScanStart = 0
        ScanEnd = 0
        CorruptFile = False
    End Sub

    ''' <summary>
    ''' Constructor
    ''' </summary>
    Public Sub New()
        AcquisitionDate = String.Empty
        AcquisitionFilename = String.Empty
        Comment1 = String.Empty
        Comment2 = String.Empty
        SampleName = String.Empty
        SampleComment = String.Empty
        CreationDate = Date.MinValue
        CreatorID = String.Empty
        Devices = New Dictionary(Of Device, Integer)()
        InstFlags = String.Empty
        InstHardwareVersion = String.Empty
        InstSoftwareVersion = String.Empty
        InstMethods = New List(Of String)()
        InstModel = String.Empty
        InstName = String.Empty
        InstrumentDescription = String.Empty
        InstSerialNumber = String.Empty
        TuneMethods = New List(Of TuneMethod)()
        VersionNumber = 0
        MassResolution = 0
        ScanStart = 0
        ScanEnd = 0
        CorruptFile = False
    End Sub
End Class

''' <summary>
''' Type for storing MRM Mass Ranges
''' </summary>
<CLSCompliant(True)>
Public Structure MRMMassRangeType
    ''' <summary>
    ''' Start Mass
    ''' </summary>
    Public StartMass As Double

    ''' <summary>
    ''' End Mass
    ''' </summary>
    Public EndMass As Double

    ''' <summary>
    ''' Central Mass
    ''' </summary>
    Public CentralMass As Double      ' Useful for MRM/SRM experiments

    ''' <summary>
    ''' Return a summary of this object
    ''' </summary>
    Public Overrides Function ToString() As String
        Return StartMass.ToString("0.000") & "-" & EndMass.ToString("0.000")
    End Function
End Structure

''' <summary>
''' Type for MRM Information
''' </summary>
<CLSCompliant(True)>
Public Class MRMInfo

    '''         <summary>
    '''      List of mass ranges monitored by the first quadrupole
    '''        </summary>
    Public ReadOnly MRMMassList As New List(Of ThermoRawFileReader.MRMMassRangeType)


    ''' <summary>
    ''' Duplicate the MRM info
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="target"></param>
    Public Shared Sub DuplicateMRMInfo(ByVal source As MRMInfo, <Out> ByRef target As MRMInfo)
        target = New MRMInfo()

        For Each item In source.MRMMassList
            target.MRMMassList.Add(item)
        Next
    End Sub
End Class

''' <summary>
''' Type for storing Parent Ion Information
''' </summary>
<CLSCompliant(True)>
Public Structure ParentIonInfoType
    ''' <summary>
    ''' MS Level of the spectrum
    ''' </summary>
    ''' <remarks>1 for MS1 spectra, 2 for MS2, 3 for MS3</remarks>
    Public MSLevel As Integer

    ''' <summary>
    ''' Parent ion m/z
    ''' </summary>
    Public ParentIonMZ As Double

    ''' <summary>
    ''' Collision mode
    ''' </summary>
    ''' <remarks>Examples: cid, etd, hcd, EThcD, ETciD</remarks>
    Public CollisionMode As String

    ''' <summary>
    ''' Secondary collision mode
    ''' </summary>
    ''' <remarks>
    ''' For example, for filter string: ITMS + c NSI r d sa Full ms2 1143.72@etd120.55@cid20.00 [120.00-2000.00]
    ''' CollisionMode = ETciD
    ''' CollisionMode2 = cid
    ''' </remarks>
    Public CollisionMode2 As String

    ''' <summary>
    ''' Collision energy
    ''' </summary>
    Public CollisionEnergy As Single

    ''' <summary>
    ''' Secondary collision energy
    ''' </summary>
    ''' <remarks>
    ''' For example, for filter string: ITMS + c NSI r d sa Full ms2 1143.72@etd120.55@cid20.00 [120.00-2000.00]
    ''' CollisionEnergy = 120.55
    ''' CollisionEnergy2 = 20.0
    ''' </remarks>
    Public CollisionEnergy2 As Single

    ''' <summary>
    ''' Activation type
    ''' </summary>
    ''' <remarks>Examples: CID, ETD, or HCD</remarks>
    Public ActivationType As ActivationTypeConstants

    ''' <summary>
    ''' Clear the data
    ''' </summary>
    Public Sub Clear()
        MSLevel = 1
        ParentIonMZ = 0
        CollisionMode = String.Empty
        CollisionMode2 = String.Empty
        CollisionEnergy = 0
        CollisionEnergy2 = 0
        ActivationType = ActivationTypeConstants.Unknown
    End Sub

    ''' <summary>
    ''' Return a simple summary of the object
    ''' </summary>
    Public Overrides Function ToString() As String
        If String.IsNullOrWhiteSpace(CollisionMode) Then
            Return "ms" & MSLevel & " " & ParentIonMZ.ToString("0.0#")
        End If

        Return "ms" & MSLevel & " " & ParentIonMZ.ToString("0.0#") & "@" & CollisionMode & CollisionEnergy.ToString("0.00")
    End Function
End Structure

''' <summary>
''' Type for Mass Precision Information
''' </summary>
<CLSCompliant(True)>
Public Structure MassPrecisionInfoType
    ''' <summary>
    ''' Peak Intensity
    ''' </summary>
    Public Intensity As Double

    ''' <summary>
    ''' Peak Mass
    ''' </summary>
    Public Mass As Double

    ''' <summary>
    ''' Peak Accuracy (in MMU)
    ''' </summary>
    Public AccuracyMMU As Double

    ''' <summary>
    ''' Peak Accuracy (in PPM)
    ''' </summary>
    Public AccuracyPPM As Double

    ''' <summary>
    ''' Peak Resolution
    ''' </summary>
    Public Resolution As Double
End Structure

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
