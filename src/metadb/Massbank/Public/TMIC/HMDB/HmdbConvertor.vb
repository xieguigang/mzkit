Imports System.Runtime.CompilerServices
Imports BiodeepDB.IntegrativeRepository
Imports BiodeepDB.IntegrativeRepository.Components
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports BioNovoGene.BioDeep.Chemistry.TMIC
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace TMIC.HMDB

    Module HmdbConvertor

        <Extension>
        Public Sub FillHMDBInformation(metabolites As IEnumerable(Of metabolite),
                                   metadb As MetaIndex,
                                   ByRef missingKEGGId As List(Of String),
                                   sidMap As Dictionary(Of String, SIDMap))

            missingKEGGId = New List(Of String)
            builderPipeline.BioDeepIDBase = 2000000

            For Each metabolite As HMDB.MetaDb In metabolites
                Dim queryXref As New xref(metabolite)

                If sidMap.ContainsKey(queryXref.HMDB) Then
                    queryXref.pubchem = sidMap(queryXref.HMDB).CID
                Else
                    queryXref.pubchem = Nothing
                End If

                Dim topBestHit As MetaAnnotation = metadb.GetTopHit(queryXref)

                If topBestHit Is Nothing Then
                    ' add new
                    Call metadb.Add(metabolite.createNewAnnotation(False, sidMap))
                Else
                    If Not topBestHit.joinInformation(metabolite, sidMap) Then
                        ' add new
                        Call metadb.Add(metabolite.createNewAnnotation(False, sidMap))
                    End If
                End If
            Next
        End Sub

        <Extension>
        Private Function createNewAnnotation(metabolite As HMDB.MetaDb, kegg As Boolean, sidMap As Dictionary(Of String, SIDMap)) As MetaAnnotation
            Dim id$

            If kegg Then
                id = metabolite.kegg_id _
                .Match("\d+") _
                .ParseInteger _
                .DoCall(Function(i) builderPipeline.formatBioDeepID(i, True))
            Else
                id = metabolite.accession _
                .Match("\d+") _
                .ParseInteger _
                .DoCall(AddressOf builderPipeline.formatBioDeepID)
            End If

            Dim xref As New xref With {
            .CAS = {metabolite.CAS},
            .chebi = metabolite.GetFormattedChEBIId,
            .HMDB = metabolite.accession,
            .InChI = metabolite.inchi,
            .InChIkey = metabolite.inchikey,
            .kegg = Strings.Trim(metabolite.kegg_id).Trim(ASCII.TAB),
            .SMILES = metabolite.smiles,
            .Wikipedia = metabolite.wikipedia_id,
            .pubchem = metabolite.GetCID(sidMap)
        }
            Dim names As New CommonName With {
            .synonyms = metabolite _
                .getNames _
                .ToArray
        }

            Return New MetaAnnotation With {
            .BioDeepID = id,
            .[class] = metabolite.class,
            .exact_mass = metabolite.exact_mass,
            .formula = metabolite.chemical_formula,
            .kingdom = metabolite.kingdom,
            .molecular_framework = metabolite.molecular_framework,
            .Source = XrefDbs.HMDB,
            .sub_class = metabolite.sub_class,
            .super_class = metabolite.super_class,
            .note = metabolite.description,
            .mass = metabolite.exact_mass,
            .xref = xref,
            .Name = names,
            .origins = {New origins With {.id = "NCBI:9606", .speciesName = "Homo sapiens"}},
            .logs = {New TraceAction With {.Action = "create_by", .db = XrefDbs.HMDB, .recordId = metabolite.accession}}
        }
        End Function

        <Extension>
        Private Iterator Function getNames(metabolite As HMDB.MetaDb) As IEnumerable(Of Name)
            Yield New Name With {.isCommonName = True, .synonym = metabolite.name, .xrefDb = XrefDbs.HMDB}
            Yield New Name With {.isCommonName = False, .synonym = metabolite.iupac_name, .xrefDb = XrefDbs.HMDB}
            Yield New Name With {.isCommonName = False, .synonym = metabolite.traditional_iupac, .xrefDb = XrefDbs.HMDB}

            For Each name As String In metabolite.synonyms.SafeQuery
                Yield New Name With {
                .isCommonName = False,
                .synonym = name,
                .xrefDb = XrefDbs.HMDB
            }
            Next
        End Function

        <Extension>
        Private Function GetCID(metabolite As HMDB.MetaDb, sidMap As Dictionary(Of String, SIDMap))
            If sidMap.ContainsKey(metabolite.accession) Then
                Return sidMap(metabolite.accession).CID
            End If

            For Each sid As String In metabolite.secondary_accessions.SafeQuery
                If sidMap.ContainsKey(sid) Then
                    Return sidMap(sid).CID
                End If
            Next

            Return Nothing
        End Function

        <Extension>
        Private Function joinInformation(anno As MetaAnnotation, metabolite As HMDB.MetaDb, sidMap As Dictionary(Of String, SIDMap)) As Boolean
            If Not anno.xref.HMDB.StringEmpty AndAlso (anno.xref.HMDB <> metabolite.accession) Then
                Call $"{anno.xref.HMDB}: {anno.name.CommonName} <> {metabolite.accession}: {metabolite.name}".Warning
                Return False
            End If

            If anno.kingdom.StringEmpty Then
                anno.kingdom = metabolite.kingdom
                anno.class = metabolite.class
                anno.sub_class = metabolite.sub_class
                anno.super_class = metabolite.super_class
                anno.molecular_framework = metabolite.molecular_framework
            End If

            If anno.formula.StringEmpty Then
                anno.formula = metabolite.chemical_formula
            End If
            If anno.exact_mass <= 0 Then
                anno.exact_mass = metabolite.exact_mass
                anno.mass = anno.exact_mass
            End If
            If anno.note.StringEmpty Then
                anno.note = metabolite.description
            End If

            anno.origins = anno.origins.JoinIterates(New origins With {.id = "NCBI:9606", .speciesName = "Homo sapiens"}).ToArray
            anno.logs.Add(New TraceAction With {.Action = "modify_by", .recordId = metabolite.accession, .db = XrefDbs.HMDB})

            Dim xref As xref = anno.xref

            If Not metabolite.CAS.StringEmpty Then
                xref.CAS = xref.CAS.JoinIterates(metabolite.CAS).Distinct.ToArray
            End If

            If xref.InChI.StringEmpty Then
                xref.InChI = metabolite.inchi
                xref.InChIkey = metabolite.inchikey
            End If
            If xref.SMILES.StringEmpty Then
                xref.SMILES = metabolite.smiles
            End If
            If xref.Wikipedia.StringEmpty Then
                xref.Wikipedia = metabolite.wikipedia_id
            End If
            If xref.pubchem.ParseInteger <= 0 Then
                xref.pubchem = metabolite.GetCID(sidMap)
            End If
            If xref.HMDB.StringEmpty Then
                xref.HMDB = metabolite.accession
            End If

            If metabolite.chebi_id > 0 Then
                xref.chebi = xref.chebi.StringSplit("\s+/\s+").JoinIterates("CHEBI:" & metabolite.chebi_id).Distinct.JoinBy("/")
            End If

            Return True
        End Function
    End Module
End Namespace