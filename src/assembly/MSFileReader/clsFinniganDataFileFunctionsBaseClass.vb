#Region "Microsoft.VisualBasic::7dace1036a690d29609fa0f896a2a125, src\assembly\MSFileReader\clsFinniganDataFileFunctionsBaseClass.vb"

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

    ' 	Class FinniganFileReaderBaseClass
    ' 
    ' 
    ' 		Enum MRMScanTypeConstants
    ' 
    ' 
    ' 
    ' 
    ' 		Enum IonModeConstants
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 		Structure udtTuneMethodType
    ' 
    ' 
    ' 
    ' 		Structure udtFileInfoType
    ' 
    ' 
    ' 
    ' 		Structure udtMRMMassRangeType
    ' 
    ' 
    ' 
    ' 		Structure udtMRMInfoType
    ' 
    ' 
    ' 
    ' 		Structure udtScanHeaderInfoType
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: FileInfo, LoadMSMethodInfo, LoadMSTuneInfo
    ' 
    '     Sub: DuplicateMRMInfo, RaiseErrorMessage, RaiseWarningMessage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Strict On
<Assembly: CLSCompliant(True)> 

' Base class for derived classes that can read Finnigan .Raw files (LCQ, LTQ, etc.)
' 
' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in November 2004
' Copyright 2005, Battelle Memorial Institute.  All Rights Reserved.
'
' Last modified April 19, 2012

Namespace FinniganFileIO

	Public MustInherit Class FinniganFileReaderBaseClass

#Region "Constants and Enums"
		Public Enum MRMScanTypeConstants
			NotMRM = 0
			MRMQMS = 1				' Multiple SIM ranges in a single scan
			SRM = 2					' Monitoring a parent ion and one or more daughter ions
			FullNL = 3				' Full neutral loss scan
		End Enum

		Public Enum IonModeConstants
			Unknown = 0
			Positive = 1
			Negative = 2
		End Enum
#End Region

#Region "Structures"

		Public Structure udtTuneMethodType
			Public Count As Integer
			Public SettingCategory() As String
			Public SettingName() As String
			Public SettingValue() As String
		End Structure

		Public Structure udtFileInfoType
			Public AcquisitionDate As String		' Will often be blank
			Public AcquisitionFilename As String	' Will often be blank
			Public Comment1 As String				' Will often be blank
			Public Comment2 As String				' Will often be blank
			Public SampleName As String				' Will often be blank
			Public SampleComment As String			' Will often be blank

			Public CreationDate As DateTime
			Public CreatorID As String				' Logon name of the user when the file was created
			Public InstFlags As String				' Values should be one of the constants in InstFlags
			Public InstHardwareVersion As String
			Public InstSoftwareVersion As String
			Public InstMethods() As String			' Typically only have one instrument method; the length of this array defines the number of instrument methods
			Public InstModel As String
			Public InstName As String
			Public InstrumentDescription As String	' Typically only defined for instruments converted from other formats
			Public InstSerialNumber As String
			Public TuneMethods() As udtTuneMethodType	' Typically have one or two tune methods; the length of this array defines the number of tune methods defined
			Public VersionNumber As Integer			' File format Version Number
			Public MassResolution As Double
			Public ScanStart As Integer
			Public ScanEnd As Integer
		End Structure

		Public Structure udtMRMMassRangeType
			Public StartMass As Double
			Public EndMass As Double
			Public CentralMass As Double		' Useful for MRM/SRM experiments
		End Structure

		Public Structure udtMRMInfoType
			Public MRMMassCount As Integer					' List of mass ranges monitored by the first quadrupole
			Public MRMMassList() As udtMRMMassRangeType
		End Structure

		Public Structure udtScanHeaderInfoType
			Public MSLevel As Integer					' 1 means MS, 2 means MS/MS, 3 means MS^3 aka MS/MS/MS
			Public EventNumber As Integer				' 1 for parent-ion scan; 2 for 1st frag scan, 3 for 2nd frag scan, etc.
			Public SIMScan As Boolean					' True if this is a selected ion monitoring (SIM) scan (i.e. a small mass range is being examined); if multiple selected ion ranges are examined simultaneously, then this will be false but MRMScanType will be .MRMQMS
			Public MRMScanType As MRMScanTypeConstants	' 1 or 2 if this is a multiple reaction monitoring scan (MRMQMS or SRM)
			Public ZoomScan As Boolean					' True when the given scan is a zoomed in mass region; these spectra are typically skipped when creating SICs

			Public NumPeaks As Integer					' Number of mass intensity value pairs in the specified scan (may not be defined until .GetScanData() is called; -1 if unknown)
			Public RetentionTime As Double				' Retention time (in minutes)
			Public LowMass As Double
			Public HighMass As Double
			Public TotalIonCurrent As Double
			Public BasePeakMZ As Double
			Public BasePeakIntensity As Double

			Public FilterText As String
			Public ParentIonMZ As Double
			Public CollisionMode As String
			Public IonMode As IonModeConstants
			Public MRMInfo As udtMRMInfoType

			Public NumChannels As Integer
			Public UniformTime As Boolean				' Indicates whether the sampling time increment for the controller is constant
			Public Frequency As Double					' Sampling frequency for the current controller
			Public IsCentroidScan As Boolean			' True if centroid (sticks) scan; False if profile (continuum) scan

			Public ScanEventNames() As String
			Public ScanEventValues() As String

			Public StatusLogNames() As String
			Public StatusLogValues() As String
		End Structure

