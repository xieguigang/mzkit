﻿#Region "Microsoft.VisualBasic::e4b1a3d28cc8b614d79f7a43eeef93a5, assembly\assembly\MarkupData\mzXML\Extensions.vb"

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

    '   Total Lines: 113
    '    Code Lines: 66 (58.41%)
    ' Comment Lines: 32 (28.32%)
    '    - Xml Docs: 90.62%
    ' 
    '   Blank Lines: 15 (13.27%)
    '     File Size: 4.24 KB


    '     Module Extensions
    ' 
    '         Function: AsMs2, (+2 Overloads) ExtractMzI, getName, IsIntact
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports r = System.Text.RegularExpressions.Regex

Namespace MarkupData.mzXML

    <HideModuleName> Public Module Extensions

        ''' <summary>
        ''' 解析出色谱峰数据
        ''' </summary>
        ''' <param name="peaks"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' the encoded peaks data base64 string content:
        ''' 
        ''' ```
        ''' [intensity,mz][intensity,mz]
        ''' ```
        ''' </remarks>
        <Extension>
        Public Function ExtractMzI(peaks As peaks) As ms2()
            Dim floats#() = peaks.Base64Decode(True)
            Dim peaksData As ms2() = floats.AsMs2.ToArray

            Return peaksData
        End Function

        ''' <summary>
        ''' [intensity, m/z]
        ''' </summary>
        ''' <param name="floats"></param>
        ''' <returns></returns>
        <Extension>
        Public Function AsMs2(floats As IEnumerable(Of Double)) As IEnumerable(Of ms2)
            Return floats _
                .Split(2) _
                .Select(Function(buffer, i)
                            Return New ms2 With {
                                .Annotation = i + 1S,
                                .intensity = buffer(Scan0), ' 信号强度, 归一化为 0-100 之间的数值
                                .mz = buffer(1)             ' m/z质核比数据                        
                            }
                        End Function)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scan">
        ''' 可以依据扫描信号结果数据<see cref="scan.msLevel"/>来获取相应的质谱信号数据
        ''' 
        ''' + 1, MS1   一级质谱信号数据
        ''' + 2, MS/MS 二级质谱信号数据
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function ExtractMzI(scan As scan) As (name$, peaks As ms2())
            Dim name$ = scan.getName
            Dim peaks As ms2()

            If scan.peaksCount = 0 Then
                peaks = {}
            Else
                peaks = scan.peaks.ExtractMzI
            End If

            Return (name, peaks)
        End Function

        Public ReadOnly msLevels As IReadOnlyCollection(Of String) = {
            "Unknown", ' = 0
            "MS1",     ' = 1
            "MS/MS",   ' = 2
            "MS3"      ' = 3
        }

        <Extension>
        Public Function getName(scan As scan) As String
            Dim level$ = Extensions.msLevels(scan.msLevel)
            Dim empty As Boolean =
                scan.scanType.StringEmpty AndAlso
                scan.polarity.StringEmpty AndAlso
                scan.retentionTime.StringEmpty

            If scan.msLevel = 0 AndAlso empty Then
                Return Nothing
            End If

            Dim rt As Double = PeakMs2.RtInSecond(scan.retentionTime)
            Dim precursorMz As Double = scan.GetPrecursorData

            If scan.msLevel = 1 Then
                Return $"[{level}] {scan.scanType} scan_{(rt / 60).ToString("F2")}min, ({scan.polarity}) basepeak_m/z={scan.basePeakMz.ToString("F4")},totalIons={scan.totIonCurrent}"
            Else
                Return $"[{level}] {scan.scanType} Scan, ({scan.polarity}) M{CInt(precursorMz)}T{CInt(rt)}, {precursorMz.ToString("F4")}@{(rt / 60).ToString("F2")}min"
            End If
        End Function

        ''' <summary>
        ''' Check if the given <paramref name="mzXML"/> raw data file is intact or not?
        ''' </summary>
        ''' <param name="mzXML">The file path of the mzXML raw data file.</param>
        ''' <returns></returns>
        Public Function IsIntact(mzXML As String) As Boolean
            Dim tails As String = LargeTextFile.Tails(mzXML, 1024, Encoding.UTF8).Trim

            Return InStr(tails, "</mzXML>", CompareMethod.Binary) > 0 AndAlso
                r.Match(tails, "[<]sha1[>].+?[<]/sha1[>]", RegexICSng).Success
        End Function
    End Module
End Namespace
