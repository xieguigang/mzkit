// export R# package module type define for javascript/typescript language
//
//    imports "GCMS" from "mzkit";
//
// ref=mzkit.GCMSData@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * GC-MS rawdata handler package module
 * 
*/
declare namespace GCMS {
   /**
    * Create gc-ms features data object
    * 
    * 
     * @param raw -
     * @param peaks -
   */
   function gcms_features(raw: object, peaks: object): object;
}
