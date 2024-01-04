// export R# package module type define for javascript/typescript language
//
//    imports "metadb" from "mzkit";
//
// ref=mzkit.MetaDbXref@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace metadb {
   module annotationStream {
      /**
        * @param env default value Is ``null``.
      */
      function compounds(compounds: any, env?: object): object;
   }
   module cbind {
      /**
        * @param env default value Is ``null``.
      */
      function metainfo(anno: object, engine: any, env?: object): any;
   }
   /**
     * @param includes_metal_ions default value Is ``false``.
     * @param excludes default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function excludeFeatures(query: object, id: string, field: string, metadb: object, includes_metal_ions?: boolean, excludes?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function getMetadata(engine: any, uniqueId: object, env?: object): any;
   module has {
      /**
        * @param env default value Is ``null``.
      */
      function metal_ion(formula: any, env?: object): any;
   }
   /**
     * @param env default value Is ``null``.
   */
   function load_asQueryHits(x: object, env?: object): object;
   module mass_search {
      /**
        * @param tolerance default value Is ``'da:0.01'``.
        * @param env default value Is ``null``.
      */
      function index(massSet: any, type: any, tolerance?: any, env?: object): object;
   }
   /**
     * @param tolerance default value Is ``'ppm:20'``.
     * @param env default value Is ``null``.
   */
   function ms1_handler(compounds: any, precursors: any, tolerance?: any, env?: object): any;
   /**
     * @param unique default value Is ``false``.
     * @param uniqueByScore default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function ms1_search(engine: any, mz: any, unique?: boolean, uniqueByScore?: boolean, env?: object): object;
   /**
     * @param keepsRaw default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function parseLipidName(name: any, keepsRaw?: boolean, env?: object): any;
   /**
   */
   function precursorIon(ion: string): object;
   /**
   */
   function queryByMass(search: object, mass: number): any;
   /**
     * @param mzdiff default value Is ``'da:0.005'``.
     * @param env default value Is ``null``.
   */
   function searchMz(mz: any, exactMass: number, adducts: object, mzdiff?: any, env?: object): object;
   /**
     * @param uniqueByScore default value Is ``false``.
     * @param scoreFactors default value Is ``null``.
     * @param format default value Is ``'F4'``.
     * @param removesZERO default value Is ``false``.
     * @param verbose default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function uniqueFeatures(query: object, uniqueByScore?: boolean, scoreFactors?: object, format?: string, removesZERO?: boolean, verbose?: boolean, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function verify_cas_number(num: any, env?: object): any;
}
