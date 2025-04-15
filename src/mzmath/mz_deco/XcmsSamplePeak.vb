#Region "Microsoft.VisualBasic::cd4134114872a13f39fd4bd859cef323, mzmath\mz_deco\XcmsSamplePeak.vb"

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

    '   Total Lines: 110
    '    Code Lines: 53 (48.18%)
    ' Comment Lines: 51 (46.36%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (5.45%)
    '     File Size: 3.66 KB


    ' Class XcmsSamplePeak
    ' 
    '     Properties: ID, intb, into, maxf, maxo
    '                 mz, mzmax, mzmin, rt, rtmax
    '                 rtmin, sample, sn
    ' 
    '     Function: ParseCsv
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language

''' <summary>
''' cast ``findPeaks`` result to dataframe
''' </summary>
Public Class XcmsSamplePeak

    ''' <summary>
    ''' 峰的中心质荷比（m/z）值。
    ''' </summary>
    ''' <returns></returns>
    Public Property mz As Double
    ''' <summary>
    ''' 峰的最低质荷比（m/z）值。
    ''' </summary>
    ''' <returns></returns>
    Public Property mzmin As Double
    ''' <summary>
    ''' 峰的最高质荷比（m/z）值。
    ''' </summary>
    ''' <returns></returns>
    Public Property mzmax As Double
    ''' <summary>
    ''' 峰的中心保留时间（Retention Time，RT）。
    ''' </summary>
    ''' <returns></returns>
    Public Property rt As Double
    ''' <summary>
    ''' 峰的起始保留时间。
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmin As Double
    ''' <summary>
    ''' 峰的结束保留时间。
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmax As Double
    ''' <summary>
    ''' 峰的总强度（积分强度）。
    ''' </summary>
    ''' <returns></returns>
    Public Property into As Double
    ''' <summary>
    ''' 峰的峰下面积（峰的积分面积）。
    ''' </summary>
    ''' <returns></returns>
    Public Property intb As Double
    ''' <summary>
    ''' 峰的最大原始强度值。
    ''' </summary>
    ''' <returns></returns>
    Public Property maxo As Double
    ''' <summary>
    ''' 峰的最大过滤强度值。
    ''' </summary>
    ''' <returns></returns>
    Public Property maxf As Double
    ''' <summary>
    ''' 峰的信噪比（Signal-to-Noise ratio）。
    ''' </summary>
    ''' <returns></returns>
    Public Property sn As Double
    Public Property sample As String
    ''' <summary>
    ''' 峰的索引或标识符。
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String

    Public Shared Function ParseFile(file As Stream, Optional deli As Char = ","c, Optional normalizeID As Boolean = True) As IEnumerable(Of XcmsSamplePeak)
        Dim s As New StreamReader(file)
        Dim pool As IEnumerable(Of XcmsSamplePeak) = ParseTabularStream(s, deli)

        If normalizeID Then
            Return XcmsSamplePeak.NormalizeID(pool)
        Else
            Return pool
        End If
    End Function

    Private Shared Iterator Function NormalizeID(table As IEnumerable(Of XcmsSamplePeak)) As IEnumerable(Of XcmsSamplePeak)
        For Each peak As XcmsSamplePeak In table
            If peak.ID.IsPattern("\d+") Then
                peak.ID = "Peak" & peak.ID
            End If

            Yield peak
        Next
    End Function

    Private Shared Iterator Function ParseTabularStream(s As StreamReader, deli As Char) As IEnumerable(Of XcmsSamplePeak)
        Dim line As Value(Of String) = s.ReadLine
        Dim headers As Index(Of String) = line.Split(","c)
        Dim mz As Integer = headers!mz
        Dim mzmin As Integer = headers!mzmin
        Dim mzmax As Integer = headers!mzmax
        Dim rt As Integer = headers!rt
        Dim rtmin As Integer = headers!rtmin
        Dim rtmax As Integer = headers!rtmax
        Dim into As Integer = headers!into
        Dim intb As Integer = headers!intb
        Dim maxo As Integer = headers!maxo
        Dim sn As Integer = headers!sn
        Dim sample As Integer = headers!sample
        Dim id As Integer = headers!ID

        Do While Not (line = s.ReadLine) Is Nothing
            Dim t As String() = line.Split(","c)
            Dim peak As New XcmsSamplePeak With {
                .ID = t(id),
                .intb = If(intb = -1, 0, Double.Parse(t(intb))),
                .into = Double.Parse(t(into)),
                .maxo = If(maxo = -1, 0, Double.Parse(t(maxo))),
                .mz = Double.Parse(t(mz)),
                .mzmax = Double.Parse(t(mzmax)),
                .mzmin = Double.Parse(t(mzmin)),
                .rt = Double.Parse(t(rt)),
                .rtmax = Double.Parse(t(rtmax)),
                .rtmin = Double.Parse(t(rtmin)),
                .sample = t(sample),
                .sn = If(sn = -1, 0, Double.Parse(t(sn)))
            }

            Yield peak
        Loop
    End Function

End Class
