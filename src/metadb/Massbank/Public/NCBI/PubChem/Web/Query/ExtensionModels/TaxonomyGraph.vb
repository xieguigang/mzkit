#Region "Microsoft.VisualBasic::9c3cf3efc36c116153f01fcaa4c3b110, G:/mzkit/src/metadb/Massbank//Public/NCBI/PubChem/Web/Query/ExtensionModels/TaxonomyGraph.vb"

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
    '    Code Lines: 20
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 887 B


    '     Class TaxonomyGraph
    ' 
    '         Properties: cid, cmpdname, dois, taxid, taxname
    '                     wikidatacmpd, wikidataref, wikidatataxon
    ' 
    '         Function: ParseJSON
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.json

Namespace NCBI.PubChem.ExtensionModels

    Public Class TaxonomyGraph

        Public Property cid As String
        Public Property taxid As String
        Public Property taxname As String
        Public Property dois As String()
        Public Property wikidatacmpd As String
        Public Property wikidatataxon As String
        Public Property wikidataref As String()
        Public Property cmpdname As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseJSON(str As String) As TaxonomyGraph()
            Return JsonParser _
                .Parse(str, strictVectorSyntax:=False) _
                .CreateObject(GetType(TaxonomyGraph()), decodeMetachar:=True)
        End Function

    End Class
End Namespace
