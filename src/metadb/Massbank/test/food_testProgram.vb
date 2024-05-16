#Region "Microsoft.VisualBasic::0573a25d0345577aecf5f74eacb35120, metadb\Massbank\test\food_testProgram.vb"

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

    '   Total Lines: 156
    '    Code Lines: 123
    ' Comment Lines: 4
    '   Blank Lines: 29
    '     File Size: 8.46 KB


    ' Module Program
    ' 
    '     Sub: buildDb, flavorCluster, FoodMatrix, gsea2, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.Massbank.FooDB
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Module Program
    Sub Main(args As String())
        ' Call gsea2()
        Call flavorCluster()

        ' Call buildDb()
        ' Call FoodMatrix()
    End Sub

    Sub buildDb()
        Dim foods = "D:\biodeep\flavor\foodb\table\Food.csv".LoadCsv(Of Food)
        Dim flavor = "D:\biodeep\flavor\foodb\table\Flavor.csv".LoadCsv(Of Flavor).GroupBy(Function(d) d.id).ToDictionary(Function(d) d.Key, Function(d) d.First.name)
        Dim compounds = "D:\biodeep\flavor\foodb\table\Compound.csv".LoadCsv(Of Compound).GroupBy(Function(d) d.id).ToDictionary(Function(d) d.Key, Function(d) d.First)
        Dim compoundFlavor = "D:\biodeep\flavor\foodb\table\CompoundsFlavor.csv".LoadCsv(Of CompoundFlavor) _
            .Where(Function(d) d.source_type = "Compound").GroupBy(Function(d) d.compound_id).ToDictionary(Function(d) d.Key, Function(d) d.Where(Function(a) flavor.ContainsKey(a.flavor_id)).Select(Function(a) flavor(a.flavor_id)).ToArray)
        ' food -> compounds
        Dim contents = "D:\biodeep\flavor\foodb\table\Content.csv".LoadCsv(Of Content).Where(Function(d) d.source_type <> "Nutrient") _
            .Where(Function(d) compoundFlavor.ContainsKey(d.source_id)) _
            .GroupBy(Function(d) d.food_id) _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Return d.ToArray
                          End Function)

        Dim foodsData = foods.Where(Function(f) contents.ContainsKey(f.id)) _
            .ToDictionary(Function(d) d.public_id,
                          Function(food)
                              Dim compoundList = contents(food.id)

                              Return New FoodData(food) With {
                                  .contents = compoundList,
                                  .compounds = compoundList.Where(Function(c) compounds.ContainsKey(c.source_id)).Select(Function(c) compounds(c.source_id)).GroupBy(Function(d) d.id).Select(Function(d) d.First).ToArray,
                                  .compoundFlavors = compoundList.Where(Function(c) compoundFlavor.ContainsKey(c.source_id)).GroupBy(Function(d) d.source_id).ToDictionary(Function(c) c.Key, Function(c) compoundFlavor(c.Key))
                              }
                          End Function)


        Call foodsData.GetJson.SaveTo(foodDataJson)

        Pause()
    End Sub

    Const foodDataJson As String = "D:\biodeep\flavor\foodb\table\FoodFlavorDb.json"

    Sub flavorCluster()
        Dim compounds = "D:\biodeep\flavor\foodb\table\Compound.csv".LoadCsv(Of Compound).GroupBy(Function(d) d.id).ToDictionary(Function(d) d.Key, Function(d) d.First)
        Dim flavor = "D:\biodeep\flavor\foodb\table\Flavor.csv".LoadCsv(Of Flavor).GroupBy(Function(d) d.id).ToDictionary(Function(d) d.Key, Function(d) d.First.name)
        Dim compoundFlavor = "D:\biodeep\flavor\foodb\table\CompoundsFlavor.csv".LoadCsv(Of CompoundFlavor) _
         .Where(Function(d) d.source_type = "Compound").GroupBy(Function(d) d.compound_id).ToDictionary(Function(d) d.Key, Function(d) d.Where(Function(a) flavor.ContainsKey(a.flavor_id)).Select(Function(a) flavor(a.flavor_id)).ToArray)

        Dim flavors = compoundFlavor.Select(Function(d) d.Value.Select(Function(fid) (cid:=d.Key, fid))) _
            .IteratesALL.GroupBy(Function(d) d.fid) _
            .ToDictionary(Function(d) d.Key, Function(d)
                                                 Return d.Where(Function(c) compounds.ContainsKey(c.cid)).GroupBy(Function(a) compounds(a.cid).public_id).Select(Function(a) compounds(a.First.cid)).ToArray
                                             End Function)
        Dim flavorClusters = flavors _
            .Select(Function(f)
                        Return New Cluster With {
                            .ID = f.Key,
                            .names = f.Key,
                            .description = f.Key,
                            .members = f.Value _
                                .Select(Function(cc As Compound)
                                            Dim id = cc.public_id
                                            Return New BackgroundGene With {
                                                .accessionID = id,
                                                .[alias] = {cc.moldb_iupac, cc.moldb_smiles},
                                                .locus_tag = New NamedValue With {
                                                    .name = id,
                                                    .text = cc.description
                                                },
                                                .name = cc.name,
                                                .term_id = {id, cc.cas_number}
                                            }
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Dim background As New Background With {.clusters = flavorClusters}

        Call background.GetXml.SaveTo("D:\biodeep\flavor\foodb\table\FoodFlavorClusters.XML")

        Pause()
    End Sub

    Sub gsea2()

        Dim background As Background = "D:\biodeep\flavor\foodb\table\FoodFlavorClusters.XML".LoadXml(Of Background)
        Dim id = "D:\biodeep\flavor\foodb\Rscript\visual\test.txt".ReadAllText.Split(ASCII.TAB)
        Dim result = background.Enrichment(id,).FDRCorrection.ToArray

        Call result.SaveTo("D:\biodeep\flavor\foodb\Rscript\visual\test_enrich.csv")

        Pause()

    End Sub

    Sub FoodMatrix()
        Dim foodData = foodDataJson.LoadJsonFile(Of Dictionary(Of String, FoodData))
        Dim flavorTags = foodData.Select(Function(d) d.Value.compoundFlavors.Values).IteratesALL.IteratesALL.Distinct.ToArray
        Dim compoundlist = foodData.Values.Select(Function(d) d.contents.Select(Function(c) $"{c.source_id}-{c.food_id}")).IteratesALL.Distinct.ToArray

        Call flavorTags.SaveTo("D:\biodeep\flavor\foodb\table\FlavorTags.txt")
        Call VBDebugger.WaitOutput()
        Call Console.WriteLine(compoundlist.Length)

        Dim allcompoundsId As String() = foodData.Values.Select(Function(d) d.compounds).IteratesALL.Select(Function(c) c.public_id).Distinct.OrderBy(Function(id) id).ToArray
        Dim allflavorId As String() = foodData.Values.Select(Function(d) d.compoundFlavors.Values).IteratesALL.IteratesALL.Distinct.ToArray
        Dim mat As New List(Of DataSet)
        Dim foodRow As DataSet
        Dim internal As Index(Of String)

        For Each food In foodData.Values
            internal = food.compounds.Select(Function(c) c.public_id).Distinct.Indexing
            foodRow = New DataSet With {
                .ID = food.name,
                .Properties = allcompoundsId.ToDictionary(Function(id) id, Function(id)
                                                                               Return CDbl(If(internal.IndexOf(id) > -1, 1, 0))
                                                                           End Function)
            }

            mat.Add(foodRow)
        Next

        Call mat.SaveTo("D:\biodeep\flavor\foodb\table\FoodCompoundMatrix.csv")

        mat.Clear()

        For Each food In foodData.Values
            internal = food.compoundFlavors.Select(Function(c) c.Value).IteratesALL.Distinct.Indexing
            foodRow = New DataSet With {
                .ID = food.name,
                .Properties = allflavorId.ToDictionary(Function(id) id, Function(id)
                                                                            Return CDbl(If(internal.IndexOf(id) > -1, 1, 0))
                                                                        End Function)
            }

            mat.Add(foodRow)
        Next

        Call mat.SaveTo("D:\biodeep\flavor\foodb\table\FoodFlavorMatrix.csv")
        Pause()
    End Sub
End Module
