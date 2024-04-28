#Region "Microsoft.VisualBasic::6d024b7f3de2633695befc7a9a272efc, E:/mzkit/Rscript/Library/mzkit_app/src/mzkit//annotations/PubChem.vb"

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

    '   Total Lines: 504
    '    Code Lines: 286
    ' Comment Lines: 170
    '   Blank Lines: 48
    '     File Size: 22.14 KB


    ' Module PubChemToolKit
    ' 
    '     Function: CID, GetMetaInfo, ImageFlyGetImages, level1Terms, MeshBackground
    '               ParseMeshTree, pubchemUrl, pugView, queryExternalMetadata, QueryKnowledgeGraph
    '               queryPubChem, readPubmed, readPugViewXml, ReadSIDMap, readWebQuerySummary
    '               SIDMapTable
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemistry.NCBI
Imports BioNovoGene.BioDeep.Chemistry.NCBI.MeSH
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem.Graph
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem.Web
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH.Tree
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

''' <summary>
''' toolkit for handling of the ncbi pubchem data
''' </summary>
''' <remarks>
''' PubChem is a database of chemical molecules and their activities against biological assays. 
''' The system is maintained by the National Center for Biotechnology Information (NCBI), a 
''' component of the National Library of Medicine, which is part of the United States National 
''' Institutes of Health (NIH). PubChem can be accessed for free through a web user interface. 
''' Millions of compound structures and descriptive datasets can be freely downloaded via FTP. 
''' PubChem contains multiple substance descriptions and small molecules with fewer than 100 
''' atoms and 1,000 bonds. More than 80 database vendors contribute to the growing PubChem 
''' database.
''' 
''' ##### History
''' PubChem was released In 2004 As a component Of the Molecular Libraries Program (MLP) Of the
''' NIH. As Of November 2015, PubChem contains more than 150 million depositor-provided substance 
''' descriptions, 60 million unique chemical structures, And 225 million biological activity test 
''' results (from over 1 million assay experiments performed On more than 2 million small-molecules 
''' covering almost 10,000 unique protein target sequences that correspond To more than 5,000 genes).
''' It also contains RNA interference (RNAi) screening assays that target over 15,000 genes.
''' 
''' As of August 2018, PubChem contains 247.3 million substance descriptions, 96.5 million unique 
''' chemical structures, contributed by 629 data sources from 40 countries. It also contains 237 
''' million bioactivity test results from 1.25 million biological assays, covering >10,000 target 
''' protein sequences.
'''
''' As of 2020, with data integration from over 100 New sources, PubChem contains more than 293 
''' million depositor-provided substance descriptions, 111 million unique chemical structures,
''' And 271 million bioactivity data points from 1.2 million biological assays experiments.
''' </remarks>
<Package("pubchem_kit")>
<RTypeExport("pubmed", GetType(PubMed))>
Module PubChemToolKit

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(SIDMap()), AddressOf SIDMapTable)
    End Sub

    Private Function SIDMapTable(maps As SIDMap(), args As list, env As Environment) As Rdataframe
        Dim data As New Rdataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        data.columns(NameOf(SIDMap.SID)) = maps.Select(Function(m) m.SID).ToArray
        data.columns(NameOf(SIDMap.sourceName)) = maps.Select(Function(m) m.sourceName).ToArray
        data.columns(NameOf(SIDMap.registryIdentifier)) = maps.Select(Function(m) m.registryIdentifier).ToArray
        data.columns(NameOf(SIDMap.CID)) = maps.Select(Function(m) m.CID).ToArray

        Return data
    End Function

    ''' <summary>
    ''' read pubmed data table files
    ''' </summary>
    ''' <param name="file">A collection of the pubmed database ascii text file</param>
    ''' <param name="lazy">just create a lazy loader instead of read all 
    ''' content into memory at once?</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.pubmed")>
    <RApiReturn(GetType(PubMed))>
    Public Function readPubmed(file As String(), Optional lazy As Boolean = True, Optional env As Environment = Nothing) As Object
        If lazy Then
            Return file.Select(Function(path) DataStream.OpenHandle(path) _
                .AsLinq(Of PubMed)(silent:=True)).IteratesALL _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return file _
                .Select(Function(path) path.LoadCsv(Of PubMed)(mute:=True)) _
                .IteratesALL _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' Request the metabolite structure image via the pubchem image_fly api
    ''' </summary>
    ''' <param name="cid">A character vector of the pubchem cid for get the molecular 
    ''' structure data image.</param>
    ''' <param name="size"></param>
    ''' <param name="ignores_invalid_CID"></param>
    ''' <param name="env"></param>
    ''' <returns>A tuple list of the image data for the input pubchem metabolite cid query</returns>
    <ExportAPI("image_fly")>
    <RApiReturn(GetType(Image))>
    Public Function ImageFlyGetImages(<RRawVectorArgument>
                                      cid As Object,
                                      <RRawVectorArgument>
                                      Optional size As Object = "500,500",
                                      Optional ignores_invalid_CID As Boolean = False,
                                      Optional env As Environment = Nothing) As Object

        Dim ids As String() = CLRVector.asCharacter(cid)
        Dim invalids As Index(Of String) = ids _
            .Where(Function(id) Not id.IsPattern("\d+")) _
            .Indexing
        Dim images As New list
        Dim sizeVector As Integer() = InteropArgumentHelper _
            .getSize(size, env, [default]:="500,500") _
            .SizeParser _
            .ToArray
        Dim img As Image

        For Each id As String In ids
            If ignores_invalid_CID AndAlso id Like invalids Then
                Continue For
            End If

            img = ImageFly.GetImage(id, sizeVector(0), sizeVector(1), doBgTransparent:=False)

            Call Thread.Sleep(1000)
            Call images.slots.Add(id, img)
        Next

        Return images
    End Function

    ''' <summary>
    ''' query of the pathways, taxonomy and reaction 
    ''' data from the pubchem database.
    ''' </summary>
    ''' <param name="cid"></param>
    ''' <param name="cache">
    ''' A local dir path for the cache data or a filesystem wrapper object
    ''' </param>
    ''' <param name="interval">
    ''' the sleep time interval in ms
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("query.external")>
    Public Function queryExternalMetadata(cid As String,
                                          Optional cache As Object = "./pubchem/",
                                          Optional interval As Integer = -1,
                                          Optional env As Environment = Nothing) As list

        Dim query As QueryPathways
        Dim result As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        If cache Is Nothing Then
            cache = "./pubchem/"
            env.AddMessage("The cache is nothing, use the dir './pubchem/' in current workdir as cache.")
        End If
        If cache.GetType.ImplementInterface(Of IFileSystemEnvironment) Then
            query = New QueryPathways(DirectCast(cache, IFileSystemEnvironment), interval:=interval)
        Else
            query = New QueryPathways(CLRVector.asCharacter(cache).First, interval:=interval)
        End If

        Call result.add("pathways", query.QueryCacheText(New NamedValue(Of PubChem.Types)(cid, PubChem.Types.pathways), cacheType:=".json"))
        Call result.add("taxonomy", query.QueryCacheText(New NamedValue(Of PubChem.Types)(cid, PubChem.Types.taxonomy), cacheType:=".json"))
        Call result.add("reaction", query.QueryCacheText(New NamedValue(Of PubChem.Types)(cid, PubChem.Types.reaction), cacheType:=".json"))

        Return result
    End Function

    ''' <summary>
    ''' Query the compound related biological context information from pubchem
    ''' </summary>
    ''' <param name="cid"></param>
    ''' <param name="cache"></param>
    ''' <returns>
    ''' A tuple list of the knowledge data that associated with the given pubchem metabolite:
    ''' 
    ''' 1. genes: the co-occurance genes with the compound 
    ''' 2. disease: a list of the related disease with the compound
    ''' 3. compounds: the co-occurance compound data
    ''' 
    ''' all of the slot data is a collection of the mzkit pubchem <see cref="MeshGraph"/> 
    ''' clr object.
    ''' </returns>
    <ExportAPI("query.knowlegde_graph")>
    <RApiReturn("genes", "disease", "compounds")>
    Public Function QueryKnowledgeGraph(cid As String,
                                        Optional cache As Object = "./graph_kb",
                                        Optional env As Environment = Nothing) As Object

        Dim geneSet As MeshGraph()
        Dim diseaseSet As MeshGraph()
        Dim metaboliteSet As MeshGraph()

        If cache Is Nothing Then
            cache = "./graph_kb/"
            Call env.AddMessage("the required cache filesystem value is nothing, use the default './graph_kb' location.")
        End If

        If TypeOf cache Is String Then
            ' is a directory path
            geneSet = WebGraph.Query(cid, PubChem.Graph.Types.ChemicalGeneSymbolNeighbor, cache)
            diseaseSet = WebGraph.Query(cid, PubChem.Graph.Types.ChemicalDiseaseNeighbor, cache)
            metaboliteSet = WebGraph.Query(cid, PubChem.Graph.Types.ChemicalNeighbor, cache)
        ElseIf cache.GetType.ImplementInterface(Of IFileSystemEnvironment) Then
            Dim web As New WebGraph(DirectCast(cache, IFileSystemEnvironment))

            geneSet = web.Query(cid, PubChem.Graph.Types.ChemicalGeneSymbolNeighbor)
            diseaseSet = web.Query(cid, PubChem.Graph.Types.ChemicalDiseaseNeighbor)
            metaboliteSet = web.Query(cid, PubChem.Graph.Types.ChemicalNeighbor)
        Else
            Return Message.InCompatibleType(GetType(IFileSystemEnvironment), cache.GetType, env)
        End If

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"genes", geneSet},
                {"disease", diseaseSet},
                {"compounds", metaboliteSet}
            }
        }
    End Function

    ''' <summary>
    ''' query cid from pubchem database
    ''' </summary>
    ''' <param name="name">any search term for query the pubchem database</param>
    ''' <param name="cache">the cache fs for the online pubchem database</param>
    ''' <param name="offline">running the search query handler in offline mode?</param>
    ''' <param name="interval">
    ''' the time sleep interval in ms
    ''' </param>
    ''' <returns>A character vector of the pubchem cid that matches the given input ``name``.
    ''' </returns>
    <ExportAPI("CID")>
    <RApiReturn(GetType(String))>
    Public Function CID(name As String,
                        Optional cache As Object = "./.pubchem",
                        Optional offline As Boolean = False,
                        Optional interval As Integer = -1,
                        Optional env As Environment = Nothing) As Object

        If cache Is Nothing Then
            cache = "./.pubchem/"
            env.AddMessage("The cache of pubchem CID query is nothing, use the default local directory './.pubchem/' at current workdir as default cache location.")
        End If

        If TypeOf cache Is String Then
            Return Query.QueryCID(
                name:=name,
                cacheFolder:=CStr(cache),
                offlineMode:=offline,
                hitCache:=Nothing,
                interval:=interval
            )
        ElseIf cache.GetType.ImplementInterface(Of IFileSystemEnvironment) Then
            Return Query.QueryCID(
                name:=name,
                cacheFolder:=DirectCast(cache, IFileSystemEnvironment),
                offlineMode:=offline,
                hitCache:=Nothing,
                interval:=interval
            )
        Else
            Return Message.InCompatibleType(GetType(String), cache.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' Generate the url for get pubchem pugviews data object
    ''' </summary>
    ''' <param name="cid">The pubchem compound cid, should be an integer value</param>
    ''' <returns>A url for get the pubchem data in pugview format</returns>
    <ExportAPI("pubchem_url")>
    <RApiReturn(GetType(String))>
    Public Function pubchemUrl(<RRawVectorArgument> cid As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of String, String)(cid, AddressOf WebQuery.pugViewApi)
    End Function

    ''' <summary>
    ''' query of the pubchem database
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="cache">
    ''' A directory path that used for cache the pubchem data
    ''' </param>
    ''' <returns>A collection of the metabolite annotation data.</returns>
    <ExportAPI("query")>
    <RApiReturn(GetType(MetaLib))>
    Public Function queryPubChem(<RRawVectorArgument>
                                 id As Object,
                                 Optional cache$ = "./",
                                 Optional env As Environment = Nothing) As list

        Dim ids As String() = CLRVector.asCharacter(id)
        Dim cid As String()
        Dim query As New Dictionary(Of String, PugViewRecord)
        Dim result As New list With {
            .slots = New Dictionary(Of String, Object)
        }
        Dim meta As Dictionary(Of String, MetaLib)

        For Each term As String In ids.Distinct.ToArray
            query = PubChem.QueryPugViews(term, cacheFolder:=cache)
            cid = query.Keys.ToArray
            meta = query _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  If a.Value Is Nothing Then
                                      Return Nothing
                                  Else
                                      Return a.Value.GetMetaInfo
                                  End If
                              End Function)

            Call result.slots.Add(term, meta)
        Next

        Return result
    End Function

    ''' <summary>
    ''' query pubchem data via a given cid value
    ''' </summary>
    ''' <param name="cid"></param>
    ''' <param name="cacheFolder">
    ''' A cache directory path to the pubchem xml files
    ''' </param>
    ''' <param name="offline"></param>
    ''' <param name="env"></param>
    ''' <returns>A collection of the pubchem pug view object that contains the metabolite annotation information.</returns>
    '''
    <ExportAPI("pugView")>
    <RApiReturn(GetType(PugViewRecord))>
    Public Function pugView(<RRawVectorArgument> cid As Object,
                            Optional cacheFolder$ = "./pubchem_cache",
                            Optional offline As Boolean = False,
                            Optional env As Environment = Nothing) As Object

        Dim api As WebQuery = $"{cacheFolder}/pugViews/".GetQueryHandler(Of WebQuery)(offline)
        Dim result = env.EvaluateFramework(Of String, PugViewRecord)(
            x:=cid,
            eval:=Function(id)
                      Return api.Query(Of PugViewRecord)(id)
                  End Function)

        Return result
    End Function

    ''' <summary>
    ''' parse the pubchem sid map data file
    ''' </summary>
    ''' <param name="sidMapText"></param>
    ''' <param name="skipNoCID">
    ''' skip of the sid map item which has no cid assigned yet?
    ''' </param>
    ''' <param name="dbfilter">
    ''' filter out the sid map data with a specific given db name
    ''' </param>
    ''' <returns>A collection of the map data that could be used for get the
    ''' knowledge base id mapping from external database, and map between the 
    ''' pubchem sid and cid.</returns>
    <ExportAPI("SID_map")>
    <RApiReturn(GetType(SIDMap))>
    Public Function ReadSIDMap(sidMapText As String,
                               Optional skipNoCID As Boolean = True,
                               Optional dbfilter$ = Nothing) As SIDMap()

        Dim ls As SIDMap() = SIDMap _
            .GetMaps(handle:=sidMapText, skipNoCID:=skipNoCID) _
            .ToArray

        If Not dbfilter.StringEmpty Then
            ls = ls _
                .Where(Function(map) map.sourceName = dbfilter) _
                .ToArray
        End If

        Return ls
    End Function

    ''' <summary>
    ''' read xml text and then parse as pugview record data object
    ''' </summary>
    ''' <param name="file">
    ''' the file path or the xml text content
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.pugView")>
    <RApiReturn(GetType(PugViewRecord))>
    Public Function readPugViewXml(file As String) As PugViewRecord
        If file.FileExists Then
            Return file.LoadXml(Of PugViewRecord)
        Else
            Return file.LoadFromXml(Of PugViewRecord)
        End If
    End Function

    ''' <summary>
    ''' read the pubchem webquery summary xml file
    ''' </summary>
    ''' <param name="file">
    ''' the file path to the pubchem query search result file, the data file which
    ''' could be downloaded from the query result url example like: 
    ''' 
    ''' > https://pubchem.ncbi.nlm.nih.gov/sdq/sdqagent.cgi?infmt=json&amp;outfmt=xml&amp;query={%22download%22:%22*%22,%22collection%22:%22compound%22,%22order%22:[%22relevancescore,desc%22],%22start%22:1,%22limit%22:10000000,%22downloadfilename%22:%22PubChem_compound_text_kegg%22,%22where%22:{%22ands%22:[{%22*%22:%22kegg%22}]}}
    ''' </param>
    ''' <param name="convert_std">
    ''' convert to a unify metabolite data model inside mzkit?
    ''' </param>
    ''' <returns>A collection of the pubchem query summary <see cref="QueryXml"/> object
    ''' (or <see cref="MetaLib"/> data model if the parameter <paramref name="convert_std"/> 
    ''' has been set to ``true``) that parsed from download result file.
    ''' </returns>
    <ExportAPI("read.webquery")>
    <RApiReturn(GetType(QueryXml), GetType(MetaLib))>
    Public Function readWebQuerySummary(file As String, Optional convert_std As Boolean = False) As pipeline
        Dim pull As IEnumerable(Of QueryXml) = QueryXml.Load(file)

        If convert_std Then
            Return pull.ToArray _
                .AsParallel _
                .Select(Function(m) m.CreateMetadata) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return pipeline.CreateFromPopulator(pull)
        End If
    End Function

    ''' <summary>
    ''' extract the compound annotation data
    ''' </summary>
    ''' <param name="pugView"></param>
    ''' <returns></returns>
    <ExportAPI("metadata.pugView")>
    Public Function GetMetaInfo(pugView As PugViewRecord) As MetaLib
        Return pugView.GetMetaInfo
    End Function

    ''' <summary>
    ''' Parse the mesh ontology tree
    ''' </summary>
    ''' <param name="file">A text file data that contains the mesh ontology tree data</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.mesh_tree")>
    <RApiReturn(GetType(Term))>
    Public Function ParseMeshTree(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim stream = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If stream Like GetType(Message) Then
            Return stream.TryCast(Of Message)
        End If

        Return MeSH.Tree.ParseTree(New StreamReader(stream.TryCast(Of Stream)))
    End Function

    ''' <summary>
    ''' create MeSH ontology gsea background based on the mesh tree
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <param name="clusters">
    ''' Create the mesh background about another topic
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("mesh_background")>
    Public Function MeshBackground(mesh As Tree(Of Term), Optional clusters As Background = Nothing) As Background
        If clusters Is Nothing Then
            Return mesh.MeshTermOntologyBackground
        Else
            Return mesh.MeshTopicBackground(clusters)
        End If
    End Function

    ''' <summary>
    ''' gets the level1 term label of the mesh tree
    ''' </summary>
    ''' <param name="mesh"></param>
    ''' <returns>A character vector of the ontology term label</returns>
    <ExportAPI("mesh_level1")>
    Public Function level1Terms(mesh As Tree(Of Term)) As String()
        Return mesh.Childs.Values.Select(Function(c) c.Data.ToString).ToArray
    End Function
End Module
