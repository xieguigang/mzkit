Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports metadata = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaInfo

Public Module ChEBIObo

    Public Iterator Function ImportsMetabolites(chebi As OBOFile) As IEnumerable(Of metadata)
        For Each term As RawTerm In chebi.GetRawTerms
            Yield term.ExtractTerm
        Next
    End Function

    <Extension>
    Public Function ExtractTerm(term As RawTerm) As metadata
        Dim obo_data = term.GetValueSet
        Dim id As String = obo_data(RawTerm.Key_id).First
        Dim name As String = obo_data(RawTerm.Key_name).First
        Dim def As String = obo_data(RawTerm.Key_def).JoinBy("; ")
        Dim synonym As String() = obo_data(RawTerm.Key_synonym) _
            .SafeQuery _
            .Select(Function(si) DelimiterParser.GetTokens(si).First) _
            .ToArray
        Dim properties = obo_data(RawTerm.Key_property_value).ParsePropertyValues
        Dim xref = obo_data(RawTerm.Key_xref).ParseXref

        Return New metadata With {
            .description = def,
            .ID = id,
            .name = name,
            .synonym = synonym,
            .IUPACName = name,
            .formula = properties("http://purl.obolibrary.org/obo/chebi/formula").FirstOrDefault?.text,
            .exact_mass = FormulaScanner.EvaluateExactMass(.formula),
            .xref = New xref With {
                .CAS = xref("CAS"),
                .chebi = id,
                .KEGG = xref("KEGG").JoinBy(", "),
                .InChI = properties("http://purl.obolibrary.org/obo/chebi/inchi").FirstOrDefault?.text,
                .InChIkey = properties("http://purl.obolibrary.org/obo/chebi/inchikey").FirstOrDefault?.text,
                .SMILES = properties("http://purl.obolibrary.org/obo/chebi/smiles").FirstOrDefault?.text,
                .MetaCyc = xref("MetaCyc").JoinBy(", "),
                .DrugBank = xref("DrugBank").JoinBy(", "),
                .Wikipedia = xref("Wikipedia").JoinBy(", "),
                .HMDB = xref("HMDB").JoinBy(", "),
                .KNApSAcK = xref("KNApSAcK").JoinBy(", "),
                .lipidmaps = xref("LIPID_MAPS_instance").JoinBy(", ")
            }
        }
    End Function
End Module
