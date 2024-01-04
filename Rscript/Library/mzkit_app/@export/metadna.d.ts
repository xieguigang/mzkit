// export R# package module type define for javascript/typescript language
//
//    imports "metadna" from "mzDIA";
//
// ref=mzkit.metaDNAInfer@mzDIA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace metadna {
   /**
     * @param precursors default value Is ``'[M]+|[M+H]+|[M+H-H2O]+'``.
     * @param mzdiff default value Is ``'ppm:20'``.
     * @param excludes default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function annotationSet(kegg: any, precursors?: any, mzdiff?: any, excludes?: any, env?: object): object;
   module as {
      /**
        * @param env default value Is ``null``.
      */
      function graph(result: any, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function seeds(seeds: any, env?: object): object;
      /**
        * @param unique default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function table(metaDNA: object, result: any, unique?: boolean, env?: object): object;
      /**
      */
      function ticks(metaDNA: object): object;
   }
   module DIA {
      /**
        * @param seeds default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function infer(metaDNA: object, sample: any, seeds?: any, env?: object): object;
   }
   module kegg {
      /**
      */
      function library(repo: string): object;
      /**
      */
      function network(repo: string): object;
   }
   module load {
      /**
        * @param env default value Is ``null``.
      */
      function kegg(metadna: object, kegg: any, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function kegg_network(metadna: object, links: any, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function raw(metadna: object, sample: any, env?: object): object;
   }
   /**
     * @param ms1ppm default value Is ``'ppm:20'``.
     * @param mzwidth default value Is ``'da:0.3'``.
     * @param dotcutoff default value Is ``0.5``.
     * @param allowMs1 default value Is ``true``.
     * @param maxIterations default value Is ``1000``.
     * @param env default value Is ``null``.
   */
   function metadna(ms1ppm?: any, mzwidth?: any, dotcutoff?: number, allowMs1?: boolean, maxIterations?: object, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function range(metadna: object, precursorTypes: any, env?: object): object;
   module reaction_class {
      /**
        * @param env default value Is ``null``.
      */
      function table(file: string, env?: object): object;
   }
   module read {
      module metadna {
         /**
           * @param env default value Is ``null``.
         */
         function infer(debugOutput: any, env?: object): object;
      }
   }
   module result {
      /**
        * @param env default value Is ``null``.
      */
      function alignment(DIAinfer: any, table: any, env?: object): object;
   }
}
