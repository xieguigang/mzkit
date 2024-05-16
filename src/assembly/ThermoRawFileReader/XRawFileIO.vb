#Region "Microsoft.VisualBasic::2ace10cac463f0e1e197b1d2b2948a6d, assembly\ThermoRawFileReader\XRawFileIO.vb"

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

    '   Total Lines: 2531
    '    Code Lines: 1499
    ' Comment Lines: 591
    '   Blank Lines: 441
    '     File Size: 109.28 KB


    ' Class XRawFileIO
    ' 
    '     Properties: FileInfo, Options, RawFilePath, ScanEnd, ScanInfoCacheMaxSize
    '                 ScanStart, TraceMode
    ' 
    '     Constructor: (+4 Overloads) Sub New
    ' 
    '     Function: CapitalizeCollisionMode, ContainsAny, ContainsText, DetermineIonizationMode, DetermineMRMScanType
    '               ExtractMSLevel, (+3 Overloads) ExtractParentIonMZFromFilterText, FillFileInfo, GetActivationType, GetCapturedValue
    '               GetChromatogramData, GetChromatogramData2D, GetCollisionEnergy, GetCollisionEnergyUnnormalized, GetDeviceInfo
    '               GetDeviceStats, getFtLabelData, GetMSLevel, GetNumScans, GetRetentionTime
    '               (+3 Overloads) GetScanData, (+3 Overloads) GetScanData2D, GetScanDataSumScans, GetScanInfo, GetScanInfoInternal
    '               GetScanLabelData, GetScanPrecisionData, GetScanTypeNameFromThermoScanFilterText, LoadFileInfoInternal, MakeGenericThermoScanFilter
    '               MakeGenericThermoScanFilterInternal, OpenRawFile, ReadScanData, ScanIsFTMS, SetMSController
    '               ToString, TuneMethodsMatch, ValidateAndSelectDevice, ValidateMSScan
    ' 
    '     Sub: CacheScanInfo, CloseRawFile, Dispose, ExtractMRMMasses, GetTuneData
    '          LoadMethodInfo, Options_OptionsUpdatedEvent, RaiseErrorMessage, RaiseWarningMessage, RemoveCachedScanInfoOverLimit
    '          UpdateReaderOptions
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Globalization
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports ThermoFisher.CommonCore.BackgroundSubtraction
Imports ThermoFisher.CommonCore.Data
Imports ThermoFisher.CommonCore.Data.Business
Imports ThermoFisher.CommonCore.Data.FilterEnums
Imports ThermoFisher.CommonCore.Data.Interfaces
Imports ThermoFisher.CommonCore.MassPrecisionEstimator
Imports ThermoFisher.CommonCore.RawFileReader
Imports stdNum = System.Math

' The methods in this class use ThermoFisher.CommonCore.RawFileReader.dll
' and related DLLs to extract scan header info and mass spec data (m/z and intensity lists)
' from Thermo .Raw files (LTQ, LTQ-FT, Orbitrap, Exactive, TSQ, etc.)
'
' For more information about the ThermoFisher.CommonCore DLLs,
' see the RawFileReaderLicense.doc file in the lib directory;
' see also http://planetorbitrap.com/rawfilereader#.W5BAoOhKjdM
' For questions, contact Jim Shofstahl at ThermoFisher.com

' -------------------------------------------------------------------------------
' Written by Matthew Monroe and Bryson Gibbons for the Department of Energy (PNNL, Richland, WA)
' Originally used XRawfile2.dll (in November 2004)
' Switched to MSFileReader.XRawfile2.dll in March 2012
' Switched to ThermoFisher.CommonCore DLLs in 2018
'
' E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov
' Website: https://omics.pnl.gov/ or https://www.pnnl.gov/sysbio/ or https://panomics.pnnl.gov/
' -------------------------------------------------------------------------------
'
' Licensed under the 2-Clause BSD License; you may not use this file except
' in compliance with the License.  You may obtain a copy of the License at
' https://opensource.org/licenses/BSD-2-Clause
'
' Copyright 2018 Battelle Memorial Institute


''' <summary>
''' Class for reading Thermo .raw files
''' </summary>
''' <remarks>
''' https://github.com/PNNL-Comp-Mass-Spec
''' </remarks>
<CLSCompliant(True)>
Public Class XRawFileIO : Implements IDisposable

    ''' <summary>
    ''' The full path to the currently loaded .raw file
    ''' </summary>
    ''' <remarks>This is changed to an empty string once the file is closed</remarks>
    Private _RawFilePath As String

#Region "Class wide Variables"

    ''' <summary>
    ''' Maximum size of the scan info cache
    ''' </summary>
    Private mMaxScansToCacheInfo As Integer = 50000

    ''' <summary>
    ''' The scan info cache
    ''' </summary>
    Private ReadOnly mCachedScanInfo As New Dictionary(Of Integer, SingleScanInfo)
    ''' <summary>
    ''' This linked list tracks the scan numbers stored in mCachedScanInfo,
    ''' allowing for quickly determining the oldest scan added to the cache when the cache limit is reached
    ''' </summary>
    Private ReadOnly mCachedScans As New LinkedList(Of Integer)

    ''' <summary>
    ''' Reader that implements ThermoFisher.CommonCore.Data.Interfaces.IRawDataPlus
    ''' </summary>
    Private mXRawFile As IRawDataPlus

    ''' <summary>
    ''' Cached file header
    ''' </summary>
    Private mXRawFileHeader As IFileHeader

    ''' <summary>
    ''' This is set to true if an exception is raised with the message "memory is corrupt"
    ''' It is also set to true if the .raw file does not have any MS data
    ''' </summary>
    Private mCorruptMemoryEncountered As Boolean
#End Region

#Region "Properties"

    ''' <summary>
    ''' File info for the currently loaded .raw file
    ''' </summary>
    Public ReadOnly Property FileInfo As RawFileInfo = New RawFileInfo()

    ''' <summary>
    ''' Thermo reader options
    ''' </summary>
    Public ReadOnly Property Options As ThermoReaderOptions

    Public Property RawFilePath As String
        Get
            Return _RawFilePath
        End Get
        Private Set(value As String)
            _RawFilePath = value
        End Set
    End Property

    ''' <summary>
    ''' Maximum number of scan metadata cached; defaults to 50000
    ''' </summary>
    ''' <remarks>Set to 0 to disable caching</remarks>
    Public Property ScanInfoCacheMaxSize As Integer
        Get
            Return mMaxScansToCacheInfo
        End Get
        Set(value As Integer)
            mMaxScansToCacheInfo = value

            If mMaxScansToCacheInfo <= 0 Then
                mMaxScansToCacheInfo = 0
            End If

            If Me.mCachedScanInfo.Count = 0 Then Return

            If mMaxScansToCacheInfo = 0 Then
                Me.mCachedScanInfo.Clear()
                Me.mCachedScans.Clear()
            Else
                RemoveCachedScanInfoOverLimit(mMaxScansToCacheInfo)
            End If
        End Set
    End Property

    ''' <summary>
    ''' First scan number in the .Raw file
    ''' </summary>
    Public ReadOnly Property ScanStart As Integer
        Get
            Return FileInfo.ScanStart
        End Get
    End Property

    ''' <summary>
    ''' Last scan number in the .Raw file
    ''' </summary>
    Public ReadOnly Property ScanEnd As Integer
        Get
            Return FileInfo.ScanEnd
        End Get
    End Property

    ''' <summary>
    ''' When true, additional messages are reported via Debug events
    ''' </summary>
    Public Property TraceMode As Boolean

#End Region

#Region "Events and Event Handlers"

    ''' <summary>
    ''' Report an error message to the error event handler
    ''' </summary>
    ''' <param name="message"></param>
    ''' <param name="ex">Optional exception</param>
    Friend Sub RaiseErrorMessage(message As String, Optional ex As Exception = Nothing)
        Call App.LogException(message)
        Call App.LogException(ex)
    End Sub

    ''' <summary>
    ''' Report a warning message to the warning event handler
    ''' </summary>
    ''' <param name="message"></param>
    Friend Sub RaiseWarningMessage(message As String)
        Call message.Warning
    End Sub

    Private Sub Options_OptionsUpdatedEvent(sender As Object)
        UpdateReaderOptions()
    End Sub

