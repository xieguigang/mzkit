
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