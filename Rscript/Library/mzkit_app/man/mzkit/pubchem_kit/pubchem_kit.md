# pubchem_kit

toolkit for handling of the ncbi pubchem data
> PubChem is a database of chemical molecules and their activities against biological assays. 
>  The system is maintained by the National Center for Biotechnology Information (NCBI), a 
>  component of the National Library of Medicine, which is part of the United States National 
>  Institutes of Health (NIH). PubChem can be accessed for free through a web user interface. 
>  Millions of compound structures and descriptive datasets can be freely downloaded via FTP. 
>  PubChem contains multiple substance descriptions and small molecules with fewer than 100 
>  atoms and 1,000 bonds. More than 80 database vendors contribute to the growing PubChem 
>  database.
>  
>  ##### History
>  PubChem was released In 2004 As a component Of the Molecular Libraries Program (MLP) Of the
>  NIH. As Of November 2015, PubChem contains more than 150 million depositor-provided substance 
>  descriptions, 60 million unique chemical structures, And 225 million biological activity test 
>  results (from over 1 million assay experiments performed On more than 2 million small-molecules 
>  covering almost 10,000 unique protein target sequences that correspond To more than 5,000 genes).
>  It also contains RNA interference (RNAi) screening assays that target over 15,000 genes.
>  
>  As of August 2018, PubChem contains 247.3 million substance descriptions, 96.5 million unique 
>  chemical structures, contributed by 629 data sources from 40 countries. It also contains 237 
>  million bioactivity test results from 1.25 million biological assays, covering >10,000 target 
>  protein sequences.
> 
>  As of 2020, with data integration from over 100 New sources, PubChem contains more than 293 
>  million depositor-provided substance descriptions, 111 million unique chemical structures,
>  And 271 million bioactivity data points from 1.2 million biological assays experiments.

+ [read.pubmed](pubchem_kit/read.pubmed.1) read pubmed data table files
+ [image_fly](pubchem_kit/image_fly.1) Request the metabolite structure image via the pubchem image_fly api
+ [query.external](pubchem_kit/query.external.1) query of the pathways, taxonomy and reaction 
+ [CID](pubchem_kit/CID.1) query cid from pubchem database
+ [pubchem_url](pubchem_kit/pubchem_url.1) Generate the url for get pubchem pugviews data object
+ [query.knowlegde_graph](pubchem_kit/query.knowlegde_graph.1) Query the compound related biological context information from pubchem
+ [query](pubchem_kit/query.1) query of the pubchem database
+ [pugView](pubchem_kit/pugView.1) query pubchem data via a given cid value
+ [SID_map](pubchem_kit/SID_map.1) parse the pubchem sid map data file
+ [read.pugView](pubchem_kit/read.pugView.1) read xml text and then parse as pugview record data object
+ [read.webquery](pubchem_kit/read.webquery.1) read the pubchem webquery summary xml file
+ [metadata.pugView](pubchem_kit/metadata.pugView.1) extract the compound annotation data
+ [read.mesh_tree](pubchem_kit/read.mesh_tree.1) Parse the mesh ontology tree
+ [mesh_background](pubchem_kit/mesh_background.1) create MeSH ontology gsea background based on the mesh tree
+ [mesh_level1](pubchem_kit/mesh_level1.1) gets the level1 term label of the mesh tree
