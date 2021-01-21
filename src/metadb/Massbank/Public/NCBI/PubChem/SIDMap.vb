#Region "Microsoft.VisualBasic::07b03c3cab6d8b37f5766225beeeda9d, src\metadb\Massbank\Public\NCBI\PubChem\SIDMap.vb"

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

    '     Class SIDMap
    ' 
    '         Properties: CID, registryIdentifier, SID, sourceName
    ' 
    '         Function: GetMaps, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text

Namespace NCBI.PubChem

    ''' <summary>
    ''' This is a listing of all (live) SIDs with their source names and
    ''' registry identifiers, and the standardized CID If present. It Is 
    ''' a gzipped text file where Each line contains at least three
    ''' columns: SID, tab, source name, tab, registry identifier; then
    ''' a fourth column Of tab, CID If there Is a standardized CID For 
    ''' the given SID.
    ''' </summary>
    Public Class SIDMap

        Public Property SID As Integer
        Public Property sourceName As String
        Public Property registryIdentifier As String
        Public Property CID As Integer

        Public Const KEGG As String = "KEGG"
        Public Const ChEBI As String = "ChEBI"
        Public Const HMDB As String = "Human Metabolome Database (HMDB)"

        Public Overrides Function ToString() As String
            Return $"[{SID}] {registryIdentifier}"
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle">SID-Map.txt file path or its file content.</param>
        ''' <param name="skipNoCID">
        ''' By default is skip all of the record rows that without CID assigned.
        ''' </param>
        ''' <returns></returns>
        Public Shared Iterator Function GetMaps(handle$, Optional skipNoCID As Boolean = True) As IEnumerable(Of SIDMap)
            Dim t As String()
            Dim CID As Integer
            Dim lineSource As IEnumerable(Of String)

            If handle.FileExists Then
                lineSource = handle.IterateAllLines
            Else
                lineSource = handle.LineTokens
            End If

            For Each line As String In lineSource
                t = line.Split(ASCII.TAB)

                If t.Length > 3 Then
                    CID = Integer.Parse(t(3))
                Else
                    If skipNoCID Then
                        Continue For
                    Else
                        CID = -1
                    End If
                End If

                Yield New SIDMap With {
                    .CID = CID,
                    .SID = Integer.Parse(t(0)),
                    .sourceName = t(1),
                    .registryIdentifier = t(2)
                }
            Next
        End Function
    End Class
End Namespace
