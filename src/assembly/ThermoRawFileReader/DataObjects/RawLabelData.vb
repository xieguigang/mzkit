Namespace DataObjects

    Public Class RawLabelData
        ''' <summary>
        ''' Scan number
        ''' </summary>
        Public Property ScanNumber As Integer

        ''' <summary>
        ''' Acquisition time (in minutes)
        ''' </summary>
        Public Property ScanTime As Double

        Public Property MsLevel As Integer

        ''' <summary>
        ''' Label data (if FTMS), otherwise peak data
        ''' </summary>
        Public Property MSData As List(Of FTLabelInfoType)

        ''' <summary>
        ''' Maximum intensity of the peaks in this scan
        ''' </summary>
        Public Property MaxIntensity As Double

        Public Overrides Function ToString() As String
            Return $"[{ScanNumber}] RT: {ScanTime} = {MaxIntensity}"
        End Function
    End Class
End Namespace