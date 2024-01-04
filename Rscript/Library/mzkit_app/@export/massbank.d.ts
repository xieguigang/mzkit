// export R# package module type define for javascript/typescript language
//
//    imports "massbank" from "mzkit";
//
// ref=mzkit.Massbank@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace massbank {
   module as {
      /**
        * @param asList default value Is ``false``.
        * @param lazy default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function lipidmaps(sdf: any, asList?: boolean, lazy?: boolean, env?: object): any;
   }
   module chebi {
      module secondary2main {
         /**
         */
         function mapping(repository: string): object;
      }
   }
   /**
   */
   function extract_chebi_compounds(chebi: object): object;
   module glycosyl {
      /**
        * @param rules default value Is ``null``.
      */
      function solver(rules?: object): object;
      /**
        * @param rules default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function tokens(glycosyl: string, rules?: object, env?: object): string;
   }
   module hmdb {
      module secondary2main {
         /**
           * @param env default value Is ``null``.
         */
         function mapping(repository: any, env?: object): object;
      }
   }
   /**
     * @param env default value Is ``null``.
   */
   function inchikey(inchi: any, env?: object): any;
   module lipid {
      /**
        * @param id default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function class(lipidmaps: any, id?: any, env?: object): object|object;
      /**
      */
      function nameMaps(lipidmaps: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function names(lipidmaps: any, env?: object): object;
   }
   /**
   */
   function lipid_classprofiles(lipid_class: object): object;
   /**
   */
   function lipid_profiles(categry: object, enrich: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function lipidmaps_id(lipidmaps: any, env?: object): any;
   /**
     * @param iupac_name default value Is ``null``.
     * @param xref default value Is ``null``.
     * @param synonym default value Is ``null``.
     * @param desc default value Is ``null``.
   */
   function metabo_anno(id: string, formula: string, name: string, iupac_name?: string, xref?: object, synonym?: any, desc?: any): object;
   module mona {
      /**
      */
      function msp_metadata(msp: object): any;
   }
   module parse {
      /**
      */
      function ChEBI_entity(xml: string): object;
   }
   /**
     * @param max_len default value Is ``32``.
     * @param min_len default value Is ``5``.
   */
   function rankingNames(x: any, max_len?: object, min_len?: object): any;
   module read {
      /**
        * @param gsea_background default value Is ``false``.
        * @param category_model default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function lipidmaps(file: any, gsea_background?: boolean, category_model?: boolean, env?: object): object|object|object;
      /**
        * @param skipSpectraInfo default value Is ``false``.
        * @param is_gcms default value Is ``false``.
        * @param verbose default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function MoNA(rawfile: string, skipSpectraInfo?: boolean, is_gcms?: boolean, verbose?: boolean, env?: object): object;
      /**
        * @param parseStruct default value Is ``true``.
        * @param lazy default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function SDF(file: string, parseStruct?: boolean, lazy?: boolean, env?: object): object;
   }
   module save {
      /**
        * @param envir default value Is ``null``.
      */
      function mapping(mapping: object, file: string, envir?: object): any;
   }
   module secondary2main {
      /**
        * @param envir default value Is ``null``.
      */
      function mapping(mapping: any, envir?: object): object;
   }
   module write {
      /**
        * @param env default value Is ``null``.
      */
      function lipidmaps(lipidmaps: any, file: any, env?: object): any;
   }
}
