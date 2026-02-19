#Region "Microsoft.VisualBasic::64d52a69f8588c9388cd3dee1e7d3c48, metadna\metaDNA\Result\ResultHandler.vb"

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

'   Total Lines: 147
'    Code Lines: 89 (60.54%)
' Comment Lines: 43 (29.25%)
'    - Xml Docs: 16.28%
' 
'   Blank Lines: 15 (10.20%)
'     File Size: 10.08 KB


' Module ResultHandler
' 
'     Function: ExportTable, FeatureUniques, GetUniques
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Module ResultHandler

    ''' <summary>
    ''' export all infer result
    ''' </summary>
    ''' <param name="candidates"></param>
    ''' <param name="kegg"></param>
    ''' <param name="keggNetwork"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function ExportTable(candidates As IEnumerable(Of CandidateInfer),
                                         kegg As MSSearch(Of GenericCompound),
                                         keggNetwork As Networking) As IEnumerable(Of MetaDNAResult)

        Dim precursorTypes As Dictionary(Of String, MzCalculator) = kegg.Calculators

        For Each infer As CandidateInfer In candidates
            Dim compound As GenericCompound = kegg.GetCompound(infer.kegg_id)

            For Each type As Candidate In infer.infers
                Dim partner As String = type.infer.reference.id.Split(":"c).Last.Trim
                Dim links As NamedValue(Of String)() = keggNetwork.FindReactions(partner, compound.Identity)

                ' 20240814
                ' links maybe empty collection
                ' 
                ' Error in <globalEnvironment@NP2212020456> -> snowfall_prallel_RPC_slave_node%&H0b000000@http://172.17.0.22:43680/ -> "slaveTask"(&argv...)(&argv...)(&argv...) -> R_invoke$slaveTask -> ".summaryTaskResult"(...)(Call ".annoReport"(Call ".protoc...) -> ".annoReport"(...)(Call ".protocol_workflow"(Call "...) -> ".protocol_workflow"(...)(Call ".raw_preprocessor"(Call "....) -> R_invoke$.protocol_workflow -> "protocol"(&args...)(&args...) -> R_invoke$workflow.OTCML_annotation -> "__searchMultiple"("rdaNames" <- &rdaNames, "args" ...)(&rawdata, "rdaNames" <- &rdaName...) -> R_invoke$__searchMultiple -> if_closure -> R_invoke$if_closure_internal -> if_closure -> R_invoke$if_closure_internal -> "ontology_search"(&rawdata, &args, "biodeep_id" <-...)(&rawdata, &args, "biodeep_id" <-...) -> R_invoke$ontology_search -> "as.table"(&dia_search, &dia_infer, "unique...)(&dia_search, &dia_infer, "unique...) -> as.table
                '   1. IndexOutOfRangeException: Index was outside the bounds of the array.
                '   2. stackFrames: 
                '    at BioNovoGene.BioDeep.MetaDNA.ResultHandler.ExportTable(IEnumerable`1 candidates, MSSearch`1 kegg, Networking keggNetwork)+MoveNext()
                '    at System.Collections.Generic.EnumerableHelpers.ToArray[T](IEnumerable`1 source, Int32& length)
                '    at System.Linq.Buffer`1..ctor(IEnumerable`1 source)
                '    at System.Linq.OrderedEnumerable`1.GetEnumerator()+MoveNext()
                '    at BioNovoGene.BioDeep.MetaDNA.MetaDNAResult.FilterInferenceHits(IEnumerable`1 result, Double cutoff)+MoveNext()
                '    at System.Collections.Generic.LargeArrayBuilder`1.AddRange(IEnumerable`1 items)
                '    at System.Collections.Generic.EnumerableHelpers.ToArray[T](IEnumerable`1 source)
                '    at mzkit.metaDNAInfer.ResultTable(Algorithm metaDNA, Object result, Boolean unique, Double cutoff, Environment env)

                '    Dim dia_table As generic = Call "as.table"(&dia_search, &dia_infer, "unique" <- False, "cutoff" <- &filter_cutoff)
                '    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                ' metadna.R#_clr_interop::.as.table at [mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]:line &Hx055624e00
                ' biodeepMSMS.call_function."as.table"(&dia_search, &dia_infer, "unique...)(&dia_search, &dia_infer, "unique...) at ontology_search.R:line 77
                ' biodeepMSMS.declare_function.R_invoke$ontology_search at ontology_search.R:line 6
                ' biodeepMSMS.call_function."ontology_search"(&rawdata, &args, "biodeep_id" <-...)(&rawdata, &args, "biodeep_id" <-...) at searchMultiple.R:line 114
                ' unknown.unknown.R_invoke$if_closure_internal at n/a:line n/a
                ' biodeepMSMS.n/a.if_closure at searchMultiple.R:line 110
                ' unknown.unknown.R_invoke$if_closure_internal at n/a:line n/a
                ' biodeepMSMS.n/a.if_closure at searchMultiple.R:line 108
                ' biodeepMSMS.declare_function.R_invoke$__searchMultiple at searchMultiple.R:line 11
                ' biodeepMSMS.call_function."__searchMultiple"("rdaNames" <- &rdaNames, "args" ...)(&rawdata, "rdaNames" <- &rdaName...) at OTCML.R:line 30
                ' biodeepMSMS.declare_function.R_invoke$workflow.OTCML_annotation at OTCML.R:line 6
                ' biodeepMSMS.call_function."protocol"(&args...)(&args...) at run_workflow.R:line 74
                ' biodeepMSMS.declare_function.R_invoke$.protocol_workflow at run_workflow.R:line 71
                ' biodeepMSMS.call_function.".protocol_workflow"(...)(Call ".raw_preprocessor"(Call "....) at slaveTask.R:line 12
                ' biodeepMSMS.call_function.".annoReport"(...)(Call ".protocol_workflow"(Call "...) at slaveTask.R:line 13
                ' biodeepMSMS.call_function.".summaryTaskResult"(...)(Call ".annoReport"(Call ".protoc...) at slaveTask.R:line 14
                ' biodeepMSMS.declare_function.R_invoke$slaveTask at slaveTask.R:line 6
                ' runSlaveNode.call_function."slaveTask"(&argv...)(&argv...)(&argv...) at slave.R:line 67
                ' unknown.unknown.snowfall_prallel_RPC_slave_node%&H0b000000@http://172.17.0.22:43680/ at n/a:line n/a
                ' SMRUCC/R#.global.<globalEnvironment@NP2212020456> at <globalEnvironment>:line n/a
                Dim reaction As NamedValue(Of String) = If(
                    links.IsNullOrEmpty,
                    New NamedValue(Of String)("Missing", "Missing"),
                    links(Scan0)
                )

                Yield New MetaDNAResult With {
                    .exactMass = compound.ExactMass,
                    .formula = compound.Formula,
                    .query_id = type.infer.query.id,
                    .forward = type.infer.forward,
                    .reverse = type.infer.reverse,
                    .jaccard = type.infer.jaccard,
                    .inferLevel = type.infer.level.Description,
                    .KEGGId = infer.kegg_id,
                    .name = If(compound.CommonName, compound.Formula),
                    .ppm = type.ppm,  ' round ppm in the external code, just export the raw value at here
                    .precursorType = type.precursorType,
                    .pvalue = type.pvalue,
                    .partnerKEGGId = partner,
                    .seed = type.infer.reference.id,
                    .mz = type.infer.query.mz,
                    .rt = type.infer.query.scan_time,
                    .intensity = type.infer.query.intensity,
                    .KEGG_reaction = reaction.Name,
                    .reaction = reaction.Value.LineTokens.Distinct.JoinBy("; "),
                    .parentTrace = type.infer.parentTrace / 100,
                    .inferSize = type.infer.inferSize,
                    .fileName = type.infer.rawFile,
                    .mzCalc = precursorTypes(.precursorType).CalcMZ(.exactMass),
                    .ROI_id = type.ROI,
                    .mirror = type.infer.mirror,
                    .alignment = MetaDNAResult.GetAlignment(type.infer).JoinBy(" "),
                    .entropy = type.infer.entropy
                }
            Next
        Next
    End Function

    <Extension>
    Public Iterator Function GetUniques(result As IEnumerable(Of MetaDNAResult), typeOrders As Index(Of String)) As IEnumerable(Of MetaDNAResult)
        For Each kegg_id In result.FeatureUniques(typeOrders).GroupBy(Function(c) c.KEGGId)
            Dim data As MetaDNAResult() = kegg_id.ToArray
            Dim pvalue As Vector = -data.Select(Function(c) c.pvalue).AsVector.Log(base:=10)
            Dim intensity As Vector = data.Select(Function(c) c.intensity).AsVector.Log(base:=10)
            Dim orders As Vector = data.Select(Function(c) typeOrders.Count - typeOrders.IndexOf(c.precursorType)).AsVector
            Dim level As Vector = data.Select(Function(c) If(c.inferLevel = "Ms1", 0.5, 1.0)).AsVector
            Dim parent As Vector = data.Select(Function(c) c.parentTrace).AsVector
            Dim scores As Double() = pvalue * intensity * (orders + 1) * parent * level
            Dim i As Integer = which.Max(scores)
            Dim max As MetaDNAResult = data(i)

            max.score2 = scores(i)

            Yield max
        Next
    End Function

    <Extension>
    Private Iterator Function FeatureUniques(result As IEnumerable(Of MetaDNAResult), typeOrders As Index(Of String)) As IEnumerable(Of MetaDNAResult)
        For Each feature In result.GroupBy(Function(c) c.ROI_id)
            Dim data As MetaDNAResult() = feature.ToArray
            Dim pvalue As Vector = -data.Select(Function(c) c.pvalue).AsVector.Log(base:=10)
            Dim orders As Vector = data.Select(Function(c) typeOrders.Count - typeOrders.IndexOf(c.precursorType)).AsVector
            Dim level As Vector = data.Select(Function(c) If(c.inferLevel = "Ms1", 0.5, 1.0)).AsVector
            Dim parent As Vector = data.Select(Function(c) c.parentTrace).AsVector
            Dim scores As Double() = pvalue * (orders + 1) * parent * level
            Dim i As Integer = which.Max(scores)
            Dim max As MetaDNAResult = data(i)

            max.score1 = scores(i)

            Yield max
        Next
    End Function
End Module
