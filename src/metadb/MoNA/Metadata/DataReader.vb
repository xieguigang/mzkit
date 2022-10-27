Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

Public Module DataReader

    <Extension>
    Public Iterator Function AsDataFrame(msp As IEnumerable(Of MspData)) As IEnumerable(Of EntityObject)
        Dim part1, part2 As Dictionary(Of String, String)

        For Each mspValue As MspData In msp
            part1 = mspValue.DictionaryTable(primitiveType:=True)
            part2 = mspValue.Comments.FillData.DictionaryTable

            Yield New EntityObject With {
                .ID = mspValue.DB_id,
                .Properties = part1.Join(part2).ToDictionary
            }
        Next
    End Function

    ''' <summary>
    ''' Annotation comments text parser for lipidBlast database.
    ''' </summary>
    ''' <param name="comments$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LipidBlastParser(comments$) As MetaData
        Dim meta As MetaData = MspData.ParseCommentMetaTable(comments).FillData
        Dim tokens$() = comments.Split(";"c).Skip(1).ToArray

        meta.name = Strings.Trim(tokens(0))
        meta.precursor_type = Strings.Trim(tokens(1))
        meta.scientific_name = Strings.Trim(tokens(2))
        meta.molecular_formula = Strings.Trim(tokens(3))

        Return meta
    End Function
End Module
