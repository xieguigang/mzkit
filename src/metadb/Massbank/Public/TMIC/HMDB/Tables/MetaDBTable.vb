#Region "Microsoft.VisualBasic::15b84934f149ae970ee4af18284be046, G:/mzkit/src/metadb/Massbank//Public/TMIC/HMDB/Tables/MetaDBTable.vb"

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

    '   Total Lines: 279
    '    Code Lines: 244
    ' Comment Lines: 3
    '   Blank Lines: 32
    '     File Size: 12.71 KB


    '     Class MetaInfo
    ' 
    '         Properties: CAS, chebi, HMDB, KEGG
    ' 
    '     Class BriefTable
    ' 
    '         Properties: AdultConcentrationAbnormal, AdultConcentrationNormal, ChildrenConcentrationAbnormal, ChildrenConcentrationNormal, disease
    '                     NewbornConcentrationAbnormal, NewbornConcentrationNormal, Sample, water_solubility
    ' 
    '         Function: Clone
    ' 
    '     Class MetaDb
    ' 
    '         Properties: [class], accession, biocyc_id, Biomarker, CAS
    '                     cellular_locations, chebi_id, chemical_formula, chemspider_id, contents
    '                     description, direct_parent, disease, Disposition, drugbank_id
    '                     exact_mass, foodb_id, inchi, inchikey, iupac_name
    '                     kegg_id, kingdom, metlin_id, molecular_framework, name
    '                     pathways, Physiological_effects, Process, proteins, pubchem_cid
    '                     Role, secondary_accessions, smiles, state, sub_class
    '                     super_class, synonyms, tissue, traditional_iupac, wikipedia_id
    ' 
    '         Function: FromMetabolite, getBioMarkers, getOntologyIndex, GetSynonym, OntologyTreeLines
    '                   PopulateTable, populateTree
    ' 
    '         Sub: WriteTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq

