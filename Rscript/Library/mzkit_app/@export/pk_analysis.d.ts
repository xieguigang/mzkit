// export R# package module type define for javascript/typescript language
//
//    imports "pk_analysis" from "mz_quantify";
//
// ref=mzkit.PKAnalysis@mz_quantify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace pk_analysis {
   /**
     * @param env default value Is ``null``.
   */
   function analysis(x: any, dose: number, route: string, env?: object): any;
}
