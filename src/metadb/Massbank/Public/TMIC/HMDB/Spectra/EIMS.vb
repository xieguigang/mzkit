#Region "Microsoft.VisualBasic::d361a18407a57a4baa85cd9d51419af9, src\metadb\Massbank\Public\TMIC\HMDB\Spectra\EIMS.vb"

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

    '     Class EIMS
    ' 
    '         Properties: peakList
    ' 
    '     Class EIMSPeak
    ' 
    '         Properties: EImsId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace TMIC.HMDB.Spectra

    <XmlType("ei-ms")> Public Class EIMS : Inherits SpectraFile
        Implements IPeakList(Of EIMSPeak)

        <XmlArray("ei-ms-peaks")>
        Public Property peakList As EIMSPeak() Implements IPeakList(Of EIMSPeak).peakList
    End Class

    <XmlType("ei-ms-peak")> Public Class EIMSPeak : Inherits MSPeak

        <XmlElement("ei-ms-id")>
        Public Property EImsId As String
    End Class
End Namespace
