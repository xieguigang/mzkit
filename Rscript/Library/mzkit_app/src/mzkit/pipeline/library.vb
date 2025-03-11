#Region "Microsoft.VisualBasic::7550527bd1200c840a08cec23604e769, Rscript\Library\mzkit_app\src\mzkit\pipeline\library.vb"

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

    '   Total Lines: 1167
    '    Code Lines: 614 (52.61%)
    ' Comment Lines: 430 (36.85%)
    '    - Xml Docs: 93.49%
    ' 
    '   Blank Lines: 123 (10.54%)
    '     File Size: 59.23 KB


    ' Module library
    ' 
    '     Function: assertAdducts, checkInSourceFragments, create_reportTable, create_table, create_workspace
    '               createAnnotation, filter_unique, GetAnnotations, ionsFromPeaktable, loadAll
    '               LoadLocalDatabase, loadPeaktable, loadWorkspace, MakeMoNALibrary, openRepository
    '               OpenResultPack, PopulateIonData, readResultPack, Save, set_xicCache
    '               tohtmlString, uniqueAnnotations, writeResultPack, writeWorkspace, xref
    ' 
    '     Sub: commit, Main, peak_assign, push_temp, saveAnnotation
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.MetaLib
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports MetaData = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaInfo
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' the metabolite annotation toolkit
''' </summary>
<Package("annotation")>
<RTypeExport("xref", GetType(xref))>
<RTypeExport("library_workspace", GetType(LibraryWorkspace))>
Module library

    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(LibraryWorkspace), AddressOf writeWorkspace)
        Call RInternal.generic.add("writeBin", GetType(AnnotationPack), AddressOf writeResultPack)
        Call RInternal.generic.add("readBin.library_workspace", GetType(Stream), AddressOf loadWorkspace)

        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(Peaktable()), AddressOf create_table)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(AnnotationData(Of xref)()), AddressOf create_reportTable)

        Call htmlPrinter.AttachHtmlFormatter(Of AnnotationPack)(AddressOf tohtmlString)
    End Sub

    Public Function create_reportTable(data As AnnotationData(Of xref)(), args As list, env As Environment) As dataframe
        Dim report As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim xrefs = data.Select(Function(a) a.Xref).ToArray
        Dim aligns = data.Select(Function(a) a.Alignment).ToArray

        Call report.add("metabolite_id", From mi As AnnotationData(Of xref) In data Select mi.ID)
        Call report.add("name", From mi As AnnotationData(Of xref) In data Select mi.CommonName)
        Call report.add("formula", From mi As AnnotationData(Of xref) In data Select mi.Formula)
        Call report.add("exact_mass", From mi As AnnotationData(Of xref) In data Select mi.ExactMass)

        Call report.add("chebi", From xi As xref In xrefs Select xi.chebi)
        Call report.add("pubchem", From xi As xref In xrefs Select xi.pubchem)
        Call report.add("cas", From xi As xref In xrefs Select xi.CAS?.FirstOrDefault)
        Call report.add("kegg", From xi As xref In xrefs Select xi.KEGG)
        Call report.add("hmdb", From xi As xref In xrefs Select xi.HMDB)
        Call report.add("lipidmaps", From xi As xref In xrefs Select xi.lipidmaps)
        Call report.add("mesh", From xi As xref In xrefs Select xi.MeSH)

        Call report.add("inchikey", From xi As xref In xrefs Select xi.InChIkey)
        Call report.add("inchi", From xi As xref In xrefs Select xi.InChI)
        Call report.add("smiles", From xi As xref In xrefs Select xi.SMILES)

        Call report.add("kingdom", From mi As AnnotationData(Of xref) In data Select mi.kingdom)
        Call report.add("super_class", From mi As AnnotationData(Of xref) In data Select mi.super_class)
        Call report.add("class", From mi As AnnotationData(Of xref) In data Select mi.class)
        Call report.add("sub_class", From mi As AnnotationData(Of xref) In data Select mi.sub_class)
        Call report.add("molecular_framework", From mi As AnnotationData(Of xref) In data Select mi.molecular_framework)

        Call report.add("forward", From ai As AlignmentOutput In aligns Select ai.forward)
        Call report.add("reverse", From ai As AlignmentOutput In aligns Select ai.reverse)
        Call report.add("jaccard", From ai As AlignmentOutput In aligns Select ai.jaccard)
        Call report.add("entropy", From ai As AlignmentOutput In aligns Select ai.entropy)

        Call report.add("evidence", From ai As AlignmentOutput In aligns Select ai.reference.id)

        Call report.add("mz", From ai As AlignmentOutput In aligns Select ai.query.mz)
        Call report.add("rt", From ai As AlignmentOutput In aligns Select ai.query.scan_time)
        Call report.add("alignment", From ai As AlignmentOutput In aligns Select ai.alignment_str)

        Return report
    End Function

    <RGenericOverloads("as.data.frame")>
    Public Function create_table(data As Peaktable(), args As list, env As Environment) As dataframe
        Dim vec As New VectorShadows(Of Peaktable)(data)
        Dim v As Object = vec
        Dim rowId As String() = CLRVector.asCharacter(v.name & "_" & v.annotation)
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = rowId
        }

        For Each name As String In vec.GetDataProperties
            ' If name <> NameOf(ReportTable.samples) Then
            Call df.add(If(vec.GetMapName(name), name), DirectCast(vec(name), Array))
            ' End If
        Next

        Return df
    End Function

    Private Function loadWorkspace(file As Stream, args As list, env As Environment) As Object
        Dim mz_bin As Boolean = args.getValue("mz_bin", env, [default]:=False)
        Dim libs = LibraryWorkspace.read(file, mz_bin)

        Return libs
    End Function

    Private Function writeResultPack(pack As AnnotationPack, args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Dim file As New AnnotationWorkspace(con)

        Call file.SetPeakTable(pack.peaks)

        For Each libs In pack.libraries
            Call file.CreateLibraryResult(libs.Key, libs.Value)
        Next

        Call file.Dispose()

        Return True
    End Function

    Private Function writeWorkspace(table As LibraryWorkspace, args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Dim commit_peaks As Boolean = args.getValue("commit_peaks", env, [default]:=False)
        Call table.save(con, commit_peaks)
        Call con.Flush()
        Return True
    End Function

    <RGenericOverloads(htmlPrinter.toHtml_apiName)>
    Public Function tohtmlString(pack As AnnotationPack, args As list, env As Environment) As Object
        Dim biodeep_id As String() = Nothing
        Dim cell_render_rt As Boolean = args.getValue(Of Boolean)("cell_render.rt", env, [default]:=False)

        If args.hasName("id") Then
            biodeep_id = CLRVector.asCharacter(args.getByName("id"))
        Else
            Throw New NotImplementedException
        End If

        Return New ReportRender(pack).HtmlTable(biodeep_id, cell_render_rt)
    End Function

    ''' <summary>
    ''' Create the database cross reference links
    ''' </summary>
    ''' <param name="chebi">Chemical Entities of Biological Interest (ChEBI)
    ''' 
    ''' Chemical Entities of Biological Interest, also known as ChEBI, is 
    ''' a chemical database and ontology of molecular entities focused on 
    ''' 'small' chemical compounds, that is part of the Open Biomedical 
    ''' Ontologies (OBO) effort at the European Bioinformatics Institute 
    ''' (EBI). The term "molecular entity" refers to any "constitutionally 
    ''' or isotopically distinct atom, molecule, ion, ion pair, radical, 
    ''' radical ion, complex, conformer, etc., identifiable as a separately 
    ''' distinguishable entity". The molecular entities in question are 
    ''' either products of nature or synthetic products which have potential 
    ''' bioactivity. Molecules directly encoded by the genome, such as 
    ''' nucleic acids, proteins and peptides derived from proteins by 
    ''' proteolytic cleavage, are not as a rule included in ChEBI.
    ''' 
    ''' ChEBI uses nomenclature, symbolism And terminology endorsed by the 
    ''' International Union Of Pure And Applied Chemistry (IUPAC) And 
    ''' nomenclature committee Of the International Union Of Biochemistry 
    ''' And Molecular Biology (NC-IUBMB).
    ''' </param>
    ''' <param name="KEGG">KEGG (Kyoto Encyclopedia of Genes and Genomes) is 
    ''' a collection of databases dealing with genomes, biological pathways, 
    ''' diseases, drugs, and chemical substances. KEGG is utilized for 
    ''' bioinformatics research and education, including data analysis in genomics, 
    ''' metagenomics, metabolomics and other omics studies, modeling and simulation 
    ''' in systems biology, and translational research in drug development.
    ''' 
    ''' The KEGG database project was initiated In 1995 by Minoru Kanehisa, 
    ''' professor at the Institute For Chemical Research, Kyoto University, 
    ''' under the Then ongoing Japanese Human Genome Program. Foreseeing the need 
    ''' For a computerized resource that can be used For biological interpretation 
    ''' Of genome sequence data, he started developing the KEGG PATHWAY database. 
    ''' It Is a collection Of manually drawn KEGG pathway maps representing 
    ''' experimental knowledge On metabolism And various other functions Of the 
    ''' cell And the organism. Each pathway map contains a network Of molecular 
    ''' interactions And reactions And Is designed To link genes In the genome 
    ''' To gene products (mostly proteins) In the pathway. This has enabled the 
    ''' analysis called KEGG pathway mapping, whereby the gene content In the 
    ''' genome Is compared With the KEGG PATHWAY database To examine which 
    ''' pathways And associated functions are likely To be encoded In the 
    ''' genome.
    ''' 
    ''' According to the developers, KEGG Is a "computer representation" of the 
    ''' biological system. It integrates building blocks And wiring diagrams of 
    ''' the system—more specifically, genetic building blocks of genes And proteins, 
    ''' chemical building blocks of small molecules And reactions, And wiring 
    ''' diagrams of molecular interaction And reaction networks. This concept 
    ''' Is realized in the following databases of KEGG, which are categorized 
    ''' into systems, genomic, chemical, And health information.</param>
    ''' <param name="pubchem">PubChem is a database of chemical molecules and 
    ''' their activities against biological assays. The system is maintained by 
    ''' the National Center for Biotechnology Information (NCBI), a component of 
    ''' the National Library of Medicine, which is part of the United States National 
    ''' Institutes of Health (NIH). PubChem can be accessed for free through a web 
    ''' user interface. Millions of compound structures and descriptive datasets 
    ''' can be freely downloaded via FTP. PubChem contains multiple substance descriptions 
    ''' and small molecules with fewer than 100 atoms and 1,000 bonds. More than 
    ''' 80 database vendors contribute to the growing PubChem database.</param>
    ''' <param name="HMDB">The Human Metabolome Database (HMDB) is a comprehensive, 
    ''' high-quality, freely accessible, online database of small molecule metabolites 
    ''' found in the human body. It bas been created by the Human Metabolome Project 
    ''' funded by Genome Canada and is one of the first dedicated metabolomics 
    ''' databases. The HMDB facilitates human metabolomics research, including the 
    ''' identification and characterization of human metabolites using NMR spectroscopy, 
    ''' GC-MS spectrometry and LC/MS spectrometry. To aid in this discovery process, 
    ''' the HMDB contains three kinds of data: 1) chemical data, 2) clinical data, and 
    ''' 3) molecular biology/biochemistry data (Fig. 1–3). The chemical data includes 
    ''' 41,514 metabolite structures with detailed descriptions along with nearly 
    ''' 10,000 NMR, GC-MS and LC/MS spectra.</param>
    ''' <param name="metlin">The METLIN Metabolite and Chemical Entity Database is the 
    ''' largest repository of experimental tandem mass spectrometry and neutral loss data 
    ''' acquired from standards. The tandem mass spectrometry data on over 870,000 molecular 
    ''' standards (as of August 16, 2022) is provided to facilitate the identification 
    ''' of chemical entities from tandem mass spectrometry experiments. In addition to the 
    ''' identification of known molecules, it is also useful for identifying unknowns using 
    ''' its similarity searching technology. All tandem mass spectrometry data comes from 
    ''' the experimental analysis of standards at multiple collision energies and in both 
    ''' positive and negative ionization modes.
    ''' 
    ''' METLIN serves as a data management system to assist in metabolite And chemical 
    ''' entity identification by providing public access to its repository of comprehensive 
    ''' MS/MS And neutral loss data. METLIN's annotated list of molecular standards include 
    ''' metabolites and other chemical entities, searching METLIN can be done based on a 
    ''' molecule's tandem mass spectrometry data, neutral loss masses, precursor mass, 
    ''' chemical formula, and structure within the METLIN website. Each molecule is linked
    ''' to outside resources such as the Kyoto Encyclopedia of Genes and Genomes (KEGG) for 
    ''' further reference and inquiry. The METLIN database was developed and is maintained 
    ''' solely by the Siuzdak laboratory at The Scripps Research Institute.</param>
    ''' <param name="DrugBank">The DrugBank database is a comprehensive, freely accessible, 
    ''' online database containing information on drugs and drug targets created and maintained 
    ''' by the University of Alberta and The Metabolomics Innovation Centre located in Alberta, 
    ''' Canada. As both a bioinformatics and a cheminformatics resource, DrugBank combines 
    ''' detailed drug (i.e. chemical, pharmacological and pharmaceutical) data with 
    ''' comprehensive drug target (i.e. sequence, structure, and pathway) information. DrugBank 
    ''' has used content from Wikipedia; Wikipedia also often links to Drugbank, posing 
    ''' potential circular reporting issues.</param>
    ''' <param name="ChEMBL">ChEMBL or ChEMBLdb is a manually curated chemical database of 
    ''' bioactive molecules with drug inducing properties. It is maintained by the European 
    ''' Bioinformatics Institute (EBI), of the European Molecular Biology Laboratory (EMBL), 
    ''' based at the Wellcome Trust Genome Campus, Hinxton, UK.
    '''
    ''' The database, originally known As StARlite, was developed by a biotechnology company 
    ''' called Inpharmatica Ltd. later acquired by Galapagos NV. The data was acquired For 
    ''' EMBL In 2008 With an award from The Wellcome Trust, resulting In the creation Of the 
    ''' ChEMBL chemogenomics group at EMBL-EBI, led by John Overington.</param>
    ''' <param name="Wikipedia">Wikipedia is a free-content online encyclopedia written and 
    ''' maintained by a community of volunteers, collectively known as Wikipedians, through
    ''' open collaboration and using a wiki-based editing system called MediaWiki. Wikipedia 
    ''' is the largest and most-read reference work in history. It has consistently been one 
    ''' of the 10 most popular websites in the world, and, as of 2023, ranks as the 4th most 
    ''' viewed website by Semrush. Founded by Jimmy Wales and Larry Sanger on January 15, 
    ''' 2001, it is hosted by the Wikimedia Foundation, an American nonprofit organization.
    ''' </param>
    ''' <param name="lipidmaps">LIPID MAPS (Lipid Metabolites and Pathways Strategy) is a web 
    ''' portal designed to be a gateway to Lipidomics resources. The resource has spearheaded 
    ''' a classification of biological lipids, dividing them into eight general categories. 
    ''' LIPID MAPS provides standardised methodologies for mass spectrometry analysis of lipids, e.g. 
    '''
    ''' LIPID MAPS has been cited As evidence Of a growing appreciation Of the study Of lipid
    ''' metabolism And the rapid development And standardisation Of the lipidomics field 
    '''
    ''' Key LIPID MAPS resources include:
    '''
    ''' 1. LIPID MAPS Structure Database (LMSD) - a database Of structures And annotations Of 
    '''    biologically relevant lipids, containing over 46000 different lipids. The paper 
    '''    describing this resource has, according To PubMed, been cited more than 200 times.
    ''' 2. LIPID MAPS In-Silico Structure Database (LMISSD) - a database Of computationally 
    '''    predicted lipids generated by expansion Of headgroups For commonly occurring lipid 
    '''    classes
    ''' 3. LIPID MAPS Gene/Proteome Database (LMPD) - a database Of genes And gene products 
    '''    which are involved In lipid metabolism
    '''    
    ''' Tools available from LIPID MAPS enable scientists To identify likely lipids In their 
    ''' samples from mass spectrometry data, a common method To analyse lipids In biological 
    ''' specimens. In particular, LipidFinder enables analysis Of MS data. Tutorials And 
    ''' educational material On lipids are also available at the site.
    ''' 
    ''' In January 2020, LIPID MAPS became an ELIXIR service.</param>
    ''' <param name="MeSH">Medical Subject Headings (MeSH) is a comprehensive controlled 
    ''' vocabulary for the purpose of indexing journal articles and books in the life sciences. 
    ''' It serves as a thesaurus that facilitates searching. Created and updated by the 
    ''' United States National Library of Medicine (NLM), it is used by the MEDLINE/PubMed 
    ''' article database and by NLM's catalog of book holdings. MeSH is also used by 
    ''' ClinicalTrials.gov registry to classify which diseases are studied by trials registered 
    ''' in ClinicalTrials.
    '''
    ''' MeSH was introduced In the 1960S, With the NLM's own index catalogue and the subject
    ''' headings of the Quarterly Cumulative Index Medicus (1940 edition) as precursors. 
    ''' The yearly printed version of MeSH was discontinued in 2007; MeSH is now available 
    ''' only online. It can be browsed and downloaded free of charge through PubMed. Originally 
    ''' in English, MeSH has been translated into numerous other languages and allows 
    ''' retrieval of documents from different origins.</param>
    ''' <param name="ChemIDplus">ChemIDplus (Chemical Identification Plus Database)
    ''' 
    ''' ChemIDplus was a dictionary Of over 400,000 chemicals (names, synonyms, And structures). 
    ''' ChemIDplus includes links To NLM And other databases And resources, including links To 
    ''' federal, state And international agencies. NLM makes a subset Of ChemIDplus data available 
    ''' For download. The ChemIDplus Subset does Not include the Structure Or the toxicity data 
    ''' available from the NLM web versions Of the database. 
    ''' </param>
    ''' <param name="MetaCyc">The MetaCyc database is one of the largest metabolic pathways 
    ''' and enzymes databases currently available. The data in the database is manually 
    ''' curated from the scientific literature, and covers all domains of life. MetaCyc has 
    ''' extensive information about chemical compounds, reactions, metabolic pathways and 
    ''' enzymes. The data have been curated from more than 58,000 publications.
    '''
    ''' MetaCyc has been designed For multiple types Of uses. It Is often used As an extensive 
    ''' online encyclopedia Of metabolism. In addition, MetaCyc Is used As a reference data 
    ''' Set For computationally predicting the metabolic network Of organisms from their sequenced 
    ''' genomes; it has been used To perform pathway predictions For thousands Of organisms, 
    ''' including those In the BioCyc Database Collection. MetaCyc Is also used In metabolic 
    ''' engineering And metabolomics research.
    '''
    ''' MetaCyc includes mini reviews For pathways And enzymes that provide background information 
    ''' As well As relevant literature references. It also provides extensive data On individual 
    ''' enzymes, describing their subunit Structure, cofactors, activators And inhibitors, 
    ''' substrate specificity, And, When available, kinetic constants. MetaCyc data On metabolites 
    ''' includes chemical structures, predicted Standard energy Of formation, And links To external 
    ''' databases. Reactions In MetaCyc are presented In a visual display that includes the 
    ''' structures Of all components. The reactions are balanced And include EC numbers, reaction 
    ''' direction, predicted atom mappings that describe the correspondence between atoms In 
    ''' the reactant compounds And the product compounds, And computed Gibbs free energy.
    '''
    ''' All objects In MetaCyc are clickable And provide easy access To related objects. For example,
    ''' the page For L-lysine lists all Of the reactions In which L-lysine participates, 
    ''' As well As the enzymes that catalyze them And pathways In which these reactions take place.</param>
    ''' <param name="KNApSAcK">KNApSAcK: A Comprehensive Species-Metabolite Relationship Database</param>
    ''' <param name="CAS">A CAS Registry Number (also referred to as CAS RN or informally CAS 
    ''' Number) is a unique identification number assigned by the Chemical Abstracts Service 
    ''' (CAS) in the US to every chemical substance described in the open scientific literature. 
    ''' It includes all substances described since 1957, plus some substances from as far 
    ''' back as the early 1800s. It is a chemical database that includes organic and inorganic 
    ''' compounds, minerals, isotopes, alloys, mixtures, and nonstructurable materials 
    ''' (UVCBs, substances of unknown or variable composition, complex reaction products, 
    ''' or biological origin). CAS RNs are generally serial numbers (with a check digit), 
    ''' so they do not contain any information about the structures themselves the way 
    ''' SMILES and InChI strings do.
    ''' 
    ''' The registry maintained by CAS Is an authoritative collection Of disclosed chemical 
    ''' substance information. It identifies more than 204 million unique organic And inorganic 
    ''' substances And 70 million protein And DNA sequences, plus additional information about 
    ''' Each substance. It Is updated With around 15,000 additional New substances daily.
    ''' A collection Of almost 500 thousand CAS registry numbers are made available under a 
    ''' CC BY-NC license at ACS Commons Chemistry.</param>
    ''' <param name="InChIkey">The International Chemical Identifier (InChI /ˈɪntʃiː/ IN-chee 
    ''' or /ˈɪŋkiː/ ING-kee) is a textual identifier for chemical substances, designed to
    ''' provide a standard way to encode molecular information and to facilitate the search 
    ''' for such information in databases and on the web. Initially developed by the International
    ''' Union of Pure and Applied Chemistry (IUPAC) and National Institute of Standards and 
    ''' Technology (NIST) from 2000 to 2005, the format and algorithms are non-proprietary. 
    ''' Since May 2009, it has been developed by the InChI Trust, a nonprofit charity from 
    ''' the United Kingdom which works to implement and promote the use of InChI.</param>
    ''' <param name="InChI">The International Chemical Identifier (InChI /ˈɪntʃiː/ IN-chee 
    ''' or /ˈɪŋkiː/ ING-kee) is a textual identifier for chemical substances, designed to
    ''' provide a standard way to encode molecular information and to facilitate the search 
    ''' for such information in databases and on the web. Initially developed by the International
    ''' Union of Pure and Applied Chemistry (IUPAC) and National Institute of Standards and 
    ''' Technology (NIST) from 2000 to 2005, the format and algorithms are non-proprietary. 
    ''' Since May 2009, it has been developed by the InChI Trust, a nonprofit charity from 
    ''' the United Kingdom which works to implement and promote the use of InChI.</param>
    ''' <param name="SMILES">The simplified molecular-input line-entry system (SMILES) is 
    ''' a specification in the form of a line notation for describing the structure of 
    ''' chemical species using short ASCII strings. SMILES strings can be imported by 
    ''' most molecule editors for conversion back into two-dimensional drawings or three-dimensional 
    ''' models of the molecules.
    ''' 
    ''' The original SMILES specification was initiated In the 1980S. It has since been 
    ''' modified And extended. In 2007, an open standard called OpenSMILES was developed 
    ''' In the open source chemistry community.</param>
    ''' <param name="chemspider">ChemSpider is a freely accessible online database of 
    ''' chemicals owned by the Royal Society of Chemistry. It contains information on more than 100 
    ''' million molecules from over 270 data sources, each of them receiving a unique identifier 
    ''' called ChemSpider Identifier.</param>
    ''' <param name="foodb">FooDB (The Food Database) is a freely available, open-access 
    ''' database containing chemical (micronutrient and macronutrient) composition data on 
    ''' common, unprocessed foods. It also contains extensive data on flavour and aroma 
    ''' constituents, food additives as well as positive and negative health effects 
    ''' associated with food constituents. The database contains information on more than
    ''' 28,000 chemicals found in more than 1000 raw or unprocessed food products. The 
    ''' data in FooDB was collected from many sources including textbooks, scientific journals, 
    ''' on-line food composition or nutrient databases, flavour and aroma databases 
    ''' and various on-line metabolomic databases. This literature-derived information has
    ''' been combined with experimentally derived data measured on thousands of compounds 
    ''' from more than 40 very common food products through the Alberta Food Metabolome 
    ''' Project which is led by David S. Wishart. Users are able to browse through the FooDB 
    ''' data by food source, name, descriptors or function. Chemical structures and molecular 
    ''' weights for compounds in FooDB may be searched via a specialized chemical structure 
    ''' search utility. Users are able to view the content of FooDB using two different 
    ''' “Viewing” options: FoodView, which lists foods by their chemical compounds, or ChemView,
    ''' which lists chemicals by their food sources. Knowledge about the precise chemical 
    ''' composition of foods can be used to guide public health policies, assist food companies 
    ''' with improved food labelling, help dieticians prepare better dietary plans, 
    ''' support nutraceutical companies with their submissions of health claims and guide 
    ''' consumer choices with regard to food purchases.</param>
    ''' <param name="KEGGdrug">KEGG DRUG is a comprehensive drug information resource for 
    ''' approved drugs in Japan, USA and Europe, unified based on the chemical structure 
    ''' and/or the chemical component of active ingredients. Each KEGG DRUG entry is 
    ''' identified by the D number and associated with KEGG original annotations including 
    ''' therapeutic targets, drug metabolism, and other molecular interaction network 
    ''' information.</param>
    ''' <returns></returns>
    ''' <example>
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
    ''' </example>
    <ExportAPI("xref")>
    <RApiReturn(GetType(xref))>
    Public Function xref(<RRawVectorArgument> Optional chebi As Object = Nothing,
                         <RRawVectorArgument> Optional KEGG As Object = Nothing,
                         <RRawVectorArgument> Optional KEGGdrug As Object = Nothing,
                         <RRawVectorArgument> Optional pubchem As Object = Nothing,
                         <RRawVectorArgument> Optional HMDB As Object = Nothing,
                         <RRawVectorArgument> Optional metlin As Object = Nothing,
                         <RRawVectorArgument> Optional DrugBank As Object = Nothing,
                         <RRawVectorArgument> Optional ChEMBL As Object = Nothing,
                         <RRawVectorArgument> Optional chemspider As Object = Nothing,
                         <RRawVectorArgument> Optional foodb As Object = Nothing,
                         <RRawVectorArgument> Optional Wikipedia As Object = Nothing,
                         <RRawVectorArgument> Optional lipidmaps As Object = Nothing,
                         <RRawVectorArgument> Optional MeSH As Object = Nothing,
                         <RRawVectorArgument> Optional ChemIDplus As Object = Nothing,
                         <RRawVectorArgument> Optional MetaCyc As Object = Nothing,
                         <RRawVectorArgument> Optional KNApSAcK As Object = Nothing,
                         <RRawVectorArgument> Optional CAS As Object = Nothing,
                         <RRawVectorArgument> Optional InChIkey As Object = Nothing,
                         <RRawVectorArgument> Optional InChI As Object = Nothing,
                         <RRawVectorArgument> Optional SMILES As Object = Nothing,
                         <RListObjectArgument>
                         Optional extras As list = Nothing,
                         Optional env As Environment = Nothing) As xref

        Dim keggSet As String() = CLRVector.asCharacter(KEGG).SafeQuery.ToArray
        Dim kegg_id As String = keggSet.Where(Function(id) id.IsPattern("C\d+")).JoinBy("; ").ToArray
        Dim kegg_drug As String = keggSet _
            .Where(Function(id) id.IsPattern("D\d+")) _
            .JoinIterates(CLRVector.asCharacter(KEGGdrug)) _
            .Distinct _
            .JoinBy("; ")
        Dim additionals As Dictionary(Of String, String()) = extras _
            .AsGeneric(Of String())(env) _
            .Select(Function(a)
                        Return (
                            a.Key,
                            a.Value.SafeQuery _
                                .Where(Function(si) Not si.StringEmpty(testEmptyFactor:=True)) _
                                .ToArray
                        )
                    End Function) _
            .Where(Function(a)
                       Return Not a.ToArray.IsNullOrEmpty
                   End Function) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)

        Return New xref With {
            .CAS = CLRVector.asCharacter(CAS),
            .chebi = CLRVector.asCharacter(chebi).JoinBy("; "),
            .ChEMBL = CLRVector.asCharacter(ChEMBL).JoinBy("; "),
            .ChemIDplus = CLRVector.asCharacter(ChemIDplus).JoinBy("; "),
            .DrugBank = CLRVector.asCharacter(DrugBank).JoinBy("; "),
            .HMDB = CLRVector.asCharacter(HMDB).JoinBy("; "),
            .InChI = CLRVector.asCharacter(InChI).JoinBy("; "),
            .InChIkey = CLRVector.asCharacter(InChIkey).JoinBy("; "),
            .KEGG = kegg_id,
            .KNApSAcK = CLRVector.asCharacter(KNApSAcK).JoinBy("; "),
            .lipidmaps = CLRVector.asCharacter(lipidmaps).JoinBy("; "),
            .MeSH = CLRVector.asCharacter(MeSH).JoinBy("; "),
            .MetaCyc = CLRVector.asCharacter(MetaCyc).JoinBy("; "),
            .metlin = CLRVector.asCharacter(metlin).JoinBy("; "),
            .pubchem = CLRVector.asCharacter(pubchem).JoinBy("; "),
            .SMILES = CLRVector.asCharacter(SMILES).JoinBy("; "),
            .Wikipedia = CLRVector.asCharacter(Wikipedia).JoinBy("; "),
            .chemspider = CLRVector.asCharacter(chemspider).JoinBy("; "),
            .foodb = CLRVector.asCharacter(foodb).JoinBy("; "),
            .KEGGdrug = kegg_drug,
            .extras = additionals
        }
    End Function

    ''' <summary>
    ''' Create spectrum reference library from mona msp file
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("library_from_mona")>
    <RApiReturn(GetType(Library(Of MetaLib)))>
    Public Function MakeMoNALibrary(<RRawVectorArgument>
                                    mona As Object,
                                    Optional libtype As IonModes = IonModes.Positive,
                                    Optional tqdm_verbose As Boolean = True,
                                    Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of SpectraSection)(mona, env)
        Dim pullSpec As SpectraSection()

        If pull.isError Then
            Return pull.getError
        Else
            pullSpec = pull.populates(Of SpectraSection)(env) _
                .Where(Function(a) a.libtype = libtype) _
                .ToArray
        End If

        Dim fetch As Func(Of IEnumerable(Of (MetaLib, PeakMs2))) =
            Iterator Function() As IEnumerable(Of (MetaLib, PeakMs2))
                Dim source As IEnumerable(Of SpectraSection)

                If tqdm_verbose Then
                    source = TqdmWrapper.Wrap(pullSpec)
                Else
                    source = pullSpec
                End If

                For Each ref As SpectraSection In source
                    Yield (ref.GetMetabolite, ref.GetSpectrumPeaks)
                Next
            End Function
        Dim libs As New Library(Of MetaLib)(fetch())

        Return libs
    End Function

    ''' <summary>
    ''' Construct of the local reference library for gcms data
    ''' </summary>
    ''' <param name="metadb"></param>
    ''' <param name="refSpec"></param>
    ''' <returns></returns>
    <ExportAPI("load_local")>
    <RApiReturn(GetType(Library(Of MetaLib)))>
    Public Function LoadLocalDatabase(metadb As IMetaDb, refSpec As PackAlignment, Optional tqdm_verbose As Boolean = True) As Object
        Return New Library(Of MetaLib)(metadb, refSpec.GetReferenceSpectrum(tqdm_verbose))
    End Function

    ''' <summary>
    ''' get metabolite annotation metadata via given reference id
    ''' </summary>
    ''' <param name="libs">the annotation data library model</param>
    ''' <param name="id">
    ''' a set of the compound reference id for get the metadata from the library.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("load_metadata")>
    <RApiReturn(GetType(MetaLib))>
    Public Function GetAnnotations(libs As Library(Of MetaLib), <RRawVectorArgument> id As Object) As Object
        Return CLRVector.asCharacter(id) _
            .SafeQuery _
            .Select(Function(refId) libs.GetMetadataByID(refId)) _
            .ToArray
    End Function

    ''' <summary>
    ''' Check of the valid adducts
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <param name="adducts"></param>
    ''' <param name="ion_mode"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("assert.adducts")>
    <RApiReturn(GetType(MzCalculator))>
    Public Function assertAdducts(formula As String,
                                  <RRawVectorArgument>
                                  adducts As Object,
                                  Optional ion_mode As Object = "+",
                                  Optional env As Environment = Nothing) As Object

        Static asserts As New Dictionary(Of IonModes, AdductsRanking) From {
            {IonModes.Positive, New AdductsRanking(IonModes.Positive)},
            {IonModes.Negative, New AdductsRanking(IonModes.Negative)}
        }

        Dim ionVal = Math.GetIonMode(ion_mode, env)
        Dim ruler As AdductsRanking = asserts(ionVal)
        Dim precursors As MzCalculator() = Math.GetPrecursorTypes(adducts, env)

        Return ruler.RankAdducts(formula, precursors).ToArray
    End Function

    ''' <summary>
    ''' a shortcut method for populate the peak ms2 data from a mzpack raw data file
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("populateIonData")>
    <Extension>
    <RApiReturn(GetType(PeakMs2))>
    Public Function PopulateIonData(raw As mzPack,
                                    Optional mzdiff As Object = "da:0.3",
                                    Optional env As Environment = Nothing) As Object

        Dim tolerance = Math.getTolerance(mzdiff, env)

        If tolerance Like GetType(Message) Then
            Return tolerance.TryCast(Of Message)
        End If

        Dim mzErr As Tolerance = tolerance.TryCast(Of Tolerance)
        Dim ions As New List(Of PeakMs2)

        For Each Ms1 As ScanMS1 In raw.MS
            For Each ms2 In Ms1.products.SafeQuery
                For Each mzi As Double In Ms1.mz
                    If mzErr(mzi, ms2.parentMz) Then
                        Dim ion2 As New PeakMs2 With {
                            .mz = mzi,
                            .rt = Ms1.rt,
                            .file = raw.source,
                            .lib_guid = ms2.scan_id,
                            .activation = ms2.activationMethod.Description,
                            .collisionEnergy = ms2.collisionEnergy,
                            .intensity = ms2.intensity,
                            .scan = ms2.scan_id,
                            .mzInto = ms2.GetMs.ToArray
                        }

                        Call ions.Add(ion2)
                    End If
                Next
            Next
        Next

        Return ions.ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="candidates">
    ''' should be a collection of the <see cref="AnnotationData(Of xref)"/> object
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("unique_candidates")>
    Public Function uniqueAnnotations(<RRawVectorArgument> candidates As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of AnnotationData(Of xref))(candidates, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim candidateList As AnnotationData(Of xref)() = pull.populates(Of AnnotationData(Of xref))(env).ToArray
        Dim unique = CrossReferenceData.UniqueGroups(Of xref, AnnotationData(Of xref))(candidateList).ToArray
        Dim list As New list

        For Each i As NamedCollection(Of AnnotationData(Of xref)) In unique
            Call list.add(i.name, i.value)
        Next

        Return list
    End Function

    ''' <summary>
    ''' create a new metabolite annotation information
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="formula"></param>
    ''' <param name="name"></param>
    ''' <param name="synonym"></param>
    ''' <param name="xref"></param>
    ''' <returns></returns>
    <ExportAPI("make.annotation")>
    Public Function createAnnotation(id As String,
                                     formula As String,
                                     name As String,
                                     Optional synonym As String() = Nothing,
                                     Optional xref As xref = Nothing) As MetaData

        Return New MetaData With {
            .xref = If(xref, New xref),
            .formula = formula,
            .ID = id,
            .name = name,
            .synonym = synonym,
            .exact_mass = CDbl(FormulaScanner.ScanFormula(formula))
        }
    End Function

    <Extension>
    Private Function ionsFromPeaktable(df As dataframe, env As Environment) As [Variant](Of Message, xcms2())
        Dim id As String()
        Dim println = env.WriteLineHandler

        Call println("get data frame object for the ms1 ions features(with data fields):")
        Call println(df.colnames)

        If Not df.rownames Is Nothing Then
            id = df.rownames
        ElseIf df.hasName("xcms_id") Then
            id = CLRVector.asCharacter(df("xcms_id"))
        ElseIf df.hasName("ID") Then
            id = CLRVector.asCharacter(df("ID"))
        Else
            Return RInternal.debug.stop({
                "missing the unique id of the ms1 ions in your dataframe!",
                "required_one_of_field: xcms_id, ID"
            }, env)
        End If

        Dim mz As Double() = CLRVector.asNumeric(df("mz"))
        Dim rt As Double() = CLRVector.asNumeric(df("rt"))

        Call println("get ms1 features unique id collection:")
        Call println(id)

        Return id _
            .Select(Function(xcms_id, i)
                        Return New xcms2 With {
                            .ID = xcms_id,
                            .mz = mz(i),
                            .rt = rt(i)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Check the ms1 parent ion is generated via the in-source fragment or not
    ''' </summary>
    ''' <param name="ms1">
    ''' the ms1 peaktable dataset, it could be a xcms peaktable object dataframe, 
    ''' a collection of ms1 scan with unique id tagged.
    ''' </param>
    ''' <param name="ms2">
    ''' the ms2 products list
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a tuple key-value pair list object that contains the flags for each ms1 ion
    ''' corresponding slot value TRUE means the key ion is a possible in-source
    ''' fragment ion data, otherwise slot value FALSE means not. 
    ''' </returns>
    ''' 
    <ExportAPI("checkInSourceFragments")>
    <RApiReturn(GetType(Boolean))>
    Public Function checkInSourceFragments(<RRawVectorArgument> ms1 As Object,
                                           <RRawVectorArgument> ms2 As Object,
                                           Optional da As Double = 0.1,
                                           Optional rt_win As Double = 5,
                                           Optional env As Environment = Nothing) As Object

        Dim xcmsPeaks As xcms2()
        Dim println = env.WriteLineHandler

        If ms1 Is Nothing Then
            Return RInternal.debug.stop("the ms1 peakdata should not be nothing!", env)
        ElseIf ms2 Is Nothing Then
            Return RInternal.debug.stop("the ms2 spectrum data should not be nothing!", env)
        End If

        If TypeOf ms1 Is dataframe Then
            Dim pull = DirectCast(ms1, dataframe).ionsFromPeaktable(env)

            If pull Like GetType(Message) Then
                Return pull.TryCast(Of Message)
            Else
                xcmsPeaks = pull.TryCast(Of xcms2())
            End If
        Else
            Dim ms1data = pipeline.TryCreatePipeline(Of xcms2)(ms1, env)

            If ms1data.isError Then
                Return ms1data.getError
            End If

            xcmsPeaks = ms1data _
                .populates(Of xcms2)(env) _
                .ToArray
        End If

        Dim ms2Products As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ms2, env)

        If ms2Products.isError Then
            Return ms2Products.getError
        End If

        Dim check As New CheckInSourceFragments(ms2Products.populates(Of PeakMs2)(env), da)
        Dim flags As New list With {.slots = New Dictionary(Of String, Object)}

        For Each ms1_ion As xcms2 In xcmsPeaks
            Call flags.add(
                name:=ms1_ion.ID,
                value:=check.CheckOfFragments(ms1_ion.mz, ms1_ion.rt, rt_win)
            )
        Next

        Return flags
    End Function

    ''' <summary>
    ''' open the annotation database file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' 
    ''' </returns>
    ''' 
    <ExportAPI("open_repository")>
    <RApiReturn(GetType(LocalRepository), GetType(RepositoryWriter))>
    Public Function openRepository(<RRawVectorArgument> file As Object,
                                   <RRawVectorArgument(TypeCodes.string)>
                                   Optional mode As Object = "read|write",
                                   Optional mapping As list = Nothing,
                                   Optional env As Environment = Nothing) As Object

        Dim modes As String() = CLRVector.asCharacter(mode)
        Dim is_filepath As Boolean

        If modes.IsNullOrEmpty Then
            Return RInternal.debug.stop("the data io mode for the local annotation metadata repository should not be nothing!", env)
        End If

        Dim access As FileAccess = If(modes(0).TextEquals("read"), FileAccess.Read, FileAccess.Write)
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, access, env, is_filepath:=is_filepath)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        ElseIf is_filepath Then
            Call VBDebugger.EchoLine($"open the local annotation database file: {CLRVector.asCharacter(file).First}")
        End If

        If access = FileAccess.Read Then
            Dim libs As New LocalRepository(buf.TryCast(Of Stream))

            If Not mapping Is Nothing AndAlso mapping.length > 0 Then
                Call libs.SetIdMapping(mapping.slots.AsCharacter)
            End If

            Return libs
        Else
            Return New RepositoryWriter(buf.TryCast(Of Stream))
        End If
    End Function

    <ExportAPI("write_metadata")>
    Public Function Save(writer As RepositoryWriter, <RRawVectorArgument> meta As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of Models.MetaInfo)(meta, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim alldata As Models.MetaInfo() = pull.populates(Of Models.MetaInfo)(env).ToArray

        For Each m As Models.MetaInfo In TqdmWrapper.Wrap(alldata)
            Call writer.Add(m)
        Next

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="io">Read or Write</param>
    ''' <param name="meta_allocated">
    ''' please increase this pre-allocation size if there are too many samples data files, default is 32MB pre-allocated size.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("open.annotation_workspace")>
    <RApiReturn(GetType(AnnotationWorkspace))>
    Public Function OpenResultPack(<RRawVectorArgument> file As Object,
                                   Optional io As FileAccess = FileAccess.Read,
                                   Optional lazy As Boolean = False,
                                   Optional meta_allocated As Long = 32 * ByteSize.MB,
                                   Optional env As Environment = Nothing) As Object

        Dim is_filepath As Boolean = False
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, io, env, lazy:=lazy, is_filepath:=is_filepath)
        Dim path As String = Nothing

        If is_filepath Then
            path = CLRVector.asCharacter(file).First
        End If
        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Return New AnnotationWorkspace(
            file:=buf.TryCast(Of Stream),
            source_file:=path,
            meta_allocated:=meta_allocated
        )
    End Function

    <ExportAPI("read.annotationPack")>
    <RApiReturn(GetType(AnnotationPack))>
    Public Function readResultPack(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim workspace As Object = OpenResultPack(file, FileAccess.Read, lazy:=False, , env)

        If TypeOf workspace Is Message Then
            Return workspace
        End If

        ' no needs for dispose of the memory data
        Return DirectCast(workspace, AnnotationWorkspace).LoadMemory
    End Function

    <ExportAPI("filter")>
    Public Function filter_unique(pack As AnnotationPack, <RRawVectorArgument> filter As Object) As AnnotationPack
        Dim filterIndex As Index(Of String) = CLRVector.asCharacter(filter).Indexing
        Dim libs = pack.libraries

        For Each key As String In libs.Keys.ToArray
            libs(key) = libs(key) _
                .Where(Function(ai)
                           Return $"{ai.xcms_id}_{ai.biodeep_id}_{ai.adducts}" Like filterIndex
                       End Function) _
                .ToArray
        Next

        pack.libraries = libs

        Return pack
    End Function

    ''' <summary>
    ''' get annotated ms1 peak features from the result data pack
    ''' </summary>
    ''' <param name="workspace"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("get_peaktable")>
    <RApiReturn(GetType(PeakSet))>
    Public Function loadPeaktable(workspace As Object, Optional env As Environment = Nothing) As Object
        If workspace Is Nothing Then
            Return Nothing
        End If

        If TypeOf workspace Is AnnotationWorkspace Then
            Return DirectCast(workspace, AnnotationWorkspace).LoadMemory.CreatePeakSet
        ElseIf TypeOf workspace Is AnnotationPack Then
            Return DirectCast(workspace, AnnotationPack).CreatePeakSet
        Else
            Return Message.InCompatibleType(GetType(AnnotationWorkspace), workspace.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' get annotation data from the given workspace object
    ''' </summary>
    ''' <param name="workspace"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("get_annotations")>
    <RApiReturn(GetType(AnnotationPack), GetType(Peaktable))>
    Public Function loadAll(workspace As Object, Optional env As Environment = Nothing) As Object
        If workspace Is Nothing Then
            Return Nothing
        End If

        If TypeOf workspace Is AnnotationWorkspace Then
            Return DirectCast(workspace, AnnotationWorkspace).LoadMemory
        ElseIf TypeOf workspace Is AnnotationPack Then
            Return DirectCast(workspace, AnnotationPack).GetAnnotation.ToArray
        Else
            Return Message.InCompatibleType(GetType(AnnotationWorkspace), workspace.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' Set the XIC cache data into the report viewer workspace
    ''' </summary>
    ''' <param name="workspace"></param>
    ''' <param name="raw_set">should be a set of the mzpack raw data objects, or a character 
    ''' vector of the file path to the mzpack rawdata files.
    ''' </param>
    ''' <param name="da"></param>
    ''' <param name="rt_win"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("set_xicCache")>
    Public Function set_xicCache(workspace As AnnotationWorkspace, <RRawVectorArgument> raw_set As Object,
                                 Optional da As Double = 0.25,
                                 Optional rt_win As Double = 7.5,
                                 Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of mzPack)(raw_set, env)

        If raw_set Is Nothing Then
            Return RInternal.debug.stop("no rawdata was provided for extract the ion XIC data!", env)
        End If

        If pull.isError Then
            If TypeOf raw_set Is list Then
                raw_set = DirectCast(raw_set, list).data.ToArray
            End If
            If TypeOf raw_set Is vector Then
                raw_set = DirectCast(raw_set, vector).data
            End If

            If TypeOf raw_set Is String Then
                raw_set = {CStr(raw_set)}
            End If

            raw_set = TryCastGenericArray(raw_set, env)

            If DataFramework.IsCollection(Of String)(raw_set.GetType) Then
                ' read ms1 from files
                Dim reader As Func(Of IEnumerable(Of mzPack)) =
                    Iterator Function() As IEnumerable(Of mzPack)
                        For Each path As String In CLRVector.asCharacter(raw_set)
                            Yield mzPack.Read(path, ignoreThumbnail:=True, skipMsn:=True, verbose:=False)
                        Next
                    End Function

                pull = pipeline.CreateFromPopulator(reader())
            Else
                Return pull.getError
            End If
        End If

        Call workspace.CacheXicTable(pull.populates(Of mzPack)(env), da, rt_win)

        Return True
    End Function

    ''' <summary>
    ''' Save the reference library annotation result.
    ''' </summary>
    ''' <param name="workspace"></param>
    ''' <param name="library">
    ''' the reference library name of the spectrum reference data
    ''' </param>
    ''' <param name="annotations">
    ''' A temp workspace of a single reference library.
    ''' </param>
    <ExportAPI("save_annotations")>
    Public Sub saveAnnotation(workspace As AnnotationWorkspace, library As String, annotations As LibraryWorkspace)
        Call workspace.CreateLibraryResult(library, annotations.GetAnnotations(filterPeaks:=True))
    End Sub

    ''' <summary>
    ''' Save the ms2 alignment hits result into current temp workspace.
    ''' </summary>
    ''' <param name="workspace"></param>
    ''' <param name="mz"></param>
    ''' <param name="rt"></param>
    ''' <param name="intensity"></param>
    ''' <param name="libname"></param>
    ''' <param name="score"></param>
    ''' <param name="forward"></param>
    ''' <param name="reverse"></param>
    ''' <param name="jaccard"></param>
    ''' <param name="entropy"></param>
    ''' <param name="source"></param>
    ''' <param name="alignment">
    ''' the ms2 spectrum alignment matrix in linear string format
    ''' </param>
    <ExportAPI("push_temp")>
    Public Sub push_temp(workspace As LibraryWorkspace,
                         mz As Double, rt As Double, intensity As Double,
                         libname As String,
                         score As Double,
                         forward As Double, reverse As Double, jaccard As Double, entropy As Double,
                         source As String,
                         alignment As String)

        Dim ms2 As SSM2MatrixFragment() = AlignmentOutput.ParseAlignmentLinearMatrix(alignment).ToArray
        Dim align As New Ms2Score With {
            .ms2 = ms2,
            .entropy = entropy,
            .forward = forward,
            .intensity = intensity,
            .jaccard = jaccard,
            .libname = libname,
            .precursor = mz,
            .reverse = reverse,
            .rt = rt,
            .score = score,
            .source = source
        }

        Call workspace.add(align)
    End Sub

    ''' <summary>
    ''' Commit library annotation
    ''' </summary>
    ''' <param name="workspace"></param>
    ''' <param name="xref_id"></param>
    <ExportAPI("commit")>
    Public Sub commit(workspace As LibraryWorkspace, xref_id As String,
                      mz As Double, rt As Double,
                      adducts As String,
                      biodeep_id As String, name As String, formula As String)

        Call workspace.commit(xref_id, New AlignmentHit With {
            .theoretical_mz = mz,
            .mz = mz,
            .rt = rt,
            .RI = 0,
            .adducts = adducts,
            .biodeep_id = biodeep_id,
            .formula = formula,
            .name = name,
            .exact_mass = FormulaScanner.EvaluateExactMass(.formula),
            .libname = xref_id.Split("|"c).First,
            .xcms_id = Nothing
        })
    End Sub

    ''' <summary>
    ''' create an empty workspace object
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("workspace")>
    Public Function create_workspace() As LibraryWorkspace
        Return New LibraryWorkspace
    End Function

    ''' <summary>
    ''' Associates the ms1 peaks with the ms2 spectrum alignment result hits.
    ''' </summary>
    ''' <param name="works">the workspace object, which could be constructed via the ``workspace`` function.</param>
    ''' <param name="libname">the reference key to the ms2 spectrum alignment result.</param>
    ''' <param name="adducts"></param>
    ''' <param name="xcms_id"></param>
    ''' <param name="mz"></param>
    ''' <param name="rt"></param>
    ''' <param name="RI"></param>
    ''' <param name="npeaks"></param>
    ''' <remarks>
    ''' a ms2 spectrum alignment result should be existed inside the workspace before assign the ms1 peaks to the result.
    ''' </remarks>
    <ExportAPI("peak_assign")>
    Public Sub peak_assign(works As LibraryWorkspace,
                           libname As String, adducts As String,
                           xcms_id As String, mz As Double, rt As Double, RI As Double, npeaks As Integer)

        Dim db_xref As String = $"{libname}|{adducts}|{CInt(mz)}"
        Dim peak As New xcms2 With {
            .ID = xcms_id,
            .mz = mz,
            .rt = rt,
            .RI = RI
        }

        Call works.commit(db_xref, peak, npeaks)
    End Sub
End Module
