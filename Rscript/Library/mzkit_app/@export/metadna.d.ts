// export R# package module type define for javascript/typescript language
//
// ref=mzkit.metaDNAInfer@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Metabolic Reaction Network-based Recursive Metabolite Annotation for Untargeted Metabolomics
 * 
*/
declare namespace metadna {
   module read {
      module metadna {
         /**
          * Load network graph model from the kegg metaDNA infer network data.
          * 
          * 
           * @param debugOutput -
           * @param env -
           * 
           * + default value Is ``null``.
         */
         function infer(debugOutput:any, env?:object): object;
      }
   }
   module reaction_class {
      /**
       * load kegg reaction class data in table format from given file
       * 
       * 
        * @param file csv table file or a directory with raw xml model data file in it.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function table(file:string, env?:object): object;
   }
   /**
     * @param ms1ppm default value Is ``'ppm:20'``.
     * @param mzwidth default value Is ``'da:0.3'``.
     * @param dotcutoff default value Is ``0.5``.
     * @param allowMs1 default value Is ``true``.
     * @param maxIterations default value Is ``1000``.
     * @param env default value Is ``null``.
   */
   function metadna(ms1ppm?:any, mzwidth?:any, dotcutoff?:number, allowMs1?:boolean, maxIterations?:object, env?:object): object;
   /**
     * @param env default value Is ``null``.
   */
   function range(metadna:object, precursorTypes:any, env?:object): object;
   module load {
      /**
       * 
       * 
        * @param metadna -
        * @param kegg a collection of the kegg compound data.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function kegg(metadna:object, kegg:any, env?:object): object;
      /**
       * 
       * 
        * @param metadna -
        * @param links a collection of the reaction class data
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function kegg_network(metadna:object, links:any, env?:object): object;
      /**
       * 
       * 
        * @param metadna -
        * @param sample a collection of the mzkit peak ms2 data objects
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function raw(metadna:object, sample:any, env?:object): object;
   }
   module DIA {
      /**
        * @param seeds default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function infer(metaDNA:object, sample:any, seeds?:any, env?:object): object;
   }
   module as {
      /**
       * create seeds from mgf file data
       * 
       * 
        * @param seeds -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function seeds(seeds:any, env?:object): object;
      /**
        * @param unique default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function table(metaDNA:object, result:any, unique?:boolean, env?:object): object;
      /**
        * @param env default value Is ``null``.
      */
      function graph(result:any, env?:object): object;
      /**
      */
      function ticks(metaDNA:object): object;
   }
   module result {
      /**
       * get result alignments raw data for data plots.
       * 
       * 
        * @param DIAinfer -
        * @param table -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function alignment(DIAinfer:any, table:any, env?:object): object;
   }
   /**
    * create the kegg compound ms1 annotation query engine.
    * 
    * 
     * @param kegg a set of kegg compound data
     * @param precursors a character vector of the ms1 precursor ion names or 
     *  a list of the given mzcalculator object models.
     * 
     * + default value Is ``'[M]+|[M+H]+|[M+H-H2O]+'``.
     * @param mzdiff the mass tolerance value to match between the 
     *  experiment m/z value and the reference m/z value
     *  which is calculated from the compound exact mass
     *  with a given specific ion precursor type.
     * 
     * + default value Is ``'ppm:20'``.
     * @param excludes 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a data query engine model to run ms1 data search 
     *  for the kegg metaolite compounds.
   */
   function annotationSet(kegg:any, precursors?:any, mzdiff?:any, excludes?:any, env?:object): object;
   module kegg {
      /**
       * load kegg compounds
       * 
       * 
        * @param repo the file path to the messagepack data repository
      */
      function library(repo:string): object;
      /**
       * load the kegg reaction class data.
       * 
       * 
        * @param repo -
      */
      function network(repo:string): object;
   }
}
