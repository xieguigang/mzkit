Imports BioNovoGene.BioDeep.Chemistry.TMIC.FooDB
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime

<Package("foodb")>
Module foodbTools

    <ExportAPI("loadFoods")>
    Public Function loadFoods(dir As String, Optional env As Environment = Nothing) As Object
        Dim foods = $"{dir}\Food.csv".LoadCsv(Of Food)
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
    End Function
End Module
