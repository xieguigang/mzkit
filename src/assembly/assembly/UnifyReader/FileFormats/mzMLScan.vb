#Region "Microsoft.VisualBasic::f77c9177b354a640c8c8d59647464bf5, src\assembly\assembly\UnifyReader\FileFormats\mzMLScan.vb"

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

    '     Class mzMLScan
    ' 
    '         Function: GetActivationMethod, GetBPC, GetCentroided, GetCharge, GetCollisionEnergy
    '                   GetMsLevel, GetMsMs, GetParentMz, GetPolarity, GetScanId
    '                   GetScanTime, GetTIC, IsEmpty
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace DataReader

    Public Class mzMLScan : Inherits MsDataReader(Of spectrum)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetScanTime(scan As spectrum) As Double
            Return scan.scan_time
        End Function

        Public Overrides Function GetScanId(scan As spectrum) As String
            Dim scanType As String = scan.scanList.scans(0).cvParams.KeyItem("filter string")?.value
            Dim polarity As String = GetPolarity(scan)

            If scan.ms_level = 1 Then
                Return $"[MS1] {scanType}, ({polarity}) retentionTime={CInt(scan.scan_time)}"
            Else
                Return $"[MS/MS] {scanType}, ({polarity}) M{CInt(scan.selectedIon.mz)}T{CInt(scan.scan_time)}"
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
            Dim charge As cvParam = scan _
                .precursorList _
                .precursor(Scan0) _
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
            Dim active = scan.precursorList.precursor(Scan0).GetActivationMethod
            Dim method As ActivationMethods = [Enum].Parse(GetType(ActivationMethods), active)

            Return method
        End Function

        Public Overrides Function GetCollisionEnergy(scan As spectrum) As Double
            Return scan.precursorList.precursor(Scan0).GetCollisionEnergy
        End Function

        Public Overrides Function GetCentroided(scan As spectrum) As Boolean
            Return Not scan.profile
        End Function
    End Class
End Namespace
