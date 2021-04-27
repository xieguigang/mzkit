Public Class RawLabelData
    ''' <summary>
    ''' Scan number
    ''' </summary>
    Public Property ScanNumber As Integer

    ''' <summary>
    ''' Acquisition time (in minutes)
    ''' </summary>
    Public Property ScanTime As Double

    ''' <summary>
    ''' Label data (if FTMS), otherwise peak data
    ''' </summary>
    Public Property MSData As List(Of udtFTLabelInfoType)

    ''' <summary>
    ''' Maximum intensity of the peaks in this scan
    ''' </summary>
    Public Property MaxIntensity As Double
End Class