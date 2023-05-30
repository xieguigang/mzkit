// export R# package module type define for javascript/typescript language
//
//    imports "flexImaging" from "mzkit";
//
// ref=mzkit.flexImaging@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace flexImaging {
   /**
     * @param env default value Is ``null``.
   */
   function importsExperiment(scans: object, env?: object): object;
   /**
     * @param scale default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function importSpotList(spots: string, spectrum: string, scale?: boolean, env?: object): object;
   module read {
      /**
      */
      function metadata(mcf: string): any;
   }
}
