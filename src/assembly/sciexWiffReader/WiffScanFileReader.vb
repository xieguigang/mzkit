Imports System.Runtime.CompilerServices
Imports Clearcore2.Data
Imports Clearcore2.Data.AnalystDataProvider
Imports Clearcore2.Data.DataAccess.SampleData
Imports Clearcore2.Data.DataAccess.SampleData.MSExperimentInfo
Imports Clearcore2.RawXYProcessing

Public Class WiffScanFileReader : Implements IDisposable

    ReadOnly wiffDataProvider As AnalystWiffDataProvider
    ReadOnly wiffFile As Batch

    Public ReadOnly Property wiffPath As String
    Public ReadOnly Property sampleNames As String()

    Dim disposedValue As Boolean
    Dim wiffSample As Sample
    Dim msSample As MassSpectrometerSample
    Dim wiffExperiments As New List(Of MSExperiment)()

    Public ScanInfos As New List(Of ScanInfo)()
    Private minRT As Single
    Private maxRT As Single

    Public Function GetDescriptionForScanNum(scannumber As Integer) As String
        Return Me.ScanInfos(scannumber).DataTitle
    End Function

    Public Function GetInstrumentName() As String
        Return Me.wiffSample.Details.InstrumentName
    End Function

    Sub New(wiffpath As String)
        Me.wiffDataProvider = New AnalystWiffDataProvider()
        Me.wiffPath = wiffpath
        Me.wiffFile = AnalystDataProviderFactory.CreateBatch(wiffpath, Me.wiffDataProvider)

        If Me.wiffFile Is Nothing Then
            Throw New Exception($"Can not open wiff file '{wiffpath}'!")
        End If

        Me.sampleNames = Me.wiffFile.GetSampleNames()

        If Me.sampleNames.Length < 1 Then
            Throw New Exception($"No samples found in wiff file '{wiffpath}'!")
        End If

        Call Me.SetCurrentSample(0)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetLastSpectrumNumber() As Integer
        Return Me.ScanInfos.Count - 1
    End Function

    Public Function GetScanNumFromRetentionTime(RT As Double) As Integer
        Dim i As Integer = Scan0
        Dim num As Integer = GetLastSpectrumNumber()
        Dim num2 As Integer = (i + num) / 2
        While i < num
            If CDbl(Me.ScanInfos(num2).RetentionTime) = RT Then
                Return Me.ScanInfos(num2).ScanNumber
            End If
            If CDbl(Me.ScanInfos(num2).RetentionTime) > RT Then
                num = num2 - 1
            Else
                i = num2 + 1
            End If
            num2 = (i + num) / 2
        End While
        Dim index As Integer = num2
        Dim num3 As Double = Global.System.Math.Abs(CDbl(Me.ScanInfos(index).RetentionTime) - RT)
        If num2 > 0 AndAlso Global.System.Math.Abs(CDbl(Me.ScanInfos(num2 - 1).RetentionTime) - RT) < num3 Then
            index = num2 - 1
            num3 = Global.System.Math.Abs(CDbl(Me.ScanInfos(index).RetentionTime) - RT)
        End If
        If num2 < Me.GetLastSpectrumNumber() - 1 AndAlso Global.System.Math.Abs(CDbl(Me.ScanInfos(num2 + 1).RetentionTime) - RT) < num3 Then
            index = num2 + 1
        End If
        Return Me.ScanInfos(index).ScanNumber
    End Function

    Public Function GetProfileFromScanNum(scannumber As Integer) As PeakList
        Dim scanInfo As ScanInfo = Me.ScanInfos(scannumber)
        Dim massSpectrum As Global.Clearcore2.Data.MassSpectrum
        Try
            massSpectrum = Me.wiffExperiments(scanInfo.ExperimentId).GetMassSpectrum(scanInfo.CycleId)
        Catch ex As Global.System.Exception
            Return New PeakList(New Double(-1) {}, New Double(-1) {})
        End Try
        Dim array As Double() = New Double(massSpectrum.NumDataPoints - 1) {}
        Dim array2 As Double() = New Double(massSpectrum.NumDataPoints - 1) {}
        For i As Integer = 0 To massSpectrum.NumDataPoints - 1
            array(i) = massSpectrum.GetXValue(i)
            array2(i) = massSpectrum.GetYValue(i)
        Next
        Return New PeakList(array, array2)
    End Function

    Public Function GetCentroidFromScanNum(scannumber As Integer) As PeakList
        Dim scanInfo As ScanInfo = Me.ScanInfos(scannumber)
        Dim peakArray As PeakClass()
        Try
            peakArray = Me.wiffExperiments(scanInfo.ExperimentId).GetPeakArray(scanInfo.CycleId)
        Catch ex As Global.System.Exception
            Return New PeakList(New Double(-1) {}, New Double(-1) {})
        End Try
        Dim array As Double() = New Double(peakArray.Length - 1) {}
        Dim array2 As Double() = New Double(peakArray.Length - 1) {}
        Dim num As Integer = -1
        For i As Integer = 0 To peakArray.Length - 1
            array(i) = peakArray(i).apexX
            array2(i) = CDbl(CSng(peakArray(i).apexY))
            If Double.IsNaN(array(i)) OrElse Double.IsInfinity(array(i)) Then
                num = i
                Exit For
            End If
            If Double.IsNaN(array2(i)) OrElse Double.IsInfinity(array2(i)) Then
                num = i
                Exit For
            End If
        Next
        If CSng(num) >= 0F Then
            array = array.SubArray(num)
            array2 = array2.SubArray(num)
        End If
        Return New PeakList(array, array2)
    End Function

    Public Sub SetCurrentSample(sampleId As Integer)
        If sampleId < 0 OrElse sampleId >= Me.sampleNames.Length Then
            Throw New Exception("Incorrect sample number.")
        Else
            Me.wiffSample = Me.wiffFile.GetSample(sampleId)
            Me.msSample = Me.wiffSample.MassSpectrometerSample
            Me.ScanInfos.Clear()

            Call InternalSetCurrentSample(sampleId)
        End If
    End Sub

    Private Sub InternalSetCurrentSample(sampleId As Integer)
        Dim experimentCount As Integer = Me.msSample.ExperimentCount
        Dim array As TotalIonChromatogram() = New TotalIonChromatogram(experimentCount - 1) {}
        For i As Integer = 0 To experimentCount - 1
            Dim msexperiment As MSExperiment = Me.msSample.GetMSExperiment(i)
            Me.wiffExperiments.Add(msexperiment)
            array(i) = msexperiment.GetTotalIonChromatogram()
        Next
        Dim numDataPoints As Integer = array(0).NumDataPoints
        For j As Integer = 0 To numDataPoints - 1
            For k As Integer = 0 To Me.msSample.ExperimentCount - 1
                Dim totalIonChromatogram As TotalIonChromatogram = array(k)
                Dim num As Single = CSng(totalIonChromatogram.GetYValue(j))
                If k = 0 OrElse num > 0F Then
                    Dim msexperiment2 As MSExperiment = Me.wiffExperiments(k)
                    Dim details As MSExperimentInfo = msexperiment2.Details
                    Dim massSpectrum As MassSpectrum = msexperiment2.GetMassSpectrum(j)
                    Dim massSpectrumInfo As MassSpectrumInfo = msexperiment2.GetMassSpectrumInfo(j)
                    Dim isolationCenter As Double = 0.0
                    Dim isolationWidth As Double = -1.0
                    If details.MassRangeInfo.Length <> 0 AndAlso massSpectrumInfo.MSLevel > 1 Then
                        isolationCenter = CType(details.MassRangeInfo(0), FragmentBasedScanMassRange).FixedMasses(0)
                        isolationWidth = CType(details.MassRangeInfo(0), FragmentBasedScanMassRange).IsolationWindow
                    End If
                    Dim actualXValues As Double() = massSpectrum.GetActualXValues()
                    Dim actualYValues As Double() = massSpectrum.GetActualYValues()
                    Dim num2 As Integer = -1
                    If actualXValues.Length <> 0 Then
                        num2 = 0
                        For l As Integer = 1 To actualYValues.Length - 1
                            If actualYValues(num2) < actualYValues(l) Then
                                num2 = l
                            End If
                        Next
                    End If
                    Dim scanInfo As New ScanInfo() With {
                        .CycleId = j,
                        .ScanNumber = Me.ScanInfos.Count,
                        .ExperimentId = k,
                        .RetentionTime = CSng(totalIonChromatogram.GetXValue(j)),
                        .TotalIonCurrent = num,
                        .ScanMode = If(massSpectrumInfo.CentroidMode, ScanMode.Centroid, ScanMode.Profile),
                        .DataTitle = massSpectrumInfo.GetDescription(True),
                        .PeaksCount = massSpectrum.NumDataPoints,
                        .BasePeakMz = If((num2 = -1), 0.0, actualXValues(num2)),
                        .BasePeakIntensity = CSng(If((num2 = -1), 0.0, actualYValues(num2))),
                        .Polarity = If((details.Polarity = PolarityEnum.Positive), Polarity.Positive, Polarity.Negative),
                        .LowMz = details.StartMass,
                        .HighMz = details.EndMass,
                        .MSLevel = massSpectrumInfo.MSLevel,
                        .ScanType = If((details.IDAType = IDAExperimentType.Survey), ScanType.MS1, ScanType.MS2),
                        .FragmentationType = "NULL",
                        .IsolationWidth = isolationWidth,
                        .IsolationCenter = isolationCenter
                    }

                    If scanInfo.MSLevel > 1 Then
                        scanInfo.FragmentationType = "QTOF-CID"
                        scanInfo.PrecursorMz = massSpectrumInfo.ParentMZ
                        scanInfo.PrecursorCharge = massSpectrumInfo.ParentChargeState
                        scanInfo.PrecursorIntensity = -1.0F
                        scanInfo.CollisionEnergy = CSng(massSpectrumInfo.CollisionEnergy)
                        scanInfo.ScanMode = ScanMode.Centroid
                        scanInfo.LowMz = massSpectrumInfo.ParentMZ - 1.0
                        scanInfo.HighMz = massSpectrumInfo.ParentMZ + 1.0
                    End If
                    Me.ScanInfos.Add(scanInfo)
                End If
            Next
        Next
        Me.minRT = Me.ScanInfos(0).RetentionTime
        Me.maxRT = Me.ScanInfos(Me.ScanInfos.Count - 1).RetentionTime
    End Sub

    Public Sub Close()
        Me.wiffDataProvider.Close()
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call Close()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
