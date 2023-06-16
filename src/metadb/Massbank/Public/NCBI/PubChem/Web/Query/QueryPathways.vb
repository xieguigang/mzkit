#Region "Microsoft.VisualBasic::512c04e4387f642dbf940ba3411d0762, mzkit\src\metadb\Massbank\Public\NCBI\PubChem\Web\Query\QueryPathways.vb"

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

'   Total Lines: 54
'    Code Lines: 42
' Comment Lines: 1
'   Blank Lines: 11
'     File Size: 2.66 KB


'     Class QueryPathways
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: url
' 
'     Enum Types
' 
'         pathways, reaction, taxonomy
' 
'  
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net.Http

Namespace NCBI.PubChem

    ' {%22download%22:%22*%22,%22collection%22:%22lotus%22,%22where%22:{%22ands%22:[{%22cid%22:%22128861%22}]},%22order%22:[%22relevancescore,desc%22],%22start%22:1,%22limit%22:10000000,%22downloadfilename%22:%22CID_128861_lotus%22}

    Public Class QueryPathways : Inherits WebQuery(Of NamedValue(Of Types))

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            Call MyBase.New(
                url:=AddressOf url,
                contextGuid:=Function(str) $"{str.Value.ToString}_{str.Name}",
                parser:=Function(str, type) str,
                prefix:=Function(str) str.MD5.Substring(0, 2),
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Sub New(cachefs As IFileSystemEnvironment,
                Optional interval As Integer = -1,
                Optional offline As Boolean = False)

            Call MyBase.New(
                url:=AddressOf url,
                contextGuid:=Function(str) $"{str.Value.ToString}_{str.Name}",
                parser:=Function(str, type) str,
                prefix:=Function(str) str.MD5.Substring(0, 2),
                cache:=cachefs,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Const queryOriginJSON As String = "{""download"":""*"",""collection"":""lotus"",""where"":{""ands"":[{""cid"":""%cid""}]},""order"":[""relevancescore,desc""],""start"":1,""limit"":10000000,""downloadfilename"":""cID_%cid_lotus""}"
        Const queryPathwayJSON As String = "{""download"":""*"",""collection"":""pathway"",""where"":{""ands"":[{""cid"":""%cid""},{""core"":""1""}]},""order"":[""taxname,asc""],""start"":1,""limit"":10000000,""downloadfilename"":""cID_%cid_pathway""}"
        Const queryReactionJSON As String = "{""download"":""*"",""collection"":""pathwayreaction"",""where"":{""ands"":[{""cid"":""%cid""}]},""order"":[""relevancescore,desc""],""start"":1,""limit"":10000000,""downloadfilename"":""cID_%cid_pathwayreaction""}"

        Private Shared Function url(cid As NamedValue(Of Types)) As String
            Dim template As String

            Select Case cid.Value
                Case Types.pathways : template = queryPathwayJSON
                Case Types.reaction : template = queryReactionJSON
                Case Types.taxonomy : template = queryOriginJSON
                Case Else
                    Throw New NotImplementedException(cid.ToString)
            End Select

            Dim query As String = template.Replace("%cid", cid.Name)
            Dim urlArg As String = query.UrlEncode

            Return $"https://pubchem.ncbi.nlm.nih.gov/sdq/sdqagent.cgi?infmt=json&outfmt=json&query={urlArg}"
        End Function
    End Class

    Public Enum Types
        pathways
        taxonomy
        reaction
    End Enum
End Namespace
