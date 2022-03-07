Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Module Actions

    ReadOnly actions As New Dictionary(Of String, Action(Of Array))

    Public ReadOnly Property allActions As IEnumerable(Of String)
        Get
            Return actions.Keys
        End Get
    End Property

    Public Sub Register(name As String, action As Action(Of Array))
        actions(name) = action
    End Sub

    Public Sub RunAction(name As String, data As Array)
        If actions.ContainsKey(name) Then
            Call actions(name)(data)
        Else
            Call MyApplication.host.warning($"missing action '{name}'!")
        End If
    End Sub

    Sub New()
        Call registerMs1Search()
        Call registerKEGGEnrichment()
    End Sub

    Private Sub registerMs1Search()
        Call Register("Peak List Annotation",
             Sub(data)
                 MyApplication.host.mzkitSearch.TextBox3.Text = data.AsObjectEnumerator.JoinBy(vbCrLf)

                 Call MyApplication.host.mzkitSearch.TabControl1.SelectTab(MyApplication.host.mzkitSearch.TabPage3)
                 Call MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
             End Sub)
    End Sub

    Private Sub registerKEGGEnrichment()
        Call Register("KEGG Enrichment",
             Sub(data)
                 Dim maps As Map() = Nothing
                 Dim kegg As Background = Globals.loadBackground(maps)
                 Dim enrich = frmTaskProgress.LoadData(
                    Function(msg)
                        Dim all = kegg.Enrichment(data.AsObjectEnumerator.Select(Function(c) c.ToString), outputAll:=True, showProgress:=True, doProgress:=msg).ToArray
                        Call msg("Do FDR...")
                        Dim fdr = all.FDRCorrection.OrderBy(Function(p) p.pvalue).ToArray

                        Return fdr
                    End Function, title:="Run KEGG Enrichment", info:="Run fisher test...")
                 Dim table = VisualStudio.ShowDocument(Of frmTableViewer)(title:="KEGG Enrichment Result")
                 Dim mapIndex = maps.ToDictionary(Function(m) m.id)

                 table.ViewRow =
                     Sub(row)
                         Dim id As String = row("term")
                         Dim map As Map = mapIndex(id)
                         Dim geneIds = row("geneIDs").ToString.StringSplit(",\s+").Select(Function(gid) New NamedValue(Of String)(gid, "blue")).ToArray
                         Dim image As Image = LocalRender.Rendering(map, geneIds)

                         VisualStudio.ShowDocument(Of frmPlotViewer)(title:=map.Name).PictureBox1.BackgroundImage = image
                     End Sub
                 table.LoadTable(Sub(grid)
                                     grid.Columns.Add(NameOf(EnrichmentResult.term), NameOf(EnrichmentResult.term))
                                     grid.Columns.Add(NameOf(EnrichmentResult.name), NameOf(EnrichmentResult.name))
                                     grid.Columns.Add(NameOf(EnrichmentResult.description), NameOf(EnrichmentResult.description))
                                     grid.Columns.Add(NameOf(EnrichmentResult.cluster), NameOf(EnrichmentResult.cluster))
                                     grid.Columns.Add(NameOf(EnrichmentResult.enriched), NameOf(EnrichmentResult.enriched))
                                     grid.Columns.Add(NameOf(EnrichmentResult.score), NameOf(EnrichmentResult.score))
                                     grid.Columns.Add(NameOf(EnrichmentResult.pvalue), NameOf(EnrichmentResult.pvalue))
                                     grid.Columns.Add(NameOf(EnrichmentResult.FDR), NameOf(EnrichmentResult.FDR))
                                     grid.Columns.Add(NameOf(EnrichmentResult.geneIDs), NameOf(EnrichmentResult.geneIDs))

                                     For Each item As EnrichmentResult In enrich
                                         Call grid.Rows.Add(item.term, item.name, item.description, item.cluster, item.enriched, item.score, item.pvalue, item.FDR, item.geneIDs.JoinBy(", "))
                                     Next
                                 End Sub)
             End Sub)
    End Sub

End Module
