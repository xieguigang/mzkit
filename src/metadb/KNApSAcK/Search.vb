Imports Microsoft.VisualBasic.ComponentModel.Collection

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

        Return result
    End Function
End Class

Public Class ResultEntry
    Public Property C_ID As String
    Public Property CAS_ID As String
    Public Property Metabolite As String()
    Public Property formula As String
    Public Property Mw As Double
End Class

Public Class Information

    Public Property name As String()
    Public Property formula As String
    Public Property mw As Double
    Public Property CAS As String()
    Public Property CID As String
    Public Property InChIKey As String
    Public Property InChICode As String
    Public Property SMILES As String
    Public Property Biosynthesis As String
    Public Property Organism As Organism()

End Class

Public Class Organism
    Public Property Kingdom As String
    Public Property Family As String
    Public Property Species As String
End Class