Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports std = System.Math

Namespace LipidMaps

    Public Class LipidMapsCategory

        Public Property [Class] As Dictionary(Of String, CatalogProfiling)

        Public Iterator Function EnumerateAllCatalogList() As IEnumerable(Of CatalogList)
            For Each [class] As CatalogProfiling In Me.Class.Values
                For Each list In [class].SubCategory.Values
                    Yield list
                Next
            Next
        End Function

        Public Function FindClass(term As String) As String
            For Each id As String In [Class].Keys
                If [Class](id).SubCategory.ContainsKey(term) Then
                    Return id
                End If
            Next

            Return Nothing
        End Function

        Public Function CreateEnrichmentProfiles(lm_enrich As IEnumerable(Of EnrichmentResult)) As CatalogProfiles
            Dim profiles As New CatalogProfiles With {
                .catalogs = New Dictionary(Of String, CatalogProfile)
            }

            For Each term As EnrichmentResult In lm_enrich
                Dim classTag As String = FindClass(term.term)
                Dim score As Double = -std.Log10(term.pvalue)

                If Not classTag.StringEmpty Then
                    Dim classList = profiles.GetCategory(term:=classTag)
                    classList.Add(term.term, score)
                End If
            Next

            Return profiles
        End Function

        Public Function CreateProfiles(lm_id As IEnumerable(Of String)) As CatalogProfiles
            Dim check As Dictionary(Of String, Double) = lm_id _
                .GroupBy(Function(id) id) _
                .ToDictionary(Function(id) id.Key,
                              Function(id)
                                  Return CDbl(id.Count)
                              End Function)
            Dim profiles As New CatalogProfiles With {
                .catalogs = New Dictionary(Of String, CatalogProfile)
            }
            Dim profile As CatalogProfile

            For Each [class] As CatalogProfiling In Me.Class.Values
                profile = CreateProfile(check, background:=[class])
                profiles.catalogs.Add([class].Catalog, profile)
            Next

            Return profiles
        End Function

        Private Shared Function CreateProfile(lm_id As Dictionary(Of String, Double), background As CatalogProfiling) As CatalogProfile
            Dim profile As New CatalogProfile With {
                .profile = New Dictionary(Of String, Double),
                .Information = New Dictionary(Of String, String)
            }
            Dim score As Double

            For Each subtype In background.SubCategory
                score = Aggregate hit As NamedValue(Of Double)
                        In subtype.Value.Intersect(lm_id)
                        Into Sum(hit.Value) '

                profile.profile.Add(subtype.Key, score)
                profile.information.Add(subtype.Key, subtype.Value.Description)
            Next

            Return profile
        End Function

    End Class
End Namespace