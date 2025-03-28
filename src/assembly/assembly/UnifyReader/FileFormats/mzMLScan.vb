﻿#Region "Microsoft.VisualBasic::698df6ed7cdcc0fd48ecf7150dca8245, assembly\assembly\UnifyReader\FileFormats\mzMLScan.vb"

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

    '   Total Lines: 132
    '    Code Lines: 109 (82.58%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 23 (17.42%)
    '     File Size: 5.49 KB


    '     Class mzMLScan
    ' 
    '         Function: GetActivationMethod, GetBPC, GetCentroided, GetCharge, GetCollisionEnergy
    '                   GetMsLevel, GetMsMs, GetParentMz, GetParentScanNumber, GetPolarity
    '                   GetScanId, GetScanNumber, GetScanTime, GetTIC, IsEmpty
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace DataReader

    Public Class mzMLScan : Inherits MsDataReader(Of spectrum)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetScanTime(scan As spectrum) As Double
            Return scan.scan_time
        End Function

        Public Overrides Function GetScanId(scan As spectrum) As String
            Dim scanParams = scan.scanList.scans(0).cvParams
            Dim scanType As String = If(scanParams Is Nothing, "?", scanParams.KeyItem("filter string")?.value)
            Dim polarity As String = GetPolarity(scan)
            Dim msLevel As String = mzXML.msLevels(CInt(Val(scan.ms_level)))

            If scan.ms_level = 1 Then
                Return $"[MS1] {scanType}, ({polarity}) scan_time={(scan.scan_time / 60).ToString("F2")}min,basepeak={GetBPC(scan)},totalIons={GetTIC(scan)}"
            Else
                Return $"[{msLevel}] {scanType}, ({polarity}) M{CInt(scan.selectedIon.mz)}T{CInt(scan.scan_time)}, {scan.selectedIon.mz.ToString("F4")}@{(scan.scan_time / 60).ToString("F2")}min"
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function IsEmpty(scan As spectrum) As Boolean
            Return Not scan.cvParams.KeyItem(UVScanType) Is Nothing
        End Function

        Public Overrides Function GetMsMs(scan As spectrum) As ms2()
            Dim mz As Double() = scan.ByteArray("m/z array").Base64Decode
            Dim intensity = scan.ByteArray("intensity array").Base64Decode
            Dim msms As ms2() = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = intensity(i)
                            }
                        End Function) _
                .ToArray

            Return msms
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetMsLevel(scan As spectrum) As Integer
            Return CInt(Val(scan.ms_level))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetBPC(scan As spectrum) As Double
            Return Val(scan.cvParams.KeyItem("base peak intensity")?.value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetTIC(scan As spectrum) As Double
            Return Val(scan.cvParams.KeyItem("total ion current")?.value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetParentMz(scan As spectrum) As Double
            Return scan.selectedIon.mz
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetPolarity(scan As spectrum) As String
            If Not scan.cvParams.KeyItem("positive scan") Is Nothing Then
                Return "+"
            Else
                Return "-"
            End If
        End Function

        Public Overrides Function GetCharge(scan As spectrum) As Integer
            Dim precursorList = scan.precursorList

            If precursorList Is Nothing OrElse precursorList.precursor.IsNullOrEmpty Then
                Return 1
            End If

            Dim precursor = precursorList.precursor(Scan0)
            Dim charge As cvParam = precursor _
                .selectedIonList _
                .selectedIon(Scan0) _
                .cvParams _
                .KeyItem("charge state")

            If charge Is Nothing Then
                Return 0
            Else
                Return charge.value
            End If
        End Function

        Public Overrides Function GetActivationMethod(scan As spectrum) As ActivationMethods
            If scan.precursorList Is Nothing Then
                Return ActivationMethods.AnyType
            Else
                Dim active = scan.precursorList.precursor(Scan0).GetActivationMethod
                Dim method As ActivationMethods = [Enum].Parse(GetType(ActivationMethods), active)

                Return method
            End If
        End Function

        Public Overrides Function GetCollisionEnergy(scan As spectrum) As Double
            If scan.precursorList Is Nothing Then
                Return 0
            Else
                Return scan.precursorList.precursor(Scan0).GetCollisionEnergy
            End If
        End Function

        Public Overrides Function GetCentroided(scan As spectrum) As Boolean
            Return Not scan.profile
        End Function

        Public Overrides Function GetScanNumber(scan As spectrum) As String
            Return scan.index
        End Function

        Public Overrides Function GetParentScanNumber(scan As spectrum) As String
            Return Nothing
        End Function
    End Class
End Namespace
