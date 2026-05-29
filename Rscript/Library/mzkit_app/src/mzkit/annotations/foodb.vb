#Region "Microsoft.VisualBasic::c0b82c1967989d2000627709752429b3, Rscript\Library\mzkit_app\src\mzkit\annotations\foodb.vb"

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

    '   Total Lines: 73
    '    Code Lines: 61 (83.56%)
    ' Comment Lines: 7 (9.59%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 5 (6.85%)
    '     File Size: 3.76 KB


    ' Module foodbTools
    ' 
    '     Function: loadFoods
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.TMIC.FooDB
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("foodb")>
<RTypeExport("foodb_compound", GetType(Compound))>
<RTypeExport("foodb_content", GetType(Content))>
<RTypeExport("foodb_flavor", GetType(Flavor))>
<RTypeExport("foodb_compoundFlavor", GetType(CompoundFlavor))>
<RTypeExport("foodb_food", GetType(Food))>
Module foodbTools

    ''' <summary>
    ''' Load foodb database csv dump
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("loadFoods")>
    Public Function loadFoods(dir As String, Optional env As Environment = Nothing) As Object
        Dim foods = $"{dir}/Food.csv".LoadCsv(Of Food)
        Dim flavor = $"{dir}/Flavor.csv".LoadCsv(Of Flavor).GroupBy(Function(d) d.id).ToDictionary(Function(d) d.Key, Function(d) d.First.name)
        Dim compounds = $"{dir}/Compound.csv".LoadCsv(Of Compound).GroupBy(Function(d) d.id).ToDictionary(Function(d) d.Key, Function(d) d.First)
        Dim compoundFlavor = $"{dir}/CompoundsFlavor.csv".LoadCsv(Of CompoundFlavor) _
            .Where(Function(d) d.source_type = "Compound") _
            .GroupBy(Function(d) d.compound_id) _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Return d _
                                  .Where(Function(a) flavor.ContainsKey(a.flavor_id)) _
                                  .Select(Function(a) flavor(a.flavor_id)) _
                                  .ToArray
                          End Function)
        ' food -> compounds
        Dim contents = $"{dir}/Content.csv".LoadCsv(Of Content) _
            .Where(Function(d) d.source_type <> "Nutrient") _
            .Where(Function(d) compoundFlavor.ContainsKey(d.source_id)) _
            .GroupBy(Function(d) d.food_id) _
            .ToDictionary(Function(d) d.Key,
                          Function(d)
                              Return d.ToArray
                          End Function)

        Dim foodsData = foods _
            .Where(Function(f) contents.ContainsKey(f.id)) _
            .ToDictionary(Function(d) d.public_id,
                          Function(food)
                              Dim compoundList = contents(food.id)

                              Return New FoodData(food) With {
                                    .contents = compoundList,
                                    .compounds = compoundList _
                                        .Where(Function(c) compounds.ContainsKey(c.source_id)) _
                                        .Select(Function(c) compounds(c.source_id)) _
                                        .GroupBy(Function(d) d.id) _
                                        .Select(Function(d) d.First) _
                                        .ToArray,
                                    .compoundFlavors = compoundList _
                                        .Where(Function(c) compoundFlavor.ContainsKey(c.source_id)) _
                                        .GroupBy(Function(d) d.source_id) _
                                        .ToDictionary(Function(c) c.Key,
                                                    Function(c)
                                                        Return compoundFlavor(c.Key)
                                                    End Function)
                                }
                          End Function)

        Return foodsData
    End Function
End Module
