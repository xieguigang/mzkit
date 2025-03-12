#Region "Microsoft.VisualBasic::91ee072db4889612dc8794516cbd8cbb, metadb\Massbank\Public\HERB\HerbReader.vb"

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

    '   Total Lines: 45
    '    Code Lines: 31 (68.89%)
    ' Comment Lines: 8 (17.78%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (13.33%)
    '     File Size: 2.28 KB


    '     Module HerbReader
    ' 
    '         Function: CreateCompoundObject, LoadDatabase
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework

Namespace HERB

    ''' <summary>
    ''' data reader helper for the herbs database tables
    ''' </summary>
    Public Module HerbReader

        ''' <summary>
        ''' load the herb database folder and then assemble the <see cref="HerbCompoundObject"/> collection.
        ''' </summary>
        ''' <param name="dir"></param>
        ''' <returns></returns>
        Public Iterator Function LoadDatabase(dir As String) As IEnumerable(Of HerbCompoundObject)
            Dim disease As HERB_disease_info() = $"{dir}/HERB_disease_info.txt".LoadCsv(Of HERB_disease_info)(mute:=True, tsv:=True)
            Dim experiment As HERB_experiment_info() = $"{dir}/HERB_experiment_info.txt".LoadCsv(Of HERB_experiment_info)(mute:=True, tsv:=True)
            Dim herb_info As HERB_herb_info() = $"{dir}/HERB_herb_info.txt".LoadCsv(Of HERB_herb_info)(mute:=True, tsv:=True)
            Dim ingredient As HERB_ingredient_info() = $"{dir}/HERB_ingredient_info.txt".LoadCsv(Of HERB_ingredient_info)(mute:=True, tsv:=True)
            Dim reference As HERB_reference_info() = $"{dir}/HERB_reference_info.txt".LoadCsv(Of HERB_reference_info)(mute:=True, tsv:=True)
            Dim target_info As HERB_target_info() = $"{dir}/HERB_target_info.txt".LoadCsv(Of HERB_target_info)(mute:=True, tsv:=True)

            Dim reference_index = reference _
                .GroupBy(Function(r) r.Herb_ingredient_name) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Return r.ToArray
                              End Function)

            For Each cpd As HERB_ingredient_info In ingredient
                Yield cpd.CreateCompoundObject(
                    reference:=reference_index.TryGetValue(cpd.Ingredient_id)
                )
            Next
        End Function

        <Extension>
        Private Function CreateCompoundObject(ingredient As HERB_ingredient_info, reference As HERB_reference_info()) As HerbCompoundObject
            Return New HerbCompoundObject(ingredient) With {
                .reference = reference
            }
        End Function
    End Module
End Namespace
