#Region "Microsoft.VisualBasic::e5b5356b24b84c2a862e564b3024012c, assembly\assembly\MarkupData\mzXML\MsData\MSData.vb"

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

    '   Total Lines: 40
    '    Code Lines: 26 (65.00%)
    ' Comment Lines: 7 (17.50%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (17.50%)
    '     File Size: 1.39 KB


    '     Class MSData
    ' 
    '         Properties: dataProcessing, endTime, msInstrument, parentFile, scanCount
    '                     scans, startTime
    ' 
    '         Function: GenericEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace MarkupData.mzXML

    ''' <summary>
    ''' a collection of the ms <see cref="scan"/> data
    ''' </summary>
    <XmlType("msRun")> Public Class MSData : Implements Enumeration(Of scan)

        <XmlAttribute> Public Property scanCount As Integer
        <XmlAttribute> Public Property startTime As String
        <XmlAttribute> Public Property endTime As String

        Public Property parentFile As parentFile
        Public Property msInstrument As msInstrument
        Public Property dataProcessing As dataProcessing

        ''' <summary>
        ''' the mass spectrum scan data collection
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("scan")>
        Public Property scans As scan()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return parentFile.ToString
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of scan) Implements Enumeration(Of scan).GenericEnumerator
            If Not scans Is Nothing Then
                For Each scan As scan In scans
                    Yield scan
                Next
            End If
        End Function
    End Class
End Namespace
