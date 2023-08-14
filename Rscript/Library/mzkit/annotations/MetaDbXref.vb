#Region "Microsoft.VisualBasic::0b5c5fd3606501c64fd45ad5c29ef376, mzkit\Rscript\Library\mzkit\annotations\MetaDbXref.vb"

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

'   Total Lines: 726
'    Code Lines: 566
' Comment Lines: 68
'   Blank Lines: 92
'     File Size: 31.81 KB


' Module MetaDbXref
' 
'     Function: AnnotationStream, (+2 Overloads) boundList, cbindMeta, CreateMs1Handler, createTable
'               excludeFeatures, getMetadata, getVector, loadQueryHits, makeUniqueQuery
'               ms1Search, ParseLipidName, ParsePrecursorIon, search1, searchMzList
'               searchMzVector, searchTable
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Internal.Object.Converts
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' Metabolite annotation database search engine
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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="massSet"></param>
    ''' <param name="type"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns>A simple mass index search engine object instance</returns>
    <ExportAPI("mass_search.index")>
    <RApiReturn(GetType(MassSearchIndex(Of )))>
    Public Function CreateMassSearchIndex(<RRawVectorArgument>
                                          massSet As Object,
                                          type As Object,
                                          Optional tolerance As Object = "da:0.01",
                                          Optional env As Environment = Nothing) As Object

        Dim indexVal As RType = env.globalEnvironment.GetType(type)
        Dim mzdiff = Math.getTolerance(tolerance, env, [default]:="da:0.01")

        If type Is Nothing Then
            Return Internal.debug.stop("the required type information could not be nothing!", env)
        End If
        If indexVal Is Nothing Then
            Return Internal.debug.stop({$"the given type information({type}) could not be resolve in current runtime session!"}, env)
        End If
        If Not indexVal.raw.ImplementInterface(Of IExactMassProvider) Then
            Return Internal.debug.stop($"the given type information({type}) should implements the interface of '{GetType(IExactMassProvider).GetType.FullName}'!", env)
        End If
        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If

        Dim schema As Type = indexVal.raw
        Dim searchEngine As Type = GetType(MassSearchIndex(Of )).MakeGenericType(schema)
        Dim massList = REnv.asVector(massSet, schema, env)

        If Program.isException(massList) Then
            Return massList
        End If

        Dim enumerator As Type = GetType(IEnumerable(Of )).MakeGenericType(schema)
        Dim activator As New IndexEmit(schema)
        Dim newf As Object = activator.CreateActivator
        Dim argv As Object() = {
            massList, newf, mzdiff.TryCast(Of Tolerance)
        }
        Dim ctor = DelegateFactory.Contructor(searchEngine, enumerator, activator.delegate, GetType(Tolerance))
        Dim engine As Object = ctor(argv)

        Return engine
    End Function

    <ExportAPI("queryByMass")>
    Public Function QueryByMass(search As IMassSearch, mass As Double) As Object
        Return search.QueryByMass(mass).ToArray(Of Object)
    End Function

    ''' <summary>
    ''' verify that the given cas registry number is correct or not
    ''' </summary>
    ''' <param name="num"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("verify_cas_number")>
    Public Function VerifyCASNumber(<RRawVectorArgument> num As Object, Optional env As Environment = Nothing) As Object
        Return SMRUCC.Rsharp.EvaluateFramework(Of String, Boolean)(env, num, AddressOf CASNumber.Verify)
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
                eval:=AddressOf ParseLipidNameList)
        End If
    End Function

    Private Function ParseLipidNameList(nameStr As String) As list
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

    ''' <summary>
    ''' Construct a basic metabolite annotation data collection
    ''' </summary>
    ''' <param name="id">the unique reference id of the metabolite collection</param>
    ''' <param name="name">common names</param>
    ''' <param name="formula">metabolite molecular formula data collection</param>
    ''' <param name="env"></param>
    ''' <remarks>
    ''' the exact mass will be evaluated based on the input <paramref name="formula"/> data.
    ''' </remarks>
    ''' <returns></returns>
    ''' <example>
    ''' annotationStream(
    '''     id = ["met1","met2","met3"],
    '''     name = ["name1", "name2", "name3"],
    '''     formula = ["CH3OH","CH3OOOOOH","CH4NH4"]
    ''' );
    ''' </example>
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
                                .Id = ref,
                                .CommonName = name(i),
                                .Formula = formula(i),
                                .ExactMass = FormulaScanner.EvaluateExactMass(.Formula)
                            }
                        End Function) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' Create the metabolite annotation data collection based on a given set of the compound annotation data
    ''' </summary>
    ''' <param name="compounds"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("annotationStream.compounds")>
    <RApiReturn(GetType(MetaboliteAnnotation))>
    Public Function AnnotationStream(<RRawVectorArgument> compounds As Object, Optional env As Environment = Nothing) As Object
        Dim metabolites As pipeline = pipeline.TryCreatePipeline(Of LipidMaps.MetaData)(compounds, env, suppress:=True)

        If Not metabolites.isError Then
            Return metabolites.populates(Of LipidMaps.MetaData)(env) _
                .Select(Function(a) a.GetAnnotation) _
                .ToArray
        End If

        metabolites = pipeline.TryCreatePipeline(Of Compound)(compounds, env, suppress:=True)

        If Not metabolites.isError Then
            Return metabolites.populates(Of Compound)(env) _
                .Select(Function(c)
                            Return New MetaboliteAnnotation With {
                                .CommonName = c.commonNames.FirstOrDefault([default]:=c.entry),
                                .ExactMass = FormulaScanner.EvaluateExactMass(c.formula),
                                .Formula = c.formula,
                                .Id = c.entry
                            }
                        End Function) _
                .ToArray
        End If

        metabolites = pipeline.TryCreatePipeline(Of MetaboliteAnnotation)(compounds, env, suppress:=True)

        If Not metabolites.isError Then
            Return metabolites.populates(Of MetaboliteAnnotation)(env).ToArray
        End If

        Return metabolites.getError
    End Function

    ''' <summary>
    ''' parse the precursor type calculator
    ''' </summary>
    ''' <param name="ion">A precursor type string, example as ``[M+H]``.</param>
    ''' <returns></returns>
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
        Dim println = env.WriteLineHandler

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
            Return CLRVector.asNumeric(mz).searchMzVector(queryEngine, unique, uniqueByScore)
        End If
    End Function

    ''' <summary>
    ''' Found the best matched mz value with the target <paramref name="exactMass"/>
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="exactMass"></param>
    ''' <param name="adducts"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns>
    ''' function returns a evaluated mz under the specific <paramref name="adducts"/> value
    ''' and it also the min mass tolerance, if no result has mass tolerance less then the 
    ''' given threshold value, then this function returns nothing
    ''' </returns>
    <ExportAPI("searchMz")>
    <RApiReturn(GetType(MzQuery))>
    Public Function searchMz(<RRawVectorArgument> mz As Object, exactMass As Double, adducts As Object(),
                             Optional mzdiff As Object = "da:0.005",
                             Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(mzdiff, env, [default]:="da:0.005")
        Dim precursors = Math.GetPrecursorTypes(adducts, env)
        Dim mzlist As Double() = precursors _
            .Select(Function(a) a.CalcMZ(exactMass)) _
            .ToArray

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim minPpm As Double = Double.MaxValue
        Dim matchMz As Double = -1
        Dim matchType As MzCalculator = Nothing
        Dim evalMz As Double = -1
        Dim matchId As String = Nothing

        If TypeOf mz Is list Then
            Dim err As Message = Nothing
            Dim mzSet As Dictionary(Of String, Double) = DirectCast(mz, list).AsGeneric(Of Double)(env, err:=err)

            If Not err Is Nothing Then
                Return err
            End If

            Dim uniqueKeys As String() = mzSet.Keys.ToArray
            Dim candidateMz As Double() = uniqueKeys.Select(Function(key) mzSet(key)).ToArray

            For i As Integer = 0 To candidateMz.Length - 1
                For j As Integer = 0 To precursors.Length - 1
                    Dim ppm As Double = PPMmethod.PPM(candidateMz(i), mzlist(j))

                    If ppm < minPpm Then
                        minPpm = ppm
                        matchMz = candidateMz(i)
                        matchType = precursors(j)
                        evalMz = mzlist(j)
                        matchId = uniqueKeys(i)
                    End If
                Next
            Next
        Else
            Dim candidateMz As Double() = CLRVector.asNumeric(mz)

            For i As Integer = 0 To candidateMz.Length - 1
                For j As Integer = 0 To precursors.Length - 1
                    Dim ppm As Double = PPMmethod.PPM(candidateMz(i), mzlist(j))

                    If ppm < minPpm Then
                        minPpm = ppm
                        matchMz = candidateMz(i)
                        matchType = precursors(j)
                        evalMz = mzlist(j)
                    End If
                Next
            Next
        End If

        If matchMz > 0 AndAlso mzErr.TryCast(Of Tolerance).IsEquals(matchMz, evalMz) Then
            Return New MzQuery With {
                .mz = matchMz,
                .mz_ref = evalMz,
                .name = exactMass.ToString("F4"),
                .ppm = minPpm,
                .precursorType = matchType.ToString,
                .score = 1,
                .unique_id = If(matchId, .name)
            }
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' get metabolite annotation metadata by a set of given unique reference id
    ''' </summary>
    ''' <param name="engine"></param>
    ''' <param name="uniqueId"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
    Private Function search1(id As KeyValuePair(Of String, Object),
                             mz As list,
                             queryEngine As IMzQuery,
                             unique As Boolean,
                             uniqueByScore As Boolean,
                             env As Environment) As Object

        Dim mzi As Double = mz.getValue(Of Double)(id.Key, env)
        Dim all As MzQuery() = queryEngine.QueryByMz(mzi).ToArray

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
                                  Return id.search1(mz, queryEngine, unique, uniqueByScore, env)
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

    <Extension>
    Private Function makeUniqueQuery(query As list,
                                     mzi As String,
                                     uniqueByScore As Boolean,
                                     scores As Dictionary(Of String, Double),
                                     format As String,
                                     verbose As Boolean,
                                     env As Environment) As NamedValue(Of MzQuery)
        ' unique of rows
        Dim all As MzQuery() = query.getValue(Of MzQuery())(mzi, env)
        Dim unique As MzQuery = Nothing

        If Not all.IsNullOrEmpty Then
            If uniqueByScore Then
                If scores.Count > 0 Then
                    Dim hits As MzQuery() = all _
                        .Where(Function(d)
                                   Return scores.ContainsKey(MzQuery.ReferenceKey(d, format))
                               End Function) _
                        .ToArray

                    If verbose AndAlso hits.Length > 1 Then
                        Dim println As Action(Of Object) = env.WriteLineHandler

                        hits = hits _
                            .OrderByDescending(Function(d) d.score * scores(MzQuery.ReferenceKey(d, format))) _
                            .ToArray

                        Call println($"take unique [{hits(Scan0).ToString}, {hits(Scan0).name}, {hits(Scan0).score * scores(MzQuery.ReferenceKey(hits(Scan0), format))}], discards:")

                        For Each r As MzQuery In hits.Skip(1)
                            Call println($"{r.ToString}, {r.name}: {r.score * scores(MzQuery.ReferenceKey(r, format))}")
                        Next
                    End If

                    unique = all _
                        .OrderByDescending(Function(d)
                                               Dim key As String = MzQuery.ReferenceKey(d, format)

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
    End Function

    ''' <summary>
    ''' removes all of the annotation result which is not 
    ''' hits in the given ``id`` set.
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="id">the required compound id set that should be hit!</param>
    ''' <param name="field"></param>
    ''' <param name="metadb"></param>
    ''' <param name="includes_metal_ions">
    ''' removes metabolite annotation result which has metal
    ''' ions inside formula string by default.
    ''' </param>
    ''' <param name="excludes">
    ''' reverse the logical of select the annotation result 
    ''' based on the given <paramref name="id"/> set.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("excludeFeatures")>
    Public Function excludeFeatures(query As list,
                                    id As String(),
                                    field As String,
                                    metadb As IMzQuery,
                                    Optional includes_metal_ions As Boolean = False,
                                    Optional excludes As Boolean = False,
                                    Optional env As Environment = Nothing) As list

        Dim includes As Index(Of String) = id.Indexing
        Dim sublist As New list With {.slots = New Dictionary(Of String, Object)}
        Dim hits As MzQuery()

        If includes_metal_ions Then
            Call println("the metabolites which contains metal ions inside its formula string will also includes into the search result!")
        Else
            Call println("the metabolites which contains metal ions inside its formula string will be removes from the search result!")
        End If

        For Each name As String In query.getNames
            hits = query.getValue(Of MzQuery())(name, env:=env, [default]:={})
            hits = hits _
                .Where(Function(m)
                           Dim xrefs = metadb.GetDbXref(m.unique_id)
                           Dim rid As String = xrefs.TryGetValue(field)

                           ' keeps current annotation result if the 
                           ' target field id is empty
                           ' or else if the target field id is exists
                           ' in the given input id list
                           If rid.StringEmpty Then
                               Return True
                           ElseIf excludes Then
                               Return Not rid Like includes
                           Else
                               Return rid Like includes
                           End If
                       End Function) _
                .ToArray

            If hits.Length > 0 Then
                If Not includes_metal_ions Then
                    hits = hits _
                        .Where(Function(m)
                                   Dim formula As String = metadb.GetAnnotation(m.unique_id).formula
                                   Dim test1 As Boolean = MetalIons.IsOrganic(formula)
                                   Dim test2 As Boolean = Not MetalIons.HasMetalIon(formula)

                                   ' should be organic andalso not
                                   ' contains some special metal ion
                                   Return test1 AndAlso test2
                               End Function) _
                        .ToArray
                End If

                If hits.Length > 0 Then
                    Call sublist.add(name, hits)
                End If
            End If
        Next

        Return sublist
    End Function

    ''' <summary>
    ''' Check the formula string has metal ion inside?
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("has.metal_ion")>
    Public Function TestMetaIon(<RRawVectorArgument> formula As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of String, Boolean)(formula, Function(f) MetalIons.HasMetalIon(f))
    End Function

    ''' <summary>
    ''' unique of the peak annotation features
    ''' </summary>
    ''' <param name="query">
    ''' all query result that comes from the ms1_search function.
    ''' </param>
    ''' <param name="scoreFactors">
    ''' the reference name this score data must be 
    ''' generated via the <see cref="MzQuery.ReferenceKey(MzQuery,String)"/> 
    ''' function.
    ''' </param>
    ''' <param name="format">
    ''' the numeric format of the mz value for generate the reference key
    ''' </param>
    ''' <param name="removesZERO">
    ''' removes all metabolites with ZERO score?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("uniqueFeatures")>
    <RApiReturn(GetType(dataframe))>
    Public Function searchTable(query As list,
                                Optional uniqueByScore As Boolean = False,
                                Optional scoreFactors As list = Nothing,
                                Optional format As String = "F4",
                                Optional removesZERO As Boolean = False,
                                Optional verbose As Boolean = False,
                                Optional env As Environment = Nothing) As Object

        Dim mz As String() = query.getNames
        Dim scores As New Dictionary(Of String, Double)
        Dim println As Action(Of Object) = env.WriteLineHandler

        If Not verbose Then
            verbose = env.globalEnvironment.options.verbose
        End If

        If verbose Then
            Call println("the verbose unique ranking processing details will be reported!")
        End If

        If Not scoreFactors Is Nothing Then
            For Each name As String In scoreFactors.getNames
                Call scores.Add(name, scoreFactors.getValue(Of Double)(name, env))
            Next

            Call println("view of the score factor vector:")
            Call env.globalEnvironment.Rscript.Inspect(scores)
        End If

        println("make unique for the annotation features...")

        Dim mzqueries = mz _
            .Select(Function(mzi)
                        Return query.makeUniqueQuery(mzi, uniqueByScore, scores, format, verbose, env)
                    End Function) _
            .ToArray
        Dim betterJ As Boolean

        println("make unique for the annotation id...")

        ' unique between features
        ' via min ppm?
        For i As Integer = 0 To mz.Length - 1
            If MzQuery.IsNullOrEmpty(mzqueries(i).Value) Then
                Continue For
            End If

            For j As Integer = 0 To mz.Length - 1
                If MzQuery.IsNullOrEmpty(mzqueries(j).Value) Then
                    Continue For
                End If
                If i = j OrElse mzqueries(j).Value.unique_id <> mzqueries(i).Value.unique_id Then
                    Continue For
                End If

                If uniqueByScore Then
                    If scores.Count > 0 Then
                        Dim sj As Double = mzqueries(j).Value.score
                        Dim si As Double = mzqueries(i).Value.score
                        Dim kj = MzQuery.ReferenceKey(mzqueries(j), format)
                        Dim ki = MzQuery.ReferenceKey(mzqueries(i), format)
                        Dim hitAny As Boolean = False

                        If scores.ContainsKey(kj) Then
                            sj *= scores(kj)
                            hitAny = True
                        End If
                        If scores.ContainsKey(ki) Then
                            si *= scores(ki)
                            hitAny = True
                        End If

                        betterJ = sj > si

                        If hitAny AndAlso verbose Then
                            If sj > si Then
                                ' i is replaced by j
                                Call println($"'{ki}, {mzqueries(i).Name}'[score={si}] is replaced by a better result '{kj}, {mzqueries(j).Name}'[score={sj}]!")
                            Else
                                ' j is replaced by i
                                Call println($"'{kj}, {mzqueries(j).Name}'[score={sj}] is replaced by a better result '{ki}, {mzqueries(i).Name}'[score={si}]!")
                            End If
                        End If
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

        If removesZERO Then
            println("removes all result that with ZERO score!")
            mzqueries = mzqueries _
                .Select(Function(d)
                            If d.Value.score = 0 Then
                                Return New NamedValue(Of MzQuery) With {
                                    .Name = d.Name,
                                    .Description = d.Description,
                                    .Value = New MzQuery With {.mz = d.Value.mz}
                                }
                            Else
                                Return d
                            End If
                        End Function) _
                .ToArray
        End If

        println("export result table!")

        Return New dataframe With {
            .rownames = mz,
            .columns = New Dictionary(Of String, Array) From {
                {"m/z", mzqueries.getVector(Function(i) i.Value.mz)},
                {"theoretical_mz", mzqueries.getVector(Function(i) i.Value.mz_ref)},
                {"ppm", mzqueries.getVector(Function(i) i.Value.ppm)},
                {"precursor_type", mzqueries.getVector(Function(i) i.Value.precursorType)},
                {"unique_id", mzqueries.getVector(Function(i) i.Value.unique_id)},
                {"name", mzqueries.getVector(Function(i) i.Value.name)},
                {"score", mzqueries.getVector(Function(i) i.Value.score)}
            }
        }
    End Function

    <Extension>
    Private Function getVector(Of T)(mzqueries As NamedValue(Of MzQuery)(), accessor As Func(Of NamedValue(Of MzQuery), T)) As T()
        Return mzqueries _
            .Select(Function(i)
                        If i.Value Is Nothing Then
                            Return Nothing
                        Else
                            Return accessor(i)
                        End If
                    End Function) _
            .ToArray
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

    <ExportAPI("load_asQueryHits")>
    Public Function loadQueryHits(x As dataframe, Optional env As Environment = Nothing) As MzQuery()
        'unique_id,name,mz,ppm,precursorType,score
        Dim unique_id As String() = CLRVector.asCharacter(x.getColumnVector("unique_id"))
        Dim name As String() = CLRVector.asCharacter(x.getColumnVector("name"))
        Dim mz As Double() = CLRVector.asNumeric(x.getColumnVector("mz"))
        Dim ppm As Double() = CLRVector.asNumeric(x.getColumnVector("ppm"))
        Dim precursorType As String() = CLRVector.asCharacter(x.getColumnVector("precursorType"))
        Dim score As Double() = CLRVector.asNumeric(x.getColumnVector("score"))

        Return unique_id _
            .Select(Function(id, i)
                        Return New MzQuery With {
                            .unique_id = id,
                            .mz = mz(i),
                            .mz_ref = mz(i),
                            .name = name(i),
                            .ppm = ppm(i),
                            .precursorType = precursorType(i),
                            .score = score(i)
                        }
                    End Function) _
            .ToArray
    End Function

End Module