#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    Public Sub New()
        Me.New(String.Empty)
    End Sub

    ''' <summary>
    ''' Constructor with an options parameter
    ''' </summary>
    ''' <param name="options">Thermo reader options</param>
    Public Sub New(options As ThermoReaderOptions)
        Me.New(String.Empty, options)
    End Sub

    ''' <summary>
    ''' Constructor with a file path parameter
    ''' </summary>
    ''' <param name="rawFilePath">Thermo .raw file to open (empty string to not open a file)</param>
    ''' <param name="traceMode">When true, additional messages are reported via Debug events</param>
    Public Sub New(rawFilePath As String, Optional traceMode As Boolean = False)
        Me.New(rawFilePath, New ThermoReaderOptions(), traceMode)
    End Sub

    ''' <summary>
    ''' Constructor with file path, options, and optionally a trace flag
    ''' </summary>
    ''' <param name="rawFilePath">Thermo .raw file to open (empty string to not open a file)</param>
    ''' <param name="options">Thermo reader options</param>
    ''' <param name="traceMode">When true, additional messages are reported via Debug events</param>
    Public Sub New(rawFilePath As String, options As ThermoReaderOptions, Optional traceMode As Boolean = False)
        Me.RawFilePath = String.Empty
        Me.TraceMode = traceMode
        Me.Options = options
        AddHandler Me.Options.OptionsUpdatedEvent, AddressOf Options_OptionsUpdatedEvent

        If Not String.IsNullOrWhiteSpace(rawFilePath) Then
            OpenRawFile(rawFilePath)
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return RawFilePath
    End Function

    Private Sub CacheScanInfo(scan As Integer, scanInfo As SingleScanInfo)
        If ScanInfoCacheMaxSize = 0 Then
            Return
        End If

        If Me.mCachedScanInfo.ContainsKey(scan) Then
            ' Updating an existing item
            Me.mCachedScanInfo.Remove(scan)
            Me.mCachedScans.Remove(scan)
        End If

        RemoveCachedScanInfoOverLimit(mMaxScansToCacheInfo - 1)
        Me.mCachedScanInfo.Add(scan, scanInfo)
        Me.mCachedScans.AddLast(scan)
    End Sub

    Private Sub RemoveCachedScanInfoOverLimit(limit As Integer)
        If Me.mCachedScanInfo.Count <= limit Then Return

        ' Remove the oldest entry/entries in mCachedScanInfo
        While Me.mCachedScanInfo.Count > limit
            Dim scan As Integer = Me.mCachedScans.First().Value
            Me.mCachedScans.RemoveFirst()

            If Me.mCachedScanInfo.ContainsKey(scan) Then
                Me.mCachedScanInfo.Remove(scan)
            End If
        End While
    End Sub

    Private Shared Function CapitalizeCollisionMode(collisionMode As String) As String
        If String.Equals(collisionMode, "EThcD", StringComparison.OrdinalIgnoreCase) Then
            Return "EThcD"
        End If

        If String.Equals(collisionMode, "ETciD", StringComparison.OrdinalIgnoreCase) Then
            Return "ETciD"
        End If

        Return collisionMode.ToUpper()
    End Function

    ''' <summary>
    ''' Close the .raw file
    ''' </summary>
    Public Sub CloseRawFile()
        Try
            mXRawFile?.Dispose()
            mCorruptMemoryEncountered = False
            ' Ignore this error
        Catch __unusedAccessViolationException1__ As AccessViolationException
            ' Ignore any errors
        Catch __unusedException2__ As Exception
        Finally
            mXRawFile = Nothing
            RawFilePath = String.Empty
            FileInfo.Clear()
        End Try
    End Sub

    Private Shared Function ContainsAny(stringToSearch As String, itemsToFind As IEnumerable(Of String), Optional indexSearchStart As Integer = 0) As Boolean
        Return itemsToFind.Any(Function(item) ContainsText(stringToSearch, item, indexSearchStart))
    End Function

    Private Shared Function ContainsText(stringToSearch As String, textToFind As String, Optional indexSearchStart As Integer = 0) As Boolean
        ' Note: need to append a space since many of the search keywords end in a space
        Return (stringToSearch & " ").IndexOf(textToFind, StringComparison.OrdinalIgnoreCase) >= indexSearchStart
    End Function

    ''' <summary>
    ''' Determines the MRM scan type by parsing the scan filter string
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <returns>MRM scan type enum</returns>
    Public Shared Function DetermineMRMScanType(filterText As String) As MRMScanTypeConstants
        If String.IsNullOrWhiteSpace(filterText) Then
            Return MRMScanTypeConstants.NotMRM
        End If

        Dim mrmQMSTags = New List(Of String) From {
            MRM_Q1MS_TEXT,
            MRM_Q3MS_TEXT
        }

        If ContainsAny(filterText, mrmQMSTags, 1) Then
            Return MRMScanTypeConstants.MRMQMS
        End If

        If ContainsText(filterText, MRM_SRM_TEXT, 1) Then
            Return MRMScanTypeConstants.SRM
        End If

        If ContainsText(filterText, MRM_SIM_PR_TEXT, 1) Then
            ' This is not technically SRM, but the data looks very similar, so we'll track it like SRM data
            Return MRMScanTypeConstants.SRM
        End If

        If ContainsText(filterText, MRM_SIM_MSX_TEXT, 1) Then
            Return MRMScanTypeConstants.SIM
        End If

        If ContainsText(filterText, MRM_FullNL_TEXT, 1) Then
            Return MRMScanTypeConstants.FullNL
        End If

        If ContainsText(filterText, SIM_MS_TEXT, 1) Then
            Return MRMScanTypeConstants.SIM
        End If

        Return MRMScanTypeConstants.NotMRM
    End Function

    ''' <summary>
    ''' Determine the Ionization mode by parsing the scan filter string
    ''' </summary>
    ''' <param name="filterText"></param>
    Public Shared Function DetermineIonizationMode(filterText As String) As IonModes
        ' Determine the ion mode by simply looking for the first + or - sign

        If String.IsNullOrWhiteSpace(filterText) Then
            Return IonModes.Unknown
        End If

        ' For safety, remove any text after a square bracket
        Dim charIndex = filterText.IndexOf("["c)
        Dim match As Match

        If charIndex > 0 Then
            match = LabelParser.mIonMode.Match(filterText.Substring(0, charIndex))
        Else
            match = LabelParser.mIonMode.Match(filterText)
        End If

        If match.Success Then
            Select Case Provider.ParseIonMode(match.Value, allowsUnknown:=True)
                Case 1 : Return IonModes.Positive
                Case -1 : Return IonModes.Negative
                Case Else
                    Return IonModes.Unknown
            End Select
        End If

        Return IonModes.Unknown
    End Function

    ''' <summary>
    ''' Parse out the MRM_QMS or SRM mass info from filterText
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <param name="mrmScanType"></param>
    ''' <param name="mrmInfo">Output: MRM info class</param>
    ''' <remarks>We do not parse mass information out for Full Neutral Loss scans</remarks>
    Public Shared Sub ExtractMRMMasses(filterText As String, mrmScanType As MRMScanTypeConstants, <Out> ByRef mrmInfo As MRMInfo)
        ' Parse out the MRM_QMS or SRM mass info from filterText
        ' It should be of the form

        ' SIM:              p NSI SIM ms [330.00-380.00]
        '                   p NSI SIM msx ms [475.0000-525.0000]
        ' or
        ' MRM_Q1MS_TEXT:    p NSI Q1MS [179.652-184.582, 505.778-510.708, 994.968-999.898]
        ' or
        ' MRM_Q3MS_TEXT:    p NSI Q3MS [150.070-1500.000]
        ' or
        ' MRM_SRM_TEXT:     c NSI SRM ms2 489.270@cid17.00 [397.209-392.211, 579.289-579.291]

        ' Note: we do not parse mass information out for Full Neutral Loss scans
        ' MRM_FullNL_TEXT: c NSI Full cnl 162.053 [300.000-1200.000]

        mrmInfo = New MRMInfo()

        If String.IsNullOrWhiteSpace(filterText) Then
            Return
        End If

        If Not (mrmScanType = MRMScanTypeConstants.SIM OrElse mrmScanType = MRMScanTypeConstants.MRMQMS OrElse mrmScanType = MRMScanTypeConstants.SRM) Then
            ' Unsupported MRM type
            Return
        End If

        ' Parse out the text between the square brackets
        Dim massListMatch = LabelParser.mMassList.Match(filterText)

        If Not massListMatch.Success Then
            Return
        End If

        Dim massRangeMatch = LabelParser.mMassRanges.Match(massListMatch.Value)

        While massRangeMatch.Success

            Try
                ' Note that group 0 is the full mass range (two mass values, separated by a dash)
                ' Group 1 is the first mass value
                ' Group 2 is the second mass value

                Dim mrmMassRange = New MRMMassRangeType With {
                    .StartMass = Double.Parse(massRangeMatch.Groups(CStr("StartMass")).Value),
                    .EndMass = Double.Parse(massRangeMatch.Groups(CStr("EndMass")).Value)
                }
                Dim centralMass = mrmMassRange.StartMass + (mrmMassRange.EndMass - mrmMassRange.StartMass) / 2
                mrmMassRange.CentralMass = stdNum.Round(centralMass, 6)
                mrmInfo.MRMMassList.Add(mrmMassRange)
            Catch __unusedException1__ As Exception
                ' Error parsing out the mass values; skip this group
            End Try

            massRangeMatch = massRangeMatch.NextMatch()
        End While
    End Sub

    ''' <summary>
    ''' Parse out the parent ion from filterText
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <param name="parentIonMz">Parent ion m/z (output)</param>
    ''' <returns>True if success</returns>
    ''' <remarks>
    ''' If multiple parent ion m/z values are listed then parentIonMz will have the last one.  
    ''' However, if the filter text contains "Full msx" then parentIonMz will have the first parent ion listed
    ''' 
    ''' This was created for use in other programs that only need the parent ion m/z, and no other functions from ThermoRawFileReader.
    ''' Other projects that use this:
    '''      PHRPReader (https://github.com/PNNL-Comp-Mass-Spec/PHRP)
    '''      
    ''' To copy this, take the code from this function, plus the RegEx strings <see cref="PARENT_ION_ONLY_NON_MSX_REGEX"/> and <see cref="PARENT_ION_ONLY_MSX_REGEX"/>,
    ''' with their uses in <see cref="mFindParentIonOnlyNonMsx"/> and <see cref="mFindParentIonOnlyMsx"/>
    ''' </remarks>
    Public Shared Function ExtractParentIonMZFromFilterText(filterText As String, <Out> ByRef parentIonMz As Double) As Boolean
        Dim matcher As Regex

        If filterText.IndexOf("msx", StringComparison.OrdinalIgnoreCase) >= 0 Then
            matcher = LabelParser.mFindParentIonOnlyMsx
        Else
            matcher = LabelParser.mFindParentIonOnlyNonMsx
        End If

        Dim match = matcher.Match(filterText)

        If match.Success Then
            Dim parentIonMzText = match.Groups("ParentMZ").Value
            Dim success = Double.TryParse(parentIonMzText, parentIonMz)
            Return success
        End If

        parentIonMz = 0
        Return False
    End Function

    ''' <summary>
    ''' Parse out the parent ion and collision energy from filterText
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <param name="parentIonMz">Parent ion m/z (output)</param>
    ''' <param name="msLevel">MSLevel (output)</param>
    ''' <param name="collisionMode">Collision mode (output)</param>
    ''' <returns>True if success</returns>
    ''' <remarks>If multiple parent ion m/z values are listed then parentIonMz will have the last one.  However, if the filter text contains "Full msx" then parentIonMz will have the first parent ion listed</remarks>
    Public Shared Function ExtractParentIonMZFromFilterText(filterText As String, <Out> ByRef parentIonMz As Double, <Out> ByRef msLevel As Integer, <Out> ByRef collisionMode As String) As Boolean
        Return XRawFileIO.ExtractParentIonMZFromFilterText(filterText, parentIonMz, msLevel, collisionMode, Nothing)
    End Function

    ''' <summary>
    ''' Parse out the parent ion and collision energy from filterText
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <param name="parentIonMz">Parent ion m/z (output)</param>
    ''' <param name="msLevel">MSLevel (output)</param>
    ''' <param name="collisionMode">Collision mode (output)</param>
    ''' <param name="parentIons">Output: parent ion list</param>
    ''' <returns>True if success</returns>
    ''' <remarks>If multiple parent ion m/z values are listed then parentIonMz will have the last one.  However, if the filter text contains "Full msx" then parentIonMz will have the first parent ion listed</remarks>
    Public Shared Function ExtractParentIonMZFromFilterText(filterText As String, <Out> ByRef parentIonMz As Double, <Out> ByRef msLevel As Integer, <Out> ByRef collisionMode As String, <Out> ByRef parentIons As List(Of ParentIonInfoType)) As Boolean
        ' filterText should be of the form "+ c d Full ms2 1312.95@45.00 [ 350.00-2000.00]"
        ' or "+ c d Full ms3 1312.95@45.00 873.85@45.00 [ 350.00-2000.00]"
        ' or "ITMS + c NSI d Full ms10 421.76@35.00"
        ' or "ITMS + c NSI d sa Full ms2 467.16@etd100.00 [50.00-1880.00]"              ' Note: sa stands for "supplemental activation"
        ' or "ITMS + c NSI d Full ms2 467.16@etd100.00 [50.00-1880.00]"
        ' or "ITMS + c NSI d Full ms2 756.98@cid35.00 [195.00-2000.00]"
        ' or "ITMS + c NSI d Full ms2 606.30@pqd27.00 [50.00-2000.00]"
        ' or "ITMS + c ESI d Full ms2 342.90@cid35.00 [50.00-2000.00]"
        ' or "FTMS + p NSI Full ms [400.00-2000.00]"  (high res full MS)
        ' or "ITMS + c ESI Full ms [300.00-2000.00]"  (low res full MS)
        ' or "ITMS + p ESI d Z ms [1108.00-1118.00]"  (zoom scan)
        ' or "+ p ms2 777.00@cid30.00 [210.00-1200.00]
        ' or "+ c NSI SRM ms2 501.560@cid15.00 [507.259-507.261, 635-319-635.32]
        ' or "FTMS + p NSI d Full msx ms2 712.85@hcd28.00 407.92@hcd28.00  [100.00-1475.00]"
        ' or "ITMS + c NSI r d sa Full ms2 1073.4800@etd120.55@cid20.00 [120.0000-2000.0000]"
        ' or "+ c NSI SRM ms2 748.371 [701.368-701.370, 773.402-773.404, 887.484-887.486, 975.513-975.515"

        Dim bestParentIon = New ParentIonInfoType()
        bestParentIon.Clear()
        msLevel = 1
        parentIonMz = 0
        collisionMode = String.Empty
        Dim matchFound = False
        parentIons = New List(Of ParentIonInfoType)()
        Dim mzText As String = Nothing

        Try
            Dim supplementalActivationEnabled = LabelParser.mFindSAFullMS.IsMatch(filterText)
            Dim multiplexedMSnEnabled = LabelParser.mFindFullMSx.IsMatch(filterText)
            Dim success = ExtractMSLevel(filterText, msLevel, mzText)

            If Not success Then
                Return False
            End If

            ' Use a RegEx to extract out the last parent ion mass listed
            ' For example, grab 1312.95 out of "1312.95@45.00 [ 350.00-2000.00]"
            ' or, grab 873.85 out of "1312.95@45.00 873.85@45.00 [ 350.00-2000.00]"
            ' or, grab 756.98 out of "756.98@etd100.00 [50.00-2000.00]"
            ' or, grab 748.371 out of "748.371 [701.368-701.370, 773.402-773.404, 887.484-887.486, 975.513-975.515"
            '
            ' However, if using multiplex ms/ms (msx) then we return the first parent ion listed

            ' For safety, remove any text after a square bracket
            Dim bracketIndex = mzText.IndexOf("["c)

            If bracketIndex > 0 Then
                mzText = mzText.Substring(0, bracketIndex)
            End If

            ' Find all of the parent ion m/z's present in mzText
            Dim startIndex = 0

            Do
                Dim parentIonMatch = LabelParser.mFindParentIon.Match(mzText, startIndex)

                If Not parentIonMatch.Success Then
                    ' Match not found
                    ' If mzText only contains a number, we will parse it out later in this function
                    Exit Do
                End If

                ' Match found

                parentIonMz = Double.Parse(parentIonMatch.Groups(CStr("ParentMZ")).Value)
                collisionMode = String.Empty
                Dim collisionEnergyValue As Single = 0
                matchFound = True
                startIndex = parentIonMatch.Index + parentIonMatch.Length
                collisionMode = XRawFileIO.GetCapturedValue(parentIonMatch, "CollisionMode1")
                Dim collisionEnergy = XRawFileIO.GetCapturedValue(parentIonMatch, "CollisionEnergy1")

                If Not String.IsNullOrWhiteSpace(collisionEnergy) Then
                    Single.TryParse(collisionEnergy, collisionEnergyValue)
                End If

                Dim collisionEnergy2Value As Single = 0
                Dim collisionMode2 = XRawFileIO.GetCapturedValue(parentIonMatch, "CollisionMode2")

                If Not String.IsNullOrWhiteSpace(collisionMode2) Then
                    Dim collisionEnergy2 = XRawFileIO.GetCapturedValue(parentIonMatch, "CollisionEnergy2")
                    Single.TryParse(collisionEnergy2, collisionEnergy2Value)
                End If

                Dim allowSecondaryActivation = True

                If String.Equals(collisionMode, "ETD", StringComparison.OrdinalIgnoreCase) AndAlso Not String.IsNullOrWhiteSpace(collisionMode2) Then
                    If String.Equals(collisionMode2, "CID", StringComparison.OrdinalIgnoreCase) Then
                        collisionMode = "ETciD"
                        allowSecondaryActivation = False
                    ElseIf String.Equals(collisionMode2, "HCD", StringComparison.OrdinalIgnoreCase) Then
                        collisionMode = "EThcD"
                        allowSecondaryActivation = False
                    End If
                End If

                If allowSecondaryActivation AndAlso Not String.IsNullOrWhiteSpace(collisionMode) Then
                    If supplementalActivationEnabled Then
                        collisionMode = "sa_" & collisionMode
                    End If
                End If

                Dim parentIonInfo = New ParentIonInfoType With {
                    .MSLevel = msLevel,
                    .ParentIonMZ = parentIonMz,
                    .CollisionEnergy = collisionEnergyValue,
                    .CollisionEnergy2 = collisionEnergy2Value
                }
                If Not collisionMode Is Nothing Then parentIonInfo.CollisionMode = String.Copy(collisionMode)
                If Not collisionMode2 Is Nothing Then parentIonInfo.CollisionMode2 = String.Copy(collisionMode2)
                parentIons.Add(parentIonInfo)

                If Not multiplexedMSnEnabled OrElse parentIons.Count = 1 Then
                    bestParentIon = parentIonInfo
                End If
            Loop While startIndex < mzText.Length - 1

            If matchFound Then
                ' Update the output values using bestParentIon
                msLevel = bestParentIon.MSLevel
                parentIonMz = bestParentIon.ParentIonMZ
                collisionMode = bestParentIon.CollisionMode
                Return True
            End If

            ' Match not found using RegEx
            ' Use manual text parsing instead

            Dim atIndex = mzText.LastIndexOf("@"c)

            If atIndex > 0 Then
                mzText = mzText.Substring(0, atIndex)
                Dim spaceIndex = mzText.LastIndexOf(" "c)

                If spaceIndex > 0 Then
                    mzText = mzText.Substring(spaceIndex + 1)
                End If

                Try
                    parentIonMz = Double.Parse(mzText)
                    matchFound = True
                Catch __unusedException1__ As Exception
                    parentIonMz = 0
                End Try

                Return matchFound
            End If

            If mzText.Length = 0 Then Return False

            ' Find the longest contiguous number that mzText starts with

            Dim charIndex = -1

            While charIndex < mzText.Length - 1

                If Char.IsNumber(mzText(charIndex + 1)) OrElse mzText(charIndex + 1) = "."c Then
                    charIndex += 1
                Else
                    Exit While
                End If
            End While

            If charIndex < 0 Then Return False

            Try
                parentIonMz = Double.Parse(mzText.Substring(0, charIndex + 1))
                matchFound = True
                Dim parentIonMzOnly = New ParentIonInfoType()
                parentIonMzOnly.Clear()
                parentIonMzOnly.MSLevel = msLevel
                parentIonMzOnly.ParentIonMZ = parentIonMz
                parentIons.Add(parentIonMzOnly)
            Catch __unusedException1__ As Exception
                parentIonMz = 0
            End Try

            Return matchFound
        Catch __unusedException1__ As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Extract the MS Level from the filter string
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <param name="msLevel"></param>
    ''' <param name="mzText"></param>
    ''' <returns>True if found and False if no match</returns>
    ''' <remarks>
    ''' Looks for "Full ms2" or "Full ms3" or " p ms2" or "SRM ms2" in filterText
    ''' Populates msLevel with the number after "ms" and mzText with the text after "ms2"
    ''' </remarks>
    Public Shared Function ExtractMSLevel(filterText As String, <Out> ByRef msLevel As Integer, <Out> ByRef mzText As String) As Boolean
        Dim matchTextLength = 0
        msLevel = 1
        Dim charIndex = 0
        Dim msMatch = LabelParser.mFindMS.Match(filterText)

        If msMatch.Success Then
            msLevel = Convert.ToInt32(msMatch.Groups(CStr("MSLevel")).Value)
            charIndex = filterText.IndexOf(msMatch.ToString(), StringComparison.OrdinalIgnoreCase)
            matchTextLength = msMatch.Length
        End If

        If charIndex > 0 Then
            ' Copy the text after "Full ms2" or "Full ms3" in filterText to mzText
            mzText = filterText.Substring(charIndex + matchTextLength).Trim()
            Return True
        End If

        mzText = String.Empty
        Return False
    End Function

    ''' <summary>
    ''' Populate mFileInfo
    ''' </summary>
    ''' <returns>True if no error, False if an error</returns>
    ''' <remarks>Called from OpenRawFile</remarks>
    Private Function FillFileInfo() As Boolean
        If mXRawFile Is Nothing Then
            Return False
        Else
            FileInfo.Clear()
        End If

        Try
            If TraceMode Then
                Call "Enumerating device data in the file".__DEBUG_ECHO
            End If

            ' Discover the devices with data in the .raw file
            For Each dev In GetDeviceStats()
                If dev.Value = 0 Then
                    Continue For
                Else
                    FileInfo.Devices.Add(dev.Key, dev.Value)
                End If
            Next

            If FileInfo.Devices.Count = 0 Then
                RaiseWarningMessage("File does not have data from any devices")
            End If

            ' Make sure the MS controller is selected
            If Not SetMSController() Then
                FileInfo.CorruptFile = True
                Return False
            Else
                Return LoadFileInfoInternal()
            End If
        Catch ex As Exception
            RaiseErrorMessage("Error: Exception in FillFileInfo: ", ex)
            Return False
        End Try
    End Function

    Private Function LoadFileInfoInternal() As Boolean
        FileInfo.CreationDate = Date.MinValue
        FileInfo.CreationDate = mXRawFileHeader.CreationDate
        If TraceMode Then Call "Checking mXRawFile.IsError".__DEBUG_ECHO

        If mXRawFile.IsError Then
            Return False
        End If

        If TraceMode Then Call "mXRawFile.IsError reports true".__DEBUG_ECHO
        If TraceMode Then Call "Accessing mXRawFileHeader.WhoCreatedId".__DEBUG_ECHO
        FileInfo.CreatorID = mXRawFileHeader.WhoCreatedId
        If TraceMode Then Call "Accessing mXRawFile.GetInstrumentData".__DEBUG_ECHO
        Dim instData = mXRawFile.GetInstrumentData()
        FileInfo.InstFlags = instData.Flags
        FileInfo.InstHardwareVersion = instData.HardwareVersion
        FileInfo.InstSoftwareVersion = instData.SoftwareVersion
        FileInfo.InstMethods.Clear()

        If Options.LoadMSMethodInfo Then
            LoadMethodInfo()
        End If

        If TraceMode Then Call "Defining the model, name, description, and serial number".__DEBUG_ECHO
        FileInfo.InstModel = instData.Model
        FileInfo.InstName = instData.Name
        FileInfo.InstrumentDescription = mXRawFileHeader.FileDescription
        FileInfo.InstSerialNumber = instData.SerialNumber
        FileInfo.VersionNumber = mXRawFileHeader.Revision
        If TraceMode Then Call "Accessing mXRawFile.RunHeaderEx".__DEBUG_ECHO
        Dim runData = mXRawFile.RunHeaderEx
        FileInfo.MassResolution = runData.MassResolution
        FileInfo.ScanStart = runData.FirstSpectrum
        FileInfo.ScanEnd = runData.LastSpectrum
        FileInfo.AcquisitionFilename = String.Empty

        ' Note that the following are typically blank
        FileInfo.AcquisitionDate = mXRawFileHeader.CreationDate.ToString(CultureInfo.InvariantCulture)
        'mXRawFile.GetAcquisitionFileName(mFileInfo.AcquisitionFilename); // DEPRECATED
        FileInfo.Comment1 = runData.Comment1
        FileInfo.Comment2 = runData.Comment2
        Dim sampleInfo = mXRawFile.SampleInformation
        FileInfo.SampleName = sampleInfo.SampleName
        FileInfo.SampleComment = sampleInfo.Comment

        If Options.LoadMSTuneInfo Then
            GetTuneData()
        End If

        Return True
    End Function

    Private Function GetActivationType(scan As Integer) As ActivationMethods
        Try
            Dim scanFilter = mXRawFile.GetFilterForScanNumber(scan)
            Dim reactions = scanFilter.MassCount

            If reactions <= 0 Then
                Dim msg = String.Format("Scan {0} has no precursor m/z values; this is unexpected for a MSn scan", scan)
                RaiseWarningMessage(msg)
                Return ActivationMethods.Unknown
            End If

            Dim index = reactions - 1

            If index > 0 AndAlso scanFilter.GetIsMultipleActivation(index) Then
                ' The last activation is part of a ETciD/EThcD pair
                index -= 1
            End If

            Dim activationTypeCode As ActivationType = scanFilter.GetActivation(index)
            Dim activationType As ActivationMethods

            Try
                activationType = CType(CInt(activationTypeCode), ActivationMethods)
            Catch
                activationType = ActivationMethods.Unknown
            End Try

            Return activationType
        Catch ex As Exception
            Dim msg = "Error: Exception in GetActivationType: " & ex.Message
            RaiseWarningMessage(msg)
            Return ActivationMethods.Unknown
        End Try
    End Function

    Private Shared Function GetCapturedValue(match As Match, captureGroupName As String) As String
        Dim capturedValue = match.Groups(captureGroupName)

        If Not String.IsNullOrWhiteSpace(capturedValue?.Value) Then
            Return capturedValue.Value
        End If

        Return String.Empty
    End Function

    ''' <summary>
    ''' Get the list of intensity values, by scan, for the given device
    ''' Use this method to retrieve scan-based values for LC devices stored in the .raw file
    ''' </summary>
    ''' <param name="deviceType">Device type</param>
    ''' <param name="deviceNumber">Device number (1 based)</param>
    ''' <param name="scanStart">Start scan, or 0 to use ScanStart</param>
    ''' <param name="scanEnd">End scan, or 0 to use ScanEnd</param>
    ''' <returns>Dictionary where keys are scan number and values are the intensity for the scan</returns>
    ''' <remarks>
    ''' If the scan has multiple intensity values, they are summed
    ''' Scans that have no data will still be present in the dictionary, but with an intensity of 0
    ''' </remarks>
    Public Function GetChromatogramData(deviceType As Device, Optional deviceNumber As Integer = 1, Optional scanStart As Integer = 0, Optional scanEnd As Integer = 0) As Dictionary(Of Integer, Double)
        Dim chromatogramData = New Dictionary(Of Integer, Double)()
        Dim chromatogramData2D = Me.GetChromatogramData2D(deviceType, deviceNumber, scanStart, scanEnd)
        If chromatogramData2D.Count = 0 Then Return chromatogramData

        ' Return the sum of the intensities for each scan
        ' LC data stored as an analog device will typically only have one data value per scan
        For Each scanItem In chromatogramData2D

            If scanItem.Value.Count = 0 Then
                chromatogramData.Add(scanItem.Key, 0)
            Else
                chromatogramData.Add(scanItem.Key, scanItem.Value.Sum())
            End If
        Next

        Return chromatogramData
    End Function

    ''' <summary>
    ''' Get the intensities, by scan, for the given device
    ''' </summary>
    ''' <param name="deviceType">Device type</param>
    ''' <param name="deviceNumber">Device number (1 based)</param>
    ''' <param name="scanStart">Start scan, or 0 to use ScanStart</param>
    ''' <param name="scanEnd">End scan, or 0 to use ScanEnd</param>
    ''' <returns>Dictionary where keys are scan number and values are the list of intensities for that scan</returns>
    ''' <remarks>Scans that have no data will still be present in the dictionary, but with an empty list of doubles</remarks>
    Public Function GetChromatogramData2D(deviceType As Device, Optional deviceNumber As Integer = 1, Optional scanStart As Integer = 0, Optional scanEnd As Integer = 0) As Dictionary(Of Integer, List(Of Double))
        Dim chromatogramData = New Dictionary(Of Integer, List(Of Double))()

        Try
            If mXRawFile Is Nothing Then Return chromatogramData
            If scanStart <= 0 Then scanStart = Me.ScanStart
            If scanEnd <= 0 OrElse scanEnd < Me.ScanStart Then scanEnd = Me.ScanEnd
            Dim warningMessage = Me.ValidateAndSelectDevice(deviceType, deviceNumber)

            If Not String.IsNullOrEmpty(warningMessage) Then
                RaiseWarningMessage(warningMessage & "; cannot load chromatogram data")
                Return chromatogramData
            End If

            Dim lastScanWithData = -1

            For scanNumber = scanStart To scanEnd

                Try
                    Dim scanData = mXRawFile.GetSegmentedScanFromScanNumber(scanNumber, Nothing)
                    If scanData.Intensities Is Nothing OrElse scanData.Intensities.Length = 0 Then Continue For

                    If lastScanWithData >= 0 AndAlso lastScanWithData < scanNumber - 1 Then
                        ' Insert empty lists for the scans that preceded this scan but did not have data
                        For scanToAdd = lastScanWithData + 1 To scanNumber - 1
                            chromatogramData.Add(scanToAdd, New List(Of Double)())
                        Next
                    End If

                    chromatogramData.Add(scanNumber, scanData.Intensities.ToList())
                    lastScanWithData = scanNumber
                Catch __unusedAccessViolationException1__ As AccessViolationException
                    Dim msg = "Unable to load data for scan " & scanNumber & "; possibly a corrupt .Raw file"
                    RaiseWarningMessage(msg)
                Catch ex As Exception
                    Dim msg = "Unable to load data for scan " & scanNumber & ": " & ex.Message & "; possibly a corrupt .Raw file"
                    RaiseErrorMessage(msg, ex)
                End Try
            Next

        Catch ex As Exception
            Dim msg = "Error: Exception in GetChromatogramData: " & ex.Message
            RaiseErrorMessage(msg, ex)
        End Try

        SetMSController()
        Return chromatogramData
    End Function

    ''' <summary>
    ''' Return the collision energy (or energies) for the given scan
    ''' </summary>
    ''' <param name="scan">Scan number</param>
    Public Function GetCollisionEnergy(scan As Integer) As List(Of Double)
        Dim collisionEnergies = New List(Of Double)()
        Dim scanInfo As SingleScanInfo = Nothing
        Dim parentIons As List(Of ParentIonInfoType) = Nothing

        Try
            If mXRawFile Is Nothing Then
                Return collisionEnergies
            Else
                Call GetScanInfo(scan, scanInfo)
            End If

            XRawFileIO.ExtractParentIonMZFromFilterText(scanInfo.FilterText, Nothing, Nothing, Nothing, parentIons)

            For Each parentIon In parentIons
                collisionEnergies.Add(parentIon.CollisionEnergy)

                If parentIon.CollisionEnergy2 > 0 Then
                    ' Filter text is of the form: ITMS + c NSI r d sa Full ms2 1143.72@etd120.55@cid20.00 [120.00-2000.00]
                    ' Data will be stored as
                    ' parentIon.CollisionEnergy = 120.55
                    ' parentIon.CollisionEnergy2 = 20.0
                    collisionEnergies.Add(parentIon.CollisionEnergy2)
                End If
            Next

        Catch ex As Exception
            Dim msg = "Error: Exception in GetCollisionEnergy: " & ex.Message
            RaiseErrorMessage(msg, ex)
        End Try

        Return collisionEnergies
    End Function

    ''' <summary>
    ''' Get the instrument information of the specified device
    ''' </summary>
    Public Function GetDeviceInfo(deviceType As Device, deviceNumber As Integer) As DeviceInfo
        Dim deviceInfo As New DeviceInfo(deviceType, deviceNumber)

        Try
            Dim warningMessage = Me.ValidateAndSelectDevice(deviceType, deviceNumber)

            If Not String.IsNullOrEmpty(warningMessage) Then
                RaiseWarningMessage(warningMessage)
                Return New DeviceInfo(Device.None, 0)
            End If

            Dim instData As InstrumentData = mXRawFile.GetInstrumentData()
            deviceInfo.InstrumentName = If(instData.Name, String.Empty)
            deviceInfo.Model = If(instData.Model, String.Empty)
            deviceInfo.SerialNumber = If(instData.SerialNumber, String.Empty)
            deviceInfo.SoftwareVersion = If(instData.SoftwareVersion, String.Empty)
            deviceInfo.Units = instData.Units
            deviceInfo.AxisLabelX = If(instData.AxisLabelX, String.Empty)
            deviceInfo.AxisLabelY = If(instData.AxisLabelY, String.Empty)
        Catch ex As Exception
            Dim msg = "Error: Exception in GetDeviceInfo: " & ex.Message
            RaiseErrorMessage(msg, ex)
        End Try

        Call SetMSController()

        Return deviceInfo
    End Function

    ''' <summary>
    ''' Get a count of the number of instruments of each device type, as stored in the .raw file
    ''' </summary>
    Public Function GetDeviceStats() As Dictionary(Of Device, Integer)
        Dim devices As New Dictionary(Of Device, Integer)()
        Dim countForDevice As Integer

        Try
            If mXRawFile Is Nothing Then
                Return devices
            End If

            For Each deviceType As Device In [Enum].GetValues(GetType(Device)).Cast(Of Device)()
                countForDevice = mXRawFile.GetInstrumentCountOfType(deviceType)
                devices.Add(deviceType, countForDevice)
            Next

        Catch ex As Exception
            Dim msg = "Error: Exception in GetDeviceStats: " & ex.Message
            RaiseErrorMessage(msg, ex)
        End Try

        Return devices
    End Function

    ''' <summary>
    ''' Number of scans in the .raw file
    ''' </summary>
    ''' <returns>The number of scans, or -1 if an error</returns>
    Public Function GetNumScans() As Integer
        Try
            If mXRawFile Is Nothing Then
                Return -1
            End If

            Dim runData = mXRawFile.RunHeaderEx
            Dim scanCount = runData.SpectraCount
            Dim errorCode = mXRawFile.IsError

            If Not errorCode Then
                Return scanCount
            End If

            Return -1
        Catch __unusedException1__ As Exception
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Get the retention time for the specified scan. Use when searching for scans in a time range.
    ''' </summary>
    ''' <param name="scan">Scan number</param>
    ''' <param name="retentionTime">retention time</param>
    ''' <returns>True if no error, False if an error</returns>
    Public Function GetRetentionTime(scan As Integer, <Out> ByRef retentionTime As Double) As Boolean
        retentionTime = 0

        Try
            If mXRawFile Is Nothing Then Return False

            ' Make sure the MS controller is selected
            If Not SetMSController() Then Return False
            retentionTime = mXRawFile.RetentionTimeFromScanNumber(scan)
        Catch ex As Exception
            Dim msg = "Error: Exception in GetRetentionTime: " & ex.Message
            RaiseWarningMessage(msg)
            Return False
        End Try

        Return True
    End Function

    Private Function GetScanInfoInternal(scanInfo As SingleScanInfo) As Boolean
        Dim scan As Integer = scanInfo.ScanNumber

        ' XRaw periodically mislabels a scan as .EventNumber = 1 when it's really an MS/MS scan; check for this
        ' Parse out the parent ion and collision energy from .FilterText
        Dim scanEventNumber As Integer = Nothing
        Dim ionInjectionTime As Double = Nothing
        Dim msLevel As Integer = Nothing
        Dim parentIonMz As Double = Nothing
        Dim collisionMode As String = Nothing
        Dim simScan As Boolean = Nothing
        Dim mrmScanType As MRMScanTypeConstants = Nothing
        Dim zoomScan As Boolean = Nothing

        ' Make sure the MS controller is selected
        If Not SetMSController() Then
            CacheScanInfo(scan, scanInfo)
            Return False
        End If

        ' Initialize the values that will be populated using GetScanHeaderInfoForScanNum()
        scanInfo.NumPeaks = 0
        scanInfo.TotalIonCurrent = 0
        scanInfo.SIMScan = False
        scanInfo.MRMScanType = MRMScanTypeConstants.NotMRM
        scanInfo.ZoomScan = False
        scanInfo.CollisionMode = String.Empty
        scanInfo.FilterText = String.Empty
        scanInfo.IonMode = IonModes.Unknown

        Dim scanStats As ScanStatistics = mXRawFile.GetScanStatsForScanNumber(scan)

        scanInfo.NumPeaks = scanStats.PacketCount
        scanInfo.RetentionTime = scanStats.StartTime
        scanInfo.LowMass = scanStats.LowMass
        scanInfo.HighMass = scanStats.HighMass
        scanInfo.TotalIonCurrent = scanStats.TIC
        scanInfo.BasePeakMZ = scanStats.BasePeakMass
        scanInfo.BasePeakIntensity = scanStats.BasePeakIntensity
        scanInfo.NumChannels = scanStats.NumberOfChannels
        scanInfo.Frequency = scanStats.Frequency

        Dim errorCode = mXRawFile.IsError

        If errorCode Then
            CacheScanInfo(scan, scanInfo)
            Return False
        End If

        scanInfo.UniformTime = scanStats.IsUniformTime
        scanInfo.IsCentroided = scanStats.IsCentroidScan

        Try

            If Not mCorruptMemoryEncountered Then
                ' Retrieve the additional parameters for this scan (including Scan Event)
                Dim data = mXRawFile.GetTrailerExtraInformation(scan)
                Dim arrayCount = data.Length
                Dim scanEventLabels = data.Labels
                Dim scanEventValues = data.Values

                If arrayCount > 0 AndAlso scanEventLabels IsNot Nothing AndAlso scanEventValues IsNot Nothing Then
                    scanInfo.StoreScanEvents(scanEventLabels, scanEventValues)
                End If
            End If

        Catch ex As AccessViolationException
            Dim msg = "Warning: Exception calling mXRawFile.GetTrailerExtraForScanNum for scan " & scan & ": " & ex.Message
            RaiseWarningMessage(msg)
        Catch ex As Exception
            Dim msg = "Warning: Exception calling mXRawFile.GetTrailerExtraForScanNum for scan " & scan & ": " & ex.Message
            RaiseWarningMessage(msg)

            If ex.Message.IndexOf("memory is corrupt", StringComparison.OrdinalIgnoreCase) >= 0 Then
                mCorruptMemoryEncountered = True
            End If
        End Try

        scanInfo.EventNumber = 1

        ' Look for the entry in scanInfo.ScanEvents named "Scan Event:"
        ' Entries for the LCQ are:
        '   Wideband Activation
        '   Micro Scan Count
        '   Ion Injection Time (ms)
        '   Scan Segment
        '   Scan Event
        '   Elapsed Scan Time (sec)
        '   API Source CID Energy
        '   Resolution
        '   Average Scan by Inst
        '   BackGd Subtracted by Inst
        '   Charge State

        If Integer.TryParse(If(scanInfo.ScanEvents.Find(Function(x) x.Key.StartsWith("scan event", StringComparison.OrdinalIgnoreCase)).Value, String.Empty), scanEventNumber) Then
            scanInfo.EventNumber = scanEventNumber
        End If

        If Double.TryParse(If(scanInfo.ScanEvents.Find(Function(x) x.Key.StartsWith("ion injection time (ms)", StringComparison.OrdinalIgnoreCase)).Value, String.Empty), ionInjectionTime) Then
            scanInfo.IonInjectionTime = ionInjectionTime
        End If

        ' Lookup the filter text for this scan
        ' Parse out the parent ion m/z for fragmentation scans
        Dim scanFilter = mXRawFile.GetFilterForScanNumber(scan)
        Dim filterText = scanFilter.ToString()
        scanInfo.FilterText = String.Copy(filterText)
        scanInfo.IsFTMS = scanFilter.MassAnalyzer = MassAnalyzerType.MassAnalyzerFTMS
        If String.IsNullOrWhiteSpace(scanInfo.FilterText) Then scanInfo.FilterText = String.Empty

        If scanInfo.EventNumber <= 1 Then
            If XRawFileIO.ExtractMSLevel(scanInfo.FilterText, msLevel, Nothing) Then
                scanInfo.EventNumber = msLevel
            End If
        End If

        If scanInfo.EventNumber > 1 Then
            ' MS/MS data
            scanInfo.MSLevel = 2

            If String.IsNullOrWhiteSpace(scanInfo.FilterText) Then
                ' FilterText is empty; this indicates a problem with the .Raw file
                ' This is rare, but does happen (see scans 2 and 3 in QC_Shew_08_03_pt5_1_MAXPRO_27Oct08_Raptor_08-01-01.raw)
                ' We'll set the Parent Ion to 0 m/z and the collision mode to CID
                scanInfo.ParentIonMZ = 0
                scanInfo.CollisionMode = "cid"

                If scanInfo.ActivationType = ActivationMethods.Unknown Then
                    scanInfo.ActivationType = ActivationMethods.CID
                End If

                scanInfo.MRMScanType = MRMScanTypeConstants.NotMRM
            Else

                If ExtractParentIonMZFromFilterText(scanInfo.FilterText, parentIonMz, msLevel, collisionMode) Then
                    scanInfo.ParentIonMZ = parentIonMz
                    scanInfo.CollisionMode = collisionMode

                    If msLevel > 2 Then
                        scanInfo.MSLevel = msLevel
                    End If

                    ' Check whether this is an SRM MS2 scan
                    scanInfo.MRMScanType = DetermineMRMScanType(scanInfo.FilterText)
                Else

                    If ValidateMSScan(scanInfo.FilterText, msLevel, simScan, mrmScanType, zoomScan) Then
                        ' Yes, scan is an MS, SIM, or MRMQMS, or SRM scan
                        scanInfo.MSLevel = msLevel
                        scanInfo.SIMScan = simScan
                        scanInfo.MRMScanType = mrmScanType
                        scanInfo.ZoomScan = zoomScan
                    Else
                        ' Unknown format for .FilterText; return an error
                        RaiseErrorMessage("Unknown format for Scan Filter: " & scanInfo.FilterText)
                        Return False
                    End If
                End If
            End If
        Else
            ' MS1 data
            ' Make sure .FilterText contains one of the known MS1, SIM or MRM tags

            If String.IsNullOrWhiteSpace(scanInfo.FilterText) Then
                ' FilterText is empty; this indicates a problem with the .Raw file
                ' This is rare, but does happen (see scans 2 and 3 in QC_Shew_08_03_pt5_1_MAXPRO_27Oct08_Raptor_08-01-01.raw)
                scanInfo.MSLevel = 1
                scanInfo.SIMScan = False
                scanInfo.MRMScanType = MRMScanTypeConstants.NotMRM
            Else

                If ValidateMSScan(scanInfo.FilterText, msLevel, simScan, mrmScanType, zoomScan) Then
                    ' Yes, scan is an MS, SIM, or MRMQMS, or SRM scan
                    scanInfo.MSLevel = msLevel
                    scanInfo.SIMScan = simScan
                    scanInfo.MRMScanType = mrmScanType
                    scanInfo.ZoomScan = zoomScan
                Else
                    ' Unknown format for .FilterText; return an error
                    RaiseErrorMessage("Unknown format for Scan Filter: " & scanInfo.FilterText)
                    Return False
                End If
            End If
        End If

        scanInfo.IonMode = DetermineIonizationMode(scanInfo.FilterText)

        ' Now that we know MSLevel we can lookup the activation type (aka activation method)
        If scanInfo.MSLevel > 1 Then
            scanInfo.ActivationType = GetActivationType(scan)
        Else
            scanInfo.ActivationType = ActivationMethods.CID
        End If

        Dim newMRMInfo As MRMInfo = Nothing

        If scanInfo.MRMScanType <> MRMScanTypeConstants.NotMRM Then
            ' Parse out the MRM_QMS or SRM information for this scan
            ExtractMRMMasses(scanInfo.FilterText, scanInfo.MRMScanType, newMRMInfo)
        Else
            newMRMInfo = New MRMInfo()
        End If

        scanInfo.MRMInfo = newMRMInfo

        ' Retrieve the Status Log for this scan using the following
        ' The Status Log includes numerous instrument parameters, including voltages, temperatures, pressures, turbo pump speeds, etc.

        Try

            If Not mCorruptMemoryEncountered Then
                Dim retentionTime = mXRawFile.RetentionTimeFromScanNumber(scan)

                ' Get the status log nearest to a retention time.
                Dim statusLogEntry = mXRawFile.GetStatusLogForRetentionTime(retentionTime)
                Dim arrayCount = statusLogEntry.Length
                Dim logNames = statusLogEntry.Labels
                Dim logValues = statusLogEntry.Values

                If arrayCount > 0 Then
                    scanInfo.StoreStatusLog(logNames, logValues)
                End If
            End If

        Catch ex As AccessViolationException
            Dim msg = "Warning: Exception calling mXRawFile.GetStatusLogForScanNum for scan " & scan & ": " & ex.Message
            RaiseWarningMessage(msg)
        Catch ex As Exception
            Dim msg = "Warning: Exception calling mXRawFile.GetStatusLogForScanNum for scan " & scan & ": " & ex.Message
            RaiseWarningMessage(msg)

            If ex.Message.IndexOf("memory is corrupt", StringComparison.OrdinalIgnoreCase) >= 0 Then
                mCorruptMemoryEncountered = True
            End If
        End Try

        Return True
    End Function

    ''' <summary>
    ''' Get the header info for the specified scan
    ''' </summary>
    ''' <param name="scan">Scan number</param>
    ''' <param name="scanInfo">Scan header info class</param>
    ''' <returns>True if no error, False if an error</returns>
    Public Function GetScanInfo(scan As Integer, <Out> ByRef scanInfo As SingleScanInfo) As Boolean
        ' Check for the scan in the cache
        If Me.mCachedScanInfo.TryGetValue(scan, scanInfo) Then
            Return True
        End If

        If scan < FileInfo.ScanStart Then
            scan = FileInfo.ScanStart
        ElseIf scan > FileInfo.ScanEnd Then
            scan = FileInfo.ScanEnd
        End If

        scanInfo = New SingleScanInfo(scan)

        If mXRawFile Is Nothing Then
            Return False
        End If

        Try
            If Not GetScanInfoInternal(scanInfo) Then
                Return False
            End If
        Catch ex As Exception
            Dim msg = "Error: Exception in GetScanInfo: " & ex.Message

            Call RaiseWarningMessage(msg)
            Call CacheScanInfo(scan, scanInfo)

            Return False
        End Try

        Call CacheScanInfo(scan, scanInfo)

        Return True
    End Function

    ''' <summary>
    ''' Parse the scan type name out of the scan filter string
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <returns>Scan type name, e.g. HMS or HCD-HMSn</returns>
    Public Shared Function GetScanTypeNameFromThermoScanFilterText(filterText As String) As String
        ' Examines filterText to determine what the scan type is
        ' Examples:
        ' Given                                                                ScanTypeName
        ' ITMS + c ESI Full ms [300.00-2000.00]                                MS
        ' FTMS + p NSI Full ms [400.00-2000.00]                                HMS
        ' ITMS + p ESI d Z ms [579.00-589.00]                                  Zoom-MS
        ' ITMS + c ESI d Full ms2 583.26@cid35.00 [150.00-1180.00]             CID-MSn
        ' ITMS + c NSI d Full ms2 606.30@pqd27.00 [50.00-2000.00]              PQD-MSn
        ' FTMS + c NSI d Full ms2 516.03@hcd40.00 [100.00-2000.00]             HCD-HMSn
        ' ITMS + c NSI d sa Full ms2 516.03@etd100.00 [50.00-2000.00]          SA_ETD-MSn

        ' FTMS + p NSI d Full msx ms2 712.85@hcd28.00 407.92@hcd28.00  [100.00-1475.00]         HCD-HMSn using multiplexed MSn (introduced with the Q-Exactive)

        ' + c d Full ms2 1312.95@45.00 [ 350.00-2000.00]                                       MSn
        ' + c d Full ms3 1312.95@45.00 873.85@45.00 [ 350.00-2000.00]                          MSn
        ' ITMS + c NSI d Full ms10 421.76@35.00                                                MSn
        ' ITMS + p NSI CRM ms3 332.14@cid35.00 288.10@cid35.00 [242.00-248.00, 285.00-291.00]  CID-MSn

        ' + p ms2 777.00@cid30.00 [210.00-1200.00]                                             CID-MSn
        ' + c NSI SRM ms2 501.560@cid15.00 [507.259-507.261, 635-319-635.32]                   CID-SRM
        ' + c NSI SRM ms2 748.371 [701.368-701.370, 773.402-773.404, 887.484-887.486, 975.513-975.515]    CID-SRM
        ' + p NSI Q1MS [179.652-184.582, 505.778-510.708, 994.968-999.898]                     Q1MS
        ' + p NSI Q3MS [150.070-1500.000]                                                      Q3MS
        ' c NSI Full cnl 162.053 [300.000-1200.000]                                            MRM_Full_NL

        ' Lumos scan filter examples
        ' FTMS + p NSI Full ms                                                                 HMS
        ' ITMS + c NSI r d Full ms2 916.3716@cid30.00 [247.0000-2000.0000]                     CID-MSn
        ' ITMS + c NSI r d Full ms2 916.3716@hcd30.00 [100.0000-2000.0000]                     HCD-MSn

        ' ITMS + c NSI r d sa Full ms2 1073.4800@etd120.55@cid20.00 [120.0000-2000.0000]       ETciD-MSn  (ETD fragmentation, then further fragmented by CID in the ion trap; detected with the ion trap)
        ' ITMS + c NSI r d sa Full ms2 1073.4800@etd120.55@hcd30.00 [120.0000-2000.0000]       EThcD-MSn  (ETD fragmentation, then further fragmented by HCD in the ion routing multipole; detected with the ion trap)

        ' FTMS + c NSI r d Full ms2 744.0129@cid30.00 [199.0000-2000.0000]                     CID-HMSn
        ' FTMS + p NSI r d Full ms2 944.4316@hcd30.00 [100.0000-2000.0000]                     HCD-HMSn

        ' FTMS + c NSI r d sa Full ms2 1073.4800@etd120.55@cid20.00 [120.0000-2000.0000]       ETciD-HMSn  (ETD fragmentation, then further fragmented by CID in the ion trap; detected with orbitrap)
        ' FTMS + c NSI r d sa Full ms2 1073.4800@etd120.55@hcd30.00 [120.0000-2000.0000]       EThcD-HMSn  (ETD fragmentation, then further fragmented by HCD in the ion routing multipole; detected with orbitrap)

        Const defaultScanTypeName = "MS"
        Dim msLevel As Integer = Nothing

        Try
            Dim validScanFilter = True
            Dim collisionMode = String.Empty
            Dim mrmScanType As MRMScanTypeConstants
            Dim simScan = False
            Dim zoomScan = False

            If String.IsNullOrWhiteSpace(filterText) Then
                Return defaultScanTypeName
            End If

            If Not XRawFileIO.ExtractMSLevel(filterText, msLevel, Nothing) Then
                ' Assume this is an MS scan
                msLevel = 1
            End If

            If msLevel > 1 Then
                ' Parse out the parent ion and collision energy from filterText

                If XRawFileIO.ExtractParentIonMZFromFilterText(filterText, Nothing, msLevel, collisionMode) Then
                    ' Check whether this is an SRM MS2 scan
                    mrmScanType = DetermineMRMScanType(filterText)
                Else
                    ' Could not find "Full ms2" in filterText
                    ' XRaw periodically mislabels a scan as .EventNumber > 1 when it's really an MS scan; check for this
                    ' Yes, scan is an MS, SIM, or MRMQMS, or SRM scan
                    If ValidateMSScan(filterText, msLevel, simScan, mrmScanType, zoomScan) Then
                    Else
                        ' Unknown format for filterText; return an error
                        validScanFilter = False
                    End If
                End If
            Else
                ' MSLevel is 1
                ' Make sure .FilterText contains one of the known MS1, SIM or MRM tags
                ' Yes, scan is an MS, SIM, or MRMQMS, or SRM scan
                If ValidateMSScan(filterText, msLevel, simScan, mrmScanType, zoomScan) Then
                Else
                    ' Unknown format for filterText; return an error
                    validScanFilter = False
                End If
            End If

            If Not validScanFilter Then
                Return defaultScanTypeName
            End If

            If mrmScanType = MRMScanTypeConstants.NotMRM OrElse mrmScanType = MRMScanTypeConstants.SIM Then
                If simScan Then
                    Return SIM_MS_TEXT.Trim()
                End If

                If zoomScan Then
                    Return "Zoom-MS"
                End If

                ' This is a standard MS or MSn scan

                Dim baseScanTypeName = If(msLevel > 1, "MSn", "MS")
                Dim scanTypeName As String

                If ScanIsFTMS(filterText) Then
                    ' HMS or HMSn scan
                    scanTypeName = "H" & baseScanTypeName
                Else
                    scanTypeName = baseScanTypeName
                End If

                If msLevel > 1 AndAlso collisionMode.Length > 0 Then
                    Return CapitalizeCollisionMode(collisionMode) & "-" & scanTypeName
                End If

                Return scanTypeName
            End If

            ' This is an MRM or SRM scan
            Select Case mrmScanType
                Case MRMScanTypeConstants.MRMQMS

                    If ContainsText(filterText, MRM_Q1MS_TEXT, 1) Then
                        Return MRM_Q1MS_TEXT.Trim()
                    ElseIf ContainsText(filterText, MRM_Q3MS_TEXT, 1) Then
                        Return MRM_Q3MS_TEXT.Trim()
                    Else
                        ' Unknown QMS mode
                        Return "MRM QMS"
                    End If

                Case MRMScanTypeConstants.SRM

                    If collisionMode.Length > 0 Then
                        Return collisionMode.ToUpper() & "-SRM"
                    Else
                        Return "CID-SRM"
                    End If

                Case MRMScanTypeConstants.FullNL
                    Return "MRM_Full_NL"
                Case Else
                    Return "MRM"
            End Select

        Catch __unusedException1__ As Exception
            ' Ignore errors here
        End Try

        Return defaultScanTypeName
    End Function

    Private Sub GetTuneData()
        Dim numTuneData = mXRawFile.GetTuneDataCount()

        For index As Integer = 0 To numTuneData - 1
            Dim tuneLabelCount = 0
            Dim tuneSettingNames As String() = Nothing
            Dim tuneSettingValues As String() = Nothing
            Dim msg As String

            Try

                If Not mCorruptMemoryEncountered Then
                    Dim tuneData = mXRawFile.GetTuneData(index)
                    tuneSettingNames = tuneData.Labels
                    tuneSettingValues = tuneData.Values
                    tuneLabelCount = tuneData.Length
                End If

            Catch __unusedAccessViolationException1__ As AccessViolationException
                msg = "Unable to load tune data; possibly a corrupt .Raw file"
                RaiseWarningMessage(msg)
                Exit For
            Catch ex As Exception
                ' Exception getting TuneData
                msg = "Warning: Exception calling mXRawFile.GetTuneData for Index " & index & ": " & ex.Message
                RaiseWarningMessage(msg)
                tuneLabelCount = 0

                If ex.Message.IndexOf("memory is corrupt", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    mCorruptMemoryEncountered = True
                    Exit For
                End If
            End Try

            If tuneLabelCount > 0 Then
                msg = String.Empty

                If tuneSettingNames Is Nothing Then
                    ' .GetTuneData returned a non-zero count, but no parameter names; unable to continue
                    msg = "Warning: the GetTuneData function returned a positive tune parameter count but no parameter names"
                ElseIf tuneSettingValues Is Nothing Then
                    ' .GetTuneData returned parameter names, but tuneSettingValues is nothing; unable to continue
                    msg = "Warning: the GetTuneData function returned tune parameter names but no tune values"
                End If

                If msg.Length > 0 Then
                    msg += " (Tune Method " & index + 1 & ")"
                    RaiseWarningMessage(msg)
                    tuneLabelCount = 0
                End If
            End If

            If tuneLabelCount <= 0 OrElse tuneSettingNames Is Nothing OrElse tuneSettingValues Is Nothing Then
                Continue For
            End If

            Dim newTuneMethod As New TuneMethod()
            ' Step through the names and store in the .Setting() arrays
            Dim tuneCategory$ = "General"

            For settingIndex = 0 To tuneLabelCount - 1

                If tuneSettingValues(settingIndex).Length = 0 AndAlso Not tuneSettingNames(settingIndex).EndsWith(":") Then
                    ' New category
                    If tuneSettingNames(settingIndex).Length > 0 Then
                        tuneCategory = String.Copy(tuneSettingNames(settingIndex))
                    Else
                        tuneCategory = "General"
                    End If
                Else
                    Dim tuneMethodSetting = New TuneMethodSettingType() With {
                        .Category = String.Copy(tuneCategory),
                        .Name = tuneSettingNames(settingIndex).TrimEnd(":"c),
                        .Value = String.Copy(tuneSettingValues(settingIndex))
                    }
                    newTuneMethod.Settings.Add(tuneMethodSetting)
                End If
            Next

            If FileInfo.TuneMethods.Count = 0 Then
                FileInfo.TuneMethods.Add(newTuneMethod)
            Else
                ' Compare this tune method to the previous one; if identical, don't keep it
                If Not TuneMethodsMatch(FileInfo.TuneMethods.Last(), newTuneMethod) Then
                    FileInfo.TuneMethods.Add(newTuneMethod)
                End If
            End If
        Next
    End Sub

    Private Sub LoadMethodInfo()
        If TraceMode Then
            Call "Accessing mXRawFile.InstrumentMethodsCount".__DEBUG_ECHO
        End If

        Try
            Dim methodCount As Integer = mXRawFile.InstrumentMethodsCount

            If TraceMode Then
                Call String.Format("File has {0} methods", methodCount).__DEBUG_ECHO
            End If

            For methodIndex As Integer = 0 To methodCount - 1
                If TraceMode Then
                    Call ("Retrieving method from index " & methodIndex).__DEBUG_ECHO
                End If

                Dim methodText = mXRawFile.GetInstrumentMethod(methodIndex)

                If Not String.IsNullOrWhiteSpace(methodText) Then
                    FileInfo.InstMethods.Add(methodText)
                End If
            Next

        Catch ex As Exception

            If Path.DirectorySeparatorChar = "/"c Then
                RaiseWarningMessage("Error while reading the method info: " & ex.Message)
            Else
                RaiseErrorMessage("Error while reading the method info: " & ex.Message)
            End If

            RaiseWarningMessage("Consider instantiating the XRawFileIO class with a ThermoReaderOptions object that has LoadMSMethodInfo = false")
        End Try
    End Sub

    Const defaultGenericScanFilterText = "MS"

    ''' <summary>
    ''' Remove scan-specific data from a scan filter string; primarily removes the parent ion m/z and the scan m/z range
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <returns>Generic filter text, e.g. FTMS + p NSI Full ms</returns>
    Public Shared Function MakeGenericThermoScanFilter(filterText As String) As String
        ' Will make a generic version of the FilterText in filterText
        ' Examples:
        ' From                                                                 To
        ' ITMS + c ESI Full ms [300.00-2000.00]                                ITMS + c ESI Full ms
        ' FTMS + p NSI Full ms [400.00-2000.00]                                FTMS + p NSI Full ms
        ' ITMS + p ESI d Z ms [579.00-589.00]                                  ITMS + p ESI d Z ms
        ' ITMS + c ESI d Full ms2 583.26@cid35.00 [150.00-1180.00]             ITMS + c ESI d Full ms2 0@cid35.00
        ' ITMS + c NSI d Full ms2 606.30@pqd27.00 [50.00-2000.00]              ITMS + c NSI d Full ms2 0@pqd27.00
        ' FTMS + c NSI d Full ms2 516.03@hcd40.00 [100.00-2000.00]             FTMS + c NSI d Full ms2 0@hcd40.00
        ' ITMS + c NSI d sa Full ms2 516.03@etd100.00 [50.00-2000.00]          ITMS + c NSI d sa Full ms2 0@etd100.00

        ' FTMS + p NSI SIM msx ms [475.0000-525.0000]                          FTMS + p NSI SIM msx ms

        ' + c d Full ms2 1312.95@45.00 [ 350.00-2000.00]                       + c d Full ms2 0@45.00
        ' + c d Full ms3 1312.95@45.00 873.85@45.00 [ 350.00-2000.00]          + c d Full ms3 0@45.00 0@45.00
        ' ITMS + c NSI d Full ms10 421.76@35.00                                ITMS + c NSI d Full ms10 0@35.00

        ' + p ms2 777.00@cid30.00 [210.00-1200.00]                             + p ms2 0@cid30.00
        ' + c NSI SRM ms2 501.560@cid15.00 [507.259-507.261, 635-319-635.32]   + c NSI SRM ms2 0@cid15.00
        ' + c NSI SRM ms2 748.371 [701.368-701.370, 773.402-773.404, 887.484-887.486, 975.513-975.515]    + c NSI SRM ms2
        ' + p NSI Q1MS [179.652-184.582, 505.778-510.708, 994.968-999.898]     + p NSI Q1MS
        ' + p NSI Q3MS [150.070-1500.000]                                      + p NSI Q3MS
        ' c NSI Full cnl 162.053 [300.000-1200.000]                            c NSI Full cnl
        If String.IsNullOrWhiteSpace(filterText) Then
            Return defaultGenericScanFilterText
        End If

        Try
            Return MakeGenericThermoScanFilterInternal(filterText)
        Catch __unusedException1__ As Exception
            ' Ignore errors
        End Try

        Return defaultGenericScanFilterText
    End Function

    Private Shared Function MakeGenericThermoScanFilterInternal(filterText As String) As String
        Dim genericScanFilterText As String

        ' First look for and remove numbers between square brackets
        Dim bracketIndex = filterText.IndexOf("["c)

        If bracketIndex > 0 Then
            genericScanFilterText = filterText.Substring(0, bracketIndex).TrimEnd(" "c)
        Else
            genericScanFilterText = filterText.TrimEnd(" "c)
        End If

        Dim fullCnlCharIndex = genericScanFilterText.IndexOf(MRM_FullNL_TEXT, StringComparison.OrdinalIgnoreCase)

        If fullCnlCharIndex > 0 Then
            ' MRM neutral loss
            ' Remove any text after MRM_FullNL_TEXT
            Return genericScanFilterText.Substring(0, fullCnlCharIndex + MRM_FullNL_TEXT.Length).Trim()
        End If

        ' Replace any digits before any @ sign with a 0
        If genericScanFilterText.IndexOf("@"c) > 0 Then
            Return LabelParser.mCollisionSpecs.Replace(genericScanFilterText, " 0@")
        End If

        ' No @ sign; look for text of the form "ms2 748.371"
        Dim match = LabelParser.mMzWithoutCE.Match(genericScanFilterText)

        If match.Success Then
            Return genericScanFilterText.Substring(0, match.Groups(CStr("MzValue")).Index)
        End If

        Return genericScanFilterText
    End Function

    Private Shared Function ScanIsFTMS(filterText As String) As Boolean
        Return ContainsText(filterText, "FTMS")
    End Function

    Private Function SetMSController() As Boolean
        Dim hasMsData As Boolean

        mXRawFile.SelectInstrument(Device.MS, 1)
        hasMsData = mXRawFile.SelectMsData()

        If Not hasMsData Then
            ' Either the file is corrupt, or it simply doesn't have Mass Spec data
            ' The ThermoRawFileReader is primarily intended for
            mCorruptMemoryEncountered = True
        End If

        Return hasMsData
    End Function

    ''' <summary>
    ''' Examines filterText to validate that it is a supported MS1 scan type (MS, SIM, or MRMQMS, or SRM scan)
    ''' </summary>
    ''' <param name="filterText"></param>
    ''' <param name="msLevel"></param>
    ''' <param name="simScan">True if mrmScanType is SIM or MRMQMS</param>
    ''' <param name="mrmScanType"></param>
    ''' <param name="zoomScan"></param>
    ''' <returns>True if filterText contains a known MS scan type</returns>
    ''' <remarks>Returns false for MSn scans (like ms2 or ms3)</remarks>
    Public Shared Function ValidateMSScan(filterText As String, <Out> ByRef msLevel As Integer, <Out> ByRef simScan As Boolean, <Out> ByRef mrmScanType As MRMScanTypeConstants, <Out> ByRef zoomScan As Boolean) As Boolean
        Static ms1Tags As New List(Of String) From {
            FULL_MS_TEXT,
            MS_ONLY_C_TEXT,
            MS_ONLY_P_TEXT,
            MS_ONLY_P_NSI_TEXT,
            FULL_PR_TEXT,
            FULL_LOCK_MS_TEXT
        }
        Static zoomTags As New List(Of String) From {
            MS_ONLY_Z_TEXT,
            MS_ONLY_PZ_TEXT,
            MS_ONLY_DZ_TEXT
        }

        simScan = False
        mrmScanType = MRMScanTypeConstants.NotMRM
        zoomScan = False

        If ContainsAny(filterText, ms1Tags, 1) Then
            ' This is a Full MS scan
            msLevel = 1
            Return True
        End If

        If ContainsAny(filterText, zoomTags, 1) Then
            msLevel = 1
            zoomScan = True
            Return True
        End If

        If ContainsText(filterText, MS_ONLY_PZ_MS2_TEXT, 1) Then
            ' Technically, this should have MSLevel = 2, but that would cause a bunch of problems elsewhere in MASIC
            ' Thus, we'll pretend it's MS1
            msLevel = 1
            zoomScan = True
            Return True
        End If

        mrmScanType = DetermineMRMScanType(filterText)

        Select Case mrmScanType
            Case MRMScanTypeConstants.SIM
                ' Selected ion monitoring, which is MS1 of a narrow m/z range
                msLevel = 1
                simScan = True
                Return True
            Case MRMScanTypeConstants.MRMQMS
                ' Multiple SIM ranges in a single scan
                msLevel = 1
                simScan = True
                Return True
            Case MRMScanTypeConstants.SRM
                msLevel = 2
                Return True
            Case MRMScanTypeConstants.FullNL
                msLevel = 2
                Return True
            Case Else
                XRawFileIO.ExtractMSLevel(filterText, msLevel, Nothing)
                Return False
        End Select
    End Function

    ''' <summary>
    ''' Obtain the mass and intensity list for the specified scan
    ''' </summary>
    ''' <param name="scanNumber">Scan number</param>
    ''' <param name="mzList">Output array of mass values</param>
    ''' <param name="intensityList">Output array of intensity values (parallel to mzList)</param>
    ''' <returns>The number of data points, or -1 if an error</returns>
    ''' <remarks>If maxNumberOfPeaks is 0 (or negative), returns all data; set maxNumberOfPeaks to > 0 to limit the number of data points returned</remarks>
    Public Function GetScanData(scanNumber As Integer, <Out> ByRef mzList As Double(), <Out> ByRef intensityList As Double()) As Integer
        Const maxNumberOfPeaks = 0
        Const centroidData = False
        Return GetScanData(scanNumber, mzList, intensityList, maxNumberOfPeaks, centroidData)
    End Function

    ''' <summary>
    ''' Obtain the mass and intensity list for the specified scan
    ''' </summary>
    ''' <param name="scanNumber">Scan number</param>
    ''' <param name="mzList">Output array of mass values</param>
    ''' <param name="intensityList">Output array of intensity values (parallel to mzList)</param>
    ''' <param name="maxNumberOfPeaks">Set to 0 (or negative) to return all of the data</param>
    ''' <returns>The number of data points, or -1 if an error</returns>
    ''' <remarks>If maxNumberOfPeaks is 0 (or negative), returns all data; set maxNumberOfPeaks to > 0 to limit the number of data points returned</remarks>
    Public Function GetScanData(scanNumber As Integer, <Out> ByRef mzList As Double(), <Out> ByRef intensityList As Double(), maxNumberOfPeaks As Integer) As Integer
        Const centroid = False
        Return GetScanData(scanNumber, mzList, intensityList, maxNumberOfPeaks, centroid)
    End Function

    ''' <summary>
    ''' Obtain the mass and intensity for the specified scan
    ''' </summary>
    ''' <param name="scan">Scan number</param>
    ''' <param name="mzList">Output array of mass values</param>
    ''' <param name="intensityList">Output array of intensity values (parallel to mzList)</param>
    ''' <param name="maxNumberOfPeaks">Set to 0 (or negative) to return all of the data</param>
    ''' <param name="centroidData">True to centroid the data, false to return as-is (either profile or centroid, depending on how the data was acquired)</param>
    ''' <returns>The number of data points, or -1 if an error</returns>
    ''' <remarks>If maxNumberOfPeaks is 0 (or negative),  returns all data; set maxNumberOfPeaks to > 0 to limit the number of data points returned</remarks>
    Public Function GetScanData(scan As Integer, <Out> ByRef mzList As Double(), <Out> ByRef intensityList As Double(), maxNumberOfPeaks As Integer, centroidData As Boolean) As Integer
        Dim dataCount As Integer

        Try
            Dim data = ReadScanData(scan, maxNumberOfPeaks, centroidData)

            dataCount = data.Masses.Length

            If dataCount <= 0 Then
                mzList = New Double(-1) {}
                intensityList = New Double(-1) {}
                Return 0
            Else
                mzList = data.Masses
                intensityList = data.Intensities
            End If

            Return dataCount
        Catch
            mzList = New Double(-1) {}
            intensityList = New Double(-1) {}
            dataCount = 0
            Dim [error] = "Unable to load data for scan " & scan & "; possibly a corrupt .Raw file"
            RaiseWarningMessage([error])
        End Try

        Return dataCount
    End Function

    ''' <summary>
    ''' Obtain the mass and intensity for the specified scan
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <param name="massIntensityPairs">2D array where the first dimension is 0 for mass or 1 for intensity while the second dimension is the data point index</param>
    ''' <returns>The number of data points, or -1 if an error</returns>
    ''' <remarks>If maxNumberOfPeaks is 0 (or negative), returns all data; set maxNumberOfPeaks to > 0 to limit the number of data points returned</remarks>
    Public Function GetScanData2D(scan As Integer, <Out> ByRef massIntensityPairs As Double(,)) As Integer
        Return GetScanData2D(scan, massIntensityPairs, maxNumberOfPeaks:=0, centroidData:=False)
    End Function

    ''' <summary>
    ''' Obtain the mass and intensity for the specified scan
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <param name="massIntensityPairs">2D array where the first dimension is 0 for mass or 1 for intensity while the second dimension is the data point index</param>
    ''' <param name="maxNumberOfPeaks">Maximum number of data points; 0 to return all data</param>
    ''' <returns>The number of data points, or -1 if an error</returns>
    ''' <remarks>If maxNumberOfPeaks is 0 (or negative), returns all data; set maxNumberOfPeaks to > 0 to limit the number of data points returned</remarks>
    Public Function GetScanData2D(scan As Integer, <Out> ByRef massIntensityPairs As Double(,), maxNumberOfPeaks As Integer) As Integer
        Return GetScanData2D(scan, massIntensityPairs, maxNumberOfPeaks, centroidData:=False)
    End Function

    ''' <summary>
    ''' Obtain the mass and intensity for the specified scan
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <param name="massIntensityPairs">2D array where the first dimension is 0 for mass or 1 for intensity while the second dimension is the data point index</param>
    ''' <param name="maxNumberOfPeaks">Maximum number of data points; 0 to return all data</param>
    ''' <param name="centroidData">True to centroid the data, false to return as-is (either profile or centroid, depending on how the data was acquired)</param>
    ''' <returns>The number of data points, or -1 if an error</returns>
    ''' <remarks>If maxNumberOfPeaks is 0 (or negative), returns all data; set maxNumberOfPeaks to > 0 to limit the number of data points returned</remarks>
    Public Function GetScanData2D(scan As Integer, <Out> ByRef massIntensityPairs As Double(,), maxNumberOfPeaks As Integer, centroidData As Boolean) As Integer
        Try
            Dim data = ReadScanData(scan, maxNumberOfPeaks, centroidData)
            Dim dataCount = data.Masses.Length

            If dataCount <= 0 Then
                massIntensityPairs = New Double(-1, -1) {}
                Return 0
            End If

            massIntensityPairs = New Double(1, dataCount - 1) {}

            ' 
            ' A more "black magic" version of doing the below array copy:
            ' Buffer.BlockCopy(data.Masses, 0, massIntensityPairs, 0, dataCount * sizeof(double))
            ' Buffer.BlockCopy(data.Intensities, 0, massIntensityPairs, dataCount * sizeof(double), dataCount * sizeof(double))
            ' 

            For i As Integer = 0 To dataCount - 1
                ' m/z
                massIntensityPairs(0, i) = data.Masses(i)
                ' Intensity
                massIntensityPairs(1, i) = data.Intensities(i)
            Next

            Return dataCount
        Catch ex As Exception
            Dim msg = "Unable to load data for scan " & scan & ": " & ex.Message & "; possibly a corrupt .Raw file"
            RaiseErrorMessage(msg, ex)
        End Try

        massIntensityPairs = New Double(-1, -1) {}

        Return 0
    End Function

    ''' <summary>
    ''' Obtain the mass and intensity for the specified scan
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <param name="maxNumberOfPeaks">Maximum number of data points; 0 to return all data</param>
    ''' <param name="centroidData">True to centroid the data, false to return as-is (either profile or centroid, depending on how the data was acquired)</param>
    ''' <returns>The scan data container, or null if an error</returns>
    Private Function ReadScanData(scan As Integer, maxNumberOfPeaks As Integer, centroidData As Boolean) As ISimpleScanAccess
        If scan < FileInfo.ScanStart Then
            scan = FileInfo.ScanStart
        ElseIf scan > FileInfo.ScanEnd Then
            scan = FileInfo.ScanEnd
        End If

        Dim scanInfo As SingleScanInfo = Nothing

        If Not GetScanInfo(scan, scanInfo) Then
            Throw New Exception("Cannot retrieve ScanInfo from cache for scan " & scan & "; cannot retrieve scan data")
        End If

        Try

            If mXRawFile Is Nothing Then
                Return Nothing
            End If

            ' Make sure the MS controller is selected
            If Not SetMSController() Then
                Return Nothing
            End If

            Dim data As ISimpleScanAccess

            ' If the scan is already centroided, Scan.FromFile and Scan.ToCentroid either won't work,
            ' or will cause centroided data to be re-centroided.
            ' Thus, use GetSegmentedScanFromScanNumber if centroidData is false or if .IsCentroided is true

            If centroidData AndAlso Not scanInfo.IsCentroided Then
                ' Dataset was acquired on an Orbitrap, Exactive, or FTMS instrument, and the scan type
                ' includes additional peak information by default, including centroids: fastest access

                ' Option 1: data = mXRawFile.GetSimplifiedCentroids(scan);
                '           This internally calls the same function as GetCentroidStream, and then copies data to new arrays.
                ' Option 2: Directly call GetCentroidStream

                data = mXRawFile.GetCentroidStream(scan, False)

                If data?.Masses Is Nothing OrElse data.Masses.Length = 0 Then
                    ' Centroiding for profile-mode ion trap data, or for other scan types that don't include a centroid stream
                    Dim scanProf = ThermoFisher.CommonCore.Data.Business.Scan.FromFile(mXRawFile, scan)
                    Dim centroided = ThermoFisher.CommonCore.Data.Business.Scan.ToCentroid(scanProf)
                    data = New SimpleScanAccessTruncated(centroided.PreferredMasses, centroided.PreferredIntensities)
                End If
            Else
                ' Option 1: var scanData = mXRawFile.GetSimplifiedScan(scan);
                '           This internally calls the same function as GetSegmentedScanFromScanNumber, and then copies data to new arrays.
                ' Option 2: Directly call GetSegmentedScanFromScanNumber

                Dim scanData = mXRawFile.GetSegmentedScanFromScanNumber(scan, Nothing)
                data = New SimpleScanAccessTruncated(scanData.Positions, scanData.Intensities)
            End If

            If maxNumberOfPeaks > 0 Then
                ' Takes the maxNumberOfPeaks highest intensities from scan, and sorts them (and their respective mass) by mass into the first maxNumberOfPeaks positions in the arrays.
                Dim sortCount = stdNum.Min(maxNumberOfPeaks, data.Masses.Length)
                Array.Sort(data.Intensities, data.Masses)
                Array.Reverse(data.Intensities)
                Array.Reverse(data.Masses)
                Array.Sort(data.Masses, data.Intensities, 0, sortCount)
                Dim masses = New Double(sortCount - 1) {}
                Dim intensities = New Double(sortCount - 1) {}
                Array.Copy(data.Masses, masses, sortCount)
                Array.Copy(data.Intensities, intensities, sortCount)
                ' ReSharper disable once RedundantIfElseBlock
                data = New SimpleScanAccessTruncated(masses, intensities)
            Else

                ' Although the data returned by mXRawFile.GetMassListFromScanNum is generally sorted by m/z,
                ' we have observed a few cases in certain scans of certain datasets that points with
                ' similar m/z values are swapped and thus slightly out of order

                ' Prior to September 2018, we assured the data was sorted using Array.Sort(data.Masses, data.Intensities);
                ' However, we now leave the data as-is for efficiency purposes

                ' ReSharper disable CommentTypo

                ' If the calling application requires that the data be sorted by m/z, it will need to verify the sort
                ' DeconTools does this in DeconTools.Backend.Runs.XCaliburRun2.GetMassSpectrum

                ' ReSharper restore CommentTypo

            End If

            Return data
        Catch __unusedAccessViolationException1__ As AccessViolationException
            Dim msg = "Unable to load data for scan " & scan & "; possibly a corrupt .Raw file"
            RaiseWarningMessage(msg)
        Catch ex As Exception
            Dim msg = "Unable to load data for scan " & scan & ": " & ex.Message & "; possibly a corrupt .Raw file"
            RaiseErrorMessage(msg, ex)
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' Gets the scan label data for an FTMS-tagged scan
    ''' </summary>
    ''' <param name="scan">Scan number</param>
    ''' <param name="ftLabelData">List of mass, intensity, resolution, baseline intensity, noise floor, and charge for each data point</param>
    ''' <returns>The number of data points, or -1 if an error</returns>
    Public Function GetScanLabelData(scan As Integer, <Out> ByRef ftLabelData As FTLabelInfoType()) As Integer
        Dim scanInfo As SingleScanInfo = Nothing

        If scan < FileInfo.ScanStart Then
            scan = FileInfo.ScanStart
        ElseIf scan > FileInfo.ScanEnd Then
            scan = FileInfo.ScanEnd
        End If

        If Not GetScanInfo(scan, scanInfo) Then
            Throw New Exception("Cannot retrieve ScanInfo from cache for scan " & scan & "; cannot retrieve scan data")
        End If

        If mXRawFile Is Nothing Then
            ftLabelData = New FTLabelInfoType(-1) {}
            Return -1
        End If

        If Not scanInfo.IsFTMS Then
            Dim msg = "Scan " & scan & " is not an FTMS scan; function GetScanLabelData cannot be used with this scan"
            RaiseWarningMessage(msg)
            ftLabelData = New FTLabelInfoType(-1) {}
            Return -1
        End If

        Try
            Dim data As CentroidStream = mXRawFile.GetCentroidStream(scan, False)
            Dim dataCount As Integer = data.Length

            If dataCount > 0 Then
                ftLabelData = getFtLabelData(data)
            Else
                ftLabelData = New FTLabelInfoType(-1) {}
            End If

            Return dataCount
        Catch __unusedAccessViolationException1__ As AccessViolationException
            Dim msg = "Unable to load data for scan " & scan & "; possibly a corrupt .Raw file"
            RaiseWarningMessage(msg)
        Catch ex As Exception
            Dim msg = "Unable to load data for scan " & scan & ": " & ex.Message & "; possibly a corrupt .Raw file"
            RaiseErrorMessage(msg, ex)
        End Try

        ftLabelData = New FTLabelInfoType(-1) {}

        Return -1
    End Function

    Private Function getFtLabelData(data As CentroidStream) As FTLabelInfoType()
        Dim masses = data.Masses
        Dim intensities = data.Intensities
        Dim resolutions = data.Resolutions
        Dim baselines = data.Baselines
        Dim noises = data.Noises
        Dim charges = data.Charges
        Dim dataCount As Integer = data.Length
        Dim ftLabelData = New FTLabelInfoType(dataCount - 1) {}

        For i As Integer = 0 To dataCount - 1
            ftLabelData(i) = New FTLabelInfoType With {
                .Mass = masses(i),
                .Intensity = intensities(i),
                .Resolution = Convert.ToSingle(resolutions(i)),
                .Baseline = Convert.ToSingle(baselines(i)),
                .Noise = Convert.ToSingle(noises(i)),
                .Charge = Convert.ToInt32(charges(i))
            }
        Next

        Return ftLabelData
    End Function

    ''' <summary>
    ''' Get the MSLevel (aka MS order) for a given scan
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <returns>The MSOrder, or 0 if an error</returns>
    ''' <remarks>
    ''' MS1 spectra will return 1, MS2 spectra will return 2, etc.
    ''' Other, specialized scan types:
    '''   Neutral gain:   -3
    '''   Neutral loss:   -2
    '''   Parent scan:    -1
    ''' </remarks>
    Public Function GetMSLevel(scan As Integer) As Integer
        Try
            If mXRawFile Is Nothing Then
                Return 0
            End If

            ' Make sure the MS controller is selected
            If Not SetMSController() Then
                Return 0
            End If

            Dim scanFilter = mXRawFile.GetFilterForScanNumber(scan)
            Dim msOrder = scanFilter.MSOrder

            Return CInt(msOrder)
        Catch ex As Exception
            Dim msg = "Unable to determine the MS Level for scan " & scan & ": " & ex.Message & "; possibly a corrupt .Raw file"
            RaiseErrorMessage(msg, ex)
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Gets scan precision data for FTMS data (resolution of each data point)
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <param name="massResolutionData">List of Intensity, Mass, AccuracyMMU, AccuracyPPM, and Resolution for each data point</param>
    ''' <returns>The number of data points, or -1 if an error</returns>
    ''' <remarks>This returns a subset of the data thatGetScanLabelData does, but with 2 additional fields.</remarks>
    Public Function GetScanPrecisionData(scan As Integer, <Out> ByRef massResolutionData As MassPrecisionInfoType()) As Integer
        Dim scanInfo As SingleScanInfo = Nothing

        If scan < FileInfo.ScanStart Then
            scan = FileInfo.ScanStart
        ElseIf scan > FileInfo.ScanEnd Then
            scan = FileInfo.ScanEnd
        End If

        If Not GetScanInfo(scan, scanInfo) Then
            Throw New Exception("Cannot retrieve ScanInfo from cache for scan " & scan & "; cannot retrieve scan data")
        End If

        Try

            If mXRawFile Is Nothing Then
                massResolutionData = New MassPrecisionInfoType(-1) {}
                Return -1
            End If

            If Not scanInfo.IsFTMS Then
                Dim msg = "Scan " & scan & " is not an FTMS scan; function GetScanLabelData cannot be used with this scan"
                RaiseWarningMessage(msg)
                massResolutionData = New MassPrecisionInfoType(-1) {}
                Return -1
            End If

            'object massResolutionDataList = null;
            Dim mpe = New PrecisionEstimate With {
                .Rawfile = mXRawFile,
                .ScanNumber = scan
            }
            Dim results = mpe.GetMassPrecisionEstimate().ToList()


            If (results.Count > 0) Then
                Dim dataCount = results.Count
                massResolutionData = New MassPrecisionInfoType(dataCount - 1) {}

                For i As Integer = 0 To dataCount - 1
                    Dim massPrecisionInfo As New MassPrecisionInfoType With {
                        .Intensity = results(i).Intensity,
                        .Mass = results(i).Mass,
                        .AccuracyMMU = results(i).MassAccuracyInMmu,
                        .AccuracyPPM = results(i).MassAccuracyInPpm,
                        .Resolution = results(i).Resolution
                    }

                    massResolutionData(i) = massPrecisionInfo
                Next

                Return dataCount
            End If

            massResolutionData = New MassPrecisionInfoType(-1) {}

            Return 0
        Catch __unusedAccessViolationException1__ As AccessViolationException
            Dim msg = "Unable to load data for scan " & scan & "; possibly a corrupt .Raw file"
            RaiseWarningMessage(msg)
        Catch ex As Exception
            Dim msg = "Unable to load data for scan " & scan & ": " & ex.Message & "; possibly a corrupt .Raw file"
            RaiseErrorMessage(msg, ex)
        End Try

        massResolutionData = New MassPrecisionInfoType(-1) {}
        Return -1
    End Function

    ''' <summary>
    ''' Sums data across scans
    ''' </summary>
    ''' <param name="scanFirst"></param>
    ''' <param name="scanLast"></param>
    ''' <param name="massIntensityPairs"></param>
    ''' <param name="maxNumberOfPeaks"></param>
    ''' <param name="centroidData"></param>
    ''' <returns>The number of data points</returns>
    ''' <remarks>Uses the scan filter of the first scan to assure that we're only averaging similar scans</remarks>
    Public Function GetScanDataSumScans(scanFirst As Integer, scanLast As Integer, <Out> ByRef massIntensityPairs As Double(,), maxNumberOfPeaks As Integer, centroidData As Boolean) As Integer
        Try

            Try
                ' Instantiate an instance of the BackgroundSubtractor to assure that file
                ' ThermoFisher.CommonCore.BackgroundSubtraction.dll exists
                Dim bgSub As New BackgroundSubtractor()
                Dim info = bgSub.ToString()

                If String.IsNullOrWhiteSpace(info) Then
                    massIntensityPairs = New Double(-1, -1) {}
                    Return -1
                End If

            Catch __unusedException1__ As Exception
                Const msg = "Unable to load data summing scans; file ThermoFisher.CommonCore.BackgroundSubtraction.dll is missing or corrupt"
                RaiseWarningMessage(msg)
            End Try

            If mXRawFile Is Nothing Then
                massIntensityPairs = New Double(-1, -1) {}
                Return -1
            End If

            ' Make sure the MS controller is selected
            If Not SetMSController() Then
                massIntensityPairs = New Double(-1, -1) {}
                Return -1
            End If

            If scanFirst < FileInfo.ScanStart Then
                scanFirst = FileInfo.ScanStart
            ElseIf scanFirst > FileInfo.ScanEnd Then
                scanFirst = FileInfo.ScanEnd
            End If

            If scanLast < scanFirst Then scanLast = scanFirst

            If scanLast < FileInfo.ScanStart Then
                scanLast = FileInfo.ScanStart
            ElseIf scanLast > FileInfo.ScanEnd Then
                scanLast = FileInfo.ScanEnd
            End If

            If maxNumberOfPeaks < 0 Then maxNumberOfPeaks = 0

            ' Filter scans to only average/sum scans with filter strings similar to the first scan
            ' Without this, AverageScansInScanRange averages/sums all scans in the range, regardless of it being appropriate (i.e., it will sum MS1 and MS2 scans together)
            Dim filter = mXRawFile.GetFilterForScanNumber(scanFirst)
            Dim data = mXRawFile.AverageScansInScanRange(scanFirst, scanLast, filter)

            data.PreferCentroids = centroidData

            Dim masses = data.PreferredMasses
            Dim dataCount = If(maxNumberOfPeaks > 0, stdNum.Min(masses.Length, maxNumberOfPeaks), masses.Length)

            If dataCount > 0 Then
                Dim intensities = data.PreferredIntensities

                If maxNumberOfPeaks > 0 Then
                    Array.Sort(intensities, masses)
                    Array.Reverse(intensities)
                    Array.Reverse(masses)
                    Array.Sort(masses, intensities, 0, dataCount)
                End If

                massIntensityPairs = New Double(1, dataCount - 1) {}

                For i = 0 To dataCount - 1
                    massIntensityPairs(0, i) = masses(i)
                    massIntensityPairs(1, i) = intensities(i)
                Next
            Else
                massIntensityPairs = New Double(-1, -1) {}
            End If

            Return dataCount
        Catch __unusedAccessViolationException1__ As AccessViolationException
            Dim msg = "Unable to load data summing scans " & scanFirst & " to " & scanLast & "; possibly a corrupt .Raw file"
            RaiseWarningMessage(msg)
        Catch ex As Exception
            Dim msg = "Unable to load data summing scans " & scanFirst & " to " & scanLast & ": " & ex.Message & "; possibly a corrupt .Raw file"
            RaiseErrorMessage(msg, ex)
        End Try

        massIntensityPairs = New Double(-1, -1) {}
        Return -1
    End Function

    ''' <summary>
    ''' Open the .raw file
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Function OpenRawFile(filePath As String) As Boolean
        Try
            Dim dataFile = New FileInfo(filePath)

            If Not dataFile.Exists Then
                RaiseErrorMessage(String.Format("File not found: {0}", filePath))
                Return False
            Else
                ' Make sure any existing open files are closed
                Call CloseRawFile()
            End If

            Me.mCachedScanInfo.Clear()
            Me.mCachedScans.Clear()

            RawFilePath = String.Empty

            If TraceMode Then
                Call ("Initializing RawFileReaderAdapter.FileFactory for " & dataFile.FullName).__DEBUG_ECHO
            End If

            mXRawFile = RawFileReaderAdapter.FileFactory(dataFile.FullName)

            If TraceMode Then
                Call "Accessing mXRawFile.FileHeader".__DEBUG_ECHO
            End If

            mXRawFileHeader = mXRawFile.FileHeader

            Call UpdateReaderOptions()

            If mXRawFile.IsError Then
                Return False
            End If

            RawFilePath = dataFile.FullName

            If Not FillFileInfo() Then
                RawFilePath = String.Empty
                Return False
            End If

            If FileInfo.ScanStart = 0 AndAlso
               FileInfo.ScanEnd = 0 AndAlso
               FileInfo.VersionNumber = 0 AndAlso
               stdNum.Abs(FileInfo.MassResolution) < Double.Epsilon AndAlso
               String.IsNullOrWhiteSpace(FileInfo.InstModel) Then

                RaiseErrorMessage("File did not load correctly; ScanStart, ScanEnd, VersionNumber, and MassResolution are all 0 for " & RawFilePath)
                FileInfo.CorruptFile = True
                RawFilePath = String.Empty
                Return False
            End If

            Return True
        Catch ex As Exception
            RaiseErrorMessage(String.Format("Exception opening {0}: {1}", filePath, ex.Message), ex)
            RawFilePath = String.Empty
            Return False
        End Try
    End Function

    Private Function TuneMethodsMatch(method1 As TuneMethod, method2 As TuneMethod) As Boolean
        If method1.Settings.Count <> method2.Settings.Count Then
            ' Different segment number of setting count; the methods don't match
            Return False
        End If

        For index As Integer = 0 To method1.Settings.Count - 1
            If Not method1.Settings(CInt(index)).Category = method2.Settings(CInt(index)).Category OrElse
               Not method1.Settings(CInt(index)).Name = method2.Settings(CInt(index)).Name OrElse
               Not method1.Settings(CInt(index)).Value = method2.Settings(CInt(index)).Value Then

                ' Different segment data; the methods don't match
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' Update options in mXRawFile based on current values in the Options instance of ThermoReaderOptions
    ''' </summary>
    ''' <remarks>Called from OpenRawFile and whenever the Options class raises event OptionsUpdated</remarks>
    Private Sub UpdateReaderOptions()
        mXRawFile.IncludeReferenceAndExceptionData = Options.IncludeReferenceAndExceptionData
    End Sub

    ''' <summary>
    ''' Validate that the .raw file has this device. If it does, select it using mXRawFile.SelectInstrument
    ''' If the device does not exist or there is an error, returns a message describing the problem
    ''' </summary>
    ''' <param name="deviceType"></param>
    ''' <param name="deviceNumber"></param>
    ''' <returns>Empty string if the device was successfully selected, otherwise an error message</returns>
    Private Function ValidateAndSelectDevice(deviceType As Device, deviceNumber As Integer) As String
        Dim deviceCount As Integer = Nothing

        If Not FileInfo.Devices.TryGetValue(deviceType, deviceCount) OrElse deviceCount = 0 Then
            Dim message = String.Format(".raw file does not have data from device type {0}", deviceType)
            Return message
        End If

        If deviceNumber > deviceCount Then
            Dim validValues As String

            If deviceCount = 1 Then
                validValues = String.Format("the file only has one entry for device type {0}; specify deviceNumber = 1", deviceType)
            Else
                validValues = String.Format("valid device numbers for device type {0} are {1} through {2}", deviceType, 1, deviceCount)
            End If

            Dim message = String.Format("The specified device number, {0}, is out of range; {1}", deviceType, validValues)
            Return message
        End If

        Try
            mXRawFile.SelectInstrument(deviceType, deviceNumber)
        Catch ex As Exception
            Dim message = String.Format("Unable to select {0} device #{1}; exception {2}", deviceNumber, deviceType, ex.Message)
            SetMSController()
            Return message
        End Try

        Return String.Empty
    End Function

    ''' <summary>
    ''' Dispose the reader
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        CloseRawFile()
    End Sub

#Region "Obsolete Functions"

    ''' <summary>
    ''' Return unnormalized collision energies via call IScanFilter.GetEnergy
    ''' </summary>
    ''' <param name="scan"></param>
    <Obsolete("The collision energies reported by IScanFilter.GetEnergy are not normalized and are thus not very useful")>
    Public Function GetCollisionEnergyUnnormalized(scan As Integer) As List(Of Double)
        Dim collisionEnergies = New List(Of Double)()

        Try
            If mXRawFile Is Nothing Then Return collisionEnergies
            Dim scanFilter = mXRawFile.GetFilterForScanNumber(scan)
            Dim numMsOrders = CInt(scanFilter.MSOrder)

            For msOrder = 1 To numMsOrders
                Dim collisionEnergy = scanFilter.GetEnergy(msOrder)

                If collisionEnergy > 0 Then
                    collisionEnergies.Add(collisionEnergy)
                End If
            Next

        Catch ex As Exception
            Dim msg = "Error: Exception in GetCollisionEnergyUnnormalized: " & ex.Message
            RaiseErrorMessage(msg, ex)
        End Try

        Return collisionEnergies
    End Function

#End Region
End Class
