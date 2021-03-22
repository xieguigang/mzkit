#Region "Microsoft.VisualBasic::5f472762b59fad264b71aba83a3024dc, src\metadb\Massbank\Public\NCBI\PubChem\Web\Models\PugView.vb"

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

    '     Class PugViewRecord
    ' 
    '         Properties: RecordNumber, RecordTitle, RecordType, Reference
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace NCBI.PubChem

    <XmlRoot("Record", [Namespace]:="http://pubchem.ncbi.nlm.nih.gov/pug_view")>
    Public Class PugViewRecord : Inherits InformationSection

        Public Property RecordType As String
        Public Property RecordNumber As String
        Public Property RecordTitle As String

        <XmlElement(NameOf(Reference))>
        Public Property Reference As Reference()

        Public Const HMDB$ = "Human Metabolome Database (HMDB)"

        Public Overrides Function ToString() As String
            Return RecordNumber
        End Function
    End Class
End Namespace
