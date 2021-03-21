#Region "Microsoft.VisualBasic::aefeedeaa033476ef98b9b8986c7ecb3, src\metadna\metaDNA\Result\ResultHandler.vb"

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

    ' Module ResultHandler
    ' 
    '     Function: ExportTable, FeatureUniques, GetUniques
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Module ResultHandler

    <Extension>
    Public Iterator Function ExportTable(candidates As IEnumerable(Of CandidateInfer),
                                         kegg As KEGGHandler,
                                         keggNetwork As KEGGNetwork) As IEnumerable(Of MetaDNAResult)

        Dim precursorTypes As Dictionary(Of String, MzCalculator) = kegg.Calculators

        For Each infer As CandidateInfer In candidates
            Dim compound As Compound = kegg.GetCompound(infer.kegg_id)

            For Each type As Candidate In infer.infers
                Dim partner As String = type.infer.reference.id.Split(":"c).Last.Trim
                Dim links As NamedValue(Of String)() = keggNetwork.FindReactions(partner, compound.entry)

                Yield New MetaDNAResult With {
                    .exactMass = compound.exactMass,
                    .formula = compound.formula,
                    .query_id = type.infer.query.id,
                    .forward = type.infer.forward,
                    .reverse = type.infer.reverse,
                    .jaccard = type.infer.jaccard,
                    .inferLevel = type.infer.level.Description,
                    .KEGGId = infer.kegg_id,
                    .name = If(compound.commonNames.FirstOrDefault, compound.formula),
                    .ppm = CInt(type.ppm),
                    .precursorType = type.precursorType,
                    .pvalue = type.pvalue,
                    .partnerKEGGId = partner,
                    .seed = type.infer.reference.id,
                    .mz = type.infer.query.mz,
                    .rt = type.infer.query.scan_time,
                    .intensity = type.infer.query.intensity,
                    .KEGG_reaction = links(Scan0).Name,
                    .reaction = links(Scan0).Value,
                    .parentTrace = type.infer.parentTrace / 100,
                    .inferSize = type.infer.inferSize,
                    .fileName = type.infer.rawFile,
                    .mzCalc = precursorTypes(.precursorType).CalcMZ(.exactMass),
                    .ROI_id = type.ROI
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

