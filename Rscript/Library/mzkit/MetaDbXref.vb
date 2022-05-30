#Region "Microsoft.VisualBasic::f28990e0d8883cd3b1b670d51954d1a9, mzkit\Rscript\Library\mzkit\MetaDbXref.vb"

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

'   Total Lines: 230
'    Code Lines: 176
' Comment Lines: 28
'   Blank Lines: 26
'     File Size: 10.94 KB


' Module MetaDbXref
' 
'     Function: cbindMeta, CreateMs1Handler, createTable, ms1Search, searchTable
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Internal.Object.Converts
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' 
''' </summary>
<Package("metadb")>
Module MetaDbXref

    Sub Main()
        Call makeDataframe.addHandler(GetType(MzQuery()), AddressOf createTable)
    End Sub

    Private Function createTable(query As MzQuery(), args As list, env As Environment) As dataframe
        Dim columns As New Dictionary(Of String, Array) From {
            {NameOf(MzQuery.unique_id), query.Select(Function(q) q.unique_id).ToArray},
            {NameOf(MzQuery.name), query.Select(Function(q) q.name).ToArray},
            {NameOf(MzQuery.mz), query.Select(Function(q) q.mz).ToArray},
            {NameOf(MzQuery.ppm), query.Select(Function(q) q.ppm).ToArray},
            {NameOf(MzQuery.precursorType), query.Select(Function(q) q.precursorType).ToArray},
            {NameOf(MzQuery.score), query.Select(Function(q) q.score).ToArray}
        }

        Return New dataframe With {
            .columns = columns,
            .rownames = columns!unique_id
        }
    End Function

    <ExportAPI("parseLipidName")>
    Public Function ParseLipidName(<RRawVectorArgument>
                                   name As Object,
                                   Optional keepsRaw As Boolean = False,
                                   Optional env As Environment = Nothing) As Object

        If keepsRaw Then
            Return SMRUCC.Rsharp.EvaluateFramework(Of String, LipidName)(env, name, eval:=AddressOf LipidName.ParseLipidName)
        Else
            Return SMRUCC.Rsharp.EvaluateFramework(Of String, list)(
                env:=env,
                x:=name,
                eval:=Function(nameStr As String)
                          Dim lipid As LipidName = LipidName.ParseLipidName(nameStr)
                          Dim rlist As New list
                          Dim chains As New list
                          Dim i As i32 = 1

                          For Each chain As Chain In lipid.chains
                              Dim chainList As New list

                              chainList.add("carbons", chain.carbons)
                              chainList.add("doubleBonds", chain.doubleBonds)
                              chainList.add("tag", chain.tag)
                              chainList.add("groups", boundList(chain.groups))
                              chainList.add("index", boundList(chain.position))

                              chains.add($"#{++i}", chainList)
                          Next

                          rlist.add("class", lipid.className)
                          rlist.add("chains", chains)
                          rlist.add("name", lipid.ToString)
                          rlist.add("overview_name", lipid.ToOverviewName)
                          rlist.add("systematic_name", lipid.ToSystematicName)

                          Return rlist
                      End Function)
        End If
    End Function

    Private Function boundList(g As Group()) As dataframe
        Dim index As Integer() = g.Select(Function(i) i.index).ToArray
        Dim t As String() = g.Select(Function(i) i.structure).ToArray
        Dim name As String() = g.Select(Function(i) i.groupName).ToArray

        If g.IsNullOrEmpty Then
            Return Nothing
        End If

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"index", index},
                {"type", t},
                {"groupName", name}
            },
            .rownames = g.Select(Function(i) i.ToString).ToArray
        }
    End Function

    Private Function boundList(b As BondPosition()) As dataframe
        Dim index As Integer() = b.Select(Function(i) i.index).ToArray
        Dim t As String() = b.Select(Function(i) i.structure).ToArray

        If b.IsNullOrEmpty Then
            Return Nothing
        End If

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"index", index},
                {"type", t}
            },
            .rownames = b.Select(Function(i) i.ToString).ToArray
        }
    End Function

    <ExportAPI("annotationStream")>
    <RApiReturn(GetType(MetaboliteAnnotation))>
    Public Function AnnotationStream(id As String(),
                                     name As String(),
                                     formula As String(),
                                     Optional env As Environment = Nothing) As Object

        If id.Length <> name.Length OrElse name.Length <> formula.Length Then
            Return Internal.debug.stop("vector size of the annotation data should be equals to each other!", env)
        Else
            Return id _
                .Select(Function(ref, i)
                            Return New MetaboliteAnnotation With {
                                .UniqueId = ref,
                                .CommonName = name(i),
                                .Formula = formula(i),
                                .ExactMass = FormulaScanner.EvaluateExactMass(.Formula)
                            }
                        End Function) _
                .ToArray
        End If
    End Function

    <ExportAPI("precursorIon")>
    Public Function ParsePrecursorIon(ion As String) As MzCalculator
        Return ParseMzCalculator(ion, ion.Last)
    End Function

    ''' <summary>
    ''' a generic function for handle ms1 search
    ''' </summary>
    ''' <param name="compounds">kegg compounds</param>
    ''' <param name="precursors"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("ms1_handler")>
    Public Function CreateMs1Handler(<RRawVectorArgument> compounds As Object,
                                     <RRawVectorArgument> precursors As Object,
                                     Optional tolerance As Object = "ppm:20",
                                     Optional env As Environment = Nothing) As Object

        Dim mzdiff = getTolerance(tolerance, env)

        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If

        Dim mz1 = GetPrecursorTypes(precursors, env)
        Dim metabolites As pipeline = pipeline.TryCreatePipeline(Of LipidMaps.MetaData)(compounds, env, suppress:=True)

        If Not metabolites.isError Then
            Return MSSearch(Of LipidMaps.MetaData).CreateIndex(metabolites.populates(Of LipidMaps.MetaData)(env), mz1, mzdiff)
        End If

        metabolites = pipeline.TryCreatePipeline(Of Compound)(compounds, env, suppress:=True)

        If Not metabolites.isError Then
            Return KEGGHandler.CreateIndex(metabolites.populates(Of Compound)(env), mz1, mzdiff.TryCast(Of Tolerance))
        End If

        metabolites = pipeline.TryCreatePipeline(Of MetaboliteAnnotation)(compounds, env, suppress:=True)

        If Not metabolites.isError Then
            Return MSSearch(Of MetaboliteAnnotation).CreateIndex(metabolites.populates(Of MetaboliteAnnotation)(env), mz1, mzdiff)
        End If

        Return metabolites.getError
    End Function

    ''' <summary>
    ''' get duplictaed raw annotation results.
    ''' </summary>
    ''' <param name="engine"></param>
    ''' <param name="mz">
    ''' a m/z numeric vector or a object list that 
    ''' contains the data mapping of unique id to 
    ''' m/z value.
    ''' </param>
    ''' <param name="uniqueByScore">
    ''' only works when <paramref name="unique"/> parameter
    ''' value is set to value TRUE.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("ms1_search")>
    <RApiReturn(GetType(MzQuery))>
    Public Function ms1Search(engine As Object,
                              <RRawVectorArgument>
                              mz As Object,
                              Optional unique As Boolean = False,
                              Optional uniqueByScore As Boolean = False,
                              Optional env As Environment = Nothing) As Object

        Dim queryEngine As IMzQuery

        If TypeOf engine Is KEGGHandler Then
            queryEngine = DirectCast(engine, KEGGHandler)
        ElseIf engine.GetType.ImplementInterface(Of IMzQuery) Then
            queryEngine = engine
        Else
            Return Internal.debug.stop("invalid handler type!", env)
        End If

        If mz Is Nothing Then
            Return Nothing
        End If

        If TypeOf mz Is list Then
            Return DirectCast(mz, list).searchMzList(queryEngine, unique, uniqueByScore, env)
        Else
            Return DirectCast(REnv.asVector(Of Double)(mz), Double()).searchMzVector(queryEngine, unique, uniqueByScore)
        End If
    End Function

    <ExportAPI("getMetadata")>
    Public Function getMetadata(engine As Object, uniqueId As list, Optional env As Environment = Nothing) As Object
        Dim queryEngine As IMzQuery

        If engine.GetType.ImplementInterface(Of IMzQuery) Then
            queryEngine = engine
        Else
            Return Internal.debug.stop("invalid handler type!", env)
        End If

        Return New list With {
            .slots = uniqueId _
                .getNames _
                .ToDictionary(Function(name) name,
                              Function(name)
                                  Dim id As String = uniqueId.getValue(Of String)(name, env, [default]:="")

                                  If id.StringEmpty Then
                                      Return Nothing
                                  Else
                                      Return queryEngine.GetMetadata(id)
                                  End If
                              End Function)
        }
    End Function

    <Extension>
    Private Function searchMzList(mz As list,
                                  queryEngine As IMzQuery,
                                  unique As Boolean,
                                  uniqueByScore As Boolean,
                                  env As Environment) As Object
        Return New list With {
            .slots = mz.slots _
                .ToDictionary(Function(id) id.Key,
                              Function(id)
                                  Dim mzi As Double = mz.getValue(Of Double)(id.Key, env)
                                  Dim all = queryEngine.QueryByMz(mzi).ToArray

                                  If unique Then
                                      If uniqueByScore Then
                                          Return all _
                                             .OrderByDescending(Function(d) d.score) _
                                             .FirstOrDefault
                                      Else
                                          Return all _
                                             .OrderBy(Function(d) d.ppm) _
                                             .FirstOrDefault
                                      End If
                                  Else
                                      Return all
                                  End If
                              End Function)
        }
    End Function

    <Extension>
    Private Function searchMzVector(mz As Double(), queryEngine As IMzQuery, unique As Boolean, uniqueByScore As Boolean) As Object
        If mz.Length = 1 Then
            Dim all = queryEngine.QueryByMz(mz(Scan0)).ToArray

            If unique Then
                If uniqueByScore Then
                    Return all _
                        .OrderByDescending(Function(d) d.score) _
                        .FirstOrDefault
                Else
                    Return all.OrderBy(Function(d) d.ppm).FirstOrDefault
                End If
            Else
                Return all
            End If
        Else
            Return New list With {
                .slots = mz _
                    .Select(Function(mzi, i) (mzi, i)) _
                    .AsParallel _
                    .Select(Function(t)
                                Dim mzi As Double = t.mzi
                                Dim i As Integer = t.i
                                Dim result As Object = queryEngine.QueryByMz(mzi).ToArray

                                If unique Then
                                    If uniqueByScore Then
                                        result = DirectCast(result, MzQuery()) _
                                            .OrderByDescending(Function(d) d.score) _
                                            .FirstOrDefault
                                    Else
                                        result = DirectCast(result, MzQuery()) _
                                            .OrderBy(Function(d) d.ppm) _
                                            .FirstOrDefault
                                    End If
                                End If

                                Return (mzi.ToString, result, i)
                            End Function) _
                    .OrderBy(Function(t) t.i) _
                    .ToDictionary(Function(mzi) mzi.Item1,
                                  Function(mzi) As Object
                                      Return mzi.Item2
                                  End Function)
            }
        End If
    End Function

    ''' <summary>
    ''' unique of the peak annotation features
    ''' </summary>
    ''' <param name="query">
    ''' all query result that comes from the ms1_search function.
    ''' </param>
    ''' <param name="scoreFactors">
    ''' the reference name this score data must be 
    ''' generated via the <see cref="MzQuery.ReferenceKey(MzQuery)"/> 
    ''' function.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("uniqueFeatures")>
    <RApiReturn(GetType(dataframe))>
    Public Function searchTable(query As list,
                                Optional uniqueByScore As Boolean = False,
                                Optional scoreFactors As list = Nothing,
                                Optional env As Environment = Nothing) As Object

        Dim mz As String() = query.getNames
        Dim scores As New Dictionary(Of String, Double)

        If Not scoreFactors Is Nothing Then
            For Each name As String In scoreFactors.getNames
                Call scores.Add(name, scoreFactors.getValue(Of Double)(name, env))
            Next
        End If

        Dim mzqueries = mz _
            .Select(Function(mzi)
                        ' unique of rows
                        Dim all As MzQuery() = query.getValue(Of MzQuery())(mzi, env)
                        Dim unique As MzQuery = Nothing

                        If Not all.IsNullOrEmpty Then
                            If uniqueByScore Then
                                If scores.Count > 0 Then
                                    unique = all _
                                        .OrderByDescending(Function(d)
                                                               Dim key As String = MzQuery.ReferenceKey(d)

                                                               If scores.ContainsKey(key) Then
                                                                   Return d.score * scores(key)
                                                               Else
                                                                   Return d.score
                                                               End If
                                                           End Function) _
                                        .First
                                Else
                                    unique = all _
                                        .OrderByDescending(Function(d) d.score) _
                                        .First
                                End If
                            Else
                                unique = all.OrderBy(Function(d) d.ppm).First
                            End If
                        End If

                        Return New NamedValue(Of MzQuery)(mzi, unique)
                    End Function) _
            .ToArray
        Dim betterJ As Boolean

        ' unique between features
        ' via min ppm?
        For i As Integer = 0 To mz.Length - 1
            If mzqueries(i).Value.isEmpty Then
                Continue For
            End If

            For j As Integer = 0 To mz.Length - 1
                If i = j OrElse mzqueries(j).Value.unique_id <> mzqueries(i).Value.unique_id Then
                    Continue For
                End If

                If uniqueByScore Then
                    If scores.Count > 0 Then
                        Dim sj As Double = mzqueries(j).Value.score
                        Dim si As Double = mzqueries(i).Value.score
                        Dim kj = MzQuery.ReferenceKey(mzqueries(j))
                        Dim ki = MzQuery.ReferenceKey(mzqueries(i))

                        If scores.ContainsKey(kj) Then
                            sj *= scores(kj)
                        End If
                        If scores.ContainsKey(ki) Then
                            si *= scores(ki)
                        End If

                        betterJ = sj > si
                    Else
                        betterJ = mzqueries(j).Value.score > mzqueries(i).Value.score
                    End If
                Else
                    betterJ = mzqueries(j).Value.ppm < mzqueries(i).Value.ppm
                End If

                If betterJ Then
                    ' j is better
                    ' set i to nothing
                    mzqueries(i) = New NamedValue(Of MzQuery)(mz(i), New MzQuery With {.mz = mzqueries(i).Value.mz})
                    Exit For
                Else
                    ' i is better
                    ' set j to nothing
                    mzqueries(j) = New NamedValue(Of MzQuery)(mz(j), New MzQuery With {.mz = mzqueries(j).Value.mz})
                End If
            Next
        Next

        Return New dataframe With {
            .rownames = mz,
            .columns = New Dictionary(Of String, Array) From {
                {"m/z", mzqueries.Select(Function(i) i.Value.mz).ToArray},
                {"theoretical_mz", mzqueries.Select(Function(i) i.Value.mz_ref).ToArray},
                {"ppm", mzqueries.Select(Function(i) i.Value.ppm).ToArray},
                {"precursor_type", mzqueries.Select(Function(i) i.Value.precursorType).ToArray},
                {"unique_id", mzqueries.Select(Function(i) i.Value.unique_id).ToArray},
                {"name", mzqueries.Select(Function(i) i.Value.name).ToArray},
                {"score", mzqueries.Select(Function(i) i.Value.score).ToArray}
            }
        }
    End Function

    <ExportAPI("cbind.metainfo")>
    Public Function cbindMeta(anno As dataframe, engine As Object, Optional env As Environment = Nothing) As Object
        If Not anno.hasName("unique_id") Then
            Return Internal.debug.stop("missing unique id of the metabolite annotation result!", env)
        End If

        If engine Is Nothing Then
            env.AddMessage("the required ms annotation engine is nothing!", MSG_TYPES.WRN)
            Return anno
        ElseIf TypeOf engine Is KEGGHandler Then
            Throw New NotImplementedException
        ElseIf TypeOf engine Is MSSearch(Of LipidMaps.MetaData) Then
            Dim data As LipidMaps.MetaData() = DirectCast(anno!unique_id, String()) _
                .Select(AddressOf DirectCast(engine, MSSearch(Of LipidMaps.MetaData)).GetCompound) _
                .ToArray

            anno.columns.Add("name", data.Select(Function(d) If(d Is Nothing, "", d.NAME)).ToArray)
            anno.columns.Add("PLANTFA_ID", data.Select(Function(d) If(d Is Nothing, "", d.PLANTFA_ID)).ToArray)
            anno.columns.Add("common_name", data.Select(Function(d) If(d Is Nothing, "", d.COMMON_NAME)).ToArray)
            anno.columns.Add("SYSTEMATIC_NAME", data.Select(Function(d) If(d Is Nothing, "", d.SYSTEMATIC_NAME)).ToArray)
            anno.columns.Add("SYNONYMS", data.Select(Function(d) If(d Is Nothing, "", d.SYNONYMS)).ToArray)
            anno.columns.Add("ABBREVIATION", data.Select(Function(d) If(d Is Nothing, "", d.ABBREVIATION)).ToArray)
            anno.columns.Add("CATEGORY", data.Select(Function(d) If(d Is Nothing, "", d.CATEGORY)).ToArray)
            anno.columns.Add("MAIN_CLASS", data.Select(Function(d) If(d Is Nothing, "", d.MAIN_CLASS)).ToArray)
            anno.columns.Add("SUB_CLASS", data.Select(Function(d) If(d Is Nothing, "", d.SUB_CLASS)).ToArray)
            anno.columns.Add("EXACT_MASS", data.Select(Function(d) If(d Is Nothing, "", d.EXACT_MASS)).ToArray)
            anno.columns.Add("FORMULA", data.Select(Function(d) If(d Is Nothing, "", d.FORMULA)).ToArray)
            anno.columns.Add("LIPIDBANK_ID", data.Select(Function(d) If(d Is Nothing, "", d.LIPIDBANK_ID)).ToArray)
            anno.columns.Add("SWISSLIPIDS_ID", data.Select(Function(d) If(d Is Nothing, "", d.SWISSLIPIDS_ID)).ToArray)
            anno.columns.Add("HMDB_ID", data.Select(Function(d) If(d Is Nothing, "", d.HMDB_ID)).ToArray)
            anno.columns.Add("PUBCHEM_CID", data.Select(Function(d) If(d Is Nothing, "", d.PUBCHEM_CID)).ToArray)
            anno.columns.Add("KEGG_ID", data.Select(Function(d) If(d Is Nothing, "", d.KEGG_ID)).ToArray)
            anno.columns.Add("CHEBI_ID", data.Select(Function(d) If(d Is Nothing, "", d.CHEBI_ID)).ToArray)
            anno.columns.Add("INCHI_KEY", data.Select(Function(d) If(d Is Nothing, "", d.INCHI_KEY)).ToArray)
            anno.columns.Add("INCHI", data.Select(Function(d) If(d Is Nothing, "", d.INCHI)).ToArray)
            anno.columns.Add("SMILES", data.Select(Function(d) If(d Is Nothing, "", d.SMILES)).ToArray)
            anno.columns.Add("CLASS_LEVEL4", data.Select(Function(d) If(d Is Nothing, "", d.CLASS_LEVEL4)).ToArray)
            anno.columns.Add("METABOLOMICS_ID", data.Select(Function(d) If(d Is Nothing, "", d.METABOLOMICS_ID)).ToArray)

            Return anno
        Else
            Return Message.InCompatibleType(GetType(KEGGHandler), engine.GetType, env)
        End If
    End Function

End Module