Namespace TMIC.HMDB

    Public Class MetaInfo : Inherits MetaLib.Models.MetaInfo

        Public Property HMDB As String
        Public Property KEGG As String
        Public Property chebi As String
        Public Property CAS As String

    End Class

    Public Class BriefTable : Inherits MetaInfo
        Implements ICloneable

        Public Property Sample As String
        Public Property disease As String()
        Public Property water_solubility As String

        <Column("concentration (children-normal)")>
        Public Property ChildrenConcentrationNormal As String()
        <Column("concentration (children-abnormal)")>
        Public Property ChildrenConcentrationAbnormal As String()
        <Column("concentration (adult-normal)")>
        Public Property AdultConcentrationNormal As String()
        <Column("concentration (adult-abnormal)")>
        Public Property AdultConcentrationAbnormal As String()
        <Column("concentration (newborn-normal)")>
        Public Property NewbornConcentrationNormal As String()
        <Column("concentration (newborn-abnormal)")>
        Public Property NewbornConcentrationAbnormal As String()

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function
    End Class

    ''' <summary>
    ''' store the hmdb metabolite information in table format
    ''' </summary>
    Public Class MetaDb : Implements ICompoundClass, ICompoundNames

        <Column("HMDB ID")> Public Property accession As String
        Public Property secondary_accessions As String()
        <Column("Common Name")> Public Property name As String
        Public Property chemical_formula As String
        Public Property exact_mass As Double
        Public Property iupac_name As String
        Public Property traditional_iupac As String
        Public Property synonyms As String()
        <Column("CAS Registry Number")> Public Property CAS As String
        <Column("SMILES")> Public Property smiles As String
        Public Property inchi As String
        Public Property inchikey As String
        Public Property kingdom As String Implements ICompoundClass.kingdom
        Public Property super_class As String Implements ICompoundClass.super_class
        Public Property [class] As String Implements ICompoundClass.class
        Public Property sub_class As String Implements ICompoundClass.sub_class
        Public Property molecular_framework As String Implements ICompoundClass.molecular_framework
        Public Property direct_parent As String
        Public Property state As String
        Public Property cellular_locations As String()
        Public Property contents As Dictionary(Of String, String)
        Public Property tissue As String()
        Public Property chebi_id As Long
        Public Property pubchem_cid As Long
        Public Property kegg_id As String
        Public Property chemspider_id As String
        Public Property drugbank_id As String
        Public Property foodb_id As String
        Public Property biocyc_id As String
        Public Property metlin_id As String
        Public Property wikipedia_id As String
        Public Property description As String
        Public Property pathways As String()
        Public Property proteins As String()
        Public Property disease As String()
        Public Property Physiological_effects As String()
        Public Property Disposition As String()
        Public Property Process As String()
        Public Property Role As String()
        Public Property Biomarker As String()

        Public Iterator Function GetSynonym() As IEnumerable(Of String) Implements ICompoundNames.GetSynonym
            Yield name
            Yield iupac_name
            Yield traditional_iupac

            For Each name As String In synonyms.SafeQuery
                Yield name
            Next
        End Function

        Private Shared Function getOntologyIndex(metabolite As metabolite) As Dictionary(Of String, ontology_term)
            If metabolite.ontology Is Nothing OrElse metabolite.ontology.root.IsNullOrEmpty Then
                Return New Dictionary(Of String, ontology_term)
            Else
                Return metabolite.ontology.root.ToDictionary(Function(a) a.term)
            End If
        End Function

        Public Shared Function FromMetabolite(metabolite As metabolite) As MetaDb
            Dim metabolite_taxonomy = metabolite.taxonomy
            Dim biosample = metabolite.biological_properties
            Dim pathways As String() = Nothing
            Dim proteins As String() = Nothing
            Dim disease As String() = Nothing
            Dim ontology = getOntologyIndex(metabolite)
            Dim contents As New Dictionary(Of String, List(Of String))
            Dim key As String

            For Each norm In metabolite.normal_concentrations.SafeQuery
                key = "normal: " & norm.biospecimen

                If Not contents.ContainsKey(key) Then
                    contents.Add(key, New List(Of String))
                End If

                If Not norm.concentration_value.StringEmpty Then
                    Call contents(key).Add(norm.ToString & $" {norm.AgeType}, {norm.subject_sex}")
                End If
            Next

            For Each abnorm In metabolite.abnormal_concentrations.SafeQuery
                key = "abnormal: " & abnorm.biospecimen

                If Not contents.ContainsKey(key) Then
                    contents.Add(key, New List(Of String))
                End If

                If Not abnorm.concentration_value.StringEmpty Then
                    Call contents(key).Add(abnorm.ToString & $" {abnorm.AgeType}, {abnorm.subject_sex}")
                End If
            Next

            If Not metabolite.protein_associations.IsNullOrEmpty Then
                proteins = metabolite.protein_associations _
                    .Select(Function(p)
                                Return $"{p.protein_accession}|{p.uniprot_id}|{p.gene_name} {p.name}"
                            End Function) _
                    .ToArray
            End If
            If Not biosample Is Nothing Then
                If Not biosample.pathways.IsNullOrEmpty Then
                    pathways = biosample.pathways _
                        .Select(Function(p) $"{p.smpdb_id}:{p.name}") _
                        .ToArray
                End If
            End If
            If Not metabolite.diseases.IsNullOrEmpty Then
                disease = metabolite.diseases _
                    .Select(Function(d)
                                Return $"{d.name}: {d.references.Select(Function(r) r.pubmed_id).JoinBy(", ")}"
                            End Function) _
                    .ToArray
            End If

            Return New MetaDb With {
                .accession = metabolite.accession,
                .secondary_accessions = metabolite.secondary_accessions.accession,
                .chebi_id = Strings.Trim(metabolite.chebi_id).Split(":"c).Last.ParseInteger,
                .pubchem_cid = Strings.Trim(metabolite.pubchem_compound_id).ParseInteger,
                .chemical_formula = metabolite.chemical_formula,
                .kegg_id = metabolite.kegg_id,
                .wikipedia_id = metabolite.wikipedia_id,
                .inchi = metabolite.inchi,
                .inchikey = metabolite.inchikey,
                .name = metabolite.name,
                .state = metabolite.state,
                .traditional_iupac = metabolite.traditional_iupac,
                .smiles = metabolite.smiles,
                .iupac_name = metabolite.iupac_name,
                .CAS = metabolite.cas_registry_number,
                .exact_mass = Val(metabolite.monisotopic_molecular_weight),
                .direct_parent = metabolite_taxonomy?.direct_parent,
                .kingdom = metabolite_taxonomy?.kingdom,
                .super_class = metabolite_taxonomy?.super_class,
                .sub_class = metabolite_taxonomy?.sub_class,
                .molecular_framework = metabolite_taxonomy?.molecular_framework,
                .[class] = metabolite_taxonomy?.class,
                .contents = contents.ToDictionary(Function(a) a.Key, Function(a) a.Value.JoinBy("; ")),
                .cellular_locations = biosample?.cellular_locations.cellular,
                .tissue = biosample?.tissue_locations.tissue,
                .synonyms = metabolite.synonyms.synonym,
                .description = metabolite.description,
                .pathways = pathways,
                .proteins = proteins,
                .biocyc_id = metabolite.biocyc_id,
                .chemspider_id = metabolite.chemspider_id,
                .drugbank_id = metabolite.drugbank_id,
                .foodb_id = metabolite.foodb_id,
                .metlin_id = metabolite.metlin_id,
                .disease = disease,
                .Physiological_effects = OntologyTreeLines(ontology.TryGetValue("Physiological effect")).ToArray,
                .Disposition = OntologyTreeLines(ontology.TryGetValue("Disposition")).ToArray,
                .Process = OntologyTreeLines(ontology.TryGetValue("Process")).ToArray,
                .Role = OntologyTreeLines(ontology.TryGetValue("Role")).ToArray,
                .Biomarker = getBioMarkers(ontology.TryGetValue("Role"))
            }
        End Function

        Private Shared Function getBioMarkers(role As ontology_term) As String()
            If role Is Nothing Then
                Return {}
            Else
                If role.term = "Biomarker" Then
                    If role.descendants.descendant.IsNullOrEmpty Then
                        Return {}
                    Else
                        Return role.descendants.descendant.Select(Function(d) d.term).ToArray
                    End If
                Else
                    If role.descendants.descendant.IsNullOrEmpty Then
                        Return {}
                    Else
                        For Each child In role.descendants.descendant
                            Dim list = getBioMarkers(child)

                            If list.Length > 0 Then
                                Return list
                            End If
                        Next
                    End If
                End If
            End If

            Return {}
        End Function

        Private Shared Iterator Function OntologyTreeLines(root As ontology_term) As IEnumerable(Of String)
            If root Is Nothing OrElse root.descendants.descendant Is Nothing Then
                Return
            Else
                For Each term In root.descendants.descendant
                    For Each line As String In populateTree(term, "")
                        Yield line
                    Next
                Next
            End If
        End Function

        Private Shared Function populateTree(term As ontology_term, parent As String) As String()
            Dim term_string As String = term.term

            If term_string = "Biomarker" Then
                Return {}
            ElseIf term.descendants.descendant.IsNullOrEmpty Then
                Return {$"{parent}|{term_string}"}
            Else
                Dim childs As New List(Of String)

                For Each child In term.descendants.descendant
                    childs.AddRange(populateTree(child, $"{parent}|{term_string}"))
                Next

                Return childs.ToArray
            End If
        End Function

        Public Shared Iterator Function PopulateTable(metabolites As IEnumerable(Of metabolite)) As IEnumerable(Of MetaDb)
            For Each metabolite As metabolite In metabolites
                Yield FromMetabolite(metabolite)
            Next
        End Function

        Public Shared Sub WriteTable(metabolites As IEnumerable(Of metabolite), out As Stream)
            Using table As New WriteStream(Of MetaDb)(New StreamWriter(out))
                For Each metabolite As metabolite In metabolites
                    Call table.Flush(FromMetabolite(metabolite))
                Next
            End Using
        End Sub
    End Class
End Namespace
