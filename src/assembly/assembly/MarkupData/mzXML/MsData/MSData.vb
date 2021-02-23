#Region "Microsoft.VisualBasic::ad37261cdf45b540473ed86e691e9486, assembly\MarkupData\mzXML\MSData.vb"

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

    '     Class MSData
    ' 
    '         Properties: dataProcessing, endTime, msInstrument, parentFile, scanCount
    '                     scans, startTime
    ' 
    '     Class scan
    ' 
    '         Properties: basePeakIntensity, basePeakMz, centroided, collisionEnergy, highMz
    '                     lowMz, msInstrumentID, msLevel, num, peaks
    '                     peaksCount, polarity, precursorMz, retentionTime, scanType
    '                     totIonCurrent
    ' 
    '         Function: ScanData, ToString
    ' 
    '     Class peaks
    ' 
    '         Properties: byteOrder, compressedLen, compressionType, contentType, precision
    '                     value
    ' 
    '         Function: GetCompressionType, GetPrecision, ToString
    ' 
    '     Structure precursorMz
    ' 
    '         Properties: activationMethod, precursorCharge, precursorIntensity, precursorScanNum, value
    '                     windowWideness
    ' 
    '         Function: CompareTo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MarkupData.mzXML

    <XmlType("msRun")> Public Class MSData

        <XmlAttribute> Public Property scanCount As Integer
        <XmlAttribute> Public Property startTime As String
        <XmlAttribute> Public Property endTime As String

        Public Property parentFile As parentFile
        Public Property msInstrument As msInstrument
        Public Property dataProcessing As dataProcessing

        <XmlElement("scan")>
        Public Property scans As scan()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return parentFile.ToString
        End Function

    End Class
End Namespace
