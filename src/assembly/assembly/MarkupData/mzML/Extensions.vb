#Region "Microsoft.VisualBasic::b99b91dbd43e9fe005997bc4fb656b4e, assembly\MarkupData\mzML\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: ByteArray, FilterAllSpectrum, GetAllMs1, GetAllMs2, LoadChromatogramList
    '                   MRMTargetMz, PopulateMS1, PopulateMS2, Ticks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData.mzML

    <HideModuleName>
    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetAllMs1(spectrums As IEnumerable(Of spectrum)) As IEnumerable(Of spectrum)
            Return spectrums.FilterAllSpectrum(msLevel:=1)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetAllMs2(spectrums As IEnumerable(Of spectrum)) As IEnumerable(Of spectrum)
            Return spectrums.FilterAllSpectrum(msLevel:=2)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function FilterAllSpectrum(spectrums As IEnumerable(Of spectrum), msLevel$) As IEnumerable(Of spectrum)
            Return spectrums.Where(Function(s) s.ms_level = msLevel)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function PopulateMS1(filePath As String) As IEnumerable(Of (scan_time#, mz#, intensity#))
            Return filePath _
                .LoadXmlDataSet(Of spectrum)(, xmlns:=indexedmzML.xmlns) _
                .GetAllMs1 _
                .Select(Function(ms1)
                            Dim time# = ms1.scan_time
                            Dim mz = ms1.ByteArray("m/z array").Base64Decode
                            Dim intensity = ms1.ByteArray("intensity array").Base64Decode

                            Return CInt(ms1.defaultArrayLength) _
                                .Sequence _
                                .Select(Function(index)
                                            Return (time, mz(index), intensity(index))
                                        End Function)
                        End Function) _
                .IteratesALL
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function PopulateMS2(filePath As String) As IEnumerable(Of LibraryMatrix)
            Return filePath _
                .LoadUltraLargeXMLDataSet(Of spectrum)(, xmlns:=indexedmzML.xmlns) _
                .GetAllMs2 _
                .Select(Function(ms2)
                            Return New LibraryMatrix With {
                                .name = ms2.id,
                                .ms2 = ms2.GetRawMatrix
                            }
                        End Function)
        End Function

        ''' <summary>
        ''' Working for MRM method
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function LoadChromatogramList(path As String) As IEnumerable(Of chromatogram)
            Return path.LoadXmlDataSet(Of chromatogram)(, xmlns:=indexedmzML.xmlns)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MRMTargetMz(selector As IMRMSelector) As Double
            Return selector _
                .isolationWindow _
                .cvParams _
                .KeyItem("isolation window target m/z") _
                .value
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="type$">
        ''' + ``m/z`` array
        ''' + intensity array
        ''' + time array
        ''' </param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ByteArray(data As BinaryData, type$) As binaryDataArray
            Return data _
                .binaryDataArrayList _
                .list _
                .Where(Function(a)
                           Return Not a.cvParams.KeyItem(type) Is Nothing
                       End Function) _
                .FirstOrDefault
        End Function

        ''' <summary>
        ''' Get this chromatogram signal ticks.(返回来的时间的单位都统一为秒)
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Ticks(chromatogram As chromatogram) As ChromatogramTick()
            Dim time = chromatogram.ByteArray("time array")
            Dim into = chromatogram.ByteArray("intensity array")
            Dim timeUnit = time.cvParams.KeyItem("time array").unitName
            Dim intoUnit = into.cvParams.KeyItem("intensity array").unitName
            Dim time_array = time.Base64Decode.AsVector
            Dim intensity_array = into.Base64Decode

            If timeUnit.TextEquals("minute") Then
                time_array = time_array * 60
            End If

            Dim data = time_array _
                .Select(Function(t, i)
                            Return New ChromatogramTick(t, intensity_array(i))
                        End Function) _
                .ToArray

            Return data
        End Function
    End Module
End Namespace
