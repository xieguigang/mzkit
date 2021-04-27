Public Class RawFileReader : Implements IDisposable

    Private _ScanMin As Integer, _ScanMax As Integer
    Public Const GET_SCAN_DATA_WARNING As String = "GetScanData2D returned no data for scan"
    Private ReadOnly mFilePath As String
    Private mRawFileReader As XRawFileIO

    Public Property ScanMin As Integer
        Get
            Return _ScanMin
        End Get
        Private Set(ByVal value As Integer)
            _ScanMin = value
        End Set
    End Property

    Public Property ScanMax As Integer
        Get
            Return _ScanMax
        End Get
        Private Set(ByVal value As Integer)
            _ScanMax = value
        End Set
    End Property

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Sub New(ByVal filePath As String)
        mFilePath = filePath
        mRawFileReader = Nothing
    End Sub

    ''' <summary>
    ''' Open the raw file with a new instance of XRawFileIO
    ''' </summary>
    ''' <param name="readerOptions"></param>
    Public Sub LoadFile(readerOptions As ThermoReaderOptions)
        mRawFileReader = New XRawFileIO(readerOptions)
        mRawFileReader.OpenRawFile(mFilePath)
        ScanMin = 1
        ScanMax = mRawFileReader.GetNumScans()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        mRawFileReader?.CloseRawFile()
    End Sub

    ''' <summary>
    ''' Get the LabelData (if FTMS) or PeakData (if not FTMS) as an enumerable list
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function GetLabelData() As IEnumerable(Of RawLabelData)
        Dim currentTask = "Initializing"
        Dim options As ThermoReaderOptions = mRawFileReader.Options

        Try

            If mRawFileReader Is Nothing Then
                currentTask = "Opening the .raw file"
                LoadFile(options)
            End If

            currentTask = "Validating the scan range"

            If options.MinScan < ScanMin Then
                options.MinScan = ScanMin
            End If

            If options.MaxScan > ScanMax OrElse options.MaxScan < 0 Then
                options.MaxScan = ScanMax
            End If

        Catch ex As Exception
            OnErrorEvent(String.Format("Exception {0}: {1}", currentTask, ex.Message), ex)
        End Try

        Dim rt As Double = Nothing

        For i = options.MinScan To options.MaxScan
            Dim data = GetScanData(i)

            ' XRawFileIO.udtMassPrecisionInfoType[] precisionInfo;
            ' mRawFileReader.GetScanPrecisionData(i, out precisionInfo);
            ' Console.WriteLine("PrecisionInfoCount: " + precisionInfo.Length);

            If data Is Nothing Then Continue For
            Dim maxInt = data.Max(Function(x) x.Intensity)

            ' Check for the maximum intensity being zero
            If Math.Abs(maxInt) < Single.Epsilon Then Continue For
            Dim dataFiltered = data.Where(Function(x)
                                              Return x.Intensity >= options.MinIntensityThreshold AndAlso x.Intensity / maxInt >= options.MinRelIntensityThresholdRatio AndAlso x.Mass >= options.MinMz AndAlso x.Mass <= options.MaxMz AndAlso x.SignalToNoise >= options.SignalToNoiseThreshold
                                          End Function).ToList()
            If dataFiltered.Count = 0 Then Continue For
            mRawFileReader.GetRetentionTime(i, rt)
            Yield New RawLabelData With {
                .ScanNumber = i,
                .ScanTime = rt,
                .MSData = dataFiltered,
                .MaxIntensity = maxInt
            }
        Next
    End Function

    ''' <summary>
    ''' Get the LabelData (if FTMS) or PeakData (if not FTMS)
    ''' </summary>
    ''' <param name="scanNumber"></param>
    ''' <returns></returns>
    Private Function GetScanData(ByVal scanNumber As Integer) As List(Of FTLabelInfoType)
        Dim scanInfo As clsScanInfo = Nothing
        If Not mRawFileReader.GetScanInfo(scanNumber, scanInfo) Then Return Nothing
        Return If(scanInfo.IsFTMS, GetLabelData(scanNumber), GetPeakData(scanNumber))
    End Function

    ''' <summary>
    ''' Get the label data for the given scan
    ''' </summary>
    ''' <param name="scanNumber"></param>
    ''' <returns></returns>
    Private Function GetLabelData(ByVal scanNumber As Integer) As List(Of FTLabelInfoType)
        Dim labelData As FTLabelInfoType() = Nothing
        mRawFileReader.GetScanLabelData(scanNumber, labelData)

        If labelData.Length > 0 Then
            Return labelData.ToList()
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Get the peak data for the given scan
    ''' </summary>
    ''' <param name="scanNumber"></param>
    ''' <returns></returns>
    Private Function GetPeakData(ByVal scanNumber As Integer) As List(Of FTLabelInfoType)
        Const MAX_NUMBER_OF_PEAKS = 0
        Const CENTROID_DATA = True
        Dim peakData As Double(,) = Nothing
        Dim dataCount = mRawFileReader.GetScanData2D(scanNumber, peakData, MAX_NUMBER_OF_PEAKS, CENTROID_DATA)

        If peakData.Length <= 0 Then
            ' Report message: GetScanData2D returned no data for scan 2760
            ' See, for example QC_Shew_13_04_2c_22Sep13_Cougar_13-06-16
            OnWarningEvent(String.Format("{0} {1}", GET_SCAN_DATA_WARNING, scanNumber))
            Return Nothing
        End If

        Dim data = New List(Of FTLabelInfoType)(dataCount)

        For i = 0 To dataCount - 1
            Dim peak = New FTLabelInfoType With {
                .Mass = peakData(0, i),
                .Intensity = peakData(1, i)
            }
            data.Add(peak)
        Next

        Return data
    End Function
End Class