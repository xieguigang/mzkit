#Region "Microsoft.VisualBasic::582e624bf902fe455109a672ac04d71b, src\metadb\Massbank\Public\lipidMAPS\MapsHelper.vb"

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

'     Module MapsHelper
' 
'         Function: AssertMap, CreateMaps, Tuple
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

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

        ''' <summary>
        ''' parse lipid name components
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns>[lipid class -> carbons, description = raw name]</returns>
        Public Function LipidName(name As String) As NamedValue(Of String)
            Dim lipidClass As String = name.StringReplace("\(.+\)", "")
            Dim components As String() = name _
                .Match("\(.+\)") _
                .GetStackValue("(", ")") _
                .Split("/"c) _
                .Select(Function(a) a.Split("_"c)) _
                .IteratesALL _
                .ToArray
            Dim carbons As Integer()() = components _
                .Select(Function(t)
                            Return t.Split(":"c) _
                                .Select(Function(x)
                                            If x.IsPattern("d\d+") Then
                                                Return CInt(Val(x.Replace("d", "")))
                                            Else
                                                Return CInt(Val(x))
                                            End If
                                        End Function) _
                                .ToArray
                        End Function) _
                .ToArray

            If components.Length = 1 AndAlso components(Scan0).IndexOf(":"c) = -1 Then
                Return New NamedValue(Of String) With {
                    .Description = name,
                    .Name = lipidClass,
                    .Value = components(Scan0)
                }
            End If

            Dim c1 = Aggregate part In carbons Into Sum(part(0))
            Dim c2 = Aggregate part In carbons Into Sum(part(1))

            Return New NamedValue(Of String) With {
                .Name = lipidClass,
                .Value = $"{c1}:{c2}",
                .Description = name
            }
        End Function

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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CreateMeta(lipidMaps As IEnumerable(Of SDF)) As IEnumerable(Of MetaData)
            Return lipidMaps.Select(Function(sdf) MetaData.Data(sdf))
        End Function

        ''' <summary>
        ''' 创建lipidmap之中的分子信息到其他的数据库的映射
        ''' </summary>
        ''' <param name="lipidMaps"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateMaps(lipidMaps As IEnumerable(Of SDF)) As NamedValue(Of Dictionary(Of String, MetaData()))()
            Dim out As New List(Of NamedValue(Of Dictionary(Of String, MetaData())))
            Dim schema = DataFramework.Schema(Of MetaData)(PropertyAccess.Readable, True)
            Dim tuple As Dictionary(Of String, MetaData())

            With lipidMaps _
                .Where(Function(sdf) Not sdf.MetaData Is Nothing) _
                .Select(Function(sdf) MetaData.Data(sdf)) _
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
            Dim read As Func(Of MetaData, String) = field.PropertyGet(Of MetaData, String)
            Dim group = lipidmaps _
                .Select(Function(m) (key:=read(m), m)) _
                .Where(Function(m) Not m.key.StringEmpty) _
                .GroupBy(Function(t)
                             Return t.key
                         End Function) _
                .ToArray
            Dim table As Dictionary(Of String, MetaData()) = group.ToDictionary(
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
