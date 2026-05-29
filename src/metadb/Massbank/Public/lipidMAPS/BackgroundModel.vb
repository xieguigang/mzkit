#Region "Microsoft.VisualBasic::bc7996bb74c2b3508ceb02f0053b3cb7, metadb\Massbank\Public\lipidMAPS\BackgroundModel.vb"

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

    '   Total Lines: 88
    '    Code Lines: 71 (80.68%)
    ' Comment Lines: 5 (5.68%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (13.64%)
    '     File Size: 3.66 KB


    '     Module BackgroundModel
    ' 
    '         Function: CreateCategoryBackground, CreateCategoryModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace LipidMaps

    Public Module BackgroundModel

        ''' <summary>
        ''' create lipidmaps class background model for run enrichment analysis
        ''' </summary>
        ''' <param name="lipidmaps"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateCategoryBackground(lipidmaps As IEnumerable(Of MetaData)) As Background
            Dim categories = lipidmaps.CreateCategoryModel
            Dim clusters As New List(Of Cluster)
            Dim subtype As Cluster

            For Each subClass As CatalogList In categories.EnumerateAllCatalogList
                subtype = New Cluster With {
                    .ID = subClass.Catalog,
                    .description = subClass.Catalog,
                    .names = subClass.Catalog,
                    .members = subClass.IDs _
                        .Select(Function(id)
                                    Return New BackgroundGene With {
                                        .accessionID = id,
                                        .[alias] = {id},
                                        .locus_tag = New NamedValue With {.name = id, .text = id},
                                        .name = id,
                                        .term_id = BackgroundGene.UnknownTerms(id).ToArray
                                    }
                                End Function) _
                        .ToArray
                }
                clusters.Add(subtype)
            Next

            Return New Background With {
                .build = Now,
                .clusters = clusters.ToArray,
                .id = "lipidmaps",
                .name = "lipidmaps structure class",
                .comments = "fisher background model for enrich lipidmaps structure class",
                .size = .clusters.BackgroundSize
            }
        End Function

        <Extension>
        Public Function CreateCategoryModel(lipidmaps As IEnumerable(Of MetaData)) As LipidMapsCategory
            Dim categories = lipidmaps.GroupBy(Function(a) a.CATEGORY).ToArray
            Dim catList As New LipidMapsCategory With {
                .[Class] = New Dictionary(Of String, CatalogProfiling)
            }

            For Each category As IGrouping(Of String, MetaData) In categories
                Dim countProfiles As New CatalogProfiling With {
                    .Catalog = category.Key,
                    .Description = .Catalog,
                    .SubCategory = New Dictionary(Of String, CatalogList)
                }
                Dim classProfiles = category.GroupBy(Function(a) a.MAIN_CLASS).ToArray
                Dim list As CatalogList

                For Each classData In classProfiles
                    list = New CatalogList With {
                        .Catalog = classData.Key,
                        .Description = classData.Key,
                        .IDs = classData _
                            .Select(Function(a) a.LM_ID) _
                            .ToArray
                    }
                    countProfiles.SubCategory.Add(classData.Key, list)
                Next

                catList.Class.Add(category.Key, countProfiles)
            Next

            Return catList
        End Function

    End Module

End Namespace
