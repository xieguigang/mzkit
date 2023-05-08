// export R# package module type define for javascript/typescript language
//
// ref=mzkit.flexImaging@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace flexImaging {
   module read {
      /**
      */
      function metadata(mcf:string): any;
   }
   /**
     * @param scale default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function importSpotList(spots:string, spectrum:string, scale?:boolean, env?:object): object;
   /**
     * @param env default value Is ``null``.
   */
   function importsExperiment(scans:object, env?:object): object;
}
