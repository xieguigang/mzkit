#Region "Microsoft.VisualBasic::0df44ca16a3a9fdc7683b9c6b49fa835, assembly\BrukerDataReader\XMass\Xml\AnalysisParameter.vb"

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

    '   Total Lines: 25
    '    Code Lines: 16
    ' Comment Lines: 4
    '   Blank Lines: 5
    '     File Size: 730 B


    '     Class AnnotationParameter
    ' 
    '         Properties: BuildingBlockName, cid, FontFaceName, FontOrientation, FontPointSize
    '                     PreviewFlag, SearchTolerance, SearchToleranceUnit, ShowMassDifferenceFlag, StringType
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace XMass

    ''' <summary>
    ''' AnalysisParameter.xml
    ''' </summary>
    ''' 
    Public Class AnnotationParameter

        <XmlAttribute>
        Public Property cid As String

        Public Property BuildingBlockName As String
        Public Property FontFaceName As String
        Public Property FontPointSize As Double
        Public Property FontOrientation As Double
        Public Property SearchTolerance As Double
        Public Property SearchToleranceUnit As String
        Public Property StringType As String
        Public Property PreviewFlag As Boolean
        Public Property ShowMassDifferenceFlag As Boolean

    End Class
End Namespace
