// export R# package module type define for javascript/typescript language
//
//    imports "pubchem_kit" from "mzkit";
//
// ref=mzkit.PubChemToolKit@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace pubchem_kit {
   /**
     * @param cache default value Is ``'./.pubchem'``.
     * @param offline default value Is ``false``.
     * @param interval default value Is ``-1``.
     * @param env default value Is ``null``.
   */
   function CID(name: string, cache?: any, offline?: boolean, interval?: object, env?: object): string;
   /**
     * @param size default value Is ``'500,500'``.
     * @param ignores_invalid_CID default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function image_fly(cid: any, size?: any, ignores_invalid_CID?: boolean, env?: object): any;
   /**
     * @param clusters default value Is ``null``.
   */
   function mesh_background(mesh: object, clusters?: object): object;
   /**
   */
   function mesh_level1(mesh: object): string;
   module metadata {
      /**
      */
      function pugView(pugView: object): object;
   }
   /**
   */
   function pubchem_url(cid: string): string;
   /**
     * @param cacheFolder default value Is ``'./pubchem_cache'``.
     * @param offline default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function pugView(cid: any, cacheFolder?: string, offline?: boolean, env?: object): any;
   module query {
      /**
        * @param cache default value Is ``'./pubchem/'``.
        * @param interval default value Is ``-1``.
        * @param env default value Is ``null``.
      */
      function external(cid: string, cache?: any, interval?: object, env?: object): object;
      /**
        * @param cache default value Is ``'./graph_kb'``.
      */
      function knowlegde_graph(cid: string, cache?: string): object;
   }
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function mesh_tree(file: any, env?: object): any;
      /**
        * @param lazy default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function pubmed(file: string, lazy?: boolean, env?: object): object;
      /**
      */
      function pugView(file: string): object;
      /**
      */
      function webquery(file: string): object;
   }
   /**
     * @param skipNoCID default value Is ``true``.
     * @param dbfilter default value Is ``null``.
   */
   function SID_map(sidMapText: string, skipNoCID?: boolean, dbfilter?: string): object;
}
