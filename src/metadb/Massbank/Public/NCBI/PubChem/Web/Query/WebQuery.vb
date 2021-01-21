#Region "Microsoft.VisualBasic::07110222891c895eb24cbfbe6369f9a3, src\metadb\Massbank\Public\NCBI\PubChem\Web\Query\WebQuery.vb"

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

    '     Class JsonQuery
    ' 
    '         Properties: [where], collection, download, limit, order
    '                     start
    ' 
    '     Class [QueryWhere]
    ' 
    '         Properties: ands
    ' 
    '     Class QueryTableExport
    ' 
    '         Properties: aids, annothitcnt, annothits, cid, cidcdate
    '                     cmpdname, cmpdsynonym, complexity, dois, hbondacc
    '                     hbonddonor, heavycnt, inchikey, iupacname, meshheadings
    '                     mf, mw, polararea, rotbonds, xlogp
    ' 
    '     Class CIDExport
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: normalizeFileName, parseExportTable, queryApi
    ' 
    '     Class CIDQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: loadQueryJson, normalizeFileName, prefix, queryApi
    ' 
    '     Class WebQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: loadPugView, prefix, pugViewApi
    ' 
    '     Class QueryResponse
    ' 
    '         Properties: Fault, IdentifierList
    ' 
    '         Function: ToString
    ' 
    '     Class Fault
    ' 
    '         Properties: Code, Details, Message
    ' 
    '     Class IdentifierList
    ' 
    '         Properties: CID
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NCBI.PubChem

    Public Class CIDQuery : Inherits WebQuery(Of String)

        ''' <summary>
        ''' Search pubchem by CAS
        ''' </summary>
        Const queryCAS_URL As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/%s/cids/JSON"

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)
            MyBase.New(AddressOf queryApi, AddressOf normalizeFileName, AddressOf loadQueryJson, AddressOf prefix, cache, interval, offline)
        End Sub

        ''' <summary>
        ''' Path prefix of the compound name
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Private Shared Function prefix(name As String) As String
            Return name.NormalizePathString.Trim("_"c, " "c).First
        End Function

        Private Shared Function loadQueryJson(jsonText As String, type As Type) As IdentifierList
            If jsonText.StringEmpty Then
                ' 404 代码之下得到的content text是空字符串
                Return Nothing
            Else
                Dim list As IdentifierList = jsonText _
                    .LoadJSON(Of QueryResponse) _
                   ?.IdentifierList
                Return list
            End If
        End Function

        Private Shared Function normalizeFileName(text As String) As String
            Return text.NormalizePathString(False)
        End Function

        Private Shared Function queryApi(name As String) As String
            Return CLangStringFormatProvider.sprintf(queryCAS_URL, name.UrlEncode(jswhitespace:=True))
        End Function

    End Class

    Public Class WebQuery : Inherits WebQuery(Of String)

        Const fetchPugView As String = "https://pubchem.ncbi.nlm.nih.gov/rest/pug_view/data/compound/%s/XML/?response_type=display"

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)
            MyBase.New(AddressOf pugViewApi, Function(cid) cid, AddressOf loadPugView, AddressOf prefix, cache, interval, offline)
        End Sub

        ''' <summary>
        ''' Path prefix of the CID number
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Private Shared Function prefix(name As String) As String
            Return Mid(name, 1, 3)
        End Function

        Private Shared Function loadPugView(xml As String, type As Type) As PugViewRecord
            If type Is GetType(PugViewRecord) Then
                Return xml.LoadFromXml(Of PugViewRecord)(throwEx:=False)
            Else
                Throw New NotImplementedException
            End If
        End Function

        Private Shared Function pugViewApi(cid As String) As String
            Return fetchPugView.Replace("%s", cid)
        End Function
    End Class

    Public Class QueryResponse

        Public Property IdentifierList As IdentifierList
        ''' <summary>
        ''' 当这个属性为空值的时候说明请求成功,反之不为空的时候说明出现了错误
        ''' </summary>
        ''' <returns></returns>
        Public Property Fault As Fault

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class Fault
        Public Property Code As String
        Public Property Message As String
        Public Property Details As String()
    End Class

    Public Class IdentifierList

        Public Property CID As String()

        Public Overrides Function ToString() As String
            Return CID.GetJson
        End Function
    End Class
End Namespace
