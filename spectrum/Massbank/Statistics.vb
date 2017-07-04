Imports System.Runtime.CompilerServices

Public Module Statistics

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cpd">KEGG compounds id list.</param>
    ''' <returns></returns>
    <Extension>
    Public Function KEGGPathwayCoverages(cpd As IEnumerable(Of String)) As Dictionary(Of String, (cover%, ALL%))

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="list">HMDB id list</param>
    ''' <param name="hmdb">The hmdb XML database file path.</param>
    ''' <returns></returns>
    <Extension>
    Public Function HMDBCoverages(list As IEnumerable(Of String), hmdb$) As Dictionary(Of String, (cover%, ALL%))
        Dim secondary2Main As New 
    End Function
End Module
