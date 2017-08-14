Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Emit.Delegates

Namespace LipidMaps

    Public Module MapsHelper

        <Extension> Public Function CreateMaps(lipidMaps As IEnumerable(Of SDF)) As NamedValue(Of Dictionary(Of String, MetaData()))()
            Dim out As New List(Of NamedValue(Of Dictionary(Of String, MetaData())))
            Dim schema = DataFramework.Schema(Of MetaData)(PropertyAccess.Readable, True)
            Dim tuple As Dictionary(Of String, MetaData())

            With lipidMaps _
                .Where(Function(sdf) Not sdf.MetaData Is Nothing) _
                .Select(Function(sdf) sdf.MetaData) _
                .ToArray

                Dim map = Sub(field$, mapName$)
                              tuple = .Tuple(schema(field))
                              out += New NamedValue(Of Dictionary(Of String, MetaData())) With {
                                  .Name = mapName,
                                  .Value = tuple
                              }
                          End Sub

                Call map(NameOf(MetaData.CHEBI_ID), "chebi")
                Call map(NameOf(MetaData.HMDBID), "hmdb")
                Call map(NameOf(MetaData.INCHI), "inchi")
                Call map(NameOf(MetaData.INCHI_KEY), "inchi_key")
                Call map(NameOf(MetaData.KEGG_ID), "kegg")
                Call map(NameOf(MetaData.LIPIDBANK_ID), "lipidbank")
                Call map(NameOf(MetaData.LM_ID), "lipidmap")
                Call map(NameOf(MetaData.PUBCHEM_CID), "pubchem")
            End With

            Return out
        End Function

        <Extension>
        Private Function Tuple(lipidmaps As MetaData(), field As PropertyInfo) As Dictionary(Of String, MetaData())
            Dim read = field.PropertyGet(Of MetaData, String)
            Dim group = lipidmaps _
                .Select(Function(m) (key:=read(m), m)) _
                .Where(Function(m) Not m.key.StringEmpty) _
                .GroupBy(Function(t) t.key)
            Dim table = group.ToDictionary(
                Function(g) g.Key,
                Function(list)
                    Return list _
                        .Select(Function(t) t.Item2) _
                        .ToArray
                End Function)
            Return table
        End Function
    End Module
End Namespace