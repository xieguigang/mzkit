#Region "Microsoft.VisualBasic::3844d7b4b65fef9a617d4bba25b0c955, E:/mzkit/src/metadna/metaDNA//Infer/Candidate.vb"

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

    '   Total Lines: 29
    '    Code Lines: 15
    ' Comment Lines: 8
    '   Blank Lines: 6
    '     File Size: 838 B


    '     Class Candidate
    ' 
    '         Properties: infer, ppm, precursorType, pvalue, ROI
    '                     score
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace Infer

    Public Class Candidate

        ''' <summary>
        ''' ms1 ROI id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ROI As String
        <XmlElement> Public Property precursorType As String
        <XmlElement> Public Property ppm As Double
        <XmlElement> Public Property score As Double
        <XmlElement> Public Property pvalue As Double

        ''' <summary>
        ''' <see cref="AlignmentOutput"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property infer As InferLink

        Public Overrides Function ToString() As String
            Return pvalue
        End Function

    End Class
End Namespace
