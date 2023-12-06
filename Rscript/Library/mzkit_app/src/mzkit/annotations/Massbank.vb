#Region "Microsoft.VisualBasic::4e8e3aed3e1241d8361086ce5b2ba8c5, mzkit\Rscript\Library\mzkit\annotations\Massbank.vb"

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

'   Total Lines: 484
'    Code Lines: 353
' Comment Lines: 65
'   Blank Lines: 66
'     File Size: 19.98 KB


' Module Massbank
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: castToClassProfiles, chebiSecondary2Main, createIdMapping, createLipidMapTable, GlycosylNameSolver
'               GlycosylTokens, hmdbSecondary2Main, KEGGPathwayCoverages, lipidnameMapping, lipidProfiles
'               name2, ParseChebiEntity, readLipidMapsRepo, readMoNA, readSDF
'               saveIDMapping, toLipidMaps, writeLipidMapsRepo
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.LipidMaps
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemistry.TMIC
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.IUPAC.InChILayers
Imports BioNovoGene.BioDeep.Chemoinformatics.NaturalProduct
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.WebServices
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports ChEBIRepo = SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.DATA
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.Invokes.base

''' <summary>
''' Metabolite annotation database toolkit
''' </summary>
<Package("massbank")>
Module Massbank

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(LipidMaps.MetaData()), AddressOf createLipidMapTable)
    End Sub

    Public Function createLipidMapTable(lipidmap As LipidMaps.MetaData(), args As list, env As Environment) As Rdataframe
        Dim table As New Rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = lipidmap.Select(Function(m) m.LM_ID).ToArray
        }

        For Each col As NamedValue(Of Array) In lipidmap.ProjectVectors
            If col.Name = "SYNONYMS" Then
                Call table.add(col.Name, From str As String() In DirectCast(col.Value, String()()) Select str.JoinBy("; "))
            Else
                Call table.columns.Add(col.Name, col.Value)
            End If
        Next

        Return table
    End Function

    ''' <summary>
    ''' Extract the annotation metadata from the MONA comment data
    ''' </summary>
    ''' <param name="msp">A metabolite data which is parse from the MONA msp dataset</param>
    ''' <returns></returns>
    <ExportAPI("mona.msp_metadata")>
    Public Function monaMSP(msp As MspData) As Object
        Return msp.GetMetadata
    End Function

    ''' <summary>
    ''' read MoNA database file.
    ''' </summary>
    ''' <param name="rawfile">
    ''' the database reader is switched automatically 
    ''' based on this file path its extension name.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a linq pipeline for populate the spectrum data 
    ''' from the MoNA database.
    ''' </returns>
    <ExportAPI("read.MoNA")>
    <RApiReturn(GetType(SpectraSection))>
    Public Function readMoNA(rawfile As String,
                             Optional skipSpectraInfo As Boolean = False,
                             Optional is_gcms As Boolean = False,
                             Optional env As Environment = Nothing) As pipeline

        Select Case rawfile.ExtensionSuffix.ToLower
            Case "sdf"
                Return SDFReader _
                    .ParseFile(
                        path:=rawfile,
                        skipSpectraInfo:=skipSpectraInfo,
                        isGcms:=is_gcms
                    ) _
                    .DoCall(AddressOf pipeline.CreateFromPopulator)
            Case "msp"
                Return MspReader _
                    .ParseFile(rawfile, parseMs2:=Not skipSpectraInfo) _
                    .DoCall(AddressOf pipeline.CreateFromPopulator)
            Case Else
                Return Internal.debug.stop(New NotSupportedException(rawfile.ExtensionSuffix), env)
        End Select
    End Function

    ''' <summary>
    ''' read metabolite data in a given sdf data file.
    ''' </summary>
    ''' <param name="file">the file path of the target sdf file</param>
    ''' <param name="parseStruct">
    ''' Andalso parse the molecular structure data inside the metabolite annotation data?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' let dataset = read.SDF(file = "./example.sdf", lazy = FALSE);
    ''' </example>
    <ExportAPI("read.SDF")>
    <RApiReturn(GetType(SDF))>
    Public Function readSDF(file As String,
                            Optional parseStruct As Boolean = True,
                            Optional lazy As Boolean = True,
                            Optional env As Environment = Nothing) As Object

        If Not file.FileExists Then
            Return Internal.debug.stop({$"the required file is not exists on your file system!", $"file: {file}"}, env)
        Else
            Dim readStream = SDF.IterateParser(file, parseStruct)

            If lazy Then
                Return readStream.DoCall(AddressOf pipeline.CreateFromPopulator)
            Else
                Return readStream.ToArray
            End If
        End If
    End Function

    ''' <summary>
    ''' save lipidmaps data repository.
    ''' </summary>
    ''' <param name="lipidmaps"></param>d
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' save the lipidmaps data object into file in messagepack format
    ''' </remarks>
    <ExportAPI("write.lipidmaps")>
    Public Function writeLipidMapsRepo(<RRawVectorArgument>
                                       lipidmaps As Object,
                                       file As Object,
                                       Optional env As Environment = Nothing) As Object

        Dim lipidstream As pipeline = pipeline.TryCreatePipeline(Of LipidMaps.MetaData)(lipidmaps, env)
        Dim output = GetFileStream(file, IO.FileAccess.Write, env)

        If output Like GetType(Message) Then
            Return output.TryCast(Of Message)
        ElseIf lipidstream.isError Then
            Return lipidstream.getError
        End If

        Return lipidstream.populates(Of LipidMaps.MetaData)(env).WriteRepository(output.TryCast(Of Stream))
    End Function

    ''' <summary>
    ''' read lipidmaps messagepack repository file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <param name="gsea_background">
    ''' and also cast the lipidmaps metabolite metadata to the gsea background model?
    ''' </param>
    ''' <returns></returns>
    ''' <example>
    ''' # gsea background model
    ''' let background = read.lipidmaps(
    '''     file = "./lipidmaps.msgpack", 
    '''     gsea.background = TRUE, 
    '''     category.model = FALSE
    ''' );
    ''' </example>
    <ExportAPI("read.lipidmaps")>
    <RApiReturn(GetType(LipidMaps.MetaData), GetType(Background), GetType(LipidMapsCategory))>
    Public Function readLipidMapsRepo(<RRawVectorArgument>
                                      file As Object,
                                      Optional gsea_background As Boolean = False,
                                      Optional category_model As Boolean = False,
                                      Optional env As Environment = Nothing) As Object

        Dim buffer = GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Dim lipidmaps = buffer.TryCast(Of Stream).ReadRepository

        If gsea_background Then
            Return lipidmaps.CreateCategoryBackground
        ElseIf category_model Then
            Return lipidmaps.CreateCategoryModel
        Else
            Return lipidmaps
        End If
    End Function

    <ExportAPI("lipid_classprofiles")>
    Public Function castToClassProfiles(lipid_class As LipidMapsCategory) As ClassProfiles
        Return New ClassProfiles With {
            .Catalogs = lipid_class.Class
        }
    End Function

    <ExportAPI("lipid_profiles")>
    Public Function lipidProfiles(categry As LipidMapsCategory, enrich As EnrichmentResult()) As Object
        Return categry.CreateEnrichmentProfiles(enrich)
    End Function

    ''' <summary>
    ''' Create lipid name helper for annotation
    ''' </summary>
    ''' <param name="lipidmaps"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' # cast sdf dataset to lipidmaps data object
    ''' let dataset = read.SDF(file = "./example.sdf", lazy = FALSE);
    ''' let lipids = dataset |> as.lipidmaps();
    ''' 
    ''' # create annotation helper
    ''' let lipidnames = lipid.names(lipids);
    ''' </example>
    <ExportAPI("lipid.names")>
    <RApiReturn(GetType(CompoundNameReader))>
    Public Function lipidNameReader(<RRawVectorArgument> lipidmaps As Object, Optional env As Environment = Nothing) As Object
        Dim lipids As pipeline = pipeline.TryCreatePipeline(Of LipidMaps.MetaData)(lipidmaps, env)

        If lipids.isError Then
            Return lipids.getError
        Else
            Return New LipidMaps.LipidNameReader(lipids.populates(Of LipidMaps.MetaData)(env))
        End If
    End Function

    ''' <summary>
    ''' Create lipid class helper for annotation
    ''' </summary>
    ''' <param name="lipidmaps"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' # cast sdf dataset to lipidmaps data object
    ''' let dataset = read.SDF(file = "./example.sdf", lazy = FALSE);
    ''' let lipids = dataset |> as.lipidmaps();
    ''' 
    ''' # create annotation helper
    ''' let class = lipid.class(lipids);
    ''' </example>
    <ExportAPI("lipid.class")>
    <RApiReturn(GetType(ClassReader))>
    Public Function lipidClassReader(<RRawVectorArgument> lipidmaps As Object, Optional env As Environment = Nothing) As Object
        Dim lipids As pipeline = pipeline.TryCreatePipeline(Of LipidMaps.MetaData)(lipidmaps, env)

        If lipids.isError Then
            Return lipids.getError
        Else
            Return New LipidMaps.LipidClassReader(lipids.populates(Of LipidMaps.MetaData)(env))
        End If
    End Function

    ''' <summary>
    ''' populate lipidmaps meta data objects from the loaded sdf data stream
    ''' </summary>
    ''' <param name="sdf">
    ''' a sequence of sdf molecular data which can be read from the ``read.SDF`` function. 
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' # cast sdf dataset to lipidmaps data object
    ''' let dataset = read.SDF(file = "./example.sdf", lazy = FALSE);
    ''' let lipids = dataset |> as.lipidmaps();
    ''' </example>
    <ExportAPI("as.lipidmaps")>
    Public Function toLipidMaps(<RRawVectorArgument>
                                sdf As Object,
                                Optional asList As Boolean = False,
                                Optional lazy As Boolean = True,
                                Optional env As Environment = Nothing) As Object

        Dim sdfStream As pipeline = pipeline.TryCreatePipeline(Of SDF)(sdf, env)

        If sdfStream.isError Then
            Return sdfStream.getError
        End If

        If asList Then
            Return New list With {
                .slots = sdfStream.populates(Of SDF)(env) _
                    .CreateMeta _
                    .ToDictionary(Function(l) l.LM_ID,
                                  Function(l)
                                      Return l.lipidmaps_data
                                  End Function)
            }
        Else
            Dim stream = sdfStream.populates(Of SDF)(env).CreateMeta

            If lazy Then
                Return stream.DoCall(AddressOf pipeline.CreateFromPopulator)
            Else
                Return stream.ToArray
            End If
        End If
    End Function

    <Extension>
    Private Function lipidmaps_data(l As LipidMaps.MetaData) As Object
        Dim list As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        If l.ABBREVIATION.StringEmpty Then
            list.add(NameOf(l.ABBREVIATION), l.ABBREVIATION)
        Else
            Dim lipidName As LipidName = LipidName.ParseLipidName(l.ABBREVIATION)

            list.add(NameOf(l.ABBREVIATION), {
                l.ABBREVIATION,
                lipidName.ToSystematicName,
                lipidName.ToOverviewName
            })

            list.add("lipid", lipidName.ToOverviewName)
        End If

        list.add(NameOf(l.CATEGORY), l.CATEGORY)
        list.add(NameOf(l.CHEBI_ID), l.CHEBI_ID)
        list.add(NameOf(l.CLASS_LEVEL4), l.CLASS_LEVEL4)
        ' list.add(NameOf(l.COMMON_NAME), l.COMMON_NAME)
        list.add(NameOf(l.EXACT_MASS), l.EXACT_MASS)
        list.add(NameOf(l.FORMULA), l.FORMULA)
        ' list.add(NameOf(l.HMDBID), l.HMDBID)
        list.add(NameOf(l.HMDB_ID), l.HMDB_ID)
        list.add(NameOf(l.INCHI), l.INCHI)
        list.add(NameOf(l.INCHI_KEY), l.INCHI_KEY)
        list.add(NameOf(l.KEGG_ID), l.KEGG_ID)
        list.add(NameOf(l.LIPIDBANK_ID), l.LIPIDBANK_ID)
        ' list.add(NameOf(l.LIPID_MAPS_CMPD_URL), l.LIPID_MAPS_CMPD_URL)
        list.add(NameOf(l.LM_ID), l.LM_ID)
        list.add(NameOf(l.MAIN_CLASS), l.MAIN_CLASS)
        ' list.add(NameOf(l.METABOLOMICS_ID), l.METABOLOMICS_ID)
        list.add(NameOf(l.NAME), l.NAME)
        list.add(NameOf(l.PLANTFA_ID), l.PLANTFA_ID)
        list.add(NameOf(l.PUBCHEM_CID), l.PUBCHEM_CID)
        ' list.add(NameOf(l.PUBCHEM_SID), l.PUBCHEM_SID)
        ' list.add(NameOf(l.PUBCHEM_SUBSTANCE_URL), l.PUBCHEM_SUBSTANCE_URL)
        list.add(NameOf(l.SMILES), l.SMILES)
        ' list.add(NameOf(l.STATUS), l.STATUS)
        list.add(NameOf(l.SUB_CLASS), l.SUB_CLASS)
        list.add(NameOf(l.SWISSLIPIDS_ID), l.SWISSLIPIDS_ID)
        list.add(NameOf(l.SYNONYMS), l.SYNONYMS)
        list.add(NameOf(l.SYSTEMATIC_NAME), l.SYSTEMATIC_NAME)

        Return CObj(list)
    End Function

    <ExportAPI("lipid.nameMaps")>
    Public Function lipidnameMapping(lipidmaps As EntityObject()) As EntityObject()
        Dim maps As New Dictionary(Of String, EntityObject)
        Dim lipidClass As New Regex("[A-Z]{2,}")
        Dim m As Match

        For Each lipid As EntityObject In lipidmaps
            Dim name As String = lipid!NAME

            If name <> "NULL" Then
                m = lipidClass.Match(name)

                If m.Success AndAlso name.StartsWith(m.Value) Then
                    maps(name) = New EntityObject With {
                        .ID = name,
                        .Properties = lipid.Properties
                    }

                    Dim nameAlt As String = name2(name)

                    If nameAlt <> name Then
                        maps(nameAlt) = New EntityObject With {
                            .ID = nameAlt,
                            .Properties = lipid.Properties
                        }
                    End If
                End If
            End If

            If lipid!SYNONYMS <> "NULL" Then
                Dim synonyms As String() = lipid!SYNONYMS.StringSplit(";\s*")

                For Each str As String In synonyms
                    m = lipidClass.Match(str)

                    If m.Success AndAlso str.StartsWith(m.Value) Then
                        maps(str) = New EntityObject With {
                            .ID = str,
                            .Properties = lipid.Properties
                        }

                        Dim nameAlt As String = name2(str)

                        If nameAlt <> str Then
                            maps(nameAlt) = New EntityObject With {
                                .ID = nameAlt,
                                .Properties = lipid.Properties
                            }
                        End If
                    End If
                Next
            End If

            If lipid!ABBREVIATION <> "NULL" Then
                Dim abbreviation As String() = lipid!ABBREVIATION.StringSplit(";\s*")

                For Each str As String In abbreviation
                    m = lipidClass.Match(str)

                    If m.Success AndAlso str.StartsWith(m.Value) Then
                        maps(str) = New EntityObject With {
                            .ID = str,
                            .Properties = lipid.Properties
                        }

                        Dim nameAlt As String = name2(str)

                        If nameAlt <> str Then
                            maps(nameAlt) = New EntityObject With {
                                .ID = nameAlt,
                                .Properties = lipid.Properties
                            }
                        End If
                    End If
                Next
            End If
        Next

        Return maps.Values.ToArray
    End Function

    Private Function name2(name1 As String) As String
        If name1.IndexOf("("c) > -1 Then
            Return name1
        ElseIf name1.IndexOf(" "c) = -1 Then
            Return name1
        Else
            Dim token As String() = name1.Split
            Dim name As String = $"{token(0)}({token.Skip(1).JoinBy("/")})"

            Return name
        End If
    End Function

    Public Function KEGGPathwayCoverages() As Object
        Throw New NotImplementedException
    End Function

    <ExportAPI("chebi.secondary2main.mapping")>
    Public Function chebiSecondary2Main(repository As String) As Dictionary(Of String, String())
        Return ChEBIRepo.ScanEntities(repository) _
            .GroupBy(Function(c) c.chebiId) _
            .Select(Function(c) c.First) _
            .ToDictionary(Function(c) c.chebiId,
                          Function(c)
                              Return c.SecondaryChEBIIds _
                                  .SafeQuery _
                                  .ToArray
                          End Function)
    End Function

    <ExportAPI("hmdb.secondary2main.mapping")>
    Public Function hmdbSecondary2Main(<RRawVectorArgument>
                                       repository As Object,
                                       Optional env As Environment = Nothing) As Dictionary(Of String, String())

        Dim metabolites As pipeline

        If TypeOf repository Is pipeline Then
            metabolites = repository
        Else
            metabolites = pipeline.TryCreatePipeline(Of HMDB.metabolite)(repository, env)

            If metabolites.isError Then
                metabolites = pipeline.CreateFromPopulator(HMDB.metabolite.Load(SMRUCC.Rsharp.Runtime.single(repository)))
            End If
        End If

        Return metabolites _
            .populates(Of HMDB.metabolite)(env) _
            .ToDictionary(Function(a) a.accession,
                          Function(a)
                              If a.secondary_accessions.accession.IsNullOrEmpty Then
                                  Return {a.accession}
                              Else
                                  Return a.secondary_accessions.accession
                              End If
                          End Function)
    End Function

    ''' <summary>
    ''' Create SecondaryIDSolver object from mapping file or mapping dictionary object data.
    ''' </summary>
    ''' <param name="mapping"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("secondary2main.mapping")>
    Public Function createIdMapping(mapping As Object, Optional envir As Environment = Nothing) As RReturn
        If mapping Is Nothing Then
            Return REnv.stop("No mapping data provided!", envir)
        ElseIf mapping.GetType Is GetType(String) Then
            mapping = DirectCast(mapping, String).LoadJSON(Of Dictionary(Of String, String()))
        End If

        Return SecondaryIDSolver.FromMaps(DirectCast(mapping, Dictionary(Of String, String()))).AsRReturn
    End Function

    ''' <summary>
    ''' Save id mapping file in json file format
    ''' </summary>
    ''' <param name="mapping"></param>
    ''' <param name="file">``*.json`` file name.</param>
    ''' <param name="envir"></param>
    ''' <returns></returns>
    <ExportAPI("save.mapping")>
    Public Function saveIDMapping(mapping As Dictionary(Of String, String()), file$, Optional envir As Environment = Nothing) As Object
        If mapping Is Nothing Then
            Return REnv.stop("No mapping data provided!", envir)
        ElseIf file.StringEmpty Then
            Return REnv.stop("Missing file parameter!", envir)
        End If

        Return mapping.GetJson.SaveTo(file)
    End Function

    <ExportAPI("glycosyl.tokens")>
    Public Function GlycosylTokens(glycosyl As String,
                                   Optional rules As list = Nothing,
                                   Optional env As Environment = Nothing) As String()

        Dim custom As Dictionary(Of String, String())

        If Not rules Is Nothing Then
            custom = rules.slots _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CLRVector.asCharacter(a.Value)
                              End Function)
        Else
            custom = New Dictionary(Of String, String())
        End If

        Return New GlycosylNameSolver(custom) _
            .GlycosylNameParser(glycosyl) _
            .ToArray
    End Function

    <ExportAPI("glycosyl.solver")>
    Public Function GlycosylNameSolver(Optional rules As list = Nothing) As GlycosylNameSolver
        Dim custom As Dictionary(Of String, String())

        If Not rules Is Nothing Then
            custom = rules.slots _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CLRVector.asCharacter(a.Value)
                              End Function)
        Else
            custom = New Dictionary(Of String, String())
        End If

        Return New GlycosylNameSolver(custom)
    End Function

    <ExportAPI("parse.ChEBI_entity")>
    <RApiReturn(GetType(ChEBIEntity))>
    Public Function ParseChebiEntity(xml As String) As Object
        Dim data = REST.ParsingRESTData(xml)

        If data.Length = 1 Then
            Return data(Scan0)
        Else
            Return data
        End If
    End Function

    ''' <summary>
    ''' extract the chebi annotation data from the chebi ontology data
    ''' </summary>
    ''' <param name="chebi"></param>
    ''' <returns></returns>
    <ExportAPI("extract_chebi_compounds")>
    Public Function ExtractChebiCompounds(chebi As OBOFile) As MetaInfo()
        Return chebi _
            .DoCall(AddressOf ChEBIObo.ImportsMetabolites) _
            .ToArray
    End Function

    <ExportAPI("rankingNames")>
    Public Function rankingNames(<RRawVectorArgument>
                                 x As Object,
                                 Optional max_len As Integer = 32,
                                 Optional min_len As Integer = 5) As Object

        Dim names As String() = CLRVector.asCharacter(x)

        If names.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim ranking = NameRanking.Ranking(names, maxLen:=max_len, minLen:=min_len).ToArray
        Dim name As String = ranking.First.value
        Dim synonym As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        For Each eval As NumericTagged(Of String) In ranking
            Call synonym.add(eval.value, eval.tag)
        Next

        Return New list(("name", CObj(name)), ("synonym", CObj(synonym)))
    End Function

    ''' <summary>
    ''' construct a new metabolite annotation information data
    ''' </summary>
    ''' <param name="xref">
    ''' the database cross reference links of current metabolite.
    ''' </param>
    ''' <returns></returns>
    ''' <example>
    ''' imports ["massbank", "annotation"] from "mzkit";
    ''' 
    ''' # an example of create metabolite annotation data
    ''' # for 'ATP'.
    ''' 
    ''' let xrefs = annotation::xref(
    '''     KEGG = 'C00002',
    '''     CAS = '56-65-5',
    '''     pubchem = '3304',
    '''     chebi = '15422',
    '''     KNApSAcK = 'C00001491'
    ''' );
    ''' let metabo = metabo_anno(
    '''     id = "ATP",
    '''     formula = "C10H16N5O13P3",
    '''     name = "ATP",
    '''     xref = xrefs,
    '''     synonym = ['ATP' 'Adenosine 5'-triphosphate']
    ''' );
    ''' 
    ''' print(JSON::json_encode(metabo));
    ''' </example>
    <ExportAPI("metabo_anno")>
    <RApiReturn(GetType(MetaLib))>
    Public Function meta_anno(id As String, formula As String, name As String,
                              Optional iupac_name As String = Nothing,
                              Optional xref As xref = Nothing,
                              <RRawVectorArgument> Optional synonym As Object = Nothing,
                              <RRawVectorArgument> Optional desc As Object = Nothing) As Object

        Return New MetaLib With {
            .ID = id,
            .formula = formula,
            .name = name,
            .IUPACName = iupac_name,
            .synonym = CLRVector.asCharacter(synonym),
            .description = CLRVector.asCharacter(desc).JoinBy(vbCrLf),
            .xref = xref,
            .exact_mass = FormulaScanner.EvaluateExactMass(formula)
        }
    End Function

    ''' <summary>
    ''' generates the inchikey hashcode based on the given inchi data
    ''' </summary>
    ''' <param name="inchi"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("inchikey")>
    Public Function inchikey(<RRawVectorArgument> inchi As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of String, InChIKey)(inchi, eval:=AddressOf IUPAC.MakeHashCode)
    End Function
End Module
