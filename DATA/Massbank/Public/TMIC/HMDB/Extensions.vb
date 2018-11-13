#Region "Microsoft.VisualBasic::3f8a75edddc94578aaab34a337fb9e60, Massbank\Public\TMIC\HMDB\Extensions.vb"

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

    '     Module HMDBExtensions
    ' 
    '         Function: BioSamples, BuildAsTable, (+2 Overloads) CheckNames, ConcentrationDisplay, LoadHMDBTaxonomy
    '                   LoadXML, MatchMetabolites, matchSampleType, NameMatch, (+2 Overloads) water_solubility
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace TMIC.HMDB

    Public Module HMDBExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function BioSamples(metabolite As metabolite) As String()
            Return metabolite.biospecimen_locations.biospecimen.AsList + metabolite.tissue_locations.tissue
        End Function

        ''' <summary>
        ''' Build hmdb metabolite classify table.
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        <Extension> Public Function LoadHMDBTaxonomy(path$) As Dictionary(Of String, taxonomy)
            Return metabolite.Load(path) _
                .Where(Function(x) Not x.taxonomy Is Nothing) _
                .Select(Function(metabolite)
                            Return metabolite.secondary_accessions _
                                .accession _
                                .SafeQuery _
                                .JoinIterates({metabolite.accession}) _
                                .Select(Function(id)
                                            Return (id, metabolite.taxonomy)
                                        End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(x) x.Item1) _
                .ToDictionary(Function(g) g.Key,
                              Function(taxonomy)
                                  Return taxonomy.First.Item2
                              End Function)
        End Function

        ''' <summary>
        ''' 返回一个函数指针，指示目标代谢物是否被匹配上？
        ''' 如果被匹配上则会返回HMDB之中的化合物的名称，否则返回空字符串
        ''' </summary>
        ''' <param name="names$"></param>
        ''' <returns></returns>
        <Extension> Public Function NameMatch(names$()) As Func(Of metabolite, (match$, type$))
            If names.IsNullOrEmpty Then
                Return Function(metabolite)
                           Return (metabolite.name, NameOf(names))
                       End Function
            Else
                Return Function(metabolite)
                           With metabolite
                               For Each name As String In names
                                   If .name.TextEquals(name) Then
                                       Return (.name, NameOf(.name))
                                   End If

                                   For Each synonym As String In .synonyms.synonym.SafeQuery
                                       If synonym.TextEquals(name) Then
                                           Return (synonym, NameOf(.synonyms))
                                       End If
                                   Next
                               Next

                               Return Nothing
                           End With
                       End Function
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="metabolites">HMDB subset from <see cref="MatchMetabolites"/></param>
        ''' <param name="names$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CheckNames(metabolites As IEnumerable(Of metabolite), names$()) As IEnumerable(Of NameValue)
            Return CheckNames(metabolites, names.SafeQuery.Select(Function(name, i) New NamedValue(Of String)(i, name)))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="metabolites">HMDB subset from <see cref="MatchMetabolites"/></param>
        ''' <param name="names"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function CheckNames(metabolites As IEnumerable(Of metabolite), names As IEnumerable(Of NamedValue(Of String))) As IEnumerable(Of NameValue)
            Dim list As metabolite() = metabolites.ToArray

            For Each name As NamedValue(Of String) In names
                Dim handle = NameMatch(names:={name.Value})

                For Each metabolite As metabolite In list
                    Dim match As (match$, type$) = handle(metabolite)

                    If Not match.match.StringEmpty Then
                        Yield New NameValue With {
                            .name = name.Value,
                            .match = match.match,
                            .metabolite = metabolite.name,
                            .type = match.type,
                            .ID = name.Name
                        }
                    Else
                        Yield New NameValue With {
                            .name = name.Value,
                            .match = "NA",
                            .metabolite = "NA",
                            .type = "NULL",
                            .ID = name.Name
                        }
                    End If
                Next
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MatchMetabolites(source As IEnumerable(Of metabolite), list$()) As IEnumerable(Of metabolite)
            With list.NameMatch
                Return source.AsParallel.Where(Function(m) Not .ByRef(m).match.StringEmpty)
            End With
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadXML(path$) As IEnumerable(Of metabolite)
            Return metabolite.Load(path)
        End Function

        ''' <summary>
        ''' 将HMDB导出为csv格式，假若<paramref name="names"/>不为空的话，还会按照这个列表进行筛选
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <param name="names$"></param>
        ''' <returns></returns>
        Public Iterator Function BuildAsTable(path$, Optional names$() = Nothing) As IEnumerable(Of BriefTable)
            Dim matchName = (names Or New String() {}.AsDefault).NameMatch

            For Each metabolite As metabolite In metabolite.Load(path)
                Dim samples$() = metabolite _
                    .biospecimen_locations _
                    .biospecimen _
                    .SafeQuery _
                    .ToArray
                Dim disease$() = metabolite _
                    .diseases _
                    .SafeQuery _
                    .Select(Function(dis) dis.name) _
                    .ToArray

                If samples.Length = 0 Then
                    samples = {"not_specific"}
                End If

                Dim name$ = matchName(metabolite).match

                If name.StringEmpty Then
                    Continue For
                End If

                Dim table As New BriefTable With {
                    .CAS = metabolite.cas_registry_number,
                    .chebi = metabolite.chebi_id,
                    .formula = metabolite.chemical_formula,
                    .HMDB = metabolite.accession,
                    .KEGG = metabolite.kegg_id,
                    .mass = metabolite.average_molecular_weight,
                    .water_solubility = metabolite.experimental_properties.water_solubility,
                    .disease = disease,
                    .name = name
                }

                For Each sampleName As String In samples
                    Dim data = DirectCast(table.Clone, BriefTable)

                    With data
                        .Sample = sampleName
                        .NewbornConcentrationNormal = metabolite.normal_concentrations.ConcentrationDisplay(PeopleAgeTypes.Newborn, sampleName)
                        .NewbornConcentrationAbnormal = metabolite.abnormal_concentrations.ConcentrationDisplay(PeopleAgeTypes.Newborn, sampleName)
                        .AdultConcentrationAbnormal = metabolite.abnormal_concentrations.ConcentrationDisplay(PeopleAgeTypes.Adult, sampleName)
                        .AdultConcentrationNormal = metabolite.normal_concentrations.ConcentrationDisplay(PeopleAgeTypes.Adult, sampleName)
                        .ChildrenConcentrationAbnormal = metabolite.abnormal_concentrations.ConcentrationDisplay(PeopleAgeTypes.Children, sampleName)
                        .ChildrenConcentrationNormal = metabolite.normal_concentrations.ConcentrationDisplay(PeopleAgeTypes.Children, sampleName)
                    End With

                    Yield data
                Next
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ConcentrationDisplay(concentrations As IEnumerable(Of concentration), type As PeopleAgeTypes, Optional sample$ = "*") As String()
            Return concentrations _
                .Where(Function(c) c.AgeType = type AndAlso c.matchSampleType(type:=sample)) _
                .Select(Function(c)
                            Dim value$ = c.concentration_value

                            If Not c.concentration_units.StringEmpty Then
                                value &= $" ({c.concentration_units})"
                            End If

                            If sample.StringEmpty OrElse sample = "*" Then
                                Return $"[{c.biofluid}] {value}"
                            Else
                                Return value
                            End If
                        End Function) _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function matchSampleType(c As concentration, type$) As Boolean
            Return type = "*" OrElse c.biofluid.TextEquals(type)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function water_solubility(properties As Properties) As String
            Return properties.PropertyList _
                .SafeQuery _
                .Where(Function(prop) prop.kind = NameOf(water_solubility)) _
                .FirstOrDefault _
                .value
        End Function

        <Extension>
        Public Function water_solubility(metabolite As metabolite) As String
            Dim ws = metabolite _
                .experimental_properties _
                .water_solubility

            If ws Is Nothing Then
                ws = metabolite _
                    .predicted_properties _
                    .water_solubility

                If Not ws Is Nothing Then
                    ws = ws & " (predicted)"
                End If
            End If

            If ws Is Nothing Then
                ws = "NA"
            Else
                ws = ws.Replace("°C", "Centigrade")
            End If

            Return ws
        End Function
    End Module
End Namespace
