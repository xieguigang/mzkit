// export R# package module type define for javascript/typescript language
//
//    imports "pubchem_kit" from "mzkit";
//
// ref=mzkit.PubChemToolKit@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * toolkit for handling of the ncbi pubchem data
 * 
 * > PubChem is a database of chemical molecules and their activities against biological assays. 
 * >  The system is maintained by the National Center for Biotechnology Information (NCBI), a 
 * >  component of the National Library of Medicine, which is part of the United States National 
 * >  Institutes of Health (NIH). PubChem can be accessed for free through a web user interface. 
 * >  Millions of compound structures and descriptive datasets can be freely downloaded via FTP. 
 * >  PubChem contains multiple substance descriptions and small molecules with fewer than 100 
 * >  atoms and 1,000 bonds. More than 80 database vendors contribute to the growing PubChem 
 * >  database.
 * >  
 * >  ##### History
 * >  PubChem was released In 2004 As a component Of the Molecular Libraries Program (MLP) Of the
 * >  NIH. As Of November 2015, PubChem contains more than 150 million depositor-provided substance 
 * >  descriptions, 60 million unique chemical structures, And 225 million biological activity test 
 * >  results (from over 1 million assay experiments performed On more than 2 million small-molecules 
 * >  covering almost 10,000 unique protein target sequences that correspond To more than 5,000 genes).
 * >  It also contains RNA interference (RNAi) screening assays that target over 15,000 genes.
 * >  
 * >  As of August 2018, PubChem contains 247.3 million substance descriptions, 96.5 million unique 
 * >  chemical structures, contributed by 629 data sources from 40 countries. It also contains 237 
 * >  million bioactivity test results from 1.25 million biological assays, covering >10,000 target 
 * >  protein sequences.
 * > 
 * >  As of 2020, with data integration from over 100 New sources, PubChem contains more than 293 
 * >  million depositor-provided substance descriptions, 111 million unique chemical structures,
 * >  And 271 million bioactivity data points from 1.2 million biological assays experiments.
