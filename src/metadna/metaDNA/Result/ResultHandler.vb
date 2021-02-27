Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Module ResultHandler

    <Extension>
    Public Iterator Function ExportTable(candidates As IEnumerable(Of CandidateInfer),
                                         kegg As KEGGHandler,
                                         keggNetwork As KEGGNetwork) As IEnumerable(Of MetaDNAResult)

        For Each infer As CandidateInfer In candidates
            Dim compound As Compound = kegg.GetCompound(infer.kegg_id)

            For Each type As Candidate In infer.infers
                Dim partner As String = type.infer.reference.id.Split(":"c).Last.Trim

                Yield New MetaDNAResult With {
                    .exactMass = compound.exactMass,
                    .formula = compound.formula,
                    .id = type.infer.query.id,
                    .forward = type.infer.forward,
                    .reverse = type.infer.reverse,
                    .inferLevel = type.infer.level,
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
                    .KEGG_reaction =
                }
            Next
        Next
    End Function

    <Extension>
    Public Iterator Function GetUniques(result As IEnumerable(Of MetaDNAResult)) As IEnumerable(Of MetaDNAResult)
        For Each kegg_id In result.GroupBy(Function(c) c.KEGGId)
            Dim data As MetaDNAResult() = kegg_id.ToArray
            Dim pvalue As Vector = -data.Select(Function(c) c.pvalue).AsVector.Log(base:=10)
            Dim intensity As Vector = data.Select(Function(c) c.intensity).AsVector.Log(base:=10)
            Dim scores As Vector = pvalue * intensity
            Dim max As MetaDNAResult = data(Which.Max(scores))

            Yield max
        Next
    End Function
End Module
