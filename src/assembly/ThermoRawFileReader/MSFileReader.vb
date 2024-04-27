#Region "Microsoft.VisualBasic::328e19e387a83867599a93560478effc, G:/mzkit/src/assembly/ThermoRawFileReader//MSFileReader.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 209
    '    Code Lines: 143
    ' Comment Lines: 31
    '   Blank Lines: 35
    '     File Size: 7.41 KB


    ' Class MSFileReader
    ' 
    '     Properties: FileName, Options, ScanMax, ScanMin, ScanTimeMax
    '                 ThermoReader
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: (+2 Overloads) GetLabelData, GetPeakData, GetScanData, GetScanInfo, InitReader
    '               ToString
    ' 
    '     Sub: Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports stdNum = System.Math

Public Class MSFileReader : Implements IDisposable

    Public Const GET_SCAN_DATA_WARNING As String = "GetScanData2D returned no data for scan"

    Dim mRawFileReader As XRawFileIO

    Public Property ScanMin As Integer
    Public Property ScanMax As Integer

    Public ReadOnly Property Options As ThermoReaderOptions
        Get
            Return mRawFileReader.Options
        End Get
    End Property

    Public ReadOnly Property FileName As String
        Get
            Return mRawFileReader.RawFilePath
        End Get
    End Property

    Public ReadOnly Property ThermoReader As XRawFileIO
        Get
            Return mRawFileReader
        End Get
    End Property

    Public ReadOnly Property ScanTimeMax As Double
        Get
            Dim rtmax As Double = Nothing
            Dim void = mRawFileReader.GetRetentionTime(mRawFileReader.ScanEnd, rtmax)
            Return rtmax
        End Get
    End Property

    ''' <summary>
    ''' Open the raw file with a new instance of XRawFileIO
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Sub New(filePath As String, Optional readerOptions As ThermoReaderOptions = Nothing)
        If readerOptions Is Nothing Then
            readerOptions = New ThermoReaderOptions
        End If

        filePath = filePath.GetFullPath
        mRawFileReader = New XRawFileIO(readerOptions)
        mRawFileReader.OpenRawFile(filePath)
        ScanMin = 1
        ScanMax = mRawFileReader.GetNumScans()
    End Sub

    Public Overrides Function ToString() As String
        Return mRawFileReader.RawFilePath
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        mRawFileReader?.CloseRawFile()
    End Sub

    Public Function InitReader() As ThermoReaderOptions
        Dim currentTask = "Initializing"
        Dim options As ThermoReaderOptions = mRawFileReader.Options

        Try
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
    Public Iterator Function GetLabelData(Optional skipEmptyScan As Boolean = True) As IEnumerable(Of RawLabelData)
        Dim options As ThermoReaderOptions = InitReader()
        Dim rt As Double = Nothing

        For i As Integer = options.MinScan To options.MaxScan
            Dim scanInfo As SingleScanInfo = Nothing
            Dim data As FTLabelInfoType() = GetScanData(i, scanInfo)

            ' XRawFileIO.udtMassPrecisionInfoType[] precisionInfo;
            ' mRawFileReader.GetScanPrecisionData(i, out precisionInfo);
            ' Console.WriteLine("PrecisionInfoCount: " + precisionInfo.Length);

            If data Is Nothing Then
                If skipEmptyScan Then
                    Continue For
                Else
                    data = {}
                End If
            ElseIf data.Length = 0 Then
                If skipEmptyScan Then
                    Continue For
                End If
            End If

            Dim maxInt As Double = data.Max(Function(x) x.Intensity)

            ' Check for the maximum intensity being zero
            If skipEmptyScan AndAlso (stdNum.Abs(maxInt) < Single.Epsilon) Then
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

            If skipEmptyScan AndAlso dataFiltered.Count = 0 Then
                Continue For
            Else
                mRawFileReader.GetRetentionTime(i, rt)
            End If

            Yield New RawLabelData With {
                .ScanNumber = i,
                .ScanTime = rt,
                .MSData = dataFiltered,
                .MaxIntensity = maxInt,
                .MsLevel = scanInfo.MSLevel
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
