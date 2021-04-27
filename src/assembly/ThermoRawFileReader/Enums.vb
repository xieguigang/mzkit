''' <summary>
''' Activation Types enum
''' </summary>
''' <remarks>This enum mirrors ThermoFisher.CommonCore.Data.FilterEnums.ActivationType</remarks>
<CLSCompliant(True)>
Public Enum ActivationTypeConstants
    ''' <summary>
    ''' Unknown activation type
    ''' </summary>
    Unknown = -1

    ''' <summary>
    ''' Collision-Induced Dissociation
    ''' </summary>
    CID = 0

    ''' <summary>
    ''' Multi Photon Dissociation
    ''' </summary>
    MPD = 1

    ''' <summary>
    ''' Electron Capture Dissociation
    ''' </summary>
    ECD = 2

    ''' <summary>
    ''' Pulsed Q Dissociation
    ''' </summary>
    PQD = 3

    ''' <summary>
    ''' Electron Transfer Dissociation
    ''' </summary>
    ETD = 4

    ''' <summary>
    ''' High-energy Collision-induce Dissociation (psi-ms: beam-type collision-induced dissociation)
    ''' </summary>
    HCD = 5

    ''' <summary>
    ''' Any activation type
    ''' </summary>
    AnyType = 6

    ''' <summary>
    ''' Supplemental Activation
    ''' </summary>
    SA = 7

    ''' <summary>
    ''' Proton Transfer Reaction
    ''' </summary>
    PTR = 8

    ' ReSharper disable once IdentifierTypo
    ''' <summary>
    ''' Negative Electron Transfer Dissociation
    ''' </summary>
    NETD = 9

    ' ReSharper disable once IdentifierTypo
    ''' <summary>
    ''' Negative Proton Transfer Reaction
    ''' </summary>
    NPTR = 10

    ''' <summary>
    ''' Ultraviolet Photo Dissociation
    ''' </summary>
    UVPD = 11

    ''' <summary>
    ''' Mode A
    ''' </summary>
    ModeA = 12

    ''' <summary>
    ''' Mode B
    ''' </summary>
    ModeB = 13

    ''' <summary>
    ''' Mode C
    ''' </summary>
    ModeC = 14

    ''' <summary>
    ''' Mode D
    ''' </summary>
    ModeD = 15

    ''' <summary>
    ''' Mode E
    ''' </summary>
    ModeE = 16

    ''' <summary>
    ''' Mode F
    ''' </summary>
    ModeF = 17

    ''' <summary>
    ''' Mode G
    ''' </summary>
    ModeG = 18

    ''' <summary>
    ''' Mode H
    ''' </summary>
    ModeH = 19

    ''' <summary>
    ''' Mode I
    ''' </summary>
    ModeI = 20

    ''' <summary>
    ''' Mode J
    ''' </summary>
    ModeJ = 21

    ''' <summary>
    ''' Mode K
    ''' </summary>
    ModeK = 22

    ''' <summary>
    ''' Mode L
    ''' </summary>
    ModeL = 23

    ''' <summary>
    ''' Mode M
    ''' </summary>
    ModeM = 24

    ''' <summary>
    ''' Mode N
    ''' </summary>
    ModeN = 25

    ''' <summary>
    ''' Mode O
    ''' </summary>
    ModeO = 26

    ''' <summary>
    ''' Mode P
    ''' </summary>
    ModeP = 27

    ''' <summary>
    ''' Mode Q
    ''' </summary>
    ModeQ = 28

    ''' <summary>
    ''' Mode R
    ''' </summary>
    ModeR = 29

    ''' <summary>
    ''' Mode S
    ''' </summary>
    ModeS = 30

    ''' <summary>
    ''' Mode T
    ''' </summary>
    ModeT = 31

    ''' <summary>
    ''' Mode U
    ''' </summary>
    ModeU = 32

    ''' <summary>
    ''' Mode V
    ''' </summary>
    ModeV = 33

    ''' <summary>
    ''' Mode W
    ''' </summary>
    ModeW = 34

    ''' <summary>
    ''' Mode X
    ''' </summary>
    ModeX = 35

    ''' <summary>
    ''' Mode Y
    ''' </summary>
    ModeY = 36

    ''' <summary>
    ''' Mode Z
    ''' </summary>
    ModeZ = 37

    ''' <summary>
    ''' Last Activation
    ''' </summary>
    LastActivation = 38
End Enum

''' <summary>
''' MRM Scan Types
''' </summary>
<CLSCompliant(True)>
Public Enum MRMScanTypeConstants
    ''' <summary>
    ''' Not MRM
    ''' </summary>
    NotMRM = 0

    ''' <summary>
    ''' Multiple SIM ranges in a single scan
    ''' </summary>
    MRMQMS = 1

    ''' <summary>
    ''' Monitoring a parent ion and one or more daughter ions
    ''' </summary>
    SRM = 2

    ''' <summary>
    ''' Full neutral loss scan
    ''' </summary>
    FullNL = 3

    ''' <summary>
    ''' Selected Ion Monitoring (SIM), which is MS1 of a limited m/z range
    ''' </summary>
    SIM = 4
End Enum

''' <summary>
''' Ion Modes
''' </summary>
<CLSCompliant(True)>
Public Enum IonModeConstants
    ''' <summary>
    ''' Unknown Ion Mode
    ''' </summary>
    Unknown = 0

    ''' <summary>
    ''' Positive Ion Mode
    ''' </summary>
    Positive = 1

    ''' <summary>
    ''' Negative Ion Mode
    ''' </summary>
    Negative = 2
End Enum

''' <summary>
''' Intensity Cutoff Types
''' </summary>
''' <remarks>Used with <see cref="XRawFileIO.mXRawFile"/> functions in <see cref="XRawFileIO.GetScanData2D"/> and <see cref="XRawFileIO.GetScanDataSumScans"/></remarks>
<CLSCompliant(True)>
Public Enum IntensityCutoffTypeConstants
    ''' <summary>
    ''' All Values Returned
    ''' </summary>
    None = 0

    ''' <summary>
    ''' Absolute Intensity Units
    ''' </summary>
    AbsoluteIntensityUnits = 1

    ''' <summary>
    ''' Intensity relative to base peak
    ''' </summary>
    RelativeToBasePeak = 2
End Enum

''' <summary>
''' Instrument Flags
''' </summary>
<CLSCompliant(True)>
Public Module InstFlags
    ''' <summary>
    ''' Total Ion Map
    ''' </summary>
    Public Const TIM As String = "Total Ion Map"

    ''' <summary>
    ''' Neutral Loss Map
    ''' </summary>
    Public Const NLM As String = "Neutral Loss Map"

    ''' <summary>
    ''' Parent Ion Map
    ''' </summary>
    Public Const PIM As String = "Parent Ion Map"

    ''' <summary>
    ''' Data Dependent ZoomScan Map
    ''' </summary>
    Public Const DDZMap As String = "Data Dependent ZoomScan Map"
End Module
