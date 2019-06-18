Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.ComponentModel

Namespace sigmaaldrich

    Public Module MetaQuery

        ReadOnly queryEngine As New Dictionary(Of String, Object)

        Public Function Search(term As String, Optional cache$ = "./sigmaaldrich/", Optional offline As Boolean = False) As SearchResult()
            Dim searchEngine As TermSearch = queryEngine _
                .ComputeIfAbsent(
                    key:=cache,
                    lazyValue:=Function()
                                   Return New TermSearch(cache,, offline)
                               End Function)

            searchEngine.offlineMode = offline

            Return searchEngine.Query(Of SearchResult())(term, ".html")
        End Function

    End Module

    Public Class TermSearch : Inherits WebQuery(Of String)

        Const searchUrl = "https://www.sigmaaldrich.com/catalog/search?term={0}&interface=All_ZH&N=0&mode=match%20partialmax&lang=zh&region=CN&focus=product"

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            MyBase.New(url:=Function(term) String.Format(searchUrl, term.UrlEncode(jswhitespace:=True)),
                       contextGuid:=Function(term) term.NormalizePathString,
                       parser:=AddressOf ParseSearchPage,
                       prefix:=Nothing,
                       cache:=cache,
                       interval:=interval,
                       offline:=offline
            )
        End Sub

        Private Shared Function ParseSearchPage(html$, null As Type) As Object

        End Function
    End Class

    Public Class SearchResult

        Public Property id As String
        Public Property name As String
        Public Property summary As String

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class
End Namespace