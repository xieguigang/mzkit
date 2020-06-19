Option Strict On

' These functions utilize MSFileReader.XRawfile2.dll to extract scan header info and
' raw mass spectrum info from Finnigan LCQ, LTQ, and LTQ-FT files
' 
' Required Dlls: fileio.dll, fregistry.dll, and MSFileReader.XRawfile2.dll
' DLLs obtained from: Thermo software named "MS File Reader 2.2"
' Download link: http://sjsupport.thermofinnigan.com/public/detail.asp?id=703
' 
' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in November 2004
' Copyright 2005, Battelle Memorial Institute.  All Rights Reserved.
'
' Switched from XRawFile2.dll to MSFileReader.XRawfile2.dll in March 2012

' Last modified January 9, 2013

Imports System.Runtime.InteropServices

Namespace FinniganFileIO

	Public Class XRawFileIO
		Inherits FinniganFileReaderBaseClass

#Region "Constants and Enums"

		' Note that each of these strings has a space at the end; this is important to avoid matching inappropriate text in the filter string
		Private Const MS_ONLY_C_TEXT As String = " c ms "
		Private Const MS_ONLY_P_TEXT As String = " p ms "

		Private Const MS_ONLY_PZ_TEXT As String = " p Z ms "			' Likely a zoom scan
		Private Const MS_ONLY_DZ_TEXT As String = " d Z ms "			' Dependent zoom scan
		Private Const MS_ONLY_PZ_MS2_TEXT As String = " d Z ms2 "			' Dependent MS2 zoom scan
		Private Const MS_ONLY_Z_TEXT As String = " NSI Z ms "			' Likely a zoom scan

		Private Const FULL_MS_TEXT As String = "Full ms "
		Private Const FULL_PR_TEXT As String = "Full pr "				' TSQ: Full Parent Scan, Product Mass
		Private Const SIM_MS_TEXT As String = "SIM ms "
		Private Const MRM_Q1MS_TEXT As String = "Q1MS "
		Private Const MRM_Q3MS_TEXT As String = "Q3MS "
		Private Const MRM_SRM_TEXT As String = "SRM ms2"
		Private Const MRM_FullNL_TEXT As String = "Full cnl "			' MRM neutral loss

		' This RegEx matches Full ms2, Full ms3, ..., Full ms10, Full ms11, ...
		' It also matches p ms2
		' It also matches SRM ms2
		' It also matches CRM ms3
		' It also matches Full msx ms2 (multiplexed parent ion selection, introduced with the Q-Exactive)
		Private Const MS2_REGEX As String = "( p|Full|SRM|CRM|Full msx) ms([2-9]|[1-9][0-9]) "

		' Used with .GetSeqRowSampleType()
		Public Enum SampleTypeConstants
			Unknown = 0
			Blank = 1
			QC = 2
			StandardClear_None = 3
			StandardUpdate_None = 4
			StandardBracket_Open = 5
			StandardBracketStart_MultipleBrackets = 6
			StandardBracketEnd_multipleBrackets = 7
		End Enum

		' Used with .SetController()
		Public Enum ControllerTypeConstants
			NoDevice = -1
			MS = 0
			Analog = 1
			AD_Card = 2
			PDA = 3
			UV = 4
		End Enum

		' Used with .GetMassListXYZ()
		Public Enum IntensityCutoffTypeConstants
			None = 0						' AllValuesReturned
			AbsoluteIntensityUnits = 1
			RelativeToBasePeak = 2
		End Enum

		'Public Enum ErrorCodeConstants
		'    MassRangeFormatIncorrect = -6
		'    FilterFormatIncorrect = -5
		'    ParameterInvalid = -4
		'    OperationNotSupportedOnCurrentController = -3
		'    CurrentControllerInvalid = -2
		'    RawFileInvalid = -1
		'    Failed = 0
		'    Success = 1
		'    NoDataPresent = 2
		'End Enum

		Public Class InstFlags
			Public Const TIM As String = "Total Ion Map"
			Public Const NLM As String = "Neutral Loss Map"
			Public Const PIM As String = "Parent Ion Map"
			Public Const DDZMap As String = "Data Dependent ZoomScan Map"
		End Class

#End Region

#Region "Structures"
		Public Structure udtParentIonInfoType
			Public MSLevel As Integer
			Public ParentIonMZ As Double
			Public CollisionEnergy As Single
			Public CollisionMode As String
			Public Sub Clear()
				MSLevel = 1
				ParentIonMZ = 0
				CollisionEnergy = 0
				CollisionMode = String.Empty
			End Sub
		End Structure
#End Region

#Region "Classwide Variables"

		' Cached XRawFile object, for faster accessing
		Private mXRawFile As MSFileReaderLib.IXRawfile5

		Private mCorruptMemoryEncountered As Boolean

