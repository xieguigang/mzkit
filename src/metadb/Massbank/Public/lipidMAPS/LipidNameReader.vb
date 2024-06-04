Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models

Namespace LipidMaps

    Public Class LipidNameReader : Inherits CompoundNameReader

        ''' <summary>
        ''' index by lipidmaps id
        ''' </summary>
        ReadOnly index As Dictionary(Of String, MetaData)

        Sub New(lipidmaps As IEnumerable(Of MetaData))
            index = lipidmaps _
                .GroupBy(Function(i) i.LM_ID) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First
                              End Function)
        End Sub

        ''' <summary>
        ''' get lipid <see cref="MetaData.ABBREVIATION"/> name by its id
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Overrides Function GetName(id As String) As String
            If index.ContainsKey(id) Then
                Return index(id).ABBREVIATION
            Else
                Return Nothing
            End If
        End Function
    End Class

End Namespace