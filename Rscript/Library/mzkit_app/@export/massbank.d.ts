// export R# package module type define for javascript/typescript language
//
// ref=mzkit.Massbank@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Metabolite annotation database toolkit
 * 
*/
declare namespace massbank {
   module as {
      /**
       * populate lipidmaps meta data objects from the loaded sdf data stream
       * 
       * 
        * @param sdf a sequence of sdf molecular data which can be read from the ``read.SDF`` function.
        * @param asList 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function lipidmaps(sdf:any, asList?:boolean, env?:object): any;
   }
   module chebi {
      module secondary2main {
         /**
         */
         function mapping(repository:string): object;
      }
   }
   module glycosyl {
      /**
        * @param rules default value Is ``null``.
      */
      function solver(rules?:object): object;
      /**
        * @param rules default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function tokens(glycosyl:string, rules?:object, env?:object): string;
   }
   module hmdb {
      module secondary2main {
         /**
           * @param env default value Is ``null``.
         */
         function mapping(repository:any, env?:object): object;
      }
   }
   module lipid {
      /**
      */
      function nameMaps(lipidmaps:object): object;
   }
   /**
   */
   function lipid_classprofiles(lipid_class:object): object;
   /**
   */
   function lipid_profiles(categry:object, enrich:object): any;
   /**
   */
   function parseChEBIEntity(xml:string): object;
   module read {
      /**
       * read lipidmaps messagepack repository file
       * 
       * 
        * @param file -
        * @param gsea_background and also cast the lipidmaps metabolite metadata to the gsea background model?
        * 
        * + default value Is ``false``.
        * @param category_model 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function lipidmaps(file:any, gsea_background?:boolean, category_model?:boolean, env?:object): object|object|object;
      /**
       * read MoNA database file.
       * 
       * 
        * @param rawfile the database reader is switched automatically 
        *  based on this file path its extension name.
        * @param skipSpectraInfo 
        * + default value Is ``false``.
        * @param is_gcms 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a linq pipeline for populate the spectrum data 
        *  from the MoNA database.
      */
      function MoNA(rawfile:string, skipSpectraInfo?:boolean, is_gcms?:boolean, env?:object): object;
      /**
       * read metabolite data in a given sdf data file.
       * 
       * 
        * @param file -
        * @param parseStruct 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function SDF(file:string, parseStruct?:boolean, env?:object): object;
   }
   module save {
      /**
       * Save id mapping file in json file format
       * 
       * 
        * @param mapping -
        * @param file ``*.json`` file name.
        * @param envir -
        * 
        * + default value Is ``null``.
      */
      function mapping(mapping:object, file:string, envir?:object): any;
   }
   module secondary2main {
      /**
       * Create SecondaryIDSolver object from mapping file or mapping dictionary object data.
       * 
       * 
        * @param mapping -
        * @param envir 
        * + default value Is ``null``.
      */
      function mapping(mapping:any, envir?:object): object;
   }
   module write {
      /**
       * save lipidmaps data repository.
       * 
       * 
        * @param lipidmaps -
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function lipidmaps(lipidmaps:any, file:any, env?:object): any;
   }
}
