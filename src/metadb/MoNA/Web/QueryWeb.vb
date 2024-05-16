#Region "Microsoft.VisualBasic::384cf917967f540852334db6bc55c99e, metadb\MoNA\Web\QueryWeb.vb"

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

    '   Total Lines: 41
    '    Code Lines: 36
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.50 KB


    ' Class QueryWeb
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: getRestUrl, parseJSON
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Friend Class QueryWeb : Inherits WebQuery(Of (name_query As Boolean, q As String))

    Public Sub New(<CallerMemberName>
                   Optional cache As String = Nothing,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        Call MyBase.New(
            url:=AddressOf getRestUrl,
            contextGuid:=Function(id) $"{id.name_query}+{id.q}".MD5,
            parser:=AddressOf parseJSON,
            prefix:=Function(id) id.Substring(0, 2),
            cache:=cache,
            interval:=interval,
            offline:=offline
        )
    End Sub

    Private Shared Function parseJSON(json As String, schema As Type) As Object
        If schema Is GetType(WebJSON) Then
            Return json.LoadJSON(Of WebJSON)
        ElseIf schema Is GetType(WebJSON()) Then
            Return json.LoadJSON(Of WebJSON())
        Else
            Throw New NotImplementedException
        End If
    End Function

    Private Shared Function getRestUrl(context As (name_query As Boolean, q As String)) As String
        If context.name_query Then
            Return WebJSON.query.Replace("{q}", context.q.UrlEncode)
        Else
            Return sprintf(WebJSON.urlBase, context.q)
        End If
    End Function
End Class