*/
declare namespace pubchem_kit {
   /**
    * query cid from pubchem database
    * 
    * 
     * @param name any search term for query the pubchem database
     * @param cache the cache fs for the online pubchem database
     * 
     * + default value Is ``'./.pubchem'``.
     * @param offline running the search query handler in offline mode?
     * 
     * + default value Is ``false``.
     * @param interval the time sleep interval in ms
     * 
     * + default value Is ``-1``.
     * @param env 
     * + default value Is ``null``.
     * @return A character vector of the pubchem cid that matches the given input ``name``.
   */
   function CID(name: string, cache?: any, offline?: boolean, interval?: object, env?: object): string;
   /**
    * Request the metabolite structure image via the pubchem image_fly api
    * 
    * 
     * @param cid A character vector of the pubchem cid for get the molecular 
     *  structure data image.
     * @param size -
     * 
     * + default value Is ``'500,500'``.
     * @param ignores_invalid_CID -
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A tuple list of the image data for the input pubchem metabolite cid query
   */
   function image_fly(cid: any, size?: any, ignores_invalid_CID?: boolean, env?: object): object;
   /**
    * create MeSH ontology gsea background based on the mesh tree
    * 
    * 
     * @param mesh -
     * @param clusters Create the mesh background about another topic
     * 
     * + default value Is ``null``.
   */
   function mesh_background(mesh: object, clusters?: object): object;
   /**
    * gets the level1 term label of the mesh tree
    * 
    * 
     * @param mesh -
     * @return A character vector of the ontology term label
   */
   function mesh_level1(mesh: object): string;
   module metadata {
      /**
       * extract the compound annotation data
       * 
       * 
        * @param pugView -
      */
      function pugView(pugView: object): object;
   }
   /**
    * Generate the url for get pubchem pugviews data object
    * 
    * 
     * @param cid The pubchem compound cid, should be an integer value
     * @param env 
     * + default value Is ``null``.
     * @return A url for get the pubchem data in pugview format
   */
   function pubchem_url(cid: any, env?: object): string;
   /**
    * query pubchem data via a given cid value
    * 
    * 
     * @param cid -
     * @param cacheFolder A cache directory path to the pubchem xml files
     * 
     * + default value Is ``'./pubchem_cache'``.
     * @param offline -
     * 
     * + default value Is ``false``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A collection of the pubchem pug view object that contains the metabolite annotation information.
   */
   function pugView(cid: any, cacheFolder?: string, offline?: boolean, env?: object): object;
   module query {
      /**
       * query of the pathways, taxonomy and reaction 
       *  data from the pubchem database.
       * 
       * 
        * @param cid -
        * @param cache A local dir path for the cache data or a filesystem wrapper object
        * 
        * + default value Is ``'./pubchem/'``.
        * @param interval the sleep time interval in ms
        * 
        * + default value Is ``-1``.
        * @param env 
        * + default value Is ``null``.
      */
      function external(cid: string, cache?: any, interval?: object, env?: object): object;
      /**
       * Query the compound related biological context information from pubchem
       * 
       * 
        * @param cid -
        * @param cache -
        * 
        * + default value Is ``'./graph_kb'``.
        * @return A tuple list of the knowledge data that associated with the given pubchem metabolite:
        *  
        *  1. genes: the co-occurance genes with the compound 
        *  2. disease: a list of the related disease with the compound
        *  3. compounds: the co-occurance compound data
        *  
        *  all of the slot data is a collection of the mzkit pubchem @``T:BioNovoGene.BioDeep.Chemistry.NCBI.PubChem.Graph.MeshGraph`` 
        *  clr object.
      */
      function knowlegde_graph(cid: string, cache?: string): object;
   }
   module read {
      /**
       * Parse the mesh ontology tree
       * 
       * 
        * @param file A text file data that contains the mesh ontology tree data
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mesh_tree(file: any, env?: object): object;
      /**
       * read pubmed data table files
       * 
       * 
        * @param file A collection of the pubmed database ascii text file
        * @param lazy just create a lazy loader instead of read all 
        *  content into memory at once?
        * 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function pubmed(file: string, lazy?: boolean, env?: object): object;
      /**
       * read xml text and then parse as pugview record data object
       * 
       * 
        * @param file the file path or the xml text content
      */
      function pugView(file: string): object;
      /**
       * read the pubchem webquery summary xml file
       * 
       * 
        * @param file the file path to the pubchem query search result file, the data file which
        *  could be downloaded from the query result url example like: 
        *  
        *  > https://pubchem.ncbi.nlm.nih.gov/sdq/sdqagent.cgi?infmt=json&outfmt=xml&query={%22download%22:%22*%22,%22collection%22:%22compound%22,%22order%22:[%22relevancescore,desc%22],%22start%22:1,%22limit%22:10000000,%22downloadfilename%22:%22PubChem_compound_text_kegg%22,%22where%22:{%22ands%22:[{%22*%22:%22kegg%22}]}}
        * @return A collection of the pubchem query summary download @``T:BioNovoGene.BioDeep.Chemistry.NCBI.PubChem.Web.QueryXml`` result file
      */
      function webquery(file: string): object;
   }
   /**
    * parse the pubchem sid map data file
    * 
    * 
     * @param sidMapText -
     * @param skipNoCID skip of the sid map item which has no cid assigned yet?
     * 
     * + default value Is ``true``.
     * @param dbfilter filter out the sid map data with a specific given db name
     * 
     * + default value Is ``null``.
     * @return A collection of the map data that could be used for get the
     *  knowledge base id mapping from external database, and map between the 
     *  pubchem sid and cid.
   */
   function SID_map(sidMapText: string, skipNoCID?: boolean, dbfilter?: string): object;
}
