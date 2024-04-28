#Region "Microsoft.VisualBasic::260e4b7c3a23c80aec54b83064730e48, E:/mzkit/src/metadb/Massbank//Public/NCBI/PubChem/Web/Query/ExtensionModels/PathwayGraph.vb"

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

    '   Total Lines: 38
    '    Code Lines: 33
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.41 KB


    '     Class PathwayGraph
    ' 
    '         Properties: annotation, category, cids, core, ecs
    '                     externalid, extid, geneids, name, pmids
    '                     protacxns, pwacc, pwtype, source, srcid
    '                     taxid, taxname, url
    ' 
    '         Function: ParseJSON, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.json

Namespace NCBI.PubChem.ExtensionModels

    Public Class PathwayGraph

        Public Property taxname As String
        Public Property pwacc As String
        Public Property name As String
        Public Property pwtype As String
        Public Property category As String
        Public Property url As String
        Public Property source As String
        Public Property srcid As String
        Public Property externalid As String
        Public Property extid As String
        Public Property taxid As String
        Public Property core As String
        Public Property cids As String()
        Public Property geneids As String()
        Public Property protacxns As String()
        Public Property ecs As String()
        Public Property pmids As String()
        Public Property annotation As String()

        Public Overrides Function ToString() As String
            Return $"{pwacc}: {name}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseJSON(str As String) As PathwayGraph()
            Return JsonParser _
                .Parse(str, strictVectorSyntax:=False) _
                .CreateObject(GetType(PathwayGraph()), decodeMetachar:=True)
        End Function
    End Class
End Namespace
