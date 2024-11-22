Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv

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