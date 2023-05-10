// export R# package module type define for javascript/typescript language
//
// ref=mzkit.PubChemToolKit@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace pubchem_kit {
   module read {
      /**
        * @param lazy default value Is ``true``.
        * @param env default value Is ``null``.
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
        * @param env default value Is ``null``.
      */
      function mesh_tree(file: any, env?: object): any;
   }
   /**
     * @param size default value Is ``'500,500'``.
     * @param ignoresInvalidCid default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function image_fly(cid: any, size?: any, ignoresInvalidCid?: boolean, env?: object): any;
   module query {
      /**
       * query of the pathways, taxonomy and reaction 
       *  data from the pubchem database.
       * 
       * 
        * @param cid -
        * @param cache -
        * 
        * + default value Is ``'./pubchem/'``.
        * @param interval the sleep time interval in ms
        * 
        * + default value Is ``-1``.
      */
      function external(cid: string, cache?: string, interval?: object): object;
      /**
        * @param cache default value Is ``'./graph_kb'``.
      */
      function knowlegde_graph(cid: string, cache?: string): object;
   }
   /**
    * query cid from pubchem database
    * 
    * 
     * @param name -
     * @param cache -
     * 
     * + default value Is ``'./.pubchem'``.
     * @param offline -
     * 
     * + default value Is ``false``.
     * @param interval the time sleep interval in ms
     * 
     * + default value Is ``-1``.
   */
   function CID(name: string, cache?: string, offline?: boolean, interval?: object): string;
   /**
   */
   function pubchem_url(cid: string): string;
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
   */
   function pugView(cid: any, cacheFolder?: string, offline?: boolean, env?: object): any;
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
   */
   function SID_map(sidMapText: string, skipNoCID?: boolean, dbfilter?: string): object;
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
   */
   function mesh_level1(mesh: object): string;
}
