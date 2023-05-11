// export R# package module type define for javascript/typescript language
//
// ref=mzkit.HMDBTools@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * toolkit for handling of the hmdb database
 * 
*/
declare namespace hmdb_kit {
   /**
    * split the hmdb database by biospecimen locations
    * 
    * 
     * @param hmdb -
     * @param locations -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function biospecimen_slicer(hmdb: object, locations: object, env?: object): any;
   /**
   */
   function chemical_taxonomy(metabolite: object): string;
   module export {
      /**
       * save the hmdb database as a csv table file
       * 
       * 
        * @param hmdb -
        * @param file this function will returns a huge metabolite table
        *  if this parameter value default null
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function hmdb_table(hmdb: object, file?: any, env?: object): any;
   }
   /**
    * get metabolite via a given hmdb id from the hmdb.ca online web services
    * 
    * 
     * @param id the given hmdb id
     * @param cache_dir 
     * + default value Is ``'./hmdb/'``.
     * @param tabular 
     * + default value Is ``false``.
     * @param env 
     * + default value Is ``null``.
   */
   function get_hmdb(id: string, cache_dir?: string, tabular?: boolean, env?: object): object|object;
   module read {
      /**
       * open a reader for read hmdb database
       * 
       * 
        * @param xml the file path of the hmdb metabolite database xml file
        * @return this function populate a collection of the hmdb metabolites data
      */
      function hmdb(xml: string): object;
      /**
       * read hmdb spectral data collection
       * 
       * 
        * @param repo A directory path to the hmdb spectral data files
        * @param hmdbRaw -
        * 
        * + default value Is ``false``.
        * @param lazy -
        * 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function hmdb_spectrals(repo: string, hmdbRaw?: boolean, lazy?: boolean, env?: object): any;
   }
}
