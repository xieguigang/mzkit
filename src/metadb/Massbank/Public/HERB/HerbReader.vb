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
            Dim disease As HERB_disease_info() = $"{dir}/HERB_disease_info.txt".LoadCsv(Of HERB_disease_info)
        End Function
    End Module
End Namespace