#End Region

#Region "Classwide Variables"
		Protected mCachedFileName As String
		Protected mFileInfo As udtFileInfoType

		Protected mLoadMSMethodInfo As Boolean = True
		Protected mLoadMSTuneInfo As Boolean = True

#End Region

#Region "Interface Functions"
		Public ReadOnly Property FileInfo() As udtFileInfoType
			Get
				Return mFileInfo
			End Get
		End Property

		Public Property LoadMSMethodInfo() As Boolean
			Get
				Return mLoadMSMethodInfo
			End Get
			Set(ByVal value As Boolean)
				mLoadMSMethodInfo = value
			End Set
		End Property

		Public Property LoadMSTuneInfo() As Boolean
			Get
				Return mLoadMSTuneInfo
			End Get
			Set(ByVal value As Boolean)
				mLoadMSTuneInfo = value
			End Set
		End Property
#End Region

#Region "Events"
		Public Event ReportError(ByVal strMessage As String)
		Public Event ReportWarning(ByVal strMessage As String)
#End Region

		Public MustOverride Function CheckFunctionality() As Boolean
		Public MustOverride Sub CloseRawFile()
		Public MustOverride Function GetNumScans() As Integer
		Public MustOverride Function GetScanInfo(ByVal Scan As Integer, ByRef udtScanHeaderInfo As udtScanHeaderInfoType) As Boolean

		Public MustOverride Overloads Function GetScanData(ByVal Scan As Integer, ByRef dblIonMZ() As Double, ByRef dblIonIntensity() As Double, ByRef udtScanHeaderInfo As udtScanHeaderInfoType) As Integer
		Public MustOverride Overloads Function GetScanData(ByVal Scan As Integer, ByRef dblIonMZ() As Double, ByRef dblIonIntensity() As Double, ByRef udtScanHeaderInfo As udtScanHeaderInfoType, ByVal intMaxNumberOfPeaks As Integer) As Integer

		Public MustOverride Function OpenRawFile(ByVal FileName As String) As Boolean

		Protected MustOverride Function FillFileInfo() As Boolean

		Public Shared Sub DuplicateMRMInfo(ByRef udtSource As udtMRMInfoType, ByRef udtTarget As udtMRMInfoType)
			With udtSource
				udtTarget.MRMMassCount = .MRMMassCount

				If .MRMMassList Is Nothing Then
					ReDim udtTarget.MRMMassList(-1)
				Else
					ReDim udtTarget.MRMMassList(.MRMMassList.Length - 1)
					Array.Copy(.MRMMassList, udtTarget.MRMMassList, .MRMMassList.Length)
				End If
			End With
		End Sub

		Protected Sub RaiseErrorMessage(ByVal strMessage As String)
			RaiseEvent ReportError(strMessage)
		End Sub

		Protected Sub RaiseWarningMessage(ByVal strMessage As String)
			RaiseEvent ReportWarning(strMessage)
		End Sub

	End Class
End Namespace
