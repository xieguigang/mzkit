#Region "Microsoft.VisualBasic::51b8022e6fa4bbcfc91efc681b041adc, metadb\Massbank\Public\NCBI\PubChem\Web\Query\ExtensionModels\ReactionGraph.vb"

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

    '   Total Lines: 74
    '    Code Lines: 59
    ' Comment Lines: 3
    '   Blank Lines: 12
    '     File Size: 2.56 KB


    '     Class ReactionGraph
    ' 
    '         Properties: cids, cidsproduct, cidsreactant, control, definition
    '                     ecs, externalid, geneids, name, protacxns
    '                     reaction, source, taxid, taxname, url
    ' 
    '         Function: getArray, GetGeneIdSet, GetName, GetProducts, GetProteinIdSet
    '                   GetReactants, ParseJSON, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports any = Microsoft.VisualBasic.Scripting

Namespace NCBI.PubChem.ExtensionModels

    ''' <summary>
    ''' reaction.json data which downloaded from pubchem
    ''' </summary>
    Public Class ReactionGraph

        Public Property name As Object
        Public Property source As String
        Public Property externalid As String
        Public Property url As String
        Public Property definition As String
        Public Property reaction As String
        Public Property control As String
        Public Property cids As Object
        Public Property protacxns As Object
        Public Property geneids As Object
        Public Property taxid As UInteger
        Public Property taxname As String
        Public Property ecs As Object
        Public Property cidsreactant As Object
        Public Property cidsproduct As Object

        Public Function GetReactants() As String()
            Return getArray(cidsreactant)
        End Function

        Public Function GetName() As String
            Return getArray(name).JoinBy("/")
        End Function

        Public Function GetProducts() As String()
            Return getArray(cidsproduct)
        End Function

        Public Function GetGeneIdSet() As String()
            Return getArray(geneids)
        End Function

        Public Function GetProteinIdSet() As String()
            Return getArray(protacxns)
        End Function

        Private Shared Function getArray(val As Object) As String()
            If val Is Nothing Then
                Return {}
            ElseIf val.GetType.IsArray Then
                Return DirectCast(val, Array) _
                    .AsObjectEnumerator _
                    .Select(Function(o) any.ToString(o)) _
                    .ToArray
            Else
                Return New String() {any.ToString(val)}
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"[{source}:{externalid}] {definition}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseJSON(str As String) As ReactionGraph()
            Return JsonParser _
                .Parse(str, strictVectorSyntax:=False) _
                .CreateObject(GetType(ReactionGraph()), decodeMetachar:=True)
        End Function

    End Class
End Namespace
