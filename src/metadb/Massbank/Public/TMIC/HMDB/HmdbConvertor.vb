Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Text
Imports Metadata2 = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

Namespace TMIC.HMDB

    Module HmdbConvertor

        <Extension>
        Public Iterator Function ConvertInternal(metabolites As IEnumerable(Of metabolite)) As IEnumerable(Of Metadata2)
            For Each metabolite As metabolite In metabolites
                Yield metabolite.CreateReferenceData
            Next
        End Function

        <Extension>
        Private Function CreateReferenceData(metabolite As metabolite) As Metadata2
            Dim id = metabolite.accession
            Dim xref As New xref With {
            .CAS = {metabolite.cas_registry_number},
            .chebi = metabolite.chebi_id,
            .HMDB = metabolite.accession,
            .InChI = metabolite.inchi,
            .InChIkey = metabolite.inchikey,
            .KEGG = Strings.Trim(metabolite.kegg_id).Trim(ASCII.TAB),
            .SMILES = metabolite.smiles,
            .Wikipedia = metabolite.wikipedia_id,
            .pubchem = metabolite.pubchem_compound_id,
            .chemspider = metabolite.chemspider_id,
            .DrugBank = metabolite.drugbank_id,
            .foodb = metabolite.foodb_id,
            .MetaCyc = metabolite.meta_cyc_id
        }
            Dim metabo_tax = metabolite.taxonomy

            Return New Metadata2 With {
            .ID = id,
            .[class] = metabo_tax.class,
            .exact_mass = FormulaScanner.EvaluateExactMass(metabolite.chemical_formula),
            .formula = metabolite.chemical_formula,
            .kingdom = metabo_tax.kingdom,
            .molecular_framework = metabo_tax.molecular_framework,
            .sub_class = metabo_tax.sub_class,
            .super_class = metabo_tax.super_class,
            .description = metabolite.description,
            .xref = xref,
            .name = metabolite.name,
            .organism = {"Homo sapiens"}
        }
        End Function
    End Module
End Namespace