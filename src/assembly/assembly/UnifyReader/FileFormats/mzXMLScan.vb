#Region "Microsoft.VisualBasic::bdf112021270a0a645b50d7db6660ed5, assembly\assembly\UnifyReader\FileFormats\mzXMLScan.vb"

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

    '   Total Lines: 95
    '    Code Lines: 69
    ' Comment Lines: 11
    '   Blank Lines: 15
    '     File Size: 3.79 KB


    '     Class mzXMLScan
    ' 
    '         Function: GetActivationMethod, GetBPC, GetCentroided, GetCharge, GetCollisionEnergy
    '                   GetMsLevel, GetMsMs, GetParentMz, GetPolarity, GetScanId
    '                   GetScanTime, GetTIC, IsEmpty
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace DataReader

    Public Class mzXMLScan : Inherits MsDataReader(Of scan)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetScanTime(scan As scan) As Double
            Return PeakMs2.RtInSecond(scan.retentionTime)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetScanId(scan As scan) As String
            Return scan.getName
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scan"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ## 20210928 when the option compressionType="none"
        ''' the value of compressedLen="0", so can not determine
        ''' that the scan is empty as the compression type
        ''' may be is ``none``.
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function IsEmpty(scan As scan) As Boolean
            Return scan.peaks Is Nothing OrElse scan.peaks.value.StringEmpty
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetMsMs(scan As scan) As ms2()
            Return scan.peaks _
                .ExtractMzI _
                .Where(Function(p) p.intensity > 0) _
                .Select(Function(p)
                            Return New ms2 With {
                                .mz = p.mz,
                                .intensity = p.intensity
                            }
                        End Function) _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetMsLevel(scan As scan) As Integer
            Return scan.msLevel
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetBPC(scan As scan) As Double
            Return scan.basePeakIntensity
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetTIC(scan As scan) As Double
            Return scan.totIonCurrent
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetParentMz(scan As scan) As Double
            Return scan.precursorMz.value
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetPolarity(scan As scan) As String
            Return scan.polarity
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetCharge(scan As scan) As Integer
            Return scan.precursorMz.precursorCharge
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetActivationMethod(scan As scan) As ActivationMethods
            Return [Enum].Parse(GetType(ActivationMethods), scan.precursorMz.activationMethod)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetCollisionEnergy(scan As scan) As Double
            Return Val(scan.collisionEnergy)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetCentroided(scan As scan) As Boolean
            Return scan.centroided = "1"
        End Function
    End Class
End Namespace
