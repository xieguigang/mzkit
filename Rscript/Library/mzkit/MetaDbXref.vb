#Region "Microsoft.VisualBasic::1887377362559c382b1ee13475c90355, Rscript\Library\mzkit\MetaDbXref.vb"

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

' Module MetaDbXref
' 
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Internal.Object.Converts
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' 
''' </summary>
<Package("metadb")>
Module MetaDbXref

    Sub New()
        Call makeDataframe.addHandler(GetType(MzQuery()), AddressOf createTable)
    End Sub

    Private Function createTable(query As MzQuery(), args As list, env As Environment) As dataframe
        Dim columns As New Dictionary(Of String, Array) From {
            {NameOf(MzQuery.unique_id), query.Select(Function(q) q.unique_id).ToArray},
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
    ''' a generic function for handle ms1 search
    ''' </summary>
    ''' <param name="compounds"></param>
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
            Return KEGGHandler.CreateIndex(metabolites.populates(Of Compound)(env), mz1, mzdiff)
        End If

        Return metabolites.getError
    End Function

    ''' <summary>
    ''' get duplictaed raw annotation results.
    ''' </summary>
    ''' <param name="engine"></param>
    ''' <param name="mz"></param>
    ''' <returns></returns>
    <ExportAPI("ms1_search")>
    Public Function ms1Search(engine As IMzQuery, mz As Double()) As Object
        If mz.IsNullOrEmpty Then
            Return Nothing
        ElseIf mz.Length = 1 Then
            Return engine.QueryByMz(mz(Scan0)).ToArray
        Else
            Return New list With {
                .slots = mz _
                    .ToDictionary(Function(mzi) mzi.ToString,
                                  Function(mzi)
                                      Return CObj(engine.QueryByMz(mzi).ToArray)
                                  End Function)
            }
        End If
    End Function

    ''' <summary>
    ''' unique of the peak annotation features
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("uniqueFeatures")>
    <RApiReturn(GetType(dataframe))>
    Public Function searchTable(query As list, Optional env As Environment = Nothing) As Object
        Dim mz As String() = query.getNames
        Dim mzquery = mz _
            .Select(Function(mzi)
                        ' unique of rows
                        Dim all = query.getValue(Of MzQuery())(mzi, env)
                        Dim unique As MzQuery = Nothing

                        If Not all Is Nothing Then
                            unique = all.OrderBy(Function(d) d.ppm).First
                        End If

                        Return New NamedValue(Of MzQuery)(mzi, unique)
                    End Function) _
            .ToArray
        ' unique between features
        ' via min ppm?
        For i As Integer = 0 To mz.Length - 1
            If Not mzquery(i).Value.isEmpty Then
                For j As Integer = 0 To mz.Length - 1
                    If i = j Then
                        Continue For
                    ElseIf mzquery(j).Value.unique_id = mzquery(i).Value.unique_id Then
                        If mzquery(j).Value.ppm < mzquery(i).Value.ppm Then
                            ' j is better
                            mzquery(i) = Nothing
                            Exit For
                        Else
                            ' i is better
                            mzquery(j) = Nothing
                        End If
                    End If
                Next
            End If
        Next

        Return New dataframe With {
            .rownames = mz,
            .columns = New Dictionary(Of String, Array) From {
                {"m/z", mzquery.Select(Function(i) i.Value.mz).ToArray},
                {"ppm", mzquery.Select(Function(i) i.Value.ppm).ToArray},
                {"precursor_type", mzquery.Select(Function(i) i.Value.precursorType).ToArray},
                {"unique_id", mzquery.Select(Function(i) i.Value.unique_id).ToArray},
                {"score", mzquery.Select(Function(i) i.Value.score).ToArray}
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
