Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language

Namespace LipidMaps

    Public Module MapsHelper

        Public Const chebi$ = NameOf(chebi)
        Public Const hmdb$ = NameOf(hmdb)
        Public Const inchi$ = NameOf(inchi)
        Public Const inchi_key$ = NameOf(inchi_key)
        Public Const kegg$ = NameOf(kegg)
        Public Const lipidbank$ = NameOf(lipidbank)
        Public Const lipidmap$ = NameOf(lipidmap)
        Public Const pubchem$ = NameOf(pubchem)

        <Extension>
        Public Function AssertMap(maps As NamedValue(Of Dictionary(Of String, MetaData()))(), xref As Dictionary(Of String, String)) As String
            For Each map In maps
                With map
                    If xref.ContainsKey(.Name) Then
                        Dim id$ = xref(.Name)
                        If .Value.ContainsKey(id) Then
                            Return .Value(id) _
                                .Select(Function(x) x.LM_ID) _
                                .Distinct _
                                .JoinBy(", ")
                        End If
                    End If
                End With
            Next

            Return Nothing
        End Function

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

                Call map(NameOf(MetaData.CHEBI_ID), chebi)
                Call map(NameOf(MetaData.HMDBID), hmdb)
                Call map(NameOf(MetaData.INCHI), inchi)
                Call map(NameOf(MetaData.INCHI_KEY), inchi_key)
                Call map(NameOf(MetaData.KEGG_ID), kegg)
                Call map(NameOf(MetaData.LIPIDBANK_ID), lipidbank)
                Call map(NameOf(MetaData.LM_ID), lipidmap)
                Call map(NameOf(MetaData.PUBCHEM_CID), pubchem)
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