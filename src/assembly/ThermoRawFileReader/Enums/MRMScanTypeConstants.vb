
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