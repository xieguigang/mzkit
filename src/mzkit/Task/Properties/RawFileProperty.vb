#Region "Microsoft.VisualBasic::65d3815e136ea4d23705869f42bb36ec, mzkit\src\mzkit\Task\Properties\RawFileProperty.vb"

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

    '   Total Lines: 44
    '    Code Lines: 37
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.48 KB


    ' Class RawFileProperty
    ' 
    '     Properties: cacheSize, fileSize, ms_scans, msms_scans, rtmax
    '                 rtmin, source
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: getRaw
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Public Class RawFileProperty

    <Description("the data file its source path.")>
    <Category("Local File")>
    Public Property source As String
    <Description("the data file its file length.")>
    <Category("Local File")>
    Public Property fileSize As String
    <Description("the cache file its source path.")>
    <Category("Local File")>
    Public Property cacheSize As String

    <Description("the rt lower bound of the ions in current data file.")>
    <Category("Data Summary")>
    Public Property rtmin As Double
    <Description("the rt upper bound of the ions in current data file.")>
    <Category("Data Summary")>
    Public Property rtmax As Double

    <Description("the number of the MS1 scans in current data file.")>
    <Category("Data Summary")>
    Public Property ms_scans As Integer
    <Description("the number of the MS/MS scans in current data file.")>
    <Category("Data Summary")>
    Public Property msms_scans As Integer

    ReadOnly raw As Raw

    Sub New(raw As Raw)
        source = raw.source.FileName
        fileSize = StringFormats.Lanudry(raw.source.FileLength)
        cacheSize = StringFormats.Lanudry(raw.GetCacheFileSize)
        rtmin = raw.rtmin
        rtmax = raw.rtmax
        ms_scans = raw.GetMs1Scans.Count
        msms_scans = raw.GetMs2Scans.Count
    End Sub

    Public Function getRaw() As Raw
        Return raw
    End Function
End Class
