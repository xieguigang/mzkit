Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.TMIC
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite.CrossReference
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML

Namespace MetaLib

    Public Module Constructor

        ''' <summary>
        ''' extract the cross reference id set from a chebi metabolite data
        ''' </summary>
        ''' <param name="chebi"></param>
        <Extension>
        Public Function ChEBICrossReference(chebi As ChEBIEntity) As xref
            Dim xref As New xref

            xref.chebi = chebi.chebiId
            xref.KEGG = chebi.FindDatabaseLinkValue(AccessionTypes.KEGG_Compound)
            xref.Wikipedia = chebi.FindDatabaseLinkValue(AccessionTypes.Wikipedia)
            xref.SMILES = chebi.smiles
            xref.InChI = chebi.inchi
            xref.InChIkey = chebi.inchiKey
            xref.CAS = chebi.RegistryNumbers _
                .SafeQuery _
                .Where(Function(r) r.type = "CAS Registry Number") _
                .Select(Function(r) r.data) _
                .ToArray

            Return xref
        End Function

        ''' <summary>
        ''' extract the cross reference id set from a hmdb metabolite data
        ''' </summary>
        ''' <param name="meta"></param>
        <Extension>
        Public Function HMDBCrossReference(meta As HMDB.MetaDb) As xref
            Dim xref As New xref

            xref.chebi = "CHEBI:" & meta.chebi_id
            xref.KEGG = Strings.Trim(meta.kegg_id).Trim(ASCII.TAB)
            xref.Wikipedia = meta.wikipedia_id
            xref.SMILES = meta.smiles
            xref.InChI = meta.inchi
            xref.InChIkey = meta.inchikey
            xref.CAS = {meta.CAS}
            xref.HMDB = meta.accession

            Return xref
        End Function

    End Module
End Namespace