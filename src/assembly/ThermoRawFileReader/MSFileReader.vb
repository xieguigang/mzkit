Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports stdNum = System.Math

Public Class MSFileReader : Implements IDisposable

    Public Const GET_SCAN_DATA_WARNING As String = "GetScanData2D returned no data for scan"

    ReadOnly mFilePath As String

    Dim mRawFileReader As XRawFileIO

    Public Property ScanMin As Integer
    Public Property ScanMax As Integer

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Sub New(filePath As String)
        mFilePath = filePath
        mRawFileReader = Nothing
    End Sub

    Public Sub LoadFile()
        Call LoadFile(New ThermoReaderOptions)
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

    Private Function InitReader() As ThermoReaderOptions
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
            Call mRawFileReader.RaiseErrorMessage(String.Format("Exception {0}: {1}", currentTask, ex.Message), ex)
        End Try

        Return options
    End Function

    ''' <summary>
    ''' Get the LabelData (if FTMS) or PeakData (if not FTMS) as an enumerable list
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function GetLabelData() As IEnumerable(Of RawLabelData)
        Dim options As ThermoReaderOptions = InitReader()
        Dim rt As Double = Nothing

        For i As Integer = options.MinScan To options.MaxScan
            Dim scanInfo As SingleScanInfo = Nothing
            Dim data As FTLabelInfoType() = GetScanData(i, scanInfo)

            ' XRawFileIO.udtMassPrecisionInfoType[] precisionInfo;
            ' mRawFileReader.GetScanPrecisionData(i, out precisionInfo);
            ' Console.WriteLine("PrecisionInfoCount: " + precisionInfo.Length);

            If data Is Nothing Then
                Continue For
            End If

            Dim maxInt = data.Max(Function(x) x.Intensity)

            ' Check for the maximum intensity being zero
            If stdNum.Abs(maxInt) < Single.Epsilon Then
                Continue For
            End If

            Dim dataFiltered = data _
                .Where(Function(x)
                           Return x.Intensity >= options.MinIntensityThreshold AndAlso
                               x.Intensity / maxInt >= options.MinRelIntensityThresholdRatio AndAlso
                               x.Mass >= options.MinMz AndAlso
                               x.Mass <= options.MaxMz AndAlso
                               x.SignalToNoise >= options.SignalToNoiseThreshold
                       End Function) _
                .ToList()

            If dataFiltered.Count = 0 Then
                Continue For
            Else
                mRawFileReader.GetRetentionTime(i, rt)
            End If

            Yield New RawLabelData With {
                .ScanNumber = i,
                .ScanTime = rt,
                .MSData = dataFiltered,
                .MaxIntensity = maxInt
            }
        Next
    End Function

    Public Function GetScanInfo(scanNumber As Integer) As SingleScanInfo
        Dim scanInfo As SingleScanInfo = Nothing
        Call mRawFileReader.GetScanInfo(scanNumber, scanInfo)
        Return scanInfo
    End Function

    ''' <summary>
    ''' Get the LabelData (if FTMS) or PeakData (if not FTMS)
    ''' </summary>
    ''' <param name="scanNumber"></param>
    ''' <returns>
    ''' the MS matrix data
    ''' </returns>
    Private Function GetScanData(scanNumber As Integer, ByRef scanInfo As SingleScanInfo) As FTLabelInfoType()
        If Not mRawFileReader.GetScanInfo(scanNumber, scanInfo) Then
            Return Nothing
        Else
            Return If(scanInfo.IsFTMS, GetLabelData(scanNumber), GetPeakData(scanNumber).ToArray)
        End If
    End Function

    ''' <summary>
    ''' Get the label data for the given scan
    ''' </summary>
    ''' <param name="scanNumber"></param>
    ''' <returns></returns>
    Private Function GetLabelData(scanNumber As Integer) As FTLabelInfoType()
        Dim labelData As FTLabelInfoType() = Nothing

        Call mRawFileReader.GetScanLabelData(scanNumber, labelData)

        If labelData.Length > 0 Then
            Return labelData
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Get the peak data for the given scan
    ''' </summary>
    ''' <param name="scanNumber"></param>
    ''' <returns></returns>
    Private Iterator Function GetPeakData(scanNumber As Integer) As IEnumerable(Of FTLabelInfoType)
        Const MAX_NUMBER_OF_PEAKS = 0
        Const CENTROID_DATA = True

        Dim peakData As Double(,) = Nothing
        Dim dataCount = mRawFileReader.GetScanData2D(scanNumber, peakData, MAX_NUMBER_OF_PEAKS, CENTROID_DATA)

        If peakData.Length <= 0 Then
            ' Report message: GetScanData2D returned no data for scan 2760
            ' See, for example QC_Shew_13_04_2c_22Sep13_Cougar_13-06-16
            Call mRawFileReader.RaiseWarningMessage(String.Format("{0} {1}", GET_SCAN_DATA_WARNING, scanNumber))
        Else
            For i As Integer = 0 To dataCount - 1
                Yield New FTLabelInfoType With {
                    .Mass = peakData(0, i),
                    .Intensity = peakData(1, i)
                }
            Next
        End If
    End Function
End Class