#Region "Microsoft.VisualBasic::6ee13ef49513554f613eb716e410571e, metadb\Chemoinformatics\CML\cml.vb"

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

    '   Total Lines: 21
    '    Code Lines: 11 (52.38%)
    ' Comment Lines: 5 (23.81%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 5 (23.81%)
    '     File Size: 541 B


    '     Class MarkupFile
    ' 
    '         Properties: molecule, title
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace ChemicalMarkupLanguage

    ''' <summary>
    ''' Chemical Markup Language
    ''' 
    ''' https://www.xml-cml.org/
    ''' </summary>
    <XmlRoot("cml", [Namespace]:=MarkupFile.xmlns)>
    <XmlType("cml", [Namespace]:=MarkupFile.xmlns)>
    Public Class MarkupFile

        Public Const xmlns As String = "http://www.xml-cml.org/schema"

        <XmlAttribute>
        Public Property title As String
        Public Property molecule As molecule

    End Class
End Namespace
