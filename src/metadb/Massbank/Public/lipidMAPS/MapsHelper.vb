#Region "Microsoft.VisualBasic::2bfc36933a2648dd04d693a369462144, G:/mzkit/src/metadb/Massbank//Public/lipidMAPS/MapsHelper.vb"

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

    '   Total Lines: 193
    '    Code Lines: 141
    ' Comment Lines: 27
    '   Blank Lines: 25
    '     File Size: 7.64 KB


    '     Class LipidClassReader
    ' 
    '         Properties: lipids
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: EnumerateId, GetClass
    ' 
    '     Class LipidNameReader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetName
    ' 
    '     Module MapsHelper
    ' 
    '         Function: AssertMap, CreateMaps, CreateMeta, Tuple
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace LipidMaps

    ''' <summary>
    ''' A helper module for get lipidmaps <see cref="CompoundClass"/> information via a given lipidmaps id
    ''' </summary>
    ''' <remarks>
    ''' the lipidmaps metabolite data in this module is indexed via the lipidmaps id: <see cref="MetaData.LM_ID"/>.
    ''' </remarks>
    Public Class LipidClassReader : Inherits ClassReader

        ''' <summary>
        ''' the lipidmaps database was indexed via the lipidmaps id at here
        ''' </summary>
        ''' <remarks>
        ''' the key is the lipidmaps id <see cref="MetaData.LM_ID"/>
        ''' </remarks>
        ReadOnly index As Dictionary(Of String, MetaData)

        ''' <summary>
        ''' get number of the lipids inside the database
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property lipids As Integer
            Get
                Return index.TryCount
            End Get
        End Property

        Sub New(lipidmaps As IEnumerable(Of MetaData))
            index = lipidmaps _
                .GroupBy(Function(i) i.LM_ID) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First
                              End Function)
        End Sub

        ''' <summary>
        ''' get lipidmaps class information via a given lipidmaps id 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns>this function may returns nothing if the given <paramref name="id"/>
        ''' is not exists inside the database index.</returns>
        Public Overrides Function GetClass(id As String) As CompoundClass
            If index.ContainsKey(id) Then
                Dim lipid As MetaData = index(id)
                Dim [class] As New CompoundClass With {
                    .kingdom = "Lipids",
                    .super_class = lipid.CATEGORY,
                    .[class] = lipid.MAIN_CLASS,
                    .sub_class = lipid.SUB_CLASS,
                    .molecular_framework = lipid.CLASS_LEVEL4
                }

                Return [class]
            Else
                Return Nothing
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function EnumerateId() As IEnumerable(Of String)
            Return index.Keys
        End Function
    End Class

    Public Class LipidNameReader : Inherits CompoundNameReader

        ReadOnly index As Dictionary(Of String, MetaData)

        Sub New(lipidmaps As IEnumerable(Of MetaData))
            index = lipidmaps _
                .GroupBy(Function(i) i.LM_ID) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First
                              End Function)
        End Sub

        Public Overrides Function GetName(id As String) As String
            If index.ContainsKey(id) Then
                Return index(id).ABBREVIATION
            Else
                Return Nothing
            End If
        End Function
    End Class

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
