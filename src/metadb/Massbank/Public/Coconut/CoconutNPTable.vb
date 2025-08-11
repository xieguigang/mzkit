Imports System.IO
Imports Microsoft.VisualBasic.Data.Framework.IO

Namespace Coconut

    Public Class CoconutNPTable

        Public Property identifier As String
        Public Property canonical_smiles As String
        Public Property standard_inchi As String
        Public Property standard_inchi_key As String
        Public Property name As String
        Public Property iupac_name As String
        Public Property annotation_level As String
        Public Property molecular_formula As String
        Public Property murcko_framework As String
        Public Property np_likeness As Double
        Public Property chemical_class As String
        Public Property chemical_sub_class As String
        Public Property chemical_super_class As String
        Public Property direct_parent_classification As String
        Public Property np_classifier_pathway As String
        Public Property np_classifier_superclass As String
        Public Property np_classifier_class As String
        Public Property np_classifier_is_glycoside As String
        Public Property organisms As String()
        Public Property synonyms As String()
        Public Property cas As String()

        Public Shared Iterator Function ParseTable(s As Stream) As IEnumerable(Of CoconutNPTable)
            Dim df As DataFrameResolver = DataFrameResolver.Load(s)
            Dim identifier As Integer = df.GetOrdinal(NameOf(CoconutNPTable.identifier))
            Dim canonical_smiles As Integer = df.GetOrdinal(NameOf(CoconutNPTable.canonical_smiles))
            Dim standard_inchi As Integer = df.GetOrdinal(NameOf(CoconutNPTable.standard_inchi))
            Dim standard_inchi_key As Integer = df.GetOrdinal(NameOf(CoconutNPTable.standard_inchi_key))
            Dim name As Integer = df.GetOrdinal(NameOf(CoconutNPTable.name))
            Dim iupac_name As Integer = df.GetOrdinal(NameOf(CoconutNPTable.iupac_name))
            Dim annotation_level As Integer = df.GetOrdinal(NameOf(CoconutNPTable.annotation_level))
            Dim molecular_formula As Integer = df.GetOrdinal(NameOf(CoconutNPTable.molecular_formula))
            Dim murcko_framework As Integer = df.GetOrdinal(NameOf(CoconutNPTable.murcko_framework))
            Dim np_likeness As Integer = df.GetOrdinal(NameOf(CoconutNPTable.np_likeness))
            Dim chemical_class As Integer = df.GetOrdinal(NameOf(CoconutNPTable.chemical_class))
            Dim chemical_sub_class As Integer = df.GetOrdinal(NameOf(CoconutNPTable.chemical_sub_class))
            Dim chemical_super_class As Integer = df.GetOrdinal(NameOf(CoconutNPTable.chemical_super_class))
            Dim direct_parent_classification As Integer = df.GetOrdinal(NameOf(CoconutNPTable.direct_parent_classification))
            Dim np_classifier_pathway As Integer = df.GetOrdinal(NameOf(CoconutNPTable.np_classifier_pathway))
            Dim np_classifier_superclass As Integer = df.GetOrdinal(NameOf(CoconutNPTable.np_classifier_superclass))
            Dim np_classifier_class As Integer = df.GetOrdinal(NameOf(CoconutNPTable.np_classifier_class))
            Dim np_classifier_is_glycoside As Integer = df.GetOrdinal(NameOf(CoconutNPTable.np_classifier_is_glycoside))
            Dim organisms As Integer = df.GetOrdinal(NameOf(CoconutNPTable.organisms))
            Dim synonyms As Integer = df.GetOrdinal(NameOf(CoconutNPTable.synonyms))
            Dim cas As Integer = df.GetOrdinal(NameOf(CoconutNPTable.cas))

            Do While df.Read
                Yield New CoconutNPTable With {
                    .annotation_level = df.GetString(annotation_level),
                    .name = df.GetString(name),
                    .canonical_smiles = df.GetString(canonical_smiles),
                    .cas = df.GetString(cas).StringSplit("[|]"),
                    .chemical_class = df.GetString(chemical_class),
                    .synonyms = df.GetString(synonyms).StringSplit("[|]"),
                    .organisms = df.GetString(organisms).StringSplit("[|]"),
                    .chemical_sub_class = df.GetString(chemical_sub_class),
                    .chemical_super_class = df.GetString(chemical_super_class),
                    .direct_parent_classification = df.GetString(direct_parent_classification),
                    .identifier = df.GetString(identifier),
                    .iupac_name = df.GetString(iupac_name),
                    .molecular_formula = df.GetString(molecular_formula),
                    .murcko_framework = df.GetString(murcko_framework),
                    .np_classifier_class = df.GetString(np_classifier_class),
                    .np_classifier_is_glycoside = df.GetString(np_classifier_is_glycoside),
                    .np_classifier_pathway = df.GetString(np_classifier_pathway),
                    .np_classifier_superclass = df.GetString(np_classifier_superclass),
                    .np_likeness = df.GetDouble(np_likeness),
                    .standard_inchi = df.GetString(standard_inchi),
                    .standard_inchi_key = df.GetString(standard_inchi_key)
                }
            Loop
        End Function
    End Class
End Namespace