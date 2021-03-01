#Region "Microsoft.VisualBasic::6d323a8e06fb28a7808857b1e5e8a6a5, assembly\MarkupData\mzXML\MsData\MSScan.vb"

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

    '     Class scan
    ' 
    '         Properties: basePeakIntensity, basePeakMz, centroided, collisionEnergy, highMz
    '                     lowMz, msInstrumentID, msLevel, num, peaks
    '                     peaksCount, polarity, precursorMz, retentionTime, scanType
    '                     totIonCurrent
    ' 
    '         Function: ScanData, ToString
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

    ''' <summary>
    ''' 一个一级或者二级的扫描结果数据的模型
    ''' </summary>
    Public Class scan

        ''' <summary>
        ''' The scan number
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property num As Integer
        <XmlAttribute> Public Property scanType As String

        ''' <summary>
        ''' ``profile`` and ``centroid`` in Mass Spectrometry?
        ''' 
        ''' 1. Profile means the continuous wave form in a mass spectrum.
        '''   + Number of data points Is large.
        ''' 2. Centroid means the peaks in a profile data Is changed to bars.
        '''   + location of the bar Is center of the profile peak.
        '''   + height of the bar Is area of the profile peak.  
        '''   
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property centroided As String
        ''' <summary>
        ''' 当前的质谱碎片的等级,一级质谱,二级质谱或者msn等级的质谱
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property msLevel As Integer
        <XmlAttribute> Public Property peaksCount As Integer
        <XmlAttribute> Public Property polarity As String
        <XmlAttribute> Public Property retentionTime As String
        <XmlAttribute> Public Property basePeakMz As Double
        <XmlAttribute> Public Property basePeakIntensity As Double
        <XmlAttribute> Public Property totIonCurrent As Double
        <XmlAttribute> Public Property collisionEnergy As String
        <XmlAttribute> Public Property lowMz As Double
        <XmlAttribute> Public Property highMz As Double
        <XmlAttribute> Public Property msInstrumentID As String

        Public Property precursorMz As precursorMz
        Public Property peaks As peaks

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="basename"></param>
        ''' <returns></returns>
        Public Function ScanData(Optional basename$ = Nothing,
                                 Optional centroid As Boolean = False,
                                 Optional raw As Boolean = False,
                                 Optional centroidTolerance As Tolerance = Nothing,
                                 Optional intocutoff As LowAbundanceTrimming = Nothing) As PeakMs2

            Dim ms2 As ms2() = peaks _
                .ExtractMzI _
                .Where(Function(p) p.intensity > 0) _
                .Select(Function(p)
                            Return New ms2 With {
                                .mz = p.mz,
                                .quantity = p.intensity,
                                .intensity = p.intensity
                            }
                        End Function) _
                .ToArray
            Dim mzInto As New LibraryMatrix With {
                .centroid = If(centroided = "1", True, False),
                .ms2 = ms2,
                .name = ToString()
            }

            Static ms1 As [Default](Of String) = "ms1"

            ' 合并碎片只针对2级碎片有效
            If (msLevel > 1) AndAlso centroid Then
                If centroidTolerance Is Nothing Then
                    centroidTolerance = Tolerance.DeltaMass(0.1)
                End If

                mzInto = mzInto.CentroidMode(centroidTolerance, intocutoff Or LowAbundanceTrimming.Default)
            End If

            If Not raw Then
                mzInto = mzInto / mzInto.Max
            End If

            Return New PeakMs2 With {
                .mz = precursorMz,
                .rt = PeakMs2.RtInSecond(retentionTime),
                .scan = num,
                .file = basename,
                .mzInto = mzInto.Array,
                .activation = precursorMz.activationMethod Or ms1,
                .collisionEnergy = Val(collisionEnergy),
                .intensity = precursorMz.precursorIntensity
            }
        End Function
    End Class
End Namespace
