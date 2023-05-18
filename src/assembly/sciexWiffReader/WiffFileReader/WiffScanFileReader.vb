#Region "Microsoft.VisualBasic::00a565701aff4bf5aa26f45ff28924bb, mzkit\src\assembly\sciexWiffReader\WiffFileReader\WiffScanFileReader.vb"

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

    '   Total Lines: 295
    '    Code Lines: 231
    ' Comment Lines: 10
    '   Blank Lines: 54
    '     File Size: 11.62 KB


    ' Class WiffScanFileReader
    ' 
    '     Properties: GetScan, InstrumentName, maxRT, minRT, sampleNames
    '                 wiffPath
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetCentroidFromScanNum, GetLastSpectrumNumber, GetProfileFromScanNum, GetScanNumFromRetentionTime, ToString
    ' 
    '     Sub: Close, (+2 Overloads) Dispose, InternalSetCurrentSample, loadScan, SetCurrentSample
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Clearcore2.Data
Imports Clearcore2.Data.AnalystDataProvider
Imports Clearcore2.Data.DataAccess.SampleData
Imports Clearcore2.Data.DataAccess.SampleData.MSExperimentInfo
Imports Clearcore2.RawXYProcessing
Imports stdNum = System.Math

Public Class WiffScanFileReader : Implements IDisposable

    ReadOnly wiffDataProvider As AnalystWiffDataProvider
    ReadOnly wiffFile As Batch
    ReadOnly scanList As New List(Of ScanInfo)()

    Public ReadOnly Property wiffPath As String
    Public ReadOnly Property sampleNames As String()

    Dim disposedValue As Boolean
    Dim wiffSample As Sample
    Dim msSample As MassSpectrometerSample
    Dim wiffExperiments As New List(Of MSExperiment)()
    Dim type_cache As ExperimentType
    Dim current_experiment As MSExperiment

    Public ReadOnly Property minRT As Single
    Public ReadOnly Property maxRT As Single

    Public ReadOnly Property GetScan(scannumber As Integer) As ScanInfo
        Get
            Return scanList(scannumber)
        End Get
    End Property

    Public ReadOnly Property InstrumentName() As String
        Get
            Return Me.wiffSample.Details.InstrumentName
        End Get
    End Property

    Public ReadOnly Property experimentType As ExperimentType
        Get
            Return current_experiment.Details.ExperimentType
        End Get
    End Property

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

        Call SetCurrentSample(Scan0)
    End Sub

    Public Overrides Function ToString() As String
        Return $"{wiffPath.FileName} scan_time=[{minRT},{maxRT}]@{scanList.Count} scans ({InstrumentName})"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetLastSpectrumNumber() As Integer
        Return Me.scanList.Count - 1
    End Function

    Public Function GetScanNumFromRetentionTime(RT As Double) As Integer
        Dim i As Integer = Scan0
        Dim num As Integer = GetLastSpectrumNumber()
        Dim num2 As Integer = (i + num) / 2

        While i < num
            If CDbl(Me.scanList(num2).RetentionTime) = RT Then
                Return Me.scanList(num2).ScanNumber
            End If
            If CDbl(Me.scanList(num2).RetentionTime) > RT Then
                num = num2 - 1
            Else
                i = num2 + 1
            End If
            num2 = (i + num) / 2
        End While

        Dim index As Integer = num2
        Dim num3 As Double = stdNum.Abs(CDbl(Me.scanList(index).RetentionTime) - RT)

        If num2 > 0 AndAlso stdNum.Abs(CDbl(Me.scanList(num2 - 1).RetentionTime) - RT) < num3 Then
            index = num2 - 1
            num3 = stdNum.Abs(CDbl(Me.scanList(index).RetentionTime) - RT)
        End If

        If num2 < Me.GetLastSpectrumNumber() - 1 AndAlso stdNum.Abs(CDbl(Me.scanList(num2 + 1).RetentionTime) - RT) < num3 Then
            index = num2 + 1
        End If

        Return Me.scanList(index).ScanNumber
    End Function

    Public Function GetProfileFromScanNum(scannumber As Integer) As PeakList
        Dim scanInfo As ScanInfo = Me.scanList(scannumber)
        Dim massSpectrum As MassSpectrum

        Try
            massSpectrum = Me.wiffExperiments(scanInfo.ExperimentId).GetMassSpectrum(scanInfo.CycleId)
        Catch ex As Exception
            Return New PeakList(New Double(-1) {}, New Double(-1) {})
        End Try

        Dim array As Double() = New Double(massSpectrum.NumDataPoints - 1) {}
        Dim array2 As Double() = New Double(massSpectrum.NumDataPoints - 1) {}

        For i As Integer = 0 To massSpectrum.NumDataPoints - 1
            array(i) = massSpectrum.GetXValue(i)
            array2(i) = massSpectrum.GetYValue(i)
        Next

        If type_cache = ExperimentType.MRM Then
            Dim info = current_experiment.Details
            Dim ions = info.MassRangeInfo _
                .Select(Function(a)
                            Dim i = DirectCast(a, Clearcore2.Data.DataAccess.SampleData.MRMMassRange)
                            Return New MRM(i.Q1Mass, i.Q3Mass)
                        End Function) _
                .ToArray

            Return New PeakList(array, array2) With {.MRM = ions}
        Else
            Return New PeakList(array, array2)
        End If
    End Function

    Public Function GetCentroidFromScanNum(scannumber As Integer) As PeakList
        Dim scanInfo As ScanInfo = Me.scanList(scannumber)
        Dim peakArray As PeakClass()

        Try
            peakArray = Me.wiffExperiments(scanInfo.ExperimentId).GetPeakArray(scanInfo.CycleId)
        Catch ex As Exception
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
            Me.scanList.Clear()

            Call InternalSetCurrentSample(_sampleNames(sampleId))

            ' the wiffExperiments will has value after the setcurrentsample 
            ' method has been called
            ' so needs set type cache after the method call of setcurrentsample
            type_cache = experimentType
        End If
    End Sub

    Private Sub InternalSetCurrentSample(sampleName As String)
        Dim experimentCount As Integer = Me.msSample.ExperimentCount
        Dim array As TotalIonChromatogram() = New TotalIonChromatogram(experimentCount - 1) {}

        For i As Integer = 0 To experimentCount - 1
            Dim msexperiment As MSExperiment = Me.msSample.GetMSExperiment(i)

            wiffExperiments.Add(msexperiment)
            array(i) = msexperiment.GetTotalIonChromatogram()
        Next

        Dim numDataPoints As Integer = array(0).NumDataPoints

        For j As Integer = 0 To numDataPoints - 1
            For k As Integer = 0 To Me.msSample.ExperimentCount - 1
                Call loadScan(array, sampleName, k, j)
            Next
        Next

        current_experiment = wiffExperiments.Last

        _minRT = Me.scanList(0).RetentionTime
        _maxRT = Me.scanList(Me.scanList.Count - 1).RetentionTime
    End Sub

    Private Sub loadScan(array As TotalIonChromatogram(), sampleName As String, k As Integer, j As Integer)
        Dim totalIonChromatogram As TotalIonChromatogram = array(k)
        Dim num As Single = CSng(totalIonChromatogram.GetYValue(j))

        If Not (k = 0 OrElse num > 0F) Then
            Return
        End If

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

        Dim centroidModel As ScanMode = If(massSpectrumInfo.CentroidMode, ScanMode.Centroid, ScanMode.Profile)
        Dim polarity As Polarity = If((details.Polarity = PolarityEnum.Positive), Polarity.Positive, Polarity.Negative)
        Dim msLevel As ScanType = If((details.IDAType = IDAExperimentType.Survey), ScanType.MS1, ScanType.MS2)
        Dim rt = CSng(totalIonChromatogram.GetXValue(j))
        Dim scanTitle As String = massSpectrumInfo.GetDescription(True)
        Dim basePeakMz = If((num2 = -1), 0.0, actualXValues(num2))
        Dim intensity = CSng(If((num2 = -1), 0.0, actualYValues(num2)))
        Dim scanInfo As New ScanInfo() With {
            .CycleId = j,
            .ScanNumber = Me.scanList.Count,
            .ExperimentId = k,
            .RetentionTime = rt,
            .TotalIonCurrent = num,
            .ScanMode = centroidModel,
            .DataTitle = scanTitle,
            .PeaksCount = massSpectrum.NumDataPoints,
            .BasePeakMz = basePeakMz,
            .BasePeakIntensity = intensity,
            .Polarity = polarity,
            .LowMz = If(details.ExperimentType <> DataAccess.SampleData.ExperimentType.MRM, details.StartMass, 0),
            .HighMz = If(details.ExperimentType <> DataAccess.SampleData.ExperimentType.MRM, details.EndMass, 0),
            .MSLevel = massSpectrumInfo.MSLevel,
            .ScanType = msLevel,
            .FragmentationType = "NULL",
            .IsolationWidth = isolationWidth,
            .IsolationCenter = isolationCenter,
            .SampleName = sampleName
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

        Call Me.scanList.Add(scanInfo)
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