#End Region

		Public Overrides Function CheckFunctionality() As Boolean
			' I have a feeling this doesn't actually work, and will always return True

			Try
				Dim objXRawFile As New MSFileReaderLib.MSFileReader_XRawfile
				objXRawFile = Nothing

				' If we get here, then all is fine
				Return True
			Catch ex As System.Exception
				Return False
			End Try

		End Function

		Public Overrides Sub CloseRawFile()

			Try
				If Not mXRawFile Is Nothing Then
					mXRawFile.Close()
				End If
				mCorruptMemoryEncountered = False

			Catch ex As System.Exception
				' Ignore any errors
			Finally
				mXRawFile = Nothing
				mCachedFileName = String.Empty
			End Try

		End Sub

		Public Shared Function DetermineMRMScanType(ByVal strFilterText As String) As MRMScanTypeConstants
			Dim eMRMScanType As MRMScanTypeConstants

			eMRMScanType = FinniganFileReaderBaseClass.MRMScanTypeConstants.NotMRM
			If Not String.IsNullOrWhiteSpace(strFilterText) Then
				If strFilterText.ToLower.IndexOf(MRM_Q1MS_TEXT.ToLower) > 0 OrElse _
				   strFilterText.ToLower.IndexOf(MRM_Q3MS_TEXT.ToLower) > 0 Then
					eMRMScanType = MRMScanTypeConstants.MRMQMS
				ElseIf strFilterText.ToLower.IndexOf(MRM_SRM_TEXT.ToLower) > 0 Then
					eMRMScanType = MRMScanTypeConstants.SRM
				ElseIf strFilterText.ToLower.IndexOf(MRM_FullNL_TEXT.ToLower) > 0 Then
					eMRMScanType = MRMScanTypeConstants.FullNL
				End If
			End If

			Return eMRMScanType
		End Function

		Public Shared Function DetermineIonizationMode(ByVal strFiltertext As String) As IonModeConstants

			' Determine the ion mode by simply looking for the first + or - sign

			Const IONMODE_REGEX As String = "[+-]"

			Static reIonMode As System.Text.RegularExpressions.Regex

			Dim eIonMode As IonModeConstants
			Dim reMatch As System.Text.RegularExpressions.Match

			If reIonMode Is Nothing Then
				reIonMode = New System.Text.RegularExpressions.Regex(IONMODE_REGEX, Text.RegularExpressions.RegexOptions.Compiled)
			End If

			eIonMode = IonModeConstants.Unknown

			If Not String.IsNullOrWhiteSpace(strFiltertext) Then
				' Parse out the text between the square brackets
				reMatch = reIonMode.Match(strFiltertext)

				If Not reMatch Is Nothing AndAlso reMatch.Success Then
					Select Case reMatch.Value
						Case "+"
							eIonMode = IonModeConstants.Positive
						Case "-"
							eIonMode = IonModeConstants.Negative
						Case Else
							eIonMode = IonModeConstants.Unknown
					End Select
				End If

			End If

			Return eIonMode

		End Function

		Public Shared Sub ExtractMRMMasses(ByVal strFilterText As String, _
		  ByVal eMRMScanType As MRMScanTypeConstants, _
		   <Out()> ByRef udtMRMInfo As udtMRMInfoType)

			' Parse out the MRM_QMS or SRM mass info from strFilterText
			' It should be of the form 
			' MRM_Q1MS_TEXT:    p NSI Q1MS [179.652-184.582, 505.778-510.708, 994.968-999.898]
			' or
			' MRM_Q3MS_TEXT:    p NSI Q3MS [150.070-1500.000]
			' or
			' MRM_SRM_TEXT:    c NSI SRM ms2 489.270@cid17.00 [397.209-392.211, 579.289-579.291]

			' Note: we do not parse mass information out for Full Neutral Loss scans
			' MRM_FullNL_TEXT: c NSI Full cnl 162.053 [300.000-1200.000]

			Const MASSLIST_REGEX As String = "\[[0-9.]+-[0-9.]+.*\]"
			Const MASSRANGES_REGEX As String = "([0-9.]+)-([0-9.]+)"

			Static reMassList As System.Text.RegularExpressions.Regex
			Static reMassRanges As System.Text.RegularExpressions.Regex

			Dim reMatch As System.Text.RegularExpressions.Match

			If reMassList Is Nothing Then
				reMassList = New System.Text.RegularExpressions.Regex(MASSLIST_REGEX, Text.RegularExpressions.RegexOptions.Compiled)
			End If

			If reMassRanges Is Nothing Then
				reMassRanges = New System.Text.RegularExpressions.Regex(MASSRANGES_REGEX, Text.RegularExpressions.RegexOptions.Compiled)
			End If


			udtMRMInfo = New udtMRMInfoType
			If udtMRMInfo.MRMMassList Is Nothing Then
				InitializeMRMInfo(udtMRMInfo, 0)
			Else
				udtMRMInfo.MRMMassCount = 0
			End If

			If Not String.IsNullOrWhiteSpace(strFilterText) Then

				If eMRMScanType = FinniganFileReaderBaseClass.MRMScanTypeConstants.MRMQMS Or _
				   eMRMScanType = FinniganFileReaderBaseClass.MRMScanTypeConstants.SRM Then

					' Parse out the text between the square brackets
					reMatch = reMassList.Match(strFilterText)

					If Not reMatch Is Nothing AndAlso reMatch.Success Then
						reMatch = reMassRanges.Match(reMatch.ToString)
						If Not reMatch Is Nothing Then

							InitializeMRMInfo(udtMRMInfo, 2)

							Do While reMatch.Success
								Try
									' Note that group 0 is the full mass range (two mass values, separated by a dash)
									' Group 1 is the first mass value
									' Group 2 is the second mass value

									If reMatch.Groups.Count >= 3 Then
										If udtMRMInfo.MRMMassCount = udtMRMInfo.MRMMassList.Length Then
											' Need to reserve more room
											ReDim Preserve udtMRMInfo.MRMMassList(udtMRMInfo.MRMMassList.Length * 2 - 1)
										End If

										With udtMRMInfo.MRMMassList(udtMRMInfo.MRMMassCount)
											.StartMass = Double.Parse(reMatch.Groups(1).Value)
											.EndMass = Double.Parse(reMatch.Groups(2).Value)
											.CentralMass = Math.Round(.StartMass + (.EndMass - .StartMass) / 2, 6)
										End With
										udtMRMInfo.MRMMassCount += 1
									End If

								Catch ex As System.Exception
									' Error parsing out the mass values; skip this group
								End Try

								reMatch = reMatch.NextMatch
							Loop

						End If
					End If
				Else
					' Unsupported MRM type
				End If
			End If

			If udtMRMInfo.MRMMassList.Length > udtMRMInfo.MRMMassCount Then
				If udtMRMInfo.MRMMassCount <= 0 Then
					ReDim udtMRMInfo.MRMMassList(-1)
				Else
					ReDim Preserve udtMRMInfo.MRMMassList(udtMRMInfo.MRMMassCount - 1)
				End If
			End If

		End Sub

		''' <summary>
		''' Parse out the parent ion and collision energy from strFilterText
		''' </summary>
		''' <param name="strFilterText"></param>
		''' <param name="dblParentIonMZ">Parent ion m/z (output)</param>
		''' <param name="intMSLevel">MSLevel (output)</param>
		''' <param name="strCollisionMode">Collision mode (output)</param>
		''' <returns>True if success</returns>
		''' <remarks>If multiple parent ion m/z values are listed then dblParentIonMZ will have the last one.  However, if the filter text contains "Full msx" then dblParentIonMZ will have the first parent ion listed</remarks>
		Public Shared Function ExtractParentIonMZFromFilterText(ByVal strFilterText As String, <Out()> ByRef dblParentIonMZ As Double, <Out()> ByRef intMSLevel As Integer, <Out()> ByRef strCollisionMode As String) As Boolean
			Dim lstParentIons As Generic.List(Of udtParentIonInfoType) = Nothing

			Return ExtractParentIonMZFromFilterText(strFilterText, dblParentIonMZ, intMSLevel, strCollisionMode, lstParentIons)

		End Function

		''' <summary>
		''' Parse out the parent ion and collision energy from strFilterText
		''' </summary>
		''' <param name="strFilterText"></param>
		''' <param name="dblParentIonMZ">Parent ion m/z (output)</param>
		''' <param name="intMSLevel">MSLevel (output)</param>
		''' <param name="strCollisionMode">Collision mode (output)</param>
		''' <returns>True if success</returns>
		''' <remarks>If multiple parent ion m/z values are listed then dblParentIonMZ will have the last one.  However, if the filter text contains "Full msx" then dblParentIonMZ will have the first parent ion listed</remarks>
		Public Shared Function ExtractParentIonMZFromFilterText(ByVal strFilterText As String, <Out()> ByRef dblParentIonMZ As Double, <Out()> ByRef intMSLevel As Integer, <Out()> ByRef strCollisionMode As String, <Out()> ByRef lstParentIons As Generic.List(Of udtParentIonInfoType)) As Boolean


			' strFilterText should be of the form "+ c d Full ms2 1312.95@45.00 [ 350.00-2000.00]"
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

			' This RegEx matches text like 1312.95@45.00 or 756.98@cid35.00
			Const PARENTION_REGEX As String = "([0-9.]+)@([a-z]*)([0-9.]+)"

			' This RegEx looks for "sa" prior to Full ms"
			Const SA_REGEX As String = " sa Full ms"

			Const MSX_REGEX As String = " Full msx "

			Dim intCharIndex As Integer
			Dim intStartIndex As Integer

			Dim strMZText As String

			Dim blnMatchFound As Boolean
			Dim blnSuccess As Boolean

			Dim blnSupplementalActivationEnabled As Boolean
			Dim blnMultiplexedMSnEnabled As Boolean

			Static reFindParentIon As System.Text.RegularExpressions.Regex
			Static reFindSAFullMS As System.Text.RegularExpressions.Regex
			Static reFindFullMSx As System.Text.RegularExpressions.Regex

			Dim reMatchParentIon As System.Text.RegularExpressions.Match

			Dim strCollisionEnergy As String
			Dim sngCollisionEngergy As Single

			Dim udtBestParentIon As udtParentIonInfoType = New udtParentIonInfoType
			udtBestParentIon.Clear()

			intMSLevel = 1
			dblParentIonMZ = 0
			strCollisionMode = String.Empty
			strMZText = String.Empty
			blnMatchFound = False

			If lstParentIons Is Nothing Then
				lstParentIons = New Generic.List(Of udtParentIonInfoType)
			Else
				lstParentIons.Clear()
			End If

			Try

				If reFindSAFullMS Is Nothing Then
					reFindSAFullMS = New System.Text.RegularExpressions.Regex(SA_REGEX, Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.Compiled)
				End If
				blnSupplementalActivationEnabled = reFindSAFullMS.IsMatch(strFilterText)

				If reFindFullMSx Is Nothing Then
					reFindFullMSx = New System.Text.RegularExpressions.Regex(MSX_REGEX, Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.Compiled)
				End If
				blnMultiplexedMSnEnabled = reFindFullMSx.IsMatch(strFilterText)

				blnSuccess = ExtractMSLevel(strFilterText, intMSLevel, strMZText)

				If blnSuccess Then
					' Use a RegEx to extract out the last parent ion mass listed
					' For example, grab 1312.95 out of "1312.95@45.00 [ 350.00-2000.00]"
					' or, grab 873.85 out of "1312.95@45.00 873.85@45.00 [ 350.00-2000.00]"
					' or, grab 756.98 out of "756.98@etd100.00 [50.00-2000.00]"
					'
					' However, if using multiplex ms/ms (msx) then we return the first parent ion listed

					' For safety, remove any text after a square bracket
					intCharIndex = strMZText.IndexOf("["c)
					If intCharIndex > 0 Then
						strMZText = strMZText.Substring(0, intCharIndex)
					End If

					If reFindParentIon Is Nothing Then
						reFindParentIon = New System.Text.RegularExpressions.Regex(PARENTION_REGEX, Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.Compiled)
					End If

					' Find all of the parent ion m/z's present in strMZText
					intStartIndex = 0
					Do
						reMatchParentIon = reFindParentIon.Match(strMZText, intStartIndex)

						If Not reMatchParentIon Is Nothing Then
							If reMatchParentIon.Groups.Count >= 2 Then
								dblParentIonMZ = Double.Parse(reMatchParentIon.Groups(1).Value)
								strCollisionMode = String.Empty
								sngCollisionEngergy = 0

								blnMatchFound = True

								intStartIndex = reMatchParentIon.Index + reMatchParentIon.Length

								If reMatchParentIon.Groups.Count >= 3 Then
									strCollisionMode = reMatchParentIon.Groups(2).Value
									If strCollisionMode Is Nothing Then
										strCollisionMode = String.Empty
									Else
										If blnSupplementalActivationEnabled Then
											strCollisionMode = "sa_" & strCollisionMode
										End If
									End If

								End If

								If reMatchParentIon.Groups.Count >= 4 Then
									strCollisionEnergy = reMatchParentIon.Groups(3).Value
									If Not strCollisionEnergy Is Nothing Then
										Single.TryParse(strCollisionEnergy, sngCollisionEngergy)
									End If
								End If

								Dim udtParentIonInfo As udtParentIonInfoType
								With udtParentIonInfo
									.MSLevel = intMSLevel
									.ParentIonMZ = dblParentIonMZ
									.CollisionEnergy = sngCollisionEngergy
									.CollisionMode = String.Copy(strCollisionMode)
								End With
								lstParentIons.Add(udtParentIonInfo)

								If Not blnMultiplexedMSnEnabled OrElse (lstParentIons.Count = 1 AndAlso blnMultiplexedMSnEnabled) Then
									udtBestParentIon = udtParentIonInfo
								End If

							ElseIf intStartIndex > 0 Then
								' Additional match was not found, exit the loop
								Exit Do
							Else
								' Match not found
								blnMatchFound = False
								Exit Do
							End If

						Else
							Exit Do
						End If
					Loop While intStartIndex < strMZText.Length - 1

					If blnMatchFound Then
						' Update the output values using udtBestParentIon
						With udtBestParentIon
							intMSLevel = .MSLevel
							dblParentIonMZ = .ParentIonMZ
							strCollisionMode = .CollisionMode
						End With
					Else
						' Match not found using RegEx
						' Use manual text parsing instead

						intCharIndex = strMZText.LastIndexOf("@"c)
						If intCharIndex > 0 Then
							strMZText = strMZText.Substring(0, intCharIndex)
							intCharIndex = strMZText.LastIndexOf(" "c)
							If intCharIndex > 0 Then
								strMZText = strMZText.Substring(intCharIndex + 1)
							End If

							Try
								dblParentIonMZ = Double.Parse(strMZText)
								blnMatchFound = True
							Catch ex As System.Exception
								dblParentIonMZ = 0
							End Try

						ElseIf strMZText.Length > 0 Then
							' Find the longest contiguous number that strMZText starts with

							intCharIndex = -1
							Do While intCharIndex < strMZText.Length - 1
								If Char.IsNumber(strMZText.Chars(intCharIndex + 1)) OrElse strMZText.Chars(intCharIndex + 1) = "."c Then
									intCharIndex += 1
								Else
									Exit Do
								End If
							Loop

							If intCharIndex >= 0 Then
								Try
									dblParentIonMZ = Double.Parse(strMZText.Substring(0, intCharIndex + 1))
									blnMatchFound = True
								Catch ex As System.Exception
									dblParentIonMZ = 0
								End Try
							End If
						End If
					End If
				End If

			Catch ex As System.Exception
				blnMatchFound = False
			End Try

			Return blnMatchFound

		End Function

		Public Shared Function ExtractMSLevel(ByVal strFilterText As String, <Out()> ByRef intMSLevel As Integer, <Out()> ByRef strMZText As String) As Boolean
			' Looks for "Full ms2" or "Full ms3" or " p ms2" or "SRM ms2" in strFilterText
			' Returns True if found and False if no match

			' Populates intMSLevel with the number after "ms" and strMZText with the text after "ms2"

			Static reFindMS As System.Text.RegularExpressions.Regex
			Dim reMatchMS As System.Text.RegularExpressions.Match

			Dim intCharIndex As Integer
			Dim intMatchTextLength As Integer

			intMSLevel = 1
			intCharIndex = 0

			If reFindMS Is Nothing Then
				reFindMS = New System.Text.RegularExpressions.Regex(MS2_REGEX, Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.Compiled)
			End If
			reMatchMS = reFindMS.Match(strFilterText)

			If Not reMatchMS Is Nothing Then
				If reMatchMS.Groups.Count >= 3 Then
					intMSLevel = CInt(reMatchMS.Groups(2).Value)
					intCharIndex = strFilterText.ToLower.IndexOf(reMatchMS.ToString.ToLower)
					intMatchTextLength = reMatchMS.Length
				Else
					' Match not found
					intCharIndex = 0
				End If
			End If

			If intCharIndex > 0 Then
				' Copy the text after "Full ms2" or "Full ms3" in strFilterText to strMZText
				strMZText = strFilterText.Substring(intCharIndex + intMatchTextLength).Trim
				Return True
			Else
				strMZText = String.Empty
				Return False
			End If

		End Function

		Protected Overrides Function FillFileInfo() As Boolean
			' Populates the mFileInfo structure
			' Function returns True if no error, False if an error

			Dim intResult As Integer

			Dim intIndex As Integer
			Dim intSettingIndex As Integer

			Dim intMethodCount As Integer
			Dim intNumTuneData As Integer
			Dim intTuneMethodCountValid As Integer

			Dim strMethod As String = String.Empty

			Dim strTuneCategory As String
			Dim strTuneSettingNames() As String
			Dim strTuneSettingValues() As String
			Dim intTuneLabelCount As Integer

			Dim objLabels As Object, objValues As Object
			Dim strWarningMessage As String

			Try
				If mXRawFile Is Nothing Then Return False

				' Make sure the MS controller is selected
				If Not SetMSController() Then Return False

				With mFileInfo

					.CreationDate = Nothing
					mXRawFile.GetCreationDate(.CreationDate)
					mXRawFile.IsError(intResult)						' Unfortunately, .IsError() always returns 0, even if an error occurred
					If intResult <> 0 Then Return False

					.CreatorID = Nothing
					mXRawFile.GetCreatorID(.CreatorID)

					.InstFlags = Nothing
					mXRawFile.GetInstFlags(.InstFlags)

					.InstHardwareVersion = Nothing
					mXRawFile.GetInstHardwareVersion(.InstHardwareVersion)

					.InstSoftwareVersion = Nothing
					mXRawFile.GetInstSoftwareVersion(.InstSoftwareVersion)

					If Not mLoadMSMethodInfo Then
						ReDim .InstMethods(-1)
					Else

						mXRawFile.GetNumInstMethods(intMethodCount)
						ReDim .InstMethods(intMethodCount - 1)

						For intIndex = 0 To intMethodCount - 1
							strMethod = Nothing
							mXRawFile.GetInstMethod(intIndex, strMethod)
							If String.IsNullOrWhiteSpace(strMethod) Then
								.InstMethods(intIndex) = String.Empty
							Else
								.InstMethods(intIndex) = String.Copy(strMethod)
							End If

						Next intIndex
					End If

					.InstModel = Nothing
					.InstName = Nothing
					.InstrumentDescription = Nothing
					.InstSerialNumber = Nothing

					mXRawFile.GetInstModel(.InstModel)
					mXRawFile.GetInstName(.InstName)
					mXRawFile.GetInstrumentDescription(.InstrumentDescription)
					mXRawFile.GetInstSerialNumber(.InstSerialNumber)

					mXRawFile.GetVersionNumber(.VersionNumber)
					mXRawFile.GetMassResolution(.MassResolution)

					mXRawFile.GetFirstSpectrumNumber(.ScanStart)
					mXRawFile.GetLastSpectrumNumber(.ScanEnd)

					.AcquisitionDate = Nothing
					.AcquisitionFilename = Nothing
					.Comment1 = Nothing
					.Comment2 = Nothing
					.SampleName = Nothing
					.SampleComment = Nothing

					' Note that the following are typically blank
					mXRawFile.GetAcquisitionDate(.AcquisitionDate)
					mXRawFile.GetAcquisitionFileName(.AcquisitionFilename)
					mXRawFile.GetComment1(.Comment1)
					mXRawFile.GetComment2(.Comment2)
					mXRawFile.GetSeqRowSampleName(.SampleName)
					mXRawFile.GetSeqRowComment(.SampleComment)

					If Not mLoadMSTuneInfo Then
						ReDim .TuneMethods(-1)
					Else
						' Note that intTuneMethodCount is set to 0, but we initially reserve space for intNumTuneData methods
						intTuneMethodCountValid = 0
						mXRawFile.GetNumTuneData(intNumTuneData)
						ReDim .TuneMethods(intNumTuneData - 1)

						For intIndex = 0 To intNumTuneData - 1
							intTuneLabelCount = 0
							objLabels = Nothing
							objValues = Nothing

							Try
								If Not mCorruptMemoryEncountered Then
									mXRawFile.GetTuneData(intIndex, objLabels, objValues, intTuneLabelCount)
								End If
							Catch ex As System.Exception
								' Exception getting TuneData
								strWarningMessage = "Warning: Exception calling mXRawFile.GetTuneData for Index " & intIndex.ToString & ": " & ex.Message
								RaiseWarningMessage(strWarningMessage)
								intTuneLabelCount = 0

								If ex.Message.ToLower.Contains("memory is corrupt") Then
									intNumTuneData = 0
									mCorruptMemoryEncountered = True
									Exit For
								End If
							End Try


							If intTuneLabelCount > 0 Then
								strWarningMessage = String.Empty
								If objLabels Is Nothing Then
									' .GetTuneData returned a non-zero count, but no parameter names; unable to continue
									strWarningMessage = "Warning: the GetTuneData function returned a positive tune parameter count but no parameter names"
								ElseIf objValues Is Nothing Then
									' .GetTuneData returned parameter names, but objValues is nothing; unable to continue
									strWarningMessage = "Warning: the GetTuneData function returned tune parameter names but no tune values"
								End If

								If strWarningMessage.Length > 0 Then
									strWarningMessage &= " (Tune Method " & (intIndex + 1).ToString & ")"
									RaiseWarningMessage(strWarningMessage)
									intTuneLabelCount = 0
								End If

							End If

							If intTuneLabelCount > 0 Then
								If intTuneMethodCountValid >= .TuneMethods.Length Then
									ReDim Preserve .TuneMethods(.TuneMethods.Length * 2 - 1)
								End If

								With .TuneMethods(intTuneMethodCountValid)

									' Note that .Count is initially 0, but we reserve space for intTuneLabelCount settings
									.Count = 0
									ReDim .SettingCategory(intTuneLabelCount - 1)
									ReDim .SettingName(intTuneLabelCount - 1)
									ReDim .SettingValue(intTuneLabelCount - 1)

									If intTuneLabelCount > 0 Then

										strTuneSettingNames = CType(objLabels, String())
										strTuneSettingValues = CType(objValues, String())

										' Step through the names and store in the .Setting() arrays
										strTuneCategory = "General"
										For intSettingIndex = 0 To intTuneLabelCount - 1
											If strTuneSettingValues(intSettingIndex).Length = 0 AndAlso _
											Not strTuneSettingNames(intSettingIndex).EndsWith(":") Then
												' New category
												If strTuneSettingNames(intSettingIndex).Length > 0 Then
													strTuneCategory = String.Copy(strTuneSettingNames(intSettingIndex))
												Else
													strTuneCategory = "General"
												End If
											Else
												.SettingCategory(.Count) = String.Copy(strTuneCategory)
												.SettingName(.Count) = strTuneSettingNames(intSettingIndex).TrimEnd(":"c)
												.SettingValue(.Count) = String.Copy(strTuneSettingValues(intSettingIndex))

												.Count += 1
											End If

										Next intSettingIndex

										If .Count < .SettingName.Length Then
											ReDim Preserve .SettingCategory(.Count - 1)
											ReDim Preserve .SettingName(.Count - 1)
											ReDim Preserve .SettingValue(.Count - 1)
										End If
									End If
								End With
								intTuneMethodCountValid += 1

								If intTuneMethodCountValid > 1 Then
									' Compare this tune method to the previous one; if identical, then don't keep it
									If TuneMethodsMatch(.TuneMethods(intTuneMethodCountValid - 2), .TuneMethods(intTuneMethodCountValid - 1)) Then
										intTuneMethodCountValid -= 1
									End If
								End If
							End If

						Next intIndex

						If .TuneMethods.Length <> intTuneMethodCountValid Then
							ReDim Preserve .TuneMethods(intTuneMethodCountValid - 1)
						End If
					End If

				End With


			Catch ex As System.Exception
				Dim strError As String = "Error: Exception in FillFileInfo: " & ex.Message
				RaiseErrorMessage(strError)
				Return False
			End Try

			Return True

		End Function

		Public Function GetCollisionEnergy(ByVal Scan As Integer) As System.Collections.Generic.List(Of Double)

			Dim intNumMSOrders As Integer
			Dim lstCollisionEnergies As System.Collections.Generic.List(Of Double) = New System.Collections.Generic.List(Of Double)
			Dim dblCollisionEnergy As Double

			Try
				If mXRawFile Is Nothing Then Return lstCollisionEnergies

				mXRawFile.GetNumberOfMSOrdersFromScanNum(Scan, intNumMSOrders)

				For intMSOrder As Integer = 1 To intNumMSOrders
					dblCollisionEnergy = 0
					mXRawFile.GetCollisionEnergyForScanNum(Scan, intMSOrder, dblCollisionEnergy)

					If (dblCollisionEnergy > 0) Then
						lstCollisionEnergies.Add(dblCollisionEnergy)
					End If
				Next

			Catch ex As System.Exception
				Dim strError As String = "Error: Exception in GetCollisionEnergy: " & ex.Message
				RaiseErrorMessage(strError)
			End Try

			Return lstCollisionEnergies

		End Function

		Public Overrides Function GetNumScans() As Integer
			' Returns the number of scans, or -1 if an error

			Dim intResult As Integer
			Dim intScanCount As Integer

			Try
				If mXRawFile Is Nothing Then Return -1

				mXRawFile.GetNumSpectra(intScanCount)
				mXRawFile.IsError(intResult)			' Unfortunately, .IsError() always returns 0, even if an error occurred
				If intResult = 0 Then
					Return intScanCount
				Else
					Return -1
				End If
			Catch ex As System.Exception
				Return -1
			End Try

		End Function

		Public Overrides Function GetScanInfo(ByVal Scan As Integer, <Out()> ByRef udtScanHeaderInfo As udtScanHeaderInfoType) As Boolean
			' Function returns True if no error, False if an error

			Dim intResult As Integer

			Dim objLabels As Object
			Dim objValues As Object

			Dim intArrayCount As Integer
			Dim intIndex As Integer

			Dim strFilterText As String

			Dim dblStatusLogRT As Double
			Dim intBooleanVal As Integer

			Dim dblParentIonMZ As Double
			Dim intMSLevel As Integer
			Dim strCollisionMode As String = String.Empty

			Dim strWarningMessage As String

			Try
				If mXRawFile Is Nothing Then Return False

				If Scan < mFileInfo.ScanStart Then
					Scan = mFileInfo.ScanStart
				ElseIf Scan > mFileInfo.ScanEnd Then
					Scan = mFileInfo.ScanEnd
				End If

				' Make sure the MS controller is selected
				If Not SetMSController() Then Return False

				udtScanHeaderInfo = New udtScanHeaderInfoType
				With udtScanHeaderInfo
					' Reset the values
					.NumPeaks = 0
					.TotalIonCurrent = 0
					.SIMScan = False
					.MRMScanType = FinniganFileReaderBaseClass.MRMScanTypeConstants.NotMRM
					.ZoomScan = False
					.CollisionMode = String.Empty
					.FilterText = String.Empty
					.IonMode = IonModeConstants.Unknown

					mXRawFile.GetScanHeaderInfoForScanNum(Scan, .NumPeaks, .RetentionTime, .LowMass, .HighMass, .TotalIonCurrent, .BasePeakMZ, .BasePeakIntensity, .NumChannels, intBooleanVal, .Frequency)
					mXRawFile.IsError(intResult)		' Unfortunately, .IsError() always returns 0, even if an error occurred

					If intResult = 0 Then
						.UniformTime = CBool(intBooleanVal)

						intBooleanVal = 0
						mXRawFile.IsCentroidScanForScanNum(Scan, intBooleanVal)
						.IsCentroidScan = CBool(intBooleanVal)

						intArrayCount = 0
						objLabels = Nothing
						objValues = Nothing

						Try
							If Not mCorruptMemoryEncountered Then
								' Retrieve the additional parameters for this scan (including Scan Event)
								mXRawFile.GetTrailerExtraForScanNum(Scan, objLabels, objValues, intArrayCount)
							End If
						Catch ex As System.Exception
							strWarningMessage = "Warning: Exception calling mXRawFile.GetTrailerExtraForScanNum for scan " & Scan & ": " & ex.Message
							RaiseWarningMessage(strWarningMessage)
							intArrayCount = 0

							If ex.Message.ToLower.Contains("memory is corrupt") Then
								mCorruptMemoryEncountered = True
							End If
						End Try

						.EventNumber = 1
						If intArrayCount > 0 Then
							.ScanEventNames = CType(objLabels, String())
							.ScanEventValues = CType(objValues, String())

							' Look for the entry in strLabels named "Scan Event:"
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

							For intIndex = 0 To intArrayCount - 1
								If .ScanEventNames(intIndex).ToLower.StartsWith("scan event") Then
									Try
										.EventNumber = CInt(.ScanEventValues(intIndex))
									Catch ex As System.Exception
										.EventNumber = 1
									End Try
									Exit For
								End If
							Next intIndex
						Else
							ReDim .ScanEventNames(-1)
							ReDim .ScanEventValues(-1)
						End If

						' Lookup the filter text for this scan
						' Parse out the parent ion m/z for fragmentation scans
						' Must set strFilterText to Nothing prior to calling .GetFilterForScanNum()
						strFilterText = Nothing
						mXRawFile.GetFilterForScanNum(Scan, strFilterText)

						.FilterText = String.Copy(strFilterText)
						If String.IsNullOrWhiteSpace(.FilterText) Then .FilterText = String.Empty

						If .EventNumber <= 1 Then
							' XRaw periodically mislabels a scan as .EventNumber = 1 when it's really an MS/MS scan; check for this
							If ExtractMSLevel(.FilterText, intMSLevel, "") Then
								.EventNumber = intMSLevel
							End If
						End If

						If .EventNumber > 1 Then
							' MS/MS data
							udtScanHeaderInfo.MSLevel = 2

							If String.IsNullOrWhiteSpace(.FilterText) Then
								' FilterText is empty; this indicates a problem with the .Raw file
								' This is rare, but does happen (see scans 2 and 3 in QC_Shew_08_03_pt5_1_MAXPRO_27Oct08_Raptor_08-01-01.raw)
								' We'll set the Parent Ion to 0 m/z and the collision mode to CID
								.ParentIonMZ = 0
								.CollisionMode = "cid"
								.MRMScanType = MRMScanTypeConstants.NotMRM
							Else

								' Parse out the parent ion and collision energy from .FilterText
								If ExtractParentIonMZFromFilterText(.FilterText, dblParentIonMZ, intMSLevel, strCollisionMode) Then
									.ParentIonMZ = dblParentIonMZ
									.CollisionMode = strCollisionMode

									If intMSLevel > 2 Then
										udtScanHeaderInfo.MSLevel = intMSLevel
									End If

									' Check whether this is an SRM MS2 scan
									.MRMScanType = DetermineMRMScanType(.FilterText)
								Else
									' Could not find "Full ms2" in .FilterText
									' XRaw periodically mislabels a scan as .EventNumber > 1 when it's really an MS scan; check for this
									If ValidateMSScan(.FilterText, .MSLevel, .SIMScan, .MRMScanType, .ZoomScan) Then
										' Yes, scan is an MS, SIM, or MRMQMS, or SRM scan
									Else
										' Unknown format for .FilterText; return an error
										RaiseErrorMessage("Unknown format for Scan Filter: " & .FilterText)
										Return False
									End If
								End If
							End If
						Else
							' MS data
							' Make sure .FilterText contains one of the following:
							'   FULL_MS_TEXT = "Full ms "
							'   FULL_PR_TEXT = "Full pr "
							'   SIM_MS_TEXT = "SIM ms "
							'   MRM_Q1MS_TEXT = "Q1MS "
							'   MRM_SRM_TEXT = "SRM "

							If .FilterText = String.Empty Then
								' FilterText is empty; this indicates a problem with the .Raw file
								' This is rare, but does happen (see scans 2 and 3 in QC_Shew_08_03_pt5_1_MAXPRO_27Oct08_Raptor_08-01-01.raw)
								.MSLevel = 1
								.SIMScan = False
								.MRMScanType = MRMScanTypeConstants.NotMRM
							Else

								If ValidateMSScan(.FilterText, .MSLevel, .SIMScan, .MRMScanType, .ZoomScan) Then
									' Yes, scan is an MS, SIM, or MRMQMS, or SRM scan
								Else
									' Unknown format for .FilterText; return an error
									RaiseErrorMessage("Unknown format for Scan Filter: " & .FilterText)
									Return False
								End If
							End If

						End If

						.IonMode = DetermineIonizationMode(.FilterText)

						If Not .MRMScanType = FinniganFileReaderBaseClass.MRMScanTypeConstants.NotMRM Then
							' Parse out the MRM_QMS or SRM information for this scan
							InitializeMRMInfo(.MRMInfo, 1)
							ExtractMRMMasses(.FilterText, .MRMScanType, .MRMInfo)
						Else
							InitializeMRMInfo(.MRMInfo, 0)
						End If

						' Retrieve the Status Log for this scan using the following
						' The Status Log includes numerous instrument parameters, including voltages, temperatures, pressures, turbo pump speeds, etc. 
						intArrayCount = 0
						objLabels = Nothing
						objValues = Nothing

						Try
							If Not mCorruptMemoryEncountered Then
								mXRawFile.GetStatusLogForScanNum(Scan, dblStatusLogRT, objLabels, objValues, intArrayCount)
							End If
						Catch ex As System.Exception
							strWarningMessage = "Warning: Exception calling mXRawFile.GetStatusLogForScanNum for scan " & Scan & ": " & ex.Message
							RaiseWarningMessage(strWarningMessage)
							intArrayCount = 0

							If ex.Message.ToLower.Contains("memory is corrupt") Then
								mCorruptMemoryEncountered = True
							End If
						End Try

						If intArrayCount > 0 Then
							.StatusLogNames = CType(objLabels, String())
							.StatusLogValues = CType(objValues, String())
						Else
							ReDim .StatusLogNames(-1)
							ReDim .StatusLogValues(-1)
						End If

					End If
				End With

			Catch ex As System.Exception
				Dim strError As String = "Error: Exception in GetScanInfo: " & ex.Message
				RaiseWarningMessage(strError)
				Return False
			End Try

			Return True

		End Function

		Public Shared Function GetScanTypeNameFromFinniganScanFilterText(ByVal strFilterText As String) As String

			' Examines strFilterText to determine what the scan type is
			' Examples:
			' Given                                                                ScanTypeName
			' ITMS + c ESI Full ms [300.00-2000.00]                                MS
			' FTMS + p NSI Full ms [400.00-2000.00]                                HMS
			' ITMS + p ESI d Z ms [579.00-589.00]                                  Zoom-MS
			' ITMS + c ESI d Full ms2 583.26@cid35.00 [150.00-1180.00]             CID-MSn
			' ITMS + c NSI d Full ms2 606.30@pqd27.00 [50.00-2000.00]              PQD-MSn
			' FTMS + c NSI d Full ms2 516.03@hcd40.00 [100.00-2000.00]             HCD-HMSn
			' ITMS + c NSI d sa Full ms2 516.03@etd100.00 [50.00-2000.00]          ETD-MSn

			' FTMS + p NSI d Full msx ms2 712.85@hcd28.00 407.92@hcd28.00  [100.00-1475.00]         HCD-HMSn using multiplexed MSn (introduced with the Q-Exactive)

			' + c d Full ms2 1312.95@45.00 [ 350.00-2000.00]                       MSn
			' + c d Full ms3 1312.95@45.00 873.85@45.00 [ 350.00-2000.00]          MSn
			' ITMS + c NSI d Full ms10 421.76@35.00                                MSn
			' ITMS + p NSI CRM ms3 332.14@cid35.00 288.10@cid35.00 [242.00-248.00, 285.00-291.00]  MS3

			' + p ms2 777.00@cid30.00 [210.00-1200.00]                             CID-MSn
			' + c NSI SRM ms2 501.560@cid15.00 [507.259-507.261, 635-319-635.32]   CID-SRM
			' + p NSI Q1MS [179.652-184.582, 505.778-510.708, 994.968-999.898]     Q1MS
			' + p NSI Q3MS [150.070-1500.000]                                      Q3MS
			' c NSI Full cnl 162.053 [300.000-1200.000]                            MRM_Full_NL

			Dim strScanTypeName As String = "MS"
			Dim intMSLevel As Integer
			Dim dblParentIonMZ As Double
			Dim strCollisionMode As String
			Dim eMRMScanType As MRMScanTypeConstants

			Dim blnSIMScan As Boolean
			Dim blnZoomScan As Boolean

			Dim blnValidScanFilter As Boolean

			Try
				blnValidScanFilter = True
				intMSLevel = 1
				strCollisionMode = ""
				eMRMScanType = MRMScanTypeConstants.NotMRM
				blnSIMScan = False
				blnZoomScan = False

				If strFilterText.Length = 0 Then
					strScanTypeName = "MS"
					Exit Try
				End If

				If Not ExtractMSLevel(strFilterText, intMSLevel, "") Then
					' Assume this is an MS scan
					intMSLevel = 1
				End If

				If intMSLevel > 1 Then
					' Parse out the parent ion and collision energy from strFilterText
					If ExtractParentIonMZFromFilterText(strFilterText, dblParentIonMZ, intMSLevel, strCollisionMode) Then

						' Check whether this is an SRM MS2 scan
						eMRMScanType = DetermineMRMScanType(strFilterText)
					Else
						' Could not find "Full ms2" in strFilterText
						' XRaw periodically mislabels a scan as .EventNumber > 1 when it's really an MS scan; check for this
						If ValidateMSScan(strFilterText, intMSLevel, blnSIMScan, eMRMScanType, blnZoomScan) Then
							' Yes, scan is an MS, SIM, or MRMQMS, or SRM scan
						Else
							' Unknown format for strFilterText; return an error
							blnValidScanFilter = False
						End If
					End If

				Else
					' Make sure strFilterText contains one of the following:
					'   FULL_MS_TEXT = "Full ms "
					'   FULL_PR_TEXT = "Full pr "
					'   SIM_MS_TEXT = "SIM ms "
					'   MRM_Q1MS_TEXT = "Q1MS "
					'   MRM_SRM_TEXT = "SRM "
					If ValidateMSScan(strFilterText, intMSLevel, blnSIMScan, eMRMScanType, blnZoomScan) Then
						' Yes, scan is an MS, SIM, or MRMQMS, or SRM scan
					Else
						' Unknown format for strFilterText; return an error
						blnValidScanFilter = False
					End If
				End If


				If blnValidScanFilter Then
					If eMRMScanType = MRMScanTypeConstants.NotMRM Then
						If blnSIMScan Then
							strScanTypeName = SIM_MS_TEXT.Trim
						ElseIf blnZoomScan Then
							strScanTypeName = "Zoom-MS"
						Else

							' Normal, plain MS or MSn scan

							If intMSLevel > 1 Then
								strScanTypeName = "MSn"
							Else
								strScanTypeName = "MS"
							End If

							If strFilterText.ToUpper.IndexOf("FTMS") >= 0 Then
								' HMS or HMSn scan
								strScanTypeName = "H" & strScanTypeName
							End If

							If intMSLevel > 1 AndAlso strCollisionMode.Length > 0 Then
								strScanTypeName = strCollisionMode.ToUpper() & "-" & strScanTypeName
							End If

						End If
					Else
						' This is an MRM or SRM scan

						Select Case eMRMScanType
							Case MRMScanTypeConstants.MRMQMS
								If strFilterText.ToLower.IndexOf(MRM_Q1MS_TEXT.ToLower) > 0 Then
									strScanTypeName = MRM_Q1MS_TEXT.Trim

								ElseIf strFilterText.ToLower.IndexOf(MRM_Q3MS_TEXT.ToLower) > 0 Then
									strScanTypeName = MRM_Q3MS_TEXT.Trim
								Else
									' Unknown QMS mode
									strScanTypeName = "MRM QMS"
								End If

							Case MRMScanTypeConstants.SRM
								If strCollisionMode.Length > 0 Then
									strScanTypeName = strCollisionMode.ToUpper() & "-SRM"
								Else
									strScanTypeName = "CID-SRM"
								End If


							Case MRMScanTypeConstants.FullNL
								strScanTypeName = "MRM_Full_NL"

							Case Else
								strScanTypeName = "MRM"
						End Select

					End If


				End If

			Catch ex As System.Exception

			End Try

			Return strScanTypeName

		End Function


		Public Shared Function MakeGenericFinniganScanFilter(ByVal strFilterText As String) As String

			' Will make a generic version of the FilterText in strFilterText
			' Examples:
			' From                                                                 To
			' ITMS + c ESI Full ms [300.00-2000.00]                                ITMS + c ESI Full ms
			' FTMS + p NSI Full ms [400.00-2000.00]                                FTMS + p NSI Full ms
			' ITMS + p ESI d Z ms [579.00-589.00]                                  ITMS + p ESI d Z ms
			' ITMS + c ESI d Full ms2 583.26@cid35.00 [150.00-1180.00]             ITMS + c ESI d Full ms2 0@cid35.00
			' ITMS + c NSI d Full ms2 606.30@pqd27.00 [50.00-2000.00]              ITMS + c NSI d Full ms2 0@pqd27.00
			' FTMS + c NSI d Full ms2 516.03@hcd40.00 [100.00-2000.00]             FTMS + c NSI d Full ms2 0@hcd40.00
			' ITMS + c NSI d sa Full ms2 516.03@etd100.00 [50.00-2000.00]          ITMS + c NSI d sa Full ms2 0@etd100.00

			' + c d Full ms2 1312.95@45.00 [ 350.00-2000.00]                       + c d Full ms2 0@45.00
			' + c d Full ms3 1312.95@45.00 873.85@45.00 [ 350.00-2000.00]          + c d Full ms3 0@45.00 0@45.00
			' ITMS + c NSI d Full ms10 421.76@35.00                                ITMS + c NSI d Full ms10 0@35.00

			' + p ms2 777.00@cid30.00 [210.00-1200.00]                             + p ms2 0@cid30.00
			' + c NSI SRM ms2 501.560@cid15.00 [507.259-507.261, 635-319-635.32]   + c NSI SRM ms2 0@cid15.00
			' + p NSI Q1MS [179.652-184.582, 505.778-510.708, 994.968-999.898]     + p NSI Q1MS
			' + p NSI Q3MS [150.070-1500.000]                                      + p NSI Q3MS
			' c NSI Full cnl 162.053 [300.000-1200.000]                            c NSI Full cnl

			Const COLLISION_SPEC_REGEX As String = "[0-9.]+@"


			Dim strGenericScanFilterText As String = "MS"
			Dim intCharIndex As Integer

			Dim reCollisionSpecs As System.Text.RegularExpressions.Regex

			Try
				If String.IsNullOrWhiteSpace(strFilterText) Then Exit Try

				strGenericScanFilterText = String.Copy(strFilterText)

				' First look for and remove numbers between square brackets
				intCharIndex = strGenericScanFilterText.IndexOf("["c)
				If intCharIndex > 0 Then
					strGenericScanFilterText = strGenericScanFilterText.Substring(0, intCharIndex).TrimEnd(" "c)
				End If

				intCharIndex = strGenericScanFilterText.ToLower.IndexOf(MRM_FullNL_TEXT.ToLower)
				If intCharIndex > 0 Then
					' MRM neutral loss
					' Remove any text after MRM_FullNL_TEXT
					strGenericScanFilterText = strGenericScanFilterText.Substring(0, intCharIndex + MRM_FullNL_TEXT.Length).Trim
					Exit Try
				End If

				' Replace any digits before any @ sign with a 0
				If strGenericScanFilterText.IndexOf("@"c) > 0 Then
					reCollisionSpecs = New System.Text.RegularExpressions.Regex(COLLISION_SPEC_REGEX, Text.RegularExpressions.RegexOptions.Compiled)

					strGenericScanFilterText = reCollisionSpecs.Replace(strGenericScanFilterText, "0@")

					''reMatch = reCollisionSpecs.Match(strGenericScanFilterText)
					''If Not reMatch Is Nothing Then

					''    Do While reMatch.Success
					''        Try
					''            ' Note that group 0 is the full mass range (two mass values, separated by a dash)
					''            ' Group 1 is the first mass value
					''            ' Group 2 is the second mass value

					''            If reMatch.Groups.Count >= 3 Then
					''                If udtMRMInfo.MRMMassCount = udtMRMInfo.MRMMassList.Length Then
					''                    ' Need to reserve more room
					''                    ReDim Preserve udtMRMInfo.MRMMassList(udtMRMInfo.MRMMassList.Length * 2 - 1)
					''                End If

					''                With udtMRMInfo.MRMMassList(udtMRMInfo.MRMMassCount)
					''                    .StartMass = Double.Parse(reMatch.Groups(1).Value)
					''                    .EndMass = Double.Parse(reMatch.Groups(2).Value)
					''                    .CentralMass = Math.Round(.StartMass + (.EndMass - .StartMass) / 2, 6)
					''                End With
					''                udtMRMInfo.MRMMassCount += 1
					''            End If

					''        Catch ex As System.Exception
					''            ' Error parsing out the mass values; skip this group
					''        End Try

					''        reMatch = reMatch.NextMatch
					''    Loop
					''End If
				End If

			Catch ex As System.Exception
				' Ignore errors
			End Try

			Return strGenericScanFilterText

		End Function

		Private Function SetMSController() As Boolean
			' A controller is typically the MS, UV, analog, etc.
			' See ControllerTypeConstants

			Dim intResult As Integer

			mXRawFile.SetCurrentController(ControllerTypeConstants.MS, 1)
			mXRawFile.IsError(intResult)		' Unfortunately, .IsError() always returns 0, even if an error occurred

			If intResult = 0 Then
				Return True
			Else
				Return False
			End If

		End Function

		''' <summary>
		''' Examines strFilterText to validate that it is a supported scan type
		''' </summary>
		''' <param name="strFilterText"></param>
		''' <param name="intMSLevel"></param>
		''' <param name="blnSIMScan"></param>
		''' <param name="eMRMScanType"></param>
		''' <param name="blnZoomScan"></param>
		''' <returns>True if strFilterText contains a known MS scan type</returns>
		''' <remarks></remarks>
		Public Shared Function ValidateMSScan(ByVal strFilterText As String, _
		   <Out()> ByRef intMSLevel As Integer, _
		   <Out()> ByRef blnSIMScan As Boolean, _
		   <Out()> ByRef eMRMScanType As MRMScanTypeConstants, _
		   <Out()> ByRef blnZoomScan As Boolean) As Boolean

			Dim blnValidScan As Boolean

			intMSLevel = 0
			blnSIMScan = False
			eMRMScanType = MRMScanTypeConstants.NotMRM
			blnZoomScan = False

			If strFilterText.ToLower.IndexOf(FULL_MS_TEXT.ToLower) > 0 OrElse _
			 strFilterText.ToLower.IndexOf(MS_ONLY_C_TEXT.ToLower) > 0 OrElse _
			 strFilterText.ToLower.IndexOf(MS_ONLY_P_TEXT.ToLower) > 0 OrElse _
			 strFilterText.ToLower.IndexOf(FULL_PR_TEXT.ToLower) > 0 Then
				' This is really a Full MS scan
				intMSLevel = 1
				blnSIMScan = False
				blnValidScan = True
			Else
				If strFilterText.ToLower.IndexOf(SIM_MS_TEXT.ToLower) > 0 Then
					' This is really a SIM MS scan
					intMSLevel = 1
					blnSIMScan = True
					blnValidScan = True
				ElseIf strFilterText.ToLower.IndexOf(MS_ONLY_Z_TEXT.ToLower) > 0 OrElse _
				 strFilterText.ToLower.IndexOf(MS_ONLY_PZ_TEXT.ToLower) > 0 OrElse _
				 strFilterText.ToLower.IndexOf(MS_ONLY_DZ_TEXT.ToLower) > 0 Then
					intMSLevel = 1
					blnZoomScan = True
					blnValidScan = True
				ElseIf strFilterText.ToLower.IndexOf(MS_ONLY_PZ_MS2_TEXT.ToLower) > 0 Then
					' Technically, this should have MSLevel = 2, but that would cause a bunch of problems elsewhere in MASIC
					' Thus, we'll pretend it's MS1
					intMSLevel = 1
					blnZoomScan = True
					blnValidScan = True
				Else
					eMRMScanType = DetermineMRMScanType(strFilterText)
					Select Case eMRMScanType
						Case FinniganFileReaderBaseClass.MRMScanTypeConstants.MRMQMS
							intMSLevel = 1
							blnValidScan = True			   ' ToDo: Add support for TSQ MRMQMS data
						Case FinniganFileReaderBaseClass.MRMScanTypeConstants.SRM
							intMSLevel = 2
							blnValidScan = True			   ' ToDo: Add support for TSQ SRM data
						Case MRMScanTypeConstants.FullNL
							intMSLevel = 2
							blnValidScan = True			   ' ToDo: Add support for TSQ Full NL data
						Case Else
							blnValidScan = False
					End Select
				End If
			End If

			Return blnValidScan
		End Function

		Public Overloads Overrides Function GetScanData(ByVal Scan As Integer, ByRef dblMZList() As Double, ByRef dblIntensityList() As Double, ByRef udtScanHeaderInfo As udtScanHeaderInfoType) As Integer
			' Return all data points by passing 0 for intMaxNumberOfPeaks
			Return GetScanData(Scan, dblMZList, dblIntensityList, udtScanHeaderInfo, 0)
		End Function

		Public Overloads Overrides Function GetScanData(ByVal Scan As Integer, ByRef dblMZList() As Double, ByRef dblIntensityList() As Double, ByRef udtScanHeaderInfo As udtScanHeaderInfoType, ByVal intMaxNumberOfPeaks As Integer) As Integer
			' Returns the number of data points, or -1 if an error
			' If intMaxNumberOfPeaks is <=0, then returns all data; set intMaxNumberOfPeaks to > 0 to limit the number of data points returned

			Dim intDataCount As Integer

			Dim strFilter As String
			Dim intIntensityCutoffValue As Integer
			Dim intCentroidResult As Integer
			Dim dblCentroidPeakWidth As Double

			Dim MassIntensityPairsList As Object = Nothing
			Dim PeakList As Object = Nothing

			Dim dblData(,) As Double

			Dim intIndex As Integer

			intDataCount = 0

			Try
				If mXRawFile Is Nothing Then
					intDataCount = -1
					Exit Try
				End If

				' Make sure the MS controller is selected
				If Not SetMSController() Then
					intDataCount = -1
					Exit Try
				End If

				If Scan < mFileInfo.ScanStart Then
					Scan = mFileInfo.ScanStart
				ElseIf Scan > mFileInfo.ScanEnd Then
					Scan = mFileInfo.ScanEnd
				End If

				strFilter = String.Empty			' Could use this to filter the data returned from the scan; must use one of the filters defined in the file (see .GetFilters())
				intIntensityCutoffValue = 0

				If intMaxNumberOfPeaks < 0 Then intMaxNumberOfPeaks = 0
				intCentroidResult = 0			' Set to 1 to indicate that peaks should be centroided (only appropriate for profile data)

				mXRawFile.GetMassListFromScanNum(Scan, strFilter, IntensityCutoffTypeConstants.None, _
				   intIntensityCutoffValue, intMaxNumberOfPeaks, intCentroidResult, dblCentroidPeakWidth, _
				   MassIntensityPairsList, PeakList, intDataCount)

				If intDataCount > 0 Then
					dblData = CType(MassIntensityPairsList, Double(,))

					If dblData.GetUpperBound(1) + 1 < intDataCount Then
						intDataCount = dblData.GetUpperBound(1) + 1
					End If

					ReDim dblMZList(intDataCount - 1)
					ReDim dblIntensityList(intDataCount - 1)

					For intIndex = 0 To intDataCount - 1
						dblMZList(intIndex) = dblData(0, intIndex)
						dblIntensityList(intIndex) = dblData(1, intIndex)
					Next intIndex

				End If

			Catch
				intDataCount = -1
			End Try

			If intDataCount <= 0 Then
				ReDim dblMZList(-1)
				ReDim dblIntensityList(-1)
			End If

			Return intDataCount

		End Function

		Public Shared Sub InitializeMRMInfo(<Out()> ByRef udtMRMInfo As udtMRMInfoType, ByVal intInitialMassCountCapacity As Integer)

			If intInitialMassCountCapacity < 0 Then
				intInitialMassCountCapacity = 0
			End If

			udtMRMInfo = New udtMRMInfoType
			With udtMRMInfo
				.MRMMassCount = 0
				ReDim .MRMMassList(intInitialMassCountCapacity - 1)
			End With
		End Sub

		Public Overrides Function OpenRawFile(ByVal FileName As String) As Boolean
			Dim intResult As Integer
			Dim blnSuccess As Boolean

			Try

				' Make sure any existing open files are closed
				CloseRawFile()

				If mXRawFile Is Nothing Then
					mXRawFile = CType(New MSFileReaderLib.MSFileReader_XRawfile, MSFileReaderLib.IXRawfile5)
				End If

				mXRawFile.Open(FileName)
				mXRawFile.IsError(intResult)		' Unfortunately, .IsError() always returns 0, even if an error occurred

				If intResult = 0 Then
					mCachedFileName = FileName
					If FillFileInfo() Then
						With mFileInfo
							If .ScanStart = 0 AndAlso .ScanEnd = 0 AndAlso .VersionNumber = 0 AndAlso .MassResolution = 0 AndAlso .InstModel = Nothing Then
								' File actually didn't load correctly, since these shouldn't all be blank
								blnSuccess = False
							Else
								blnSuccess = True
							End If
						End With
					Else
						blnSuccess = False
					End If
				Else
					blnSuccess = False
				End If

			Catch ex As System.Exception
				blnSuccess = False
			Finally
				If Not blnSuccess Then
					mCachedFileName = String.Empty
				End If
			End Try

			Return blnSuccess

		End Function

		Private Function TuneMethodsMatch(ByVal udtMethod1 As udtTuneMethodType, ByVal udtMethod2 As udtTuneMethodType) As Boolean
			Dim blnMatch As Boolean
			Dim intIndex As Integer

			blnMatch = True

			With udtMethod1
				If .Count <> udtMethod2.Count Then
					' Different segment number of setting count; the methods don't match
					blnMatch = False
				Else
					For intIndex = 0 To .Count - 1
						If .SettingCategory(intIndex) <> udtMethod2.SettingCategory(intIndex) OrElse _
						   .SettingName(intIndex) <> udtMethod2.SettingName(intIndex) OrElse _
						   .SettingValue(intIndex) <> udtMethod2.SettingValue(intIndex) Then
							' Different segment data; the methods don't match
							blnMatch = False
							Exit For
						End If
					Next intIndex
				End If
			End With

			Return blnMatch

		End Function

		Public Sub New()
			CloseRawFile()
		End Sub

	End Class
End Namespace
