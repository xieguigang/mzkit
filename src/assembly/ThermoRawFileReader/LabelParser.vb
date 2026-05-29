#Region "Microsoft.VisualBasic::f333fe2a19bf552d4b017ad86f31815d, assembly\ThermoRawFileReader\LabelParser.vb"

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

    '   Total Lines: 69
    '    Code Lines: 44 (63.77%)
    ' Comment Lines: 14 (20.29%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (15.94%)
    '     File Size: 5.00 KB


    ' Module LabelParser
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions

Module LabelParser

#Region "Constants"

    ' Note that each of these strings has a space at the end; this is important to avoid matching inappropriate text in the filter string
    Public Const MS_ONLY_C_TEXT As String = " c ms "
    Public Const MS_ONLY_P_TEXT As String = " p ms "
    Public Const MS_ONLY_P_NSI_TEXT As String = " p NSI ms "
    Public Const MS_ONLY_PZ_TEXT As String = " p Z ms "          ' Likely a zoom scan
    Public Const MS_ONLY_DZ_TEXT As String = " d Z ms "          ' Dependent zoom scan
    Public Const MS_ONLY_PZ_MS2_TEXT As String = " d Z ms2 "     ' Dependent MS2 zoom scan
    Public Const MS_ONLY_Z_TEXT As String = " NSI Z ms "         ' Likely a zoom scan
    Public Const FULL_MS_TEXT As String = "Full ms "
    Public Const FULL_PR_TEXT As String = "Full pr "             ' TSQ: Full Parent Scan, Product Mass
    Public Const SIM_MS_TEXT As String = "SIM ms "
    Public Const FULL_LOCK_MS_TEXT As String = "Full lock ms "   ' Lock mass scan
    Public Const MRM_Q1MS_TEXT As String = "Q1MS "
    Public Const MRM_Q3MS_TEXT As String = "Q3MS "
    Public Const MRM_SRM_TEXT As String = "SRM ms2"
    Public Const MRM_FullNL_TEXT As String = "Full cnl "         ' MRM neutral loss; yes, cnl starts with a c
    Public Const MRM_SIM_PR_TEXT As String = "SIM pr "           ' TSQ: Isolated and fragmented parent, monitor multiple product ion ranges; e.g., Biofilm-1000pg-std-mix_06Dec14_Smeagol-3
    Public Const MRM_SIM_MSX_TEXT As String = "SIM msx "         ' Q-Exactive Plus: Isolated and fragmented parent, monitor multiple product ion ranges; e.g., MM_unsorted_10ng_digestionTest_t-SIM_MDX_3_17Mar20_Oak_Jup-20-03-01

    ' This RegEx matches Full ms2, Full ms3, ..., Full ms10, Full ms11, ...
    ' It also matches p ms2
    ' It also matches SRM ms2
    ' It also matches CRM ms3
    ' It also matches Full msx ms2 (multiplexed parent ion selection, introduced with the Q-Exactive)
    Public Const MS2_REGEX As String = "(?<ScanMode> p|Full|SRM|CRM|Full msx) ms(?<MSLevel>[2-9]|[1-9][0-9]) "
    Public Const ION_MODE_REGEX As String = "[+-]"
    Public Const MASS_LIST_REGEX As String = "\[[0-9.]+-[0-9.]+.*\]"
    Public Const MASS_RANGES_REGEX As String = "(?<StartMass>[0-9.]+)-(?<EndMass>[0-9.]+)"

    ' This RegEx matches text like 1312.95@45.00 or 756.98@cid35.00 or 902.5721@etd120.55@cid20.00
    Public Const PARENT_ION_REGEX As String = "(?<ParentMZ>[0-9.]+)@(?<CollisionMode1>[a-z]*)(?<CollisionEnergy1>[0-9.]+)(@(?<CollisionMode2>[a-z]+)(?<CollisionEnergy2>[0-9.]+))?"

    ' This RegEx is used to extract parent ion m/z from a filter string that does not contain msx
    ' ${ParentMZ} will hold the last parent ion m/z found
    ' For example, 756.71 in FTMS + p NSI d Full ms3 850.70@cid35.00 756.71@cid35.00 [195.00-2000.00]
    Public Const PARENT_ION_ONLY_NON_MSX_REGEX As String = "[Mm][Ss]\d*[^\[\r\n]* (?<ParentMZ>[0-9.]+)@?[A-Za-z]*\d*\.?\d*(\[[^\]\r\n]\])?"

    ' This RegEx is used to extract parent ion m/z from a filter string that does contain msx
    ' ${ParentMZ} will hold the first parent ion m/z found (the first parent ion m/z corresponds to the highest peak)
    ' For example, 636.04 in FTMS + p NSI Full msx ms2 636.04@hcd28.00 641.04@hcd28.00 654.05@hcd28.00 [88.00-1355.00]
    Public Const PARENT_ION_ONLY_MSX_REGEX As String = "[Mm][Ss]\d* (?<ParentMZ>[0-9.]+)@?[A-Za-z]*\d*\.?\d*[^\[\r\n]*(\[[^\]\r\n]+\])?"

    ' This RegEx looks for "sa" prior to Full ms"
    Public Const SA_REGEX As String = " sa Full ms"
    Public Const MSX_REGEX As String = " Full msx "
    Public Const COLLISION_SPEC_REGEX As String = "(?<MzValue> [0-9.]+)@"
    Public Const MZ_WITHOUT_COLLISION_ENERGY As String = "ms[2-9](?<MzValue> [0-9.]+)$"

#End Region

    Friend ReadOnly mFindMS As New Regex(MS2_REGEX, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Friend ReadOnly mIonMode As New Regex(ION_MODE_REGEX, RegexOptions.Compiled)
    Friend ReadOnly mMassList As New Regex(MASS_LIST_REGEX, RegexOptions.Compiled)
    Friend ReadOnly mMassRanges As New Regex(MASS_RANGES_REGEX, RegexOptions.Compiled)
    Friend ReadOnly mFindParentIon As New Regex(PARENT_ION_REGEX, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Friend ReadOnly mFindParentIonOnlyNonMsx As New Regex(PARENT_ION_ONLY_NON_MSX_REGEX, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Friend ReadOnly mFindParentIonOnlyMsx As New Regex(PARENT_ION_ONLY_MSX_REGEX, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Friend ReadOnly mFindSAFullMS As New Regex(SA_REGEX, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Friend ReadOnly mFindFullMSx As New Regex(MSX_REGEX, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Friend ReadOnly mCollisionSpecs As New Regex(COLLISION_SPEC_REGEX, RegexOptions.Compiled)
    Friend ReadOnly mMzWithoutCE As New Regex(MZ_WITHOUT_COLLISION_ENERGY, RegexOptions.Compiled)

End Module
