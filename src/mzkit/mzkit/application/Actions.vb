#Region "Microsoft.VisualBasic::295ba5f2f1918f08eb52f012f54d7943, mzkit\src\mzkit\mzkit\application\Actions.vb"

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

    '   Total Lines: 102
    '    Code Lines: 88
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 5.16 KB


    ' Module Actions
    ' 
    '     Properties: allActions
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: Register, registerKEGGEnrichment, registerMs1Search, RunAction
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport

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
                        Dim all = kegg.Enrichment(data.AsObjectEnumerator.Where(Function(c) Not c Is Nothing).Select(Function(c) c.ToString), outputAll:=True, showProgress:=True, doProgress:=msg).ToArray
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
                         Dim image As String = ReportRender.Render(map, geneIds)
                         Dim temp As String = TempFileSystem.GetAppSysTempFile(".html", sessionID:=App.PID, prefix:="kegg_pathway")
                         Dim browser = VisualStudio.ShowDocument(Of frmHtmlViewer)(title:=map.Name)

                         Call image.SaveTo(temp)
                         Call browser.LoadHtml(temp)
                     End Sub
                 table.LoadTable(Sub(grid)
                                     grid.Columns.Add(NameOf(EnrichmentResult.term), GetType(String))
                                     grid.Columns.Add(NameOf(EnrichmentResult.name), GetType(String))
                                     grid.Columns.Add(NameOf(EnrichmentResult.description), GetType(String))
                                     grid.Columns.Add(NameOf(EnrichmentResult.cluster), GetType(Integer))
                                     grid.Columns.Add(NameOf(EnrichmentResult.enriched), GetType(String))
                                     grid.Columns.Add(NameOf(EnrichmentResult.score), GetType(Double))
                                     grid.Columns.Add(NameOf(EnrichmentResult.pvalue), GetType(Double))
                                     grid.Columns.Add(NameOf(EnrichmentResult.FDR), GetType(Double))
                                     grid.Columns.Add(NameOf(EnrichmentResult.geneIDs), GetType(String))

                                     For Each item As EnrichmentResult In enrich
                                         Call grid.Rows.Add(
                                            If(item.term.IsPattern("\d+"), $"map{item.term}", item.term),
                                            item.name,
                                            item.description,
                                            item.cluster,
                                            item.enriched,
                                            item.score,
                                            item.pvalue,
                                            item.FDR,
                                            item.geneIDs.JoinBy(", ")
                                         )
                                     Next
                                 End Sub)
             End Sub)
    End Sub

End Module
