#Region "Microsoft.VisualBasic::8c84238e07a568c5050cbbb1fb14b050, E:/mzkit/src/metadb/KNApSAcK//Web/Search.vb"

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
    '    Code Lines: 32
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.50 KB


    ' Class Search
    ' 
    '     Function: GetData, Search
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.Massbank.KNApSAcK.Data
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Net.Http

Public Class Search

    Public Shared Function Search(word As String, Optional type As Types = Types.metabolite, Optional cache$ = "./") As IEnumerable(Of ResultEntry)
        Static query As New Dictionary(Of String, SearchQuery)

        Dim term As New QueryInput With {
            .type = type,
            .word = word
        }
        Dim result As ResultEntry() = query _
            .ComputeIfAbsent(cache, Function() New SearchQuery(cache)) _
            .Query(Of ResultEntry())(term, ".html")

        Return result
    End Function

    Public Shared Function GetData(cid As String, Optional cache$ = "./") As Information
        Static query As New Dictionary(Of String, InformationQuery)

        Dim result As Information = query _
            .ComputeIfAbsent(cache, Function() New InformationQuery(cache)) _
            .Query(Of Information)(cid, ".html")
        Dim img As String = result.img
        Dim imgLocal As String = $"{cache}/{img}"

        If Not imgLocal.FileExists Then
            Call $"{My.Resources.knapsack}/{img}".DownloadFile(imgLocal)
        End If

        If imgLocal.FileExists Then
            result.img = New DataURI(imgLocal.LoadImage).ToString
        End If

        Return result
    End Function
End Class
