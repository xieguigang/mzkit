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
