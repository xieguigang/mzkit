#Region "Microsoft.VisualBasic::e0f4c12c5c055efa3ded595b31f7c171, mzmath\mz_deco\XcmsSamplePeak.vb"

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

    '   Total Lines: 58
    '    Code Lines: 52 (89.66%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (10.34%)
    '     File Size: 2.19 KB


    ' Class XcmsSamplePeak
    ' 
    '     Properties: ID, intb, into, maxo, mz
    '                 mzmax, mzmin, rt, rtmax, rtmin
    '                 sample, sn
    ' 
    '     Function: ParseCsv
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language

Public Class XcmsSamplePeak

    Public Property mz As Double
    Public Property mzmin As Double
    Public Property mzmax As Double
    Public Property rt As Double
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property into As Double
    Public Property intb As Double
    Public Property maxo As Double
    Public Property sn As Double
    Public Property sample As String
    Public Property ID As String

    Public Shared Iterator Function ParseCsv(file As Stream) As IEnumerable(Of XcmsSamplePeak)
        Dim s As New StreamReader(file)
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
